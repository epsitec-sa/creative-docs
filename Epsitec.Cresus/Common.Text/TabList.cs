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
			return this.NewTab (tag, position, units, disposition, null, TabPositionMode.Absolute);
		}
		
		public Properties.TabProperty NewTab(string tag, double position, Properties.SizeUnits units, double disposition, string docking_mark, TabPositionMode position_mode)
		{
			if (tag == null)
			{
				tag = this.GenerateUniqueName ();
			}
			
			Properties.TabProperty tab = new Properties.TabProperty (tag);
			
			this.Attach (tab, new TabRecord (position, units, disposition, docking_mark, position_mode));
			
			return tab;
		}
		
		
		public void RedefineTab(Properties.TabProperty tab, double position, Properties.SizeUnits units, double disposition, string docking_mark, TabPositionMode position_mode)
		{
			System.Diagnostics.Debug.Assert (tab != null);
			System.Diagnostics.Debug.Assert (tab.TabTag != null);
			
			TabRecord record = this.GetTabRecord (tab);
			
			if (record == null)
			{
				throw new System.ArgumentException (string.Format ("TabProperty named {0} does not exist", tab.TabTag), "tab");
			}
			
			this.StyleVersion.Change ();
			
			lock (record)
			{
				record.Initialise (position, units, disposition, docking_mark, position_mode);
			}
		}
		
		public void RecycleTab(Properties.TabProperty tab)
		{
			this.Detach (tab);
		}
		
		
		public string[] GetTabTags()
		{
			string[] tags = new string[this.tab_hash.Count];
			this.tab_hash.Keys.CopyTo (tags, 0);
			System.Array.Sort (tags);
			return tags;
		}
		
		
		public Properties.TabProperty GetTabProperty(string tag)
		{
			if (this.tab_hash.Contains (tag))
			{
				return new Properties.TabProperty (tag);
			}
			else
			{
				return null;
			}
		}
		
		
		public double GetTabPosition(Properties.TabProperty tab)
		{
			return this.GetTabRecord (tab).Position;
		}
		
		public double GetTabPositionInPoints(Properties.TabProperty tab)
		{
			return this.GetTabRecord (tab).PositionInPoints;
		}

		public Properties.SizeUnits GetTabUnits(Properties.TabProperty tab)
		{
			return this.GetTabRecord (tab).Units;
		}
		
		public double GetTabDisposition(Properties.TabProperty tab)
		{
			return this.GetTabRecord (tab).Disposition;
		}
		
		public string GetTabDockingMark(Properties.TabProperty tab)
		{
			return this.GetTabRecord (tab).DockingMark;
		}
		
		public TabPositionMode GetTabPositionMode(Properties.TabProperty tab)
		{
			return this.GetTabRecord (tab).PositionMode;
		}
		
		public long GetTabVersion(Properties.TabProperty tab)
		{
			TabRecord record = this.GetTabRecord (tab);
			
			lock (record)
			{
				if (record.Version == 0)
				{
					record.Version = this.Version;
				}
				
				return record.Version;
			}
		}
		
		
		#region TabRecord Class
		private class TabRecord
		{
			public TabRecord(double position, Properties.SizeUnits units, double disposition, string docking_mark, TabPositionMode position_mode)
			{
				this.Initialise (position, units, disposition, docking_mark, position_mode);
			}
			
			
			public double						Position
			{
				get
				{
					return this.position;
				}
			}
			
			public double						PositionInPoints
			{
				get
				{
					return Properties.UnitsTools.ConvertToPoints (this.position, this.units);
				}
			}
			
			public Properties.SizeUnits			Units
			{
				get
				{
					return this.units;
				}
			}
			
			public double						Disposition
			{
				get
				{
					return this.disposition;
				}
			}
			
			public string						DockingMark
			{
				get
				{
					return this.docking_mark;
				}
			}
			
			public TabPositionMode				PositionMode
			{
				get
				{
					return this.position_mode;
				}
			}
			
			public long							Version
			{
				get
				{
					return this.version;
				}
				set
				{
					this.version = value;
				}
			}
			
			
			public void Initialise(double position, Properties.SizeUnits units, double disposition, string docking_mark, TabPositionMode position_mode)
			{
				this.position      = position;
				this.units         = units;
				this.disposition   = disposition;
				this.docking_mark  = docking_mark;
				this.position_mode = position_mode;
				
				this.version = 0;
			}
			
			
			private double						position;
			private Properties.SizeUnits		units;
			private double						disposition;				//	0.0 = aligné à gauche, 0.5 = centré, 1.0 = aligné à droite
			private string						docking_mark;				//	"." = aligne sur le point décimal
			private TabPositionMode				position_mode;
			private long						version;
		}
		#endregion
		
		private string GenerateUniqueName()
		{
			lock (this)
			{
				return string.Format (System.Globalization.CultureInfo.InvariantCulture, "#ID#{0}", this.unique_id++);
			}
		}
		
		
		private TabRecord GetTabRecord(Properties.TabProperty tab)
		{
			return this.tab_hash[tab.TabTag] as TabRecord;
		}
		
		private void Attach(Properties.TabProperty tab, TabRecord record)
		{
			string tag = tab.TabTag;
			
			if (this.tab_hash.Contains (tag))
			{
				throw new System.ArgumentException (string.Format ("TabProperty named {0} already exists", tag), "tab");
			}
			
			this.tab_hash[tag] = record;
		}
		
		private void Detach(Properties.TabProperty tab)
		{
			string tag = tab.TabTag;
			
			if (this.tab_hash.Contains (tag))
			{
				this.tab_hash.Remove (tag);
			}
			else
			{
				throw new System.ArgumentException (string.Format ("TabProperty named {0} does not exist", tag), "tab");
			}
		}
		
		
		private System.Collections.Hashtable	tab_hash;
		private long							unique_id;
	}
}
