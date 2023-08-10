namespace Mango.Web.Utility
{
    public class SD
    {
        public static string CouponAPIBaseUrl { get; set; }
        public static string AuthAPIBaseUrl { get; set; }
		public static string ProductAPIBaseUrl { get; set; }
		public static string RoleAdmin = "ADMIN";
        public static string RoleCostumer = "COSTUMER";
        public static string token = "jwtToken";

        public enum ApiTypes
        {
            GET,
            POST,
            PUT,
            DELETE
        }
    }
}
