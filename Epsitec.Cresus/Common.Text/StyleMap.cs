//	Copyright © 2005-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
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
			this.style_list = list;
			this.t_style_hash = new System.Collections.Hashtable ();
			this.caption_hash = new System.Collections.Hashtable ();
			this.rank_hash    = new System.Collections.Hashtable ();
		}

		
		public void SetCaption(Common.Support.OpletQueue queue, TextStyle style, string caption)
		{
			string key = this.GetKeyName (style);
			
			int    old_rank    = this.GetRank (style);
			string old_caption = this.GetCaption (style);
			string new_caption = caption;
			
			if (old_caption != new_caption)
			{
				if (old_caption != null)
				{
					this.caption_hash.Remove (old_caption);
				}

				if (new_caption != null)
				{
					this.caption_hash[new_caption] = key;
					this.t_style_hash[key] = new_caption;
				}
				else
				{
					this.t_style_hash.Remove (key);
				}
				
				if (queue != null)
				{
					using (queue.BeginAction ())
					{
						queue.Insert (new ChangeOplet (this, style, old_caption, old_rank));
						queue.ValidateAction ();
					}
				}
			}
			
			System.Diagnostics.Debug.Assert (this.GetCaption (style) == new_caption);
			System.Diagnostics.Debug.Assert (new_caption == null || this.GetTextStyle (new_caption) == style);
		}

		public void SetRank(Common.Support.OpletQueue queue, TextStyle style, int rank)
		{
			int    old_rank    = this.GetRank (style);
			int    new_rank    = rank;
			string old_caption = this.GetCaption (style);
			
			if (old_rank != new_rank)
			{
				if (new_rank < 0)
				{
					if (new_rank != -1)
					{
						throw new System.ArgumentOutOfRangeException ("rank", new_rank, string.Format ("Rank {0} not allowed", new_rank));
					}
				
					if (this.rank_hash.Contains (style))
					{
						this.rank_hash.Remove (style);
						this.ClearCache ();
					}
				}
				else
				{
					this.rank_hash[style] = new_rank;
					this.ClearCache ();
				}
				
				if (queue != null)
				{
					using (queue.BeginAction ())
					{
						queue.Insert (new ChangeOplet (this, style, old_caption, old_rank));
						queue.ValidateAction ();
					}
				}
			}
		}
		
		
		public string GetCaption(TextStyle style)
		{
			string key = this.GetKeyName (style);

			if (this.t_style_hash.Contains (key))
			{
				return this.t_style_hash[key] as string;
			}

			return null;
		}

		public int    GetRank(TextStyle style)
		{
			if (this.rank_hash.Contains (style))
			{
				return (int) this.rank_hash[style];
			}
			
			return -1;
		}
		
		
		public TextStyle GetTextStyle(string caption)
		{
			string key = this.caption_hash[caption] as string;

			if (key != null)
			{
				return this.FindTextStyle (key);
			}

			return null;
		}

		public TextStyle GetTextStyle(int rank)
		{
			foreach (System.Collections.DictionaryEntry entry in this.rank_hash)
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
			return (TextStyle[]) this.sorted_list.Clone ();
		}
		
		
		internal void Serialize(System.Text.StringBuilder buffer)
		{
			System.Diagnostics.Debug.Assert (this.t_style_hash.Count == this.caption_hash.Count);
			
			SerializerSupport.SerializeStringStringHash (this.t_style_hash, buffer);
			buffer.Append ("/");
			
			System.Collections.Hashtable hash = new System.Collections.Hashtable ();
			
			foreach (System.Collections.DictionaryEntry entry in this.rank_hash)
			{
				TextStyle key  = entry.Key as TextStyle;
				string    name = StyleList.GetFullName (key);
				
				hash[name] = entry.Value;
			}
			
			SerializerSupport.SerializeStringIntHash (hash, buffer);
		}
		
		internal void Deserialize(TextContext context, int version, string[] args, ref int offset)
		{
			this.t_style_hash = new System.Collections.Hashtable ();
			this.caption_hash = new System.Collections.Hashtable ();
			this.rank_hash    = new	System.Collections.Hashtable ();
			this.sorted_list  = null;
			
			System.Collections.Hashtable hash = new System.Collections.Hashtable ();
			
			SerializerSupport.DeserializeStringStringHash (args, ref offset, this.t_style_hash);
			SerializerSupport.DeserializeStringIntHash (args, ref offset, hash);
			
			foreach (System.Collections.DictionaryEntry entry in hash)
			{
				string    name = entry.Key as string;
				TextStyle key  = this.style_list.GetTextStyle (name);
				
				this.rank_hash[key] = entry.Value;
			}
			
			//	Construit encore le dictionnaire inverse utilisé pour retrouver
			//	rapidement un style d'après son nom (caption) :
			
			foreach (System.Collections.DictionaryEntry entry in this.t_style_hash)
			{
				string key   = entry.Key as string;
				string value = entry.Value as string;
				
				this.caption_hash[value] = key;
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
			
			TextStyleClass text_style_class = (TextStyleClass) System.Enum.Parse (typeof (TextStyleClass), s[s.Length-1]);
			
			return this.style_list[name, text_style_class];
		}
		
		private void ClearCache()
		{
			this.sorted_list = null;
		}
		
		private void UpdateCache()
		{
			if (this.sorted_list == null)
			{
				lock (this)
				{
					if (this.sorted_list == null)
					{
						int n = this.rank_hash.Count;
						
						int[]       ranks  = new int[n];
						TextStyle[] styles = new TextStyle[n];
						
						this.rank_hash.Values.CopyTo (ranks, 0);
						this.rank_hash.Keys.CopyTo (styles, 0);
						
						System.Array.Sort (ranks, styles);
						
						this.sorted_list = styles;
					}
				}
			}
		}
		
		
		#region IEnumerable Members
		public System.Collections.IEnumerator GetEnumerator()
		{
			this.UpdateCache ();
			return this.sorted_list.GetEnumerator ();
		}
		#endregion

		#region ChangeOplet Class
		private class ChangeOplet : Common.Support.AbstractOplet
		{
			public ChangeOplet(StyleMap map, TextStyle style, string old_caption, int old_rank)
			{
				this.map     = map;
				this.style   = style;
				this.caption = old_caption;
				this.rank    = old_rank;
			}
			
			public override Epsitec.Common.Support.IOplet Undo()
			{
				string new_caption = this.caption;
				string old_caption = this.map.GetCaption (this.style);
				
				int new_rank = this.rank;
				int old_rank = this.map.GetRank (this.style);
				
				this.map.SetCaption (null, this.style, new_caption);
				this.map.SetRank (null, this.style, new_rank);
				
				this.caption = old_caption;
				this.rank    = old_rank;
				
				return this;
			}
			
			public override Epsitec.Common.Support.IOplet Redo()
			{
				return this.Undo ();
			}
			
			
			private StyleMap					map;
			private TextStyle					style;
			private int							rank;
			private string						caption;
		}
		#endregion
		
		private StyleList						style_list;
		private System.Collections.Hashtable	t_style_hash;		//	text style -> caption
		private System.Collections.Hashtable	caption_hash;		//	caption -> text style
		private System.Collections.Hashtable	rank_hash;			//	rank -> text style
		private TextStyle[]						sorted_list;
	}
}
