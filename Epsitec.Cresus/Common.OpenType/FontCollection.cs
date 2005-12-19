//	Copyright © 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.OpenType
{
	public delegate void FontIdentityCallback(FontIdentity fid);
	
	/// <summary>
	/// La classe FontCollection gère une collection de fontes, donnant accès
	/// aux fontes individuelles.
	/// </summary>
	public sealed class FontCollection : System.Collections.IEnumerable
	{
		public FontCollection()
		{
			this.font_hash = new System.Collections.Hashtable ();
			this.full_hash = new System.Collections.Hashtable ();
			this.fuid_hash = new System.Collections.Hashtable ();
			this.full_list = new System.Collections.ArrayList ();
		}
		
		
		public FontIdentity						this[string name]
		{
			get
			{
				return this.full_hash[name] as FontIdentity;
			}
		}
		
		
		public static bool						LoadTTC
		{
			get
			{
				return FontCollection.load_ttc;
			}
			set
			{
				FontCollection.load_ttc = value;
			}
		}
		
		public static FontCollection			Default
		{
			get
			{
				if (FontCollection.default_collection == null)
				{
					FontCollection.default_collection = new FontCollection ();
				}
				
				return FontCollection.default_collection;
			}
		}
		
		public bool Initialize()
		{
			//	Initialise la table des fontes (FontIdentity) en complétant ce qui
			//	est déjà connu (par ex. parce que chargé via un LoadFromCache).
			//	Retourne 'true' s'il y a eu du changement.
			
			this.changes = 0;
			
			System.Collections.Hashtable delete = new System.Collections.Hashtable ();
			
			foreach (FontIdentity fid in this.full_list)
			{
				delete[fid.OsFontName] = fid;
			}
			
			this.families = Platform.Neutral.GetFontFamilies ();
			
			foreach (string family in this.families)
			{
				string[] styles = Platform.Neutral.GetFontStyles (family);
				
				foreach (string style in styles)
				{
					string os_font_name = FontCollection.GetInternalOsFontName (family, style);
					
					delete.Remove (os_font_name);
					
					if (this.font_hash.Contains (os_font_name))
					{
						continue;
					}
					
					byte[] data_name = Platform.Neutral.LoadFontDataNameTable (family, style);
					
					Table_name name_t = null;
					object     record = Platform.Neutral.GetFontSystemDescription (family, style);
					
					string full_name = null;
					string fuid_name = null;
					
					if (data_name == null)
					{
						//	La table des noms n'est pas disponible si c'est une fonte
						//	de type TTC (TrueType Collection). On doit donc s'y prendre
						//	autrement.
						
						if ((record != null) &&
							(FontCollection.LoadTTC))
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
								int  name_t_length = fid_n.FontData["name"].Length;
								
								name_t    = new Table_name (data, name_t_offset);
								full_name = name_t.GetFullFontName ();
								fuid_name = name_t.GetUniqueFontIdentifier ();
								
								fid_n.DefineTableName (name_t, name_t_length);
								fid_n.DefineSystemFontFamilyAndStyle (family, style);
								
								this.Add (full_name, fuid_name, fid_n);
							}
						}
					}
					else
					{
						//	Fonte normale, simple à gérer :
						
						name_t    = new Table_name (data_name, 0);
						full_name = name_t.GetFullFontName ();
						fuid_name = name_t.GetUniqueFontIdentifier ();
						
						if ((record != null) &&
							(full_name != null) &&
							(this.full_hash.ContainsKey (full_name) == false))
						{
							FontIdentity fid = new FontIdentity (name_t, data_name.Length, record);
							
							fid.DefineSystemFontFamilyAndStyle (family, style);
							
							this.Add (full_name, fuid_name, fid);
						}
					}
				}
			}
			
			foreach (FontIdentity fid in delete.Values)
			{
				this.Remove (fid);
			}
			
			this.RefreshFullList ();
			
			return (this.changes > 0);
		}
		
		
		public bool RefreshCache()
		{
			return this.RefreshCache (null);
		}
		
		public bool RefreshCache(FontIdentityCallback callback)
		{
			//	Retourne true si le contenu du cache a été modifié.
			
			System.Diagnostics.Debug.WriteLine ("Loading font collection from cache.");
			
			this.LoadFromCache ();
			
			System.Diagnostics.Debug.WriteLine ("Done.");
			
			if (this.Initialize ())
			{
				System.Diagnostics.Debug.WriteLine ("Updating cache on disk.");
				
				return this.SaveToCache (callback);
			}
			
			return false;
		}
		
		public bool SaveToCache()
		{
			return this.SaveToCache (null);
		}
		
		public bool SaveToCache(FontIdentityCallback callback)
		{
			string app_data_path = System.Environment.GetFolderPath (System.Environment.SpecialFolder.ApplicationData);
			string cache_dir_path = System.IO.Path.Combine (app_data_path, "Epsitec Cache");
			
			try
			{
				System.IO.Directory.CreateDirectory (cache_dir_path);
			}
			catch
			{
			}
			
			string path = System.IO.Path.Combine (cache_dir_path, FontCollection.CacheFileName);
			
			try
			{
				System.IO.File.Delete (path);
			}
			catch
			{
			}
			
			try
			{
				using (System.IO.FileStream file = new System.IO.FileStream (path, System.IO.FileMode.CreateNew, System.IO.FileAccess.Write))
				{
					using (System.IO.Stream compressor = IO.Compression.CreateDeflateStream(file, 1))
					{
						foreach (FontIdentity fid in this.full_list)
						{
							if (callback != null)
							{
								callback (fid);
							}
						
							FontIdentity.Serialize (compressor, fid);
						}
						
						compressor.Flush ();
					}
				}
			}
			catch
			{
				return false;
			}
			
			return true;
		}
		
		public void LoadFromCache()
		{
			string app_data_path  = System.Environment.GetFolderPath (System.Environment.SpecialFolder.ApplicationData);
			string cache_dir_path = System.IO.Path.Combine (app_data_path, "Epsitec Cache");
			string path           = System.IO.Path.Combine (cache_dir_path, FontCollection.CacheFileName);
			
			try
			{
				using (System.IO.FileStream file = new System.IO.FileStream (path, System.IO.FileMode.Open, System.IO.FileAccess.Read))
				{
					string name;
					using (System.IO.Stream decompressor = IO.Decompression.CreateStream (file, out name))
					{
						FontIdentity fid = FontIdentity.Deserialize (decompressor);
						
						while (fid != null)
						{
							this.Add (fid.FullName, fid.UniqueFontId, fid);
							fid = FontIdentity.Deserialize (decompressor);
						}
					}
				}
			}
			catch
			{
			}
			
			this.RefreshFullList ();
		}
		
		
		public string[] GetFontFamilies()
		{
			return (string[]) this.families.Clone ();
		}
		
		
		public FontIdentity FindFontByUniqueFontIdentifier(string fuid)
		{
			return this.fuid_hash[fuid] as FontIdentity;
		}
		
		
		public Font CreateFont(string face, string style)
		{
			Font font = this.InternalCreateFont (face, style);
			
			if (font == null)
			{
				font = this.InternalCreateFont (face, string.Concat (style, " -Bold"));
			}
			
			if (font == null)
			{
				font = this.InternalCreateFont (face, string.Concat (style, " -Italic"));
			}
			
			if (font == null)
			{
				font = this.InternalCreateFont (face, string.Concat (style, " -Bold -Italic"));
			}
			
			if (font == null)
			{
				//	Zut. On n'a toujours rien trouvé de tel... Il faut absolument
				//	trouver quelque chose :
				
				foreach (FontIdentity identity in this.full_list)
				{
					if (identity.InvariantFaceName == face)
					{
						return this.CreateFont (identity);
					}
				}
				
				//	Mince, cette fonte n'existe vraiment pas !
				
				font = this.InternalCreateFont ("Arial", style);
			}
			
			if (font == null)
			{
				font = this.InternalCreateFont ("Arial", "Regular");
			}
			
			System.Diagnostics.Debug.Assert (font != null);
			
			return font;
		}
		
		
		internal Font InternalCreateFont(string face, string style)
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
		
		public static string GetStyleHash(string style)
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
		
		public static string GetInternalOsFontName(string family, string style)
		{
			return string.Concat (family, "/", style);
		}
		
		
		private void Add(string full_name, string fuid_name, FontIdentity fid)
		{
			if (this.full_hash.ContainsKey (full_name) == false)
			{
				string font_name = fid.OsFontName;
				
				this.font_hash[font_name] = fid;
				this.full_hash[full_name] = fid;
				this.fuid_hash[fuid_name] = fid;
				this.changes++;
			}
		}
		
		private void Remove(FontIdentity fid)
		{
			this.font_hash.Remove (fid.OsFontName);
			this.full_hash.Remove (fid.FullName);
			this.fuid_hash.Remove (fid.UniqueFontId);
			this.changes++;
		}
		
		private void RefreshFullList()
		{
			this.full_list.Clear ();
			
			foreach (string name in this.full_hash.Keys)
			{
				this.full_list.Add (this[name]);
			}
			
			System.Collections.Hashtable hash = new System.Collections.Hashtable ();
			
			foreach (FontIdentity fid in this.full_hash.Values)
			{
				string face = fid.InvariantFaceName;
				
				if (hash.Contains (face))
				{
					hash[face] = (int) hash[face] + 1;
				}
				else
				{
					hash[face] = 1;
				}
			}
			
			foreach (FontIdentity fid in this.full_hash.Values)
			{
				fid.DefineFontStyleCount ((int) hash[fid.InvariantFaceName]);
			}
			
			this.full_list.Sort (FontIdentity.Comparer);
		}
		
		
		private static bool						load_ttc;
		private static FontCollection			default_collection;
		private static string					CacheFileName = "OpenType.FontCollection.1.1.data";
		
		private System.Collections.Hashtable	font_hash;
		private System.Collections.Hashtable	full_hash;
		private System.Collections.Hashtable	fuid_hash;
		private System.Collections.ArrayList	full_list;
		private string[]						families;
		private int								changes;
	}
}
