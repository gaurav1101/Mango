namespace Mango.Web.Utility
{
    public class SD
    {
        public static string CouponAPIBaseUrl { get; set; }
        public static string AuthAPIBaseUrl { get; set; }
		public static string ProductAPIBaseUrl { get; set; }
        public static string ShoppingCartAPIBaseUrl { get; set; }
        public static string OrderAPIBaseUrl { get; set; }
        public static string RoleAdmin = "ADMIN";
        public static string RoleCostumer = "COSTUMER";
        public static string token = "jwtToken";
        public const string Pending = "Pending";
        public const string Approved = "Approved";
        public const string ReadyForPickUp = "ReadyForPickUp";
        public const string Completed = "Completed";
        public const string Refunded = "Refunded";
        public const string Cancelled = "Cancelled";

        public enum ApiTypes
        {
            GET,
            POST,
            PUT,
            DELETE
        }

    }
}
