//	Copyright © 2004-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

[assembly: Epsitec.Common.Types.DependencyClass (typeof (Epsitec.Common.Types.IntegerType))]

namespace Epsitec.Common.Types
{
	/// <summary>
	/// La classe IntegerType décrit une valeur de type System.Int32.
	/// </summary>
	public class IntegerType : AbstractNumericType
	{
		public IntegerType()
			: this (int.MinValue, int.MaxValue)
		{
		}

		public IntegerType(int min, int max)
			: base ("Integer", new DecimalRange (min, max))
		{
		}

		public IntegerType(Caption caption)
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
				return TypeCode.Integer;
			}
		}

		public override System.Type SystemType
		{
			get
			{
				return typeof (int);
			}
		}
		
		public override bool IsValidValue(object value)
		{
			if (this.IsNullValue (value))
			{
				return this.IsNullable;
			}

			if (value is int)
			{
				if (this.Range.IsEmpty)
				{
					return true;
				}
				else
				{
					int num = (int) value;

					return this.Range.Constrain (num) == num;
				}
			}
			
			return false;
		}

		public static IntegerType Default
		{
			get
			{
				TypeRosetta.InitializeKnownTypes ();

				if (IntegerType.defaultValue == null)
				{
					IntegerType.defaultValue = (IntegerType) TypeRosetta.CreateTypeObject (Support.Druid.Parse ("[1006]"));
				}

				return IntegerType.defaultValue;
			}
		}

		private static IntegerType defaultValue;
	}
}
