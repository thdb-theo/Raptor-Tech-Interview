using System.Text.Json;

namespace Tests;



public class TestStatus
{

    public void RunTests()
    {
        TestStatuses();

        Console.WriteLine("All Status tests passed\n");
    }

    public void TestStatuses()
    {
        var reg = Status.REGULAR;
        var silver = Status.SILVER;
        var gold = Status.GOLD;

        Assert.IsTrue(reg.Discount > silver.Discount);
        Assert.IsTrue(silver.Discount > gold.Discount);

        Assert.Equals(reg.ToString(), "REGULAR");
        Assert.Equals(silver.ToString(), "SILVER");
        Assert.Equals(gold.ToString(), "GOLD");

        Assert.Equals(reg.Upgraded() , Status.SILVER);
        Assert.Equals(silver.Upgraded() , Status.GOLD);
        Assert.Equals(gold.Upgraded() , Status.GOLD);

        Console.WriteLine("TestCalculation passed");

    }
}