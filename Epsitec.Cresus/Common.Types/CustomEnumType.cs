//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : OK/PA, 29/04/2004

namespace Epsitec.Common.Types
{
	/// <summary>
	/// La classe CustomEnumType décrit une énumération, basée sur une enum
	/// native, mais avec la possibilité d'accepter d'autres valeurs.
	/// </summary>
	public class CustomEnumType : EnumType
	{
		public CustomEnumType(System.Type enum_type) : base (enum_type)
		{
		}
		
		public CustomEnumType(System.Type enum_type, IDataConstraint constraint) : this (enum_type)
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
		
		
		public override bool CheckConstraint(object value)
		{
			if ((CustomEnumType.IsCustomName (value as string)) ||
				(base.CheckConstraint (value)))
			{
				if (this.constraint != null)
				{
					return this.constraint.CheckConstraint (value);
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
			if (CustomEnumType.IsCustomName (value))
			{
				return value.Substring (1, value.Length-2);
			}
			
			throw new System.ArgumentException (string.Format ("Specified name ({0}) is not a valid custom name.", value), "value");
		}
		
		
		private IDataConstraint					constraint;
	}
}
