using System;
using System.Collections.Generic;
using System.Linq;

public interface TailCall<T>
{
    TailCall<T> Apply();
    bool IsComplete();
    T Result();
    T Get()
    {
        return EnumerableEx
        .Generate(this, x => !x.IsComplete(), x => x.Apply(), x => x.Result())
        .Last();
    }
}

public class MyTailCall<T> : TailCall<T>
{
    private readonly T value;

    public MyTailCall(T value)
    {
        this.value = value;
    }

    public TailCall<T> Apply()
    {
        throw new Exception("not implemented!!!");
    }

    public bool IsComplete()
    {
        return true;
    }

    public T Result()
    {
        return value;
    }
}

public static class TailCalls
{
    public static TailCall<T> Call<T>(TailCall<T> nextcall)
    {
        return nextcall;
    }

    public static TailCall<T> Done<T>(T value)
    {
        return new MyTailCall<T>(value);
    }
}

public class FactorialTailCall : TailCall<long>
{
    private readonly long result;
    private readonly int number;
    private readonly int factorial;

    public FactorialTailCall(int number, int factorial, long result)
    {
        this.number = number;
        this.factorial = factorial;
        this.result = result;
    }

    public TailCall<long> Apply()
    {
        if (number == 0)
        {
            return TailCalls.Done(result);
        }
        else
        {
            return TailCalls.Call(new FactorialTailCall(number - 1, factorial * number, result));
        }
    }

    public bool IsComplete()
    {
        return false;
    }

    public long Result()
    {
        return result;
    }

    public long GetFactorial()
    {
        return TailCalls.Call(new FactorialTailCall(number, factorial, 1)).Get();
    }

    public long Get()
    {
        return EnumerableEx
            .Generate(this, x => !x.IsComplete(), x => x.Apply(), x => x.Result())
            .Last();
    }
}

class Program
{
    public static void Main()
    {
        FactorialTailCall factorialTailCall = new FactorialTailCall(5, 1, 1);
        long factorial = factorialTailCall.GetFactorial();
        Console.WriteLine(factorial); // Output: 120
    }
}




/*

using System;

public interface TailCall<T>
{
    TailCall<T> Apply();
    bool IsComplete();
    T Result();
    T Get()
    {
        return EnumerableEx
        .Generate(this, x => !x.IsComplete(), x => x.Apply(), x => x.Result())
        .Last();
    }
}

public class MyTailCall<T> : TailCall<T>
{
    private readonly T value;

    public MyTailCall(T value)
    {
        this.value = value;
    }

    public TailCall<T> Apply()
    {
        throw new Exception("not implemented!!!");
    }

    public bool IsComplete()
    {
        return true;
    }

    public T Result()
    {
        return value;
    }
}



public static class TailCalls
{
    public static TailCall<T> Call<T>(TailCall<T> nextcall)
    {
        return nextcall;
    }

    public static TailCall<T> Done<T>(T value)
    {
        return new MyTailCall<T>(value);
    }
}


public class FactorialTailCall : TailCall<long>
{
    private readonly long result;
    private readonly int number;
    private readonly int factorial;

    public FactorialTailCall(int number, int factorial, long result)
    {
        this.number = number;
        this.factorial = factorial;
        this.result = result;
    }

    public TailCall<long> Apply()
    {
        if (number == 0)
        {
            return TailCalls.Done(result);
        }
        else
        {
            return TailCalls.Call(new FactorialTailCall(number - 1, factorial * number, result));
        }
    }

    public bool IsComplete()
    {
        return false;
    }

    public long Result()
    {
        return result;
    }

    public long GetFactorial()
    {
        return TailCalls.Call(new FactorialTailCall(number, factorial, 1)).Get();
    }

    public long Get()
    {
        return EnumerableEx
        .Generate(this, x => !x.IsComplete(), x => (FactorialTailCall)x.Apply(), x => x.Result())
        .Last();
    }
}

class Program
{
    public static void Main()
    {
        FactorialTailCall factorialTailCall = new FactorialTailCall(5, 1, 1);
        long factorial = factorialTailCall.GetFactorial();
        Console.WriteLine(factorial); // Output: 120
    }
}*/