//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

[assembly: Epsitec.Common.Types.DependencyClass (typeof (Epsitec.Common.Types.AbstractDateTimeType))]

namespace Epsitec.Common.Types
{
	public abstract class AbstractDateTimeType : AbstractType
	{
		protected AbstractDateTimeType(string name)
			: base (name)
		{
		}

		protected AbstractDateTimeType(Caption caption)
			: base (caption)
		{
		}

		public TimeResolution Resolution
		{
			get
			{
				return (TimeResolution) this.Caption.GetValue (AbstractDateTimeType.ResolutionProperty);
			}
		}

		public Date MinimumDate
		{
			get
			{
				return (Date) this.Caption.GetValue (AbstractDateTimeType.MinimumDateProperty);
			}
		}

		public Date MaximumDate
		{
			get
			{
				return (Date) this.Caption.GetValue (AbstractDateTimeType.MaximumDateProperty);
			}
		}

		public Time MinimumTime
		{
			get
			{
				return (Time) this.Caption.GetValue (AbstractDateTimeType.MinimumTimeProperty);
			}
		}

		public Time MaximumTime
		{
			get
			{
				return (Time) this.Caption.GetValue (AbstractDateTimeType.MaximumTimeProperty);
			}
		}

		public void DefineResolution(TimeResolution resolution)
		{
			if (resolution == TimeResolution.Default)
			{
				this.Caption.ClearValue (AbstractDateTimeType.ResolutionProperty);
			}
			else
			{
				this.Caption.SetValue (AbstractDateTimeType.ResolutionProperty, resolution);
			}
		}

		public void DefineMinimumDate(Date value)
		{
			if (value == Date.Null)
			{
				this.Caption.ClearValue (AbstractDateTimeType.MinimumDateProperty);
			}
			else
			{
				this.Caption.SetValue (AbstractDateTimeType.MinimumDateProperty, value);
			}
		}

		public void DefineMaximumDate(Date value)
		{
			if (value == Date.Null)
			{
				this.Caption.ClearValue (AbstractDateTimeType.MaximumDateProperty);
			}
			else
			{
				this.Caption.SetValue (AbstractDateTimeType.MaximumDateProperty, value);
			}
		}

		public void DefineMinimumTime(Time value)
		{
			if (value == Time.Null)
			{
				this.Caption.ClearValue (AbstractDateTimeType.MinimumTimeProperty);
			}
			else
			{
				this.Caption.SetValue (AbstractDateTimeType.MinimumTimeProperty, value);
			}
		}

		public void DefineMaximumTime(Time value)
		{
			if (value == Time.Null)
			{
				this.Caption.ClearValue (AbstractDateTimeType.MaximumTimeProperty);
			}
			else
			{
				this.Caption.SetValue (AbstractDateTimeType.MaximumTimeProperty, value);
			}
		}

		public sealed override bool IsValidValue(object value)
		{
			if (this.IsNullValue (value))
			{
				return this.IsNullable;
			}

			if (value.GetType () == this.SystemType)
			{
				return this.IsInRange (value);
			}

			return false;
		}

		protected abstract bool IsInRange(object value);

		public static readonly DependencyProperty ResolutionProperty = DependencyProperty.RegisterAttached ("Resolution", typeof (TimeResolution), typeof (AbstractDateTimeType), new DependencyPropertyMetadata (TimeResolution.Default));
		public static readonly DependencyProperty MinimumDateProperty = DependencyProperty.RegisterAttached ("MinDate", typeof (Date), typeof (AbstractDateTimeType), new DependencyPropertyMetadata (Date.Null));
		public static readonly DependencyProperty MaximumDateProperty = DependencyProperty.RegisterAttached ("MaxDate", typeof (Date), typeof (AbstractDateTimeType), new DependencyPropertyMetadata (Date.Null));
		public static readonly DependencyProperty MinimumTimeProperty = DependencyProperty.RegisterAttached ("MinTime", typeof (Time), typeof (AbstractDateTimeType), new DependencyPropertyMetadata (Time.Null));
		public static readonly DependencyProperty MaximumTimeProperty = DependencyProperty.RegisterAttached ("MaxTime", typeof (Time), typeof (AbstractDateTimeType), new DependencyPropertyMetadata (Time.Null));
	}
}
