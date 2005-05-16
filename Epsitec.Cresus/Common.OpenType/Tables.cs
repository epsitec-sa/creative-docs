//	Copyright © 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.OpenType
{
	/// <summary>
	/// Summary description for Tables.
	/// </summary>
	public class Tables
	{
		public Tables(byte[] data, int offset)
		{
			this.data   = data;
			this.offset = offset;
		}
		
		
		public byte[]							BaseData
		{
			get
			{
				return this.data;
			}
		}
		
		public int								BaseOffset
		{
			get
			{
				return this.offset;
			}
		}
		
		
		protected int ReadInt8(int offset)
		{
			return (int) this.data[this.offset + offset];
		}
		
		protected int ReadInt16(int offset)
		{
			return (int) Support.ReadInt16 (this.data, this.offset + offset);
		}
		
		protected int ReadInt32(int offset)
		{
			return (int) Support.ReadInt32 (this.data, this.offset + offset);
		}
		
		
		protected byte[]						data;
		protected int							offset;
	}
	
	
	public class TableEntry : Tables
	{
		public TableEntry(byte[] data, int offset) : base (data, offset)
		{
		}
		
		
		public string	Tag
		{
			get
			{
				char[] tag = new char[4];
				
				tag[0] = (char) this.ReadInt8 (0);
				tag[1] = (char) this.ReadInt8 (1);
				tag[2] = (char) this.ReadInt8 (2);
				tag[3] = (char) this.ReadInt8 (3);
				
				return new string (tag);
			}
		}
		
		public int		Checksum
		{
			get
			{
				return this.ReadInt32 (4);
			}
		}
		
		public int		Offset
		{
			get
			{
				return this.ReadInt32 (8);
			}
		}
		
		public int		Length
		{
			get
			{
				return this.ReadInt32 (12);
			}
		}
	}
	
	public class TableDirectory : Tables
	{
		public TableDirectory(byte[] data, int offset) : base (data, offset)
		{
		}
		
		
		public int		Version
		{
			get
			{
				return this.ReadInt32 (0);
			}
		}
		
		public int		NumTables
		{
			get
			{
				return this.ReadInt16 (4);
			}
		}
		
		public int		SearchRange
		{
			get
			{
				return this.ReadInt16 (6);
			}
		}
		
		public int		EntrySelector
		{
			get
			{
				return this.ReadInt16 (8);
			}
		}
		
		public int		RangeShift
		{
			get
			{
				return this.ReadInt16 (10);
			}
		}
		
		
		public TableEntry GetEntry(int n)
		{
			return new TableEntry (this.data, this.offset + 16*n + 12);
		}
		public TableEntry FindTable(string name)
		{
			int n = (int) this.NumTables;
			
			for (int i = 0; i < n; i++)
			{
				TableEntry entry = this.GetEntry (i);
				
				if (entry.Tag == name)
				{
					return entry;
				}
			}
			
			return null;
		}
	}
	
	public class Table_head : Tables
	{
		public Table_head(byte[] data, int offset) : base (data, offset)
		{
		}
		
		public Table_head(TableEntry entry) : base (entry.BaseData, entry.Offset)
		{
		}
		
		
		public int		TableVersion
		{
			get
			{
				return this.ReadInt32 (0);
			}
		}
		
		public int		FontRevision
		{
			get
			{
				return this.ReadInt32 (4);
			}
		}
		
		public int		ChecksumAdjustment
		{
			get
			{
				return this.ReadInt32 (8);
			}
		}
		
		public int		MagicNumber
		{
			get
			{
				return this.ReadInt32 (12);
			}
		}
		
		public int		Flags
		{
			get
			{
				return this.ReadInt16 (16);
			}
		}
		
		public int		UnitsPerEm
		{
			get
			{
				return this.ReadInt16 (18);
			}
		}
		
		public ulong	Created
		{
			get
			{
				ulong h = (uint) this.ReadInt32 (20);
				ulong l = (uint) this.ReadInt32 (24);
				return (h << 32) | l;
			}
		}
		
		public ulong	Modified
		{
			get
			{
				ulong h = (uint) this.ReadInt32 (28);
				ulong l = (uint) this.ReadInt32 (32);
				return (h << 32) | l;
			}
		}
		
		public int		XMin
		{
			get
			{
				return this.ReadInt16 (36);
			}
		}
		
		public int		YMin
		{
			get
			{
				return this.ReadInt16 (38);
			}
		}
		
		public int		XMax
		{
			get
			{
				return this.ReadInt16 (40);
			}
		}
		
		public int		YMax
		{
			get
			{
				return this.ReadInt16 (42);
			}
		}
		
		public int		MacStyle
		{
			get
			{
				return this.ReadInt16 (44);
			}
		}
		
		public int		LowestRecPpEm
		{
			get
			{
				return this.ReadInt16 (46);
			}
		}
		
		public int		FontDirectionHint
		{
			get
			{
				return this.ReadInt16 (48);
			}
		}
		
		public int		IndexToLocFormat
		{
			get
			{
				return this.ReadInt16 (50);
			}
		}
		
		public int		GlyphDataFormat
		{
			get
			{
				return this.ReadInt16 (52);
			}
		}
	}
	
	public class Table_glyf : Tables
	{
		public Table_glyf(byte[] data, int offset) : base (data, offset)
		{
		}
		
		public Table_glyf(TableEntry entry) : base (entry.BaseData, entry.Offset)
		{
		}
		
		
		public int		NumContours
		{
			get
			{
				return this.ReadInt16 (0);
			}
		}
		
		public int		XMin
		{
			get
			{
				return this.ReadInt16 (2);
			}
		}
		
		public int		YMin
		{
			get
			{
				return this.ReadInt16 (4);
			}
		}
		
		public int		XMax
		{
			get
			{
				return this.ReadInt16 (6);
			}
		}
		
		public int		YMax
		{
			get
			{
				return this.ReadInt16 (8);
			}
		}
		
		
		public bool		IsSimpleGlyph
		{
			get
			{
				return this.NumContours > 0;
			}
		}
		
		public bool		IsCompositeGlyph
		{
			get
			{
				return this.NumContours == 0xffff;
			}
		}
		
		
		public int GetSimpleEndPtsOfContour(int n)
		{
			return this.ReadInt16 (10+n*2);
		}
		
		public int GetSimpleInstructionLength()
		{
			return this.ReadInt16 ((int)(10+2*this.NumContours+0));
		}
		
		public int GetSimpleInstruction(int n)
		{
			return this.ReadInt8 ((int)(10+2*this.NumContours+2+n));
		}
		
		public int GetSimpleFlag(int n)
		{
			return this.ReadInt8 ((int)(10+2*this.NumContours+2+this.GetSimpleInstructionLength ()+n));
		}
		
		
		public int		CompositeFlags
		{
			get
			{
				return this.ReadInt16 (10);
			}
		}
		
		public int		CompositeGlyphIndex
		{
			get
			{
				return this.ReadInt16 (12);
			}
		}
	}
	
	public class Table_loca_Short : Tables
	{
		public Table_loca_Short(byte[] data, int offset) : base (data, offset)
		{
		}
		
		
		public int GetOffset(int n)
		{
			return this.ReadInt16 (n*2);
		}
	}
	
	public class Table_loca_Long : Tables
	{
		public Table_loca_Long(byte[] data, int offset) : base (data, offset)
		{
		}
		
		
		public int GetOffset(int n)
		{
			return this.ReadInt32 (n*4);
		}
	}
	
	public class Table_maxp : Tables
	{
		public Table_maxp(byte[] data, int offset) : base (data, offset)
		{
		}
		
		public Table_maxp(TableEntry entry) : base (entry.BaseData, entry.Offset)
		{
		}
		
		
		public int		TableVersion
		{
			get
			{
				return this.ReadInt32 (0);
			}
		}
		
		public int		NumGlyphs
		{
			get
			{
				return this.ReadInt16 (4);
			}
		}
		
		public int		MaxPoints
		{
			get
			{
				return this.ReadInt16 (6);
			}
		}
		
		public int		MaxContours
		{
			get
			{
				return this.ReadInt16 (8);
			}
		}
		
		public int		MaxCompositePoints
		{
			get
			{
				return this.ReadInt16 (10);
			}
		}
		
		public int		MaxCompositeContours
		{
			get
			{
				return this.ReadInt16 (12);
			}
		}
		
		public int		MaxZones
		{
			get
			{
				return this.ReadInt16 (14);
			}
		}
		
		public int		MaxTwilightPoints
		{
			get
			{
				return this.ReadInt16 (16);
			}
		}
		
		public int		MaxStorage
		{
			get
			{
				return this.ReadInt16 (18);
			}
		}
		
		public int		MaxFunctionDefs
		{
			get
			{
				return this.ReadInt16 (20);
			}
		}
		
		public int		MaxInstructionDefs
		{
			get
			{
				return this.ReadInt16 (22);
			}
		}
		
		public int		MaxStackElements
		{
			get
			{
				return this.ReadInt16 (24);
			}
		}
		
		public int		MaxSizeOfInstructions
		{
			get
			{
				return this.ReadInt16 (26);
			}
		}
		
		public int		MaxComponentElements
		{
			get
			{
				return this.ReadInt16 (28);
			}
		}
		
		public int		MaxComponentDepth
		{
			get
			{
				return this.ReadInt16 (30);
			}
		}
	}
	
	public class Table_cmap : Tables
	{
		//	http://partners.adobe.com/public/developer/opentype/index_cmap.html
		
		public Table_cmap(byte[] data, int offset) : base (data, offset)
		{
		}
		
		public Table_cmap(TableEntry entry) : base (entry.BaseData, entry.Offset)
		{
		}
		
		
		public int		TableVersion
		{
			get
			{
				return this.ReadInt16 (0);
			}
		}
		
		public int		NumEncodingTables
		{
			get
			{
				return this.ReadInt16 (2);
			}
		}
		
		
		public int GetSubTablePlatformId(int n)
		{
			return this.ReadInt16 (4+n*8+0);
		}
		
		public int GetSubTableEncodingId(int n)
		{
			return this.ReadInt16 (4+n*8+2);
		}
		
		public int GetSubTableOffset(int n)
		{
			return this.ReadInt32 (4+n*8+4);
		}
		
		public IndexMappingTable GetGenericSubTable(int n)
		{
			return new IndexMappingTable (this.data, this.offset + (int) this.GetSubTableOffset (n));
		}
		
		
		public IndexMappingTable FindFormatSubTable(int platform, int encoding, int format)
		{
			int n = (int) this.NumEncodingTables;
			
			for (int i = 0; i < n; i++)
			{
				if ((this.GetSubTablePlatformId (i) == platform) &&
					(this.GetSubTableEncodingId (i) == encoding))
				{
					IndexMappingTable fmt = this.GetGenericSubTable (i);
					
					if (fmt.Format == format)
					{
						switch (fmt.Format)
						{
							case 0:  return new IndexMappingTable0 (this.data, this.offset + (int) this.GetSubTableOffset (i));
							case 4:  return new IndexMappingTable4 (this.data, this.offset + (int) this.GetSubTableOffset (i));
							case 12: return new IndexMappingTable12 (this.data, this.offset + (int) this.GetSubTableOffset (i));
						}
						
						return null;
					}
				}
			}
			
			return null;
		}
		
		public IndexMappingTable FindFormatSubTable()
		{
			IndexMappingTable sub;
			
			sub = this.FindFormatSubTable (3, 1, 12);
			
			if (sub == null)
			{
				sub = this.FindFormatSubTable (3, 1, 4);
			}
			
			return sub;
		}
	}
	
	public class IndexMappingTable : Tables
	{
		public IndexMappingTable(byte[] data, int offset) : base (data, offset)
		{
		}
		
		
		public int		Format
		{
			get
			{
				return this.ReadInt16 (0);
			}
		}
		
		public int		Length
		{
			get
			{
				return this.ReadInt16 (2);
			}
		}
		
		
		public virtual ushort GetGlyphIndex(int n)
		{
			throw new System.NotSupportedException ();
		}
	}
	
	public class IndexMappingTable0 : IndexMappingTable
	{
		public IndexMappingTable0(byte[] data, int offset) : base (data, offset)
		{
		}
		
		
		public int		Language
		{
			get
			{
				return this.ReadInt16 (4);
			}
		}
		
		
		public override ushort GetGlyphIndex(int n)
		{
			return (ushort) this.ReadInt8 (6+n);
		}
	}
	
	public class IndexMappingTable4 : IndexMappingTable
	{
		public IndexMappingTable4(byte[] data, int offset) : base (data, offset)
		{
		}
		
		
		public int		Language
		{
			get
			{
				return this.ReadInt16 (4);
			}
		}
		
		public int		SegCountX2
		{
			get
			{
				return this.ReadInt16 (6);
			}
		}
		
		public int		SearchRange
		{
			get
			{
				return this.ReadInt16 (8);
			}
		}
		
		public int		EntrySelector
		{
			get
			{
				return this.ReadInt16 (10);
			}
		}
		
		public int		RangeShift
		{
			get
			{
				return this.ReadInt16 (12);
			}
		}
		
		
		public override ushort GetGlyphIndex(int n)
		{
			int code = (int) n;
			
			int max = (int) this.SegCountX2;
			
			int o_end_code   = 14;
			int o_start_code = o_end_code + max + 2;
			int o_id_delta   = o_start_code + max;
			int o_id_range_o = o_id_delta + max;
			int o_glyph_ids  = o_id_range_o + max;
			
			for (int i = 0; i < max; i += 2)
			{
				if (this.ReadInt16 (o_end_code + i) >= code)
				{
					int start = this.ReadInt16 (o_start_code + i);
					
					if (start <= code)
					{
						int id_range_offset = this.ReadInt16 (o_id_range_o + i);
						int index = 0;
						
						if (id_range_offset != 0)
						{
							index = this.ReadInt16 (o_id_range_o + i + (int) id_range_offset + 2 * (int)(code - start));
							
							if (index != 0)
							{
								index += this.ReadInt16 (o_id_delta + i);
							}
						}
						else
						{
							index = this.ReadInt16 (o_id_delta + i) + code;
						}
						
						return (ushort)(index);
					}
					else
					{
						return 0;
					}
				}
			}
			
			throw new System.InvalidOperationException ();
		}
	}
	
	public class IndexMappingTable12 : IndexMappingTable
	{
		public IndexMappingTable12(byte[] data, int offset) : base (data, offset)
		{
		}
		
		
		public int		LongLength
		{
			get
			{
				return this.ReadInt32 (4);
			}
		}
		
		public int		Language
		{
			get
			{
				return this.ReadInt32 (8);
			}
		}
		
		public int		NumGroups
		{
			get
			{
				return this.ReadInt16 (12);
			}
		}
		
		
		public override ushort GetGlyphIndex(int n)
		{
			int code = (int) n;
			
			int max = (int) this.NumGroups;
			
			int o_start_char_code = 16;
			int o_end_char_code   = 20;
			int o_start_glyph_id  = 24;
			
			for (int i = 0; i < max; i++)
			{
				if (code <= this.ReadInt32 (12*i + o_end_char_code))
				{
					if (code >= this.ReadInt32 (12*i + o_start_char_code))
					{
						return (ushort) (code - this.ReadInt32 (12*i + o_start_char_code)
							/**/              + this.ReadInt32 (12*i + o_start_glyph_id));
					}
					
					return 0;
				}
			}
			
			throw new System.InvalidOperationException ();
		}
	}
	
	public class Table_name : Tables
	{
		public Table_name(byte[] data, int offset) : base (data, offset)
		{
		}
		
		public Table_name(TableEntry entry) : base (entry.BaseData, entry.Offset)
		{
		}
		
		
		public int		FormatSelector
		{
			get
			{
				return this.ReadInt16 (0);
			}
		}
		
		public int		NumNameRecords
		{
			get
			{
				return this.ReadInt16 (2);
			}
		}
		
		public int		StorageAreaOffset
		{
			get
			{
				return this.ReadInt16 (4);
			}
		}
		
		
		public struct NameEncoding
		{
			public PlatformId		Platform;
			public int				Encoding;
			public int				Language;
			public NameId			Name;
		}
		
		public NameEncoding[] GetAvailableNameEncodings()
		{
			int num = (int) this.NumNameRecords;
			
			NameEncoding[] encodings = new NameEncoding[num];
			
			int o_platform_id   = 6;
			int o_encoding_id   = 8;
			int o_language_id   = 10;
			int o_name_id       = 12;
			
			for (int i = 0; i < num; i++)
			{
				encodings[i].Platform = (PlatformId) (this.ReadInt16 (i*12 + o_platform_id));
				encodings[i].Encoding = this.ReadInt16 (i*12 + o_encoding_id);
				encodings[i].Language = this.ReadInt16 (i*12 + o_language_id);
				encodings[i].Name     = (NameId) (this.ReadInt16 (i*12 + o_name_id));
			}
			
			return encodings;
		}
		
		
		public string GetLatinName(int language, NameId name, PlatformId platform)
		{
			int num = (int) this.NumNameRecords;
			
			int lang_id = (int) language;
			int name_id = (int) name;
			int plat_id = (int) platform;
			
			int o_platform_id   = 6;
			int o_encoding_id   = 8;
			int o_language_id   = 10;
			int o_name_id       = 12;
			int o_string_length = 14;
			int o_string_offset = 16;
			
			for (int i = 0; i < num; i++)
			{
				if ((this.ReadInt16 (i*12 + o_platform_id) == plat_id) &&
					(this.ReadInt16 (i*12 + o_encoding_id) == 0) &&
					(this.ReadInt16 (i*12 + o_language_id) == lang_id) &&
					(this.ReadInt16 (i*12 + o_name_id)     == name_id))
				{
					int length = (int) (this.ReadInt16 (i*12 + o_string_length));
					int offset = (int) (this.ReadInt16 (i*12 + o_string_offset) + this.StorageAreaOffset);
					
					System.Text.StringBuilder buffer = new System.Text.StringBuilder ();
					
					for (int j = 0; j < length; j++)
					{
						buffer.Append ((char) this.ReadInt8 (offset+j));
					}
					
					return buffer.ToString ();
				}
			}
			
			return null;
		}
		
		public string GetUnicodeName(int language, NameId name, PlatformId platform)
		{
			int num = (int) this.NumNameRecords;
			
			int lang_id = (int) language;
			int name_id = (int) name;
			int plat_id = (int) platform;
			
			int o_platform_id   = 6;
			int o_encoding_id   = 8;
			int o_language_id   = 10;
			int o_name_id       = 12;
			int o_string_length = 14;
			int o_string_offset = 16;
			
			for (int i = 0; i < num; i++)
			{
				if ((this.ReadInt16 (i*12 + o_platform_id) == plat_id) &&
					((this.ReadInt16 (i*12 + o_encoding_id) == 1) || (plat_id == 0)) &&
					(this.ReadInt16 (i*12 + o_language_id) == lang_id) &&
					(this.ReadInt16 (i*12 + o_name_id)     == name_id))
				{
					int length = (int) (this.ReadInt16 (i*12 + o_string_length));
					int offset = (int) (this.ReadInt16 (i*12 + o_string_offset) + this.StorageAreaOffset);
					
					System.Text.StringBuilder buffer = new System.Text.StringBuilder ();
					
					for (int j = 0; j < length; j += 2)
					{
						buffer.Append ((char) this.ReadInt16 (offset+j));
					}
					
					return buffer.ToString ();
				}
			}
			
			return null;
		}
		
		public string GetUniqueFontIdentifier()
		{
			string name;
			
			name = this.GetLatinName (0, NameId.UniqueFontIdentifier, PlatformId.Macintosh);
			
			if (name == null)
			{
				name = this.GetUnicodeName (1033, NameId.UniqueFontIdentifier, PlatformId.Microsoft);
			}
			
			return name;
		}
		
		public string GetFullFontName()
		{
			string name;
			
			name = this.GetLatinName (0, NameId.FullFontName, PlatformId.Macintosh);
			
			if (name == null)
			{
				name = this.GetUnicodeName (1033, NameId.FullFontName, PlatformId.Microsoft);
			}
			
			return name;
		}
	}
	
	#region PlatformId and NameId Enumeration
	public enum PlatformId
	{
		Unicode		= 0,
		Macintosh	= 1,
		Microsoft	= 3,
		Custom		= 4,
	}
	
	public enum NameId
	{
		CopyrightNotice,
		FontFamily,				//	"Futura Lt BT"
		FontSubfamily,			//	"Light Italic"
		UniqueFontIdentifier,	//	"Futura Light Italic, Geometric 211"
		FullFontName,			//	"Futura Lt BT Light Italic"
		Version,				//	"Version 2.001 mfgpctt 4.4"
		PostScriptName,			//	"FuturaBT-LightItalic"
		Trademark,
		Manufacturer,
		Designer,
		Description,
		VendorURL,
		DesignerURL,
		License,
		LicenseURL,
		Reserved_0,
		PreferredFamily,
		PreferredSubfamily,
		Mac_CompatibleFull,
		SampleText,
		PostScriptCID,
	}
	#endregion
	
	public class Table_hhea : Tables
	{
		//	http://partners.adobe.com/public/developer/opentype/index_hhea.html
		
		public Table_hhea(byte[] data, int offset) : base (data, offset)
		{
		}
		
		public Table_hhea(TableEntry entry) : base (entry.BaseData, entry.Offset)
		{
		}
		
		
		public int		TableVersion
		{
			get
			{
				return this.ReadInt32 (0);
			}
		}
		
		public int		MacAscender
		{
			get
			{
				return (short) this.ReadInt16 (4);
			}
		}
		
		public int		MacDescender
		{
			get
			{
				return (short) this.ReadInt16 (6);
			}
		}
		
		public int		MacLineGap
		{
			get
			{
				return (short) this.ReadInt16 (8);
			}
		}
		
		public int		AdvanceWidthMax
		{
			get
			{
				return this.ReadInt16 (10);
			}
		}
		
		public int		MinLeftSideBearing
		{
			get
			{
				return (short) this.ReadInt16 (12);
			}
		}
		
		public int		MinRightSideBearing
		{
			get
			{
				return (short) this.ReadInt16 (14);
			}
		}
		
		public int		XMaxExtent
		{
			get
			{
				return (short) this.ReadInt16 (16);
			}
		}
		
		public int		CaretSlopeRise
		{
			get
			{
				return this.ReadInt16 (18);
			}
		}
		
		public int		CaretSlopeRun
		{
			get
			{
				return this.ReadInt16 (20);
			}
		}
		
		public int		CaretOffset
		{
			get
			{
				return this.ReadInt16 (22);
			}
		}
		
		public int		MetricDataFormat
		{
			get
			{
				return this.ReadInt16 (32);
			}
		}
		
		public int		NumHMetrics
		{
			get
			{
				return this.ReadInt16 (34);
			}
		}
	}
	
	public class Table_hmtx : Tables
	{
		//	http://partners.adobe.com/public/developer/opentype/index_hmtx.html
		
		public Table_hmtx(byte[] data, int offset) : base (data, offset)
		{
		}
		
		public Table_hmtx(TableEntry entry) : base (entry.BaseData, entry.Offset)
		{
		}
		
		
		public int GetAdvanceWidth(int n)
		{
			return this.ReadInt16 ((int)(n*4+0));
		}
		
		public int GetLeftSideBearing(int n)
		{
			return (short) this.ReadInt16 ((int)(n*4+2));
		}
		
		public int GetExtraLeftSideBearing(int number_h_metrics, int n)
		{
			return (short) this.ReadInt16 ((int)(number_h_metrics*4 + n*2));
		}
	}
	
	public class Table_kern : Tables
	{
		public Table_kern(byte[] data, int offset) : base (data, offset)
		{
		}
		
		public Table_kern(TableEntry entry) : base (entry.BaseData, entry.Offset)
		{
		}
		
		
		public int		TableVersion
		{
			get
			{
				return this.ReadInt16 (0);
			}		
		}
		
		public int		Count
		{
			get
			{
				return this.ReadInt16 (2);
			}		
		}
		
		
		public KerningTable	GetKerningTable(int n)
		{
			if ((n >= 0) &&
				(n < this.Count))
			{
				int offset = 4;
				
				for (int i = 0; i < n; i++)
				{
					offset += this.ReadInt16 (offset + 2);
				}
				
				return new KerningTable (this.data, this.offset + offset);
			}
			
			return null;
		}
	}
	
	public class KerningTable : Tables
	{
		public KerningTable(byte[] data, int offset) : base (data, offset)
		{
		}
		
		
		public int		TableVersion
		{
			get
			{
				return this.ReadInt16 (0);
			}
		}
		
		public int		Length
		{
			get
			{
				return this.ReadInt16 (2);
			}
		}
		
		public int		Coverage
		{
			get
			{
				return this.ReadInt16 (4);
			}
		}
		
		public bool		IsHorizontal
		{
			get
			{
				return (this.Coverage & 0x0001) == 0x0001;
			}
		}
		
		public bool		HasMinimumValues
		{
			get
			{
				return (this.Coverage & 0x0002) == 0x0002;
			}
		}
		
		public bool		IsCrossStream
		{
			get
			{
				return (this.Coverage & 0x0004) == 0x0004;
			}
		}
		
		public bool		IsOverride
		{
			get
			{
				return (this.Coverage & 0x0008) == 0x0008;
			}
		}
				
		public int		SubtableFormat
		{
			get
			{
				return (this.Coverage & 0xff00) >> 8;
			}
		}
		
		
		public KerningTableFormat0	Format0Subtable
		{
			get
			{
				if (this.SubtableFormat == 0)
				{
					return new KerningTableFormat0 (this.data, this.offset);
				}
				
				return null;
			}
		}
	}
	
	public class KerningTableFormat0 : KerningTable
	{
		public KerningTableFormat0(byte[] data, int offset) : base (data, offset)
		{
		}
		
		
		public int		PairCount
		{
			get
			{
				return this.ReadInt16 (6);
			}
		}
		
		public int		SearchRange
		{
			get
			{
				return this.ReadInt16 (8);
			}
		}
		
		public int		EntrySelector
		{
			get
			{
				return this.ReadInt16 (10);
			}
		}
		
		public int		RangeShift
		{
			get
			{
				return this.ReadInt16 (12);
			}
		}
		
		
		public int GetLeftGlyph(int n)
		{
			return this.ReadInt16 (14+n*6);
		}
		
		public int GetRightGlyph(int n)
		{
			return this.ReadInt16 (16+n*6);
		}
		
		
		public uint GetLeftRightCombined(int n)
		{
			return (uint) this.ReadInt32 (14+n*6);
		}
		
		public int GetKernValue(int n)
		{
			return this.ReadInt16 (18+n*6);
		}
		
		public bool FindKernValue(int left, int right, out int value)
		{
			uint combined = (uint)(left << 16) | (uint)(right & 0xffff);
			
			int range = this.SearchRange;
			int index = this.EntrySelector;
			int count = this.PairCount;
			int pow_2 = range;
			int fence = count * 6;
			
			for (int i = 0; ; i++)
			{
				uint test = this.GetLeftRightCombined (range / 6);
				
				if (test == combined)
				{
					value = this.GetKernValue (range / 6);
					return true;
				}
				
				if (i >= index)
				{
					if (range == 6)
					{
						test = this.GetLeftRightCombined (0);
					
						if (test == combined)
						{
							value = this.GetKernValue (0);
							return true;
						}
					}
					
					break;
				}
				
				pow_2 >>= 1;
				
				if (test > combined)
				{
					range -= pow_2;
				}
				else
				{
					range += pow_2;
					
					while (range >= fence)
					{
						pow_2 >>= 1;
						range  -= pow_2;
						
						if (++i >= index)
						{
							value = 0;
							return false;
						}
					}
				}
			}
			
			value = 0;
			return false;
		}
	}
	
	
	public class Table_GDEF : Tables
	{
		//	http://partners.adobe.com/public/developer/opentype/index_table_formats5.html
		//	http://www.microsoft.com/OpenType/OTSpec/gdef.htm
		
		public Table_GDEF(byte[] data, int offset) : base (data, offset)
		{
		}
		
		public Table_GDEF(TableEntry entry) : base (entry.BaseData, entry.Offset)
		{
		}
		
		
		public int						TableVersion
		{
			get
			{
				return this.ReadInt32 (0);
			}
		}
		
		public int						GlyphClassDefOffset
		{
			get
			{
				return this.ReadInt16 (4);
			}
		}
		
		public int						AttachListOffset
		{
			get
			{
				return this.ReadInt16 (6);
			}
		}
		
		public int						LigatureCaretListOffset
		{
			get
			{
				return this.ReadInt16 (8);
			}
		}
		
		public int						MarkAttachClassDefOffset
		{
			get
			{
				return this.ReadInt16 (10);
			}
		}
		
		
		public LigatureCaretListTable	LigatureCaretListTable
		{
			get
			{
				if (this.LigatureCaretListOffset > 0)
				{
					return new LigatureCaretListTable(this.data, this.offset + this.LigatureCaretListOffset);
				}
				else
				{
					return null;
				}
			}
		}
	}
	
	public class LigatureCaretListTable : Tables
	{
		public LigatureCaretListTable(byte[] data, int offset) : base (data, offset)
		{
		}
		
		
		public int						CoverageOffset
		{
			get
			{
				return this.ReadInt16 (0);
			}
		}
		
		public Coverage					Coverage
		{
			get
			{
				return new Coverage (this.data, this.offset + (int) this.CoverageOffset);
			}
		}
		
		public int						LigatureGlyphCount
		{
			get
			{
				return this.ReadInt16 (2);
			}
		}
		
		
		public int GetLigatureGlyphOffset(int n)
		{
			return this.ReadInt16 ((int)(4+n*2));
		}
		
		public LigatureGlyphTable GetLigatureGlyphTable(int n)
		{
			return new LigatureGlyphTable (this.data, this.offset + (int) this.GetLigatureGlyphOffset (n));
		}
	}
	
	public class LigatureGlyphTable : Tables
	{
		public LigatureGlyphTable(byte[] data, int offset) : base (data, offset)
		{
		}
		
		
		public int						CaretCount
		{
			get
			{
				return this.ReadInt16 (0);
			}
		}
		
		
		public int GetCaretOffset(int n)
		{
			return this.ReadInt16 (2+2*n);
		}
		
		public int GetCaretValueFormat(int n)
		{
			return this.ReadInt16 (this.GetCaretOffset (n) + 0);
		}
		
		public int GetCaretCoordinateFmt1(int n)
		{
			return this.ReadInt16 (this.GetCaretOffset (n) + 2);
		}
		
		public int GetCaretValuePointFmt2(int n)
		{
			return this.ReadInt16 (this.GetCaretOffset (n) + 2);
		}
		
		public int GetCaretCoordinateFmt3(int n)
		{
			return this.ReadInt16 (this.GetCaretOffset (n) + 2);
		}
		
		public int GetCaretDeviceTableFmt3(int n)
		{
			return this.ReadInt16 (this.GetCaretOffset (n) + 4);
		}
	}
	
	
	public class Table_GSUB : Tables
	{
		//	See http://www.microsoft.com/OpenType/OTSpec/gsub.htm or
		//	http://partners.adobe.com/public/developer/opentype/index_table_formats1.html

		//	See http://www.microsoft.com/OpenType/OTSpec/chapter2.htm for the common table format
		//	See http://www.microsoft.com/OpenType/OTSpec/featurelist.htm for 'liga' and others.
		
		public Table_GSUB(byte[] data, int offset) : base (data, offset)
		{
		}
		
		public Table_GSUB(TableEntry entry) : base (entry.BaseData, entry.Offset)
		{
		}
		
		
		public int		TableVersion
		{
			get
			{
				return this.ReadInt32 (0);
			}
		}
		
		public int		ScriptListOffset
		{
			get
			{
				return this.ReadInt16 (4);
			}
		}
		
		public int		FeatureListOffset
		{
			get
			{
				return this.ReadInt16 (6);
			}
		}
		
		public int		LookupListOffset
		{
			get
			{
				return this.ReadInt16 (8);
			}
		}
		
		
		public ScriptListTable	ScriptListTable
		{
			get
			{
				return new ScriptListTable (this.data, (int)(this.offset + this.ScriptListOffset));
			}
		}
		
		public FeatureListTable	FeatureListTable
		{
			get
			{
				return new FeatureListTable (this.data, (int)(this.offset + this.FeatureListOffset));
			}
		}
		
		public LookupListTable	LookupListTable
		{
			get
			{
				return new LookupListTable (this.data, (int)(this.offset + this.LookupListOffset));
			}
		}
		
		
		public int GetRequiredFeatureIndex(string script_tag, string language_tag)
		{
			ScriptTable script_table = this.ScriptListTable.GetScriptTable (script_tag);
			
			if (script_table != null)
			{
				LangSysTable lang_sys_table = script_table.GetLangSysTable (language_tag);
				
				if (lang_sys_table == null)
				{
					lang_sys_table = script_table.DefaultLangSysTable;
				}
				
				return lang_sys_table.RequiredFeatureIndex;
			}
			else
			{
				return 0xffff;
			}
		}
		
		
		public int[] GetFeatureIndexes(string script_tag, string language_tag)
		{
			ScriptTable script_table = this.ScriptListTable.GetScriptTable (script_tag);
			
			if (script_table != null)
			{
				LangSysTable lang_sys_table = script_table.GetLangSysTable (language_tag);
				
				if (lang_sys_table == null)
				{
					lang_sys_table = script_table.DefaultLangSysTable;
				}
				
				int   f_count  = lang_sys_table.FeatureCount;
				int[] features = new int[f_count];
				
				for (int i = 0; i < f_count; i++)
				{
					features[i] = lang_sys_table.GetFeatureIndex (i);
				}
				
				return features;
			}
			else
			{
				return new int[0];
			}
		}
		
		public int[] GetFeatureIndexes(string feature_tag)
		{
			FeatureListTable table = this.FeatureListTable;
			
			int max = table.FeatureCount;
			int hit = 0xffff;
			int  num = 0;
			
			for (int i = 0; i < max; i++)
			{
				if (table.GetFeatureTag (i) == feature_tag)
				{
					if (num == 0)
					{
						hit = i;
					}
					
					num++;
				}
			}
			
			int[] features = new int[num];
			int    index    = 0;
			
			for (int i = hit; i < max; i++)
			{
				if (table.GetFeatureTag (i) == feature_tag)
				{
					features[index++] = i;
					
					if (index == num)
					{
						break;
					}
				}
			}
			
			return features;
		}
		
	}
	
	public class ScriptListTable : Tables
	{
		public ScriptListTable(byte[] data, int offset) : base (data, offset)
		{
		}
		
		
		public int		ScriptCount
		{
			get
			{
				return this.ReadInt16 (0);
			}
		}
		
		
		public string GetScriptTag(int n)
		{
			char[] tag = new char[4];
			int offset = (int)(2+6*n+0);
				
			tag[0] = (char) this.ReadInt8 (offset+0);
			tag[1] = (char) this.ReadInt8 (offset+1);
			tag[2] = (char) this.ReadInt8 (offset+2);
			tag[3] = (char) this.ReadInt8 (offset+3);
				
			return new string (tag);
		}
		
		public int GetScriptOffset(int n)
		{
			return this.ReadInt16 ((int)(2+6*n+4));
		}
		
		public ScriptTable GetScriptTable(int n)
		{
			return new ScriptTable (this.data, this.offset + (int) this.GetScriptOffset (n));
		}
		
		public ScriptTable GetScriptTable(string tag)
		{
			int max  = this.ScriptCount;
			
			for (int i = 0; i < max; i++)
			{
				if (this.GetScriptTag (i) == tag)
				{
					return this.GetScriptTable (i);
				}
			}
			
			return null;
		}
		
		public bool ContainsScript(string tag)
		{
			int max  = this.ScriptCount;
			
			for (int i = 0; i < max; i++)
			{
				if (this.GetScriptTag (i) == tag)
				{
					return true;
				}
			}
			
			return false;
		}
	}
	
	public class ScriptTable : Tables
	{
		public ScriptTable(byte[] data, int offset) : base (data, offset)
		{
		}
		
		
		public int			DefaultLangSysOffset
		{
			get
			{
				return this.ReadInt16 (0);
			}
		}
		
		public LangSysTable	DefaultLangSysTable
		{
			get
			{
				return new LangSysTable (this.data, this.offset + (int) this.DefaultLangSysOffset);
			}
		}
		
		public int			LangSysCount
		{
			get
			{
				return this.ReadInt16 (2);
			}
		}
		
		
		public string GetLangSysTag(int n)
		{
			char[] tag = new char[4];
			int offset = (int)(4+6*n+0);
				
			tag[0] = (char) this.ReadInt8 (offset+0);
			tag[1] = (char) this.ReadInt8 (offset+1);
			tag[2] = (char) this.ReadInt8 (offset+2);
			tag[3] = (char) this.ReadInt8 (offset+3);
				
			return new string (tag);
		}
		
		public int GetLangSysOffset(int n)
		{
			return this.ReadInt16 ((int)(4+6*n+4));
		}
		
		
		public LangSysTable GetLangSysTable(int n)
		{
			return new LangSysTable (this.data, this.offset + (int) this.GetLangSysOffset (n));
		}
		
		public LangSysTable GetLangSysTable(string tag)
		{
			int max  = this.LangSysCount;
			
			for (int i = 0; i < max; i++)
			{
				if (this.GetLangSysTag (i) == tag)
				{
					return this.GetLangSysTable (i);
				}
			}
			
			return null;
		}
	}
	
	public class LangSysTable : Tables
	{
		public LangSysTable(byte[] data, int offset) : base (data, offset)
		{
		}
		
		
		public int		RequiredFeatureIndex
		{
			get
			{
				return this.ReadInt16 (2);
			}
		}
		
		public int		FeatureCount
		{
			get
			{
				return this.ReadInt16 (4);
			}
		}
		
		
		public int GetFeatureIndex(int n)
		{
			return this.ReadInt16 ((int)(6+2*n));
		}
	}
	
	public class FeatureListTable : Tables
	{
		public FeatureListTable(byte[] data, int offset) : base (data, offset)
		{
		}
		
		
		public int		FeatureCount
		{
			get
			{
				return this.ReadInt16 (0);
			}
		}
		
		
		public string GetFeatureTag(int n)
		{
			char[] tag = new char[4];
			int offset = (int)(2+6*n+0);
				
			tag[0] = (char) this.ReadInt8 (offset+0);
			tag[1] = (char) this.ReadInt8 (offset+1);
			tag[2] = (char) this.ReadInt8 (offset+2);
			tag[3] = (char) this.ReadInt8 (offset+3);
				
			return new string (tag);
		}
		
		public int GetFeatureOffset(int n)
		{
			return this.ReadInt16 ((int)(2+6*n+4));
		}
		
		public FeatureTable GetFeatureTable(int n)
		{
			return new FeatureTable (this.data, this.offset + (int) this.GetFeatureOffset (n));
		}
		
		public TaggedFeatureTable GetTaggedFeatureTable(int n)
		{
			return new TaggedFeatureTable (this.data, this.offset + (int) this.GetFeatureOffset (n), this.GetFeatureTag (n));
		}
		
		public FeatureTable GetFeatureTable(string tag)
		{
			int max  = this.FeatureCount;
			
			for (int i = 0; i < max; i++)
			{
				if (this.GetFeatureTag (i) == tag)
				{
					return this.GetFeatureTable (i);
				}
			}
			
			return null;
		}
	}
	
	public class FeatureTable : Tables
	{
		public FeatureTable(byte[] data, int offset) : base (data, offset)
		{
		}
		
		
		public int		LookupCount
		{
			get
			{
				return this.ReadInt16 (2);
			}
		}
		
		
		public int GetLookupIndex(int n)
		{
			return this.ReadInt16 ((int)(4+2*n));
		}
	}
	
	public class TaggedFeatureTable : FeatureTable
	{
		public TaggedFeatureTable(byte[] data, int offset, string tag) : base (data, offset)
		{
			this.tag = tag;
		}
		
		
		public string		Tag
		{
			get
			{
				return this.tag;
			}
		}
		
		
		private string		tag;
	}
		
	public class LookupListTable : Tables
	{
		public LookupListTable(byte[] data, int offset) : base (data, offset)
		{
		}
		
		
		public int		LookupCount
		{
			get
			{
				return this.ReadInt16 (0);
			}
		}
		
		
		public int GetLookupOffset(int n)
		{
			return this.ReadInt16 ((int)(2+n*2));
		}
		
		public LookupTable GetLookupTable(int n)
		{
			return new LookupTable (this.data, this.offset + (int) this.GetLookupOffset (n));
		}
	}
	
	public class LookupTable : Tables
	{
		public LookupTable(byte[] data, int offset) : base (data, offset)
		{
		}
		
		
		public int		LookupType
		{
			get
			{
				return this.ReadInt16 (0);
			}
		}
		
		public int		LookupFlags
		{
			get
			{
				return this.ReadInt16 (2);
			}
		}
		
		public int		SubTableCount
		{
			get
			{
				return this.ReadInt16 (4);
			}
		}
		
		
		public int GetSubTableOffset(int n)
		{
			return this.ReadInt16 ((int)(6+n*2));
		}
		
		public SubstSubTable GetSubTable(int n)
		{
			return new SubstSubTable (this.data, this.offset + (int) this.GetSubTableOffset (n));
		}
		
		public BaseSubstitution GetSubstitution(int n)
		{
			int offset = this.offset + (int) this.GetSubTableOffset (n);
			
			switch (this.LookupType)
			{
				case 1: return new SingleSubstitution (this.data, offset);
				case 4: return new LigatureSubstitution (this.data, offset);
				case 6: return new ChainingContextSubstitution (this.data, offset);
				
				default:
					throw new System.NotSupportedException ();
			}
		}
	}
	
	
	public class SubstSubTable : Tables
	{
		public SubstSubTable(byte[] data, int offset) : base (data, offset)
		{
		}
		
		
		public int					SubstFormat
		{
			get
			{
				return this.ReadInt16 (0);
			}
		}
		
		protected virtual int		CoverageOffset
		{
			get
			{
				return this.ReadInt16 (2);
			}
		}
		
		public Coverage				Coverage
		{
			get
			{
				return new Coverage (this.data, this.offset + (int) this.CoverageOffset);
			}
		}
	}
	
	public class Coverage : Tables
	{
		public Coverage(byte[] data, int offset) : base (data, offset)
		{
		}
		
		
		public int		CoverageFormat
		{
			get
			{
				return this.ReadInt16 (0);
			}
		}
		
		
		public int FindIndex(int glyph)
		{
			switch (this.CoverageFormat)
			{
				case 1: return this.FindIndexFmt1 (glyph);
				case 2: return this.FindIndexFmt2 (glyph);
				
				default:
					throw new System.NotSupportedException ();
			}
		}
		
		
		private int FindIndexFmt1(int glyph)
		{
			int max  = this.ReadInt16 (2);
			int dist = max / 2;
			int iter = dist;
			
			for (;;)
			{
				int find = this.ReadInt16 ((int)(4+2*iter));
				
				if (find == glyph)
				{
					return (int) iter;
				}
				
				if (dist < 3)
				{
					if (find < glyph)
					{
						while ((find < glyph)
							&& (iter < max-1))
						{
							iter++;
							find = this.ReadInt16 ((int)(4+2*iter));
							
							if (find == glyph)
							{
								return (int) iter;
							}
						}
						
						return -1;
					}
					else
					{
						while ((find > glyph)
							&& (iter > 0))
						{
							iter--;
							find = this.ReadInt16 ((int)(4+2*iter));
							
							if (find == glyph)
							{
								return (int) iter;
							}
						}
						
						return -1;
					}
				}
				
				dist = dist / 2;
				
				if (find < glyph)
				{
					iter -= dist;
				}
				else
				{
					iter += dist;
				}
			}
		}
		
		private int FindIndexFmt2(int glyph)
		{
			int range = this.ReadInt16 (2);
			
			for (int i = 0; i < range; i++)
			{
				int start = this.ReadInt16 ((int)(4+6*i+0));
				int end   = this.ReadInt16 ((int)(4+6*i+2));
				
				if ((glyph >= start) &&
					(glyph <= end))
				{
					int start_coverage = this.ReadInt16 ((int)(4+6*i+4));
					
					return (int)(start_coverage + glyph - start);
				}
			}
			
			return -1;
		}
	}
	
	public class BaseSubstitution : SubstSubTable
	{
		public BaseSubstitution(byte[] data, int offset) : base (data, offset)
		{
		}
		
		
		public virtual bool ProcessSubstitution(ushort[] i_glyphs, ref int i_offset, int length, ushort[] o_glyphs, ref int o_offset)
		{
			return false;
		}
	}
		
	
	public class SingleSubstitution : BaseSubstitution
	{
		public SingleSubstitution(SubstSubTable sub) : base (sub.BaseData, sub.BaseOffset)
		{
		}
		
		public SingleSubstitution(byte[] data, int offset) : base (data, offset)
		{
		}
		
		
		public ushort FindSubstitution(int glyph)
		{
			switch (this.SubstFormat)
			{
				case 1: return this.FindSubstitutionFmt1 (glyph);
				case 2: return this.FindSubstitutionFmt2 (glyph);
				
				default:
					throw new System.NotSupportedException ();
			}
		}
		
		
		public override bool ProcessSubstitution(ushort[] i_glyphs, ref int i_offset, int length, ushort[] o_glyphs, ref int o_offset)
		{
			length -= i_offset;
			
			if (length > 0)
			{
				int cov = this.Coverage.FindIndex (i_glyphs[i_offset]);
				
				if (cov >= 0)
				{
					o_glyphs[o_offset] = this.FindSubstitution (i_glyphs[i_offset]);
					
					i_offset += 1;
					o_offset += 1;
					
					return true;
				}
			}
			
			return false;
		}
		
		
		private ushort FindSubstitutionFmt1(int glyph)
		{
			return (ushort) (this.ReadInt16 (4) + glyph);
		}
		
		private ushort FindSubstitutionFmt2(int glyph)
		{
			int max = this.ReadInt16 (4);
			int  cov = this.Coverage.FindIndex (glyph);
			
			if ((cov >= 0) &&
				(cov < max))
			{
				return (ushort) (this.ReadInt16 ((int)(6+2*cov)));
			}
			
			return (ushort) (0xffff);
		}
	}
	
	public class ChainingContextSubstitution : BaseSubstitution
	{
		public ChainingContextSubstitution(SubstSubTable sub) : base (sub.BaseData, sub.BaseOffset)
		{
		}
		
		public ChainingContextSubstitution(byte[] data, int offset) : base (data, offset)
		{
		}
		
		
		protected override int		CoverageOffset
		{
			get
			{
				switch (this.SubstFormat)
				{
					case 1:
					case 2:
						return base.CoverageOffset;
					case 3:
						return this.ReadInt16 (4);
					default:
						throw new System.NotSupportedException ();
				}
			}
		}

		public int					ChainSubRuleSetCount
		{
			get
			{
				return this.ReadInt16 (4);
			}
		}
	}
	
	public class LigatureSubstitution : BaseSubstitution
	{
		public LigatureSubstitution(SubstSubTable sub) : base (sub.BaseData, sub.BaseOffset)
		{
		}
		
		public LigatureSubstitution(byte[] data, int offset) : base (data, offset)
		{
		}
		
		
		public int		LigatureSetCount
		{
			get
			{
				return this.ReadInt16 (4);
			}
		}
		
		
		public int GetLigatureSetOffset(int n)
		{
			return this.ReadInt16 ((int)(6+n*2));
		}
		
		private int GetLigatureSetInfoCount(int offset, int n)
		{
			return this.ReadInt16 ((int)(offset+0));
		}
		
		private int GetLigatureSetInfoOffset(int offset, int info)
		{
			return this.ReadInt16 ((int)(offset+2+2*info));
		}
		
		
		public LigatureSet GetLigatureSet(int n)
		{
			return new LigatureSet (this.data, this.offset + (int) this.GetLigatureSetOffset (n));
		}
		
		
		public override bool ProcessSubstitution(ushort[] i_glyphs, ref int i_offset, int length, ushort[] o_glyphs, ref int o_offset)
		{
			length -= i_offset;
			
			if (length > 0)
			{
				int cov = this.Coverage.FindIndex (i_glyphs[i_offset]);
				
				if (cov >= 0)
				{
					int max_set = this.LigatureSetCount;
					
					Debug.Assert.IsTrue (cov < max_set);
					
					int set_offset = this.GetLigatureSetOffset ((int)cov);
					int max_info   = this.GetLigatureSetInfoCount (set_offset, (int)cov);
					
					for (int j = 0; j < max_info; j++)
					{
						int info_offset = this.GetLigatureSetInfoOffset (set_offset, j) + set_offset;
						int comp_count  = this.ReadInt16 ((int)(2+info_offset));
						
						if (comp_count <= length)
						{
							for (int k = 1; k < comp_count; k++)
							{
								int comp_elem = this.ReadInt16 ((int)(2+info_offset+2*k));
								
								if (comp_elem != i_glyphs[i_offset+k])
								{
									goto try_next_in_set;
								}
							}
							
							//	Hit: input glyph sequence matched.
							
							o_glyphs[o_offset] = (ushort) this.ReadInt16 ((int)(0+info_offset));
							
							i_offset += comp_count;
							o_offset += 1;
							
							return true;
						}
						
					try_next_in_set:
						continue;
					}
				}
			}
			
			return false;
		}
	}
	
	public class LigatureSet : Tables
	{
		public LigatureSet(byte[] data, int offset) : base (data, offset)
		{
		}
		
		
		public int		LigatureInfoCount
		{
			get
			{
				return this.ReadInt16 (0);
			}
		}
		
		
		public int GetLigatureInfoOffset(int n)
		{
			return this.ReadInt16 ((int)(2+2*n));
		}
		
		public LigatureInfo GetLigatureInfo(int n)
		{
			return new LigatureInfo (this.data, this.offset + (int) this.GetLigatureInfoOffset (n));
		}
	}
	
	public class LigatureInfo : Tables
	{
		public LigatureInfo(byte[] data, int offset) : base (data, offset)
		{
		}
		
		
		public int		Glyph
		{
			get
			{
				return this.ReadInt16 (0);
			}
		}
		
		public int		ComponentCount
		{
			get
			{
				return this.ReadInt16 (2);
			}
		}
		
		
		public int GetComponent(int n)
		{
			return this.ReadInt16 ((int)(4+n*2));
		}
	}
}
