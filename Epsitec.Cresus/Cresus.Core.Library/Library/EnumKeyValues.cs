//	Copyright © 2010-2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Library
{
	/// <summary>
	/// The <c>EnumKeyValues</c> class is used to store an <c>enum</c> key
	/// and associated texts which represent its value. See the generic
	/// type <c>EnumKeyValues{T}</c> for a concrete implementation.
	/// </summary>
	public abstract class EnumKeyValues
	{
		public static EnumKeyValues<T> Create<T>(T key, params string[] values)
		{
			return new EnumKeyValues<T> (key, values);
		}

		public static IEnumerable<EnumKeyValues<T>> FromEnum<T>()
			where T : struct
		{
			var typeObject = TypeRosetta.GetTypeObject (typeof (T));
			var enumType   = typeObject as EnumType;

			if (enumType == null)
			{
				yield break;
			}

			foreach (var item in enumType.Values)
			{
				var enumKey = (T) (object) item.Value;
				var caption = item.Caption;

				yield return EnumKeyValues.Create<T> (enumKey, caption.Labels.ToArray ());
			}
		}

		public abstract FormattedText[] Values
		{
			get;
		}
	}
}
