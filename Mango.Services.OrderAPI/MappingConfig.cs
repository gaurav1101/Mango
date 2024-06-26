﻿using AutoMapper;
using Mango.Services.OrderAPI.Models;
using Mango.Services.OrderAPI.Models.Dto;
using System.Collections.Generic;
using AutoMapper.Extensions.ExpressionMapping;
using Mango.Services.OrderAPI.Models.Dtos;
using Microsoft.AspNetCore.Routing.Constraints;

namespace Mango.Services.OrderAPI
{
    public class MappingConfig 
    {
        public static MapperConfiguration RegisterMaps() 
        {
            var mappingConfig = new MapperConfiguration(config =>
                {
                    config.CreateMap<OrderDetailsDto,OrderDetails >().ReverseMap();
                    config.CreateMap<OrderHeaderDto, OrderHeader>()
                    .ForMember(dest => dest.orderDetails, u => u.MapFrom(src => src.orderDetailsdto)).ReverseMap();
                    // config.CreateMap<CartDetails, CartDto>().ReverseMap();
                    config.CreateMap<OrderHeaderDto, CartHeaderDto>()
                         .ForMember(dest => dest.CartTotal, u => u.MapFrom(src => src.CartTotal)).ReverseMap();

                    config.CreateMap<CartDetailsDto, OrderDetailsDto>()
                    .ForPath(dest => dest.ProductName, u => u.MapFrom(src => src.Product.Name))
                    .ForMember(dest => dest.Price, u => u.MapFrom(src => src.Product.Price));
                    config.CreateMap<OrderDetailsDto, CartDetailsDto>();
                    
                }
            ) ;
            return mappingConfig;
        }



    }
}
