using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using AutoMapper;
using ECommerce.Domain.Contracts;
using ECommerce.Domain.Entities.OrderModule;
using ECommerce.Services.Abstraction;
using ECommerce.Services.Specifications;
using ECommerce.Shared.CommonResponses;
using ECommerce.Shared.DTOs.BasketModuleDTOs;
using Microsoft.Extensions.Configuration;
using Stripe;
using Stripe.Forwarding;
using Product = ECommerce.Domain.Entities.ProductModule.Product;

namespace ECommerce.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;
        private readonly IBasketRepository _basketRepository;
        private readonly IMapper _mapper;

        public PaymentService(
            IUnitOfWork unitOfWork,
            IConfiguration configuration,
            IBasketRepository basketRepository,
            IMapper mapper
        )
        {
            _unitOfWork = unitOfWork;
            _configuration = configuration;
            _basketRepository = basketRepository;
            _mapper = mapper;
        }

        public async Task<Result<BasketDTO>> ProcessPaymentAsync(string basketId)
        {
            // get the secret key from the configuration file and use it
            // if the secret key is not null then fill the stripe confifuration api
            // key with the secret key to be authenticated to stripe server
            var secretKey = _configuration["Stripe:SecretKey"];

            if (string.IsNullOrEmpty(secretKey))
                return Result<BasketDTO>.Fail(
                    Error.Faliure(description: "Failed To Obtain Stripe Secret Key")
                );

            StripeConfiguration.ApiKey = secretKey;

            // retrive the basket by its id and validate that it exists
            var userBasket = await _basketRepository.GetBasketAsync(basketId);

            if (userBasket is null)
                return Result<BasketDTO>.Fail(Error.NotFound(description: "Basket Not Found"));

            // validate the delivery method and make sure that its available (from the stored ones)

            if (userBasket.DeliveryMethodId is null)
                return Result<BasketDTO>.Fail(
                    Error.NotFound(description: "Delivery Method Not Found")
                );

            var deliveryMethods = await _unitOfWork
                .GetRepository<DeliveryMethod, int>()
                .GetAllAsync();

            var deliveryMethodDoesntExist = deliveryMethods.All(dm =>
                dm.Id != userBasket.DeliveryMethodId
            );

            if (deliveryMethodDoesntExist)
                return Result<BasketDTO>.Fail(
                    Error.NotFound(description: "Delivery Method Not Found Or Deprecated")
                );

            var selectedDeliveryMethod = deliveryMethods.First(dm =>
                dm.Id == userBasket.DeliveryMethodId
            );

            // fill the shipping price based on the delivery method
            userBasket.ShippingPrice = selectedDeliveryMethod.Price;

            /* validate the basket items and make sure that they are available and their prices are correct
             * (if one product doesn't exist then return error not found)
             * after validating the basket items, update the basket items with the correct prices and names and picture urls
             * after all of this calculate the items total by sum the prices * quantities of the basket items and return the total amount to be paid
             * the stripe wants the total amount to be paid in cents so we need to multiply the total amount by 100 and round it to the nearest integer
            */

            var selectedProductsIds = userBasket.Items?.Select(i => i.Id).ToList() ?? [];

            if (selectedProductsIds.Count == 0)
                return Result<BasketDTO>.Fail(Error.Validation(description: "Basket Is Empty"));

            var productsSpec = new ProductWithSpecifications(selectedProductsIds);
            var products = await _unitOfWork
                .GetRepository<Product, int>()
                .GetAllAsync(productsSpec);

            foreach (var item in userBasket.Items!)
            {
                var selectedItem = products.FirstOrDefault(p => p.Id == item.Id);
                if (selectedItem is null)
                    return Result<BasketDTO>.Fail(
                        Error.NotFound(description: $"Product With Id {item.Id} Not Found")
                    );

                item.Price = selectedItem.Price;
                item.ProductName = selectedItem.Name;
                item.PictureUrl = selectedItem.PictureUrl;
            }

            var amount = (long)userBasket.Items!.Sum(i => i.Price * i.Quantity) * 100;

            // create or update payment intent
            // if basket has payment intent id then update the payment intent with the new amount
            /*
                    // Integration with any external service
                    // download the stripe nuget package
                    // collection of classes that will help us to integrate with stripe
                    // Main class to interact with stripe [create object from it] [PaymentIntentService =>
                    this takes an PaymentIntentCreateOptions object with is we declate the amount and the type and currency]
                        
                    // use service inside the main object [Call Function]
                 */
            var stripeService = new PaymentIntentService();

            if (userBasket.PaymentIntentId is not null)
            {
                /*
                    // we here update the amount using an object of PaymentIntentUpdateOptions and fill it with the new amount
                    and then call the update function of the PaymentIntentService class
                 */
                var stripeOptions = new PaymentIntentUpdateOptions { Amount = amount };
                await stripeService.UpdateAsync(userBasket.PaymentIntentId, stripeOptions);
            }
            else
            {
                // else => create a new payment intent with the total amount and get the client secret and fill the basket dto with new info

                var stripeOptions = new PaymentIntentCreateOptions
                {
                    Amount = amount,
                    Currency = "usd",
                    PaymentMethodTypes = new List<string> { "card" },
                };

                var stripePaymentIntent = await stripeService.CreateAsync(stripeOptions);
                userBasket.PaymentIntentId = stripePaymentIntent.Id;
                userBasket.ClientSecret = stripePaymentIntent.ClientSecret;
            }

            // update the basket
            await _basketRepository.CreateOrUpdateBasketAsync(userBasket);

            var basketToReturn = _mapper.Map<BasketDTO>(userBasket);

            // return the basket
            return Result<BasketDTO>.Ok(basketToReturn);
        }

        public async Task UpdateOrderPaymentStatus(string request, string stripeSignature)
        {
            var endpointSecret = _configuration["Stripe:EndpointSecret"];
            var stripeEvent = EventUtility.ConstructEvent(request, stripeSignature, endpointSecret);

            using var doc = JsonDocument.Parse(request);
            var paymentIntentId = doc
                .RootElement.GetProperty("data")
                .GetProperty("object")
                .GetProperty("id")
                .GetString();

            if (string.IsNullOrEmpty(paymentIntentId))
            {
                Console.WriteLine("Could not extract payment intent id from event");
                return;
            }

            var orderSpec = new OrderSpecifications(paymentIntentId, "");
            var order = await _unitOfWork.GetRepository<Order, Guid>().GetByIdAsync(orderSpec);

            if (order is null)
            {
                Console.WriteLine("Order not found");
                return;
            }

            if (stripeEvent.Type == EventTypes.PaymentIntentSucceeded)
            {
                order.Status = OrderStatus.PaymentReceived;
                _unitOfWork.GetRepository<Order, Guid>().Update(order);
                await _unitOfWork.SaveChangesAsync();
            }
            else if (stripeEvent.Type == EventTypes.PaymentIntentPaymentFailed)
            {
                order.Status = OrderStatus.PaymentFailed;
                _unitOfWork.GetRepository<Order, Guid>().Update(order);
                await _unitOfWork.SaveChangesAsync();
            }
            else
            {
                Console.WriteLine("Unhandled event type: {0}", stripeEvent.Type);
            }
        }
    }
}
