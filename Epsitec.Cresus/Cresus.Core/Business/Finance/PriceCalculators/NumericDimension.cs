using Epsitec.Common.Support.Extensions;

using Epsitec.Common.Types;

using System.Collections.Generic;

using System.Linq;


namespace Epsitec.Cresus.Core.Business.Finance.PriceCalculators
{


	/// <summary>
	/// The <see cref="NumericDimension"/> class is an <see cref="AbstractDimension"/> whose points
	/// are numbers such as -1, 0, 1.2345, etc. Values which are between the min and the max of can
	/// be rounded given a strategy to match a defined point.
	/// </summary>
	public sealed class NumericDimension : AbstractDimension
	{


		/// <summary>
		/// Builds a new <see cref="NumericDimension"/>.
		/// </summary>
		/// <param name="name">The name of the instance.</param>
		/// <param name="values">The values that are the points of the instance.</param>
		/// <param name="roundingMode">The strategy used to round values in order to get nearest values.</param>
		/// <exception cref="System.ArgumentException">If <paramref name="name"/> is <c>null</c> or empty.</exception>
		/// <exception cref="System.ArgumentNullException">If <paramref name="values"/> is <c>null</c>.</exception>
		/// <exception cref="System.ArgumentException">If <paramref name="values"/> is empty.</exception>
		public NumericDimension(string name, IEnumerable<decimal> values, RoundingMode roundingMode)
			: base (name)
		{
			values.ThrowIfNull ("values");
			
			this.values = new SortedSet<decimal> (values);

			this.values.ThrowIf (v => !v.Any (), "values is empty.");
			
			this.RoundingMode = roundingMode;
		}


		/// <summary>
		/// Gets the set of points that defined the current instance.
		/// </summary>
		/// <value></value>
		public override IEnumerable<object> Values
		{
			get
			{
				return this.values.Cast<object> ();
			}
		}


		/// <summary>
		/// Gets the rounding strategy used to round numbers in order to obtain the nearest value of
		/// a given number.
		/// </summary>
		public RoundingMode RoundingMode
		{
			get;
			private set;
		}


		/// <summary>
		/// Tells whether the given value is a point which is exactly defined in the current instance.
		/// </summary>
		/// <param name="value">The value to check.</param>
		/// <returns><c>true</c> if the point is exactly defined, <c>false</c> if it is not. </returns>
		/// <exception cref="System.ArgumentException">If <paramref name="value"/> is <c>null</c> or invalid.</exception>
		public override bool IsValueDefined(object value)
		{
			value.ThrowIfNull ("value");
			
			return (value is decimal) && (this.values.Contains ((decimal) value));
		}


		/// <summary>
		/// Tells whether there exist a nearest point in the current instance of the given value. Such
		/// a point exists if the given value is a <see cref="System.Decimal"/> whose value is between
		/// the min and the max value of the current instance, and if the rounding strategy is not none.
		/// </summary>
		/// <param name="value">The value to check.</param>
		/// <returns><c>true</c> if there is a nearest point defined, <c>false</c> if there is not.</returns>
		/// <exception cref="System.ArgumentException">If <paramref name="value"/> is <c>null</c> or invalid.</exception>
		public override bool IsNearestValueDefined(object value)
		{
			value.ThrowIfNull ("value");
			
			bool isDefined;
			
			switch (this.RoundingMode)
			{
				case RoundingMode.None:
					isDefined = this.IsValueDefined (value);
					break;

				case RoundingMode.Down:
				case RoundingMode.Nearest:
				case RoundingMode.Up:

					if (value is decimal)
					{
						decimal decimalValue = (decimal) value;

						isDefined = (decimalValue >= this.values.Min) && (decimalValue <= this.values.Max);
					}
					else
					{
						isDefined = false;
					}

					break;

				default:
					throw new System.NotImplementedException ();
			}

			return isDefined;
		}


		/// <summary>
		/// Gets the nearest point defined in this current instance for the given value, according to
		/// the rounding strategy of this instance.
		/// </summary>
		/// <param name="value">The value whose nearest point to get.</param>
		/// <returns>The nearest point defined, if there is any.</returns>
		/// <exception cref="System.ArgumentException">If <paramref name="value"/> is <c>null</c> or invalid.</exception>
		public override object GetNearestValue(object value)
		{
			value.ThrowIfNull ("value");
			
			if (!this.IsNearestValueDefined (value))
			{
				throw new System.ArgumentException ("The given value is not defined on the current dimension.");
			}

			decimal decimalValue = (decimal) value;

			if (this.values.Contains (decimalValue))
			{
				return decimalValue;
			}
			else
			{
				return this.GetNearestElement (decimalValue);
			}
		}

		
		/// <summary>
		/// Rounds the given value to the nearest point in this instance.
		/// </summary>
		/// <param name="d">The value whose nearest value to get.</param>
		/// <returns>The nearest value to get.</returns>
		private decimal GetNearestElement(decimal d)
		{
			switch (this.RoundingMode)
			{
				case RoundingMode.None:
					return d;

				case RoundingMode.Down:
					return this.GetNearestLowerElement (d);

				case RoundingMode.Nearest:
					return this.GetNearestClosestElement (d);

				case RoundingMode.Up:
					return this.GetNearestUpperElement (d);

				default:
					throw new System.NotImplementedException ();
			}
		}


		/// <summary>
		/// Rounds the given value to the nearest point in this instance with the "down" Rounding
		/// strategy.
		/// </summary>
		/// <param name="d">The value whose nearest value to get.</param>
		/// <returns>The nearest value to get.</returns>
		private decimal GetNearestLowerElement(decimal d)
		{
			return this.values.Reverse ().First (e => e < d);
		}


		/// <summary>
		/// Rounds the given value to the nearest point in this instance with the "up" Rounding
		/// strategy.
		/// </summary>
		/// <param name="d">The value whose nearest value to get.</param>
		/// <returns>The nearest value to get.</returns>
		private decimal GetNearestUpperElement(decimal d)
		{
			return this.values.First (e => e > d);
		}


		/// <summary>
		/// Rounds the given value to the nearest point in this instance with the "closest" Rounding
		/// strategy.
		/// </summary>
		/// <param name="d">The value whose nearest value to get.</param>
		/// <returns>The nearest value to get.</returns>
		private decimal GetNearestClosestElement(decimal d)
		{
			decimal nearestLower = this.GetNearestLowerElement (d);
			decimal nearestUpper = this.GetNearestUpperElement (d);

			if (nearestUpper - d <= d - nearestLower)
			{
				return nearestUpper;
			}
			else
			{
				return nearestLower;
			}
		}


		/// <summary>
		/// Gets a <see cref="System.String"/> that contains the data that is necessary to serialize
		/// the current instance and deserialize it later.
		/// </summary>
		/// <returns> A <see cref="System.String"/> that can be used to build a clone of the current instance.</returns>
		public override string GetStringData()
		{
			string mode = System.Enum.GetName (typeof (RoundingMode), this.RoundingMode);

			var values = this.values
				.Select (v => InvariantConverter.ConvertToString (v));

			return mode + NumericDimension.valueSeparator + string.Join (NumericDimension.valueSeparator, values);
		}


		/// <summary>
		/// Builds a new instance of <see cref="NumericDimension"/> given a name and the serialized data
		/// obtained by the <see cref="NumericDimension.GetStringData"/> method.
		/// </summary>
		/// <param name="name">The name of the dimension.</param>
		/// <param name="stringData">The serialized string data.</param>
		/// <returns>The new <see cref="NumericDimension"/>.</returns>
		/// <exception cref="System.ArgumentException">If <paramref name="name"> is <c>null</c> or empty.</exception>
		/// <exception cref="System.ArgumentException">If <paramref name="stringData"> is <c>null</c>, empty or invalid.</exception>
		public static NumericDimension BuildNumericDimension(string name, string stringData)
		{
			name.ThrowIfNullOrEmpty ("name");
			stringData.ThrowIfNullOrEmpty ("stringData");

			var splittedData = stringData.Split (NumericDimension.valueSeparator).ToList ();

			var values = splittedData.Skip (1).Select (v => InvariantConverter.ConvertFromString<decimal> (v));
			var mode = (RoundingMode) System.Enum.Parse (typeof (RoundingMode), splittedData.First ());

			return new NumericDimension (name, values, mode);
		}
		
		
		/// <summary>
		/// The set of values that defines the points of the current instance.
		/// </summary>
		private SortedSet<decimal> values;


		/// <summary>
		/// The separator used to separate the different values in the serialized string data.
		/// </summary>
		private static readonly string valueSeparator = ";";


	}


}
