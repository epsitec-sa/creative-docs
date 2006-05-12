//	Copyright © 2004-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

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
		public OpenEnumType(System.Type enum_type) : base (enum_type)
		{
		}
		
		public OpenEnumType(System.Type enum_type, IDataConstraint constraint) : this (enum_type)
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
		
		
		public override bool ValidateValue(object value)
		{
			if ((OpenEnumType.IsCustomName (value as string)) ||
				(base.ValidateValue (value)))
			{
				if (this.constraint != null)
				{
					return this.constraint.ValidateValue (value);
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
