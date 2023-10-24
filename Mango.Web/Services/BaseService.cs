using Mango.Web.Models;
using Mango.Web.Models.Dto;
using Mango.Web.Services.IService;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Net;
using System.Text;
using Newtonsoft.Json;
using Mango.Web.Utility;

namespace Mango.Web.Services
{
    public class BaseService : IBaseService
    {
        private readonly IHttpClientFactory _httpClientFactory;
		private readonly ITokenProvider _tokenProvider;
		public BaseService(IHttpClientFactory httpClientFactory, ITokenProvider tokenProvider) 
        {
            _httpClientFactory = httpClientFactory;
            _tokenProvider = tokenProvider;
        }

        public async Task<ResponseDto?> sendAsync(RequestDto apirequest, bool withBearer)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("MangoAPI");
                HttpRequestMessage httpRequest = new HttpRequestMessage();
                httpRequest.Headers.Add("Accept", "application/json");
                httpRequest.RequestUri = new Uri(apirequest.url);
                if(withBearer)
                {
                    var token=_tokenProvider.getToken();
					httpRequest.Headers.Add("Authorization", $"Bearer {token}");
				}

                if (apirequest.Data != null)
                {
                    httpRequest.Content = new StringContent(JsonConvert.SerializeObject(apirequest.Data), Encoding.UTF8, "application/json");
                }
                switch (apirequest.ApiType)
                {
                    case SD.ApiTypes.POST:
                        httpRequest.Method = HttpMethod.Post;
                        break;
                    case SD.ApiTypes.PUT:
                        httpRequest.Method = HttpMethod.Put;
                        break;
                    case SD.ApiTypes.DELETE:
                        httpRequest.Method = HttpMethod.Delete;
                        break;
                    default:
                        httpRequest.Method = HttpMethod.Get;
                        break;
                }
                HttpResponseMessage httpResponse = null;
                httpResponse = await client.SendAsync(httpRequest);
                switch (httpResponse.StatusCode)
                {
                    case HttpStatusCode.NotFound:
                        return new ResponseDto()
                        {
                            IsSuccess = false,
                            Result = new CouponDto(),
							Error = httpResponse.StatusCode.ToString()
						}; 
      //              case HttpStatusCode.BadRequest:
      //                  return new ResponseDto()
      //                  {
      //                      IsSuccess = false,
      //                      Result = new CouponDto(),
						//	Error = httpResponse.StatusCode.ToString()
						//};
                    case HttpStatusCode.Unauthorized:
                        return new ResponseDto()
                        {
                            IsSuccess = false,
                            Result = new CouponDto(),
                            Error= httpResponse.StatusCode.ToString()
						};
                        
                    default:
                        var api = await httpResponse.Content.ReadAsStringAsync();
                        ResponseDto responseDto= JsonConvert.DeserializeObject<ResponseDto>(api);
                        if (responseDto != null)
                        {
                            if (!string.IsNullOrEmpty(responseDto.Error))
                            {
                                responseDto.IsSuccess = false;
                                return responseDto;
                            }
                            return responseDto;
                        }
                        else
                        {
                            return new ResponseDto();
                        }
                }
            }
            catch (Exception ex)
            {
                ResponseDto responseDto = new ResponseDto()
                {
                    Error = ex.Message,
                };
                return responseDto;
            }
           
        }
    }

}
