namespace Mango.Web.Utility;

public class SD
{
    public static string CouponAPIBase { get; set; } = null!;
    public static string AuthAPIBase { get; set; } = null!;
    public static string ProductAPIBase { get; set; } = null!;
    public static string ShoppingCartAPIBase { get; set; } = null!;
    public static string OrderAPIBase { get; set; } = null!;

    public const string RoleAdmin = "ADMIN";
    public const string RoleCustomer = "CUSTOMER";

    public const string TokenCookie = "JWTToken";

    public enum ApiType
    {
        GET,
        POST,
        PUT,
        DELETE
    }

    public const string StatusPending = "Pending";
    public const string StatusApproved = "Approved";
    public const string StatusReadyForPickup = "ReadyForPickup";
    public const string StatusCompleted = "Completed";
    public const string StatusRefunded = "Refunded";
    public const string StatusCancelled = "Cancelled";

    public enum ContentType
    {
        Json,
        MultipartFormData
    }
}
