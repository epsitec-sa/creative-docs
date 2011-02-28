//	Copyright © 2010-2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Library
{
	/// <summary>
	/// The <c>EnumKeyValue{T}</c> class is used to store an <c>enum</c> key
	/// and associated texts which represent its value.
	/// </summary>
	/// <typeparam name="T">The enumeration type.</typeparam>
	public class EnumKeyValues<T> : EnumKeyValues
	{
		public EnumKeyValues(T key, params string[] values)
		{
			this.key = key;
			this.values = values;
		}


		/// <summary>
		/// Gets the key which is an <c>enum</c> value.
		/// </summary>
		/// <value>The key.</value>
		public T Key
		{
			get
			{
				return this.key;
			}
		}

		/// <summary>
		/// Gets the values for the key.
		/// </summary>
		/// <value>The values.</value>
		public override string[] Values
		{
			get
			{
				return this.values;
			}
		}


		private readonly T key;
		private readonly string[] values;
	}
}
