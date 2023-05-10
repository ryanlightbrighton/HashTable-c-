using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace OpenAddressHashTable
{

    public class Program
    {
        public static void Main()
        {
            // vars /////////////////////////////////////////////////////////
            int range = 4000;
            OpenAddressHashTable<string, int>.PROBE_TYPE probe = OpenAddressHashTable<string, int>.PROBE_TYPE.GR_PROBE;
            int select = 1;
            ///////////////////////////////////////////////////////////////////////////////////////////
            ///
            /*Console.WriteLine("Pls enter KGB password: ");
            Console.ReadLine();*/
            /*DEFAULT*/
            if (select == 0)
            {
                Hashtable table = new();
                Stopwatch stopWatch2 = new();
                stopWatch2.Start();
                Enumerable.Range(0, range).ToList().ForEach(i =>
                    Enumerable.Range(1, range).Reverse().ToList().ForEach(j =>
                    {
                        table.Add(i + ":" + j, i * j);
                    })
                );

                Enumerable.Range(0, range).ToList().ForEach(i =>
                    Enumerable.Range(1, range).Reverse().ToList().ForEach(j =>
                    {
                        int value = (int)table[i + ":" + j];
                    })
                );
                stopWatch2.Stop();
                TimeSpan ts2 = stopWatch2.Elapsed;
                string elapsedTime2 = string.Format(
                    "{0:00}:{1:00}:{2:00}.{3:00}",
                    ts2.Hours,
                    ts2.Minutes,
                    ts2.Seconds,
                    ts2.Milliseconds / 10
                );
                Console.WriteLine("RunTime Default: " + elapsedTime2);
            }
            if (select == 1) { 
                    /*OPEN ADDRESS*/
                OpenAddressHashTable<string, int> myTable = new(50, probe);
                Stopwatch stopWatch = new();
                stopWatch.Start();
                Enumerable.Range(0, range).ToList().ForEach(i =>
                    Enumerable.Range(1, range).Reverse().ToList().ForEach(j =>
                    {
                        myTable.InsertPair(i + ":" + j, i * j);
                    })
                );

                Enumerable.Range(0, range).ToList().ForEach(i =>
                    Enumerable.Range(1, range).Reverse().ToList().ForEach(j =>
                    {
                        myTable.GetValue(i + ":" + j);
                    })
                );
                stopWatch.Stop();
                TimeSpan ts = stopWatch.Elapsed;
                string elapsedTime = string.Format(
                    "{0:00}:{1:00}:{2:00}.{3:00}",
                    ts.Hours,
                    ts.Minutes,
                    ts.Seconds,
                    ts.Milliseconds / 10
                );
                Console.WriteLine("RunTime OpenAddress: " + elapsedTime);
                /*Console.WriteLine($"Is in table: {myTable.GetValue("not in table").found}");
                myTable.InsertPair("hello", 69);
                myTable.Delete("hello");
                myTable.InsertPair("hello", 699);
                Console.WriteLine($"Is in table: {myTable.GetValue("hello").found}");*/
            }




            /*STRUCT*/
            if (select == 2)
            {
                OpenAddressHashTableStruct<string, int> myTable2 = new(50, probe);
                Stopwatch stopWatch3 = new();
                stopWatch3.Start();
                Enumerable.Range(0, range).ToList().ForEach(i =>
                    Enumerable.Range(1, range).Reverse().ToList().ForEach(j =>
                    {
                        myTable2.InsertPair(i + ":" + j, i * j);
                    })
                );

                Enumerable.Range(0, range).ToList().ForEach(i =>
                    Enumerable.Range(1, range).Reverse().ToList().ForEach(j =>
                    {
                        myTable2.GetValue(i + ":" + j);
                    })
                );
                stopWatch3.Stop();
                TimeSpan ts3 = stopWatch3.Elapsed;
                string elapsedTime3 = string.Format(
                    "{0:00}:{1:00}:{2:00}.{3:00}",
                    ts3.Hours,
                    ts3.Minutes,
                    ts3.Seconds,
                    ts3.Milliseconds / 10
                );
                Console.WriteLine("RunTime Struct:  " + elapsedTime3);
            }


        }
    }
    public class OpenAddressHashTable<K, V> {
        /*static OpenAddressHashTable() {
            // Code to run at program inception goes here
            Console.WriteLine("hello");
            primes = new bool[Math.Max(1 * 10, 100000000)];
            Console.WriteLine("def array");
            MakeSieve(primes.Length);
            Console.WriteLine("goodbye");
        }*/
        public OpenAddressHashTable(int initialCapacity, PROBE_TYPE pt) {
            probeType = pt;
            ConstructorCommon(initialCapacity);
        }

        protected void ConstructorCommon(int initialCapacity) {
            primes = new bool[Math.Max(initialCapacity * 10, 100000000)];
            MakeSieve(primes.Length);
            SetCapacity(NextPrime(initialCapacity));
            Pair[] newArr = new Pair[GetCapacity()];
            SetTableArray(newArr);
            SetCapacity(GetCapacity());
            SetItemCount(0);
        }

        protected static Pair[]? tableArray;                                // an array of Pair objects
        protected static int max;                                           // the size of arr. This should be a prime number
        protected static int itemCount;                                     // the number of items stored in arr
        protected const double maxLoad = 0.6;                               // the maximum load factor
        protected static bool[]? primes;                                    // array of bool used to find primes
        protected static readonly double ratio = (1 + Math.Sqrt(5)) / 2;    // golden ratio - used in probe
        public enum PROBE_TYPE
        {
            LINEAR_PROBE, 
            QUADRATIC_PROBE, 
            DOUBLE_HASH, 
            GR_PROBE
        }
        // list of primes used by the hash function
        protected static readonly int[] akrownePrimes  = new int[] {
            1610612741,
            805306457, 
            402653189, 
            201326611, 
            100663319, 
            50331653, 
            25165843, 
            12582917, 
            6291469, 
            3145739, 
            1572869, 
            786433, 
            393241, 
            196613, 
            98317, 
            49157, 
            24593, 
            12289, 
            6151, 
            3079, 
            1543, 
            769, 
            389, 
            193, 
            97, 
            53 
        };
        protected static  PROBE_TYPE probeType;
        public double GetMaxLoad() {
            return maxLoad;
        }
        public static Pair[] GetTableArray() {
            return tableArray;
        }
        public void SetTableArray(Pair[] x) {
            tableArray = (OpenAddressHashTable<K, V>.Pair[])x;
        }
        public static int GetCapacity() {
            return max;
        }
        public void SetCapacity(int x)
        {
            max = x;
        }
        public int GetItemCount()
        {
            return itemCount;
        }
        public void SetItemCount(int x)
        {
            itemCount = x;
        }
        public void IncrementItemCount()
        {
            itemCount++;
        }
        public static bool[] GetPrimes()
        {
            return primes;
        }
        public void SetPrimes(bool[] x)
        {
            primes = x;
        }
        public void InsertPair(K key, V value)
        {
            if (GetLoadFactor() > GetMaxLoad())
            {
                Resize();
            }
            int index = Hash(key);
            if (GetTableArray()[index] != null)
            {
                //index = FindEmpty(index, 0, key);
                index = FindEmptyStackSafe(index, 0, key);
            }
            GetTableArray()[index] = OpenAddressHashTable<K, V>.CreateNode(key, value);
            IncrementItemCount();
        }
        public (V? value, bool found) GetValue(K key)
        {
            int startPos = Hash(key);
            //Pair? pair = FindPair(startPos, key, 0);
            Pair? pair = FindPairStackSafe(startPos, key, 0);
            if (pair != null)
            {
                return (pair.GetValue(), true);
            }
            else return (default, false);
        }
        public bool HasKey(K key)
        {
            bool hasKey = false;
            if (GetValue(key).found)
            {
                hasKey = true;
            }
            return hasKey;
        }

        public Collection<K> GetKeys()
        {
            Collection<K> coll = new ();
            return GetKeys(coll, 0);
        }

        /**
         * <font size='5'><h1>{@code getKeys(Collection<K> coll,int i)}</h1></font>
         * <b><h2>Description</h2></b>
         * Iterates through array and returns arraylist of keys  
         * @param Arraylist K
         * @param integer
         * @return ArrayList K
         */

        public Collection<K> GetKeys(Collection<K> coll, int i)
        {
            if (i == GetCapacity())
            {
                return coll;
            }
            else
            {
                if (GetTableArray()[i] != null)
                {
                    coll.Add(GetTableArray()[i].GetKey());
                }
                return GetKeys(coll, i + 1);
            }
        }

        /*public TailCall<Collection<K>> getKeys(Collection<K> coll, int i)
        {
            if (i == getCapacity())
            {
                return TailCalls.done(coll);
            }
            else
            {
                if (getTableArray()[i] != null)
                {
                    coll.add(((OpenAddressHashTable<K, V>.Pair)getTableArray()[i]).getKey());
                }
                return TailCalls.call(()->getKeys(coll, i + 1));
            }
        }*/

        /**
         * <font size='5'><h1>{@code getLoadFactor()}</h1></font>
         * <b><h2>Description</h2></b>
         * Returns the load factor of the hash table  
         * @param none
         * @return double
         */
        public double GetLoadFactor()
        {
            return ((double)GetItemCount() / GetCapacity());
        }

        /*public bool Remove(K key)
        {
            int startPos = Hash(key);

            Pair pair = GetTableArray()[startPos] as Pair;

            return false;
        }*/

        /**
         * <font size='5'><h1>{@code find(int startPos, K key, int stepNum)}</h1></font>
         * <b><h2>Description</h2></b>
         * Finds the value stored for this key, starting the search at position startPos in the array. If
         * the item at position startPos is null, the Hash table does not contain the value, so returns null. 
         * If the key stored in the pair at position startPos matches the key we're looking for, then it return the associated 
         * value. If the key stored in the pair at position startPos does not match the key we're looking for, this
         * is a hash collision so it uses the getNextLocation method with an incremented value of stepNum to find 
         * the next location to search (the way that this is calculated will differ depending on the probe type 
         * being used). Then it uses the value of the next location in a recursive call to find.
         * @param startPos
         * @param key
         * @param stepNum
         * @return V
         */

        private Pair? FindPair(int startPos, K key, int stepNum)
        {
            Pair pair = (OpenAddressHashTable<K, V>.Pair)GetTableArray()[startPos];
            if (pair == null)
            {
                return null;
            }
            else
            if (pair.GetKey().Equals(key))
            {
                return pair;
            }
            else
            {
                return FindPair(GetNextLocation(startPos, 1 + stepNum, key), key, stepNum);
            }
        }
        /*private TailCall<V> find(int startPos, K key, int stepNum)
        {
            Pair pair = (OpenAddressHashTable<K, V>.Pair)getTableArray()[startPos];
            if (pair == null)
            {
                return TailCalls.done(null);
            }
            else if (pair.getKey().Equals(key))
            {
                return TailCalls.done(pair.getValue());
            }
            else
            {
                return TailCalls.call(()->find(getNextLocation(startPos, 1 + stepNum, key), key, stepNum));
            }
        }*/



        // used for deleting
        private int FindIndex(int startPos, K key, int stepNum)
        {
            Pair pair = (OpenAddressHashTable<K, V>.Pair)GetTableArray()[startPos];
            if (pair == null)
            {
                return -1;
            }
            else
            if (pair.GetKey().Equals(key))
            {
                return startPos;
            }
            else
            {
                return FindIndex(GetNextLocation(startPos, 1 + stepNum, key), key, stepNum);
            }
        }
        public bool Delete(K key)
        {
            int startPos = Hash(key);
            int index = FindIndex(startPos, key, 0);
            if (index > -1)
            {
                GetTableArray()[index] = null;
                return true;
            }
            return false; // key not found
        }
        /**
         * <font size='5'><h1>{@code findEmpty(int startPos, int stepNum, T key)}</h1></font>
         * <b><h2>Description</h2></b>
         * Finds the first unoccupied location where a value associated with key can be stored, starting the
         * search at position startPos. If startPos is unoccupied, it returns startPos. Otherwise it uses the getNextLocation
         * method with an incremented value of stepNum to find the appropriate next position to check 
         * (which will differ depending on the probe type being used) and uses this in a recursive call to findEmpty.
         * @param startPos
         * @param stepNum
         * @param key
         * @return
         */
        protected int FindEmpty(int startPos, int stepNum, K key)
        {
            if (GetTableArray()[startPos] == null)
            {
                return startPos;
            }
            else
            {
                return FindEmpty(GetNextLocation(startPos, 1 + stepNum, key), stepNum, key);
            }
        }
        /*private TailCall<Integer> findEmpty(int startPos, int stepNum, K key)
        {
            if (getTableArray()[startPos] == null)
            {
                return TailCalls.done(startPos);
            }
            else
            {
                return TailCalls.call(()->findEmpty(getNextLocation(startPos, 1 + stepNum, key), stepNum, key));
            }
        }*/

        /**
         * <font size='5'><h1>{@code getNextLocation(int startPos, int stepNum, T key)}</h1></font>
         * <b><h2>Description</h2></b>
         * Finds the next position in the Hashtable array starting at position startPos. If the linear
         * probe is being used, it increments startPos. If the double hash probe type is being used, 
         * add the double hashed value of the key to startPos. If the quadratic probe is being used, it adds
         * the square of the step number to startPos.  If Golden Ratio probe is used, it adds the current capacity
         * of the array divided by the Golden Ratio
         * @param integer
         * @param stepNum
         * @param T
         * @return integer
         */
        protected static  int GetNextLocation(int startPos, int stepNum, K key)
        {
            int step = startPos;
            switch (probeType)
            {
                case PROBE_TYPE.LINEAR_PROBE:       step++;                                                 break;
                case PROBE_TYPE.DOUBLE_HASH:        step += OpenAddressHashTable<K, V>.DoubleHash(key);     break;
                case PROBE_TYPE.QUADRATIC_PROBE:    step += stepNum * stepNum;                              break;
                case PROBE_TYPE.GR_PROBE:           step += (int)(GetCapacity() / ratio);                   break;
                default: break;
            }
            return step % GetCapacity();
        }

        /**
         * <font size='5'><h1>{@code doubleHash(K input)}</h1></font>
         * <b><h2>Description</h2></b>
         * A secondary hash function which returns a small value
         * to probe the next location if the double hash probe type is being used
         * @param K
         * @return integer
         */
        protected static int DoubleHash(K input)
        {
            string key = input.ToString();
            int prime = 683;
            long hash = 0;
            for (int i = 0; i < key.Length; i++)
            {
                hash += (prime - (key[i]));
            }
            hash &= 0x7fffffff;
            return (int)hash % prime;
        }

        /**
         * <font size='5'><h1>{@code hash(K input)}</h1></font>
         * <b><h2>Description</h2></b>
         * Return an integer value calculated by hashing the key. The bitwise AND operator is applied to
         * make sure the return value is not negative and modulus of the capacity to keep within bounds
         * @param key
         * @return integer
         */
        protected int Hash(K input)
        {
            string key = input.ToString();
            long hash = 5381;
            int prime = 6291469;
            for (int i = 0; i < key.Length; i++)
            {
                hash = ((hash << 5) + hash + (prime << 4) + (key[i]));
                prime = akrownePrimes[i % akrownePrimes.Length];
            }
            hash &= 0x7fffffff;
            return (int)hash % GetCapacity();
        }

        /**
         * <font size='5'><h1>{@code makeSieve(int length)}</h1></font>
         * <b><h2>Description</h2></b>
         * Creates the primes sieve used by the resize function
         * @param integer
         * @return none
         */
        public static void MakeSieve(int length)
        {
            for (int i = 0; i < length; i++)
            {
                GetPrimes()[i] = true;
            }
            GetPrimes()[0] = GetPrimes()[1] = false;  // 2 is smallest prime
            for (int i = 2; i < GetPrimes().Length; i++)
            {
                //if i is prime its multiples are not
                if (GetPrimes()[i])
                {
                    for (int j = 2; i * j < GetPrimes().Length; j++)
                    {
                        GetPrimes()[i * j] = false;
                    }
                }
            }
        }

        /**
         * <font size='5'><h1>{@code isPrime(int n)}</h1></font>
         * <b><h2>Description</h2></b>
         * Returns true if n is prime
         * @param integer
         * @return boolean
         */
        protected bool IsPrime(int n)
        {
            return GetPrimes()[n];
        }

        /**
         * <font size='5'><h1>{@code nextPrime(int n)}</h1></font>
         * <b><h2>Description</h2></b>
         * Gets the smallest prime number which is larger than n
         * @param n
         * @return integer
         */
        protected int NextPrime(int n)
        {
            int nextPrime = -1;
            if (n >= GetPrimes().Length - 1)
            { 
                // we start our check at n+1 (n++) so check against primes length -1 (so not out of range);
                throw new Exception("Not enough primes - make your prime array bigger!");
            }
            for (n++; n < GetPrimes().Length; n++)
            {
                if (IsPrime(n))
                {
                    nextPrime = n;
                    break;
                }
            }
            return nextPrime;
        }

        /**
         * <font size='5'><h1>{@code resize()}</h1></font>
         * <b><h2>Description</h2></b>
         * Resizes the hashtable, when the load factor exceeds maxLoad. The new size of
         * the underlying array is the smallest prime number which is at least twice the size
         * of the old array.
         * @param none
         * @return none
         */
        protected void Resize()
        {
            Pair[] oldArr = GetTableArray();
            int primeNumb = NextPrime(2 * oldArr.Length);
            Pair[] newArr = new Pair[primeNumb];
            SetCapacity(primeNumb);
            SetTableArray(newArr);
            SetItemCount(0);
            for (int i = 0; i < oldArr.Length; i++)
            {
                Pair pair = (OpenAddressHashTable<K, V>.Pair)oldArr[i];
                if (pair != null)
                {
                    K key = pair.GetKey();
                    V value = pair.GetValue();
                    InsertPair(key, value);
                }
            }
        }

        /**
         * <font size='5'><h1>{@code createNode(K key, V value)}</h1></font>
         * <b><h2>Description</h2></b>
         * Creates a new Pair object and assigns key and value
         * @param K
         * @param V
         * @return Pair
         */
        protected static Pair CreateNode(K key, V value)
        {
            return new Pair(key, value);
        }


        /**
         * <font size='5'><h1>{@code Pair}</h1></font>
         * <b><h2>Description</h2></b>
         * Class for Pair objects
         */

        public struct KeyValuePair<Kk, Vv>
        {
            public Kk? Key { get; set; }
            public Vv? Value { get; set; }

            public KeyValuePair(Kk key, Vv value)
            {
                Key = key;
                Value = value;
            }


        }
        public class Pair
        {
            private readonly K key;
            private readonly V value;
            public Pair(K key, V value)
            {
                this.key = key;
                this.value = value;
            }
            public K GetKey()
            {
                return key;
            }
            public V GetValue()
            {
                return value;
            }
        }

        public interface TailCall<T>
        {
            TailCall<T> Apply();
            bool IsComplete();
            T Result();
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

        public class FindPairTailCall : TailCall<Pair?>
        {
            private readonly int startPos;
            private readonly K key;
            private readonly int stepNum;

            public FindPairTailCall(int startPos, K key, int stepNum)
            {
                this.startPos = startPos;
                this.key = key;
                this.stepNum = stepNum;
            }

            public TailCall<Pair?> Apply()
            {
                Pair pair = (OpenAddressHashTable<K, V>.Pair)GetTableArray()[startPos];
                if (pair == null)
                {
                    return new MyTailCall<Pair?>(null);
                }
                else if (pair.GetKey().Equals(key))
                {
                    return new MyTailCall<Pair?>(pair);
                }
                else
                {
                    return new FindPairTailCall(GetNextLocation(startPos, 1 + stepNum, key), key, stepNum);
                }
            }

            public bool IsComplete()
            {
                return false;
            }

            public Pair? Result()
            {
                throw new Exception("not implemented!!!");
            }

            public Pair? GetResult()
            {
                TailCall<Pair?> tailCall = this;
                while (!tailCall.IsComplete())
                {
                    tailCall = tailCall.Apply();
                }
                return tailCall.Result();
            }
        }

        public Pair? FindPairStackSafe(int startPos, K key, int stepNum) {
            return new FindPairTailCall(startPos, key, stepNum).GetResult();
        }

        public class FindEmptyTailCall : TailCall<int>
        {
            private readonly int startPos;
            private readonly int stepNum;
            private readonly K key;
            public FindEmptyTailCall(int startPos, int stepNum, K key)
            {
                this.startPos = startPos;
                this.stepNum = stepNum;
                this.key = key;
            }

            public TailCall<int> Apply()
            {
                Pair pair = (OpenAddressHashTable<K, V>.Pair)GetTableArray()[startPos];
                if (pair == null)
                {
                    return new MyTailCall<int>(startPos);
                }
                else
                {
                    return new FindEmptyTailCall(GetNextLocation(startPos, 1 + stepNum, key), stepNum, key);
                }
            }

            public bool IsComplete()
            {
                return false;
            }

            public int Result()
            {
                return startPos;
            }

            public int GetResult()
            {
                TailCall<int> tailCall = this;
                while (!tailCall.IsComplete())
                {
                    tailCall = tailCall.Apply();
                }
                return tailCall.Result();
            }
        }

        protected int FindEmptyStackSafe(int startPos, int stepNum, K key) {
            if (GetTableArray()[startPos] == null) {
                return startPos;
            } else {
                return new FindEmptyTailCall(GetNextLocation(startPos, 1 + stepNum, key), stepNum, key).GetResult();
            }
        }

    }

    public class OpenAddressHashTableStruct<K, V> : OpenAddressHashTable<K, V> {
        public OpenAddressHashTableStruct(int initialCapacity, PROBE_TYPE pt) : base(initialCapacity, pt) {
            probeType = pt;
            ConstructorCommon(initialCapacity);
        }

        protected new KeyValuePair<K, V>?[] tableArray;

        public KeyValuePair<K, V>?[] GetTableArray() {
            return tableArray;
        }
        public void SetTableArray(KeyValuePair<K, V>?[] x) {
            tableArray = x;
        }

        protected KeyValuePair<K, V> CreateNode(K key, V value) {
            KeyValuePair <K, V> p = new KeyValuePair<K, V >();
            return p;
        }

        private KeyValuePair<K, V>? FindPair(int startPos, K key, int stepNum)
        {
            KeyValuePair<K, V>? pair = GetTableArray()[startPos];
            if (pair == null) {
                return null;
            } else if (pair?.Key.Equals(key) == true) {
                return pair;
            } else {
                return FindPair(GetNextLocation(startPos, 1 + stepNum, key), key, stepNum);
            }
        }

        protected void Resize() {
            KeyValuePair<K, V>?[] oldArr = GetTableArray();
            int primeNumb = NextPrime(2 * oldArr.Length);
            KeyValuePair<K, V>?[] newArr = new KeyValuePair<K, V>?[primeNumb];
            SetCapacity(primeNumb);
            SetTableArray(newArr);
            SetItemCount(0);
            for (int i = 0; i < oldArr.Length; i++) {
                KeyValuePair<K, V>? pair = oldArr[i];
                if (pair != null) {
                    K key = pair.Value.Key;
                    V value = pair.Value.Value;
                    InsertPair(key, value);
                }
            }
        }

    }
}