using Epsitec.Common.Support.Extensions;

using Epsitec.Common.Types;

using System.Collections.Generic;

using System.Linq;


namespace Epsitec.Cresus.Core.Business.Finance.PriceCalculators
{


	public sealed class CodeDimension : AbstractDimension
	{
		
		
		public CodeDimension(string name, IEnumerable<string> values)
			: base (name)
		{
			values.ThrowIfNull ("values");
			
			this.values = new SortedSet<string> (values);

			this.values.ThrowIf (v => !v.Any (), "values is empty.");
			this.values.ThrowIf (e => e.Any (v => string.IsNullOrEmpty (v)), "values in values cannot be null or empty.");
			this.values.ThrowIf (e => e.Any (v => !v.IsAlphaNumeric ()), "values in values must be alpha numeric.");
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
			return this.IsValueDefined (value);
		}


		public override object GetNearestValue(object value)
		{
			if (!this.IsValueDefined (value))
			{
				throw new System.ArgumentException ("The given value is not defined on the current dimension.");
			}

			return value;
		}


		public override string GetStringData()
		{
			return string.Join (CodeDimension.valueSeparator, this.values);
		}
		
		
		public static CodeDimension BuildCodeDimension(string name, string stringData)
		{
			name.ThrowIfNullOrEmpty ("name");
			stringData.ThrowIfNullOrEmpty ("stringData");

			var values = stringData.Split (CodeDimension.valueSeparator);

			return new CodeDimension (name, values);
		}


		private SortedSet<string> values;


		private static readonly string valueSeparator = ";";


	}


}
