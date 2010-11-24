using System.Collections.Generic;

using System.Linq;


namespace Epsitec.Cresus.Core.Business.Finance.PriceCalculators
{


	internal sealed class PriceCalculatorNumericDimension : PriceCalculatorDimension<decimal>
	{


		public PriceCalculatorNumericDimension(string name, IEnumerable<decimal> values, RoundingMode roundingMode)
			: base (name, values)
		{
			this.RoundingMode = roundingMode;
		}


		public RoundingMode RoundingMode
		{
			get;
			private set;
		}


		public override decimal GetGenericValue(decimal d)
		{
			if (this.InternalValues.Contains (d))
			{
				return d;
			}
			else if (d < this.InternalValues.Min)
			{
				throw new System.ArgumentException ("Value " + d + " is lower than the minimum value.");
			}
			else if (d > this.InternalValues.Max)
			{
				throw new System.ArgumentException ("Value " + d + " is greater than the minimum value.");
			}
			else
			{
				return this.GetNearestElement (d);
			}
		}

		
		private decimal GetNearestElement(decimal d)
		{
			switch (this.RoundingMode)
			{
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
			return this.InternalValues.Reverse ().First (e => e < d);
		}

		
		private decimal GetNearestUpperElement(decimal d)
		{
			return this.InternalValues.First (e => e > d);
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


	}


}
