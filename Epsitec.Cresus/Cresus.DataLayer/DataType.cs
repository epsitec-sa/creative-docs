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

		
		
		protected DataAttributes			attributes;
		
		protected Database.DbSimpleType		db_simple_type;
		protected Database.DbNumDef			db_num_def;
	}
}
