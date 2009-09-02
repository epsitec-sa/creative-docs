//	Copyright © 2005-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text
{
	/// <summary>
	/// La classe ParagraphManagerList gère la liste des gestionnaires de paragraphes.
	/// </summary>
	public sealed class ParagraphManagerList
	{
		public ParagraphManagerList(Text.TextContext context)
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
			System.Type[] assemblyTypes = assembly.GetTypes ();
			
			foreach (System.Type type in assemblyTypes)
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
		
		
		private Text.TextContext					context;
		private System.Collections.Hashtable	managers;
	}
}
