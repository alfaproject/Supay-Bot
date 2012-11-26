using System;
using System.Collections.Generic;
using System.Linq;

namespace Supay.Bot
{
    internal static class LinqExtensions
    {
        /// <summary>
        /// Concatenates an element into a sequence.
        /// </summary>
        /// <remarks>
        /// This method is implemented by using deferred execution.
        /// The immediate return value is an object that stores all the information that is required to perform the action.
        /// The query represented by this method is not executed until the object is enumerated either by calling its GetEnumerator method directly or by using foreach in Visual C# or For Each in Visual Basic.
        /// </remarks>
        /// <typeparam name="TSource">The type of the elements of the sequence.</typeparam>
        /// <param name="sequence">The sequence to concatenate.</param>
        /// <param name="element">The element to concatenate to the sequence.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> that contains the concatenated elements of the sequence and element.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="sequence"/> is null.</exception>
        public static IEnumerable<TSource> Concat<TSource>(this IEnumerable<TSource> sequence, TSource element)
        {
            if (sequence == null)
            {
                throw new ArgumentNullException("sequence");
            }

            return sequence.Concat(Enumerable.Repeat(element, 1));
        }

        /// <summary>
        /// Applies an accumulator function over a sequence. The specified seed value is used as the initial
        /// accumulator value.
        /// </summary>
        /// <remarks>
        /// The <see cref="AggregateOrDefault{TSource,TAccumulate}"/> method makes it simple to perform a calculation
        /// over a sequence of values. This method works by calling <paramref name="func"/> one time for each element
        /// in <paramref name="source"/>. Each time <paramref name="func"/> is called,
        /// <see cref="AggregateOrDefault{TSource, TAccumulate}"/> passes both the element from the sequence and an
        /// aggregated value (as the first argument to <paramref name="func"/>). The value of the
        /// <paramref name="seed"/> parameter is used as the initial aggregate value. The result of
        /// <paramref name="func"/> replaces the previous aggregated value.
        /// <see cref="AggregateOrDefault{TSource, TAccumulate}"/> returns the final result of <paramref name="func"/>.
        /// </remarks>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <typeparam name="TAccumulate">The type of the accumulator value.</typeparam>
        /// <param name="source">An <see cref="IEnumerable{T}"/> to aggregate over.</param>
        /// <param name="seed">The initial accumulator value.</param>
        /// <param name="func">An accumulator function to be invoked on each element.</param>
        /// <returns>The final accumulator value or default(<typeparamref name="TAccumulate"/>) if the source is empty.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="func"/> is null.</exception>
        public static TAccumulate AggregateOrDefault<TSource, TAccumulate>(this IEnumerable<TSource> source, TAccumulate seed, Func<TAccumulate, TSource, TAccumulate> func)
        {
            if (func == null)
            {
                throw new ArgumentNullException("func");
            }

            var e = source.GetEnumerator();
            if (!e.MoveNext())
            {
                return default(TAccumulate);
            }

            var result = func(seed, e.Current);
            while (e.MoveNext())
            {
                result = func(result, e.Current);
            }
            return result;
        }

        public static int IndexOf<T>(this IEnumerable<T> source, T value, IEqualityComparer<T> comparer)
        {
            var index = 0;
            comparer = comparer ?? EqualityComparer<T>.Default;
            foreach (var item in source)
            {
                if (comparer.Equals(item, value))
                {
                    return index;
                }
                index++;
            }
            return -1;
        }

        public static int IndexOf<T>(this IEnumerable<T> source, T value)
        {
            return source.IndexOf(value, null);
        }
    }
}
