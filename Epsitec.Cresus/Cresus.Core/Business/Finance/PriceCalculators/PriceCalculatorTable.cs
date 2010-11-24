using System.Collections.Generic;

using System.Linq;


namespace Epsitec.Cresus.Core.Business.Finance.PriceCalculators
{



	internal sealed class PriceCalculatorTable
	{
		
		
		public PriceCalculatorTable(decimal defaultValue, params PriceCalculatorDimension[] dimensions)
		{
			this.DefaultValue = defaultValue;
			this.dimensions = dimensions.OrderBy (d => d.Name).ToList ();

			this.data = new Dictionary<object[], decimal> (new PriceCalculatorEqualityComparer ());
		}


		public decimal DefaultValue
		{
			get;
			private set;
		}


		public IEnumerable<PriceCalculatorDimension> Dimensions
		{
			get
			{
				return this.dimensions;
			}
		}


		public decimal this[params object[] keys]
		{
			get
			{
				decimal value;

				bool found = this.data.TryGetValue (this.GetNearestKeys (keys), out value);

				if (!found)
				{
					value = this.DefaultValue;
				}

				return value;
			}
			set
			{
				this.CheckExactKeys (keys);

				this.data[this.GetNearestKeys (keys)] = value;
			}
		}


		public bool IsNearestValueDefined(params object[] keys)
		{
			return this.data.ContainsKey (this.GetNearestKeys (keys));
		}


		public bool IsExactValueDefined(params object[] keys)
		{
			this.CheckExactKeys (keys);

			return this.data.ContainsKey (keys);
		}


		public void Clear()
		{
			this.data.Clear ();
		}


		private object[] GetNearestKeys(params object[] keys)
		{
			object[] nearestKeys = new object[keys.Length];

			for (int i = 0; i < keys.Length; i++)
			{
				nearestKeys[i] = this.dimensions[i].GetValue (keys[i]);
			}

			return nearestKeys;
		}


		private void CheckExactKeys(params object[] keys)
		{
			for (int i = 0; i < keys.Length; i++)
			{
				if (!this.dimensions[i].IsDefinedForExactValue (keys[i]))
				{
					throw new System.ArgumentException ("Invalid value for key " + i);
				}
			}
		}


		private List<PriceCalculatorDimension> dimensions;


		private IDictionary<object[], decimal> data;


	}


}
