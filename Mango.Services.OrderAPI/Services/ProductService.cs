﻿using Mango.Services.OrderAPI.Models.Dtos;
using Mango.Services.OrderAPI.Services.IServices;
using Mango.Services.OrderAPI.Models.Dto;
using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace Mango.Services.OrderAPI.Services
{
    public class ProductService : IProductService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public ProductService(IHttpClientFactory httpClientFactory) 
        { 
            _httpClientFactory = httpClientFactory;
        }
        public async Task<IEnumerable<ProductDto>> GetProducts()
        {
            var client= _httpClientFactory.CreateClient("Product");
            var response = await client.GetAsync($"/api/ProductAPI");
            var apicontent= await response.Content.ReadAsStringAsync();
            var resp=JsonConvert.DeserializeObject<ResponseDto>(apicontent);
            if (resp!=null && resp.IsSuccess)
            {
                return JsonConvert.DeserializeObject<IEnumerable<ProductDto>>(Convert.ToString(resp.Result));
            }
            return new List<ProductDto>();
        }
    }
}
