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
	}
}
