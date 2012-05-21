//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.Extensions;


namespace Epsitec.Cresus.DataLayer.Infrastructure
{
	
	
	/// <summary>
	/// The <c>UidSlot</c> defines a tuple based on inclusive minimum and maximum
	/// values, used to define a slot for the <see cref="UidGenerator"/>.
	/// </summary>
	public sealed class UidSlot
	{
		
		
		/// <summary>
		/// Initializes a new instance of the <see cref="UidSlot"/> class.
		/// </summary>
		/// <param name="minValue">The minimum (inclusive) value.</param>
		/// <param name="maxValue">The maximum (inclusive) value.</param>
		/// <exception cref="System.ArgumentException">If <paramref name="minValue"/> is lower than zero.</exception>
		/// <exception cref="System.ArgumentException">If <paramref name="maxValue"/> is lower than zero.</exception>
		/// <exception cref="System.ArgumentException">If <paramref name="minValue"/> is greater than <paramref name="maxValue"/>.</exception>		
		public UidSlot(long minValue, long maxValue)
		{
			minValue.ThrowIf (min => min < 0, "minValue cannot be lower than zero");
			minValue.ThrowIf (max => max < 0, "maxValue cannot be lower than zero");
			minValue.ThrowIf (min => min > maxValue, "minValue cannot be greater than maxValue");

			this.minValue = minValue;
			this.maxValue = maxValue;
		}


		/// <summary>
		/// The minimum (inclusive) value of the slot.
		/// </summary>
		public long MinValue
		{
			get
			{
				return this.minValue;
			}
		}


		/// <summary>
		/// The maximum (inclusive) value of the slot.
		/// </summary>
		public long MaxValue
		{
			get
			{
				return this.maxValue;
			}
		}



		private readonly long minValue;


		private readonly long maxValue;

	}


}
