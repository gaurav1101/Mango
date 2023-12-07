namespace Mango.Services.OrderAPI.Utility
{
    public class SD
    {
        public enum Status
        {
            Pending,
            Approved,
            ReadyForPickUp,
            Completed,
            Refunded,
            Cancelled
        }

        public const string role_customer = "CUSTOMER";
        public const string role_admin = "ADMIN";

    }
}
