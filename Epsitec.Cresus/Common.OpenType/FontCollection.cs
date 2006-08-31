//	Copyright © 2005-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.OpenType
{
	public delegate void FontIdentityCallback(FontIdentity fid);
	
	/// <summary>
	/// The <c>FontCollection</c> class manages the collection of available
	/// fonts.
	/// </summary>
	public sealed class FontCollection : IEnumerable<FontIdentity>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="FontCollection"/> class.
		/// </summary>
		public FontCollection()
		{
			this.fontDict = new Dictionary<string, FontIdentity> ();
			this.fullDict = new Dictionary<string, FontIdentity> ();
			this.fuidDict = new Dictionary<string, FontIdentity> ();
			this.fullList = new List<FontIdentity> ();
		}


		/// <summary>
		/// Gets the <see cref="FontIdentity"/> with the specified name.
		/// </summary>
		/// <value>The <see cref="FontIdentity"/> or <c>null</c> if it does
		/// not exist in the collection.</value>
		public FontIdentity						this[string name]
		{
			get
			{
				lock (this.localExclusion)
				{
					FontIdentity value;

					if (this.fullDict.TryGetValue (name, out value))
					{
						return value;
					}
					else
					{
						return null;
					}
				}
			}
		}

		/// <summary>
		/// Gets the <see cref="FontIdentity"/> with the specified name.
		/// </summary>
		/// <value>The <see cref="FontIdentity"/> or <c>null</c> if it does
		/// not exist in the collection.</value>
		public FontIdentity this[FontName name]
		{
			get
			{
				lock (this.localExclusion)
				{
					string faceName = name.FaceName;
					string styleHash = FontCollection.GetStyleHash (name.StyleName);
					
					foreach (FontIdentity fid in this.fullList)
					{
						if ((fid.InvariantFaceName == faceName) &&
							(fid.InvariantStyleHash == styleHash))
						{
							return fid;
						}
					}
				}
				
				return null;
			}
		}


		/// <summary>
		/// Gets or sets a value indicating whether to load True Type Collections
		/// (TTC files).
		/// </summary>
		/// <value>
		///		<c>true</c> if True Type Collections should be loaded; otherwise, <c>false</c>.
		/// </value>
		public static bool						LoadTrueTypeCollections
		{
			get
			{
				return FontCollection.loadTtc;
			}
			set
			{
				FontCollection.loadTtc = value;
			}
		}

		/// <summary>
		/// Gets the default font collection object.
		/// </summary>
		/// <value>The default font collection object.</value>
		public static FontCollection			Default
		{
			get
			{
				if (FontCollection.defaultCollection == null)
				{
					lock (FontCollection.globalExclusion)
					{
						if (FontCollection.defaultCollection == null)
						{
							FontCollection.defaultCollection = new FontCollection ();
						}
					}
				}
				
				return FontCollection.defaultCollection;
			}
		}


		/// <summary>
		/// Initializes this font collection object. If font identities have
		/// already been loaded by <see cref="LoadFromCache()"/>.
		/// </summary>
		/// <returns><c>true</c> if the font collection has changed; otherwise, <c>false</c>.</returns>
		public bool Initialize()
		{
			lock (this.localExclusion)
			{
				return this.LockedInitialize ();
			}
		}

		/// <summary>
		/// Refreshes the cache.
		/// </summary>
		/// <returns><c>true</c> if the contents of the cache changed; otherwise, <c>false</c>.</returns>
		public bool RefreshCache()
		{
			return this.RefreshCache (null);
		}

		/// <summary>
		/// Refreshes the cache.
		/// </summary>
		/// <param name="callback">A callback called on every saved font identity.</param>
		/// <returns><c>true</c> if the contents of the cache changed; otherwise, <c>false</c>.</returns>
		public bool RefreshCache(FontIdentityCallback callback)
		{
			lock (this.localExclusion)
			{
				this.LockedLoadFromCache ();

				if (this.LockedInitialize ())
				{
					return this.LockedSaveToCache (callback);
				}
				else
				{
					return false;
				}
			}
		}

		/// <summary>
		/// Saves the current state of the font collection to the disk cache.
		/// </summary>
		/// <returns><c>true</c> if the cache could be written; otherwise, <c>false</c>.</returns>
		public bool SaveToCache()
		{
			return this.SaveToCache (null);
		}

		/// <summary>
		/// Saves the current state of the font collection to the disk cache.
		/// </summary>
		/// <param name="callback">A callback called on every saved font identity.</param>
		/// <returns><c>true</c> if the cache could be written; otherwise, <c>false</c>.</returns>
		public bool SaveToCache(FontIdentityCallback callback)
		{
			lock (this.localExclusion)
			{
				return this.LockedSaveToCache (callback);
			}
		}

		/// <summary>
		/// Loads the font identity information from the disk cache.
		/// </summary>
		public void LoadFromCache()
		{
			lock (this.localExclusion)
			{
				this.LockedLoadFromCache ();
			}
		}

		/// <summary>
		/// Enumerates the font families.
		/// </summary>
		/// <returns></returns>
		public IEnumerable<string> GetFontFamilies()
		{
			lock (this.localExclusion)
			{
				return (string[]) this.families.Clone ();
			}
		}

		/// <summary>
		/// Gets the <see cref="FontIdentity"/> with the specified unique font
		/// identifier.
		/// </summary>
		/// <returns>The <see cref="FontIdentity"/> or <c>null</c> if it does
		/// not exist in the collection.</returns>
		public FontIdentity FindFontByUniqueFontIdentifier(string fuid)
		{
			lock (this.localExclusion)
			{
				FontIdentity value;

				if (this.fuidDict.TryGetValue (fuid, out value))
				{
					return value;
				}
				else
				{
					return null;
				}
			}
		}

		/// <summary>
		/// Creates the font object based on the font face and font style.
		/// </summary>
		/// <param name="face">The font face.</param>
		/// <param name="style">The font style.</param>
		/// <returns>The font object or <c>null</c> if no font can be found.</returns>
		public Font CreateFont(string face, string style)
		{
			lock (this.localExclusion)
			{
				Font font = this.LockedCreateFont (face, style);

				if (font == null)
				{
					font = this.LockedCreateFont (face, string.Concat (style, " -Bold"));
				}

				if (font == null)
				{
					font = this.LockedCreateFont (face, string.Concat (style, " -Italic"));
				}

				if (font == null)
				{
					font = this.LockedCreateFont (face, string.Concat (style, " -Bold -Italic"));
				}

				if (font == null)
				{
					//	Zut. On n'a toujours rien trouvé de tel... Il faut absolument
					//	trouver quelque chose :

					foreach (FontIdentity identity in this.fullList)
					{
						if (identity.InvariantFaceName == face)
						{
							return this.CreateFont (identity);
						}
					}

					//	Mince, cette fonte n'existe vraiment pas !

					font = this.LockedCreateFont ("Arial", style);
				}

				if (font == null)
				{
					font = this.LockedCreateFont ("Arial", "Regular");
				}

				System.Diagnostics.Debug.Assert (font != null);

				return font;
			}
		}

		/// <summary>
		/// Creates the font object based on the full font name.
		/// </summary>
		/// <param name="font">The full font name.</param>
		/// <returns>The font object or <c>null</c> if no font can be found.</returns>
		public Font CreateFont(string font)
		{
			return this.CreateFont (this[font]);
		}

		/// <summary>
		/// Creates the font object based on the font identity.
		/// </summary>
		/// <param name="font">The font identity.</param>
		/// <returns>The font object or <c>null</c> if no font can be found.</returns>
		public Font CreateFont(FontIdentity font)
		{
			if ((font == null) ||
				(font.FontData == null))
			{
				return null;
			}

			return new Font (font);
		}


		/// <summary>
		/// Registers the font as a dynamic font.
		/// </summary>
		/// <param name="data">The font data.</param>
		/// <returns>the font identity of the newly registered font; otherwise, <c>null</c>.</returns>
		public FontIdentity RegisterDynamicFont(byte[] data)
		{
			FontIdentity fid = FontIdentity.CreateDynamicFont (data);

			int name_t_offset = fid.FontData["name"].Offset;
			int name_t_length = fid.FontData["name"].Length;

			fid.DefineTableName (new Table_name (data, name_t_offset), name_t_length);
			
			if (this.fullDict.ContainsKey (fid.FullName))
			{
				return null;
			}

			this.Add (fid);
			this.RefreshFullList ();
			
			return fid;
		}
		
		/// <summary>
		/// Gets the style hash, which is a simplified version of the style
		/// name.
		/// </summary>
		/// <param name="style">The raw style name.</param>
		/// <returns>The hashed style name.</returns>
		public static string GetStyleHash(string style)
		{
			//	Le "hash" d'un style de fonte correspond à une forme simplifiée
			//	et triée des éléments constituant un nom de style. On évite des
			//	problèmes de comparaison liés à des permutations, etc.

			//	En plus, le nom de style peut contenir des éléments "*Xyz" où "x"
			//	peut être "+", "-" ou "!" pour ajouter, supprimer ou inverser un
			//	style tel que "Bold" ou "Italic".

			if (string.IsNullOrEmpty (style))
			{
				return style;
			}

			string[] parts = style.Split (' ');

			int bold   = 0;
			int italic = 0;

			List<string> list = new List<string> ();

			foreach (string part in parts)
			{
				if (part.Length > 0)
				{
					switch (part)
					{
						case "Regular":
							break;
						case "Normal":
							break;
						case "Roman":
							break;

						case "Bold":
							bold  = 1;
							break;
						case "+Bold":
							bold += 1;
							break;
						case "-Bold":
							bold -= 1;
							break;

						case "Italic":
							italic  = 1;
							break;
						case "+Italic":
							italic += 1;
							break;
						case "-Italic":
							italic -= 1;
							break;

						case "!Bold":
							bold   = (bold > 0)   ? bold-1   : bold+1;
							break;
						case "!Italic":
							italic = (italic > 0) ? italic-1 : italic+1;
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

			return string.Join (" ", list.ToArray ());
		}


		#region IEnumerable Members
		
		public IEnumerator<FontIdentity> GetEnumerator()
		{
			return this.fullList.GetEnumerator ();
		}
		
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return this.fullList.GetEnumerator ();
		}

		#endregion

		private bool LockedInitialize()
		{
			this.changes = 0;

			Dictionary<string, FontIdentity> delete = new Dictionary<string, FontIdentity> ();
			
			foreach (FontIdentity fid in this.fullList)
			{
				delete[fid.InternalFontName] = fid;
			}
			
			this.families = Platform.Neutral.GetFontFamilies ();
			
			foreach (string family in this.families)
			{
				string[] styles = Platform.Neutral.GetFontStyles (family);

				if (styles.Length == 0)
				{
					System.Diagnostics.Debug.WriteLine (string.Format ("{0} has no styles", family));
				}
				
				foreach (string style in styles)
				{
					string internalFontName = FontCollection.GetInternalFontName (family, style);
					
					delete.Remove (internalFontName);
					
					if (this.fontDict.ContainsKey (internalFontName))
					{
						continue;
					}
					
					byte[] dataName = Platform.Neutral.LoadFontDataNameTable (family, style);
					
					Table_name name_t = null;
					object     record = Platform.Neutral.GetFontSystemDescription (family, style);
					
					string fullName = null;
					string fuidName = null;
					
					if (dataName == null)
					{
						//	If the file is a TrueType Collection, there will be no available name
						//	table. We will have to access it by other means.
						
						if ((record != null) &&
							(FontCollection.LoadTrueTypeCollections))
						{
							FontIdentity fid  = new FontIdentity (record, 0);
							Table_ttcf   ttcf = fid.FontData.TrueTypeCollectionTable;
							
							int    num  = ttcf.NumFonts;
							byte[] data = ttcf.BaseData;
							
							for (int i = 0; i < num; i++)
							{
								FontData  fontData = new FontData (data, i);
								FontIdentity fid_n = new FontIdentity (fontData, record, i);
								int  name_t_offset = fid_n.FontData["name"].Offset;
								int  name_t_length = fid_n.FontData["name"].Length;
								
								name_t    = new Table_name (data, name_t_offset);
								fullName = name_t.GetFullFontName ();
								fuidName = name_t.GetUniqueFontIdentifier ();
								
								fid_n.DefineTableName (name_t, name_t_length);
								fid_n.DefineSystemFontFamilyAndStyle (family, style);
								
								this.Add (fullName, fuidName, fid_n);
							}
						}
					}
					else
					{
						//	Standard font file, easy to process...
						
						name_t   = new Table_name (dataName, 0);
						fullName = name_t.GetFullFontName ();
						fuidName = name_t.GetUniqueFontIdentifier ();
						
						if ((record != null) &&
							(fullName != null) &&
							(this.fullDict.ContainsKey (fullName) == false))
						{
							FontIdentity fid = new FontIdentity (name_t, dataName.Length, record);
							
							fid.DefineSystemFontFamilyAndStyle (family, style);
							
							this.Add (fullName, fuidName, fid);
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

		private bool LockedSaveToCache(FontIdentityCallback callback)
		{
			string appDataPath  = System.Environment.GetFolderPath (System.Environment.SpecialFolder.ApplicationData);
			string cacheDirPath = System.IO.Path.Combine (appDataPath, FontCollection.CacheFolderName);
			
			try
			{
				System.IO.Directory.CreateDirectory (cacheDirPath);
			}
			catch
			{
				//	Never mind if we fail...
			}
			
			string path = System.IO.Path.Combine (cacheDirPath, FontCollection.CacheFileName);
			
			try
			{
				System.IO.File.Delete (path);
			}
			catch
			{
				//	Never mind if we fail...
			}
			
			try
			{
				using (System.IO.FileStream file = new System.IO.FileStream (path, System.IO.FileMode.CreateNew, System.IO.FileAccess.Write))
				{
					using (System.IO.Stream compressor = Epsitec.Common.IO.Compression.CreateDeflateStream (file, 1))
					{
						foreach (FontIdentity fid in this.fullList)
						{
							if (fid.IsDynamicFont)
							{
								//	Never store dynamic fonts into the cache.
								
								continue;
							}
							
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

		private void LockedLoadFromCache()
		{
			string appDataPath  = System.Environment.GetFolderPath (System.Environment.SpecialFolder.ApplicationData);
			string cacheDirPath = System.IO.Path.Combine (appDataPath, FontCollection.CacheFolderName);
			string path         = System.IO.Path.Combine (cacheDirPath, FontCollection.CacheFileName);
			
			try
			{
				using (System.IO.FileStream file = new System.IO.FileStream (path, System.IO.FileMode.Open, System.IO.FileAccess.Read))
				{
					string name;
					using (System.IO.Stream decompressor = Epsitec.Common.IO.Decompression.CreateStream (file, out name))
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
				//	Never mind...
			}
			
			this.RefreshFullList ();
		}

		private Font LockedCreateFont(string face, string style)
		{
			//	Pour trouver la fonte correspondante, on se base sur le "hash"
			//	du nom de style, ce qui permet d'être plus souple dans le cas
			//	des fontes à variantes.
			
			string hash = FontCollection.GetStyleHash (style);
			
			foreach (FontIdentity identity in this.fullList)
			{
				if ((identity.InvariantFaceName == face) &&
					(identity.InvariantStyleHash == hash))
				{
					return this.CreateFont (identity);
				}
			}
			
			return null;
		}

		private void Add(FontIdentity fid)
		{
			string fullName = fid.FullName;
			string fuidName = fid.UniqueFontId;

			this.Add (fullName, fuidName, fid);
		}
		
		private void Add(string fullName, string fuidName, FontIdentity fid)
		{
			if (this.fullDict.ContainsKey (fullName) == false)
			{
				string fontName = fid.InternalFontName;
				
				this.fontDict[fontName] = fid;
				this.fullDict[fullName] = fid;
				this.fuidDict[fuidName] = fid;
				this.changes++;
			}
		}
		
		private void Remove(FontIdentity fid)
		{
			this.fontDict.Remove (fid.InternalFontName);
			this.fullDict.Remove (fid.FullName);
			this.fuidDict.Remove (fid.UniqueFontId);
			
			this.changes++;
		}
		
		private void RefreshFullList()
		{
			this.fullList.Clear ();
			
			foreach (string name in this.fullDict.Keys)
			{
				this.fullList.Add (this[name]);
			}

			//	For every font identity, find out how many other font identities
			//	belong to the same font face :

			Dictionary<string, int> faceCount = new Dictionary<string, int> ();
			
			foreach (FontIdentity fid in this.fullDict.Values)
			{
				string face = fid.InvariantFaceName;
				
				if (faceCount.ContainsKey (face))
				{
					faceCount[face] = faceCount[face] + 1;
				}
				else
				{
					faceCount[face] = 1;
				}
			}
			
			foreach (FontIdentity fid in this.fullDict.Values)
			{
				fid.DefineFontStyleCount (faceCount[fid.InvariantFaceName]);
			}
			
			this.fullList.Sort (FontIdentity.Comparer);
		}

		internal static string GetInternalFontName(string family, string style)
		{
			return string.Concat (family, "/", style);
		}
		
		
		private static object					globalExclusion = new object ();
		private static bool						loadTtc = true;
		private static FontCollection			defaultCollection;
		
		private static string CacheFolderName	= "Epsitec Cache";
		private static string CacheFileName		= "OpenType.FontCollection.2.0.data";
		
		private object							localExclusion = new object ();
		private Dictionary<string, FontIdentity> fontDict;
		private Dictionary<string, FontIdentity> fullDict;
		private Dictionary<string, FontIdentity> fuidDict;
		private List<FontIdentity>				fullList;
		private string[]						families;
		private int								changes;
	}
}
