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
    }
}
