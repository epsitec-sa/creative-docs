//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

[assembly: Epsitec.Common.Types.DependencyClass (typeof (Epsitec.Common.Types.DateType))]

namespace Epsitec.Common.Types
{
	public sealed class DateType : AbstractDateTimeType
	{
		public DateType()
			: this ("Date")
		{
		}

		public DateType(string name)
			: base (name)
		{
		}

		public DateType(Caption caption)
			: base (caption)
		{
		}


		public override System.Type SystemType
		{
			get
			{
				return typeof (Date);
			}
		}

		public override bool IsNullValue(object value)
		{
			if (base.IsNullValue (value))
			{
				return true;
			}

			if ((value.GetType () == typeof (Date)) &&
				((Date) value == Date.Null))
			{
				return true;
			}
			
			return false;
		}
		
		protected override bool IsInRange(object value)
		{
			Date date = (Date) value;

			Date min = this.MinimumDate;
			Date max = this.MaximumDate;

			if (((!min.IsNull) && (date < min)) ||
				((!max.IsNull) && (date > max)))
			{
				return false;
			}
			else
			{
				return true;
			}
		}
	}
}
