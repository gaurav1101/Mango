using AutoMapper;
using Mango.Services.OrderAPI.Models;
using Mango.Services.OrderAPI.Models.Dto;


namespace Mango.Services.OrderAPI
{
    public class MappingConfig 
    {
        public static MapperConfiguration RegisterMaps() 
        {
            var mappingConfig = new MapperConfiguration(config =>
                {
                config.CreateMap<OrderDetails, OrderDetailsDto>().ReverseMap();
                config.CreateMap<OrderHeader, OrderHeaderDto>().ReverseMap();
                    // config.CreateMap<CartDetails, CartDto>().ReverseMap();
                config.CreateMap<OrderHeaderDto,CartHeaderDto>()
                     .ForMember(dest => dest.CartTotal, u => u.MapFrom(src => src.OrderTotal)).ReverseMap();
                config.CreateMap<OrderDetailsDto, CartDetailsDto>()
                     .ForMember(dest => dest.Product.Name, u => u.MapFrom(src => src.ProductName))
                     .ForMember(dest => dest.Product.Price, u => u.MapFrom(src => src.Price));


                }
            ) ;
            return mappingConfig;
        }



    }
}
