using System.ComponentModel;

namespace OrdersApi.Data.Domain
{
    public enum OrderStatus
    {
        [Description("Created")]
        Created,

        [Description("Pending")]
        Pending,

        [Description("Cancelled")]
        Cancelled,

        [Description("Paid")]
        Paid,

        [Description("AwaitingPayment")]
        AwaitingPayment,

        [Description("ReadyForShipping")]
        ReadyForShipping,

        [Description("Shipped")]
        Shipped,

        [Description("Delivered")]
        Delivered,

        [Description("Completed")]
        Completed

    }
}
