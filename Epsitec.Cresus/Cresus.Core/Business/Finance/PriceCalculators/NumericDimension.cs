using Epsitec.Common.Support.Extensions;

using Epsitec.Common.Types;

using System.Collections.Generic;

using System.Linq;


namespace Epsitec.Cresus.Core.Business.Finance.PriceCalculators
{


	internal sealed class NumericDimension : AbstractDimension
	{


		public NumericDimension(string name, IEnumerable<decimal> values, RoundingMode roundingMode)
			: base (name)
		{
			values.ThrowIfNull ("values");
			
			this.values = new SortedSet<decimal> (values);

			this.values.ThrowIf (v => !v.Any (), "values is empty.");
			
			this.RoundingMode = roundingMode;
		}


		public override IEnumerable<object> Values
		{
			get
			{
				return this.values.Cast<object> ();
			}
		}


		public RoundingMode RoundingMode
		{
			get;
			private set;
		}


		public override bool IsValueDefined(object value)
		{
			value.ThrowIfNull ("value");
			
			return (value is decimal) && (this.values.Contains ((decimal) value));
		}


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


		private decimal GetNearestLowerElement(decimal d)
		{
			return this.values.Reverse ().First (e => e < d);
		}

		
		private decimal GetNearestUpperElement(decimal d)
		{
			return this.values.First (e => e > d);
		}


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


		public override string GetStringData()
		{
			string mode = System.Enum.GetName (typeof (RoundingMode), this.RoundingMode);

			var values = this.values
				.Select (v => InvariantConverter.ConvertToString (v));

			return mode + NumericDimension.valueSeparator + string.Join (NumericDimension.valueSeparator, values);
		}


		public static NumericDimension BuildNumericDimension(string name, string stringData)
		{
			var splittedData = stringData.Split (NumericDimension.valueSeparator).ToList ();

			var values = splittedData.Skip (1).Select (v => InvariantConverter.ConvertFromString<decimal> (v));
			var mode = (RoundingMode) System.Enum.Parse (typeof (RoundingMode), splittedData.First ());

			return new NumericDimension (name, values, mode);
		}
		
		
		private SortedSet<decimal> values;


		private static readonly string valueSeparator = ";";


	}


}
