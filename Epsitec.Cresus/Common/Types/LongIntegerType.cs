//	Copyright © 2006-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

[assembly: Epsitec.Common.Types.DependencyClass (typeof (Epsitec.Common.Types.LongIntegerType))]

namespace Epsitec.Common.Types
{
	/// <summary>
	/// La classe LongIntegerType décrit une valeur de type System.Int64.
	/// </summary>
	public class LongIntegerType : AbstractNumericType
	{
		public LongIntegerType()
			: this (long.MinValue, long.MaxValue)
		{
		}

		public LongIntegerType(long min, long max)
			: base ("LongInteger", new DecimalRange (min, max))
		{
		}

		public LongIntegerType(Caption caption)
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
				return TypeCode.LongInteger;
			}
		}

		public override System.Type SystemType
		{
			get
			{
				return typeof (long);
			}
		}
		
		public override bool IsValidValue(object value)
		{
			if (this.IsNullValue (value))
			{
				return this.IsNullable;
			}

			if (value is long)
			{
				if (this.Range.IsEmpty)
				{
					return true;
				}
				else
				{
					long num = (long) value;
					return this.Range.Constrain (num) == num;
				}
			}
			
			return false;
		}

		public static LongIntegerType Default
		{
			get
			{
				TypeRosetta.InitializeKnownTypes ();

				if (LongIntegerType.defaultValue == null)
				{
					LongIntegerType.defaultValue = (LongIntegerType) TypeRosetta.CreateTypeObject (Support.Druid.Parse ("[1007]"));
				}

				return LongIntegerType.defaultValue;
			}
		}

		private static LongIntegerType defaultValue;
	}
}
