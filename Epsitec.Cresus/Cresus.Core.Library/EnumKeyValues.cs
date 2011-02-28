//	Copyright © 2010-2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

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

		public abstract string[] Values
		{
			get;
		}
	}
}
