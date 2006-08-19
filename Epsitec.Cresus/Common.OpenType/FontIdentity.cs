//	Copyright © 2005-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
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
		
		
		public string							LocaleFaceName
		{
			get
			{
				lock (this.exclusion)
				{
					string face  = this.GetName (NameId.PreferredFamily);
					string style = this.LocaleStyleName;

					if (face == null)
					{
						face = this.GetName (NameId.FontFamily);
					}

					if (face.EndsWith (style))
					{
						face = face.Substring (0, face.Length - style.Length).Trim ();
					}

					return face;
				}
			}
		}
		
		public string							LocaleStyleName
		{
			get
			{
				lock (this.exclusion)
				{
					string name = this.GetName (NameId.PreferredSubfamily);

					if (name == null)
					{
						name = this.GetName (NameId.FontSubfamily);
					}

					return name;
				}
			}
		}
		
		
		public string							InvariantFaceName
		{
			get
			{
				lock (this.exclusion)
				{
					string face  = this.GetName (NameId.PreferredFamily, FontIdentity.InvariantLocale);
					string style = this.InvariantStyleName;

					if (face == null)
					{
						face = this.GetName (NameId.FontFamily, FontIdentity.InvariantLocale);
					}

					if (face.EndsWith (style))
					{
						face = face.Substring (0, face.Length - style.Length).Trim ();
					}

					return face;
				}
			}
		}
		
		public string							InvariantStyleName
		{
			get
			{
				lock (this.exclusion)
				{
					string name = this.GetName (NameId.PreferredSubfamily, FontIdentity.InvariantLocale);

					if (name == null)
					{
						name = this.GetName (NameId.FontSubfamily, FontIdentity.InvariantLocale);
					}

					return name;
				}
			}
		}
		
		public string							InvariantStyleHash
		{
			get
			{
				lock (this.exclusion)
				{
					if (this.styleHash == null)
					{
						this.styleHash = FontCollection.GetStyleHash (this.InvariantStyleName);
					}

					return this.styleHash;
				}
			}
		}
		
		
		public int								FontStyleCount
		{
			get
			{
				return this.fontStyleCount;
			}
		}
		
		public string							FullName
		{
			get
			{
				return this.otName.GetFullFontName ();
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

							this.isSymbolFont         = cmap.FindFormatSubTable (3, 0, 4) != null;
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

		
		internal FontData						FontData
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
			buffer.Append (fid.isSymbolFont ? "S" : "s");

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

			fid.isSymbolFont         = flags.IndexOf ("S") != -1;
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
			
			if (name == null)
			{
				name = this.otName.GetLatinName (0, id, PlatformId.Macintosh);
			}
			
			return name;
		}


		private const int InvariantLocale = 1033;
		
		private object							exclusion = new object ();
		
		private Table_name						otName;
		private int								otNameLength;
		
		private object							record;
		private FontData						fontData;
		private Dictionary<int, FontSizeInfo>	fontSizes;
		private string							styleHash;
		private int								ttcIndex;
		private int								fontStyleCount;
		private string							systemFontFamily;
		private string							systemFontStyle;
		private bool							isSymbolFont;
		private bool							isSymbolFontDefined;
		private bool							isDynamicFont;
		private byte[]							blob1;
		private byte[]							blob2;
	}
}
