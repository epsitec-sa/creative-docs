//	Copyright © 2004-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

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


		#region ISystemType Members
		public override System.Type				SystemType
		{
			get
			{
				return typeof (System.String);
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
		
		#endregion

		#region IDataConstraint Members

		public override bool IsValidValue(object value)
		{
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

		public void DefineMinimumLength(int value)
		{
			this.Caption.SetValue (StringType.MinimumLengthProperty, value);
		}

		public void DefineMaximumLength(int value)
		{
			this.Caption.SetValue (StringType.MaximumLengthProperty, value);
		}
		
		public static readonly StringType		Default = new StringType ();

		public static readonly DependencyProperty MinimumLengthProperty = DependencyProperty.RegisterAttached ("MinimumLength", typeof (int), typeof (StringType), new DependencyPropertyMetadata (0));
		public static readonly DependencyProperty MaximumLengthProperty = DependencyProperty.RegisterAttached ("MaximumLength", typeof (int), typeof (StringType), new DependencyPropertyMetadata (100*1000));
	}
}
