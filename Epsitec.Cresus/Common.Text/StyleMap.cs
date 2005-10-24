//	Copyright � 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text
{
	/// <summary>
	/// La classe StyleMap permet de faire correspondre des styles � des noms
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

		
		public void SetCaption(TextStyle style, string caption)
		{
			string key = this.GetKeyName (style);
			
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
			}
			
			System.Diagnostics.Debug.Assert (this.GetCaption (style) == new_caption);
			System.Diagnostics.Debug.Assert (new_caption == null || this.GetTextStyle (new_caption) == style);
		}

		public void SetRank(TextStyle style, int rank)
		{
			if (rank < 0)
			{
				if (rank != -1)
				{
					throw new System.ArgumentOutOfRangeException ("rank", rank, string.Format ("Rank {0} not allowed", rank));
				}
				
				if (this.rank_hash.Contains (style))
				{
					this.rank_hash.Remove (style);
					this.ClearCache ();
				}
			}
			else if (this.GetRank (style) != rank)
			{
				this.rank_hash[style] = rank;
				this.ClearCache ();
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
		
		
		private StyleList						style_list;
		private System.Collections.Hashtable	t_style_hash;
		private System.Collections.Hashtable	caption_hash;
		private System.Collections.Hashtable	rank_hash;
		private TextStyle[]						sorted_list;
	}
}
