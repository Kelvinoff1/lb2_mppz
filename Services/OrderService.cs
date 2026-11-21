using StoreDomain.Data;
using StoreDomain.Models;

namespace StoreDomain.Services;

public class OrderService
{
    private readonly StoreRepository _repository;

    public OrderService(StoreRepository repository)
    {
        _repository = repository;
    }

    public Order CreateOrder(int customerId, int employeeId, DateTime date)
    {
        var order = new Order
        {
            Id = _repository.NextOrderId(),
            OrderDate = date,
            CustomerId = customerId,
            EmployeeId = employeeId
        };
        _repository.AddOrder(order);
        return order;
    }

    public OrderItem AddProduct(int orderId, int productId, int quantity)
    {
        var order = _repository.GetOrderById(orderId)
            ?? throw new InvalidOperationException($"Замовлення з Id={orderId} не знайдено.");
        var product = _repository.GetProductById(productId)
            ?? throw new InvalidOperationException($"Товар з Id={productId} не знайдено.");

        if (order.Status != OrderStatus.New)
            throw new InvalidOperationException($"Замовлення #{orderId} вже оплачено, змінювати його не можна.");

        product.DecreaseStock(quantity); // кине помилку, якщо товару недостатньо

        var existing = order.Items.FirstOrDefault(i => i.ProductId == productId);
        if (existing != null)
        {
            existing.Quantity += quantity;
            return existing;
        }

        var item = new OrderItem
        {
            Id = _repository.NextOrderItemId(),
            OrderId = orderId,
            ProductId = productId,
            Quantity = quantity
        };
        _repository.AddOrderItem(item);
        return item;
    }

    public Payment Pay(int orderId, PaymentMethod method, DateTime date)
    {
        var order = _repository.GetOrderById(orderId)
            ?? throw new InvalidOperationException($"Замовлення з Id={orderId} не знайдено.");

        if (order.Items.Count == 0)
            throw new InvalidOperationException($"Замовлення #{orderId} порожнє.");

        var payment = new Payment
        {
            Id = _repository.NextPaymentId(),
            OrderId = orderId,
            Amount = order.Total,
            PaymentDate = date,
            Method = method
        };
        _repository.AddPayment(payment);
        return payment;
    }
}
