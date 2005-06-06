//	Copyright � 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text
{
	/// <summary>
	/// La classe Unicode encapsule certaines informations au sujet des
	/// caract�res cod�s en UTF-32.
	/// </summary>
	public sealed class Unicode
	{
		private Unicode()
		{
		}
		
		
		#region Bits Class
		public class Bits
		{
			public const ulong	CodeMask		= 0x001fffff;
			public const ulong	FullCodeMask	= 0x00ffffff;
			
			public const ulong	CombiningFlag	= 0x00200000;
			public const ulong	ReorderingFlag	= 0x00400000;
			public const ulong	SpecialCodeFlag	= 0x00800000;
			
			public const ulong	InfoMask		= 0x07000000;
			public const int	InfoShift		= 24;
			
			
			public static int  GetCode(ulong value)
			{
				return (int) (value & Bits.CodeMask);
			}
			
			public static void SetCode(ref ulong value, int code)
			{
				if ((code < 0) || (code > 0x0010FFFF))
				{
					throw new Unicode.IllegalCodeException ();
				}
				
				value = (value & ~Bits.CodeMask) | (uint) code;
			}
			
			
			public static bool GetCombiningFlag(ulong value)
			{
				return (value & Bits.CombiningFlag) != 0;
			}
			
			public static void SetCombiningFlag(ref ulong value, bool flag)
			{
				if (flag)
				{
					value = value | Bits.CombiningFlag;
				}
				else
				{
					value = value & ~Bits.CombiningFlag;
				}
			}
			
			
			public static bool GetReorderingFlag(ulong value)
			{
				return (value & Bits.ReorderingFlag) != 0;
			}
			
			public static void SetReorderingFlag(ref ulong value, bool flag)
			{
				if (flag)
				{
					value = value | Bits.ReorderingFlag;
				}
				else
				{
					value = value & ~Bits.ReorderingFlag;
				}
			}
			
			
			public static bool GetSpecialCodeFlag(ulong value)
			{
				return (value & Bits.SpecialCodeFlag) != 0;
			}
			
			public static void SetSpecialCodeFlag(ref ulong value, bool flag)
			{
				if (flag)
				{
					value = value | Bits.SpecialCodeFlag;
				}
				else
				{
					value = value & ~Bits.SpecialCodeFlag;
				}
			}
			
			
			public static Unicode.BreakInfo GetBreakInfo(ulong value)
			{
				return (Unicode.BreakInfo) ((value & Bits.InfoMask) >> Bits.InfoShift);
			}
			
			public static void              SetBreakInfo(ref ulong value, Unicode.BreakInfo info)
			{
				value = (value & ~Bits.InfoMask) | (((ulong) info) << Bits.InfoShift);
			}
			
			public static void SetBreakInfo(ulong[] text, int offset, Unicode.BreakInfo[] breaks)
			{
				for (int i = 0; i < breaks.Length; i++)
				{
					Unicode.Bits.SetBreakInfo (ref text[offset+i], breaks[i]);
				}
			}
		}
		#endregion
		
		#region IllegalCodeException Class
		public class IllegalCodeException : System.ApplicationException
		{
			public IllegalCodeException()
			{
			}
			
			public IllegalCodeException(string message) : base (message)
			{
			}
		}
		#endregion
		
		#region Surrogate Constants
		public const char	SurrogateLowMin		= (char) 0xD800;
		public const char	SurrogateLowMax		= (char) 0xDBFF;
		public const char	SurrogateHighMin	= (char) 0xDC00;
		public const char	SurrogateHighMax	= (char) 0xDFFF;
		
		public const char	SurrogateMin		= SurrogateLowMin;
		public const char	SurrogateMax		= SurrogateHighMax;
		#endregion
		
		#region Code, BreakInfo and BreakClass Enumerations
		public enum Code
		{
			StartOfText				= 0x0002,		//	start of inserted text
			EndOfText				= 0x0003,		//	end of inserted text
			StartOfGuardedArea		= 0x0096,
			EndOfGuardedArea		= 0x0097,
			
			HorizontalTab			= 0x0009,
			
			Space					= 0x0020,
			QuotationMark			= 0x0022,
			PercentSign				= 0x0025,
			Ampersand				= 0x0026,
			LessThanSign			= 0x003C,
			GreaterThanSign			= 0x003E,
			
			LineFeed				= 0x000A,		//	don't use internally (LF)
			VerticalTab				= 0x000B,		//	don't use internally (VT)
			FormFeed				= 0x000C,		//	- renamed PageSeparator
			CarriageReturn			= 0x000D,		//	don't use internally (CR)
			NextLine				= 0x0085,		//	don't use internally (NEL)
			
			//	Arabic:

			Tatweel					= 0x0640,		//	kashida
			
			//	Spaces:
			
			EnQuad					= 0x2000,
			EmQuad					= 0x2001,
			EnSpace					= 0x2002,
			EmSpace					= 0x2003,
			ThreePerEmSpace			= 0x2004,		//	1/3 em
			FourPerEmSpace			= 0x2005,		//	1/4 em
			SixPerEmSpace			= 0x2006,		//	1/6 em
			FigureSpace				= 0x2007,		//	non-breaking, same width as a digit
			PunctuationSpace		= 0x2008,
			ThinSpace				= 0x2009,		//	1/5 em
			HairSpace				= 0x200A,
			ZeroWidthSpace			= 0x200B,		//	may expand in justification
			ZeroWidthNonJoiner		= 0x200C,
			ZeroWidthJoiner			= 0x200D,
			
			Ellipsis				= 0x2026,		//	...
			
			MediumMathSpace			= 0x205F,		//	4/18 em
			WordJoiner				= 0x2060,		//	same as ZeroWidthSpace, not breaking
			
			FunctionApplication		= 0x2061,		//	math: contiguity operator
			InvisibleTimes			= 0x2062,		//	math: contiguity operator
			InvisibleSeparator		= 0x2063,		//	math: contiguity operator (invisible comma)
			
			NoBreakSpace			= 0x00A0,
			NarrowNoBreakSpace		= 0x202F,

			
			//	Formatting characters:
			
			LeftToRightMark			= 0x200E,
			RightToLeftMark			= 0x200F,
			
			LeftToRightEmbedding	= 0x202A,
			RightToLeftEmbedding	= 0x202B,
			PopDirectionalFormatting= 0x202C,
			LeftToRightOverride		= 0x202D,
			RightToLeftOverride		= 0x202E,
			
			PageSeparator			= FormFeed,
			
			LineSeparator			= 0x2028,
			ParagraphSeparator		= 0x2029,
			
			//	Dashes:
			
			HyphenMinus				= 0x002D,
			SoftHyphen				= 0x00AD,
			ArmenianHyphen			= 0x058A,		//	same as SoftHyphen, but with different shape
			MongolianTodoHyphen		= 0x1806,		//	same as SoftHyphen, but at beginning of next line
			
			Hyphen					= 0x2010,
			NonBreakingHyphen		= 0x2011,
			FigureDash				= 0x2012,		//	non-breaking, same width as a digit
			EnDash					= 0x2013,
			EmDash					= 0x2014,
			QuotationDash			= 0x2015,
			
			//	Special:
			
			ObjectReplacement		= 0xFFFC,
			
			Invalid					= 0xFFFF,
		}
		
		public enum BreakInfo : byte
		{
			No,
			NoAlpha,
			Yes,
			Optional,
			HyphenatePoorChoice,
			HyphenateGoodChoice,
			HorizontalTab
		}
		
		public enum BreakClass : byte
		{
			XX_Unknown = 0,							//	unknown line breaking behaviour
				
			//	Normative Properties
				
			BK_MandatoryBreak,						//	cause a line break (after)
			CR_CarriageReturn,						//	cause a line break (after), except between CR and LF
			LF_LineFeed,							//	cause a line break (after)
			CM_CombiningMarks,						//	prohibit a line break between char and preceding char
			SG_Surrogates,							//	prohibit a break between high and following low surrogate
			GL_NonBreakingGlue,						//	prohibit a line break before or after
			CB_ContingentBreakOpportunity,			//	provide a line break opportunity contingent on add. info.
			SP_Space,								//	generally provide a line break opportunity after, enables indirect breaks
			ZW_ZeroWidthSpace,						//	optional break
			NL_NewLine,								//	causes a line break (after)
			WJ_WordJoiner,							//	prohibit line breaks before or after
				
			//	Break Opportunities
				
			BA_BreakOpportunityAfter,				//	generally provide a line break opportunity after
			BB_BreakOpportunityBefore,				//	generally provide a line break opportunity before
			B2_BreakOpportunityBeforeAndAfter,		//	provide a line break opportunity before and after
			HY_Hyphen,								//	provide a line break opportunity after, except in numeric context
				
			//	Characters Prohibiting Certain Breaks
				
			IN_Inseparable,							//	allow only indirect line breaks between pairs
			NS_NonStarter,							//	allow only indirect line breaks before
			OP_OpeningPunctuation,					//	prohibit a line break after
			CL_ClosingPunctuation,					//	prohibit a line break before
			QU_AmbiguousQuotation,					//	act as they are both opening and closing
			EX_ExclamationInterrogation,			//	prohibit line break before
				
			//	Numeric Context
				
			NU_Numeric,								//	form numeric expressions
			IS_InfixSeparatorNumeric,				//	prevent breaks after any and before numeric
			SY_SymbolsAllowingBreaks,				//	prevent a break before and allow a break after
			PR_PrefixNumeric,						//	don't break in front of a numeric expression
			PO_PostfixNumeric,						//	don't break following a numeric expression
				
			//	Other Characters
				
			AL_OrdinaryAlphabeticAndSymbol,			//	...
			ID_Ideographic,							//	break before or after, except in some numeric context
			AI_AmbiguousAlphabeticOrIdeographic,	//	...
			SA_ComplexContextSouthEastAsian,		//	...
		}
		
		public enum StretchClass : byte
		{
			NoStretch			= 0,
			Character			= 1,
			Space				= 2,
			CharacterSpace		= 3,
			Kashida				= 4,
		}
		#endregion
		
		public static BreakAnalyzer				DefaultBreakAnalyzer
		{
			get
			{
				if (Unicode.break_analyzer == null)
				{
					System.Type                host_type = typeof (Common.Drawing.TextBreak);
					System.Reflection.Assembly assembly  = host_type.Assembly;
					
					using (System.IO.Stream stream = assembly.GetManifestResourceStream ("Epsitec.Common.Drawing.Resources.LineBreak.compressed"))
					{
						using (System.IO.Stream data = Common.IO.Decompression.CreateStream (stream))
						{
							using (System.IO.StreamReader reader = new System.IO.StreamReader (data, System.Text.Encoding.ASCII))
							{
								Unicode.break_analyzer = new BreakAnalyzer ();
								Unicode.break_analyzer.LoadFile (reader);
							}
						}
					}
				}
				
				return Unicode.break_analyzer;
			}
		}
		
		
		public class BreakAnalyzer
		{
			public BreakAnalyzer()
			{
				/*
				 *	Find the break class for a specified Unicode character. We have split
				 *	the Unicode range into following ranges:
				 *
				 *	- 0x000000 - 0x0031FF : table 1
				 *	- 0x003200 - 0x00D7FF : known to be ideographic
				 *	- 0x00D800 - 0x00DFFF : known to be surrogates
				 *	- 0x00E000 - 0x00F8FF : known to be custom
				 *	- 0x00F900 - 0x00FFFF : table 2
				 *	- 0x010000 - ...      : list of ranges
				 *
				 *	The most used zone will map into table 1 (or will be ideographic, which
				 *	will be found very quickly too). Less used codes usually reside in table 2
				 *	or in the list.
				 *
				 *	The tables take up less than 15 KB. Let us hope that the accesses in the
				 *	first pages will be done in the CPUs cache.
				 */
				
				this.table_1  = new Unicode.BreakClass[0x3200];	//	Unicode 000000-0031FF
				this.table_2  = new Unicode.BreakClass[0x0700];	//	Unicode 00F900-00FFFF
				this.elements = new System.Collections.ArrayList ();
			}
			
			
			public Unicode.BreakClass			this[int code]
			{
				get
				{
					if (code < 0x003200)
					{
						return this.table_1[code];
					}
					else if (code < 0x00D800)
					{
						return Unicode.BreakClass.ID_Ideographic;
					}
					else if (code < 0x00E000)
					{
						return Unicode.BreakClass.SG_Surrogates;
					}
					else if (code < 0x00F900)
					{
						return Unicode.BreakClass.XX_Unknown;
					}
					else if (code < 0x010000)
					{
						return this.table_2[code - 0x00F900];
					}
					else
					{
						foreach (Element elem in this.elements)
						{
							if ((elem.code_begin <= code) &&
								(elem.code_end   >= code))
							{
								return elem.break_class;
							}
						}
						
						return Unicode.BreakClass.XX_Unknown;
					}
				}
			}
			
			
			public void LoadFile(string path)
			{
				//	Voir http://www.unicode.org/Public/LineBreak.txt pour le
				//	fichier � jour; les explications relatives � son format sont
				//	ici http://www.unicode.org/Public/UNIDATA/UCD.html.
				
				using (System.IO.StreamReader reader = new System.IO.StreamReader (path, System.Text.Encoding.ASCII))
				{
					this.LoadFile (reader);
				}
			}
			
			public void LoadFile(System.IO.TextReader reader)
			{
				for (;;)
				{
					string line = reader.ReadLine ();
					
					if (line == null)
					{
						break;
					}
					
					if (line.StartsWith ("#"))
					{
						continue;
					}
					
					int pos = line.IndexOf (';');
					
					if (pos > 0)
					{
						string code_range = line.Substring (0, pos);
						string break_name = line.Substring (pos+1, 2);
						
						Unicode.BreakClass break_class = BreakAnalyzer.ParseBreakClass (break_name);
						
						int code_begin;
						int code_end;
						
						if (code_range.IndexOf ("..") > -1)
						{
							string[] range = code_range.Split ('.');
							
							code_begin = int.Parse (range[0], System.Globalization.NumberStyles.HexNumber);
							code_end   = int.Parse (range[2], System.Globalization.NumberStyles.HexNumber);
						}
						else
						{
							code_begin = int.Parse (code_range, System.Globalization.NumberStyles.HexNumber);
							code_end   = code_begin + 1;
						}
						
					again_range:
						if (code_begin < 0x003200)				//	0x000000 - 0x0031FF, 1�re table
						{
							this.table_1[code_begin - 0x000000] = break_class;
							
							code_begin++;
							
							if (code_begin < code_end)
							{
								goto again_range;
							}
						}
						else if (code_begin < 0x00F900)			//	0x003200 - 0x00F8FF, tranche connue
						{
							continue;
						}
						else if (code_begin < 0x010000)			//	0x00F900 - 0x00FFFF, 2�me table
						{
							this.table_2[code_begin - 0x00F900] = break_class;
							
							code_begin++;
							
							if (code_begin < code_end)
							{
								goto again_range;
							}
						}
						else if ((this.elements.Count == 0) ||
							/**/ (this.TailElement.break_class != break_class) ||
							/**/ (this.TailElement.code_end + 1 != code_begin))
						{
							Element element = new Element ();
							
							element.break_class = break_class;
							element.code_begin  = code_begin;
							element.code_end    = code_end;
							
							this.elements.Add (element);
						}
						else
						{
							this.TailElement.code_end = code_end;
						}
					}
				}
			}
			
			
			public bool IsSpace(Unicode.BreakClass break_class)
			{
				switch (break_class)
				{
					case Unicode.BreakClass.SP_Space:
					case Unicode.BreakClass.ZW_ZeroWidthSpace:
					case Unicode.BreakClass.BK_MandatoryBreak:
						return true;
					default:
						return false;
				}
			}
			
			public bool IsSpace(int code)
			{
				return this.IsSpace (this[code]);
			}
			
			public bool IsControl(int code)
			{
				switch ((Unicode.Code)code)
				{
					case Unicode.Code.Invalid:
						throw new System.ArgumentException ("Invalid Unicode code.");
					
					case Unicode.Code.CarriageReturn:
					case Unicode.Code.LineFeed:
					case Unicode.Code.NextLine:
					case Unicode.Code.VerticalTab:
						throw new System.ArgumentException ("Deprecated Unicode code.");
					
					case Unicode.Code.LineSeparator:
					case Unicode.Code.PageSeparator:
					case Unicode.Code.ParagraphSeparator:
						break;
					
					case Unicode.Code.HorizontalTab:
						break;
					
					case Unicode.Code.LeftToRightEmbedding:
					case Unicode.Code.LeftToRightMark:
					case Unicode.Code.LeftToRightOverride:
					case Unicode.Code.RightToLeftEmbedding:
					case Unicode.Code.RightToLeftMark:
					case Unicode.Code.RightToLeftOverride:
					case Unicode.Code.PopDirectionalFormatting:
						break;
					
					case Unicode.Code.StartOfGuardedArea:
					case Unicode.Code.EndOfGuardedArea:
						break;
					
					case Unicode.Code.StartOfText:
					case Unicode.Code.EndOfText:
						break;
					
					default:
						return false;
				}
				
				return true;
			}
			
			public bool IsBreak(int code)
			{
				switch ((Unicode.Code)code)
				{
					case Unicode.Code.Invalid:
						throw new System.ArgumentException ("Invalid Unicode code.");
					
					case Unicode.Code.CarriageReturn:
					case Unicode.Code.LineFeed:
					case Unicode.Code.NextLine:
					case Unicode.Code.VerticalTab:
						throw new System.ArgumentException ("Deprecated Unicode code.");
					
					case Unicode.Code.LineSeparator:
					case Unicode.Code.PageSeparator:
					case Unicode.Code.ParagraphSeparator:
						break;
					
					case Unicode.Code.EndOfText:
						break;
					
					default:
						return false;
				}
				
				return true;
			}
			
			public static StretchClass GetStretchClass(int code)
			{
				switch ((Unicode.Code) code)
				{
					case Unicode.Code.Space:
					case Unicode.Code.NoBreakSpace:
					case Unicode.Code.NarrowNoBreakSpace:
						return StretchClass.Space;
					
					case Unicode.Code.Tatweel:
						return StretchClass.Kashida;
					
					case Unicode.Code.EnQuad:
					case Unicode.Code.EmQuad:
					case Unicode.Code.EnSpace:
					case Unicode.Code.EmSpace:
					case Unicode.Code.ThreePerEmSpace:
					case Unicode.Code.FourPerEmSpace:
					case Unicode.Code.SixPerEmSpace:
					case Unicode.Code.ThinSpace:
					case Unicode.Code.HairSpace:
					case Unicode.Code.MediumMathSpace:
						return StretchClass.CharacterSpace;
					
					case Unicode.Code.ZeroWidthSpace:
					case Unicode.Code.ZeroWidthNonJoiner:
					case Unicode.Code.ZeroWidthJoiner:
					case Unicode.Code.WordJoiner:
						return StretchClass.NoStretch;
					
					case Unicode.Code.FigureSpace:
					case Unicode.Code.PunctuationSpace:
						return StretchClass.CharacterSpace;
				}
				
				return StretchClass.Character;
			}
			
			public static byte GetStretchClass(ulong code)
			{
				return (byte) BreakAnalyzer.GetStretchClass ((int) code);
			}
			
			public static void GetStretchClass(ulong[] text, int start, int length, byte[] attributes)
			{
				for (int i = 0; i < length; i++)
				{
					int          code    = Unicode.Bits.GetCode (text[start+i]);
					StretchClass stretch = BreakAnalyzer.GetStretchClass (code);
					
					attributes[i] = (byte) stretch;
				}
			}
			
			
			public void GenerateBreaks(ulong[] text, int start, int length, Unicode.BreakInfo[] breaks)
			{
				//	L'algorithme est tir� de l'Unicode Standard Annex #14, d�crit
				//	ici : http://www.unicode.org/reports/tr14
				
				if (length == 0)
				{
					return;
				}
				
				Unicode.BreakClass[] cclass = new Unicode.BreakClass[length+1];
				
				int cur_unicode = 0;
				int prv_unicode = 0;
				
				Unicode.BreakClass cur_class;
				Unicode.BreakClass prv_class = Unicode.BreakClass.AL_OrdinaryAlphabeticAndSymbol;
				
				for (int i = 0; i < length; i++)
				{
					cur_unicode = Unicode.Bits.GetCode (text[start+i]);
					cur_class   = this[cur_unicode];
					
					//	Comme on passe d�j� en revue le texte, on profite de mettre
					//	� jour les fanions Combining et Reordering :
					
					if (cur_class == Unicode.BreakClass.CM_CombiningMarks)
					{
						Unicode.Bits.SetCombiningFlag (ref text[start+i], true);
						
						if (prv_class == Unicode.BreakClass.SP_Space)
						{
							cur_class = Unicode.BreakClass.AL_OrdinaryAlphabeticAndSymbol;
						}
						else
						{
							cur_class = prv_class;
						}
					}
					else
					{
						Unicode.Bits.SetCombiningFlag (ref text[start+i], false);
					}
					
					//	TODO: impl�menter le reoredering... Voir :
					//	- http://www.unicode.org/Public/UNIDATA/UCD.html#Bidi_Class_Values
					//	- http://www.unicode.org/reports/tr9/
					//	- http://www.unicode.org/Public/UNIDATA/UnicodeData.txt
					
					//	Simplifie les traitements ult�rieurs en rempla�ant certaines
					//	classes par d'autres :
					
					switch (cur_class)
					{
						case Unicode.BreakClass.SG_Surrogates:
							throw new Unicode.IllegalCodeException ("Found surrogate in UTF-32");
						
						case Unicode.BreakClass.NL_NewLine:
							cur_class = Unicode.BreakClass.BK_MandatoryBreak;
							break;
						
						case Unicode.BreakClass.WJ_WordJoiner:
							cur_class = Unicode.BreakClass.GL_NonBreakingGlue;
							break;
						
						case Unicode.BreakClass.AI_AmbiguousAlphabeticOrIdeographic:
						case Unicode.BreakClass.CB_ContingentBreakOpportunity:
						case Unicode.BreakClass.XX_Unknown:
							cur_class = Unicode.BreakClass.AL_OrdinaryAlphabeticAndSymbol;
							break;
						
						case Unicode.BreakClass.SA_ComplexContextSouthEastAsian:
							cur_class = Unicode.BreakClass.ID_Ideographic;
							break;
						
						//	LB 9
						//	LB 11a
						
						case Unicode.BreakClass.SP_Space:
							if ((prv_class == Unicode.BreakClass.OP_OpeningPunctuation) ||				
								(prv_class == Unicode.BreakClass.B2_BreakOpportunityBeforeAndAfter))
							{
								cur_class = prv_class;
							}
							break;
						
						//	Remplace "a'a" par "aaa" pour simplifier le traitement
						//	des apostrophes latins (en fran�ais, par exemple).
						
						case Unicode.BreakClass.AL_OrdinaryAlphabeticAndSymbol:
							if ((i > 1) &&
								(cclass[i-2] == Unicode.BreakClass.AL_OrdinaryAlphabeticAndSymbol) &&
								(prv_unicode == '\''))
							{
								cclass[i-1] = Unicode.BreakClass.AL_OrdinaryAlphabeticAndSymbol;
							}
							break;
					}
					
					cclass[i]   = cur_class;
					prv_class   = cur_class;
					prv_unicode = cur_unicode;
				}
				
				cclass[length] = Unicode.BreakClass.BK_MandatoryBreak;
				breaks[0]      = cclass[0] == Unicode.BreakClass.BK_MandatoryBreak ? Unicode.BreakInfo.Yes : Unicode.BreakInfo.No;
				
				for (int i = 1; i < length; i++)
				{
					if ((cclass[i-1] == Unicode.BreakClass.AL_OrdinaryAlphabeticAndSymbol) &&
						(breaks[i-1] == Unicode.BreakInfo.No))
					{
						breaks[i-1] = Unicode.BreakInfo.NoAlpha;
					}
					
					//	LB 3a / Always break after hard line breaks
					
					if ((cclass[i] == Unicode.BreakClass.BK_MandatoryBreak) ||
						(cclass[i] == Unicode.BreakClass.LF_LineFeed))
					{
						breaks[i] = Unicode.BreakInfo.Yes;
						continue;
					}
					
					//	LB 3b / Treat CR followed by LF as hard line breaks
					
					if (cclass[i] == Unicode.BreakClass.CR_CarriageReturn)
					{
						if (cclass[i+1] == Unicode.BreakClass.LF_LineFeed)
						{
							breaks[i] = Unicode.BreakInfo.No;
						}
						else
						{
							breaks[i] = Unicode.BreakInfo.Yes;
						}
						continue;
					}
					
					
					//	LB 3c / Don't break before hard line breaks
					//	LB 4  / Don't break before spaces or zero-width space
					
					if ((cclass[i+1] == Unicode.BreakClass.BK_MandatoryBreak) ||
						(cclass[i+1] == Unicode.BreakClass.LF_LineFeed) ||
						(cclass[i+1] == Unicode.BreakClass.CR_CarriageReturn) ||
						(cclass[i+1] == Unicode.BreakClass.SP_Space) ||
						(cclass[i+1] == Unicode.BreakClass.ZW_ZeroWidthSpace))
					{
						breaks[i] = Unicode.BreakInfo.No;
						continue;
					}
					
					//	LB 5  / Break after zero-width space
					
					if (cclass[i] == Unicode.BreakClass.ZW_ZeroWidthSpace)
					{
						breaks[i] = Unicode.BreakInfo.Optional;
						continue;
					}
					
					//	LB 6 and LB 7 are specific to combining marks, which have been mapped to
					//	the appropriate breaking classes in the preliminary conversion; no need
					//	to handle these rules here.
					
					//	LB 8  / Don't break before ']', '!', ';', '/', even after spaces
					
					if ((cclass[i+1] == Unicode.BreakClass.CL_ClosingPunctuation) ||
						(cclass[i+1] == Unicode.BreakClass.EX_ExclamationInterrogation) ||
						(cclass[i+1] == Unicode.BreakClass.IS_InfixSeparatorNumeric) ||
						(cclass[i+1] == Unicode.BreakClass.SY_SymbolsAllowingBreaks))
					{
						breaks[i] = Unicode.BreakInfo.No;
						continue;
					}
					
					//	LB 9  / Don't break after '[', even after spaces
					
					if (cclass[i] == Unicode.BreakClass.OP_OpeningPunctuation)
					{
						breaks[i] = Unicode.BreakInfo.No;
						continue;
					}
#if false
		if (cclass[i] == agg::unicode_helper::QU_AmbiguousQuotation)				//	LB 10 / Don't break within '"[', even with intervening spaces
		{
			Count j = i+1;
			
			while ( (j < length)
				 && (cclass[j] == agg::unicode_helper::SP_Space) )
			{
				j++;		//	skip the spaces
			}
			
			if (cclass[j] == agg::unicode_helper::OP_OpeningPunctuation)
			{
				breaks[i] = agg::unicode_helper::break_no;
                continue;
			}
		}
		if (cclass[i] == agg::unicode_helper::CL_ClosingPunctuation)				//	LB 11 / Don't break within ']<non starter>', even with intervening spaces
		{
			Count j = i+1;
			
			while ( (j < length)
				 && (cclass[j] == agg::unicode_helper::SP_Space) )
			{
				j++;		//	skip the spaces
			}
			
			if (cclass[j] == agg::unicode_helper::NS_NonStarter)
			{
				breaks[i] = agg::unicode_helper::break_no;
                continue;
			}
		}
#endif
					//	LB 11a/ Don't break within '-- --', even with intervening spaces
					
					if (cclass[i] == Unicode.BreakClass.B2_BreakOpportunityBeforeAndAfter)
					{
						breaks[i] = Unicode.BreakInfo.No;
						continue;
					}
					
					//	LB 12 / Break after spaces
					
					if (cclass[i] == Unicode.BreakClass.SP_Space)
					{
						breaks[i] = Unicode.BreakInfo.Optional;
						continue;
					}
					
					//	LB 13 / Don't break before or after NBSP or WORD JOINER
					
					if ((cclass[i] == Unicode.BreakClass.GL_NonBreakingGlue) ||
						(cclass[i+1] == Unicode.BreakClass.GL_NonBreakingGlue))
					{
						breaks[i] = Unicode.BreakInfo.No;
						continue;
					}
					
					//	LB 14 / Don't break before or after '"'
					
					if ((cclass[i] == Unicode.BreakClass.QU_AmbiguousQuotation) ||
						(cclass[i+1] == Unicode.BreakClass.QU_AmbiguousQuotation))
					{
						breaks[i] = Unicode.BreakInfo.No;
						continue;
					}
					
					//	LB 15 / Don't break before hyphen-minus, etc. or after accute accents
					
					if ((cclass[i+1] == Unicode.BreakClass.BA_BreakOpportunityAfter) ||
						(cclass[i+1] == Unicode.BreakClass.HY_Hyphen) ||
						(cclass[i+1] == Unicode.BreakClass.NS_NonStarter) ||
						(cclass[i] == Unicode.BreakClass.BB_BreakOpportunityBefore))
					{
						breaks[i] = Unicode.BreakInfo.No;
						continue;
					}
					
					//	LB 15b/ Break after hyphen-minus and before accute accents
					
					if ((cclass[i] == Unicode.BreakClass.HY_Hyphen) ||
						(cclass[i+1] == Unicode.BreakClass.BB_BreakOpportunityBefore))
					{
						breaks[i] = Unicode.BreakInfo.Optional;
						continue;
					}
					
					//	LB 16 / Don't break between two ellipses, or between letters or numbers and ellipsis
					
					if (cclass[i+1] == Unicode.BreakClass.IN_Inseparable)
					{
						if ((cclass[i] == Unicode.BreakClass.AL_OrdinaryAlphabeticAndSymbol) ||
							(cclass[i] == Unicode.BreakClass.ID_Ideographic) ||
							(cclass[i] == Unicode.BreakClass.IN_Inseparable) ||
							(cclass[i] == Unicode.BreakClass.NU_Numeric))
						{
							breaks[i] = Unicode.BreakInfo.No;
							continue;
						}
					}
					
					//	LB 17 / Don't break within 'a9', '3a' or '<ideogram>%'
					
					if ((cclass[i] == Unicode.BreakClass.ID_Ideographic) &&
						(cclass[i+1] == Unicode.BreakClass.PO_PostfixNumeric))
					{
						breaks[i] = Unicode.BreakInfo.No;
						continue;
					}
					if ((cclass[i] == Unicode.BreakClass.AL_OrdinaryAlphabeticAndSymbol) &&
						(cclass[i+1] == Unicode.BreakClass.NU_Numeric))
					{
						breaks[i] = Unicode.BreakInfo.No;
						continue;
					}
					if ((cclass[i] == Unicode.BreakClass.NU_Numeric) &&
						(cclass[i+1] == Unicode.BreakClass.AL_OrdinaryAlphabeticAndSymbol))
					{
						breaks[i] = Unicode.BreakInfo.No;
						continue;
					}
					
					
					//	LB 18 / Don't break between the following pairs of classes ...
					//			which accepts numbers in the form ...
					//			PR ? ( OP | HY ) ? NU ( NU | IS ) * CL ? PO ?
					
					if (cclass[i+1] == Unicode.BreakClass.NU_Numeric)
					{
						if ((cclass[i] == Unicode.BreakClass.HY_Hyphen) ||
							(cclass[i] == Unicode.BreakClass.IS_InfixSeparatorNumeric) ||
							(cclass[i] == Unicode.BreakClass.NU_Numeric) ||
							(cclass[i] == Unicode.BreakClass.PR_PrefixNumeric) ||
							(cclass[i] == Unicode.BreakClass.SY_SymbolsAllowingBreaks))
						{
							breaks[i] = Unicode.BreakInfo.No;
							continue;
						}
					}
					if (cclass[i] == Unicode.BreakClass.PR_PrefixNumeric)
					{
						if ((cclass[i+1] == Unicode.BreakClass.AL_OrdinaryAlphabeticAndSymbol) ||
							(cclass[i+1] == Unicode.BreakClass.HY_Hyphen) ||
							(cclass[i+1] == Unicode.BreakClass.ID_Ideographic) ||
							(cclass[i+1] == Unicode.BreakClass.OP_OpeningPunctuation))
						{
							breaks[i] = Unicode.BreakInfo.No;
							continue;
						}
					}
					if (cclass[i+1] == Unicode.BreakClass.PO_PostfixNumeric)
					{
						if ((cclass[i] == Unicode.BreakClass.CL_ClosingPunctuation) ||
							(cclass[i] == Unicode.BreakClass.NU_Numeric))
						{
							breaks[i] = Unicode.BreakInfo.No;
							continue;
						}
					}
					
					//	LB 19 / Don't break between alphabetics
					
					if (cclass[i] == Unicode.BreakClass.AL_OrdinaryAlphabeticAndSymbol)
					{
						if ((cclass[i+1] == Unicode.BreakClass.AL_OrdinaryAlphabeticAndSymbol) ||
							(cclass[i+1] == Unicode.BreakClass.BK_MandatoryBreak) ||
							(cclass[i+1] == Unicode.BreakClass.NL_NewLine) ||
							(cclass[i+1] == Unicode.BreakClass.LF_LineFeed) ||
							(cclass[i+1] == Unicode.BreakClass.CR_CarriageReturn))
						{
							breaks[i] = Unicode.BreakInfo.No;
							continue;
						}
					}
					
					//	LB 20 / Break everywhere else
					
					breaks[i] = Unicode.BreakInfo.Optional;
				}
				
				//	Traitement final : analyse encore le dernier �l�ment dans la table
				//	pour ajuster quelques cas particuliers :
				
				if (breaks[length-1] == Unicode.BreakInfo.No)
				{
					switch (cclass[length-1])
					{
						case Unicode.BreakClass.AL_OrdinaryAlphabeticAndSymbol:
							breaks[length-1] = Unicode.BreakInfo.NoAlpha;
							break;
						
						case Unicode.BreakClass.SP_Space:
						case Unicode.BreakClass.ZW_ZeroWidthSpace:
							breaks[length-1] = Unicode.BreakInfo.Optional;
							break;
					}
				}
				
				for (int i = 0; i < length; i++)
				{
					if (Unicode.Bits.GetCode (text[start+i]) == (int) Unicode.Code.HorizontalTab)
					{
						breaks[i] = Unicode.BreakInfo.HorizontalTab;
					}
				}
			}
			
			
			private static Unicode.BreakClass ParseBreakClass(string token)
			{
				switch (token)
				{
					case "XX": return Unicode.BreakClass.XX_Unknown;
					case "BK": return Unicode.BreakClass.BK_MandatoryBreak;
					case "CR": return Unicode.BreakClass.CR_CarriageReturn;
					case "LF": return Unicode.BreakClass.LF_LineFeed;
					case "CM": return Unicode.BreakClass.CM_CombiningMarks;
					case "SG": return Unicode.BreakClass.SG_Surrogates;
					case "GL": return Unicode.BreakClass.GL_NonBreakingGlue;
					case "CB": return Unicode.BreakClass.CB_ContingentBreakOpportunity;
					case "SP": return Unicode.BreakClass.SP_Space;
					case "ZW": return Unicode.BreakClass.ZW_ZeroWidthSpace;
					case "NL": return Unicode.BreakClass.NL_NewLine;
					case "WJ": return Unicode.BreakClass.WJ_WordJoiner;
					case "BA": return Unicode.BreakClass.BA_BreakOpportunityAfter;
					case "BB": return Unicode.BreakClass.BB_BreakOpportunityBefore;
					case "B2": return Unicode.BreakClass.B2_BreakOpportunityBeforeAndAfter;
					case "HY": return Unicode.BreakClass.HY_Hyphen;
					case "IN": return Unicode.BreakClass.IN_Inseparable;
					case "NS": return Unicode.BreakClass.NS_NonStarter;
					case "OP": return Unicode.BreakClass.OP_OpeningPunctuation;
					case "CL": return Unicode.BreakClass.CL_ClosingPunctuation;
					case "QU": return Unicode.BreakClass.QU_AmbiguousQuotation;
					case "EX": return Unicode.BreakClass.EX_ExclamationInterrogation;
					case "NU": return Unicode.BreakClass.NU_Numeric;
					case "IS": return Unicode.BreakClass.IS_InfixSeparatorNumeric;
					case "SY": return Unicode.BreakClass.SY_SymbolsAllowingBreaks;
					case "PR": return Unicode.BreakClass.PR_PrefixNumeric;
					case "PO": return Unicode.BreakClass.PO_PostfixNumeric;
					case "AL": return Unicode.BreakClass.AL_OrdinaryAlphabeticAndSymbol;
					case "ID": return Unicode.BreakClass.ID_Ideographic;
					case "AI": return Unicode.BreakClass.AI_AmbiguousAlphabeticOrIdeographic;
					case "SA": return Unicode.BreakClass.SA_ComplexContextSouthEastAsian;
					
					case "JL": case "JV": case "JT":	//	Jamo, 4.1.0
					case "H2": case "H3":				//	Hangul, 4.1.0
						return Unicode.BreakClass.ID_Ideographic;
				}
			
				throw new System.ArgumentOutOfRangeException ("token", token, "Invalid token");
			}
			
			
			private Element						TailElement
			{
				get
				{
					return this.elements[this.elements.Count-1] as Element;
				}
			}
			
			
			#region Element Class
			private class Element
			{
				public Unicode.BreakClass		break_class;
				public int						code_begin;
				public int						code_end;
			}
			#endregion
			
			Unicode.BreakClass[]				table_1;
			Unicode.BreakClass[]				table_2;
			System.Collections.ArrayList		elements;
		}
		
		
		private static BreakAnalyzer			break_analyzer;
	}
}
