namespace CasaDaRosa.Domain.Entities.Orders;

public enum OrderStatus
{
    Pending = 1,
    Confirmed = 2,
    InPreparation = 3,
    OutForDelivery = 4,
    Delivered = 5,
    Cancelled = 6
}
