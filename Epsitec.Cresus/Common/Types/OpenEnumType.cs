//	Copyright © 2004-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

[assembly: Epsitec.Common.Types.DependencyClass (typeof (Epsitec.Common.Types.OpenEnumType))]

namespace Epsitec.Common.Types
{
	/// <summary>
	/// La classe OpenEnumType décrit une énumération, basée sur une enum
	/// native, mais avec la possibilité d'accepter d'autres valeurs. En ce
	/// sens, l'énumération est "ouverte".
	/// Les noms des éléments ajoutés à l'énumération native sont décorés par
	/// une paire d'accolades; par exemple "{Xyz}"
	/// </summary>
	public class OpenEnumType : EnumType
	{
		public OpenEnumType(System.Type enumType) : base (enumType)
		{
		}
		
		public OpenEnumType(System.Type enumType, IDataConstraint constraint) : this (enumType)
		{
			this.constraint = constraint;
		}
		
		
		public IDataConstraint					Constraint
		{
			get
			{
				return this.constraint;
			}
		}
		
		public override bool					IsCustomizable
		{
			get
			{
				return true;
			}
		}
		
		public override System.Type				SystemType
		{
			get
			{
				return typeof (string);
			}
		}
		
		
		public override bool IsValidValue(object value)
		{
			if (this.IsNullValue (value))
			{
				return this.IsNullable;
			}

			if ((OpenEnumType.IsCustomName (value as string)) ||
				(base.IsValidValue (value)))
			{
				if (this.constraint != null)
				{
					return this.constraint.IsValidValue (value);
				}
				
				return true;
			}
			
			return false;
		}
		
		
		public static bool IsCustomName(string value)
		{
			if ((value != null) &&
				(value.Length > 1) &&
				(value[0] == '{') &&
				(value[value.Length-1] == '}'))
			{
				return true;
			}
			
			return false;
		}
		
		public static string ToCustomName(string value)
		{
			if (value != null)
			{
				return string.Concat ("{", value, "}");
			}
			
			return "{}";
		}
		
		public static string FromCustomName(string value)
		{
			if (OpenEnumType.IsCustomName (value))
			{
				return value.Substring (1, value.Length-2);
			}
			
			throw new System.ArgumentException (string.Format ("Specified name ({0}) is not a valid custom name", value), "value");
		}
		
		
		private IDataConstraint					constraint;
	}
}
