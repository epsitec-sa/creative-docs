/*
This file is part of CreativeDocs.

Copyright © 2003-2024, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland

CreativeDocs is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
any later version.

CreativeDocs is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/


using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Support.EntityEngine;

namespace Epsitec.Common.Support.Extensions
{
    public static class TypeExtensions
    {
        public static bool IsEntity(this System.Type type)
        {
            return (type.IsClass) && (typeof(AbstractEntity).IsAssignableFrom(type));
        }

        /// <summary>
        /// Determines whether the specified type is a static class.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>
        ///   <c>true</c> if the specified type is a static class; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsStaticClass(this System.Type type)
        {
            return type.IsClass && type.IsSealed && type.IsAbstract;
        }

        /// <summary>
        /// Determines whether the type is a nullable type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>
        ///   <c>true</c> if the type is a nullable type; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsNullable(this System.Type type)
        {
            return (type.IsGenericType)
                && (type.GetGenericTypeDefinition() == typeof(System.Nullable<>));
        }

        public static bool IsGenericIList(this System.Type type)
        {
            return (type.IsGenericType)
                && (type.GetGenericTypeDefinition() == typeof(System.Collections.Generic.IList<>));
        }

        public static bool IsGenericIListOfEntities(this System.Type type)
        {
            if (type.IsGenericIList())
            {
                return type.GetGenericArguments()[0].IsEntity();
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Gets the underlying type of a nullable type.
        /// </summary>
        /// <param name="type">The nullable type.</param>
        /// <returns>The underlying type if the specified type is a nullable type; otherwise, <c>null</c>.</returns>
        public static System.Type GetNullableTypeUnderlyingType(this System.Type type)
        {
            if (
                (type.IsGenericType)
                && (type.GetGenericTypeDefinition() == typeof(System.Nullable<>))
            )
            {
                return type.GetGenericArguments()[0];
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Gets the custom attributes of the specified type.
        /// </summary>
        /// <typeparam name="T">The type of the attributes.</typeparam>
        /// <param name="type">The type.</param>
        /// <param name="inherit">if set to <c>true</c>, the attributes will be inherited from the base types too; default to no inheritance.</param>
        /// <returns>The collection of custom attributes of the specified type.</returns>
        public static IEnumerable<T> GetCustomAttributes<T>(
            this System.Type type,
            bool inherit = false
        )
            where T : System.Attribute
        {
            return type.GetCustomAttributes(typeof(T), inherit).Cast<T>();
        }

        /// <summary>
        /// Determines whether the type contains the specified interface.
        /// </summary>
        /// <typeparam name="T">The type of the interface.</typeparam>
        /// <param name="type">The type.</param>
        /// <returns>
        ///   <c>true</c> if the type contains the specified interface; otherwise, <c>false</c>.
        /// </returns>
        public static bool ContainsInterface<T>(this System.Type type)
        {
            return typeof(T).IsAssignableFrom(type);

            //	Note: IsAssignableFrom is 3x faster than the following piece of code:

            //			return type.GetInterfaces ().Contains (typeof (T));
        }

        public static bool ContainsInterface(this System.Type type, System.Type interfaceType)
        {
            return interfaceType.IsAssignableFrom(type);
        }

        /// <summary>
        /// Gets all the base types of the given one, including the given type.
        /// </summary>
        public static IEnumerable<System.Type> GetBaseTypes(this System.Type type)
        {
            while (type != null)
            {
                yield return type;

                type = type.BaseType;
            }
        }
    }
}
