//	Copyright © 2003, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : OK/PA, 19/10/2003

namespace Epsitec.Cresus.Database
{
	using Tags = Epsitec.Common.Support.Tags;
	using ResourceLevel = Epsitec.Common.Support.ResourceLevel;
	
	/// <summary>
	/// La classe DbType décrit un type de donnée pour spécifier DbColumn de
	/// manière plus précise.
	/// </summary>
	public class DbType : IDbAttributesHost, System.ICloneable
	{
		public DbType()
		{
		}
		
		public DbType(params string[] attributes) : this ()
		{
			this.attributes = new DbAttributes (attributes);
		}
		
		public DbType(DbSimpleType type, params string[] attributes) : this (attributes)
		{
			this.Initialise (type);
		}
		
		public DbType(DbNumDef num_def, params string[] attributes) : this (attributes)
		{
			this.Initialise (num_def);
		}
		
		
		internal void Initialise(DbNumDef num_def)
		{
			this.EnsureTypeIsNotInitialised();
			
			this.simple_type = DbSimpleType.Decimal;
			this.num_def     = num_def;
		}
		
		internal void Initialise(DbSimpleType type)
		{
			this.EnsureTypeIsNotInitialised();
			
			this.simple_type = type;
			this.num_def     = null;
		}
		
		
		public string						Name
		{
			get { return this.Attributes[Tags.Name, ResourceLevel.Default]; }
		}
		
		public string						Caption
		{
			get { return this.Attributes[Tags.Caption]; }
		}
		
		public string						Description
		{
			get { return this.Attributes[Tags.Description]; }
		}
		
		
		public DbNumDef						DbNumDef
		{
			get { return this.num_def; }
		}
		
		public DbSimpleType					DbSimpleType
		{
			get { return this.simple_type; }
		}
		
		
		#region ICloneable Members
		public virtual object Clone()
		{
			DbType type = System.Activator.CreateInstance (this.GetType ()) as DbType;
			
			type.attributes  = this.attributes == null ? null : this.attributes.Clone () as DbAttributes;
			type.simple_type = this.simple_type;
			type.num_def     = this.num_def == null ? null : this.num_def.Clone () as DbNumDef;
			
			return type;
		}
		#endregion
		
		#region IDbAttributesHost Members
		public DbAttributes					Attributes
		{
			get
			{
				if (this.attributes == null)
				{
					this.attributes = new DbAttributes ();
				}
				
				return this.attributes;
			}
		}
		#endregion
		
		#region Equals and GetHashCode support
		public override bool Equals(object obj)
		{
			//	ATTENTION: L'égalité se base uniquement sur le nom des types, pas sur les
			//	détails internes...
			
			DbType that = obj as DbType;
			
			if (that == null)
			{
				return false;
			}
			
			return (this.Name == that.Name);
		}
		
		public override int GetHashCode()
		{
			string name = this.Name;
			return (name == null) ? 0 : name.GetHashCode ();
		}

		#endregion
		
		protected virtual void EnsureTypeIsNotInitialised()
		{
			if (this.simple_type != DbSimpleType.Unsupported)
			{
				throw new System.InvalidOperationException ("Cannot reinitialise type");
			}
		}
		
		
		
		
		protected DbAttributes				attributes;
		protected DbSimpleType				simple_type;
		protected DbNumDef					num_def;
	}
}
