//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Dialogs
{
	/// <summary>
	/// La classe FilterItem représente un filtre pour les dialogues
	/// opérant sur les fichiers.
	/// </summary>
	public class FilterItem
	{
		public FilterItem(string name, string caption, string filter)
		{
			this.name    = name;
			this.caption = caption;
			this.filter  = filter;
		}
		
		
		public string					Name
		{
			get { return this.name; }
		}
		
		public string					Caption
		{
			get { return this.caption; }
		}
		
		public string					Filter
		{
			get { return this.filter; }
		}
		
		public string					FileDialogFilter
		{
			get
			{
				System.Text.StringBuilder buffer = new System.Text.StringBuilder ();
				
				buffer.Append (this.caption);
				buffer.Append (" (");
				buffer.Append (this.filter);
				buffer.Append (")|");
				buffer.Append (this.filter);
				
				return buffer.ToString ();
			}
		}
		
		
		protected string				name;
		protected string				caption;
		protected string				filter;
	}
}
