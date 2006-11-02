//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

[assembly: Epsitec.Common.Types.DependencyClass (typeof (Epsitec.Common.Types.DateTimeType))]

namespace Epsitec.Common.Types
{
	public sealed class DateTimeType : AbstractDateTimeType
	{
		public DateTimeType()
			: this ("DateTime")
		{
		}

		public DateTimeType(string name)
			: base (name)
		{
		}

		public DateTimeType(Caption caption)
			: base (caption)
		{
		}


		public override System.Type SystemType
		{
			get
			{
				return typeof (System.DateTime);
			}
		}

		protected override bool IsInRange(object value)
		{
			System.DateTime dateTime = (System.DateTime) value;

			Date date = new Date (dateTime);
			Time time = new Time (dateTime);

			Date minDate = this.MinimumDate;
			Date maxDate = this.MaximumDate;

			if (((!minDate.IsNull) && (date < minDate)) ||
				((!maxDate.IsNull) && (date > maxDate)))
			{
				return false;
			}
			
			Time minTime = this.MinimumTime;
			Time maxTime = this.MaximumTime;

			if (((!minTime.IsNull) && (time < minTime)) ||
				((!maxTime.IsNull) && (time > maxTime)))
			{
				return false;
			}

			return true;
		}
	}
}
