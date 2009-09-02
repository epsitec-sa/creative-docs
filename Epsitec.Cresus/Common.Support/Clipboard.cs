//	Copyright © 2004-2009, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Epsitec.Common.Support
{
	using IDataObject=System.Windows.Forms.IDataObject;
	
	/// <summary>
	/// La classe Clipboard donne accès au presse-papier.
	/// </summary>
	public class Clipboard
	{
		public static ClipboardReadData GetData()
		{
			return new ClipboardReadData (null);
		}
		
		public static void     SetData(ClipboardWriteData data)
		{
			System.Windows.Forms.Clipboard.SetDataObject (data.Data, true);
		}
		
		
		public static string ConvertBrokenUtf8ToString(string value)
		{
			//	Le presse-papier peut contenir des données encodées en UTF-8 que l'environnement
			//	.NET traite comme s'il s'agissait de texte normal, ce qui résulte en une conversion
			//	incorrecte dans System.String (les données en entrée sont traités comme des 'char',
			//	alors que ce sont des 'byte').
			//	
			//	ConvertBrokenUtf8ToString permet de corriger cela en ré-encodant la string avec le
			//	bon encodage; le hic, c'est que .NET ne fournit pas de moyen d'accéder sous une
			//	forme binaire brute au contenu de la string...
			
			int len = value.Length;
			
			char[] chars = value.ToCharArray ();
			byte[] bytes = new byte[len];
			
			for (int i = 0; i < len; i++)
			{
				char c = chars[i];
				
				if (c > 255)
				{
					throw new System.FormatException ("Text is not UTF-8 based.");
				}
				
				bytes[i] = (byte) c;
			}
			
			return System.Text.Encoding.UTF8.GetString (bytes);
		}
		
		public static string ConvertStringToBrokenUtf8(string value)
		{
			byte[] bytes = System.Text.Encoding.UTF8.GetBytes (value);
			char[] chars = new char[bytes.Length];
			
			for (int i = 0; i < bytes.Length; i++)
			{
				chars[i] = (char) bytes[i];
			}
			
			return new string (chars);
		}
		
		public static string ConvertSimpleXmlToHtml(string value)
		{
			System.Text.StringBuilder buffer = new System.Text.StringBuilder ();
			
			int lineBegin  = 0;
			int lastTag    = -1;
			int spaceCount = 0;
			
			for (int i = 0; i < value.Length; i++)
			{
				char c = value[i];
				
				if (c == '<')
				{
					lastTag = buffer.Length;
					spaceCount = 0;
				}
				else if (c == ' ')
				{
					if (lastTag == -1)
					{
						spaceCount++;
					}
				}
				else
				{
					if (spaceCount > 1)
					{
						//	Il y a plusieurs espaces consécutifs; ceux-ci sont générelement supprimés
						//	par le consommateur HTML.
						
						buffer.Length = buffer.Length - spaceCount;
						buffer.Append (@" <span style=""mso-spacerun: yes"">");
						
						for (int j = 1; j < spaceCount; j++)
						{
							buffer.Append ((char)160);
						}
						
						buffer.Append (@"</span>");
					}
					
					spaceCount = 0;
				}
				
				buffer.Append (c);
				
				if (c == '>')
				{
					//	Si on a trouvé une fin de tag ">", il y a forcément eu un début de tag
					//	auparavant. On peut maintenant analyser ce tag et déterminer ce qu'il
					//	faut en faire :
					
					System.Diagnostics.Debug.Assert (lastTag >= 0);
					
					string tag = buffer.ToString (lastTag, buffer.Length - lastTag);
					
					if (tag == "<br/>")
					{
						//	FrontPage n'aime pas <br/>, alors on doit modifier le tag pour que ça
						//	soit plus digeste :
						
						buffer.Length = lastTag;
						buffer.Append ("<br />");
						lineBegin = buffer.Length;
					}
					
					if (tag == "<tab/>")
					{
						buffer.Length = lastTag;
						buffer.Append ("<span style='mso-tab-count:1'>");
						buffer.Append ((char)160, 11);
						buffer.Append (' ');
						buffer.Append ("</span>");
					}
					
					lastTag = -1;
				}
			}
			
			return buffer.ToString ();
		}
		
		public static string ConvertHtmlToSimpleXml(string value)
		{
			System.Text.StringBuilder buffer = new System.Text.StringBuilder ();
			
			int lastTag  = -1;
			int lastElem = -1;
			
			bool isSpace        = true;
			bool preserveSpaces = false;
			bool deleteSpaces   = false;
			
			for (int i = 0; i < value.Length; i++)
			{
				char c = value[i];
				
				if (((c == ' ') || (c == (char)160)) &&
					(deleteSpaces))
				{
					//	Si on est dans un mode spécial dicté par Office
					//	(<span style="mso-tab-count:1">    </span>) on mange
					//	tous les espaces.
					
					continue;
				}
				
				//	Tous les caratères "blancs" sont considérés comme des espaces :
				
				if ((c == '\t') || (c == '\n') || (c == '\r'))
				{
					c = ' ';
				}
				
				//	Traite de manière différenciée les débuts de tags "<x>", les débuts
				//	d'entités "&amp;" et le texte normal :
				
				if (c == '<')
				{
					if (i+4 < value.Length)
					{
						if (value.Substring (i, 4) == "<!--")
						{
							int endPos = value.IndexOf ("-->", i+4);
							
							if (endPos > 0)
							{
								i = endPos + 2;
								continue;
							}

							throw new System.FormatException ("Unbalanced HTML comment found");
						}
					}

					lastTag        = buffer.Length;
					preserveSpaces = false;
					deleteSpaces   = false;
				}
				else if (c == '&')
				{
					lastElem = buffer.Length;
				}
				else if (lastTag == -1)
				{
					if (c == ' ')
					{
						if (isSpace && !preserveSpaces)
						{
							//	Si plusieurs espaces se suivent, on n'en conserve que le
							//	premier, sauf si on est dans le mode spécial dicté par
							//	Office (<span style="mso-spacerun: yes">..</span>) :
							
							continue;
						}
						
						isSpace = true;
						
					}
					else
					{
						isSpace = false;
					}
				}
				
				buffer.Append (c);
				
				//	Traite la fin des tags et éléments :
				
				if (c == '>')
				{
					//	Si on a trouvé une fin de tag ">", il y a forcément eu un début de tag
					//	auparavant. On peut maintenant analyser ce tag et déterminer ce qu'il
					//	faut en faire :
					
					System.Diagnostics.Debug.Assert (lastTag >= 0);
					
					string tag = buffer.ToString (lastTag, buffer.Length - lastTag);
					
					//	Supprime le tag du buffer; s'il est compatible avec la version simplifiée
					//	il sera rajouté par la suite :
					
					buffer.Length = lastTag;
					lastTag      = -1;
					
					int spacePos = tag.IndexOf (' ');
					
					string cleanTag;
					
					if (spacePos > 0)
					{
						if (tag.EndsWith ("/>"))
						{
							cleanTag = tag.Substring (0, spacePos).ToLower () + "/>";
						}
						else
						{
							cleanTag = tag.Substring (0, spacePos).ToLower () + ">";
						}
					}
					else
					{
						cleanTag = tag.ToLower ();
					}
					
					switch (cleanTag)
					{
						case  "<b>":
						case  "<strong>":
							buffer.Append ("<b>");
							continue;
						
						case "</b>":
						case "</strong>":
							buffer.Append ("</b>");
							continue;
						
						case  "<i>":
						case  "<em>":
							buffer.Append ("<i>");
							continue;
						
						case "</i>":
						case "</em>":
							buffer.Append ("</i>");
							continue;
						
						case  "<u>":
							buffer.Append ("<u>");
							continue;
						
						case "</u>":
							buffer.Append ("</u>");
							continue;
						
						case "</p>":
						case "<br>":
						case "<br/>":
							buffer.Append ("<br/>");
							isSpace = true;
							continue;
					}
					
					if (tag == "<![if !supportEmptyParas]>")
					{
						string msoNbspEndif = "&nbsp;<![endif]>";
						
						if (value.Substring (i+1).StartsWith (msoNbspEndif))
						{
							i += msoNbspEndif.Length;
						}
					}
					
					Match matchStyle = Clipboard.regexStyle.Match (tag);
					
					if (matchStyle.Success)
					{
						CaptureCollection styleCaptures = matchStyle.Groups["style"].Captures;
						
						foreach (Capture styleCapture in styleCaptures)
						{
							string style     = styleCapture.Value;
							Match  matchOpt = Clipboard.regexMsoSpacerun.Match (style);
							
							if ((matchOpt.Success) &&
								(matchOpt.Groups["opt"].Captures.Count == 1) &&
								(matchOpt.Groups["opt"].Captures[0].Value == "yes"))
							{
								preserveSpaces = true;
							}
							else
							{
								matchOpt = Clipboard.regexMsoTabcount.Match (style);
								
								if ((matchOpt.Success) &&
									(matchOpt.Groups["opt"].Captures.Count == 1))
								{
									int n;
									try
									{
										Types.InvariantConverter.Convert (matchOpt.Groups["opt"].Captures[0].Value, out n);
										
										while (n-- > 0)
										{
											buffer.Append ("<tab/>");
										}
										
										deleteSpaces = true;
									}
									catch
									{
									}
								}
							}
						}
					}
				}
				else if ((c == ';') && (lastElem >= 0))
				{
					//	Si on a trouvé une fin d'entité, il faut vérifier si c'est l'une des
					//	entités de base valides pour du XML, ou au contraire, s'il s'agit de
					//	particularités liées à HTML :
					
					string elem = buffer.ToString (lastElem, buffer.Length - lastElem);
					
					buffer.Length = lastElem;
					lastElem     = -1;
					
					switch (elem)
					{
						case "&lt;":
						case "&gt;":
						case "&amp;":
						case "&quot;":
						case "&apos;":
							buffer.Append (elem);
							continue;
					}
					
					elem = elem.Substring (1, elem.Length - 2);
					string mappedElem;
					
					if (Clipboard.mapEntities.TryGetValue (elem, out mappedElem))
					{
						elem = mappedElem;
					}
					
					if (elem.StartsWith ("#"))
					{
						int num = System.Int32.Parse (elem.Substring (1), System.Globalization.CultureInfo.InvariantCulture);
						buffer.Append ((char) num);
					}
					else
					{
						throw new System.FormatException (string.Format ("Illegal entity {0} found.", elem));
					}
				}
			}
			
			return buffer.ToString ();
		}
		
		
		public static ClipboardReadData CreateReadDataFromIDataObject(IDataObject data)
		{
			return new ClipboardReadData (data);
		}
		
		
		
		#region Clipboard setup
		
		static Clipboard()
		{
			//	Référence: http://www.w3.org/TR/REC-html40/sgml/entities.html#iso-88591
			
			string[] pairs = new string[]
				{
					"nbsp",		"#160",
					"iexcl",	"#161",
					"cent",		"#162",
					"pound",	"#163",
					"curren",	"#164",
					"yen",		"#165",
					"brvbar",	"#166",
					"sect",		"#167",
					"uml",		"#168",
					"copy",		"#169",
					"ordf",		"#170",
					"laquo",	"#171",
					"not",		"#172",
					"shy",		"#173",
					"reg",		"#174",
					"macr",		"#175",
					"deg",		"#176",
					"plusmn",	"#177",
					"sup2",		"#178",
					"sup3",		"#179",
					"acute",	"#180",
					"micro",	"#181",
					"para",		"#182",
					"middot",	"#183",
					"cedil",	"#184",
					"sup1",		"#185",
					"ordm",		"#186",
					"raquo",	"#187",
					"frac14",	"#188",
					"frac12",	"#189",
					"frac34",	"#190",
					"iquest",	"#191",
					"Agrave",	"#192",
					"Aacute",	"#193",
					"Acirc",	"#194",
					"Atilde",	"#195",
					"Auml",		"#196",
					"Aring",	"#197",
					"AElig",	"#198",
					"Ccedil",	"#199",
					"Egrave",	"#200",
					"Eacute",	"#201",
					"Ecirc",	"#202",
					"Euml",		"#203",
					"Igrave",	"#204",
					"Iacute",	"#205",
					"Icirc",	"#206",
					"Iuml",		"#207",
					"ETH",		"#208",
					"Ntilde",	"#209",
					"Ograve",	"#210",
					"Oacute",	"#211",
					"Ocirc",	"#212",
					"Otilde",	"#213",
					"Ouml",		"#214",
					"times",	"#215",
					"Oslash",	"#216",
					"Ugrave",	"#217",
					"Uacute",	"#218",
					"Ucirc",	"#219",
					"Uuml",		"#220",
					"Yacute",	"#221",
					"THORN",	"#222",
					"szlig",	"#223",
					"agrave",	"#224",
					"aacute",	"#225",
					"acirc",	"#226",
					"atilde",	"#227",
					"auml",		"#228",
					"aring",	"#229",
					"aelig",	"#230",
					"ccedil",	"#231",
					"egrave",	"#232",
					"eacute",	"#233",
					"ecirc",	"#234",
					"euml",		"#235",
					"igrave",	"#236",
					"iacute",	"#237",
					"icirc",	"#238",
					"iuml",		"#239",
					"eth",		"#240",
					"ntilde",	"#241",
					"ograve",	"#242",
					"oacute",	"#243",
					"ocirc",	"#244",
					"otilde",	"#245",
					"ouml",		"#246",
					"divide",	"#247",
					"oslash",	"#248",
					"ugrave",	"#249",
					"uacute",	"#250",
					"ucirc",	"#251",
					"uuml",		"#252",
					"yacute",	"#253",
					"thorn",	"#254",
					"yuml",		"#255",
				};

			Clipboard.mapEntities = new Dictionary<string, string> ();
			
			for (int i = 0; i < pairs.Length; i += 2)
			{
				Clipboard.mapEntities.Add (pairs[i+0], pairs[i+1]);
			}
			
			Clipboard.regexStyle       = new Regex (@"\A<span((\s*style\s*=\s*(?<a>['""])(?<style>.*?)\k<a>)|(\s*\w*)|(\s*\w*\s*=\s*[^\s>]*?\s*))*\s*\>\Z", RegexOptions.Compiled | RegexOptions.Multiline);
			Clipboard.regexMsoSpacerun = new Regex (@"\Amso\-spacerun\:\s*(?<opt>\w*)\s*\Z", RegexOptions.Compiled | RegexOptions.Multiline);
			Clipboard.regexMsoTabcount = new Regex (@"\Amso\-tab\-count\:\s*(?<opt>\w*)\s*\Z", RegexOptions.Compiled | RegexOptions.Multiline);

			Platform.Win32.Clipboard.Initialize ();
			Platform.Win32.Clipboard.DataChanged += Clipboard.HandleDataChanged;
		}

		#endregion

		private static void HandleDataChanged(object sender, ClipboardDataChangedEventArgs e)
		{
			var handler = Clipboard.DataChanged;

			if (handler != null)
			{
				handler (sender, e);
			}
		}

		
		public static class Formats
		{
			public const string Hmtl			= "HTML Format";
			public const string String			= "System.String";
			public const string Bitmap			= "System.Drawing.Bitmap";
			public const string TextLayoutV1	= "Epsitec:TextLayout ver:1";
			
			public const string RawBitmap		= "Bitmap";
			public const string CommaSeparatedValue = "CommaSeparatedValue";
			public const string Dib				= "Dib";
			public const string EnhancedMetafile = "EnhancedMetafile";
			public const string FileDrop		= "FileDrop";
			public const string RawHtml			= "Html";
			public const string Locale			= "Locale";
			public const string MetafilePict	= "MetafilePict";
			public const string OemText			= "OemText";
			public const string Palette			= "Palette";
			public const string PenData			= "PenData";
			public const string Riff			= "Riff";
			public const string Rtf				= "Rtf";
			public const string Serializable	= "Serializable";
			public const string StringFormat	= "StringFormat";
			public const string SymbolicLink	= "SymbolicLink";
			public const string Text			= "Text";
			public const string Tiff			= "Tiff";
			public const string UnicodeText		= "UnicodeText";
			public const string WaveAudio		= "WaveAudio";
		}

		

		public static event EventHandler<ClipboardDataChangedEventArgs> DataChanged;
		
		private static readonly Dictionary<string, string>	mapEntities;
		private static readonly Regex						regexStyle;
		private static readonly Regex						regexMsoSpacerun;
		private static readonly Regex						regexMsoTabcount;
	}
}
