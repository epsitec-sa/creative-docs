//	Copyright © 2003, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : OK/PA, 22/11/2003

namespace Epsitec.Cresus.DataLayer
{
	public class DataChangeEventArgs : System.EventArgs
	{
		public DataChangeEventArgs(string path, System.Data.DataRowAction action)
		{
			this.path   = path;
			this.action = action;
		}
		
		
		public string							Path
		{
			get { return this.path; }
		}
		
		public System.Data.DataRowAction		Action
		{
			get { return this.action; }
		}
		
		
		protected string						path;
		protected System.Data.DataRowAction		action;
	}
	
	public delegate void DataChangeEventHandler(object sender, DataChangeEventArgs e);
}
