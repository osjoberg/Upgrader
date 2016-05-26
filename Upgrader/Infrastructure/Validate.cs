using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Upgrader.Infrastructure
{
    internal static class Validate
    {
        public static void IsTrue(bool argument, string argumentName, string message) 
        {
            if (argument == false)
            {
                throw new ArgumentException(message, argumentName);
            }
        }

        public static void IsNotEmpty<T>(T argument, string argumentName) where T : IEnumerable
        {
            if (argument == null)
            {
                return;
            }

            if (argument.GetEnumerator().MoveNext() == false)
            {
                throw new ArgumentException("Value cannot be empty.", argumentName);
            }
        }

        public static void IsNotNullAndNotEmpty<T>(T argument, string argumentName) where T : class, IEnumerable
        {
            IsNotNull(argument, argumentName);
            IsNotEmpty(argument, argumentName);
        }

        public static void IsNotNullAndNotEmptyEnumerable<T>(IEnumerable<T> argument, string argumentName) where T : class, IEnumerable
        {
            var argumentShallowClone = argument.ToArray();

            IsNotNull(argumentShallowClone, argumentName);
            IsNotEmpty(argumentShallowClone, argumentName);

            foreach (var item in argumentShallowClone)
            {
                IsNotNullAndNotEmpty(item, argumentName);
            }
        }

        public static void IsNotNull<T>(T argument, string argumentName) where T : class 
        {
            if (argument == null)
            {
                throw new ArgumentNullException(argumentName);
            }
        }
    }
}
