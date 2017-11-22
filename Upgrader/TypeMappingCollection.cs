using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Upgrader.Infrastructure;

namespace Upgrader
{
    /// <inheritdoc />
    /// <summary>
    /// Mapping between CLR types and SQL data types.
    /// </summary>
    public class TypeMappingCollection : IEnumerable<KeyValuePair<Type, string>>
    {
        private readonly Dictionary<Type, string> mapping = new Dictionary<Type, string>();

        /// <summary>
        /// Gets or sets an association between a CLR type and a SQL data type.
        /// </summary>
        /// <param name="type">CLR type.</param>
        /// <returns>SQL Data type.</returns>
        public string this[Type type]
        {
            get
            {
                Validate.IsNotNull(type, nameof(type));
                Validate.IsNotNullable(type, nameof(type));

                string dataType;
                return mapping.TryGetValue(type, out dataType) ? dataType : null;
            }

            set
            {
                Validate.IsNotNull(type, nameof(type));
                Validate.IsNotNullable(type, nameof(type));

                Validate.IsNotNullAndNotEmpty(value, nameof(value));

                mapping[type] = value;
            }
        }

        /// <summary>
        /// Adds a new mapping between a CLR type and a data type.
        /// </summary>
        /// <param name="type">CLR type.</param>
        /// <param name="dataType">Data type.</param>
        public void Add(Type type, string dataType)
        {
            Validate.IsNotNull(type, nameof(type));
            Validate.IsNotNullable(type, nameof(type));
            Validate.IsNotNullAndNotEmpty(dataType, nameof(dataType));

            mapping.Add(type, dataType);
        }

        /// <summary>
        /// Adds a new mapping.
        /// </summary>
        /// <param name="dataType">Data type.</param>
        /// <typeparam name="TType">CLR type.</typeparam>
        public void Add<TType>(string dataType)
        {
            Validate.IsNotNullable(typeof(TType), null);
            Validate.IsNotNullAndNotEmpty(dataType, nameof(dataType));

            mapping.Add(typeof(TType), dataType);
        }

        /// <summary>
        /// Removes a mapping.
        /// </summary>
        /// <typeparam name="TType">CLR type.</typeparam>
        public void Remove<TType>()
        {
            mapping.Remove(typeof(TType));
        }

        /// <summary>
        /// Removes a mapping.
        /// </summary>
        /// <param name="type">CLR type.</param>
        public void Remove(Type type)
        {
            Validate.IsNotNull(type, nameof(type));

            mapping.Remove(type);
        }

        public IEnumerator<KeyValuePair<Type, string>> GetEnumerator()
        {
            return mapping.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return mapping.GetEnumerator();
        }

        internal string GetDataType(Type type, params int[] parameters)
        {
            var nonNullableType = Nullable.GetUnderlyingType(type) ?? type;

            var mappedType = this[nonNullableType];
            if (mappedType == null)
            {
                return null;
            }

            if (parameters.Length == 0)
            {
                return mappedType;
            }

            var parameterIndex = mappedType.IndexOf('(');
            var mappedTypeWithoutParameters = parameterIndex == -1 ? mappedType : mappedType.Substring(0, parameterIndex);
            var joinedParameters = string.Join(",", parameters.Select(parameter => parameter.ToString()));

            return $"{mappedTypeWithoutParameters}({joinedParameters})";
        }
    }
}