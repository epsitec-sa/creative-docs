using Epsitec.Common.Support.Extensions;

using Epsitec.Common.Types;

using System.Collections.Generic;

using System.Linq;


namespace Epsitec.Cresus.Core.Business.Finance.PriceCalculators
{


	public sealed class CodeDimension : AbstractDimension
	{
		
		
		public CodeDimension(string name, bool isNullable, IEnumerable<string> values)
			: base (name)
		{
			values.ThrowIfNull ("values");
			values.ThrowIf (e => e.Any (v => string.IsNullOrEmpty (v)), "values in values cannot be null or empty.");
			values.ThrowIf (e => e.Any (v => !v.IsAlphaNumeric ()), "values in values must be alpha numeric.");

			this.IsNullable = isNullable;

			this.values = new SortedSet<string> (values);

			this.values.ThrowIf (v => !v.Any (), "values is empty.");
		}


		public bool IsNullable
		{
			get;
			private set;
		}


		public override IEnumerable<object> Values
		{
			get
			{
				var values = this.values.Cast<object> ();

				if (this.IsNullable)
				{
					values = values.Append (CodeDimension.NullValue);
				}

				return values;
			}
		}


		public override bool IsValueDefined(object value)
		{
			value.ThrowIfNull ("value");

			return (this.IsNullable && value == CodeDimension.NullValue)
				|| ((value is string) && this.values.Contains ((string) value));
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
			string isNullable = InvariantConverter.ConvertToString (this.IsNullable);

			return isNullable + CodeDimension.valueSeparator + string.Join (CodeDimension.valueSeparator, this.values);
		}


		public static object NullValue
		{
			get
			{
				return CodeDimension.nullValue;
			}
		}
		
		
		public static CodeDimension BuildCodeDimension(string name, string stringData)
		{
			name.ThrowIfNullOrEmpty ("name");
			stringData.ThrowIfNullOrEmpty ("stringData");

			var splittedData = stringData.Split (CodeDimension.valueSeparator).ToList ();

			bool isNullable = InvariantConverter.ConvertFromString<bool> (splittedData.First ());
			var values = splittedData.Skip (1);

			return new CodeDimension (name, isNullable, values);
		}


		private SortedSet<string> values;


		private static readonly string valueSeparator = ";";


		private static readonly object nullValue = new object ();


	}


}
