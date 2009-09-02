//	Copyright © 2005-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text
{
	/// <summary>
	/// La classe TabList gère la liste globale des tabulateurs.
	/// </summary>
	public sealed class TabList
	{
		public TabList(TextContext context)
		{
			this.context  = context;
			this.tagHash = new System.Collections.Hashtable ();
			this.autoTabHash = new System.Collections.Hashtable ();
			this.sharedTabHash = new System.Collections.Hashtable ();
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
		
		public Properties.TabProperty NewTab(string tag, double position, Properties.SizeUnits units, double disposition, string dockingMark, TabPositionMode positionMode)
		{
			return this.NewTab (tag, position, units, disposition, dockingMark, positionMode, null);
		}
		
		public Properties.TabProperty NewTab(string tag, double position, Properties.SizeUnits units, double disposition, string dockingMark, TabPositionMode positionMode, string attribute)
		{
			if (tag == null)
			{
				tag = this.GenerateAutoTagName ();
			}
			else if (tag == TabList.GenericSharedName)
			{
				tag = this.GenerateSharedTagName ();
			}
			
			Properties.TabProperty tab = new Properties.TabProperty (tag);
			
			this.Attach (new TabRecord (tag, position, units, disposition, dockingMark, positionMode, attribute));
			this.NotifyChanged (null);
			
			return tab;
		}
		
		
		public Properties.TabProperty FindAutoTab(double position, Properties.SizeUnits units, double disposition, string dockingMark, TabPositionMode positionMode, string attribute)
		{
			TabRecord find = new TabRecord ("?", position, units, disposition, dockingMark, positionMode, attribute);
			
			if (this.autoTabHash.Contains (find))
			{
				find = this.autoTabHash[find] as TabRecord;
				
				return this.GetTabProperty (find.Tag);
			}
			
			return null;
		}
		
		public Properties.TabProperty FindSharedTab(double position, Properties.SizeUnits units, double disposition, string dockingMark, TabPositionMode positionMode, string attribute)
		{
			TabRecord find = new TabRecord ("?", position, units, disposition, dockingMark, positionMode, attribute);
			
			if (this.sharedTabHash.Contains (find))
			{
				find = this.sharedTabHash[find] as TabRecord;
				
				return this.GetTabProperty (find.Tag);
			}
			
			return null;
		}
		
		
		public Properties.TabProperty CreateOrGetAutoTab(double position, Properties.SizeUnits units, double disposition, string dockingMark, TabPositionMode positionMode, string attribute)
		{
			Properties.TabProperty find = this.FindAutoTab (position, units, disposition, dockingMark, positionMode, attribute);
			
			if (find == null)
			{
				find = this.NewTab (null, position, units, disposition, dockingMark, positionMode, attribute);
			}
			
			return find;
		}
		
		
		public void RedefineTab(Common.Support.OpletQueue queue, string tag, double position, Properties.SizeUnits units, double disposition, string dockingMark, TabPositionMode positionMode, string attribute)
		{
			this.RedefineTab (queue, null, new Properties.TabProperty (tag), position, units, disposition, dockingMark, positionMode, attribute);
		}
		
		public void RedefineTab(Common.Support.OpletQueue queue, TextStory story, Properties.TabProperty tab, double position, Properties.SizeUnits units, double disposition, string dockingMark, TabPositionMode positionMode, string attribute)
		{
			System.Diagnostics.Debug.Assert (tab != null);
			System.Diagnostics.Debug.Assert (tab.TabTag != null);
			
			TabRecord record = this.GetTabRecord (tab);
			
			if (record == null)
			{
				throw new System.ArgumentException (string.Format ("TabProperty named {0} does not exist", tab.TabTag), "tab");
			}
			
			this.version.ChangeVersion ();
			
			if (queue != null)
			{
				if (queue.IsActionDefinitionInProgress)
				{
					TextStory.InsertOplet (queue, new RedefineOplet (this, record));
				}
				else
				{
					if (story == null)
					{
						using (queue.BeginAction ())
						{
							TextStory.InsertOplet (queue, new RedefineOplet (this, record));
							queue.ValidateAction ();
						}
					}
					else
					{
						using (story.BeginAction ())
						{
							TextStory.InsertOplet (queue, new RedefineOplet (this, record));
							story.ValidateAction ();
						}
					}
				}
			}
			
			lock (record)
			{
				int count = record.UserCount;
				
				this.Detach (record.Tag);
				record.Initialize (tab.TabTag, position, units, disposition, dockingMark, positionMode, attribute);
				this.Attach (record);
				
				System.Diagnostics.Debug.Assert (record.UserCount == count);
			}
			
			this.NotifyChanged (record);
		}
		
		public void RecycleTab(Properties.TabProperty tab)
		{
			this.Detach (tab.TabTag);
		}
		
		
		public string[] GetTabTags()
		{
			string[] tags = new string[this.tagHash.Count];
			this.tagHash.Keys.CopyTo (tags, 0);
			System.Array.Sort (tags);
			return tags;
		}
		
		public string[] GetUnusedTabTags()
		{
			System.Collections.ArrayList list = new System.Collections.ArrayList ();
			
			foreach (TabRecord record in this.autoTabHash.Values)
			{
				if (record.UserCount == 0)
				{
					list.Add (record.Tag);
				}
			}
			
			foreach (TabRecord record in this.sharedTabHash.Values)
			{
				if (record.UserCount == 0)
				{
					list.Add (record.Tag);
				}
			}
			
			return (string[]) list.ToArray (typeof (string));
		}
		
		
		public void SortTabs(string[] tags)
		{
			//	Trie la table passée en entrée selon des positions de tabulateurs
			//	croissantes.
			
			//	NB: ceci ne tient en compte que la position "simple" du tabulateur
			//	et ignore les attributs qui permettent de régler de manière plus
			//	subtile la position.
			
			double[] pos = new double[tags.Length];
			
			for (int i = 0; i < tags.Length; i++)
			{
				pos[i] = this.GetTabRecord (tags[i]).PositionInPoints;
			}
			
			System.Array.Sort (pos, tags);
		}
		
		public void CloneTabs(Property[] properties)
		{
			for (int p = 0; p < properties.Length; p++)
			{
				if (properties[p].WellKnownType == Properties.WellKnownType.Tabs)
				{
					Properties.TabsProperty tabs = properties[p] as Properties.TabsProperty;
					string[] tags = tabs.TabTags;
					
					for (int i = 0; i < tags.Length; i++)
					{
						tags[i] = this.CloneTab (tags[i]);
						
						System.Diagnostics.Debug.Assert (tags[i] != null);
						System.Diagnostics.Debug.Assert (this.GetTabRecord (tags[i]).UserCount == 0);
					}
					
					properties[p] = new Properties.TabsProperty (tags);
					this.NotifyChanged (null);
				}
			}
		}
		
		public void ClearUnusedTabTags()
		{
			System.Collections.ArrayList list = new System.Collections.ArrayList ();
			
			foreach (TabRecord record in this.autoTabHash.Values)
			{
				if (record.UserCount == 0)
				{
					list.Add (record.Tag);
				}
			}
			
			foreach (TabRecord record in this.sharedTabHash.Values)
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
			if (this.tagHash.Contains (tag))
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
			//	Il y a deux classes de tabulateurs : ceux qui sont définis localement
			//	dans le texte (TabClass.Auto) et ceux qui sont définis au sein d'un
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
		
		
		public static Properties.TabsProperty FilterTabs(Properties.TabsProperty property, TabClass tabClass)
		{
			string[] tags = property.TabTags;
			
			int count = 0;
			
			for (int i = 0; i < tags.Length; i++)
			{
				if (TabList.GetTabClass (tags[i]) == tabClass)
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
					if (TabList.GetTabClass (tags[i]) == tabClass)
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
				
				this.NotifyChanged (null);
				
				return new Properties.TabsProperty (tags);
			}
			else
			{
				return null;
			}
		}
		
		
		internal void Serialize(System.Text.StringBuilder buffer)
		{
			int count = this.tagHash.Count;
			
			buffer.Append (SerializerSupport.SerializeLong (this.uniqueId));
			buffer.Append ("/");
			buffer.Append (SerializerSupport.SerializeInt (count));
			
			foreach (TabRecord record in this.tagHash.Values)
			{
				buffer.Append ("/");
				record.Serialize (buffer);
			}
		}
		
		internal void Deserialize(TextContext context, int version, string[] args, ref int offset)
		{
			this.uniqueId = SerializerSupport.DeserializeLong (args[offset++]);
			
			int count = SerializerSupport.DeserializeInt (args[offset++]);
			
			this.tagHash        = new System.Collections.Hashtable ();
			this.autoTabHash   = new System.Collections.Hashtable ();
			this.sharedTabHash = new System.Collections.Hashtable ();
			
			for (int i = 0; i < count; i++)
			{
				TabRecord record = new TabRecord ();
				
				record.Deserialize (context, version, args, ref offset);
				
				string                 tag = record.Tag;
				Properties.TabProperty tab = new Properties.TabProperty (tag);
				
				this.Attach (record);
			}
			
			//	TODO: mettre à jour les compteurs d'utilisation des tabulateurs
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
		
		
		public static double GetLevelOffset(double fontSizeInPoints, int level, string attribute)
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
							offset = TabList.GetLevelOffsetFromMultiplier (fontSizeInPoints, level, value);
							break;
						}
						else if (args[i].StartsWith (TabList.LevelTable))
						{
							string value = args[i].Substring (TabList.LevelTable.Length);
							offset = TabList.GetLevelOffsetFromTable (fontSizeInPoints, level, value);
							break;
						}
					}
				}
			}
			
			return offset;
		}
		
		public static double GetRelativeOffset(double fontSizeInPoints, string attribute)
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
						offset = scale * fontSizeInPoints;
						break;
					}
				}
			}
			
			return offset;
		}
		
		public static string CreateLevelTable(double[] offsets)
		{
			//	Convertit une table de positions exprimées en points en un
			//	attribut utilisable pour créer un tabulateur.
			
			System.Text.StringBuilder buffer = new System.Text.StringBuilder ();
			
			buffer.Append (TabList.LevelTable);
			
			for (int i = 0; i < offsets.Length; i++)
			{
				if (i > 0)
				{
					buffer.Append (";");
				}
				
				buffer.Append (Properties.UnitsTools.SerializeSizeUnits (offsets[i], Properties.SizeUnits.Points));
			}
			
			return TabList.PackToAttribute (buffer.ToString ());
		}
		
		public static double[] ParseLevelTable(string attribute)
		{
			//	Analyse et décortique un argument créé avec CreateLevelTable
			//	et retourne un tableau des offsets en points.
			
			string[] args = TabList.UnpackFromAttribute (attribute);
			
			for (int i = 0; i < args.Length; i++)
			{
				if (args[i] != null)
				{
					if (args[i].StartsWith (TabList.LevelTable))
					{
						string   value   = args[i].Substring (TabList.LevelTable.Length);
						string[] nums    = value.Split (';');
						double[] offsets = new double[nums.Length];
						
						for (int j = 0; j < nums.Length; j++)
						{
							double offset;
							Properties.SizeUnits units;
							Properties.UnitsTools.DeserializeSizeUnits (nums[j], out offset, out units);
							offsets[j] = Properties.UnitsTools.ConvertToPoints (offset, units);
						}
						
						return offsets;
					}
				}
			}
			
			return new double[0];
		}
		
		
		private static double GetLevelOffsetFromMultiplier(double fontSizeInPoints, int level, string value)
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
					return Properties.UnitsTools.ConvertToScale (multiplier, units) * level * fontSizeInPoints;
				}
				else
				{
					return Properties.UnitsTools.ConvertToPoints (multiplier, units) * level;
				}
			}
		}
		
		private static double GetLevelOffsetFromTable(double fontSizeInPoints, int level, string value)
		{
			string[] args = value.Split (';');
			
			level = System.Math.Min (level, args.Length-1);
			
			double               offset;
			Properties.SizeUnits units;
			
			Properties.UnitsTools.DeserializeSizeUnits (args[level], out offset, out units);
			
			if (Properties.UnitsTools.IsScale (units))
			{
				return Properties.UnitsTools.ConvertToScale (offset, units) * fontSizeInPoints;
			}
			else
			{
				return Properties.UnitsTools.ConvertToPoints (offset, units);
			}
		}
		
		
		#region RedefineOplet Class
		public class RedefineOplet : Common.Support.AbstractOplet
		{
			internal RedefineOplet(TabList list, TabRecord record)
			{
				this.list   = list;
				this.record = record;
				this.state  = this.record.Save ();
			}
			
			public override Common.Support.IOplet Undo()
			{
				string oldState = this.record.Save ();
				string newState = this.state;
				
				this.record.Restore (newState);
				this.state = oldState;
				this.list.NotifyChanged (this.record);
				
				return this;
			}
			
			public override Common.Support.IOplet Redo()
			{
				return this.Undo ();
			}
			
			
			public bool MergeWith(RedefineOplet other)
			{
				if ((this.list == other.list) &&
					(this.record == other.record))
				{
					return true;
				}
				else
				{
					return false;
				}
			}
			
			
			private TabList						list;
			private TabRecord					record;
			private string						state;
		}
		#endregion
		
		#region TabRecord Class
		internal class TabRecord : IContentsSignature, IContentsComparer
		{
			public TabRecord()
			{
			}
			
			public TabRecord(string tag, double position, Properties.SizeUnits units, double disposition, string dockingMark, TabPositionMode positionMode, string attribute)
			{
				this.Initialize (tag, position, units, disposition, dockingMark, positionMode, attribute);
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
					return this.dockingMark;
				}
			}
			
			public TabPositionMode				PositionMode
			{
				get
				{
					return this.positionMode;
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
					return this.userCount;
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
				Debug.Assert.IsInBounds (this.userCount+n, 1, TabRecord.MaxUserCount);
				
				for (int i = 0; i < n; i++)
				{
					System.Threading.Interlocked.Increment (ref this.userCount);
				}
			}
		
			public void DecrementUserCount()
			{
				Debug.Assert.IsInBounds (this.userCount, 1, TabRecord.MaxUserCount);
				System.Threading.Interlocked.Decrement (ref this.userCount);
			}
		
			
			public void Initialize(string tag, double position, Properties.SizeUnits units, double disposition, string dockingMark, TabPositionMode positionMode, string attribute)
			{
				this.tag           = tag;
				this.position      = position;
				this.units         = units;
				this.disposition   = disposition;
				this.dockingMark  = dockingMark;
				this.positionMode = positionMode;
				this.attribute     = attribute;
				
				this.version = 0;
			}
			
			public void Serialize(System.Text.StringBuilder buffer)
			{
				buffer.Append (SerializerSupport.SerializeString (this.tag));
				buffer.Append ("/");
				buffer.Append (SerializerSupport.SerializeDouble (this.position));
				buffer.Append ("/");
				buffer.Append (SerializerSupport.SerializeSizeUnits (this.units));
				buffer.Append ("/");
				buffer.Append (SerializerSupport.SerializeDouble (this.disposition));
				buffer.Append ("/");
				buffer.Append (SerializerSupport.SerializeString (this.dockingMark));
				buffer.Append ("/");
				buffer.Append (SerializerSupport.SerializeEnum (this.positionMode));
				buffer.Append ("/");
				buffer.Append (SerializerSupport.SerializeString (this.attribute));
			}
			
			public void Deserialize(TextContext context, int version, string[] args, ref int offset)
			{
				this.tag           = SerializerSupport.DeserializeString (args[offset++]);
				this.position      = SerializerSupport.DeserializeDouble (args[offset++]);
				this.units         = SerializerSupport.DeserializeSizeUnits (args[offset++]);
				this.disposition   = SerializerSupport.DeserializeDouble (args[offset++]);
				this.dockingMark  = SerializerSupport.DeserializeString (args[offset++]);
				this.positionMode = (TabPositionMode) SerializerSupport.DeserializeEnum (typeof (TabPositionMode), args[offset++]);
				this.attribute     = SerializerSupport.DeserializeString (args[offset++]);
			}
			
			
			internal string Save()
			{
				System.Text.StringBuilder buffer = new System.Text.StringBuilder ();
				this.Serialize (buffer);
				return buffer.ToString ();
			}
			
			internal void Restore(string archive)
			{
				string[] args = archive.Split ('/');
				
				int offset  = 0;
				int version = TextContext.SerializationVersion;
				
				this.Deserialize (null, version, args, ref offset);
			}
			
			internal void RestoreAndRename(string archive, string tag)
			{
				this.Restore (archive);
				this.tag = tag;
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
					checksum.UpdateValue (this.dockingMark);
					checksum.UpdateValue ((int) this.positionMode);
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
					&& that.dockingMark == this.dockingMark
					&& that.positionMode == this.positionMode
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
			private double						disposition;				//	0.0 = aligné à gauche, 0.5 = centré, 1.0 = aligné à droite
			private string						dockingMark;				//	"." = aligne sur le point décimal
			private TabPositionMode				positionMode;
			private string						attribute;
			private long						version;
			private int							signature;
			private int							userCount;
		}
		#endregion
		
		private string CloneTab(string tag)
		{
			TabRecord record = this.GetTabRecord (tag);
			
			if (record == null)
			{
				return null;
			}
			
			string newTag = null;
			
			switch (record.TabClass)
			{
				case TabClass.Auto:		newTag = this.GenerateAutoTagName ();		break;
				case TabClass.Shared:	newTag = this.GenerateSharedTagName ();	break;
			}
			
			if (newTag != null)
			{
				string state = record.Save ();
				record = new TabRecord ();
				record.RestoreAndRename (state, newTag);
				this.Attach (record);
			}
			
			return newTag;
		}
		
		private string GenerateUniqueName(string prefix)
		{
			lock (this)
			{
				return string.Format (System.Globalization.CultureInfo.InvariantCulture, "{0}{1}", prefix, this.uniqueId++);
			}
		}
		
		
		private void NotifyChanged(TabRecord record)
		{
			if (record != null)
			{
				foreach (TextStory story in this.context.GetTextStories ())
				{
					TextStats stats = new TextStats (story);
					string[]  tags  = stats.GetTabsUse ();
					
					foreach (string tag in tags)
					{
						if (tag == record.Tag)
						{
							story.NotifyTextChanged ();
							break;
						}
					}
				}
			}
			
			if (this.Changed != null)
			{
				this.Changed (this);
			}
		}
		
		
		private TabRecord GetTabRecord(Properties.TabProperty tab)
		{
			return this.tagHash[tab.TabTag] as TabRecord;
		}
		
		private TabRecord GetTabRecord(string tag)
		{
			return this.tagHash[tag] as TabRecord;
		}
		
		
		private void Attach(TabRecord record)
		{
			string tag = record.Tag;
			
			if (this.tagHash.Contains (tag))
			{
				throw new System.ArgumentException (string.Format ("TabProperty named {0} already exists", tag), "record");
			}
			
			this.tagHash[tag] = record;
			
			switch (record.TabClass)
			{
				case TabClass.Auto:
					this.autoTabHash[record] = record;
					break;
				
				case TabClass.Shared:
					this.sharedTabHash[record] = record;
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
			
			System.Diagnostics.Debug.Assert (this.tagHash.Contains (tag));
			
			this.tagHash.Remove (tag);
			
			switch (record.TabClass)
			{
				case TabClass.Auto:
					System.Diagnostics.Debug.Assert (this.autoTabHash.Contains (record));
					this.autoTabHash.Remove (record);
					break;
				
				case TabClass.Shared:
					System.Diagnostics.Debug.Assert (this.sharedTabHash.Contains (record));
					this.sharedTabHash.Remove (record);
					break;
			}
		}
		
		
		public event Common.Support.EventHandler Changed;
		
		private TextContext						context;
		private System.Collections.Hashtable	tagHash;
		private System.Collections.Hashtable	autoTabHash;
		private System.Collections.Hashtable	sharedTabHash;
		private long							uniqueId;
		private StyleVersion					version = new StyleVersion ();
		
		private const string					AutoTagPrefix = "#A#";
		private const string					SharedTagPrefix = "#S#";
		
		public const string						GenericSharedName = "[shared]";
		
		public const string						LevelMultiplier = "LevelMultiplier:";
		public const string						LevelTable = "LevelTable:";
		public const string						RelativeEmOffset = "Em:";
	}
}
