//	Copyright © 2005-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text
{
	/// <summary>
	/// La classe StyleMap permet de faire correspondre des styles à des noms
	/// de haut niveau, tels que vus par l'utilisateur.
	/// </summary>
	public sealed class StyleMap : System.Collections.IEnumerable
	{
		internal StyleMap(StyleList list)
		{
			this.styleList   = list;
			this.tStyleHash  = new System.Collections.Hashtable ();
			this.captionHash = new System.Collections.Hashtable ();
			this.rankHash    = new System.Collections.Hashtable ();
		}

		
		public void SetCaption(Common.Support.OpletQueue queue, TextStyle style, string caption)
		{
			string key = this.GetKeyName (style);
			
			int    oldRank    = this.GetRank (style);
			string oldCaption = this.GetCaption (style);
			string newCaption = caption;
			
			if (oldCaption != newCaption)
			{
				if (oldCaption != null)
				{
					if (this.captionHash[oldCaption] as string == key)
					{
						this.captionHash.Remove (oldCaption);
					}
				}

				if (newCaption != null)
				{
					if (this.captionHash.Contains (newCaption) == false)
					{
						this.captionHash[newCaption] = key;
						this.tStyleHash[key] = newCaption;
					}
				}
				else
				{
					this.tStyleHash.Remove (key);
				}
				
				if (queue != null)
				{
					TextStory.InsertOplet (queue, new ChangeOplet (this, style, oldCaption, oldRank));
				}
				
				this.styleList.NotifyStyleMapChanged ();
			}
			
			System.Diagnostics.Debug.Assert (this.GetCaption (style) == newCaption);
			System.Diagnostics.Debug.Assert (newCaption == null || this.GetTextStyle (newCaption) == style);
		}

		public void SetRank(Common.Support.OpletQueue queue, TextStyle style, int rank)
		{
			int    oldRank    = this.GetRank (style);
			int    newRank    = rank;
			string oldCaption = this.GetCaption (style);
			
			if (oldRank != newRank)
			{
				if (newRank < 0)
				{
					if (newRank != -1)
					{
						throw new System.ArgumentOutOfRangeException ("rank", newRank, string.Format ("Rank {0} not allowed", newRank));
					}
				
					if (this.rankHash.Contains (style))
					{
						this.rankHash.Remove (style);
						this.ClearCache ();
					}
				}
				else
				{
					this.rankHash[style] = newRank;
					this.ClearCache ();
				}
				
				if (queue != null)
				{
					TextStory.InsertOplet (queue, new ChangeOplet (this, style, oldCaption, oldRank));
				}
				
				this.styleList.NotifyStyleMapChanged ();
			}
		}
		
		
		public string GetCaption(TextStyle style)
		{
			string key = this.GetKeyName (style);

			if (this.tStyleHash.Contains (key))
			{
				return this.tStyleHash[key] as string;
			}

			return null;
		}

		public int    GetRank(TextStyle style)
		{
			if (this.rankHash.Contains (style))
			{
				return (int) this.rankHash[style];
			}
			
			return -1;
		}
		
		
		public TextStyle GetTextStyle(string caption)
		{
			string key = this.captionHash[caption] as string;

			if (key != null)
			{
				return this.FindTextStyle (key);
			}

			return null;
		}

		public TextStyle GetTextStyle(int rank)
		{
			foreach (System.Collections.DictionaryEntry entry in this.rankHash)
			{
				if ((int) entry.Value == rank)
				{
					return entry.Key as TextStyle;
				}
			}
			
			return null;
		}
		
		
		public TextStyle[] GetSortedStyles()
		{
			this.UpdateCache ();
			return (TextStyle[]) this.sortedList.Clone ();
		}
		
		
		internal void Serialize(System.Text.StringBuilder buffer)
		{
			System.Diagnostics.Debug.Assert (this.tStyleHash.Count == this.captionHash.Count);
			
			SerializerSupport.SerializeStringStringHash (this.tStyleHash, buffer);
			buffer.Append ("/");
			
			System.Collections.Hashtable hash = new System.Collections.Hashtable ();
			
			foreach (System.Collections.DictionaryEntry entry in this.rankHash)
			{
				TextStyle key  = entry.Key as TextStyle;
				string    name = StyleList.GetFullName (key);
				
				hash[name] = entry.Value;
			}
			
			SerializerSupport.SerializeStringIntHash (hash, buffer);
		}
		
		internal void Deserialize(TextContext context, int version, string[] args, ref int offset)
		{
			this.tStyleHash = new System.Collections.Hashtable ();
			this.captionHash = new System.Collections.Hashtable ();
			this.rankHash    = new	System.Collections.Hashtable ();
			this.sortedList  = null;
			
			System.Collections.Hashtable hash = new System.Collections.Hashtable ();
			
			SerializerSupport.DeserializeStringStringHash (args, ref offset, this.tStyleHash);
			SerializerSupport.DeserializeStringIntHash (args, ref offset, hash);
			
			foreach (System.Collections.DictionaryEntry entry in hash)
			{
				string    name = entry.Key as string;
				TextStyle key  = this.styleList.GetTextStyle (name);
				
				this.rankHash[key] = entry.Value;
			}
			
			//	Construit encore le dictionnaire inverse utilisé pour retrouver
			//	rapidement un style d'après son nom (caption) :
			
			foreach (System.Collections.DictionaryEntry entry in this.tStyleHash)
			{
				string key   = entry.Key as string;
				string value = entry.Value as string;
				
				this.captionHash[value] = key;
			}
		}
		
		
		private string GetKeyName(TextStyle style)
		{
			return string.Concat (style.Name, ".", style.TextStyleClass.ToString ());
		}

		private TextStyle FindTextStyle(string key)
		{
			string[] s = key.Split ('.');
			string name = string.Join (".", s, 0, s.Length-1);
			
			TextStyleClass textStyleClass = (TextStyleClass) System.Enum.Parse (typeof (TextStyleClass), s[s.Length-1]);
			
			return this.styleList[name, textStyleClass];
		}
		
		private void ClearCache()
		{
			this.sortedList = null;
		}
		
		private void UpdateCache()
		{
			if (this.sortedList == null)
			{
				lock (this)
				{
					if (this.sortedList == null)
					{
						int n = this.rankHash.Count;
						
						int[]       ranks  = new int[n];
						TextStyle[] styles = new TextStyle[n];
						
						this.rankHash.Values.CopyTo (ranks, 0);
						this.rankHash.Keys.CopyTo (styles, 0);
						
						System.Array.Sort (ranks, styles);
						
						this.sortedList = styles;
					}
				}
			}
		}
		
		
		#region IEnumerable Members
		public System.Collections.IEnumerator GetEnumerator()
		{
			this.UpdateCache ();
			return this.sortedList.GetEnumerator ();
		}
		#endregion

		#region ChangeOplet Class
		public class ChangeOplet : Common.Support.AbstractOplet
		{
			public ChangeOplet(StyleMap map, TextStyle style, string oldCaption, int oldRank)
			{
				this.map     = map;
				this.style   = style;
				this.caption = oldCaption;
				this.rank    = oldRank;
			}
			
			
			public override Epsitec.Common.Support.IOplet Undo()
			{
				string newCaption = this.caption;
				string oldCaption = this.map.GetCaption (this.style);
				
				int newRank = this.rank;
				int oldRank = this.map.GetRank (this.style);
				
				this.map.SetCaption (null, this.style, newCaption);
				this.map.SetRank (null, this.style, newRank);
				
				this.caption = oldCaption;
				this.rank    = oldRank;
				
				return this;
			}
			
			public override Epsitec.Common.Support.IOplet Redo()
			{
				return this.Undo ();
			}
			
			
			public bool MergeWith(ChangeOplet other)
			{
				if ((this.style == other.style) &&
					(this.map   == other.map))
				{
					return true;
				}
				
				return false;
			}
						
			
			private StyleMap					map;
			private TextStyle					style;
			private int							rank;
			private string						caption;
		}
		#endregion
		
		private StyleList						styleList;
		private System.Collections.Hashtable	tStyleHash;		//	text style -> caption
		private System.Collections.Hashtable	captionHash;		//	caption -> text style
		private System.Collections.Hashtable	rankHash;			//	rank -> text style
		private TextStyle[]						sortedList;
	}
}
