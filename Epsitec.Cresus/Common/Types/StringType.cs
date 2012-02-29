//	Copyright © 2004-2010, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Types;

[assembly: DependencyClass (typeof (StringType))]

namespace Epsitec.Common.Types
{
	/// <summary>
	/// La classe StringType décrit des valeurs de type System.String.
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
				if (this.UseFormattedText)
				{
					return false;
				}
			}

			if (value is FormattedText)
			{
				if (!this.UseFormattedText)
				{
					return false;
				}

				FormattedText formattedText = (FormattedText) value;
				text = formattedText.ToString ();
			}

			if (text != null)
			{
				int length = text.Length;

				if ((length >= this.MinimumLength) &&
					(length <= this.MaximumLength))
				{
					return true;
				}

				System.Diagnostics.Debug.Fail (string.Format ("String length {0} not between {1} and {2}", length, this.MinimumLength, this.MaximumLength));
			}
			
			return false;
		}

		#endregion

		public override bool IsNullValue(object value)
		{
			if (base.IsNullValue (value))
			{
				return true;
			}

			if (value is FormattedText)
			{
				FormattedText text = (FormattedText) value;
				return text.IsNull;
			}

			return false;
		}

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

		public static bool IsMultilineText(INamedType type)
		{
			var stringType = type as StringType;

			var isMultiline = false;

			if (stringType != null)
			{
				// NOTE The multi lined text types are the following ones :
				// [10AH] => Default.TextMultiline
				// [1016] => Default.StringMultiline

				isMultiline = stringType.CaptionId == Druid.Parse ("[10AH]")
						   || stringType.CaptionId == Druid.Parse ("[1016]");
			}

			return isMultiline;
		}
		
		public static StringType					Default
		{
			get
			{
				TypeRosetta.InitializeKnownTypes ();

				if (StringType.defaultValue == null)
				{
					StringType.defaultValue = (StringType) TypeRosetta.CreateTypeObject (Support.Druid.Parse ("[1008]"));	//	Default.String
				}

				return StringType.defaultValue;
			}
		}

		public static StringType					NativeDefault
		{
			get
			{
				TypeRosetta.InitializeKnownTypes ();

				if (StringType.defaultNativeValue == null)
				{
					StringType.defaultNativeValue = (StringType) TypeRosetta.CreateTypeObject (Support.Druid.Parse ("[10AJ]"));			//	Default.StringNative
				}
				
				return StringType.defaultNativeValue;
			}
		}

		
		public static readonly DependencyProperty	MinimumLengthProperty = DependencyProperty<StringType>.RegisterAttached ("MinLength", typeof (int), new DependencyPropertyMetadata (0));
		public static readonly DependencyProperty	MaximumLengthProperty = DependencyProperty<StringType>.RegisterAttached ("MaxLength", typeof (int), new DependencyPropertyMetadata (1000000));

		public static readonly DependencyProperty	UseFixedLengthStorageProperty  = DependencyProperty<StringType>.RegisterAttached ("UseFixedLengthStorage", typeof (bool), new DependencyPropertyMetadata (false));
		public static readonly DependencyProperty	UseMultilingualStorageProperty = DependencyProperty<StringType>.RegisterAttached ("UseMultilingualStorage", typeof (bool), new DependencyPropertyMetadata (false));
		public static readonly DependencyProperty	UseFormattedTextProperty       = DependencyProperty<StringType>.RegisterAttached ("UseFormattedText", typeof (bool), new DependencyPropertyMetadata (false));

		public static readonly DependencyProperty	DefaultSearchBehaviorProperty     = DependencyProperty<StringType>.RegisterAttached ("DefaultSearchBehavior", typeof (StringSearchBehavior), new DependencyPropertyMetadata (StringSearchBehavior.ExactMatch));
		public static readonly DependencyProperty	DefaultComparisonBehaviorProperty = DependencyProperty<StringType>.RegisterAttached ("DefaultComparisonBehavior", typeof (StringComparisonBehavior), new DependencyPropertyMetadata (StringComparisonBehavior.Ordinal));

		private static StringType					defaultNativeValue;
		private static StringType					defaultValue;
	}
}
