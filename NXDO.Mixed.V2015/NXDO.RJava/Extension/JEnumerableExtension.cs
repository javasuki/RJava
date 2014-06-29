using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NXDO.RJava.Core;

namespace NXDO.RJava.Extension
{
    /// <summary>
    /// 扩展枚举器
    /// </summary>
    public static class JEnumerableExtension
    {
        /// <summary>
        /// 将枚举器转换成可以迭代的 java.lang.List&lt;?&gt; 接口。
        /// </summary>
        /// <typeparam name="T">迭代类型，必须是简单类型 或 java 的类型。</typeparam>
        /// <param name="enumerable">dotnet 枚举器</param>
        /// <returns>java 的 List 接口</returns>
        public static JList<T> ToJavaList<T>(this IEnumerable<T> enumerable)
        {
            if (enumerable is JavaList<T>)
                return enumerable as JavaList<T>;

            var lst = enumerable.ToList();
            var jlst = new JavaList<T>();
            foreach (T t in lst)
                jlst.Add(t);
            return jlst;
        }

        /// <summary>
        /// 将 键/值对的集合转换成 java.util.Map&lt;K, V&gt; 接口。
        /// </summary>
        /// <typeparam name="K">键的类型</typeparam>
        /// <typeparam name="V">值的类型</typeparam>
        /// <param name="dictionary">dotnet 键/值对的集合</param>
        /// <returns>java 的 Map 接口</returns>
        public static JDictionary<K, V> ToJavaDictionary<K, V>(this IDictionary<K, V> dictionary)
        {
            var jdics = new JavaDictionary<K, V>();
            foreach (var item in dictionary)
            {
                jdics.Add(item);
            }
            return jdics;
        }

        /// <summary>
        /// 将表示对象的先进先出集合转换成 java.util.Queue&lt;?&gt; 接口。
        /// </summary>
        /// <typeparam name="T">元素类型</typeparam>
        /// <param name="queue">dotnet Queue&lt;T&gt;集合</param>
        /// <returns>java 的 Queue 接口</returns>
        public static JQueue<T> ToJavaQueue<T>(this Queue<T> queue)
        {
            var jqueue = new JavaQueue<T>();
            foreach (T t in queue)
            {
                jqueue.Enqueue(t);
            }
            return jqueue;
        }

        public static JStack<T> ToJavaStack<T>(this Stack<T> stack)
        {
            var jstack = new JavaStack<T>();
            var lst = stack.ToList();
            lst.Reverse();
            foreach (T t in lst)
            {
                jstack.Add(t);
            }
            return jstack;
        }
    }
}
