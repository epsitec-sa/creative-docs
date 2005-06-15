//	Copyright © 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text
{
	/// <summary>
	/// La classe ParagraphManagerList gère la liste des gestionnaires de paragraphes.
	/// </summary>
	public sealed class ParagraphManagerList
	{
		public ParagraphManagerList(Text.Context context)
		{
			this.context  = context;
			this.managers = new System.Collections.Hashtable ();
		}
		
		
		public ParagraphManager					this[string name]
		{
			get
			{
				return this.managers[name] as ParagraphManager;
			}
		}
		
		
		public void AddParagraphManager(ParagraphManager manager)
		{
			string name = manager.Name;
			
			System.Diagnostics.Debug.Assert (this.managers.Contains (name) == false);
			
			this.managers[name] = manager;
		}
		
		public void RemoveParagraphManager(ParagraphManager manager)
		{
			string name = manager.Name;
			
			System.Diagnostics.Debug.Assert (this.managers.Contains (name));
			
			this.managers.Remove (name);
		}
		
		
		
		private Text.Context					context;
		private System.Collections.Hashtable	managers;
	}
}
