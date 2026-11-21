using System.Text;
using StoreDomain.Data;
using StoreDomain.Models;
using StoreDomain.Services;

Console.OutputEncoding = Encoding.UTF8;

var repository = StoreRepository.CreateWithSeedData();
var infoService = new StoreInfoService(repository);
var orderService = new OrderService(repository);

infoService.PrintAll();

StoreInfoService.PrintHeader("Оформлення нового замовлення");

var newOrder = orderService.CreateOrder(customerId: 2, employeeId: 1, date: new DateTime(2026, 11, 16));
orderService.AddProduct(newOrder.Id, productId: 1, quantity: 3);   // Раф x3
orderService.AddProduct(newOrder.Id, productId: 3, quantity: 1);   // Салямі x1
orderService.Pay(newOrder.Id, PaymentMethod.Card, new DateTime(2026, 11, 16));

infoService.PrintOrder(newOrder);
Console.WriteLine($"Залишок товару \"Раф\" на складі: {repository.GetProductById(1)!.StockQuantity}");

// Тут я намагаюсь купити більше, ніж є на складі
StoreInfoService.PrintHeader("Перевірка залишків на складі");
try
{
    orderService.AddProduct(orderId: 4, productId: 4, quantity: 100);
}
catch (InvalidOperationException ex)
{
    Console.WriteLine($"Помилка: {ex.Message}");
}
