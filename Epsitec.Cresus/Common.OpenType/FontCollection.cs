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
					byte[] data_name = Platform.Neutral.LoadFontDataNameTable (family, style);
					
					Table_name name_t = null;
					object     record = Platform.Neutral.GetFontSystemDescription (family, style);
					
					string full_name = null;
					
					if (data_name == null)
					{
						//	La table des noms n'est pas disponible si c'est une fonte
						//	de type TTC (TrueType Collection). On doit donc s'y prendre
						//	autrement.
						
						if (record != null)
						{
							FontIdentity fid  = new FontIdentity (record, 0);
							Table_ttcf   ttcf = fid.FontData.TrueTypeCollection;
							
							int    num  = ttcf.NumFonts;
							byte[] data = ttcf.BaseData;
							
							for (int i = 0; i < num; i++)
							{
								FontData font_data = new FontData (data, i);
								FontIdentity fid_n = new FontIdentity (font_data, record, i);
								int  name_t_offset = fid_n.FontData["name"].Offset;
								
								name_t    = new Table_name (data, name_t_offset);
								full_name = name_t.GetFullFontName ();
								
								fid_n.DefineTableName (name_t);
								
								if (this.full_hash.ContainsKey (full_name) == false)
								{
									this.full_hash[full_name] = fid_n;
								}
							}
						}
					}
					else
					{
						//	Fonte normale, simple à gérer :
						
						name_t    = new Table_name (data_name, 0);
						full_name = name_t.GetFullFontName ();
						
						if ((record != null) &&
							(full_name != null) &&
							(this.full_hash.ContainsKey (full_name) == false))
						{
							this.full_hash[full_name] = new FontIdentity (name_t, record);
						}
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
		
		
		public Font CreateFont(string face, string style)
		{
			//	Pour trouver la fonte correspondante, on se base sur le "hash"
			//	du nom de style, ce qui permet d'être plus souple dans le cas
			//	des fontes à variantes.
			
			string hash = FontCollection.GetStyleHash (style);
			
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
		
		internal static string GetStyleHash(string style)
		{
			//	Le "hash" d'un style de fonte correspond à une forme simplifiée
			//	et triée des éléments constituant un nom de style. On évite des
			//	problèmes de comparaison liés à des permutations, etc.

			//	En plus, le nom de style peut contenir des éléments "*Xyz" où "x"
			//	peut être "+", "-" ou "!" pour ajouter, supprimer ou inverser un
			//	style tel que "Bold" ou "Italic".
			
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
						case "Regular":	break;
						case "Normal":	break;
						case "Roman":	break;
						
						case "Bold":	bold  = 1;	break;
						case "+Bold":	bold += 1;	break;
						case "-Bold":	bold -= 1;	break;
						
						case "Italic":	italic  = 1; break;
						case "+Italic":	italic += 1; break;
						case "-Italic":	italic -= 1; break;
						
						case "!Bold":	bold   = (bold > 0)   ? bold-1   : bold+1;	 break;
						case "!Italic":	italic = (italic > 0) ? italic-1 : italic+1; break;
						
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
			
			return string.Join (" ", (string[]) list.ToArray (typeof (string)));
		}
		
		
		private System.Collections.Hashtable	full_hash;
		private System.Collections.ArrayList	full_list;
		private string[]						families;
	}
}
