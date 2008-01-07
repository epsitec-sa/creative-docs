//	Copyright � 2004-2008, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

[assembly: Epsitec.Common.Types.DependencyClass (typeof (Epsitec.Common.Types.OpenEnumType))]

namespace Epsitec.Common.Types
{
	/// <summary>
	/// La classe OpenEnumType d�crit une �num�ration, bas�e sur une enum
	/// native, mais avec la possibilit� d'accepter d'autres valeurs. En ce
	/// sens, l'�num�ration est "ouverte".
	/// Les noms des �l�ments ajout�s � l'�num�ration native sont d�cor�s par
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
