namespace Mango.Web.Utility
{
    public class SD
    {
        public static string CouponAPIBaseUrl { get; set; }
        public static string AuthAPIBaseUrl { get; set; }

        public enum ApiTypes
        {
            GET,
            POST,
            PUT,
            DELETE
        }
    }
}
