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
		
		public uint		Offset
		{
			get
			{
				return this.ReadInt32 (4);
			}
		}
		
		public uint		Length
		{
			get
			{
				return this.ReadInt32 (8);
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
		
		
		public TableEntry GetEntry(int n)
		{
			return new TableEntry (this.data, this.offset + 16*n + 10);
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
	
	public class TableHead : Tables
	{
		public TableHead(byte[] data, int offset) : base (data, offset)
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
	
	public class TableGlyf : Tables
	{
		public TableGlyf(byte[] data, int offset) : base (data, offset)
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
	
	public class TableLocaShort : Tables
	{
		public TableLocaShort(byte[] data, int offset) : base (data, offset)
		{
		}
		
		
		public uint GetOffset(int n)
		{
			return this.ReadInt16 (n*2);
		}
	}
	
	public class TableLocaLong : Tables
	{
		public TableLocaLong(byte[] data, int offset) : base (data, offset)
		{
		}
		
		
		public uint GetOffset(int n)
		{
			return this.ReadInt32 (n*4);
		}
	}
	
	public class TableMaxp : Tables
	{
		public TableMaxp(byte[] data, int offset) : base (data, offset)
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
	
	public class TableCMap : Tables
	{
		//	http://partners.adobe.com/public/developer/opentype/index_cmap.html
		
		public TableCMap(byte[] data, int offset) : base (data, offset)
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
		
		public CMapFormat GetGenericSubtable(int n)
		{
			return new CMapFormat (this.data, this.offset + (int) this.GetSubtableOffset (n));
		}
	}
	
	public class CMapFormat : Tables
	{
		public CMapFormat(byte[] data, int offset) : base (data, offset)
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
	
	public class CMapFormat0 : CMapFormat
	{
		public CMapFormat0(byte[] data, int offset) : base (data, offset)
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
	
	public class CMapFormat4 : CMapFormat
	{
		public CMapFormat4(byte[] data, int offset) : base (data, offset)
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
	
	public class CMapFormat12 : CMapFormat
	{
		public CMapFormat12(byte[] data, int offset) : base (data, offset)
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
}
