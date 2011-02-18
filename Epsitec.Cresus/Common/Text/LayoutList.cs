//	Copyright © 2005-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text
{
	/// <summary>
	/// La classe LayoutList gère la liste des moteurs de layout.
	/// </summary>
	public sealed class LayoutList
	{
		public LayoutList(Text.TextContext context)
		{
			this.context = context;
			this.layouts = new System.Collections.Hashtable ();
		}
		
		
		public Layout.BaseEngine				this[string name]
		{
			get
			{
				return this.layouts[name] as Layout.BaseEngine;
			}
		}
		
		
		public Layout.BaseEngine NewEngine(string name, System.Type type)
		{
			Debug.Assert.IsFalse (this.layouts.Contains (name));
			
			Layout.BaseEngine engine = System.Activator.CreateInstance (type) as Layout.BaseEngine;
			
			engine.Initialize (this.context, name);
			
			this.layouts[name] = engine;
			
			return engine;
		}
		
		public void DisposeEngine(Layout.BaseEngine engine)
		{
			Debug.Assert.IsTrue (this.layouts.Contains (engine.Name));
			Debug.Assert.IsTrue (this.context == engine.TextContext);
			
			this.layouts.Remove (engine.Name);
			
			engine.Dispose ();
		}
		
		
		
		private Text.TextContext					context;
		private System.Collections.Hashtable	layouts;
	}
}
