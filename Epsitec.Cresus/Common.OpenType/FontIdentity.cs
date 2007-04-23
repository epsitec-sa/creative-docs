//	Copyright © 2005-2007, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.OpenType
{
	/// <summary>
	/// The <c>FontIdentity</c> class provides detailed name information about
	/// a font.
	/// </summary>
	public sealed class FontIdentity
	{
		private FontIdentity()
		{
		}

		internal FontIdentity(Table_name openTypeNameTable, int length, object systemRecord)
			: this (openTypeNameTable, length, systemRecord, -1)
		{
		}

		internal FontIdentity(Table_name openTypeNameTable, int length, object systemRecord, int ttcIndex)
		{
			this.DefineTableName (openTypeNameTable, length);

			this.record    = systemRecord;
			this.ttcIndex = ttcIndex;
		}

		internal FontIdentity(object systemRecord, int ttcIndex)
		{
			this.record    = systemRecord;
			this.ttcIndex = ttcIndex;
		}

		internal FontIdentity(FontData fontData, object systemRecord, int ttcIndex)
			: this (systemRecord, ttcIndex)
		{
			this.fontData = fontData;
		}
		
		
		/// <summary>
		/// Gets the font face name for the current locale, using
		/// <c>CultureInfo.CurrentCulture</c>.
		/// </summary>
		/// <value>The font face name.</value>
		public string							LocaleFaceName
		{
			get
			{
				lock (this.exclusion)
				{
					string face  = this.LocalePreferredFaceName ?? this.LocaleSimpleFaceName;
					string style = this.LocalePreferredStyleName ?? this.LocaleSimpleStyleName;

					face = FontIdentity.RepairBrokenFaceName (face);
					
					if (face != null)
					{
						if (face.EndsWith (style))
						{
							face = face.Substring (0, face.Length - style.Length).Trim ();
						}
					}

					return face;
				}
			}
		}
		
		/// <summary>
		/// Gets the font style name for the current locale, using
		/// <c>CultureInfo.CurrentCulture</c>.
		/// </summary>
		/// <value>The font style name.</value>
		public string							LocaleStyleName
		{
			get
			{
				string preferred = this.LocalePreferredStyleName;
				string simple    = this.LocaleSimpleStyleName;
				string adobe     = this.LocaleAdobeStyleName;

				string localeName = FontIdentity.ComposeStyleName (preferred, simple, adobe);
				
				preferred = this.InvariantPreferredStyleName;
				simple    = this.InvariantSimpleStyleName;
				adobe     = this.InvariantAdobeStyleName;

				string invariantName = FontIdentity.ComposeStyleName (preferred, simple, adobe);

				if (localeName == invariantName)
				{
					return this.InvariantStyleName;
				}
				else
				{
					return localeName;
				}
			}
		}
		
		public string LocaleFullName
		{
			get
			{
				lock (this.exclusion)
				{
					return this.GetName (NameId.FullFontName);
				}
			}
		}


		/// <summary>
		/// Gets the font face name for the current locale, using
		/// <c>CultureInfo.CurrentCulture</c>.
		/// </summary>
		/// <value>The font face name.</value>
		public string							LocaleSimpleFaceName
		{
			get
			{
				lock (this.exclusion)
				{
					string face  = this.GetName (NameId.FontFamily);
					string style = this.GetName (NameId.FontSubfamily);

					if (face.EndsWith (style))
					{
						face = face.Substring (0, face.Length - style.Length).Trim ();
					}

					return face;
				}
			}
		}

		/// <summary>
		/// Gets the font style name for the current locale, using
		/// <c>CultureInfo.CurrentCulture</c>.
		/// </summary>
		/// <value>The font style name.</value>
		public string							LocaleSimpleStyleName
		{
			get
			{
				lock (this.exclusion)
				{
					return this.GetName (NameId.FontSubfamily);
				}
			}
		}

		/// <summary>
		/// Gets the Adobe font style name for the current locale, using
		/// <c>CultureInfo.CurrentCulture</c>.
		/// </summary>
		/// <value>The Adobe font style name.</value>
		public string LocaleAdobeStyleName
		{
			get
			{
				lock (this.exclusion)
				{
					return this.GetName (NameId.AdobeFontStyle);
				}
			}
		}



		/// <summary>
		/// Gets the preferred font face name for the current locale, using
		/// <c>CultureInfo.CurrentCulture</c>.
		/// </summary>
		/// <value>The preferred font face name or <c>null</c>.</value>
		public string							LocalePreferredFaceName
		{
			get
			{
				lock (this.exclusion)
				{
					return this.GetName (NameId.PreferredFamily);
				}
			}
		}

		/// <summary>
		/// Gets the preferred font style name for the current locale, using
		/// <c>CultureInfo.CurrentCulture</c>.
		/// </summary>
		/// <value>The font style name or <c>null</c>.</value>
		public string							LocalePreferredStyleName
		{
			get
			{
				lock (this.exclusion)
				{
					return this.GetName (NameId.PreferredSubfamily);
				}
			}
		}


		/// <summary>
		/// Gets the invariant font face name. This name is independent
		/// of the current culture.
		/// </summary>
		/// <value>The font face name.</value>
		public string							InvariantFaceName
		{
			get
			{
				string face  = this.InvariantPreferredFaceName  ?? this.MacintoshFaceName  ?? this.InvariantSimpleFaceName;
				string style = this.InvariantPreferredStyleName ?? this.MacintoshStyleName ?? this.InvariantSimpleStyleName;

				face = FontIdentity.RepairBrokenFaceName (face);

				if (face != null)
				{
					if (face.EndsWith (style))
					{
						face = face.Substring (0, face.Length - style.Length).Trim ();
					}
				}

				return face;
			}
		}

		private static string RepairBrokenFaceName(string face)
		{
			if (face != null)
			{
				if ((face.StartsWith ("Futura ")) &&
					(face.EndsWith (" BT")))
				{
					face = "Futura";
				}
			}

			return face;
		}

		/// <summary>
		/// Gets the invariant font style name. This name is independent
		/// of the current culture.
		/// </summary>
		/// <value>The font style name.</value>
		public string							InvariantStyleName
		{
			get
			{
				string preferred = this.InvariantPreferredStyleName;
				string simple    = this.InvariantSimpleStyleName;
				string adobe     = this.InvariantAdobeStyleName;

				string name = FontIdentity.ComposeStyleName (preferred, simple, adobe);
				string full = this.FullName;

				if ((name != null) &&
					(full != null))
				{
					name = FontIdentity.RepairBrokenStyleName (this.FullName, name);

					//	Special post-processing here for the poorly named Futura font
					//	collection, where the "Condensed" is neither present in the style
					//	name, nor in the face name, which leads to possible confusions if
					//	we do not add it manually :
					
					string[] attributes = new string[] { "Condensed" };

					foreach (string attribute in attributes)
					{
						if ((full.Contains (attribute)) &&
							(!name.Contains (attribute)))
						{
							name = string.Concat (attribute, " ", name);
						}
					}
				}

				return name;
			}
		}

		private static string RepairBrokenStyleName(string fullName, string style)
		{
			if (fullName.Contains (" LtCn BT"))
			{
				if (!style.Contains ("Condensed Light"))
				{
					style = string.Concat ("Condensed Light ", style);
				}
			}
			else if (fullName.Contains (" MdCn BT"))
			{
				if (!style.Contains ("Condensed Medium"))
				{
					style = string.Concat ("Condensed Medium ", style);
				}
			}

			return style;
		}


		public string InvariantFullName
		{
			get
			{
				lock (this.exclusion)
				{
					return this.GetName (NameId.FullFontName, FontIdentity.InvariantLocale);
				}
			}
		}


		/// <summary>
		/// Gets the invariant font face name. This name is independent
		/// of the current culture.
		/// </summary>
		/// <value>The font face name.</value>
		public string							InvariantSimpleFaceName
		{
			get
			{
				lock (this.exclusion)
				{
					string face  = this.GetName (NameId.FontFamily, FontIdentity.InvariantLocale);
					string style = this.GetName (NameId.FontSubfamily, FontIdentity.InvariantLocale);

					if (face.EndsWith (style))
					{
						face = face.Substring (0, face.Length - style.Length).Trim ();
					}

					return face;
				}
			}
		}

		/// <summary>
		/// Gets the invariant font style name. This name is independent
		/// of the current culture.
		/// </summary>
		/// <value>The font style name.</value>
		public string							InvariantSimpleStyleName
		{
			get
			{
				lock (this.exclusion)
				{
					return this.GetName (NameId.FontSubfamily, FontIdentity.InvariantLocale);
				}
			}
		}

		/// <summary>
		/// Gets the invariant Adobe font style name. This name is independent
		/// of the current culture.
		/// </summary>
		/// <value>The Adobe font style name.</value>
		public string InvariantAdobeStyleName
		{
			get
			{
				lock (this.exclusion)
				{
					return this.GetName (NameId.AdobeFontStyle, FontIdentity.InvariantLocale);
				}
			}
		}

		/// <summary>
		/// Gets the Macintosh font face name. This name is independent
		/// of the current culture.
		/// </summary>
		/// <value>The font face name.</value>
		public string							MacintoshFaceName
		{
			get
			{
				lock (this.exclusion)
				{
					string face  = this.GetMacName (NameId.FontFamily);
					string style = this.GetMacName (NameId.FontSubfamily);

					if ((face != null) &&
						(face.EndsWith (style)))
					{
						face = face.Substring (0, face.Length - style.Length).Trim ();
					}

					return face;
				}
			}
		}

		/// <summary>
		/// Gets the Macintosh font style name. This name is independent
		/// of the current culture.
		/// </summary>
		/// <value>The font style name.</value>
		public string							MacintoshStyleName
		{
			get
			{
				lock (this.exclusion)
				{
					return this.GetMacName (NameId.FontSubfamily);
				}
			}
		}


		/// <summary>
		/// Gets the invariant preferred font face name. This name is independent
		/// of the current culture.
		/// </summary>
		/// <value>The preferred font face name or <c>null</c> if there is no
		/// preferred font face associated with this font.</value>
		public string							InvariantPreferredFaceName
		{
			get
			{
				lock (this.exclusion)
				{
					return this.GetName (NameId.PreferredFamily, FontIdentity.InvariantLocale);
				}
			}
		}

		/// <summary>
		/// Gets the invariant preferred font style name. This name is independent
		/// of the current culture.
		/// </summary>
		/// <value>The preferred font style name or <c>null</c> if there is no
		/// preferred font style associated with this font.</value>
		public string							InvariantPreferredStyleName
		{
			get
			{
				lock (this.exclusion)
				{
					return this.GetName (NameId.PreferredSubfamily, FontIdentity.InvariantLocale);
				}
			}
		}

		/// <summary>
		/// Gets a simplified version of the invariant font style name. The
		/// names <c>"Regular"</c> and <c>"Normal"</c> are mapped to <c>""</c>.
		/// </summary>
		/// <value>The simplified font style name.</value>
		public string							InvariantStyleHash
		{
			get
			{
				if (this.styleHash == null)
				{
					lock (this.exclusion)
					{
						if (this.styleHash == null)
						{
							this.styleHash = FontCollection.GetStyleHash (this.InvariantStyleName);
						}
					}
				}

				return this.styleHash;
			}
		}

		/// <summary>
		/// Gets a simplified version of the full font name. The names <c>"Regular"</c>
		/// and <c>"Normal"</c> are mapped to <c>""</c> and all elements are sorted
		/// alphabetically.
		/// </summary>
		/// <value>The simplified version of the full font name.</value>
		public string							FullHash
		{
			get
			{
				if (this.fullHash == null)
				{
					lock (this.exclusion)
					{
						if (this.fullHash == null)
						{
							this.fullHash = FontName.GetFullHash (this.FullName);
						}
					}
				}
				
				return this.fullHash;
			}
		}
		
		/// <summary>
		/// Gets the number of font styles available for the font face
		/// described by this <c>FontIdentity</c> object.
		/// </summary>
		/// <value>The number of font styles.</value>
		public int								FontStyleCount
		{
			get
			{
				return this.fontStyleCount;
			}
		}
		
		/// <summary>
		/// Gets the full name of the font. This is the OpenType
		/// </summary>
		public string							FullName
		{
			get
			{
				lock (this.exclusion)
				{
				return this.otName.GetFullFontName ();
				}
			}
		}
		
		public string							UniqueFontId
		{
			get
			{
				return this.otName.GetUniqueFontIdentifier ();
			}
		}
		
		public byte[]							AssociatedBlob1
		{
			get
			{
				if (this.blob1 == null)
				{
					return new byte[0];
				}
				else
				{
					return this.blob1;
				}
			}
			set
			{
				this.blob1 = value;
			}
		}
		
		public byte[]							AssociatedBlob2
		{
			get
			{
				if (this.blob2 == null)
				{
					return new byte[0];
				}
				else
				{
					return this.blob2;
				}
			}
			set
			{
				this.blob2 = value;
			}
		}

		public object							DrawingFont
		{
			get
			{
				return this.drawingFont;
			}
			set
			{
				this.drawingFont = value;
			}
		}
		
		public FontWeight						FontWeight
		{
			get
			{
				return (FontWeight) Platform.Neutral.GetFontWeight (this.Record);
			}
		}
		
		public FontStyle						FontStyle
		{
			get
			{
				if (Platform.Neutral.GetFontItalic (this.Record) == 0)
				{
					return FontStyle.Normal;
				}
				
				string name = this.InvariantStyleName.ToLower (System.Globalization.CultureInfo.InvariantCulture);
				
				if ((name.IndexOf ("italic") != -1) ||
					(name.IndexOf ("cursive") != -1) ||
					(name.IndexOf ("kursiv") != -1))
				{
					return FontStyle.Italic;
				}
				else
				{
					return FontStyle.Oblique;
				}
			}
		}
		
		
		public bool								IsSymbolFont
		{
			get
			{
				if (this.isSymbolFontDefined == false)
				{
					lock (this.exclusion)
					{
						if (this.isSymbolFontDefined == false)
						{
							Table_cmap cmap = this.InternalGetTable_cmap ();

							this.isSymbolFont        = cmap.FindFormatSubTable (3, 0, 4) != null;
							this.isSymbolFontDefined = true;
						}
					}
				}
				
				return this.isSymbolFont;
			}
		}

		public bool								IsDynamicFont
		{
			get
			{
				return this.isDynamicFont;
			}
		}

		
		public FontData							FontData
		{
			get
			{
				if (this.fontData == null)
				{
					lock (this.exclusion)
					{
						if (this.fontData == null)
						{
							byte[] data = Platform.Neutral.LoadFontData (this.Record);

							this.fontData = data == null ? null : new FontData (data, this.ttcIndex);
						}
					}
				}

				return this.fontData;
			}
		}

		internal string							InternalFontName
		{
			get
			{
				return FontCollection.GetInternalFontName (this.systemFontFamily, this.systemFontStyle);
			}
		}

		public Table_name						OpenTypeTable_name
		{
			get
			{
				return this.otName;
			}
		}

		
		private object							Record
		{
			get
			{
				if (this.record == null)
				{
					lock (this.exclusion)
					{
						if (this.record == null)
						{
							this.record = Platform.Neutral.GetFontSystemDescription (this.systemFontFamily, this.systemFontStyle);
						}
						if (this.record == null)
						{
							//	For some fonts, such as the Meiryo font collection, the system font name
							//	is returned in the local language; just to prevent not finding the font
							//	based on the system font family & system font style, we try also a look-
							//	up with the localized names :
							
							this.record = Platform.Neutral.GetFontSystemDescription (this.LocaleFaceName, this.LocaleStyleName);
						}
					}
				}

				return this.record;
			}
		}

		
		public static IComparer<FontIdentity>	Comparer
		{
			get
			{
				return new FontComparer ();
			}
		}
		
		public static event FontIdentityCallback Serializing;


		/// <summary>
		/// Gets the name of the specified glyph.
		/// </summary>
		/// <param name="glyph">The glyph index.</param>
		/// <returns>The name of the glyph or <c>null</c> if the glyph is not
		/// supported in the font.</returns>
		public string GetGlyphName(int glyph)
		{
			TableEntry entry = this.FontData["post"];
			Table_post post  = entry == null ? null : new Table_post (entry);

			if (post != null)
			{
				return post.GetGlyphName (glyph);
			}
			else
			{
				return null;
			}
		}

		
		public Table_cmap InternalGetTable_cmap()
		{
			return new Table_cmap (this.FontData["cmap"]);
		}

		public void InternalClearFontData()
		{
			this.fontData = null;
		}

		public Platform.IFontHandle GetPlatformFontHandle(int size)
		{
			FontSizeInfo info = this.GetSizeInfo (size);

			return info == null ? null : info.Handle;
		}
		
		internal static void Serialize(System.IO.Stream stream, FontIdentity fid)
		{
			if (FontIdentity.Serializing != null)
			{
				FontIdentity.Serializing (fid);
			}

			System.Text.StringBuilder buffer = new System.Text.StringBuilder ();

			buffer.Append (fid.systemFontFamily);
			buffer.Append ('\0');
			buffer.Append (fid.systemFontStyle);
			buffer.Append ('\0');
			buffer.AppendFormat (System.Globalization.CultureInfo.InvariantCulture, "{0}", fid.ttcIndex);
			buffer.Append ('\0');
			buffer.Append (fid.IsSymbolFont ? "S" : "s");

			byte[] data_0 = new byte[10];
			byte[] data_1 = System.Text.Encoding.UTF8.GetBytes (buffer.ToString ());
			byte[] data_2 = new byte[fid.otNameLength];
			byte[] data_3 = fid.AssociatedBlob1;
			byte[] data_4 = fid.AssociatedBlob2;

			System.Buffer.BlockCopy (fid.otName.BaseData, fid.otName.BaseOffset, data_2, 0, fid.otNameLength);

			int length_1 = data_1.Length;
			int length_2 = data_2.Length;
			int length_3 = data_3.Length;
			int length_4 = data_4.Length;

			if (length_3 > 0xfff0)
			{
				length_3 = 0;
			}

			if (length_4 > 0xfff0)
			{
				length_4 = 0;
			}

			data_0[0] = 0;
			data_0[1] = 0;
			data_0[2] = (byte) (length_1 >> 8);
			data_0[3] = (byte) (length_1 & 0xff);
			data_0[4] = (byte) (length_2 >> 8);
			data_0[5] = (byte) (length_2 & 0xff);
			data_0[6] = (byte) (length_3 >> 8);
			data_0[7] = (byte) (length_3 & 0xff);
			data_0[8] = (byte) (length_4 >> 8);
			data_0[9] = (byte) (length_4 & 0xff);

			stream.Write (data_0, 0, data_0.Length);
			stream.Write (data_1, 0, length_1);
			stream.Write (data_2, 0, length_2);
			stream.Write (data_3, 0, length_3);
			stream.Write (data_4, 0, length_4);
		}

		internal static FontIdentity Deserialize(System.IO.Stream stream)
		{
			byte[] data_0 = new byte[10];

			int read = IO.Reader.Read (stream, data_0, 0, 10);

			if (read == 0)
			{
				return null;
			}

			System.Diagnostics.Debug.Assert (data_0[0] == 0);
			System.Diagnostics.Debug.Assert (data_0[1] == 0);

			int length_1 = (data_0[2] << 8) | data_0[3];
			int length_2 = (data_0[4] << 8) | data_0[5];
			int length_3 = (data_0[6] << 8) | data_0[7];
			int length_4 = (data_0[8] << 8) | data_0[9];

			byte[] data_1 = new byte[length_1];
			byte[] data_2 = new byte[length_2];
			byte[] data_3 = new byte[length_3];
			byte[] data_4 = new byte[length_4];

			IO.Reader.Read (stream, data_1, 0, length_1);
			IO.Reader.Read (stream, data_2, 0, length_2);
			IO.Reader.Read (stream, data_3, 0, length_3);
			IO.Reader.Read (stream, data_4, 0, length_4);

			string text = System.Text.Encoding.UTF8.GetString (data_1);
			string[] args = text.Split ('\0');

			System.Diagnostics.Debug.Assert (args.Length == 4);

			string systemFontFamily = args[0];
			string systemFontStyle  = args[1];

			int    ttcIndex = System.Int32.Parse (args[2], System.Globalization.CultureInfo.InvariantCulture);
			string flags    = args[3];

			FontIdentity fid = new FontIdentity ();

			fid.otName           = new Table_name (data_2, 0);
			fid.otNameLength     = length_2;
			fid.systemFontFamily = systemFontFamily;
			fid.systemFontStyle  = systemFontStyle;
			fid.blob1            = data_3;
			fid.blob2            = data_4;

			fid.isSymbolFont        = flags.IndexOf ("S") != -1;
			fid.isSymbolFontDefined = flags.IndexOfAny (new char[] { 's', 'S' }) != -1;

			return fid;
		}

		internal static FontIdentity CreateDynamicFont(byte[] data)
		{
			FontData fontData = new FontData (data, 0);
			FontIdentity id = new FontIdentity (fontData, null, 0);
			id.isDynamicFont = true;
			return id;
		}
		

		internal void DefineTableName(Table_name openTypeNameTable, int length)
		{
			this.otName        = openTypeNameTable;
			this.otNameLength = length;
		}

		internal void DefineSystemFontFamilyAndStyle(string family, string style)
		{
			this.systemFontFamily = family;
			this.systemFontStyle  = style;
		}

		internal void DefineFontStyleCount(int value)
		{
			this.fontStyleCount = value;
		}

		internal FontSizeInfo GetSizeInfo(int size)
		{
			lock (this)
			{
				if (this.fontSizes == null)
				{
					this.fontSizes = new Dictionary<int, FontSizeInfo> ();
				}
				
				if (this.fontSizes.ContainsKey (size) == false)
				{
					this.fontSizes[size] = new FontSizeInfo (size, Platform.Neutral.GetFontHandle (this.Record, size));
				}
			}
			
			return this.fontSizes[size];
		}


		internal bool EqualsFullHash(string fullHash)
		{
			if (this.FullHash == fullHash)
			{
				return true;
			}
			if (FontName.GetFullHash (FontName.GetFullName (this.InvariantSimpleFaceName, this.InvariantSimpleStyleName)) == fullHash)
			{
				return true;
			}
			if (FontName.GetFullHash (FontName.GetFullName (this.MacintoshFaceName, this.MacintoshStyleName)) == fullHash)
			{
				return true;
			}
			if (FontName.GetFullHash (FontName.GetFullName (this.InvariantFaceName, this.InvariantStyleName)) == fullHash)
			{
				return true;
			}

			return false;
		}
		
		
		#region FontComparer Class
		
		private class FontComparer : IComparer<FontIdentity>
		{
			#region IComparer Members
			public int Compare(FontIdentity x, FontIdentity y)
			{
				int comp = string.Compare (x.InvariantFaceName, y.InvariantFaceName);
				
				if (comp == 0)
				{
					comp = string.Compare (x.InvariantStyleName, y.InvariantStyleName);
				}
				
				return comp;
			}
			#endregion
		}
		
		#endregion

		private static string ComposeStyleName(string preferred, string simple, string adobe)
		{
			//	Examples of preferred subfamily names and subfamily names, with the
			//	expected resulting composed style name :
			//
			//	"Light Italic Display" / "Italic" --> "Light Italic Display"
			//	"Regular" / "Regular" --------------> "Regular"
			//	"Narrow" / "Bold" ------------------> "Narrow Bold"
			
			if (preferred == null)
			{
				return simple;
			}
			if (preferred == simple)
			{
				return preferred;
			}
			if (string.IsNullOrEmpty (adobe))
			{
				if (preferred.Contains (simple))
				{
					return preferred;
				}
				else
				{
					return string.Concat (preferred, " ", simple);
				}
			}
			else if (preferred.Contains (adobe))
			{
				if (preferred == adobe)
				{
					return preferred;
				}
				if (preferred.StartsWith (adobe))
				{
					string suffix = preferred.Substring (adobe.Length).Trim ();
					return string.Concat (adobe, " (", suffix, ")");
				}
				else
				{
					return preferred;
				}
			}
			else
			{
				return string.Concat (adobe, " (", preferred, ")");
			}
		}

		private string GetName(NameId id)
		{
			System.Globalization.CultureInfo info = System.Globalization.CultureInfo.CurrentCulture;
			
			int locale_id = (info.LCID & 0x03ff) + 0x0400;
			
			string name = this.GetName (id, locale_id);

			return (name == null) ? this.GetName (id, FontIdentity.InvariantLocale) : name;
		}
		
		private string GetName(NameId id, int localeId)
		{
			string name;
			
			name = this.otName.GetUnicodeName (localeId, id, PlatformId.Microsoft);
			
			if ((name == null) &&
				(localeId == FontIdentity.InvariantLocale))
			{
				name = this.GetMacName (id);
			}
			
			return name;
		}

		private string GetMacName(NameId id)
		{
			return this.otName.GetLatinName (0, id, PlatformId.Macintosh);
		}


		private const int InvariantLocale = 1033;
						
		private object							exclusion = new object ();
		
		private Table_name						otName;
		private int								otNameLength;
		
		private object							record;
		private FontData						fontData;
		private Dictionary<int, FontSizeInfo>	fontSizes;
		private string							styleHash;
		private string							fullHash;
		private int								ttcIndex;
		private int								fontStyleCount;
		private string							systemFontFamily;
		private string							systemFontStyle;
		private bool							isSymbolFont;
		private bool							isSymbolFontDefined;
		private bool							isDynamicFont;
		private byte[]							blob1;
		private byte[]							blob2;
		private object							drawingFont;
	}
}
