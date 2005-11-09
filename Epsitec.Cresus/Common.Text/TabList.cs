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
			return this.NewTab (tag, position, units, disposition, null, TabPositionMode.Absolute, null);
		}
		
		public Properties.TabProperty NewTab(string tag, double position, Properties.SizeUnits units, double disposition, string docking_mark, TabPositionMode position_mode)
		{
			return this.NewTab (tag, position, units, disposition, docking_mark, position_mode, null);
		}
		
		public Properties.TabProperty NewTab(string tag, double position, Properties.SizeUnits units, double disposition, string docking_mark, TabPositionMode position_mode, string attribute)
		{
			if (tag == null)
			{
				tag = this.GetAutoTagName ();
			}
			
			Properties.TabProperty tab = new Properties.TabProperty (tag);
			
			this.Attach (tab, new TabRecord (position, units, disposition, docking_mark, position_mode, attribute));
			
			return tab;
		}
		
		
		public void RedefineTab(Properties.TabProperty tab, double position, Properties.SizeUnits units, double disposition, string docking_mark, TabPositionMode position_mode, string attribute)
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
				record.Initialise (position, units, disposition, docking_mark, position_mode, attribute);
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
		
		
		public string GetAutoTagName()
		{
			return this.GenerateUniqueName (TabList.AutoTagPrefix);
		}
		
		public string GetSharedTagName()
		{
			return this.GenerateUniqueName (TabList.SharedTagPrefix);
		}
		
		
		public TabClass GetTabClass(Properties.TabProperty tab)
		{
			string tag = tab.TabTag;
			
			if ((tag != null) &&
				(tag.Length > 3))
			{
				switch (tag.Substring (0, 3))
				{
					case "#A#":	return TabClass.Auto;
					case "#S#":	return TabClass.Shared;
				}
			}
			
			return TabClass.Unknown;
		}
		
		internal void Serialize(System.Text.StringBuilder buffer)
		{
			int count = this.tab_hash.Count;
			
			buffer.Append (SerializerSupport.SerializeLong (this.unique_id));
			buffer.Append ("/");
			buffer.Append (SerializerSupport.SerializeInt (count));
			
			foreach (System.Collections.DictionaryEntry entry in this.tab_hash)
			{
				string    tag    = entry.Key as string;
				TabRecord record = entry.Value as TabRecord;
				
				buffer.Append ("/");
				buffer.Append (SerializerSupport.SerializeString (tag));
				buffer.Append ("/");
				record.Serialize (buffer);
			}
		}
		
		internal void Deserialize(TextContext context, int version, string[] args, ref int offset)
		{
			this.unique_id = SerializerSupport.DeserializeLong (args[offset++]);
			
			int count = SerializerSupport.DeserializeInt (args[offset++]);
			
			this.tab_hash = new System.Collections.Hashtable ();
			
			for (int i = 0; i < count; i++)
			{
				string    tag    = SerializerSupport.DeserializeString (args[offset++]);
				TabRecord record = new TabRecord ();
				
				record.Deserialize (context, version, args, ref offset);
				
				Properties.TabProperty tab = new Properties.TabProperty (tag);
				
				this.Attach (tab, record);
			}
		}
		
		
		#region TabRecord Class
		private class TabRecord
		{
			public TabRecord()
			{
			}
			
			public TabRecord(double position, Properties.SizeUnits units, double disposition, string docking_mark, TabPositionMode position_mode, string attribute)
			{
				this.Initialise (position, units, disposition, docking_mark, position_mode, attribute);
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
			
			public string						Attribute
			{
				get
				{
					return this.attribute;
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
			
			
			public void Initialise(double position, Properties.SizeUnits units, double disposition, string docking_mark, TabPositionMode position_mode, string attribute)
			{
				this.position      = position;
				this.units         = units;
				this.disposition   = disposition;
				this.docking_mark  = docking_mark;
				this.position_mode = position_mode;
				this.attribute     = attribute;
				
				this.version = 0;
			}
			
			public void Serialize(System.Text.StringBuilder buffer)
			{
				buffer.Append (SerializerSupport.SerializeDouble (this.position));
				buffer.Append ("/");
				buffer.Append (SerializerSupport.SerializeSizeUnits (this.units));
				buffer.Append ("/");
				buffer.Append (SerializerSupport.SerializeDouble (this.disposition));
				buffer.Append ("/");
				buffer.Append (SerializerSupport.SerializeString (this.docking_mark));
				buffer.Append ("/");
				buffer.Append (SerializerSupport.SerializeEnum (this.position_mode));
				buffer.Append ("/");
				buffer.Append (SerializerSupport.SerializeString (this.attribute));
			}
			
			public void Deserialize(TextContext context, int version, string[] args, ref int offset)
			{
				this.position      = SerializerSupport.DeserializeDouble (args[offset++]);
				this.units         = SerializerSupport.DeserializeSizeUnits (args[offset++]);
				this.disposition   = SerializerSupport.DeserializeDouble (args[offset++]);
				this.docking_mark  = SerializerSupport.DeserializeString (args[offset++]);
				this.position_mode = (TabPositionMode) SerializerSupport.DeserializeEnum (typeof (TabPositionMode), args[offset++]);
				this.attribute     = SerializerSupport.DeserializeString (args[offset++]);
			}
			
			
			private double						position;
			private Properties.SizeUnits		units;
			private double						disposition;				//	0.0 = aligné à gauche, 0.5 = centré, 1.0 = aligné à droite
			private string						docking_mark;				//	"." = aligne sur le point décimal
			private TabPositionMode				position_mode;
			private string						attribute;
			private long						version;
		}
		#endregion
		
		private string GenerateUniqueName(string prefix)
		{
			lock (this)
			{
				return string.Format (System.Globalization.CultureInfo.InvariantCulture, "{0}{1}", prefix, this.unique_id++);
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
		
		private const string					AutoTagPrefix = "#A#";
		private const string					SharedTagPrefix = "#S#";
	}
}
