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
			this.families = Platform.Win32.GetFontFamilies ();
			
			foreach (string family in this.families)
			{
				string[] styles = Platform.Win32.GetFontStyles (family);
				
				foreach (string style in styles)
				{
					Table_name name_t = new Table_name (Platform.Win32.LoadFontDataNameTable (family, style), 0);
					
					object record    = Platform.Win32.GetFontSystemDescription (family, style);
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
