using System.Collections;
using System;

public static class TailCalls
{
    public static TailCall<T> Call<T>(TailCall<T> nextCall) => nextCall;

    public static TailCall<T> Done<T>(T value) => new CompletedTailCall<T>(value);

    private sealed class CompletedTailCall<T> : TailCall<T>
    {
        private readonly T _value;

        public CompletedTailCall(T value)
        {
            _value = value;
        }

        public bool IsComplete => true;

        public T Result => _value;

        public TailCall<T> Apply() => throw new InvalidOperationException("Cannot apply a completed tail call");
    }
}

public interface TailCall<T>
{
    bool IsComplete { get; }
    T Result { get; }
    TailCall<T> Apply();
}

public static class Factorial
{
    public static ulong RecursiveFactorial(ulong n)
    {
        return RecursiveFactorialHelper(TailCalls.Done<ulong>(1), n).Apply();
    }

    private static TailCall<ulong> RecursiveFactorialHelper(TailCall<ulong> current, ulong n)
    {
        if (n == 1)
        {
            return current;
        }
        else
        {
            return TailCalls.Call<ulong>(() => RecursiveFactorialHelper(TailCalls.Done<ulong>(n * current.Result), n - 1));
        }
    }
}


class Program
{
    static void Main(string[] args)
    {
        ulong result = Factorial.RecursiveFactorial(10);
        Console.WriteLine(result); // Output: 3628800
    }
}
