//	Copyright © 2003, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : OK/PA, 19/11/2003

namespace Epsitec.Cresus.Database
{
	/// <summary>
	/// La classe DbTypeString définit les paramètres d'un texte 'string'
	/// (DbSimpleType.String).
	/// </summary>
	public class DbTypeString : DbType
	{
		public DbTypeString(int length) : base (DbSimpleType.String)
		{
			this.length = length;
		}
		
		public DbTypeString(int length, params string[] attributes) : base (DbSimpleType.String, attributes)
		{
			this.length = length;
		}
		
		
		public int							Length
		{
			get { return this.length; }
		}
		
		
		public override object Clone()
		{
			DbTypeString type = base.Clone () as DbTypeString;
			
			type.length = this.length;
			
			return type;
		}
		
		
		protected int						length;
	}
}
