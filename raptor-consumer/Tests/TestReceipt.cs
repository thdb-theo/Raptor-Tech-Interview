using System.Text.Json;

namespace Tests;



public class TestReceipt
{

    public void RunTests()
    {
        TestCalculation();

        Console.WriteLine("All Receipt tests passed\n");
    }

    public void TestCalculation()
    {
        var reg_customer = new Customer("regular@a.a");
        reg_customer.Status = Status.REGULAR;

        var silver_customer = new Customer("silver@a.a");
        silver_customer.Status = Status.SILVER;

        var gold_customer = new Customer("gold@a.a");
        gold_customer.Status = Status.GOLD;

        var order = new Order {Price = 100, Id = 1, CustomerEmail = reg_customer.Email};

        var regular_receipt = new Receipt(order, reg_customer);
        var silver_receipt = new Receipt(order, silver_customer);
        var gold_receipt = new Receipt(order, gold_customer);

        Assert.IsTrue(regular_receipt.Discount == 1.0f);
    
        Assert.Equals(regular_receipt.Price, order.Price);
        Assert.IsTrue(regular_receipt.Price > silver_receipt.Price);
        Assert.IsTrue(silver_receipt.Price > gold_receipt.Price);
    
        Console.WriteLine("TestCalculation passed");
    }


}