//	Copyright © 2005-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
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
		
		
		public OpenType.FontName[] GetFontUse(FontNaming fontNaming)
		{
			PropertyWrapper[] wrappers = this.GetPropertyUsage (Properties.WellKnownType.Font);

			List<OpenType.FontName> list = new List<OpenType.FontName> ();
			
			foreach (PropertyWrapper wrapper in wrappers)
			{
				Properties.FontProperty fontProperty = wrapper.Value as Properties.FontProperty;
				
				string fontFace  = fontProperty.FaceName;
				string fontStyle = OpenType.FontCollection.GetStyleHash (fontProperty.StyleName);

				OpenType.FontName name = new OpenType.FontName (fontFace, fontStyle);
				OpenType.FontIdentity id = OpenType.FontCollection.Default[name];

				if (id != null)
				{
					switch (fontNaming)
					{
						case FontNaming.Invariant:
							name = new OpenType.FontName (id.InvariantFaceName, id.InvariantStyleName);
							break;
						
						case FontNaming.Localized:
							name = new OpenType.FontName (id.LocaleFaceName, id.LocaleStyleName);
							break;
					}
				}
				
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
				Properties.ImageProperty imageProperty = wrapper.Value as Properties.ImageProperty;
				
				if (hash.Contains (imageProperty.ImageTag) == false)
				{
					hash[imageProperty.ImageTag] = imageProperty;
				}
			}
			
			string[] names = new string[hash.Count];
			
			hash.Keys.CopyTo (names, 0);
			System.Array.Sort (names);
			
			return names;
		}
		
		public string[] GetTabsUse()
		{
			PropertyWrapper[] wrappers1 = this.GetPropertyUsage (Properties.WellKnownType.Tabs);
			PropertyWrapper[] wrappers2 = this.GetPropertyUsage (Properties.WellKnownType.Tab);
			
			System.Collections.Hashtable hash = new System.Collections.Hashtable ();
			
			foreach (PropertyWrapper wrapper in wrappers1)
			{
				Properties.TabsProperty tabsProperty = wrapper.Value as Properties.TabsProperty;
				
				foreach (string tag in tabsProperty.TabTags)
				{
					if (hash.Contains (tag) == false)
					{
						hash[tag] = tabsProperty;
					}
				}
			}
			
			foreach (PropertyWrapper wrapper in wrappers2)
			{
				Properties.TabProperty tabProperty = wrapper.Value as Properties.TabProperty;
				
				if (hash.Contains (tabProperty.TabTag) == false)
				{
					hash[tabProperty.TabTag] = tabProperty;
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
			Styles.CoreSettings  coreSettings;
			Styles.LocalSettings localSettings;
			Styles.ExtraSettings extraSettings;
			
			this.settings.GetCoreAndSettings (bits, out coreSettings, out localSettings, out extraSettings);
			
			if (coreSettings != null)
			{
				foreach (Property property in coreSettings.GetProperties ())
				{
					this.Record (property, count);
				}
			}
			
			if (localSettings != null)
			{
				foreach (Property property in localSettings.GetProperties ())
				{
					this.Record (property, count);
				}
			}
			
			if (extraSettings != null)
			{
				foreach (Property property in extraSettings.GetProperties ())
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

		public enum FontNaming
		{
			Invariant,
			Localized
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
