//	Copyright © 2003, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : OK/PA, 20/11/2003

namespace Epsitec.Cresus.Database
{
	/// <summary>
	/// La classe DbTypeNum définit les paramètres d'une valeur numérique.
	/// </summary>
	public class DbTypeNum : DbType
	{
		public DbTypeNum(DbNumDef num_def) : base (DbSimpleType.Decimal)
		{
			this.num_def = num_def;
		}
		
		public DbTypeNum(DbNumDef num_def, params string[] attributes) : base (DbSimpleType.Decimal, attributes)
		{
			this.num_def = num_def;
		}
		
		
		public DbNumDef						NumDef
		{
			get { return this.num_def; }
		}
		
		
		public override object Clone()
		{
			DbTypeNum type = base.Clone () as DbTypeNum;
			
			type.num_def = (this.num_def == null) ? null : this.num_def.Clone () as DbNumDef;
			
			return type;
		}
		
		
		protected DbNumDef					num_def;
	}
}
