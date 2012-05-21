//	Copyright © 2006-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

[assembly: Epsitec.Common.Types.DependencyClass (typeof (Epsitec.Common.Types.AbstractNumericType))]

namespace Epsitec.Common.Types
{
	/// <summary>
	/// The <c>AbstractNumericType</c> class is the base class used by all numeric
	/// type definition classes.
	/// </summary>
	public abstract class AbstractNumericType : AbstractType, INumericType
	{
		protected AbstractNumericType(string name, DecimalRange range)
			: base (name)
		{
			this.DefineRange (range);
		}

		protected AbstractNumericType(Caption caption)
			: base (caption)
		{
		}

		#region INumericType Members

		/// <summary>
		/// Gets the range of values accepted by this numeric type.
		/// </summary>
		/// <value>The range of values.</value>
		public DecimalRange						Range
		{
			get
			{
				return (DecimalRange) this.Caption.GetValue (AbstractNumericType.RangeProperty);
			}
		}

		/// <summary>
		/// Gets the preferred range of values. This is not used as a constraint;
		/// it is just a hint for the user interface controls.
		/// </summary>
		/// <value>The preferred range of values.</value>
		public DecimalRange						PreferredRange
		{
			get
			{
				return (DecimalRange) this.Caption.GetValue (AbstractNumericType.PreferredRangeProperty);
			}
		}

		/// <summary>
		/// Gets the value which should be used by the user interface to increment
		/// or decrement a number by a small amount.
		/// </summary>
		/// <value>The small step value.</value>
		public decimal							SmallStep
		{
			get
			{
				return (decimal) this.Caption.GetValue (AbstractNumericType.SmallStepProperty);
			}
		}

		/// <summary>
		/// Gets the value which should be used by the user interface to increment
		/// or decrement a number by a large amount.
		/// </summary>
		/// <value>The large step value.</value>
		public decimal							LargeStep
		{
			get
			{
				return (decimal) this.Caption.GetValue (AbstractNumericType.LargeStepProperty);
			}
		}


		/// <summary>
		/// Gets a value indicating whether this numeric type should use compact storage.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this numeric type should use compact storage; otherwise, <c>false</c>.
		/// </value>
		public bool								UseCompactStorage
		{
			get
			{
				return (bool) this.Caption.GetValue (AbstractNumericType.UseCompactStorageProperty);
			}
		}

		#endregion

		/// <summary>
		/// Defines the valid range.
		/// </summary>
		/// <param name="range">The range.</param>
		public void DefineRange(DecimalRange range)
		{
			if (range.IsEmpty)
			{
				this.Caption.ClearValue (AbstractNumericType.RangeProperty);
			}
			else
			{
				this.Caption.SetValue (AbstractNumericType.RangeProperty, range);
			}
		}

		/// <summary>
		/// Defines the preferred range.
		/// </summary>
		/// <param name="range">The range.</param>
		public void DefinePreferredRange(DecimalRange range)
		{
			if (range.IsEmpty)
			{
				this.Caption.ClearValue (AbstractNumericType.PreferredRangeProperty);
			}
			else
			{
				this.Caption.SetValue (AbstractNumericType.PreferredRangeProperty, range);
			}
		}

		/// <summary>
		/// Defines the small increment.
		/// </summary>
		/// <param name="value">The value.</param>
		public void DefineSmallStep(decimal value)
		{
			if (value == 0)
			{
				this.Caption.ClearValue (AbstractNumericType.SmallStepProperty);
			}
			else
			{
				this.Caption.SetValue (AbstractNumericType.SmallStepProperty, value);
			}
		}

		/// <summary>
		/// Defines the large increment.
		/// </summary>
		/// <param name="value">The value.</param>
		public void DefineLargeStep(decimal value)
		{
			if (value == 0)
			{
				this.Caption.ClearValue (AbstractNumericType.LargeStepProperty);
			}
			else
			{
				this.Caption.SetValue (AbstractNumericType.LargeStepProperty, value);
			}
		}

		/// <summary>
		/// Defines whether to use compact storage for this numeric type.
		/// </summary>
		/// <param name="value">If set to <c>true</c>, uses compact storage for this numeric type.</param>
		public void DefineUseCompactStorage(bool value)
		{
			if (value == false)
			{
				this.Caption.ClearValue (AbstractNumericType.UseCompactStorageProperty);
			}
			else
			{
				this.Caption.SetValue (AbstractNumericType.UseCompactStorageProperty, value);
			}
		}
		
		public static readonly DependencyProperty RangeProperty = DependencyProperty.RegisterAttached ("Range", typeof (DecimalRange), typeof (AbstractNumericType), new DependencyPropertyMetadata (DecimalRange.Empty));
		public static readonly DependencyProperty PreferredRangeProperty = DependencyProperty.RegisterAttached ("PreferredRange", typeof (DecimalRange), typeof (AbstractNumericType), new DependencyPropertyMetadata (DecimalRange.Empty));
		public static readonly DependencyProperty SmallStepProperty = DependencyProperty.RegisterAttached ("SmallStep", typeof (decimal), typeof (AbstractNumericType), new DependencyPropertyMetadata (0M));
		public static readonly DependencyProperty LargeStepProperty = DependencyProperty.RegisterAttached ("LargeStep", typeof (decimal), typeof (AbstractNumericType), new DependencyPropertyMetadata (0M));
		public static readonly DependencyProperty UseCompactStorageProperty = DependencyProperty.RegisterAttached ("UseCompactStorage", typeof (bool), typeof (AbstractNumericType), new DependencyPropertyMetadata (false));
	}
}
