//	Copyright © 2003, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : en chantier/PA

namespace Epsitec.Cresus.DataLayer
{
	/// <summary>
	/// La classe DataType décrit un type de donnée.
	/// </summary>
	public class DataType
	{
		public DataType()
		{
		}
		
		public DataType(string binder_name)
		{
			this.Initialise (binder_name);
		}
		
		
		public void Initialise(string binder_name)
		{
			this.binder_name = binder_name;
		}
		
		
		public string							BinderName
		{
			get { return this.binder_name; }
		}
		
		
		protected string						binder_name;
	}
}
