namespace Mango.Web.Services.IService
{
    public interface ITokenProvider
    {
        string? getToken();
        void setToken(string? token);
        void clearToken();
    }
}
