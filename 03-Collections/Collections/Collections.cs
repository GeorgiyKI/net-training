using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Collections.Tasks
{

    /// <summary>
    ///  Tree node item 
    /// </summary>
    /// <typeparam name="T">the type of tree node data</typeparam>
    public interface ITreeNode<T>
    {
        T Data { get; set; }                             // Custom data
        IEnumerable<ITreeNode<T>> Children { get; set; } // List of childrens
    }

    public class Task
    {

        /// <summary> Generate the Fibonacci sequence f(x) = f(x-1)+f(x-2) </summary>
        /// <param name="count">the size of a required sequence</param>
        /// <returns>
        ///   Returns the Fibonacci sequence of required count
        /// </returns>
        /// <exception cref="System.InvalidArgumentException">count is less then 0</exception>
        /// <example>
        ///   0 => { }  
        ///   1 => { 1 }    
        ///   2 => { 1, 1 }
        ///   12 => { 1, 1, 2, 3, 5, 8, 13, 21, 34, 55, 89, 144 }
        /// </example>
        public static IEnumerable<int> GetFibonacciSequence(int count)
        {
            if (count < 0)
                throw new ArgumentException($"{nameof(count)} can't be less then 0");
            
            int firstFebNumber = 0;
            int secondFebNumber = 1;
            for (int i = 0; i < count; i++)
            {
                yield return secondFebNumber;
                int temp = secondFebNumber;
                secondFebNumber += firstFebNumber;
                firstFebNumber = temp;
            }
        }

        /// <summary>
        ///    Parses the input string sequence into words
        /// </summary>
        /// <param name="reader">input string sequence</param>
        /// <returns>
        ///   The enumerable of all words from input string sequence. 
        /// </returns>
        /// <exception cref="System.ArgumentNullException">reader is null</exception>
        /// <example>
        ///  "TextReader is the abstract base class of StreamReader and StringReader, which ..." => 
        ///   {"TextReader","is","the","abstract","base","class","of","StreamReader","and","StringReader","which",...}
        /// </example>
        public static IEnumerable<string> Tokenize(TextReader reader)
        {
            if (reader == null)
                throw new ArgumentNullException(nameof(reader));

            char[] delimeters = new[] { ',', ' ', '.', '\t', '\n' };

            string line;
            while ((line = reader.ReadLine()) != null)
            {
                foreach (var text in line.Split(delimeters, StringSplitOptions.RemoveEmptyEntries))
                    yield return text;
            }
        }

        /// <summary>
        ///   Traverses a tree using the depth-first strategy
        /// </summary>
        /// <typeparam name="T">tree node type</typeparam>
        /// <param name="root">the tree root</param>
        /// <returns>
        ///   Returns the sequence of all tree node data in depth-first order
        /// </returns>
        /// <example>
        ///    source tree (root = 1):
        ///    
        ///                      1
        ///                    / | \
        ///                   2  6  7
        ///                  / \     \
        ///                 3   4     8
        ///                     |
        ///                     5   
        ///                   
        ///    result = { 1, 2, 3, 4, 5, 6, 7, 8 } 
        /// </example>
        public static IEnumerable<T> DepthTraversalTree<T>(ITreeNode<T> root)
        {
            if (root == null)
                throw new ArgumentNullException(nameof(root));

            var stackTreeNode = new Stack<ITreeNode<T>>();
            stackTreeNode.Push(root);

            while (stackTreeNode.Count != 0)
            {
                root = stackTreeNode.Pop();
                yield return root.Data;

                if (root.Children != null)
                    foreach (var item in root.Children.Reverse())
                        stackTreeNode.Push(item);                
            };
        }

        /// <summary>
        ///   Traverses a tree using the width-first strategy
        /// </summary>
        /// <typeparam name="T">tree node type</typeparam>
        /// <param name="root">the tree root</param>
        /// <returns>
        ///   Returns the sequence of all tree node data in width-first order
        /// </returns>
        /// <example>
        ///    source tree (root = 1):
        ///    
        ///                      1
        ///                    / | \
        ///                   2  3  4
        ///                  / \     \
        ///                 5   6     7
        ///                     |
        ///                     8   
        ///                   
        ///    result = { 1, 2, 3, 4, 5, 6, 7, 8 } 
        /// </example>
        public static IEnumerable<T> WidthTraversalTree<T>(ITreeNode<T> root)
        {
            if (root == null)
                throw new ArgumentNullException(nameof(root));

            var queueTreeNode = new Queue<ITreeNode<T>>();
            queueTreeNode.Enqueue(root);

            while (queueTreeNode.Count != 0)
            {
                root = queueTreeNode.Dequeue();
                yield return root.Data;

                if (root.Children != null)
                    foreach (var children in root.Children)
                        queueTreeNode.Enqueue(children);
            };
        }

        /// <summary>
        ///   Generates all permutations of specified length from source array
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">source array</param>
        /// <param name="count">permutation length</param>
        /// <returns>
        ///    All permuations of specified length
        /// </returns>
        /// <exception cref="System.InvalidArgumentException">count is less then 0 or greater then the source length</exception>
        /// <example>
        ///   source = { 1,2,3,4 }, count=1 => {{1},{2},{3},{4}}
        ///   source = { 1,2,3,4 }, count=2 => {{1,2},{1,3},{1,4},{2,3},{2,4},{3,4}}
        ///   source = { 1,2,3,4 }, count=3 => {{1,2,3},{1,2,4},{1,3,4},{2,3,4}}
        ///   source = { 1,2,3,4 }, count=4 => {{1,2,3,4}}
        ///   source = { 1,2,3,4 }, count=5 => ArgumentOutOfRangeException
        /// </example>
        public static IEnumerable<T[]> GenerateAllPermutations<T>(T[] source, int count)
        {
            var curr = new HashSet<T>(source);
            var list = new List<HashSet<T>>();

            foreach (var s in GetAllPermutationsRec(count, curr))
            {
                if (HashContains(list, s))
                    list.Add(new HashSet<T>(s.Reverse()));
            }

            foreach (var item in list)
            {
                yield return item.ToArray();
            }
        }

        static bool HashContains<T>(List<HashSet<T>> list, HashSet<T> hash)
        {
            foreach (var item in list)
                if (item.IsSupersetOf(hash))
                    return false;

            return true;
        }

        static IEnumerable<HashSet<T>> GetAllPermutationsRec<T>(int n, HashSet<T> curr)
        {
            if (n > 0)
            {
                foreach (var c in curr.ToList())
                {
                    curr.Remove(c);
                    foreach (var s in GetAllPermutationsRec(n - 1, curr))
                    {
                        s.Add(c);
                        yield return s;
                    }
                    curr.Add(c);
                }
            }
            else if (n == 0)
            {
                yield return new HashSet<T>();
            }
        }
    }

    public static class DictionaryExtentions
    {

        /// <summary>
        ///    Gets a value from the dictionary cache or build new value
        /// </summary>
        /// <typeparam name="TKey">TKey</typeparam>
        /// <typeparam name="TValue">TValue</typeparam>
        /// <param name="dictionary">source dictionary</param>
        /// <param name="key">key</param>
        /// <param name="builder">builder function to build new value if key does not exist</param>
        /// <returns>
        ///   Returns a value assosiated with the specified key from the dictionary cache. 
        ///   If key does not exist than builds a new value using specifyed builder, puts the result into the cache 
        ///   and returns the result.
        /// </returns>
        /// <example>
        ///   IDictionary<int, Person> cache = new SortedDictionary<int, Person>();
        ///   Person value = cache.GetOrBuildValue(10, ()=>LoadPersonById(10) );  // should return a loaded Person and put it into the cache
        ///   Person cached = cache.GetOrBuildValue(10, ()=>LoadPersonById(10) );  // should get a Person from the cache
        /// </example>
        public static TValue GetOrBuildValue<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, Func<TValue> builder)
        {
            if (!dictionary.ContainsKey(key))
                dictionary.Add(key, builder.Invoke());

            return dictionary[key];
        }
    }
}
