using System.Text.Json;

namespace Tests;



public class TestOrder
{

    public void RunTests()
    {
        TestContruction();
        TestSerialization();

        Console.WriteLine("All Order tests passed\n");
    }

    public void TestContruction()
    {
        var order = new Order();
        Assert.IsTrue(order != null);

        Console.WriteLine("TestContruction passed");
    }

    public void TestSerialization()
    {
        var order = new Order
        {
            Id = 1,
            Price = 2.0f,
            CustomerEmail = "a@a.a"
        };
        var json = JsonSerializer.Serialize(order);
        Assert.Equals(json, "{\"Price\":2,\"Id\":1,\"CustomerEmail\":\"a@a.a\"}");
        var unserialized = JsonSerializer.Deserialize<Order>(json);
        Assert.IsTrue(unserialized != null);
        Assert.IsTrue(unserialized.Id == 1);
        Assert.IsTrue(unserialized.Price == 2.0f);
        Assert.Equals(unserialized.CustomerEmail, "a@a.a");

        Console.WriteLine("TestSerialization passed");
    }
}