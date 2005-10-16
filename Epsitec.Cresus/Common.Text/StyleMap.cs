//	Copyright © 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text
{
	/// <summary>
	/// La classe StyleMap permet de faire correspondre des styles à des noms
	/// de haut niveau, tels que vus par l'utilisateur.
	/// </summary>
	public sealed class StyleMap
	{
		internal StyleMap(StyleList list)
		{
			this.style_list = list;
			this.t_style_hash = new System.Collections.Hashtable ();
			this.caption_hash = new System.Collections.Hashtable ();
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

		public string GetCaption(TextStyle style)
		{
			string key = this.GetKeyName (style);

			if (this.t_style_hash.Contains (key))
			{
				return this.t_style_hash[key] as string;
			}

			return null;
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
		
		
		private StyleList						style_list;
		private System.Collections.Hashtable	t_style_hash;
		private System.Collections.Hashtable	caption_hash;
	}
}
