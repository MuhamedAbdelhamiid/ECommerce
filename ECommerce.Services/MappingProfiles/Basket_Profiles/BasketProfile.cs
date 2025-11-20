using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ObjectiveC;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using ECommerce.Domain.Entities.Basket_Module;
using ECommerce.Shared.DTOs.BasketDTOs;

namespace ECommerce.Services.MappingProfiles.Basket_Profiles
{
    public class BasketProfile : Profile
    {
        public BasketProfile()
        {
            CreateMap<BasketDTO, CustomerBasket>().ReverseMap();

            CreateMap<BasketItemDTO, BasketItem>().ReverseMap();
        }
    }
}
