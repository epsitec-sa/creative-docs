//	Copyright © 2003, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : en chantier/PA

using Epsitec.Common.Support;

namespace Epsitec.Cresus.DataLayer
{
	/// <summary>
	/// La classe DataType décrit un type de donnée.
	/// </summary>
	public class DataType : IDataAttributesHost
	{
		public DataType()
		{
		}
		
		public DataType(params string[] attributes)
		{
			this.Attributes.SetFromInitialisationList (attributes);
		}
		
		
		public string							Name
		{
			get { return this.Attributes.GetAttribute ("name"); }
		}
		
		public string							BinderEngine
		{
			get
			{
				string binder = this.Attributes.GetAttribute ("binder");
				return (binder == null) ? this.Name : binder;
			}
		}
		
		public string							UserLabel
		{
			get { return this.Attributes.GetAttribute ("label"); }
		}
		
		public string							UserDescription
		{
			get { return this.Attributes.GetAttribute ("descr"); }
		}
		
		
		#region IDataAttributesHost Members
		public DataAttributes					Attributes
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

		
		
		protected DataAttributes				attributes;
	}
}
