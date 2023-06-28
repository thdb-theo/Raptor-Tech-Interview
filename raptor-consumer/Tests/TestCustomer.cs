using System.Text.Json;

namespace Tests;



public class TestCustomer
{
    public void RunTests()
    {
        TestStatusUpdateBumpToSilver();
        TestStatusUpdateBumpToGold();

        Console.WriteLine("All Customer tests passed\n");
    }

    static void TestStatusUpdateBumpToSilver() {
        var customer = new Customer("test@test.test");
        var receipt = new Receipt(customer.Email, 151, 1f, DateTime.Now, 1);

        var orderHistory = new List<Receipt>();
        Assert.IsTrue(!customer.UpdateStatus(orderHistory, new Order()));
        Assert.Equals(customer.Status, Status.REGULAR);
        
        orderHistory.Add(receipt);
        Assert.IsTrue(!customer.UpdateStatus(orderHistory, new Order()));
        Assert.Equals(customer.Status, Status.REGULAR);


        orderHistory.Add(receipt);
        Assert.IsTrue(customer.UpdateStatus(orderHistory, new Order()));
        Assert.Equals(customer.Status, Status.SILVER);

        orderHistory.Add(receipt);
        Assert.IsTrue(!customer.UpdateStatus(orderHistory, new Order()));
        Assert.Equals(customer.Status, Status.SILVER);


        // You cannot be bumped with one purchase
        var bigOrder = new Order {
            Price = 1000,
            Id = 1,
            CustomerEmail = customer.Email
        };

        customer.Status = Status.REGULAR;
        orderHistory.Clear();
        // orderHistory.Add(bigOrder);
        Assert.IsTrue(!customer.UpdateStatus(orderHistory, bigOrder));
        Assert.Equals(customer.Status, Status.REGULAR);

        orderHistory.Add(new Receipt(bigOrder, customer));

        Assert.IsTrue(customer.UpdateStatus(orderHistory, new Order() { Price = 1 }));
        Assert.Equals(customer.Status, Status.SILVER);
        
        Console.WriteLine("TestStatusUpdateBumpToSilver passed");
    }

    static void TestStatusUpdateBumpToGold() {
        var customer = new Customer("test@test.test");
        customer.Status = Status.SILVER;
        var orderHistory = new List<Receipt>();
        Assert.IsTrue(!customer.UpdateStatus(orderHistory, new Order()));
        
        var receipt = new Receipt(customer.Email, 601f, 0.9f, DateTime.Now, 1);

        // Too recently bumped
        orderHistory.Add(receipt);
        customer.BumpedAt = DateTime.Now - TimeSpan.FromDays(3);
        Assert.IsTrue(!customer.UpdateStatus(orderHistory, new Order()));
        Assert.Equals(customer.Status, Status.SILVER);

        // You can be bumped after 7 days
        // Also with only one purchase
        orderHistory.Clear();
        customer.BumpedAt = DateTime.Now - TimeSpan.FromDays(8);
        Assert.IsTrue(customer.UpdateStatus(orderHistory, new Order() {Price = 1000}));
        Assert.Equals(customer.Status, Status.GOLD);

        // Not enough money spent
        customer.Status = Status.SILVER;
        Assert.IsTrue(!customer.UpdateStatus(orderHistory, new Order() {Price = 400}));
        Assert.Equals(customer.Status, Status.SILVER);

        Console.WriteLine("TestStatusUpdateBumpToGold passed");
    }
}