using Epsitec.Common.Support.Extensions;

using System.Collections;
using System.Collections.Generic;


namespace Epsitec.Cresus.Core.Business.Finance.PriceCalculators
{


	internal abstract class PriceCalculatorDimension
	{

		
		public PriceCalculatorDimension(string name)
		{
			name.ThrowIfNullOrEmpty ("name");

			this.Name = name;
		}


		public string Name
		{
			get;
			private set;
		}


		public abstract IEnumerable Values
		{
			get;
		}


		public abstract object GetValue(object value);


		public abstract bool IsDefinedForExactValue(object value);


	}


	
	internal class PriceCalculatorDimension<T> : PriceCalculatorDimension
	{


		public PriceCalculatorDimension(string name, IEnumerable<T> values)
			: base (name)
		{
			values.ThrowIfNull ("values");

			this.InternalValues = new SortedSet<T> (values);
		}


		public override IEnumerable Values
		{
			get
			{
				return this.GenericValues;
			}
		}


		public IEnumerable<T> GenericValues
		{
			get
			{
				return this.InternalValues;
			}
		}


		protected SortedSet<T> InternalValues
		{
			get;
			private set;
		}


		public override object GetValue(object value)
		{
			return this.GetGenericValue ((T) value);
		}


		public override bool IsDefinedForExactValue(object value)
		{
			return this.IsDefinedForExactGenericValue ((T) value);
		}


		public bool IsDefinedForExactGenericValue(T value)
		{
			return this.InternalValues.Contains (value);
		}


		public virtual T GetGenericValue(T t)
		{
			if (!this.InternalValues.Contains (t))
			{
				throw new System.ArgumentException ("Value " + t + " is not defined.");
			}

			return t;
		}


	}


}
