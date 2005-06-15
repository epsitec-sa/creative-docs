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
			
			//	Ajoute les gestionnaires de paragraphes connus :
			
			this.RegisterTrustedAssembly (this.GetType ().Assembly);
		}
		
		
		public IParagraphManager				this[string name]
		{
			get
			{
				return this.managers[name] as IParagraphManager;
			}
		}
		
		
		public void AddParagraphManager(IParagraphManager manager)
		{
			string name = manager.Name;
			
			System.Diagnostics.Debug.Assert (this.managers.Contains (name) == false);
			
			this.managers[name] = manager;
		}
		
		public void RemoveParagraphManager(IParagraphManager manager)
		{
			string name = manager.Name;
			
			System.Diagnostics.Debug.Assert (this.managers.Contains (name));
			
			this.managers.Remove (name);
		}
		
		
		
		private void RegisterTrustedAssembly(System.Reflection.Assembly assembly)
		{
			System.Type[] assembly_types = assembly.GetTypes ();
			
			foreach (System.Type type in assembly_types)
			{
				if ((type.IsClass) &&
					(!type.IsAbstract))
				{
					if (type.GetInterface ("IParagraphManager") != null)
					{
						this.AddParagraphManager (System.Activator.CreateInstance (type, true) as IParagraphManager);
					}
				}
			}
		}
		
		
		private Text.Context					context;
		private System.Collections.Hashtable	managers;
	}
}
