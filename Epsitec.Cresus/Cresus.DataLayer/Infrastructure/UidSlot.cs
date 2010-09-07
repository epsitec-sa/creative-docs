//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Cresus.Database;
using System.Collections.Generic;

namespace Epsitec.Cresus.DataLayer.Infrastructure
{
	/// <summary>
	/// The <c>UidSlot</c> defines a tuple based on inclusive minimum and maximum
	/// values, used to define a slot for the <see cref="UidGenerator"/>.
	/// </summary>
	public class UidSlot : System.Tuple<long, long>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="UidSlot"/> class.
		/// </summary>
		/// <param name="minValue">The minimum (inclusive) value.</param>
		/// <param name="maxValue">The maximum (inclusive) value.</param>
		public UidSlot(long minValue, long maxValue)
			: base (minValue, maxValue)
		{
		}

		public long MinValue
		{
			get
			{
				return this.Item1;
			}
		}

		public long MaxValue
		{
			get
			{
				return this.Item2;
			}
		}
	}
}
