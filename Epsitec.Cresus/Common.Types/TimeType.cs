//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

[assembly: Epsitec.Common.Types.DependencyClass (typeof (Epsitec.Common.Types.TimeType))]

namespace Epsitec.Common.Types
{
	public sealed class TimeType : AbstractDateTimeType
	{
		public TimeType()
			: this ("Time")
		{
		}

		public TimeType(string name)
			: base (name)
		{
		}

		public TimeType(Caption caption)
			: base (caption)
		{
		}

		public override System.Type SystemType
		{
			get
			{
				return typeof (Time);
			}
		}

		public override bool IsNullValue(object value)
		{
			if (base.IsNullValue (value))
			{
				return true;
			}

			if ((value.GetType () == typeof (Time)) &&
				((Time) value == Time.Null))
			{
				return true;
			}

			return false;
		}

		protected override bool IsInRange(object value)
		{
			Time time = (Time) value;

			Time min = this.MinimumTime;
			Time max = this.MaximumTime;

			if (((!min.IsNull) && (time < min)) ||
				((!max.IsNull) && (time > max)))
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
