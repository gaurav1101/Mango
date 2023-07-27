using Mango.Web.Services.IService;
using Mango.Web.Utility;
using Newtonsoft.Json.Linq;

namespace Mango.Web.Services
{
    public class TokenProvider:ITokenProvider
    {
        private IHttpContextAccessor _contextAccessor;
        public TokenProvider(IHttpContextAccessor httpContextAccessor) 
        { 
            _contextAccessor = httpContextAccessor;
        }

        public void clearToken()
        {
            _contextAccessor.HttpContext?.Response.Cookies.Delete(SD.token);
        }

        public string? getToken()
        {
            string? token = null;
            bool? hasTokenValue= _contextAccessor.HttpContext?.Request.Cookies.TryGetValue(SD.token,out token);
            return hasTokenValue is true ? token : null;
        }

        public void setToken(string? token)
        {
            _contextAccessor.HttpContext?.Response.Cookies.Append(SD.token, token);
        }
    }
}
