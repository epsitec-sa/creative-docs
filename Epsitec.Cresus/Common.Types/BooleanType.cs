//	Copyright © 2004-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

[assembly: Epsitec.Common.Types.DependencyClass (typeof (Epsitec.Common.Types.BooleanType))]

namespace Epsitec.Common.Types
{
	/// <summary>
	/// La classe BooleanType décrit des valeurs de type System.Boolean.
	/// </summary>
	public class BooleanType : AbstractNumericType
	{
		public BooleanType()
			: base ("Boolean", new DecimalRange (0, 1, 1))
		{
		}

		public BooleanType(Caption caption)
			: base (caption)
		{
		}
		
		
		public override System.Type				SystemType
		{
			get
			{
				return typeof (bool);
			}
		}


		/// <summary>
		/// Gets the type code for the type.
		/// </summary>
		/// <value>The type code.</value>
		public override TypeCode TypeCode
		{
			get
			{
				return TypeCode.Boolean;
			}
		}



		public override bool IsValidValue(object value)
		{
			if (this.IsNullValue (value))
			{
				return this.IsNullable;
			}

			if (value is bool)
			{
				return true;
			}
			
			return false;
		}

		static BooleanType()
		{
			DependencyPropertyMetadata metadata = new DependencyPropertyMetadata ("Boolean");

			BooleanType.DefaultControllerProperty.OverrideMetadata (typeof (BooleanType), metadata);
		}

		public static BooleanType Default
		{
			get
			{
				TypeRosetta.InitializeKnownTypes ();
				
				if (BooleanType.defaultValue == null)
				{
					BooleanType.defaultValue = (BooleanType) TypeRosetta.CreateTypeObject (Support.Druid.Parse ("[1003]"));
				}

				return BooleanType.defaultValue;
			}
		}

		private static BooleanType defaultValue;
	}
}
