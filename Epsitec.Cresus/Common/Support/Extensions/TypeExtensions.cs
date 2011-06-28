//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.EntityEngine;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Common.Support.Extensions
{
	public static class TypeExtensions
	{
		public static bool IsEntity(this System.Type type)
		{
			return (type.IsClass) && (typeof (AbstractEntity).IsAssignableFrom (type));
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
			return (type.IsGenericType) && (type.GetGenericTypeDefinition () == typeof (System.Nullable<>));
		}

		public static bool IsGenericIList(this System.Type type)
		{
			return (type.IsGenericType) && (type.GetGenericTypeDefinition () == typeof (System.Collections.Generic.IList<>));
		}

		public static bool IsGenericIListOfEntities(this System.Type type)
		{
			if (type.IsGenericIList ())
			{
				return type.GetGenericArguments ()[0].IsEntity ();
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
			if ((type.IsGenericType) &&
				(type.GetGenericTypeDefinition () == typeof (System.Nullable<>)))
			{
				return type.GetGenericArguments ()[0];
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
		public static IEnumerable<T> GetCustomAttributes<T>(this System.Type type, bool inherit = false)
			where T : System.Attribute
		{
			return type.GetCustomAttributes (typeof (T), inherit).Cast<T> ();
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
			return type.GetInterfaces ().Contains (typeof (T));
		}
	}
}
