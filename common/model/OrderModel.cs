namespace CafeBisleriumPOS.Common.model;
using CafeBisleriumPOS.Modules.Coffee.model;

public class OrderModel
{
    // Gets or sets the email associated with the coffee order.
    public string Email { get; set; }

    // Gets or sets the count of coffee items in the order.
    public int Count { get; set; }

    // Gets or sets the number of days for which the order is applicable.
    public int Days { get; set; }

    // Gets or sets the total price of the coffee order.
    public decimal TotalPrice { get; set; }

    // Gets or sets the list of CommonModel instances representing coffee data in the order.
    public List<CommonModel> CoffeeData { get; set; }

    // Gets or sets the date when the coffee order was placed.
    public DateTime Date { get; set; }
}