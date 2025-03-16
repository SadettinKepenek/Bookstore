namespace Order.Domain.Constants;

public static class ErrorMessages
{
    public const string CannotUpdateOrderStatus = "{0} order status cannot be updated to {1} status";
    public const string InvalidStatusData = "Invalid status data provided for status update";
    public const string CancelOrderStatusCannotBeCompleted = "Order cannot be completed due to cancelled status";
    public const string OrderIsAlreadyCompleted = "Order is already completed";
    public const string OrderIsAlreadyCancelled = "Order is already cancelled";
    public const string OrderQuantityInvalid = "Order quantity cannot be lower than or equal to zero";
    public const string OrderUnitPriceIsInvalid = "Order unit price cannot be lower than or equal to zero ";
}