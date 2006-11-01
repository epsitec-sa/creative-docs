//	Copyright © 2004-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

[assembly: Epsitec.Common.Types.DependencyClass (typeof (Epsitec.Common.Types.StringType))]

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

		public bool								UseFixedLengthStorage
		{
			get
			{
				return (bool) this.Caption.GetValue (StringType.IsFixedLengthProperty);
			}
		}
		
		#endregion

		#region IDataConstraint Members

		public override bool IsValidValue(object value)
		{
			if ((this.IsNullValue (value)) &&
				(this.IsNullable))
			{
				return true;
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
				this.Caption.SetValue (StringType.IsFixedLengthProperty, value);
			}
			else
			{
				this.Caption.ClearValue (StringType.IsFixedLengthProperty);
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

		public static readonly DependencyProperty MinimumLengthProperty = DependencyProperty.RegisterAttached ("MinimumLength", typeof (int), typeof (StringType), new DependencyPropertyMetadata (0));
		public static readonly DependencyProperty MaximumLengthProperty = DependencyProperty.RegisterAttached ("MaximumLength", typeof (int), typeof (StringType), new DependencyPropertyMetadata (100*1000));
		public static readonly DependencyProperty IsFixedLengthProperty = DependencyProperty.RegisterAttached ("IsFixedLength", typeof (bool), typeof (StringType), new DependencyPropertyMetadata (false));

		private static StringType defaultValue;
	}
}
