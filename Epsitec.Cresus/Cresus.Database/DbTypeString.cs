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
		public DbTypeString(int length) : this (length, true)
		{
		}
		
		public DbTypeString(int length, bool is_fixed_length) : base (DbSimpleType.String)
		{
			this.length = length;
			this.is_fixed_length = is_fixed_length;
		}
		
		public DbTypeString(int length, params string[] attributes) : this (length, true, attributes)
		{
		}
		
		public DbTypeString(int length, bool is_fixed_length, params string[] attributes) : base (DbSimpleType.String, attributes)
		{
			this.length = length;
			this.is_fixed_length = is_fixed_length;
		}
		
		
		public int						Length
		{
			get { return this.length; }
		}
		
		public bool						IsFixedLength
		{
			get
			{
				return this.is_fixed_length;
			}
		}
		
		
		public override object Clone()
		{
			DbTypeString type = base.Clone () as DbTypeString;
			
			type.length = this.length;
			type.is_fixed_length = this.is_fixed_length;
			
			return type;
		}
		
		
		protected int					length;
		protected bool					is_fixed_length;
	}
}
