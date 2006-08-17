//	Copyright © 2005-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.OpenType
{
	//	Data types:     http://partners.adobe.com/public/developer/opentype/index_font_file.html
	//	List of tables: http://partners.adobe.com/public/developer/opentype/index_tables.html
	
	/// <summary>
	/// <c>Tables</c> is the base class used to access all OpenType tables.
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
	
	public class Table_ttcf : Tables
	{
		public Table_ttcf(byte[] data, int offset) : base (data, offset)
		{
		}
		
		
		public int		Version
		{
			get
			{
				return this.ReadInt32 (4);
			}
		}
		
		public int		NumFonts
		{
			get
			{
				return this.ReadInt32 (8);
			}
		}
		
		
		public int GetFontOffset(int n)
		{
			return this.ReadInt32 (12 + 4*n);
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
				return (short) this.ReadInt16 (36);
			}
		}
		
		public int		YMin
		{
			get
			{
				return (short) this.ReadInt16 (38);
			}
		}
		
		public int		XMax
		{
			get
			{
				return (short) this.ReadInt16 (40);
			}
		}
		
		public int		YMax
		{
			get
			{
				return (short) this.ReadInt16 (42);
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
				return (short) this.ReadInt16 (2);
			}
		}
		
		public int		YMin
		{
			get
			{
				return (short) this.ReadInt16 (4);
			}
		}
		
		public int		XMax
		{
			get
			{
				return (short) this.ReadInt16 (6);
			}
		}
		
		public int		YMax
		{
			get
			{
				return (short) this.ReadInt16 (8);
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
	
	public class Table_post : Tables
	{
		public Table_post(byte[] data, int offset) : base (data, offset)
		{
		}
		
		public Table_post(TableEntry entry) : base (entry.BaseData, entry.Offset)
		{
		}
		
		
		public int		Version
		{
			get
			{
				return this.ReadInt32 (0);
			}
		}
		
		public int		UnderlinePosition
		{
			get
			{
				return this.ReadInt16 (4+4+0);
			}
		}
		
		public int		UnderlineThickness
		{
			get
			{
				return this.ReadInt16 (4+4+2);
			}
		}
		
		public bool		IsFixedPitch
		{
			get
			{
				return this.ReadInt32 (4+4+4) != 0;
			}
		}
		
		public int		NumberOfGlyphs
		{
			get
			{
				if (this.Version == 0x00020000)
				{
					return this.ReadInt16 (32);
				}
				else
				{
					return 0;
				}
			}
		}
		
		
		public int GetGlyphNameIndex(int glyph)
		{
			if (this.Version == 0x00020000)
			{
				if ((glyph < 0) ||
					(glyph >= this.NumberOfGlyphs))
				{
					return -1;
				}
				else
				{
					return this.ReadInt16 (32+2+2*glyph);
				}
			}
			else
			{
				return glyph;
			}
		}
		
		public string GetGlyphName(int glyph)
		{
			if (this.Version == 0x00010000)
			{
				return Table_post.GetMacGlyphName (glyph);
			}
			else if (this.Version != 0x00020000)
			{
				return null;
			}
			
			int index = this.GetGlyphNameIndex (glyph);
			
			if (index < 1)
			{
				return ".notdef";
			}
			
			if (index > 257)
			{
				index -= 258;
				
				int offset = 32+2+2*this.NumberOfGlyphs;
				
				while (index > 0)
				{
					offset += this.ReadInt8 (offset) + 1;
					index--;
				}
				
				System.Text.StringBuilder buffer = new System.Text.StringBuilder ();
				
				int len = this.ReadInt8 (offset++);
				
				for (int i = 0; i < len; i++)
				{
					buffer.Append ((char) this.ReadInt8 (offset+i));
				}
				
				return buffer.ToString ();
			}
			
			return Table_post.GetMacGlyphName (index);
		}
		
		
		static private string GetMacGlyphName(int index)
		{
			//	http://developer.apple.com/fonts/TTRefMan/RM06/Chap6post.html
			
			switch (index)
			{
				case 1: return ".null";
				case 2: return "nonmarkingreturn";
				case 3: return "space";
				case 4: return "exclam";
				case 5: return "quotedbl";
				case 6: return "numbersign";
				case 7: return "dollar";
				case 8: return "percent";
				case 9: return "ampersand";
				case 10: return "quotesingle";
				case 11: return "parenleft";
				case 12: return "parenright";
				case 13: return "asterisk";
				case 14: return "plus";
				case 15: return "comma";
				case 16: return "hyphen";
				case 17: return "period";
				case 18: return "slash";
				case 19: return "zero";
				case 20: return "one";
				case 21: return "two";
				case 22: return "three";
				case 23: return "four";
				case 24: return "five";
				case 25: return "six";
				case 26: return "seven";
				case 27: return "eight";
				case 28: return "nine";
				case 29: return "colon";
				case 30: return "semicolon";
				case 31: return "less";
				case 32: return "equal";
				case 33: return "greater";
				case 34: return "question";
				case 35: return "at";
				case 36: return "A";
				case 37: return "B";
				case 38: return "C";
				case 39: return "D";
				case 40: return "E";
				case 41: return "F";
				case 42: return "G";
				case 43: return "H";
				case 44: return "I";
				case 45: return "J";
				case 46: return "K";
				case 47: return "L";
				case 48: return "M";
				case 49: return "N";
				case 50: return "O";
				case 51: return "P";
				case 52: return "Q";
				case 53: return "R";
				case 54: return "S";
				case 55: return "T";
				case 56: return "U";
				case 57: return "V";
				case 58: return "W";
				case 59: return "X";
				case 60: return "Y";
				case 61: return "Z";
				case 62: return "bracketleft";
				case 63: return "backslash";
				case 64: return "bracketright";
				case 65: return "asciicircum";
				case 66: return "underscore";
				case 67: return "grave";
				case 68: return "a";
				case 69: return "b";
				case 70: return "c";
				case 71: return "d";
				case 72: return "e";
				case 73: return "f";
				case 74: return "g";
				case 75: return "h";
				case 76: return "i";
				case 77: return "j";
				case 78: return "k";
				case 79: return "l";
				case 80: return "m";
				case 81: return "n";
				case 82: return "o";
				case 83: return "p";
				case 84: return "q";
				case 85: return "r";
				case 86: return "s";
				case 87: return "t";
				case 88: return "u";
				case 89: return "v";
				case 90: return "w";
				case 91: return "x";
				case 92: return "y";
				case 93: return "z";
				case 94: return "braceleft";
				case 95: return "bar";
				case 96: return "braceright";
				case 97: return "asciitilde";
				case 98: return "Adieresis";
				case 99: return "Aring";
				case 100: return "Ccedilla";
				case 101: return "Eacute";
				case 102: return "Ntilde";
				case 103: return "Odieresis";
				case 104: return "Udieresis";
				case 105: return "aacute";
				case 106: return "agrave";
				case 107: return "acircumflex";
				case 108: return "adieresis";
				case 109: return "atilde";
				case 110: return "aring";
				case 111: return "ccedilla";
				case 112: return "eacute";
				case 113: return "egrave";
				case 114: return "ecircumflex";
				case 115: return "edieresis";
				case 116: return "iacute";
				case 117: return "igrave";
				case 118: return "icircumflex";
				case 119: return "idieresis";
				case 120: return "ntilde";
				case 121: return "oacute";
				case 122: return "ograve";
				case 123: return "ocircumflex";
				case 124: return "odieresis";
				case 125: return "otilde";
				case 126: return "uacute";
				case 127: return "ugrave";
				case 128: return "ucircumflex";
				case 129: return "udieresis";
				case 130: return "dagger";
				case 131: return "degree";
				case 132: return "cent";
				case 133: return "sterling";
				case 134: return "section";
				case 135: return "bullet";
				case 136: return "paragraph";
				case 137: return "germandbls";
				case 138: return "registered";
				case 139: return "copyright";
				case 140: return "trademark";
				case 141: return "acute";
				case 142: return "dieresis";
				case 143: return "notequal";
				case 144: return "AE";
				case 145: return "Oslash";
				case 146: return "infinity";
				case 147: return "plusminus";
				case 148: return "lessequal";
				case 149: return "greaterequal";
				case 150: return "yen";
				case 151: return "mu";
				case 152: return "partialdiff";
				case 153: return "summation";
				case 154: return "product";
				case 155: return "pi";
				case 156: return "integral";
				case 157: return "ordfeminine";
				case 158: return "ordmasculine";
				case 159: return "Omega";
				case 160: return "ae";
				case 161: return "oslash";
				case 162: return "questiondown";
				case 163: return "exclamdown";
				case 164: return "logicalnot";
				case 165: return "radical";
				case 166: return "florin";
				case 167: return "approxequal";
				case 168: return "Delta";
				case 169: return "guillemotleft";
				case 170: return "guillemotright";
				case 171: return "ellipsis";
				case 172: return "nonbreakingspace";
				case 173: return "Agrave";
				case 174: return "Atilde";
				case 175: return "Otilde";
				case 176: return "OE";
				case 177: return "oe";
				case 178: return "endash";
				case 179: return "emdash";
				case 180: return "quotedblleft";
				case 181: return "quotedblright";
				case 182: return "quoteleft";
				case 183: return "quoteright";
				case 184: return "divide";
				case 185: return "lozenge";
				case 186: return "ydieresis";
				case 187: return "Ydieresis";
				case 188: return "fraction";
				case 189: return "currency";
				case 190: return "guilsinglleft";
				case 191: return "guilsinglright";
				case 192: return "fi";
				case 193: return "fl";
				case 194: return "daggerdbl";
				case 195: return "periodcentered";
				case 196: return "quotesinglbase";
				case 197: return "quotedblbase";
				case 198: return "perthousand";
				case 199: return "Acircumflex";
				case 200: return "Ecircumflex";
				case 201: return "Aacute";
				case 202: return "Edieresis";
				case 203: return "Egrave";
				case 204: return "Iacute";
				case 205: return "Icircumflex";
				case 206: return "Idieresis";
				case 207: return "Igrave";
				case 208: return "Oacute";
				case 209: return "Ocircumflex";
				case 210: return "apple";
				case 211: return "Ograve";
				case 212: return "Uacute";
				case 213: return "Ucircumflex";
				case 214: return "Ugrave";
				case 215: return "dotlessi";
				case 216: return "circumflex";
				case 217: return "tilde";
				case 218: return "macron";
				case 219: return "breve";
				case 220: return "dotaccent";
				case 221: return "ring";
				case 222: return "cedilla";
				case 223: return "hungarumlaut";
				case 224: return "ogonek";
				case 225: return "caron";
				case 226: return "Lslash";
				case 227: return "lslash";
				case 228: return "Scaron";
				case 229: return "scaron";
				case 230: return "Zcaron";
				case 231: return "zcaron";
				case 232: return "brokenbar";
				case 233: return "Eth";
				case 234: return "eth";
				case 235: return "Yacute";
				case 236: return "yacute";
				case 237: return "Thorn";
				case 238: return "thorn";
				case 239: return "minus";
				case 240: return "multiply";
				case 241: return "onesuperior";
				case 242: return "twosuperior";
				case 243: return "threesuperior";
				case 244: return "onehalf";
				case 245: return "onequarter";
				case 246: return "threequarters";
				case 247: return "franc";
				case 248: return "Gbreve";
				case 249: return "gbreve";
				case 250: return "Idotaccent";
				case 251: return "Scedilla";
				case 252: return "scedilla";
				case 253: return "Cacute";
				case 254: return "cacute";
				case 255: return "Ccaron";
				case 256: return "ccaron";
				case 257: return "dcroat";
				default: return ".notdef";
			}
		}
	}
	
	public class Table_loca_Short : Tables
	{
		public Table_loca_Short(byte[] data, int offset) : base (data, offset)
		{
		}
		
		public Table_loca_Short(TableEntry entry) : base (entry.BaseData, entry.Offset)
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
		
		public Table_loca_Long(TableEntry entry) : base (entry.BaseData, entry.Offset)
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
		public Table_cmap(byte[] data, int offset) : base (data, offset)
		{
			//	http://partners.adobe.com/public/developer/opentype/index_cmap.html
			
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
			if (sub == null)
			{
				sub = this.FindFormatSubTable (3, 0, 4);
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
			if (n == 0)
			{
				return 0xffff;
			}
			else
			{
				return (ushort) this.ReadInt8 (6+n);
			}
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
			if (n == 0)
			{
				return 0xffff;
			}
			
			int code = (int) n;
			
			int max = (int) this.SegCountX2;
			
			int o_end_code   = 14;
			int o_start_code = o_end_code + max + 2;
			int o_id_delta   = o_start_code + max;
			int o_id_range_o = o_id_delta + max;
			
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
			if (n == 0)
			{
				return 0xffff;
			}
			
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
//			int o_encoding_id   = 8;
			int o_language_id   = 10;
			int o_name_id       = 12;
			int o_string_length = 14;
			int o_string_offset = 16;
			
			for (int i = 0; i < num; i++)
			{
				if ((this.ReadInt16 (i*12 + o_platform_id) == plat_id) &&
/*					(this.ReadInt16 (i*12 + o_encoding_id) == 0) && */
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
//			int o_encoding_id   = 8;
			int o_language_id   = 10;
			int o_name_id       = 12;
			int o_string_length = 14;
			int o_string_offset = 16;
			
			for (int i = 0; i < num; i++)
			{
				if ((this.ReadInt16 (i*12 + o_platform_id) == plat_id) &&
/*					((this.ReadInt16 (i*12 + o_encoding_id) == 1) || (plat_id == 0)) && */
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
		PreferredFamily,		//	Arial (for "Arial Narrow", "Arial" and "Arial Black")
		PreferredSubfamily,		//	Black Italic (for "Arial Black Italic")
		Mac_CompatibleFull,
		SampleText,				//	"The quick brown fox..."
		PostScriptCID,
	}
	#endregion
	
	public class Table_hhea : Tables
	{
		public Table_hhea(byte[] data, int offset) : base (data, offset)
		{
			//	http://partners.adobe.com/public/developer/opentype/index_hhea.html
			
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
				return (short) this.ReadInt16 (10);
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
				return (short) this.ReadInt16 (18);
			}
		}
		
		public int		CaretSlopeRun
		{
			get
			{
				return (short) this.ReadInt16 (20);
			}
		}
		
		public int		CaretOffset
		{
			get
			{
				return (short) this.ReadInt16 (22);
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
		public Table_hmtx(byte[] data, int offset) : base (data, offset)
		{
			//	http://partners.adobe.com/public/developer/opentype/index_hmtx.html
			
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
		
		
		public static Table_kern Create(TableEntry entry)
		{
			if (entry == null)
			{
				return null;
			}
			
			return new Table_kern (entry);
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
			return (short) this.ReadInt16 (18+n*6);
		}
		
		public bool FindKernValue(int left, int right, out int value)
		{
			uint combined = (uint)(left << 16) | (uint)(right & 0xffff);
			
			int range = this.SearchRange;
			int index = this.EntrySelector;
			int count = this.PairCount;
			int pow_2 = range;
			int fence = count * 6;
			
			if (range == fence)
			{
				//	The OpenType specification says that 'range' is defined as :
				//
				//		"The largest power of two less than or equal to the value of nPairs,
				//		multiplied by the size in bytes of an entry in the table".
				//
				//	http://www.microsoft.com/OpenType/OTSpec/kern.htm
				//
				//	In case nPairs (= PairCount) is a power of two itself, then we won't
				//	start at the right place with our bisection; adjust the position in
				//	order to start right in the middle :
				
				range = ((range / 2) / 6) * 6;
			}
			
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
		public Table_GDEF(byte[] data, int offset) : base (data, offset)
		{
			//	http://partners.adobe.com/public/developer/opentype/index_table_formats5.html
			//	http://www.microsoft.com/OpenType/OTSpec/gdef.htm
			
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
		
		
		public static Table_GDEF Create(TableEntry entry)
		{
			if (entry == null)
			{
				return null;
			}
			
			return new Table_GDEF (entry);
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
		public Table_GSUB(byte[] data, int offset) : base (data, offset)
		{
			//	See http://www.microsoft.com/OpenType/OTSpec/gsub.htm or
			//	http://partners.adobe.com/public/developer/opentype/index_table_formats1.html
	
			//	See http://www.microsoft.com/OpenType/OTSpec/chapter2.htm for the common table format
			//	See http://www.microsoft.com/OpenType/OTSpec/featurelist.htm for 'liga' and others.
			
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
		
		
		public static Table_GSUB Create(TableEntry entry)
		{
			if (entry == null)
			{
				return null;
			}
			
			return new Table_GSUB (entry);
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
		
		
		public LookupType	LookupType
		{
			get
			{
				return (LookupType) this.ReadInt16 (0);
			}
		}
		
		public int			LookupFlags
		{
			get
			{
				return this.ReadInt16 (2);
			}
		}
		
		public int			SubTableCount
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
				case LookupType.Single:				return new SingleSubstitution (this.data, offset);
				case LookupType.Alternate:			return new AlternateSubstitution (this.data, offset);
				case LookupType.Ligature:			return new LigatureSubstitution (this.data, offset);
				case LookupType.ChainingContext:	return new ChainingContextSubstitution (this.data, offset);
				
				case LookupType.Context:
					
					//	TODO: ...
					
					return null;
				
				case LookupType.ExtensionSubstitution:
					
					//	TODO: ...
					
					return null;
				
				case LookupType.Multiple:
					
					//	TODO: ...
					
					return null;
				
				default:
					throw new System.NotSupportedException (string.Format ("LookupType {0} not supported.", this.LookupType));
			}
		}
	}
	
	public enum LookupType
	{
		Single							= 1,
		Multiple						= 2,
		Alternate						= 3,
		Ligature						= 4,
		Context							= 5,
		ChainingContext					= 6,
		ExtensionSubstitution			= 7,
		ReverseChainingContextSingle	= 8,
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
			int cov = this.Coverage.FindIndex (glyph);
			
			if (cov >= 0)
			{
				return (ushort) (this.ReadInt16 (4) + glyph);
			}
			
			return (ushort) (0xffff);
		}
		
		private ushort FindSubstitutionFmt2(int glyph)
		{
			int max = this.ReadInt16 (4);
			int cov = this.Coverage.FindIndex (glyph);
			
			if ((cov >= 0) &&
				(cov < max))
			{
				return (ushort) (this.ReadInt16 ((int)(6+2*cov)));
			}
			
			return (ushort) (0xffff);
		}
	}
	
	public class AlternateSubstitution : BaseSubstitution
	{
		public AlternateSubstitution(SubstSubTable sub) : base (sub.BaseData, sub.BaseOffset)
		{
			//	http://partners.adobe.com/public/developer/opentype/index_table_formats1.html#ASF1
			
		}
		
		public AlternateSubstitution(byte[] data, int offset) : base (data, offset)
		{
		}
		
		
		public int			AlternateSetCount
		{
			get
			{
				return this.ReadInt16 (4);
			}
		}
		
		public int GetAlternateSetOffset(int i)
		{
			return this.ReadInt16 (6+2*i);
		}
		
		public ushort[] GetAlternates(ushort glyph)
		{
			int cov = this.Coverage.FindIndex (glyph);
			int max = this.AlternateSetCount;
			
			if ((cov >= 0) &&
				(cov < max))
			{
				int o = this.GetAlternateSetOffset (cov);
				int n = this.ReadInt16 (o);
				
				ushort[] alts = new ushort[n];
				
				for (int i = 0; i < n; i++)
				{
					alts[i] = (ushort) this.ReadInt16 (o + 2 + 2*i);
				}
				
				return alts;
			}
			
			return null;
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
