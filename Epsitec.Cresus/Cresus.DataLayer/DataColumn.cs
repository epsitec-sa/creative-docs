//	Copyright © 2003, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : en chantier/PA

using Epsitec.Common.Support;

namespace Epsitec.Cresus.DataLayer
{
	/// <summary>
	/// La classe DataColumn représente les informations sur une colonne
	/// d'une table. Elle ne contient pas de données, mais des descriptions.
	/// </summary>
	public class DataColumn : IDataAttributesHost, System.ICloneable
	{
		protected DataColumn()
		{
		}
		
		public DataColumn(int index, params string[] attributes)
		{
			this.index = index;
			this.Attributes.SetFromInitialisationList (attributes);
		}
		
		
		public string						Name
		{
			get { return this.Attributes.GetAttribute (Tags.Name, ResourceLevel.Default); }
		}
		
		public string						UserLabel
		{
			get { return this.Attributes.GetAttribute (Tags.Caption); }
		}
		
		public string						UserDescription
		{
			get { return this.Attributes.GetAttribute (Tags.Description); }
		}
		
		public int							Index
		{
			get { return this.index; }
		}
		
		
		#region ICloneable Members
		public virtual object Clone()
		{
			DataColumn column = System.Activator.CreateInstance (this.GetType ()) as DataColumn;
			
			column.attributes = this.attributes == null ? null : this.attributes.Clone () as DataAttributes;
			column.index      = this.index;
			
			return column;
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
		
		#region Equals and GetHashCode support
		public override bool Equals(object obj)
		{
			//	ATTENTION: L'égalité se base uniquement sur le nom des colonnes, pas sur les
			//	détails internes...
			
			DataColumn that = obj as DataColumn;
			
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
		
		
		protected int						index;
		protected DataAttributes			attributes;
	}
}
