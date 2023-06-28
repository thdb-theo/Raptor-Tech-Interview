// I tried for long time to get a UnitTest library to work, but it just wouldn't. So I wrote my own.

class Assert{
    public static void IsTrue(bool condition)
    {
        if (!condition)
        {
            throw new Exception("Assertion failed");
        }
    }

    public static void Equals<T>(T a, T b)
    {
        if (!a.Equals(b))
        {
            throw new Exception("Assertion failed! Expected " + a + " to equal " + b);
        }
    }

}