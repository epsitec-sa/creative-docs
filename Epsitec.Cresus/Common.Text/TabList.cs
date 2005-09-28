//	Copyright © 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text
{
	/// <summary>
	/// La classe TabList gère la liste globale des tabulateurs.
	/// </summary>
	public sealed class TabList
	{
		public TabList()
		{
			this.tab_list = new System.Collections.ArrayList ();
			this.tab_hash = new System.Collections.Hashtable ();
		}
		
		
		public long								Version
		{
			get
			{
				return this.StyleVersion.Current;
			}
		}
		
		public StyleVersion						StyleVersion
		{
			get
			{
				return Text.StyleVersion.Default;
			}
		}
		
		
		public Properties.TabProperty			this[string tag]
		{
			get
			{
				return this.GetTabProperty (tag);
			}
		}
		
		
		public Properties.TabProperty NewTab(string tag, double position, Properties.SizeUnits units, double disposition)
		{
			return this.NewTab (tag, position, units, disposition, null);
		}
		
		public Properties.TabProperty NewTab(string tag, double position, Properties.SizeUnits units, double disposition, string docking_mark)
		{
			Properties.TabProperty tab = new Properties.TabProperty (tag, position, units, disposition, docking_mark);
			
			this.Attach (tab);
			
			return tab;
		}
		
		
		public void RedefineTab(Properties.TabProperty tab, double position, Properties.SizeUnits units, double disposition, string docking_mark)
		{
			tab.Initialise (position, units, disposition, docking_mark);
		}
		
		public void RecycleTab(Properties.TabProperty tab)
		{
			this.Detach (tab);
		}
		
		
		public Properties.TabProperty GetTabProperty(string tag)
		{
			if (this.tab_hash.Contains (tag))
			{
				return this.tab_hash[tag] as Properties.TabProperty;
			}
			else
			{
				return null;
			}
		}
		
		
		private void Attach(Properties.TabProperty tab)
		{
			string tag = tab.TabTag;
			
			if (this.tab_hash.Contains (tag))
			{
				throw new System.ArgumentException (string.Format ("TabProperty named {0} already exists", tag), "tab");
			}
			
			this.tab_list.Add (tab);
			this.tab_hash[tag] = tab;
		}
		
		private void Detach(Properties.TabProperty tab)
		{
			string tag = tab.TabTag;
			
			if (this.tab_hash.Contains (tag))
			{
				this.tab_list.Remove (tab);
				this.tab_hash.Remove (tag);
			}
			else
			{
				throw new System.ArgumentException (string.Format ("TabProperty named {0} does not exist", tag), "tab");
			}
		}
		
		
		private System.Collections.ArrayList	tab_list;
		private System.Collections.Hashtable	tab_hash;
	}
}
