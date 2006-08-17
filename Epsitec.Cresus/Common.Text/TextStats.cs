//	Copyright © 2005-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Text
{
	/// <summary>
	/// La classe TextStats permet d'accéder à des statistiques relatives à un
	/// TextStory.
	/// </summary>
	public class TextStats
	{
		public TextStats(TextStory story)
		{
			this.story    = story;
			this.context  = this.story.TextContext;
			this.settings = this.context.StyleList.InternalSettingsTable;
		}
		
		
		public void Clear()
		{
			this.properties = null;
		}
		
		
		public OpenType.FontName[] GetFontUse()
		{
			PropertyWrapper[] wrappers = this.GetPropertyUsage (Properties.WellKnownType.Font);

			List<OpenType.FontName> list = new List<OpenType.FontName> ();
			
			foreach (PropertyWrapper wrapper in wrappers)
			{
				Properties.FontProperty font_property = wrapper.Value as Properties.FontProperty;
				
				string font_face  = font_property.FaceName;
				string font_style = OpenType.FontCollection.GetStyleHash (font_property.StyleName);
				
				OpenType.FontName name = new OpenType.FontName (font_face, font_style);
				
				if (list.Contains (name) == false)
				{
					list.Add (name);
				}
			}

			list.Sort ();
			
			return list.ToArray ();
		}
		
		public string[] GetImageUse()
		{
			PropertyWrapper[] wrappers = this.GetPropertyUsage (Properties.WellKnownType.Image);
			
			System.Collections.Hashtable hash = new System.Collections.Hashtable ();
			
			foreach (PropertyWrapper wrapper in wrappers)
			{
				Properties.ImageProperty image_property = wrapper.Value as Properties.ImageProperty;
				
				if (hash.Contains (image_property.ImageTag) == false)
				{
					hash[image_property.ImageTag] = image_property;
				}
			}
			
			string[] names = new string[hash.Count];
			
			hash.Keys.CopyTo (names, 0);
			System.Array.Sort (names);
			
			return names;
		}
		
		public string[] GetTabsUse()
		{
			PropertyWrapper[] wrappers_1 = this.GetPropertyUsage (Properties.WellKnownType.Tabs);
			PropertyWrapper[] wrappers_2 = this.GetPropertyUsage (Properties.WellKnownType.Tab);
			
			System.Collections.Hashtable hash = new System.Collections.Hashtable ();
			
			foreach (PropertyWrapper wrapper in wrappers_1)
			{
				Properties.TabsProperty tabs_property = wrapper.Value as Properties.TabsProperty;
				
				foreach (string tag in tabs_property.TabTags)
				{
					if (hash.Contains (tag) == false)
					{
						hash[tag] = tabs_property;
					}
				}
			}
			
			foreach (PropertyWrapper wrapper in wrappers_2)
			{
				Properties.TabProperty tab_property = wrapper.Value as Properties.TabProperty;
				
				if (hash.Contains (tab_property.TabTag) == false)
				{
					hash[tab_property.TabTag] = tab_property;
				}
			}
			
			string[] names = new string[hash.Count];
			
			hash.Keys.CopyTo (names, 0);
			System.Array.Sort (names);
			
			return names;
		}
		
		
		public PropertyWrapper[] GetPropertyUsage(Properties.WellKnownType type)
		{
			this.Update ();

			int n = (int) Properties.WellKnownType.TotalCount;
			int i = (int) type;
			
			if ((i < n) &&
				(i > 0) &&
				(this.properties[i] != null))
			{
				return this.properties[i].GetProperties ();
			}
			
			return new PropertyWrapper[0];
		}
		
		
		private void Update()
		{
			if (this.properties == null)
			{
				int count  = (int) Properties.WellKnownType.TotalCount;
				int length = this.story.TextLength;
				
				this.properties = new PropertyBag[count];
				
				ulong[] buffer = new ulong[length];
				ulong   cache  = 0;
				
				this.story.ReadText (0, length, buffer);
				
				count = 0;
				
				foreach (ulong code in buffer)
				{
					ulong bits = Internal.CharMarker.ExtractCoreAndSettings (code);
					
					if (bits != cache)
					{
						if (count > 0)
						{
							//	Génère les statistiques pour ce style et ces réglages.
							
							this.Record (cache, count);
						}
						
						cache = bits;
						count = 1;
					}
				}
				
				if (count > 0)
				{
					this.Record (cache, count);
				}
			}
		}
		
		
		private void Record(ulong bits, int count)
		{
			Styles.CoreSettings  core_settings;
			Styles.LocalSettings local_settings;
			Styles.ExtraSettings extra_settings;
			
			this.settings.GetCoreAndSettings (bits, out core_settings, out local_settings, out extra_settings);
			
			if (core_settings != null)
			{
				foreach (Property property in core_settings.GetProperties ())
				{
					this.Record (property, count);
				}
			}
			
			if (local_settings != null)
			{
				foreach (Property property in local_settings.GetProperties ())
				{
					this.Record (property, count);
				}
			}
			
			if (extra_settings != null)
			{
				foreach (Property property in extra_settings.GetProperties ())
				{
					this.Record (property, count);
				}
			}
		}
		
		private void Record(Property property, int count)
		{
			int n = (int) Properties.WellKnownType.TotalCount;
			int i = (int) property.WellKnownType;
			
			if ((i < n) &&
				(i > 0))
			{
				if (this.properties[i] == null)
				{
					this.properties[i] = new PropertyBag ();
				}
				
				this.properties[i].Record (property, count);
			}
		}
		
		
		#region PropertyBag Class
		private sealed class PropertyBag
		{
			public PropertyBag()
			{
				this.hash = new System.Collections.Hashtable ();
			}
			
			
			public void Record(Property property, int count)
			{
				PropertyWrapper wrapper = new PropertyWrapper (property);
				
				if (this.hash.Contains (wrapper))
				{
					wrapper = this.hash[wrapper] as PropertyWrapper;
					wrapper.IncrementCount (count);
				}
				else
				{
					this.hash[wrapper] = wrapper;
					wrapper.IncrementCount (count);
				}
			}
			
			public PropertyWrapper[] GetProperties()
			{
				PropertyWrapper[] wrappers = new PropertyWrapper[this.hash.Count];
				this.hash.Values.CopyTo (wrappers, 0);
				return wrappers;
			}
			
			
			System.Collections.Hashtable		hash;
		}
		#endregion
		
		#region PropertyWrapper Class
		public sealed class PropertyWrapper
		{
			public PropertyWrapper(Property property)
			{
				this.property = property;
			}
			
			
			public Property						Value
			{
				get
				{
					return this.property;
				}
			}
			
			public int							Count
			{
				get
				{
					return this.count;
				}
				set
				{
					this.count = value;
				}
			}
			
			
			public void IncrementCount(int n)
			{
				this.count += n;
			}
			
			
			public override int GetHashCode()
			{
				return this.property.GetContentsSignature ();
			}
			
			public override bool Equals(object obj)
			{
				PropertyWrapper that = obj as PropertyWrapper;
				
				if (that == null) return false;
				if (that == this) return true;
				
				return (this.GetHashCode () == that.GetHashCode ())
					&& (this.property.CompareEqualContents (that.property));
			}
			
			
			private Property					property;
			private int							count;
		}
		#endregion
		
		private TextStory						story;
		private TextContext						context;
		private Internal.SettingsTable			settings;
		
		private PropertyBag[]					properties;
	}
}
