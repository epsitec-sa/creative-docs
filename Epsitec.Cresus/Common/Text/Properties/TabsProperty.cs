//	Copyright © 2005-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text.Properties
{
	/// <summary>
	/// La classe TabsProperty décrit une tabulation.
	/// </summary>
	public class TabsProperty : Property
	{
		public TabsProperty()
		{
		}
		
		public TabsProperty(params TabProperty[] tabs)
		{
			string[] tags = new string[tabs.Length];
			
			for (int i = 0; i < tabs.Length; i++)
			{
				tags[i] = tabs[i].TabTag;
			}
			
			this.DefineTabTags (tags);
		}
		
		public TabsProperty(params string[] tabTags)
		{
			this.DefineTabTags (tabTags);
		}
		
		
		public override WellKnownType			WellKnownType
		{
			get
			{
				return WellKnownType.Tabs;
			}
		}
		
		public override PropertyType			PropertyType
		{
			get
			{
				return PropertyType.ExtraSetting;
			}
		}
		
		
		public override bool					RequiresUniformParagraph
		{
			get
			{
				return true;
			}
		}
		
		
		public string[]							TabTags
		{
			get
			{
				System.Collections.ArrayList list = new System.Collections.ArrayList ();
				
				for (int i = 0; i < this.tabTags.Length; i++)
				{
					if (this.tabTags[i].StartsWith ("-") == false)
					{
						list.Add (this.tabTags[i]);
					}
				}
				
				return (string[]) list.ToArray (typeof (string));
			}
		}
		
		
		public bool ContainsTabTag(string tag)
		{
			if (this.tabTags != null)
			{
				for (int i = 0; i < this.tabTags.Length; i++)
				{
					if (this.tabTags[i] == tag)
					{
						return true;
					}
				}
			}
			
			return false;
		}
		
		
		public override Property EmptyClone()
		{
			return new TabsProperty ();
		}
		
		public override void SerializeToText(System.Text.StringBuilder buffer)
		{
			SerializerSupport.Join (buffer,
				/**/				SerializerSupport.SerializeStringArray (this.tabTags));
		}
		
		public override void DeserializeFromText(TextContext context, string text, int pos, int length)
		{
			string[] args = SerializerSupport.Split (text, pos, length);
			
			Debug.Assert.IsTrue (args.Length == 1);
			
			string[] tabTags = SerializerSupport.DeserializeStringArray (args[0]);
			
			this.tabTags = tabTags;
		}
		
		
		public override Property GetCombination(Property property)
		{
			Debug.Assert.IsTrue (property is Properties.TabsProperty);
			
			TabsProperty a = this;
			TabsProperty b = property as TabsProperty;
			
			System.Collections.ArrayList tags = new System.Collections.ArrayList ();
			
			if ((a.tabTags != null) &&
				(a.tabTags.Length > 0))
			{
				foreach (string tag in a.tabTags)
				{
					if (tags.Contains (tag) == false)
					{
						tags.Add (tag);
					}
				}
			}
			
			if ((b.tabTags != null) &&
				(b.tabTags.Length > 0))
			{
				foreach (string tag in b.tabTags)
				{
					if (tag.StartsWith ("-"))
					{
						tags.Remove (tag.Substring (1));
					}
					else if (tags.Contains (tag) == false)
					{
						tags.Add (tag);
					}
				}
			}
			
			TabsProperty c = new TabsProperty ((string[]) tags.ToArray (typeof (string)));
			
			return c;
		}
		
		
		public override void UpdateContentsSignature(IO.IChecksum checksum)
		{
			checksum.UpdateValue (this.tabTags);
		}
		
		public override bool CompareEqualContents(object value)
		{
			return TabsProperty.CompareEqualContents (this, value as TabsProperty);
		}
		
		
		private void DefineTabTags(string[] tags)
		{
			this.tabTags = (string[]) tags.Clone ();
			this.Invalidate ();
		}
		
		
		private static bool CompareEqualContents(TabsProperty a, TabsProperty b)
		{
			return Types.Comparer.Equal (a.tabTags, b.tabTags);
		}
		
		
		private string[]						tabTags;
	}
}
