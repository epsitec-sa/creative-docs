//	Copyright © 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.OpenType
{
	/// <summary>
	/// La classe FontCollection gère une collection de fontes, donnant accès
	/// aux fontes individuelles.
	/// </summary>
	public sealed class FontCollection : System.Collections.IEnumerable
	{
		public FontCollection()
		{
			this.full_hash = new System.Collections.Hashtable ();
			this.full_list = new System.Collections.ArrayList ();
		}
		
		
		public FontIdentity						this[string name]
		{
			get
			{
				return this.full_hash[name] as FontIdentity;
			}
		}
		
		
		public void Initialize()
		{
			this.families = Platform.Neutral.GetFontFamilies ();
			
			foreach (string family in this.families)
			{
				string[] styles = Platform.Neutral.GetFontStyles (family);
				
				foreach (string style in styles)
				{
					Table_name name_t = new Table_name (Platform.Neutral.LoadFontDataNameTable (family, style), 0);
					
					object record    = Platform.Neutral.GetFontSystemDescription (family, style);
					string full_name = name_t.GetFullFontName ();
					
					if ((record != null) &&
						(full_name != null))
					{
						this.full_hash[full_name] = new FontIdentity (name_t, record);
					}
				}
			}
			
			this.full_list.Clear ();
			
			foreach (string name in this.full_hash.Keys)
			{
				this.full_list.Add (this[name]);
			}
			
			this.full_list.Sort (FontIdentity.Comparer);
		}
		
		
		public string[] GetFontFamilies()
		{
			return (string[]) this.families.Clone ();
		}
		
		
		public static string GetStyleHash(string style)
		{
			string[] parts = style.Split (' ');
			
			int bold   = 0;
			int italic = 0;
			
			System.Collections.ArrayList list = new System.Collections.ArrayList ();
			
			foreach (string part in parts)
			{
				if (part.Length > 0)
				{
					switch (part)
					{
						case "Regular":
						case "Normal":
						case "Roman":
							break;
						
						case "Bold":
						case "(+Bold)":
							bold = bold + 1;
							break;
						
						case "(-Bold)":
							bold = bold - 1;
							break;
						
						case "(!Bold)":
							bold = (bold > 0) ? bold - 1 : bold + 1;
							break;
						
						case "Italic":
						case "(+Italic)":
							italic = italic + 1;
							break;
						
						case "(-Italic)":
							italic = italic - 1;
							break;
						
						case "(!Italic)":
							italic = (italic > 0) ? italic - 1 : italic + 1;
							break;
						
						default:
							if (list.Contains (part) == false)
							{
								list.Add (part);
							}
							break;
					}
				}
			}
			
			if (bold > 0)
			{
				list.Add ("Bold");
			}
			if (italic > 0)
			{
				list.Add ("Italic");
			}
			
			list.Sort ();
			
			parts = (string[]) list.ToArray (typeof (string));
			
			return string.Join (" ", parts);
		}
		
		
		public Font CreateFont(string face, string style)
		{
			string hash = FontCollection.GetStyleHash (style);
			
			System.Diagnostics.Debug.WriteLine (string.Format ("Mapping style <{0}> to <{1}>", style, hash));
			
			foreach (FontIdentity identity in this.full_list)
			{
				if ((identity.InvariantFaceName == face) &&
					(identity.InvariantStyleHash == hash))
				{
					return this.CreateFont (identity);
				}
			}
			
			return null;
		}
		
		public Font CreateFont(string font)
		{
			return this.CreateFont (this[font]);
		}
			
		public Font CreateFont(FontIdentity font)
		{
			if ((font == null) ||
				(font.FontData == null))
			{
				return null;
			}
			
			return new Font (font);
		}
		
		
		#region IEnumerable Members
		public System.Collections.IEnumerator GetEnumerator()
		{
			return this.full_list.GetEnumerator ();
		}
		#endregion
		
		private System.Collections.Hashtable	full_hash;
		private System.Collections.ArrayList	full_list;
		private string[]						families;
	}
}
