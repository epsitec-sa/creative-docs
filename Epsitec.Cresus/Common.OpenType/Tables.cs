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
		
		protected uint ReadInt8(int offset)
		{
			return (uint) this.data[this.offset + offset];
		}
		
		protected uint ReadInt16(int offset)
		{
			return Support.ReadInt16 (this.data, this.offset + offset);
		}
		
		protected uint ReadInt32(int offset)
		{
			return Support.ReadInt32 (this.data, this.offset + offset);
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
		
		public uint		Checksum
		{
			get
			{
				return this.ReadInt32 (4);
			}
		}
		
		public uint		Offset
		{
			get
			{
				return this.ReadInt32 (8);
			}
		}
		
		public uint		Length
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
		
		
		public uint		Version
		{
			get
			{
				return this.ReadInt32 (0);
			}
		}
		
		public uint		NumTables
		{
			get
			{
				return this.ReadInt16 (4);
			}
		}
		
		public uint		SearchRange
		{
			get
			{
				return this.ReadInt16 (6);
			}
		}
		
		public uint		EntrySelector
		{
			get
			{
				return this.ReadInt16 (8);
			}
		}
		
		public uint		RangeShift
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
		
		
		public uint		TableVersion
		{
			get
			{
				return this.ReadInt32 (0);
			}
		}
		
		public uint		FontRevision
		{
			get
			{
				return this.ReadInt32 (4);
			}
		}
		
		public uint		ChecksumAdjustment
		{
			get
			{
				return this.ReadInt32 (8);
			}
		}
		
		public uint		MagicNumber
		{
			get
			{
				return this.ReadInt32 (12);
			}
		}
		
		public uint		Flags
		{
			get
			{
				return this.ReadInt16 (16);
			}
		}
		
		public uint		UnitsPerEm
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
				ulong h = this.ReadInt32 (20);
				ulong l = this.ReadInt32 (24);
				return (h << 32) | l;
			}
		}
		
		public ulong	Modified
		{
			get
			{
				ulong h = this.ReadInt32 (28);
				ulong l = this.ReadInt32 (32);
				return (h << 32) | l;
			}
		}
		
		public uint		XMin
		{
			get
			{
				return this.ReadInt16 (36);
			}
		}
		
		public uint		YMin
		{
			get
			{
				return this.ReadInt16 (38);
			}
		}
		
		public uint		XMax
		{
			get
			{
				return this.ReadInt16 (40);
			}
		}
		
		public uint		YMax
		{
			get
			{
				return this.ReadInt16 (42);
			}
		}
		
		public uint		MacStyles
		{
			get
			{
				return this.ReadInt16 (44);
			}
		}
		
		public uint		LowestRecPpEm
		{
			get
			{
				return this.ReadInt16 (46);
			}
		}
		
		public uint		FontDirectionHint
		{
			get
			{
				return this.ReadInt16 (48);
			}
		}
		
		public uint		IndexToLocFormat
		{
			get
			{
				return this.ReadInt16 (50);
			}
		}
		
		public uint		GlyphDataFormat
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
		
		
		public uint		NumContours
		{
			get
			{
				return this.ReadInt16 (0);
			}
		}
		
		public uint		XMin
		{
			get
			{
				return this.ReadInt16 (2);
			}
		}
		
		public uint		YMin
		{
			get
			{
				return this.ReadInt16 (4);
			}
		}
		
		public uint		XMax
		{
			get
			{
				return this.ReadInt16 (6);
			}
		}
		
		public uint		YMax
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
		
		
		public uint GetSimpleEndPtsOfContour(int n)
		{
			return this.ReadInt16 (10+n*2);
		}
		
		public uint	GetSimpleInstructionLength()
		{
			return this.ReadInt16 ((int)(10+2*this.NumContours+0));
		}
		
		public uint GetSimpleInstruction(int n)
		{
			return this.ReadInt8 ((int)(10+2*this.NumContours+2+n));
		}
		
		public uint GetSimpleFlag(int n)
		{
			return this.ReadInt8 ((int)(10+2*this.NumContours+2+this.GetSimpleInstructionLength ()+n));
		}
		
		
		public uint		CompositeFlags
		{
			get
			{
				return this.ReadInt16 (10);
			}
		}
		
		public uint		CompositeGlyphIndex
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
		
		
		public uint GetOffset(int n)
		{
			return this.ReadInt16 (n*2);
		}
	}
	
	public class Table_loca_Long : Tables
	{
		public Table_loca_Long(byte[] data, int offset) : base (data, offset)
		{
		}
		
		
		public uint GetOffset(int n)
		{
			return this.ReadInt32 (n*4);
		}
	}
	
	public class Table_maxp : Tables
	{
		public Table_maxp(byte[] data, int offset) : base (data, offset)
		{
		}
		
		
		public uint		TableVersion
		{
			get
			{
				return this.ReadInt32 (0);
			}
		}
		
		public uint		NumGlyphs
		{
			get
			{
				return this.ReadInt16 (4);
			}
		}
		
		public uint		MaxPoints
		{
			get
			{
				return this.ReadInt16 (6);
			}
		}
		
		public uint		MaxContours
		{
			get
			{
				return this.ReadInt16 (8);
			}
		}
		
		public uint		MaxCompositePoints
		{
			get
			{
				return this.ReadInt16 (10);
			}
		}
		
		public uint		MaxCompositeContours
		{
			get
			{
				return this.ReadInt16 (12);
			}
		}
		
		public uint		MaxZones
		{
			get
			{
				return this.ReadInt16 (14);
			}
		}
		
		public uint		MaxTwilightPoints
		{
			get
			{
				return this.ReadInt16 (16);
			}
		}
		
		public uint		MaxStorage
		{
			get
			{
				return this.ReadInt16 (18);
			}
		}
		
		public uint		MaxFunctionDefs
		{
			get
			{
				return this.ReadInt16 (20);
			}
		}
		
		public uint		MaxInstructionDefs
		{
			get
			{
				return this.ReadInt16 (22);
			}
		}
		
		public uint		MaxStackElements
		{
			get
			{
				return this.ReadInt16 (24);
			}
		}
		
		public uint		MaxSizeOfInstructions
		{
			get
			{
				return this.ReadInt16 (26);
			}
		}
		
		public uint		MaxComponentElements
		{
			get
			{
				return this.ReadInt16 (28);
			}
		}
		
		public uint		MaxComponentDepth
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
		
		
		public uint		TableVersion
		{
			get
			{
				return this.ReadInt16 (0);
			}
		}
		
		public uint		NumEncodingTables
		{
			get
			{
				return this.ReadInt16 (2);
			}
		}
		
		
		public uint GetSubtablePlatformId(int n)
		{
			return this.ReadInt16 (4+n*8+0);
		}
		
		public uint GetSubtableEncodingId(int n)
		{
			return this.ReadInt16 (4+n*8+2);
		}
		
		public uint GetSubtableOffset(int n)
		{
			return this.ReadInt32 (4+n*8+4);
		}
		
		public IndexMappingTable GetGenericSubtable(int n)
		{
			return new IndexMappingTable (this.data, this.offset + (int) this.GetSubtableOffset (n));
		}
		
		
		public IndexMappingTable FindFormatSubtable(uint platform, uint encoding, uint format)
		{
			int n = (int) this.NumEncodingTables;
			
			for (int i = 0; i < n; i++)
			{
				if ((this.GetSubtablePlatformId (i) == platform) &&
					(this.GetSubtableEncodingId (i) == encoding))
				{
					IndexMappingTable fmt = this.GetGenericSubtable (i);
					
					if (fmt.Format == format)
					{
						switch (fmt.Format)
						{
							case 0:  return new IndexMappingTable0 (this.data, this.offset + (int) this.GetSubtableOffset (i));
							case 4:  return new IndexMappingTable4 (this.data, this.offset + (int) this.GetSubtableOffset (i));
							case 12: return new IndexMappingTable12 (this.data, this.offset + (int) this.GetSubtableOffset (i));
						}
						
						return null;
					}
				}
			}
			
			return null;
		}
		
		public IndexMappingTable FindFormatSubtable()
		{
			IndexMappingTable sub;
			
			sub = this.FindFormatSubtable (3, 1, 12);
			
			if (sub == null)
			{
				sub = this.FindFormatSubtable (3, 1, 4);
			}
			
			return sub;
		}
	}
	
	public class IndexMappingTable : Tables
	{
		public IndexMappingTable(byte[] data, int offset) : base (data, offset)
		{
		}
		
		
		public uint		Format
		{
			get
			{
				return this.ReadInt16 (0);
			}
		}
		
		public uint		Length
		{
			get
			{
				return this.ReadInt16 (2);
			}
		}
		
		
		public virtual uint GetGlyphIndex(int n)
		{
			throw new System.NotSupportedException ();
		}
	}
	
	public class IndexMappingTable0 : IndexMappingTable
	{
		public IndexMappingTable0(byte[] data, int offset) : base (data, offset)
		{
		}
		
		
		public uint		Language
		{
			get
			{
				return this.ReadInt16 (4);
			}
		}
		
		
		public override uint GetGlyphIndex(int n)
		{
			return this.ReadInt8 (6+n);
		}
	}
	
	public class IndexMappingTable4 : IndexMappingTable
	{
		public IndexMappingTable4(byte[] data, int offset) : base (data, offset)
		{
		}
		
		
		public uint		Language
		{
			get
			{
				return this.ReadInt16 (4);
			}
		}
		
		public uint		SegCountX2
		{
			get
			{
				return this.ReadInt16 (6);
			}
		}
		
		public uint		SearchRange
		{
			get
			{
				return this.ReadInt16 (8);
			}
		}
		
		public uint		EntrySelector
		{
			get
			{
				return this.ReadInt16 (10);
			}
		}
		
		public uint		RangeShift
		{
			get
			{
				return this.ReadInt16 (12);
			}
		}
		
		
		public override uint GetGlyphIndex(int n)
		{
			uint code = (uint) n;
			
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
					uint start = this.ReadInt16 (o_start_code + i);
					
					if (start <= code)
					{
						uint id_range_offset = this.ReadInt16 (o_id_range_o + i);
						uint index = 0;
						
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
						
						return index & 0xffff;
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
		
		
		public uint		LongLength
		{
			get
			{
				return this.ReadInt32 (4);
			}
		}
		
		public uint		Language
		{
			get
			{
				return this.ReadInt32 (8);
			}
		}
		
		public uint		NumGroups
		{
			get
			{
				return this.ReadInt16 (12);
			}
		}
		
		
		public override uint GetGlyphIndex(int n)
		{
			uint code = (uint) n;
			
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
						return code - this.ReadInt32 (12*i + o_start_char_code)
							/**/    + this.ReadInt32 (12*i + o_start_glyph_id);
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
		
		
		public uint		FormatSelector
		{
			get
			{
				return this.ReadInt16 (0);
			}
		}
		
		public uint		NumNameRecords
		{
			get
			{
				return this.ReadInt16 (2);
			}
		}
		
		public uint		StorageAreaOffset
		{
			get
			{
				return this.ReadInt16 (4);
			}
		}
		
		
		public struct NameEncoding
		{
			public PlatformId		Platform;
			public uint				Encoding;
			public uint				Language;
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
		
		public string GetLatinName(uint language, NameId name, PlatformId platform)
		{
			int num = (int) this.NumNameRecords;
			
			uint lang_id = (uint) language;
			uint name_id = (uint) name;
			uint plat_id = (uint) platform;
			
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
		
		public string GetUnicodeName(uint language, NameId name, PlatformId platform)
		{
			int num = (int) this.NumNameRecords;
			
			uint lang_id = (uint) language;
			uint name_id = (uint) name;
			uint plat_id = (uint) platform;
			
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
		
		
		public uint		TableVersion
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
		
		public uint		AdvanceWidthMax
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
		
		public uint		CaretSlopeRise
		{
			get
			{
				return this.ReadInt16 (18);
			}
		}
		
		public uint		CaretSlopeRun
		{
			get
			{
				return this.ReadInt16 (20);
			}
		}
		
		public uint		CaretOffset
		{
			get
			{
				return this.ReadInt16 (22);
			}
		}
		
		public uint		MetricDataFormat
		{
			get
			{
				return this.ReadInt16 (32);
			}
		}
		
		public uint		NumHMetrics
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
		
		
		public uint GetAdvanceWidth(uint n)
		{
			return this.ReadInt16 ((int)(n*4+0));
		}
		
		public int GetLeftSideBearing(uint n)
		{
			return (short) this.ReadInt16 ((int)(n*4+2));
		}
		
		public int GetExtraLeftSideBearing(uint number_h_metrics, uint n)
		{
			return (short) this.ReadInt16 ((int)(number_h_metrics*4 + n*2));
		}
	}
	
	public class Table_GDEF : Tables
	{
		//	http://partners.adobe.com/public/developer/opentype/index_table_formats5.html
		
		public Table_GDEF(byte[] data, int offset) : base (data, offset)
		{
		}
		
		
		public uint		TableVersion
		{
			get
			{
				return this.ReadInt32 (0);
			}
		}
		
		public uint		GlyphClassDefOffset
		{
			get
			{
				return this.ReadInt16 (4);
			}
		}
		
		public uint		AttachListOffset
		{
			get
			{
				return this.ReadInt16 (6);
			}
		}
		
		public uint		LigCaretListOffset
		{
			get
			{
				return this.ReadInt16 (8);
			}
		}
		
		public uint		MarkAttachClassDefOffset
		{
			get
			{
				return this.ReadInt16 (10);
			}
		}
	}
	
	public class Table_GSUB
	{
	}
}
