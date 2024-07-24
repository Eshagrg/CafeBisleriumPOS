
using System.Diagnostics;
using System.Text.Json;
using CafeBisleriumPOS.common.helperClass;
using CafeBisleriumPOS.Common.model;
using CafeBisleriumPOS.Modules.Coffee.model;

namespace CafeBisleriumPOS.Modules.Staff.service;

public class StaffService
{
    // Private member to keep track of the order count
    int orderCount = 0;
    // Method to take an order and store it in the order data file
    public async Task<CustomType> TakeOrder(List<CommonModel> coffeeData, string Email, decimal totalPrice, bool free)
    {
        try
        {
            // Constructing the path for the order data JSON file
            var path = new FileManagement().DirectoryPath("database", "orderData.json");
            Trace.WriteLine("This is Path: " + path);
            Directory.CreateDirectory(Path.GetDirectoryName(path));

            List<OrderModel> existingOrderData = new List<OrderModel>();
            // Checking if the order data file exists
            if (File.Exists(path))
            {
                var existingJson = await File.ReadAllTextAsync(path);
                existingOrderData = JsonSerializer.Deserialize<List<OrderModel>>(existingJson) ?? new List<OrderModel>();
            }
            // Checking for existing orders from the same user
            for (int i = 0; i < existingOrderData.Count; i++)
            {
                if (existingOrderData[i].Email == Email)
                {
                    orderCount = existingOrderData[i].Count;
                }
            }
            // Creating a new order model
            var newOrderData = new OrderModel
            {
                Email = Email,
                TotalPrice = totalPrice,
                CoffeeData = coffeeData,
                Date = DateTime.Now,
                Count = orderCount + 1
            };
            // Resetting count to 1 for free orders
            if (free)
            {
                newOrderData.Count = 1;
            }
            existingOrderData.Add(newOrderData);

            var jsonData = JsonSerializer.Serialize(existingOrderData);
            await File.WriteAllTextAsync(path, jsonData);
            Trace.WriteLine(path);

            return new CustomType { Success = true, Message = "Order placed successfully" };
        }
        catch (Exception ex)
        {
            Trace.WriteLine("Exception: {0}", ex.Message);
            return new CustomType { Success = false, Message = ex.Message };
        }
    }
    // List to store ordered coffee items
    List<CommonModel> coffeeList = new List<CommonModel>();
    // Method to add a coffee item to the ordered list
    public void setOrderedList(CommonModel coffeeData)
    {
        Trace.WriteLine("This is CoffeeData in Staff: " + coffeeData);
        this.coffeeList.Add(coffeeData);

    }
    // Method to get the list of ordered coffee items
    public Task<List<CommonModel>> getOrderedCoffee()
    {
        return Task.FromResult(this.coffeeList);
    }
    // Method to remove a coffee item from the ordered list
    public async Task<List<CommonModel>> removeOrderList(int index)
    {
        var itemToRemove = this.coffeeList.FirstOrDefault(c => c.index == index);
        this.coffeeList.Remove(itemToRemove);
        return this.coffeeList;
    }
    // Method to clear the entire ordered list
    public async Task<CustomType> clearOrderList()
    {
        this.coffeeList.Clear();
        return new CustomType { Success = true, Message = "Success" };
    }
    // Method to check if a user is registered
    public async Task<CustomType> isUserRegistered(string email)
    {
        var path = new FileManagement().DirectoryPath("database", "user.json");
        List<UserModel> userList = new List<UserModel>();
        Directory.CreateDirectory(Path.GetDirectoryName(path));
        if (File.Exists(path))
        {
            var existingData = await File.ReadAllTextAsync(path);
            try
            {
                userList = JsonSerializer.Deserialize<List<UserModel>>(existingData) ?? new List<UserModel>();
                for (int i = 0; i < userList.Count; i++)
                {
                    Trace.WriteLine("This is Email: " + userList[i].Email);
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("Error during deserialization: " + ex.Message);
            }
        }
        for (int i = 0; i < userList.Count; i++)
        {
            if (userList[i].Email == email)
            {
                Trace.WriteLine("This is Email: " + userList[i].Email);
                return new CustomType { Success = true, Message = "User has been successfully found. You can now proceed with the order." };            }
        }
        return new CustomType { Success = false, Message = "User Not Found" };
    }

    // Method to check if a user is eligible for a free coffee
    public async Task<bool> getIsFree(string email)
    {
        var path = new FileManagement().DirectoryPath("database", "orderData.json");
        Trace.WriteLine("This is Path in Get is Free: " + path);
        int count = 0;
        List<OrderModel> orderList = new List<OrderModel>();
        if (File.Exists(path))
        {
            Trace.WriteLine("File Exists");
            var existingData = await File.ReadAllTextAsync(path);
            orderList = JsonSerializer.Deserialize<List<OrderModel>>(existingData) ?? new List<OrderModel>();
            Trace.WriteLine("This is Order Count: " + orderList.Count());
            for (int i = 0; i < orderList.Count(); i++)
            {
                if (orderList[i].Email == email)
                {
                    count = orderList[i].Count;
                }
                Trace.WriteLine("This is Count: " + count);
            }
            if (count >= 10)
            {
                return true;
            }
        }
        return false;
    }
    // Method to check if a member is a regular customer and update discount eligibility
    public async Task<bool> IsRegularCustomer(string email)
    {
        var orderPath = new FileManagement().DirectoryPath("database", "orderData.json");
        var userPath = new FileManagement().DirectoryPath("database", "user.json");
        List<OrderModel> orderList = new List<OrderModel>();
        List<UserModel> userList = new List<UserModel>();
        if (File.Exists(orderPath))
        {
            var existingOrderData = await File.ReadAllTextAsync(orderPath);
            orderList = JsonSerializer.Deserialize<List<OrderModel>>(existingOrderData) ?? new List<OrderModel>();
        }
        if (File.Exists(userPath))
        {
            var existingUserData = await File.ReadAllTextAsync(userPath);
            userList = JsonSerializer.Deserialize<List<UserModel>>(existingUserData) ?? new List<UserModel>();
        }

        var userOrders = orderList.Where(o => o.Email == email).OrderBy(o => o.Date).ToList();
        var user = userList.FirstOrDefault(u => u.Email == email);
        if (user == null)
        {
            return false;
        }

        var groupedOrders = userOrders.GroupBy(o => new { o.Date.Year, o.Date.Month, o.Date.Day });
        Trace.WriteLine("This is Grouped Orders: " + groupedOrders.Count());
        var distinctDaysWithOrdersInCurrentMonth = groupedOrders.Count(g => g.Key.Month == DateTime.Now.Month && g.Key.Year == DateTime.Now.Year);
        Trace.WriteLine("This is Distinct Days: " + distinctDaysWithOrdersInCurrentMonth);
        if (distinctDaysWithOrdersInCurrentMonth >= 26)
        {
            Trace.WriteLine("This is User: ", user.Email);
            var currentMonth = DateTime.Now.Month;
            var currentYear = DateTime.Now.Year;
            user.DiscountEligibleUntil = new DateTime(currentYear, currentMonth == 12 ? 1 : currentMonth + 1, DateTime.DaysInMonth(currentYear, currentMonth == 12 ? 1 : currentMonth + 1));
            var updatedUserData = JsonSerializer.Serialize(userList);
            await File.WriteAllTextAsync(userPath, updatedUserData);
            return true;
        }

        return false;
    }
    // Method to check if a user is eligible for a discount
    public async Task<bool> IsEligibleForDiscount(string email)
    {
        try
        {
            var userPath = new FileManagement().DirectoryPath("database", "user.json");
            List<UserModel> userList = new List<UserModel>();
            if (File.Exists(userPath))
            {
                var existingUserData = await File.ReadAllTextAsync(userPath);
                userList = JsonSerializer.Deserialize<List<UserModel>>(existingUserData) ?? new List<UserModel>();
            }

            var user = userList.FirstOrDefault(u => u.Email == email);
            if (user == null)
            {
                return false;
            }

            DateTime discountEligibleUntil;
            Trace.WriteLine("This is Date of Eligible: " + user.DiscountEligibleUntil);
            if (user.DiscountEligibleUntil != null)
            {
                if (DateTime.TryParse(user.DiscountEligibleUntil.ToString(), out discountEligibleUntil))
                {
                    Trace.WriteLine("10% Discount for 1 Month");
                    bool value = DateTime.Now <= discountEligibleUntil;
                    Trace.WriteLine("This is Value: " + value);
                    return value;
                }
                else
                {
                    Trace.WriteLine("Invalid date: " + user.DiscountEligibleUntil);
                    return false;
                }
            }
            return false;
        }
        catch (Exception error)
        {
            throw error;
        }
    }
}