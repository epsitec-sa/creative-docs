//	Copyright © 2003, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : en chantier/PA

using Epsitec.Common.Support;

namespace Epsitec.Cresus.DataLayer
{
	/// <summary>
	/// La classe DataType décrit un type de donnée.
	/// </summary>
	public class DataType : IDataAttributesHost, System.ICloneable
	{
		public DataType()
		{
		}
		
		public DataType(params string[] attributes)
		{
			this.Attributes.SetFromInitialisationList (attributes);
		}
		
		public DataType(Database.DbSimpleType type, params string[] attributes) : this (attributes)
		{
			this.Initialise (type);
		}
		
		public DataType(Database.DbNumDef num_def, params string[] attributes) : this (attributes)
		{
			this.Initialise (num_def);
		}
		
		
		public void Initialise(Database.DbNumDef num_def)
		{
			this.EnsureTypeIsNotInitialised();
			
			this.db_simple_type = Database.DbSimpleType.Decimal;
			this.db_num_def     = num_def;
		}
		
		public void Initialise(Database.DbSimpleType type)
		{
			this.EnsureTypeIsNotInitialised();
			
			this.db_simple_type = type;
			this.db_num_def     = null;
		}
		
		
		public string						Name
		{
			get { return this.Attributes.GetAttribute (Tags.Name, ResourceLevel.Default); }
		}
		
		public string						UserLabel
		{
			get { return this.Attributes.GetAttribute (Tags.Label); }
		}
		
		public string						UserDescription
		{
			get { return this.Attributes.GetAttribute (Tags.Description); }
		}
		
		
		public Database.DbNumDef			DbNumDef
		{
			get { return this.db_num_def; }
		}
		
		public Database.DbSimpleType		DbSimpleType
		{
			get { return this.db_simple_type; }
		}
		
		
		#region ICloneable Members
		public virtual object Clone()
		{
			DataType type = System.Activator.CreateInstance (this.GetType ()) as DataType;
			
			type.attributes     = this.attributes == null ? null : this.attributes.Clone () as DataAttributes;
			type.db_simple_type = this.db_simple_type;
			type.db_num_def     = this.db_num_def == null ? null : this.db_num_def.Clone () as Database.DbNumDef;
			
			return type;
		}
		#endregion
		
		#region IDataAttributesHost Members
		public DataAttributes				Attributes
		{
			get
			{
				if (this.attributes == null)
				{
					this.attributes = new DataAttributes ();
				}
				
				return this.attributes;
			}
		}
		#endregion
		
		public override bool Equals(object obj)
		{
			//	ATTENTION: L'égalité se base uniquement sur le nom des types, pas sur les
			//	détails internes...
			
			DataType that = obj as DataType;
			
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

		
		protected virtual void EnsureTypeIsNotInitialised()
		{
			if (this.db_simple_type != Database.DbSimpleType.Unsupported)
			{
				throw new System.InvalidOperationException ("Cannot reinitialise type");
			}
		}
		
		
		
		
		protected DataAttributes			attributes;
		
		protected Database.DbSimpleType		db_simple_type;
		protected Database.DbNumDef			db_num_def;
	}
}
