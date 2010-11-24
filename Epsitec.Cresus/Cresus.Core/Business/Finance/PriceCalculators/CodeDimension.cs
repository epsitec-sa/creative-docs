using Epsitec.Common.Support;
using Epsitec.Common.Support.Extensions;

using System.Collections.Generic;

using System.Linq;


namespace Epsitec.Cresus.Core.Business.Finance.PriceCalculators
{


	 internal sealed class CodeDimension : AbstractDimension
	{


		 public CodeDimension(string name, IEnumerable<string> values)
			 : base (name)
		 {
			 values.ThrowIfNull ("values");
			 values.ThrowIf (e => e.Any (v => string.IsNullOrEmpty (v)), "values in values cannot be null or empty.");
			 values.ThrowIf (e => e.Any (v => !RegexFactory.AlphaNumName.IsMatch (v)), "values in values must be alpha numeric.");

			 this.values = new SortedSet<string> (values);

			 this.values.ThrowIf (v => !v.Any (), "values is empty.");
		 }


		public override IEnumerable<object> Values
		{
			get
			{
				return this.values.Cast<object> ();
			}
		}


		public override bool IsValueDefined(object value)
		{
			value.ThrowIfNull ("value");

			return (value is string) && this.values.Contains ((string) value);
		}


		public override bool IsNearestValueDefined(object value)
		{
			value.ThrowIfNull ("value");

			return this.IsValueDefined (value);
		}


		public override object GetNearestValue(object value)
		{
			value.ThrowIfNull ("value");

			if (!this.IsNearestValueDefined (value))
			{
				throw new System.ArgumentException ("The given value is not defined on the current dimension.");
			}

			return value;
		}


		private SortedSet<string> values;


	}


}
