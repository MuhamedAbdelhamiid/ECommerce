using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using ECommerce.Domain.Contracts;
using ECommerce.Domain.Entities.BasketModule;
using ECommerce.Domain.Entities.OrderModule;
using ECommerce.Domain.Entities.ProductModule;
using ECommerce.Services.Abstraction;
using ECommerce.Services.Specifications;
using ECommerce.Shared.CommonResponses;
using ECommerce.Shared.DTOs.OrderDTOs;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Services
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IBasketRepository _basketRepository;

        public OrderService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IBasketRepository basketRepository
        )
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _basketRepository = basketRepository;
        }

        public async Task<Result<OrderToReturnDTO>> CreateOrderAsync(
            OrderToCreateDTO orderToCreate,
            string email
        )
        {
            // Retrives the basket and validates its existence
            var userBasket = await _basketRepository.GetBasketAsync(orderToCreate.BasketId);

            if (userBasket is null)
                return Result<OrderToReturnDTO>.Fail(
                    Error.NotFound(
                        code: "Basket.NotFound",
                        description: $"Basket With Id: {orderToCreate.BasketId} Not Found"
                    )
                );
            // make sure that the order payment intent is valid and not already processed and if processed and exists remove the old one
            if (userBasket.PaymentIntentId is null)
                return Result<OrderToReturnDTO>.Fail(
                    Error.Validation(
                        code: "PaymentIntentId.NotProvided",
                        description: "PaymentIntentId Was Not Provided"
                    )
                );

            var orderRepo = _unitOfWork.GetRepository<Order, Guid>();
            var orderSpec = new OrderSpecifications(userBasket.PaymentIntentId);
            var orderWithSamePaymentIntent = await orderRepo.GetByIdAsync(orderSpec);

            if (orderWithSamePaymentIntent is not null)
                orderRepo.Delete(orderWithSamePaymentIntent);
            // Map provided shipping address to order address entity(needs a create map)
            var orderAddressToStore = _mapper.Map<OrderAddress>(orderToCreate.ShipToAddress);

            if (userBasket is null)
                return Result<OrderToReturnDTO>.Fail(
                    Error.NotFound(code: "Basket.NotFound", description: "Basket Not Found")
                );
            if (!userBasket.Items.Any())
                return Result<OrderToReturnDTO>.Fail(
                    Error.NotFound(
                        code: "BasketItems.NotFound",
                        description: $"Basket With Id: {userBasket.Id} His Items Was Not Found"
                    )
                );

            // create list of order items by fetching product details from the database
            // and validating each product existence and price

            var orderItemsToStore = new List<OrderItem>();

            var productRepo = _unitOfWork.GetRepository<Product, int>();
            foreach (var item in userBasket.Items)
            {
                var product = await productRepo.GetByIdAsync(item.Id);

                if (product is null)
                    return Result<OrderToReturnDTO>.Fail(
                        Error.NotFound(
                            code: "Product.NotFound",
                            description: $"Product With Id: {item.Id} Not Found"
                        )
                    );

                orderItemsToStore.Add(CreateOrderItem(item, product));
            }
            // Retrives the selected delivery method and validate its existence
            var deliveryMethod = await _unitOfWork
                .GetRepository<DeliveryMethod, int>()
                .GetByIdAsync(orderToCreate.DeliveryMethodId);

            if (deliveryMethod is null)
                return Result<OrderToReturnDTO>.Fail(
                    Error.NotFound(
                        code: "DeliveryMethod.NotFound",
                        description: $"DeliveryMethodId: {orderToCreate.DeliveryMethodId} Not Found"
                    )
                );

            // Calculates the sub total of the order based on the items and thier quantities
            var subTotal = orderItemsToStore.Sum(orderItem => orderItem.Price * orderItem.Quantity);
            var total = subTotal + deliveryMethod.Price;
            // create new order with all the revelant details
            var orderToInsert = new Order()
            {
                DeliveryMethod = deliveryMethod,
                UserEmail = email,
                Items = orderItemsToStore,
                ShippingAddress = orderAddressToStore,
                SubTotal = subTotal,
                PaymentIntentId = userBasket.PaymentIntentId,
                OrderDate = DateTimeOffset.UtcNow,
                Status = OrderStatus.Pending,
            };

            await _unitOfWork.GetRepository<Order, Guid>().AddAsync(orderToInsert);
            bool result = await _unitOfWork.SaveChangesAsync() > 0;
            // return an order confirmationDTO
            if (!result)
                return Result<OrderToReturnDTO>.Fail(
                    Error.Faliure(
                        code: "CreateOrder.Failure",
                        description: "Error Occured While Creating Order"
                    )
                );

            var orderToReturn = _mapper.Map<OrderToReturnDTO>(orderToInsert);

            return Result<OrderToReturnDTO>.Ok(orderToReturn);
        }

        public async Task<Result<ICollection<DeliveryMethodsDTO>>> GetDeliveryMethodsAsync()
        {
            var deliveryMethods = await _unitOfWork
                .GetRepository<DeliveryMethod, int>()
                .GetAllAsync();

            if (deliveryMethods is not null)
            {
                var deliveryMethodsToReturn = _mapper.Map<ICollection<DeliveryMethodsDTO>>(
                    deliveryMethods
                );
                return Result<ICollection<DeliveryMethodsDTO>>.Ok(deliveryMethodsToReturn);
            }

            return Result<ICollection<DeliveryMethodsDTO>>.Fail(
                Error.NotFound(
                    code: "DeliveryMehtodsNotFound",
                    description: "Delivery Methods Data Was Not Found"
                )
            );
        }

        public async Task<Result<ICollection<OrderToReturnDTO>>> GetAllOrdersAsync(string userEmail)
        {
            var ordersSpec = new OrderSpecifications(userEmail);
            var orderRepo = _unitOfWork.GetRepository<Order, Guid>();

            var orders = await orderRepo.GetAllAsync(ordersSpec);

            if (orders is not null && orders.Any())
            {
                var ordersToReturn = _mapper.Map<ICollection<OrderToReturnDTO>>(orders);
                return Result<ICollection<OrderToReturnDTO>>.Ok(ordersToReturn);
            }

            return Result<ICollection<OrderToReturnDTO>>.Fail(
                Error.NotFound(
                    code: "Orders.NotFound",
                    description: "Orders For This User Was Not Found"
                )
            );
        }

        public async Task<Result<OrderToReturnDTO>> GetOrderByIdAsync(
            Guid orderId,
            string userEmail
        )
        {
            var orderRepo = _unitOfWork.GetRepository<Order, Guid>();
            var orderSpec = new OrderSpecifications(orderId, userEmail);

            var order = await orderRepo.GetByIdAsync(orderSpec);
            if (order is not null)
            {
                var orderToReturn = _mapper.Map<OrderToReturnDTO>(order);
                return Result<OrderToReturnDTO>.Ok(orderToReturn);
            }
            return Result<OrderToReturnDTO>.Fail(
                Error.NotFound(
                    code: "Order.NotFound",
                    description: $"Order With Id: {orderId} Was Not Found"
                )
            );
        }

        #region Helper Methods
        private static OrderItem CreateOrderItem(BasketItem item, Product product)
        {
            // validate price
            if (product.Price != item.Price)
                item.Price = product.Price;

            var orderItemToInsert = new OrderItem()
            {
                Price = product.Price,
                Quantity = item.Quantity,
                Product = new ProductItemOrdered()
                {
                    ProductId = product.Id,
                    PictureUrl = product.PictureUrl,
                    ProductName = product.Name,
                },
            };

            return orderItemToInsert;
        }

        #endregion
    }
}
