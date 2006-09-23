//	Copyright � 2004-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Types
{
	/// <summary>
	/// La classe DecimalType d�crit des valeurs de type System.Decimal.
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
		
		
		public override System.Type				SystemType
		{
			get
			{
				return typeof (decimal);
			}
		}
		
		public override bool IsValidValue(object value)
		{
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
