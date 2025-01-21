namespace VaccineChildren.Core.Store;

public enum OrderHistoryStatus
{
    [CustomName("Order Received")]
    ORDER_RECEIVE = 1,

    [CustomName("Pending Delivery")]
    PENDING_DELIVERY = 2,

    [CustomName("Delivery In Progress")]
    DELIVERY_IN_PROGRESS = 3,

    [CustomName("Delivered")]
    DELIVERIED = 4,

    [CustomName("Cancelled")]
    CANCELLED = 5,

    [CustomName("Returned")]
    RETURNED = 6,
}
