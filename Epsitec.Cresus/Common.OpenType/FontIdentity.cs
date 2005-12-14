//	Copyright © 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.OpenType
{
	/// <summary>
	/// La classe FontIdentity donne accès aux noms détaillés de la fonte et
	/// cache l'accès aux données complètes (chargées qu'en cas de besoin).
	/// </summary>
	public sealed class FontIdentity
	{
		private FontIdentity()
		{
		}
		
		public FontIdentity(Table_name open_type_name_table, int length, object system_record) : this (open_type_name_table, length, system_record, -1)
		{
		}
		
		public FontIdentity(Table_name open_type_name_table, int length, object system_record, int ttc_index)
		{
			this.DefineTableName (open_type_name_table, length);
			
			this.record    = system_record;
			this.ttc_index = ttc_index;
		}
		
		
		internal FontIdentity(object system_record, int ttc_index)
		{
			this.record    = system_record;
			this.ttc_index = ttc_index;
		}
		
		internal FontIdentity(FontData font_data, object system_record, int ttc_index) : this (system_record, ttc_index)
		{
			this.font_data = font_data;
		}
		
		
		public string							LocaleFaceName
		{
			get
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
		
		public string							LocaleStyleName
		{
			get
			{
				string name = this.GetName (NameId.PreferredSubfamily);
				
				if (name == null)
				{
					name = this.GetName (NameId.FontSubfamily);
				}
				
				return name;
			}
		}
		
		
		public string							InvariantFaceName
		{
			get
			{
				string face  = this.GetName (NameId.PreferredFamily, 1033);
				string style = this.InvariantStyleName;
				
				if (face == null)
				{
					face = this.GetName (NameId.FontFamily, 1033);
				}
				
				if (face.EndsWith (style))
				{
					face = face.Substring (0, face.Length - style.Length).Trim ();
				}
				
				return face;
			}
		}
		
		public string							InvariantStyleName
		{
			get
			{
				string name = this.GetName (NameId.PreferredSubfamily, 1033);
				
				if (name == null)
				{
					name = this.GetName (NameId.FontSubfamily, 1033);
				}
				
				return name;
			}
		}
		
		public string							InvariantStyleHash
		{
			get
			{
				if (this.style_hash == null)
				{
					this.style_hash = FontCollection.GetStyleHash (this.InvariantStyleName);
				}
				
				return this.style_hash;
			}
		}
		
		
		public string							FullName
		{
			get
			{
				return this.ot_name.GetFullFontName ();
			}
		}
		
		public string							UniqueFontId
		{
			get
			{
				return this.ot_name.GetUniqueFontIdentifier ();
			}
		}
		
		
		internal FontData						FontData
		{
			get
			{
				lock (this)
				{
					if (this.font_data == null)
					{
						byte[] data = Platform.Neutral.LoadFontData (this.record);
						
						this.font_data = data == null ? null : new FontData (data, this.ttc_index);
					}
				}
				
				return this.font_data;
			}
		}
		
		public FontWeight						FontWeight
		{
			get
			{
				return (FontWeight) Platform.Neutral.GetFontWeight (this.record);
			}
		}
		
		public FontStyle						FontStyle
		{
			get
			{
				if (Platform.Neutral.GetFontItalic (this.record) == 0)
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
				if (this.is_symbol_font_defined == false)
				{
					Table_cmap cmap = this.GetTable_cmap ();
					
					this.is_symbol_font         = cmap.FindFormatSubTable (3, 0, 4) != null;
					this.is_symbol_font_defined = true;
				}
				
				return this.is_symbol_font;
			}
		}
		
		
		public Table_cmap GetTable_cmap()
		{
			return new Table_cmap (this.FontData["cmap"]);
		}
		
		
		internal void DefineTableName(Table_name open_type_name_table, int length)
		{
			this.ot_name        = open_type_name_table;
			this.ot_name_length = length;
		}
		
		internal void DefineSystemFontFamilyAndStyle(string family, string style)
		{
			this.os_font_family = family;
			this.os_font_style  = style;
		}
		
		
		public static System.Collections.IComparer Comparer
		{
			get
			{
				return new FontComparer ();
			}
		}
		
		
		public Platform.IFontHandle GetFontHandle(int size)
		{
			SizeInfo info = this.GetSizeInfo (size);
			
			return info == null ? null : info.Handle;
		}
		
		public SizeInfo GetSizeInfo(int size)
		{
			lock (this)
			{
				if (this.font_sizes == null)
				{
					this.font_sizes = new System.Collections.Hashtable ();
				}
				
				if (this.font_sizes.Contains (size) == false)
				{
					this.font_sizes[size] = new SizeInfo (size, Platform.Neutral.GetFontHandle (this.record, size));
				}
			}
			
			return this.font_sizes[size] as SizeInfo;
		}
		
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
		
		
		public static void Serialize(System.IO.Stream stream, FontIdentity fid)
		{
			System.Text.StringBuilder buffer = new System.Text.StringBuilder ();
			
			buffer.Append (fid.os_font_family);
			buffer.Append ('\0');
			buffer.Append (fid.os_font_style);
			buffer.Append ('\0');
			buffer.AppendFormat (System.Globalization.CultureInfo.InvariantCulture, "{0}", fid.ttc_index);
			buffer.Append ('\0');
			buffer.Append (fid.is_symbol_font ? "S" : "s");
			
			byte[] data_0 = new byte[6];
			byte[] data_1 = System.Text.Encoding.UTF8.GetBytes (buffer.ToString ());
			byte[] data_2 = new byte[fid.ot_name_length];
			
			System.Buffer.BlockCopy (fid.ot_name.BaseData, fid.ot_name.BaseOffset, data_2, 0, fid.ot_name_length);
			
			int length_1 = data_1.Length;
			int length_2 = data_2.Length;
			
			data_0[0] = 0;
			data_0[1] = 0;
			data_0[2] = (byte)(length_1 >> 8);
			data_0[3] = (byte)(length_1 & 0xff);
			data_0[4] = (byte)(length_2 >> 8);
			data_0[5] = (byte)(length_2 & 0xff);
			
			stream.Write (data_0, 0, data_0.Length);
			stream.Write (data_1, 0, length_1);
			stream.Write (data_2, 0, length_2);
		}
		
		public static FontIdentity Deserialize(System.IO.Stream stream)
		{
			byte[] data_0 = new byte[6];
			
			stream.Read (data_0, 0, 6);
			
			System.Diagnostics.Debug.Assert (data_0[0] == 0);
			System.Diagnostics.Debug.Assert (data_0[1] == 0);
			
			int length_1 = (data_0[2] << 8) | data_0[3];
			int length_2 = (data_0[4] << 8) | data_0[5];
			
			byte[] data_1 = new byte[length_1];
			byte[] data_2 = new byte[length_2];
			
			stream.Read (data_1, 0, length_1);
			stream.Read (data_2, 0, length_2);
			
			string   text = System.Text.Encoding.UTF8.GetString (data_1);
			string[] args = text.Split ('\0');
			
			System.Diagnostics.Debug.Assert (args.Length == 4);
			
			string os_font_family = args[0];
			string os_font_style  = args[1];
			int    ttc_index      = System.Int32.Parse (args[2], System.Globalization.CultureInfo.InvariantCulture);
			string flags          = args[3];

			FontIdentity fid = new FontIdentity ();
			
			fid.ot_name        = new Table_name (data_2, 0);
			fid.ot_name_length = length_2;
			fid.os_font_family = os_font_family;
			fid.os_font_style  = os_font_style;
			
			fid.is_symbol_font         = flags.IndexOf ("S") != -1;
			fid.is_symbol_font_defined = flags.IndexOfAny (new char[] { 's', 'S' }) != -1;
			
			return fid;
		}
		
		
		#region FontComparer Class
		private class FontComparer : System.Collections.IComparer
		{
			#region IComparer Members
			public int Compare(object x, object y)
			{
				FontIdentity fx = x as FontIdentity;
				FontIdentity fy = y as FontIdentity;
				
				int comp = string.Compare (fx.InvariantFaceName, fy.InvariantFaceName);
				
				if (comp == 0)
				{
					comp = string.Compare (fx.InvariantStyleName, fy.InvariantStyleName);
				}
				
				return comp;
			}
			#endregion
		}
		#endregion
		
		#region SizeInfo Class
		public class SizeInfo
		{
			public SizeInfo(int size, Platform.IFontHandle handle)
			{
				this.point_size   = size;
				this.font_handle  = handle;
				this.glyph_widths = new int[4][];
				
				Platform.Neutral.FillFontHeights (handle, out this.height, out this.ascender, out this.descender, out this.int_leading, out this.ext_leading);
			}
			
			
			public Platform.IFontHandle			Handle
			{
				get
				{
					return this.font_handle;
				}
			}
			
			public int							PointSize
			{
				get
				{
					return this.point_size;
				}
			}
			
			public int							Ascender
			{
				get
				{
					return this.ascender;
				}
			}
			
			public int							Descender
			{
				get
				{
					return this.descender;
				}
			}
			
			public int							Height
			{
				get
				{
					return this.height;
				}
			}
			
			
			public int GetGlyphWidth(int glyph)
			{
				if (glyph >= 0xffff)
				{
					return 0;
				}
				
				int block = glyph / 64;
				int index = glyph % 64;
				
				if (block >= this.glyph_widths.Length)
				{
					int[][] old_widths = this.glyph_widths;
					int[][] new_widths = new int[block+1][];
					
					for (int i = 0; i < old_widths.Length; i++)
					{
						new_widths[i] = old_widths[i];
					}
					
					this.glyph_widths = new_widths;
				}
				
				if (this.glyph_widths[block] == null)
				{
					this.glyph_widths[block] = new int[64];
					
					Platform.Neutral.FillFontWidths (this.font_handle, block*64, 64, this.glyph_widths[block], null, null);
				}
				
				return this.glyph_widths[block][index];
			}
			
			
			int									point_size;
			Platform.IFontHandle				font_handle;
			int[][]								glyph_widths;
			int									height;
			int									ascender;
			int									descender;
			int									int_leading;
			int									ext_leading;
		}
		#endregion
		
		private string GetName(NameId id)
		{
			System.Globalization.CultureInfo info = System.Globalization.CultureInfo.CurrentCulture;
			
			int locale_id = (info.LCID & 0x03ff) + 1024;
			
			string name = this.GetName (id, locale_id);
			
			return (name == null) ? this.GetName (id, 1033) : name;
		}
		
		private string GetName(NameId id, int locale_id)
		{
			string name;
			
			name = this.ot_name.GetUnicodeName (locale_id, id, PlatformId.Microsoft);
			
			if (name == null)
			{
				name = this.ot_name.GetLatinName (0, id, PlatformId.Macintosh);
			}
			
			return name;
		}
		
		
		private Table_name						ot_name;
		private int								ot_name_length;
		
		private object							record;
		private FontData						font_data;
		private System.Collections.Hashtable	font_sizes;
		private string							style_hash;
		private int								ttc_index;
		private string							os_font_family;
		private string							os_font_style;
		private bool							is_symbol_font;
		private bool							is_symbol_font_defined;
	}
}
