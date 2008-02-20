//	Copyright © 2006-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

[assembly: Epsitec.Common.Types.DependencyClass (typeof (Epsitec.Common.Types.AbstractDateTimeType))]

namespace Epsitec.Common.Types
{
	/// <summary>
	/// The <c>AbstractDateTimeType</c> class is the base class for the <c>DateType</c>,
	/// <c>TimeType</c> and <c>DateTimeType</c> classes.
	/// </summary>
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

		/// <summary>
		/// Gets the date or time resolution which may affect how the data is
		/// stored and represented.
		/// </summary>
		/// <value>The date or time resolution.</value>
		public TimeResolution Resolution
		{
			get
			{
				return (TimeResolution) this.Caption.GetValue (AbstractDateTimeType.ResolutionProperty);
			}
		}

		/// <summary>
		/// Gets the minimum valid date.
		/// </summary>
		/// <value>The minimum date.</value>
		public Date MinimumDate
		{
			get
			{
				return (Date) this.Caption.GetValue (AbstractDateTimeType.MinimumDateProperty);
			}
		}

		/// <summary>
		/// Gets the maximum valid date.
		/// </summary>
		/// <value>The maximum date.</value>
		public Date MaximumDate
		{
			get
			{
				return (Date) this.Caption.GetValue (AbstractDateTimeType.MaximumDateProperty);
			}
		}

		/// <summary>
		/// Gets the minimum valid time.
		/// </summary>
		/// <value>The minimum time.</value>
		public Time MinimumTime
		{
			get
			{
				return (Time) this.Caption.GetValue (AbstractDateTimeType.MinimumTimeProperty);
			}
		}

		/// <summary>
		/// Gets the maximum valid time.
		/// </summary>
		/// <value>The maximum time.</value>
		public Time MaximumTime
		{
			get
			{
				return (Time) this.Caption.GetValue (AbstractDateTimeType.MaximumTimeProperty);
			}
		}

		/// <summary>
		/// Gets the time increment.
		/// </summary>
		/// <value>The time increment.</value>
		public System.TimeSpan TimeStep
		{
			get
			{
				return (System.TimeSpan) this.Caption.GetValue (AbstractDateTimeType.TimeStepProperty);
			}
		}

		/// <summary>
		/// Gets the date increment.
		/// </summary>
		/// <value>The date increment.</value>
		public DateSpan DateStep
		{
			get
			{
				return (DateSpan) this.Caption.GetValue (AbstractDateTimeType.DateStepProperty);
			}
		}

		/// <summary>
		/// Defines the date or time resolution.
		/// </summary>
		/// <param name="resolution">The resolution.</param>
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

		/// <summary>
		/// Defines the minimum valid date.
		/// </summary>
		/// <param name="value">The value.</param>
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

		/// <summary>
		/// Defines the maximum valid date.
		/// </summary>
		/// <param name="value">The value.</param>
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

		/// <summary>
		/// Defines the minimum valid time.
		/// </summary>
		/// <param name="value">The value.</param>
		public void DefineMinimumTime(Time value)
		{
			if (value.IsNull)
			{
				this.Caption.ClearValue (AbstractDateTimeType.MinimumTimeProperty);
			}
			else
			{
				this.Caption.SetValue (AbstractDateTimeType.MinimumTimeProperty, value);
			}
		}

		/// <summary>
		/// Defines the maximum valid time.
		/// </summary>
		/// <param name="value">The value.</param>
		public void DefineMaximumTime(Time value)
		{
			if (value.IsNull)
			{
				this.Caption.ClearValue (AbstractDateTimeType.MaximumTimeProperty);
			}
			else
			{
				this.Caption.SetValue (AbstractDateTimeType.MaximumTimeProperty, value);
			}
		}

		/// <summary>
		/// Defines the time increment.
		/// </summary>
		/// <param name="value">The value.</param>
		public void DefineTimeStep(System.TimeSpan value)
		{
			if (value.TotalSeconds == 1.0)
			{
				this.Caption.ClearValue (AbstractDateTimeType.TimeStepProperty);
			}
			else
			{
				this.Caption.SetValue (AbstractDateTimeType.TimeStepProperty, value);
			}
		}

		/// <summary>
		/// Defines the date increment.
		/// </summary>
		/// <param name="value">The value.</param>
		public void DefineDateStep(DateSpan value)
		{
			if (value == new DateSpan (1))
			{
				this.Caption.ClearValue (AbstractDateTimeType.DateStepProperty);
			}
			else
			{
				this.Caption.SetValue (AbstractDateTimeType.DateStepProperty, value);
			}
		}

		/// <summary>
		/// Determines whether the specified value is valid according to the
		/// constraint.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns>
		/// 	<c>true</c> if the specified value is valid; otherwise, <c>false</c>.
		/// </returns>
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

		/// <summary>
		/// Determines whether the specified value is in a valid range.
		/// </summary>
		/// <param name="value">The value (never null and always of a valid type).</param>
		/// <returns>
		/// 	<c>true</c> if the specified value is in a valid range; otherwise, <c>false</c>.
		/// </returns>
		protected abstract bool IsInRange(object value);

		public static readonly DependencyProperty ResolutionProperty = DependencyProperty.RegisterAttached ("Resolution", typeof (TimeResolution), typeof (AbstractDateTimeType), new DependencyPropertyMetadata (TimeResolution.Default));
		public static readonly DependencyProperty MinimumDateProperty = DependencyProperty.RegisterAttached ("MinDate", typeof (Date), typeof (AbstractDateTimeType), new DependencyPropertyMetadata (Date.Null));
		public static readonly DependencyProperty MaximumDateProperty = DependencyProperty.RegisterAttached ("MaxDate", typeof (Date), typeof (AbstractDateTimeType), new DependencyPropertyMetadata (Date.Null));
		public static readonly DependencyProperty MinimumTimeProperty = DependencyProperty.RegisterAttached ("MinTime", typeof (Time), typeof (AbstractDateTimeType), new DependencyPropertyMetadata (Time.Null));
		public static readonly DependencyProperty MaximumTimeProperty = DependencyProperty.RegisterAttached ("MaxTime", typeof (Time), typeof (AbstractDateTimeType), new DependencyPropertyMetadata (Time.Null));
		
		public static readonly DependencyProperty TimeStepProperty = DependencyProperty.RegisterAttached ("TimeStep", typeof (System.TimeSpan), typeof (AbstractDateTimeType), new DependencyPropertyMetadata (new System.TimeSpan (0, 0, 1)));
		public static readonly DependencyProperty DateStepProperty = DependencyProperty.RegisterAttached ("DateStep", typeof (DateSpan), typeof (AbstractDateTimeType), new DependencyPropertyMetadata (new DateSpan (1)));
	}
}
