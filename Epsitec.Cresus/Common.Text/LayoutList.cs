//	Copyright © 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text
{
	/// <summary>
	/// La classe LayoutList gère la liste des moteurs de layout.
	/// </summary>
	public sealed class LayoutList
	{
		public LayoutList(Text.Context context)
		{
			this.context = context;
			this.hash    = new System.Collections.Hashtable ();
		}
		
		
		public Layout.BaseEngine				this[string name]
		{
			get
			{
				return this.hash[name] as Layout.BaseEngine;
			}
		}
		
		
		public Layout.BaseEngine NewEngine(string name, System.Type type)
		{
			Debug.Assert.IsFalse (this.hash.Contains (name));
			
			Layout.BaseEngine engine = System.Activator.CreateInstance (type) as Layout.BaseEngine;
			
			engine.Initialise (this.context, name);
			
			this.hash[name] = engine;
			
			return engine;
		}
		
		public void DisposeEngine(Layout.BaseEngine engine)
		{
			Debug.Assert.IsTrue (this.hash.Contains (engine.Name));
			Debug.Assert.IsTrue (this.context == engine.TextContext);
			
			this.hash.Remove (engine.Name);
			
			engine.Dispose ();
		}
		
		
		
		private Text.Context					context;
		private System.Collections.Hashtable	hash;
	}
}
