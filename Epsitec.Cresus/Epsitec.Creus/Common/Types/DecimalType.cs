//	Copyright © 2004-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

[assembly: Epsitec.Common.Types.DependencyClass (typeof (Epsitec.Common.Types.DecimalType))]

namespace Epsitec.Common.Types
{
	/// <summary>
	/// La classe DecimalType décrit des valeurs de type System.Decimal.
	/// </summary>
	public class DecimalType : AbstractNumericType
	{
		public DecimalType()
			: this (int.MinValue, int.MaxValue, 1)
		{
		}

		public DecimalType(DecimalRange range)
			: base ("Decimal", range)
		{
		}

		public DecimalType(decimal min, decimal max, decimal resolution)
			: this (new DecimalRange (min, max, resolution))
		{
		}

		public DecimalType(Caption caption)
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
				return TypeCode.Decimal;
			}
		}


		
		
		public override System.Type				SystemType
		{
			get
			{
				return typeof (decimal);
			}
		}
		
		public override bool IsValidValue(object value)
		{
			if (this.IsNullValue (value))
			{
				return this.IsNullable;
			}

			if (value is decimal)
			{
				if (this.Range.IsEmpty)
				{
					return true;
				}
				else
				{
					decimal num = (decimal) value;
					return this.Range.Constrain (num) == num;
				}
			}
			
			return false;
		}

		public static DecimalType Default
		{
			get
			{
				TypeRosetta.InitializeKnownTypes ();

				if (DecimalType.defaultValue == null)
				{
					DecimalType.defaultValue = (DecimalType) TypeRosetta.CreateTypeObject (Support.Druid.Parse ("[1004]"));
				}

				return DecimalType.defaultValue;
			}
		}

		private static DecimalType defaultValue;
	}
}
