//	Copyright � 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : OK/PA, 29/04/2004

namespace Epsitec.Common.Types
{
	/// <summary>
	/// La classe CustomEnumType d�crit une �num�ration, bas�e sur une enum
	/// native, mais avec la possibilit� d'accepter d'autres valeurs.
	/// </summary>
	public class CustomEnumType : EnumType, IDataConstraint
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
		
		
		#region IDataConstraint Members
		public bool CheckConstraint(object value)
		{
			if (this.constraint == null)
			{
				return value is string;
			}
			
			return this.constraint.CheckConstraint (value);
		}
		#endregion
		
		
		private IDataConstraint					constraint;
	}
}
