//	Copyright � 2004-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;

[assembly: DependencyClass (typeof (StringType))]

namespace Epsitec.Common.Types
{
	/// <summary>
	/// La classe StringType d�crit des valeurs de type System.String.
	/// </summary>
	public class StringType : AbstractType, IStringType
	{
		public StringType()
			: base ("String", "String", null)
		{
		}

		public StringType(int minimumLength)
			: this ()
		{
			this.DefineMinimumLength (minimumLength);
		}

		public StringType(int minimumLength, int maximumLength)
			: this ()
		{
			this.DefineMinimumLength (minimumLength);
			this.DefineMaximumLength (maximumLength);
		}

		public StringType(Caption caption)
			: base (caption)
		{
		}

		/// <summary>
		/// Gets the type code for the type.
		/// </summary>
		/// <value>The type code.</value>
		public override TypeCode TypeCode
		{
			get
			{
				return TypeCode.String;
			}
		}

		#region ISystemType Members
		public override System.Type				SystemType
		{
			get
			{
				if (this.UseFormattedText)
				{
					return typeof (FormattedText);
				}
				else
				{
					return typeof (string);
				}
			}
		}
		#endregion
		
		#region IStringType Members

		public int								MinimumLength
		{
			get
			{
				return (int) this.Caption.GetValue (StringType.MinimumLengthProperty);
			}
		}

		public int								MaximumLength
		{
			get
			{
				return (int) this.Caption.GetValue (StringType.MaximumLengthProperty);
			}
		}

		public bool								UseFixedLengthStorage
		{
			get
			{
				return (bool) this.Caption.GetValue (StringType.UseFixedLengthStorageProperty);
			}
		}

		public bool								UseMultilingualStorage
		{
			get
			{
				return (bool) this.Caption.GetValue (StringType.UseMultilingualStorageProperty);
			}
		}

		public bool								UseFormattedText
		{
			get
			{
				return (bool) this.Caption.GetValue (StringType.UseFormattedTextProperty);
			}
		}

		public StringSearchBehavior				DefaultSearchBehavior
		{
			get
			{
				return StringType.GetDefaultSearchBehavior (this.Caption);
			}
		}

		public StringComparisonBehavior			DefaultComparisonBehavior
		{
			get
			{
				return StringType.GetDefaultComparisonBehavior (this.Caption);
			}
		}

		#endregion

		#region IDataConstraint Members

		public override bool IsValidValue(object value)
		{
			if (this.IsNullValue (value))
			{
				return this.IsNullable;
			}

			string text = value as string;

			if (text != null)
			{
				int length = text.Length;

				if ((length >= this.MinimumLength) &&
					(length <= this.MaximumLength))
				{
					return true;
				}
			}
			
			return false;
		}

		#endregion

		public override object DefaultValue
		{
			get
			{
				object value = base.DefaultValue;
				
				if ((value == null) &&
					(this.IsNullable == false))
				{
					return "";
				}

				return value;
			}
		}

		public void DefineMinimumLength(int value)
		{
			if (value > 0)
			{
				this.Caption.SetValue (StringType.MinimumLengthProperty, value);
			}
			else
			{
				this.Caption.ClearValue (StringType.MinimumLengthProperty);
			}
		}

		public void DefineMaximumLength(int value)
		{
			this.Caption.SetValue (StringType.MaximumLengthProperty, value);
		}

		public void DefineUseFixedLengthStorage(bool value)
		{
			if (value)
			{
				this.Caption.SetValue (StringType.UseFixedLengthStorageProperty, value);
			}
			else
			{
				this.Caption.ClearValue (StringType.UseFixedLengthStorageProperty);
			}
		}

		public void DefineUseMultilingualStorage(bool value)
		{
			if (value)
			{
				this.Caption.SetValue (StringType.UseMultilingualStorageProperty, value);
			}
			else
			{
				this.Caption.ClearValue (StringType.UseMultilingualStorageProperty);
			}
		}

		public void DefineUseFormattedText(bool value)
		{
			if (value)
			{
				this.Caption.SetValue (StringType.UseFormattedTextProperty, value);
			}
			else
			{
				this.Caption.ClearValue (StringType.UseFormattedTextProperty);
			}
		}

		public void DefineDefaultSearchBehavior(StringSearchBehavior value)
		{
			StringType.SetDefaultSearchBehavior (this.Caption, value);
		}

		public void DefineDefaultComparisonBehavior(StringComparisonBehavior value)
		{
			StringType.SetDefaultComparisonBehavior (this.Caption, value);
		}

		public static StringSearchBehavior GetDefaultSearchBehavior(DependencyObject obj)
		{
			return (StringSearchBehavior) obj.GetValue (StringType.DefaultSearchBehaviorProperty);
		}

		public static void SetDefaultSearchBehavior(DependencyObject obj, StringSearchBehavior value)
		{
			if (value == StringSearchBehavior.ExactMatch)
			{
				obj.ClearValue (StringType.DefaultSearchBehaviorProperty);
			}
			else
			{
				obj.SetValue (StringType.DefaultSearchBehaviorProperty, value);
			}
		}
		
		public static StringComparisonBehavior GetDefaultComparisonBehavior(DependencyObject obj)
		{
			return (StringComparisonBehavior) obj.GetValue (StringType.DefaultComparisonBehaviorProperty);
		}

		public static void SetDefaultComparisonBehavior(DependencyObject obj, StringComparisonBehavior value)
		{
			if (value == StringComparisonBehavior.Ordinal)
			{
				obj.ClearValue (StringType.DefaultComparisonBehaviorProperty);
			}
			else
			{
				obj.SetValue (StringType.DefaultComparisonBehaviorProperty, value);
			}
		}
		
		public static StringType Default
		{
			get
			{
				TypeRosetta.InitializeKnownTypes ();

				if (StringType.defaultValue == null)
				{
					StringType.defaultValue = (StringType) TypeRosetta.CreateTypeObject (Support.Druid.Parse ("[1008]"));
				}
				
				return StringType.defaultValue;
			}
		}

		public static readonly DependencyProperty MinimumLengthProperty = DependencyProperty.RegisterAttached ("MinLength", typeof (int), typeof (StringType), new DependencyPropertyMetadata (0));
		public static readonly DependencyProperty MaximumLengthProperty = DependencyProperty.RegisterAttached ("MaxLength", typeof (int), typeof (StringType), new DependencyPropertyMetadata (1000000));
		
		public static readonly DependencyProperty UseFixedLengthStorageProperty  = DependencyProperty.RegisterAttached ("UseFixedLengthStorage",  typeof (bool), typeof (StringType), new DependencyPropertyMetadata (false));
		public static readonly DependencyProperty UseMultilingualStorageProperty = DependencyProperty.RegisterAttached ("UseMultilingualStorage", typeof (bool), typeof (StringType), new DependencyPropertyMetadata (false));
		public static readonly DependencyProperty UseFormattedTextProperty       = DependencyProperty.RegisterAttached ("UseFormattedText",       typeof (bool), typeof (StringType), new DependencyPropertyMetadata (false));

		public static readonly DependencyProperty DefaultSearchBehaviorProperty     = DependencyProperty.RegisterAttached ("DefaultSearchBehavior", typeof (StringSearchBehavior), typeof (StringType), new DependencyPropertyMetadata (StringSearchBehavior.ExactMatch));
		public static readonly DependencyProperty DefaultComparisonBehaviorProperty = DependencyProperty.RegisterAttached ("DefaultComparisonBehavior", typeof (StringComparisonBehavior), typeof (StringType), new DependencyPropertyMetadata (StringComparisonBehavior.Ordinal));

		private static StringType defaultValue;
	}
}
