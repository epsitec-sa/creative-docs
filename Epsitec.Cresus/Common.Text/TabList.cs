//	Copyright � 2005-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text
{
	/// <summary>
	/// La classe TabList g�re la liste globale des tabulateurs.
	/// </summary>
	public sealed class TabList
	{
		public TabList(TextContext context)
		{
			this.context  = context;
			this.tag_hash = new System.Collections.Hashtable ();
			this.auto_tab_hash = new System.Collections.Hashtable ();
			this.shared_tab_hash = new System.Collections.Hashtable ();
		}
		
		
		public long								Version
		{
			get
			{
				return this.version.Current;
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
				tag = this.GenerateAutoTagName ();
			}
			else if (tag == "shared")
			{
				tag = this.GenerateSharedTagName ();
			}
			
			Properties.TabProperty tab = new Properties.TabProperty (tag);
			
			this.Attach (new TabRecord (tag, position, units, disposition, docking_mark, position_mode, attribute));
			
			return tab;
		}
		
		
		public Properties.TabProperty FindAutoTab(double position, Properties.SizeUnits units, double disposition, string docking_mark, TabPositionMode position_mode, string attribute)
		{
			TabRecord find = new TabRecord ("?", position, units, disposition, docking_mark, position_mode, attribute);
			
			if (this.auto_tab_hash.Contains (find))
			{
				find = this.auto_tab_hash[find] as TabRecord;
				
				return this.GetTabProperty (find.Tag);
			}
			
			return null;
		}
		
		public Properties.TabProperty FindSharedTab(double position, Properties.SizeUnits units, double disposition, string docking_mark, TabPositionMode position_mode, string attribute)
		{
			TabRecord find = new TabRecord ("?", position, units, disposition, docking_mark, position_mode, attribute);
			
			if (this.shared_tab_hash.Contains (find))
			{
				find = this.shared_tab_hash[find] as TabRecord;
				
				return this.GetTabProperty (find.Tag);
			}
			
			return null;
		}
		
		
		public Properties.TabProperty CreateOrGetAutoTab(double position, Properties.SizeUnits units, double disposition, string docking_mark, TabPositionMode position_mode, string attribute)
		{
			Properties.TabProperty find = this.FindAutoTab (position, units, disposition, docking_mark, position_mode, attribute);
			
			if (find == null)
			{
				find = this.NewTab (null, position, units, disposition, docking_mark, position_mode, attribute);
			}
			
			return find;
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
			
			this.version.ChangeVersion ();
			
			lock (record)
			{
				this.Detach (record.Tag);
				record.Initialise (tab.TabTag, position, units, disposition, docking_mark, position_mode, attribute);
				this.Attach (record);
			}
		}
		
		public void RecycleTab(Properties.TabProperty tab)
		{
			this.Detach (tab.TabTag);
		}
		
		
		public string[] GetTabTags()
		{
			string[] tags = new string[this.tag_hash.Count];
			this.tag_hash.Keys.CopyTo (tags, 0);
			System.Array.Sort (tags);
			return tags;
		}
		
		public string[] GetUnusedTabTags()
		{
			System.Collections.ArrayList list = new System.Collections.ArrayList ();
			
			foreach (TabRecord record in this.auto_tab_hash.Values)
			{
				if (record.UserCount == 0)
				{
					list.Add (record.Tag);
				}
			}
			
			foreach (TabRecord record in this.shared_tab_hash.Values)
			{
				if (record.UserCount == 0)
				{
					list.Add (record.Tag);
				}
			}
			
			return (string[]) list.ToArray (typeof (string));
		}
		
		
		public void ClearUnusedTabTags()
		{
			System.Collections.ArrayList list = new System.Collections.ArrayList ();
			
			foreach (TabRecord record in this.auto_tab_hash.Values)
			{
				if (record.UserCount == 0)
				{
					list.Add (record.Tag);
				}
			}
			
			foreach (TabRecord record in this.shared_tab_hash.Values)
			{
				if (record.UserCount == 0)
				{
					list.Add (record.Tag);
				}
			}
			
			foreach (string tag in list)
			{
				this.Detach (tag);
			}
		}
		
		
		public Properties.TabProperty GetTabProperty(string tag)
		{
			if (this.tag_hash.Contains (tag))
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
		
		public string GetTabAttribute(Properties.TabProperty tab)
		{
			return this.GetTabRecord (tab).Attribute;
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
		
		public int GetTabUserCount(Properties.TabProperty tab)
		{
			return this.GetTabRecord (tab).UserCount;
		}
		
		
		public void IncrementTabUserCount(Properties.TabProperty tab)
		{
			this.GetTabRecord (tab).IncrementUserCount (1);
		}
		
		public void IncrementTabUserCount(string tag)
		{
			this.GetTabRecord (tag).IncrementUserCount (1);
		}
		
		public void DecrementTabUserCount(Properties.TabProperty tab)
		{
			this.GetTabRecord (tab).DecrementUserCount ();
		}
		
		public void DecrementTabUserCount(string tag)
		{
			this.GetTabRecord (tag).DecrementUserCount ();
		}
		
		
		internal void IncreaseTabUserCount(Properties.TabProperty tab, int n)
		{
			this.GetTabRecord (tab).IncrementUserCount (n);
		}
		
		
		public string GenerateAutoTagName()
		{
			//	Il y a deux classes de tabulateurs : ceux qui sont d�finis localement
			//	dans le texte (TabClass.Auto) et ceux qui sont d�finis au sein d'un
			//	style (TabClass.Shared).
			
			return this.GenerateUniqueName (TabList.AutoTagPrefix);
		}
		
		public string GenerateSharedTagName()
		{
			return this.GenerateUniqueName (TabList.SharedTagPrefix);
		}
		
		
		public static TabClass GetTabClass(Properties.TabProperty tab)
		{
			return TabList.GetTabClass (tab == null ? null : tab.TabTag);
		}
		
		public static TabClass GetTabClass(string tag)
		{
			if ((tag != null) &&
				(tag.Length > 3))
			{
				switch (tag.Substring (0, 3))
				{
					case TabList.AutoTagPrefix:		return TabClass.Auto;
					case TabList.SharedTagPrefix:	return TabClass.Shared;
				}
			}
			
			return TabClass.Unknown;
		}
		
		
		public static Properties.TabsProperty FilterTabs(Properties.TabsProperty property, TabClass tab_class)
		{
			string[] tags = property.TabTags;
			
			int count = 0;
			
			for (int i = 0; i < tags.Length; i++)
			{
				if (TabList.GetTabClass (tags[i]) == tab_class)
				{
					count++;
				}
			}
			
			if (count > 0)
			{
				if (count == tags.Length)
				{
					return property;
				}
				
				string[] copy = new string[count];
				
				for (int i = 0, j = 0; (i < tags.Length) && (j < count); i++)
				{
					if (TabList.GetTabClass (tags[i]) == tab_class)
					{
						copy[j] = tags[i];
						j++;
					}
				}
				
				return new Properties.TabsProperty (copy);
			}
			else
			{
				return null;
			}
		}
		
		
		public Properties.TabsProperty PromoteToSharedTabs(Properties.TabsProperty property)
		{
			string[] tags = property.TabTags;
			
			if (tags.Length > 0)
			{
				for (int i = 0; i < tags.Length; i++)
				{
					TabRecord record = this.GetTabRecord (tags[i]);
					string    tag    = this.GenerateSharedTagName ();
					
					this.NewTab (tag, record.Position, record.Units, record.Disposition, record.DockingMark, record.PositionMode, record.Attribute);
					
					tags[i] = tag;
				}
				
				return new Properties.TabsProperty (tags);
			}
			else
			{
				return null;
			}
		}
		
		
		internal void Serialize(System.Text.StringBuilder buffer)
		{
			int count = this.tag_hash.Count;
			
			buffer.Append (SerializerSupport.SerializeLong (this.unique_id));
			buffer.Append ("/");
			buffer.Append (SerializerSupport.SerializeInt (count));
			
			foreach (TabRecord record in this.tag_hash.Values)
			{
				buffer.Append ("/");
				record.Serialize (buffer);
			}
		}
		
		internal void Deserialize(TextContext context, int version, string[] args, ref int offset)
		{
			this.unique_id = SerializerSupport.DeserializeLong (args[offset++]);
			
			int count = SerializerSupport.DeserializeInt (args[offset++]);
			
			this.tag_hash        = new System.Collections.Hashtable ();
			this.auto_tab_hash   = new System.Collections.Hashtable ();
			this.shared_tab_hash = new System.Collections.Hashtable ();
			
			for (int i = 0; i < count; i++)
			{
				TabRecord record = new TabRecord ();
				
				record.Deserialize (context, version, args, ref offset);
				
				string                 tag = record.Tag;
				Properties.TabProperty tab = new Properties.TabProperty (tag);
				
				this.Attach (record);
			}
			
			//	TODO: mettre � jour les compteurs d'utilisation des tabulateurs
		}
		
		
		public static string PackToAttribute()
		{
			return SerializerSupport.SerializeStringArray (new string[0]);
		}
		
		public static string PackToAttribute(params string[] values)
		{
			return SerializerSupport.SerializeStringArray (values);
		}
		
		public static string[] UnpackFromAttribute(string value)
		{
			return SerializerSupport.DeserializeStringArray (value);
		}
		
		
		public static double GetLevelOffset(double font_size_in_points, int level, string attribute)
		{
			string[] args = TabList.UnpackFromAttribute (attribute);
			double offset = 0;
			
			if (level >= 0)
			{
				for (int i = 0; i < args.Length; i++)
				{
					if (args[i] != null)
					{
						if (args[i].StartsWith (TabList.LevelMultiplier))
						{
							string value = args[i].Substring (TabList.LevelMultiplier.Length);
							offset = TabList.GetLevelOffsetFromMultiplier (font_size_in_points, level, value);
							break;
						}
						else if (args[i].StartsWith (TabList.LevelTable))
						{
							string value = args[i].Substring (TabList.LevelTable.Length);
							offset = TabList.GetLevelOffsetFromTable (font_size_in_points, level, value);
							break;
						}
					}
				}
			}
			
			return offset;
		}
		
		public static double GetRelativeOffset(double font_size_in_points, string attribute)
		{
			string[] args = TabList.UnpackFromAttribute (attribute);
			double offset = 0;
			
			for (int i = 0; i < args.Length; i++)
			{
				if (args[i] != null)
				{
					if (args[i].StartsWith (TabList.RelativeEmOffset))
					{
						string value = args[i].Substring (TabList.RelativeEmOffset.Length);
						double scale = System.Double.Parse (value, System.Globalization.CultureInfo.InvariantCulture);
						offset = scale * font_size_in_points;
						break;
					}
				}
			}
			
			return offset;
		}
		
		private static double GetLevelOffsetFromMultiplier(double font_size_in_points, int level, string value)
		{
			if (level == 0)
			{
				return 0;
			}
			else
			{
				double               multiplier;
				Properties.SizeUnits units;
				
				Properties.UnitsTools.DeserializeSizeUnits (value, out multiplier, out units);
				
				if (Properties.UnitsTools.IsScale (units))
				{
					return Properties.UnitsTools.ConvertToScale (multiplier, units) * level * font_size_in_points;
				}
				else
				{
					return Properties.UnitsTools.ConvertToPoints (multiplier, units) * level;
				}
			}
		}
		
		private static double GetLevelOffsetFromTable(double font_size_in_points, int level, string value)
		{
			string[] args = value.Split (';');
			
			level = System.Math.Min (level, args.Length-1);
			
			double               offset;
			Properties.SizeUnits units;
			
			Properties.UnitsTools.DeserializeSizeUnits (args[level], out offset, out units);
			
			if (Properties.UnitsTools.IsScale (units))
			{
				return Properties.UnitsTools.ConvertToScale (offset, units) * font_size_in_points;
			}
			else
			{
				return Properties.UnitsTools.ConvertToPoints (offset, units);
			}
		}
		
		
		#region TabRecord Class
		private class TabRecord : IContentsSignature, IContentsComparer
		{
			public TabRecord()
			{
			}
			
			public TabRecord(string tag, double position, Properties.SizeUnits units, double disposition, string docking_mark, TabPositionMode position_mode, string attribute)
			{
				this.Initialise (tag, position, units, disposition, docking_mark, position_mode, attribute);
			}
			
			
			public string						Tag
			{
				get
				{
					return this.tag;
				}
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
			
			public int							UserCount
			{
				get
				{
					return this.user_count;
				}
			}
		
			public TabClass						TabClass
			{
				get
				{
					return TabList.GetTabClass (this.tag);
				}
			}
			
			
			public void IncrementUserCount(int n)
			{
				Debug.Assert.IsInBounds (this.user_count+n, 1, TabRecord.MaxUserCount);
				
				for (int i = 0; i < n; i++)
				{
					System.Threading.Interlocked.Increment (ref this.user_count);
				}
			}
		
			public void DecrementUserCount()
			{
				Debug.Assert.IsInBounds (this.user_count, 1, TabRecord.MaxUserCount);
				System.Threading.Interlocked.Decrement (ref this.user_count);
			}
		
			
			public void Initialise(string tag, double position, Properties.SizeUnits units, double disposition, string docking_mark, TabPositionMode position_mode, string attribute)
			{
				this.tag           = tag;
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
				System.Diagnostics.Debug.WriteLine (string.Format ("Tab {0} at {1} {2} used {3} times", this.tag, (int) this.position, this.units, this.user_count));
				
				buffer.Append (SerializerSupport.SerializeString (this.tag));
				buffer.Append ("/");
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
				this.tag           = SerializerSupport.DeserializeString (args[offset++]);
				this.position      = SerializerSupport.DeserializeDouble (args[offset++]);
				this.units         = SerializerSupport.DeserializeSizeUnits (args[offset++]);
				this.disposition   = SerializerSupport.DeserializeDouble (args[offset++]);
				this.docking_mark  = SerializerSupport.DeserializeString (args[offset++]);
				this.position_mode = (TabPositionMode) SerializerSupport.DeserializeEnum (typeof (TabPositionMode), args[offset++]);
				this.attribute     = SerializerSupport.DeserializeString (args[offset++]);
			}
			
			
			#region IContentsSignature Members
			public int GetContentsSignature()
			{
				//	Ne prend en compte ni le tag, ni le compteur d'utilisation pour
				//	le calcul de la signature :
				
				if (this.signature == 0)
				{
					IO.IChecksum checksum = IO.Checksum.CreateAdler32 ();
					
					checksum.UpdateValue (this.position);
					checksum.UpdateValue ((int) this.units);
					checksum.UpdateValue (this.disposition);
					checksum.UpdateValue (this.docking_mark);
					checksum.UpdateValue ((int) this.position_mode);
					checksum.UpdateValue (this.attribute);
					
					int signature = checksum.Value.GetHashCode ();
					
					this.signature = (signature == 0) ? 1 : signature;
				}
				
				return this.signature;
			}
			#endregion
			
			#region IContentsComparer Members
			public bool CompareEqualContents(object value)
			{
				//	Ne prend en compte ni le tag, ni le compteur d'utilisation pour
				//	la comparaison :
				
				TabRecord that = value as TabRecord;
				
				if (that == null) return false;
				if (that == this) return true;
				
				if (this.GetContentsSignature () != that.GetContentsSignature ())
				{
					return false;
				}
				
				return that.position == this.position
					&& that.units == this.units
					&& that.disposition == this.disposition
					&& that.docking_mark == this.docking_mark
					&& that.position_mode == this.position_mode
					&& that.attribute == this.attribute;
			}
			#endregion
			
			public override int GetHashCode()
			{
				//	Ignore le tag dans le calcul de la valeur de hachage :
				
				return this.GetContentsSignature ();
			}
			
			public override bool Equals(object obj)
			{
				//	Ignore le nom dans la comparaison :
				
				return this.CompareEqualContents (obj);
			}


			
			public const int					MaxUserCount = 999999999;
			
			private string						tag;
			private double						position;
			private Properties.SizeUnits		units;
			private double						disposition;				//	0.0 = align� � gauche, 0.5 = centr�, 1.0 = align� � droite
			private string						docking_mark;				//	"." = aligne sur le point d�cimal
			private TabPositionMode				position_mode;
			private string						attribute;
			private long						version;
			private int							signature;
			private int							user_count;
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
			return this.tag_hash[tab.TabTag] as TabRecord;
		}
		
		private TabRecord GetTabRecord(string tag)
		{
			return this.tag_hash[tag] as TabRecord;
		}
		
		
		private void Attach(TabRecord record)
		{
			string tag = record.Tag;
			
			if (this.tag_hash.Contains (tag))
			{
				throw new System.ArgumentException (string.Format ("TabProperty named {0} already exists", tag), "record");
			}
			
			this.tag_hash[tag] = record;
			
			switch (record.TabClass)
			{
				case TabClass.Auto:
					this.auto_tab_hash[record] = record;
					break;
				
				case TabClass.Shared:
					this.shared_tab_hash[record] = record;
					break;
			}
		}
		
		private void Detach(string tag)
		{
			TabRecord record = this.GetTabRecord (tag);
			
			if (record == null)
			{
				throw new System.ArgumentException (string.Format ("TabProperty named {0} does not exist", tag), "tag");
			}
			
			System.Diagnostics.Debug.Assert (this.tag_hash.Contains (tag));
			
			this.tag_hash.Remove (tag);
			
			switch (record.TabClass)
			{
				case TabClass.Auto:
					System.Diagnostics.Debug.Assert (this.auto_tab_hash.Contains (record));
					this.auto_tab_hash.Remove (record);
					break;
				
				case TabClass.Shared:
					System.Diagnostics.Debug.Assert (this.shared_tab_hash.Contains (record));
					this.shared_tab_hash.Remove (record);
					break;
			}
		}
		
		
		private TextContext						context;
		private System.Collections.Hashtable	tag_hash;
		private System.Collections.Hashtable	auto_tab_hash;
		private System.Collections.Hashtable	shared_tab_hash;
		private long							unique_id;
		private StyleVersion					version = new StyleVersion ();
		
		private const string					AutoTagPrefix = "#A#";
		private const string					SharedTagPrefix = "#S#";
		
		public const string						LevelMultiplier = "LevelMultiplier:";
		public const string						LevelTable = "LevelTable:";
		public const string						RelativeEmOffset = "Em:";
	}
}
