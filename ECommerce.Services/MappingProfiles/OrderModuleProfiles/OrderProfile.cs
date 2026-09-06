using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using ECommerce.Domain.Entities.OrderModule;
using ECommerce.Shared.DTOs.OrderDTOs;

namespace ECommerce.Services.MappingProfiles.OrderModuleProfiles
{
    public class OrderProfile : Profile
    {
        public OrderProfile()
        {
            CreateMap<ShippingAddressDTO, OrderAddress>().ReverseMap();

            CreateMap<Order, OrderToReturnDTO>()
                .ForMember(
                    dest => dest.DeliveryMethod,
                    opt => opt.MapFrom(src => src.DeliveryMethod.ShortName)
                )
                .ForMember(
                    dest => dest.OrderStatus,
                    opt => opt.MapFrom(src => src.Status.ToString())
                )
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.ShippingAddress));

            CreateMap<OrderItem, OrderItemConfirmationDTO>()
                .ForMember(
                    dest => dest.ProductName,
                    opt => opt.MapFrom(src => src.Product.ProductName)
                )
                .ForMember(
                    dest => dest.PictureUrl,
                    opt => opt.MapFrom(src => src.Product.PictureUrl)
                );

            CreateMap<DeliveryMethod, DeliveryMethodsDTO>();
        }
    }
}
