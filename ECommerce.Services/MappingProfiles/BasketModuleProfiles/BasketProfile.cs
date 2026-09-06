using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using ECommerce.Domain.Entities.BasketModule;
using ECommerce.Shared.DTOs.BasketModuleDTOs;

namespace ECommerce.Services.MappingProfiles.BasketModuleProfiles
{
    public class BasketProfile : Profile
    {
        public BasketProfile()
        {
            CreateMap<BasketItem, BasketItemDTO>().ReverseMap();
            CreateMap<BasketDTO, CustomerBasket>().ReverseMap();
        }
    }
}
