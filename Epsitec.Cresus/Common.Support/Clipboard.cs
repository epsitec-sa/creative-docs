//	Copyright � 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : en chantier/PA

namespace Epsitec.Common.Support
{
	using IDataObject       = System.Windows.Forms.IDataObject;
	using Regex             = System.Text.RegularExpressions.Regex;
	using RegexOptions      = System.Text.RegularExpressions.RegexOptions;
	using Match             = System.Text.RegularExpressions.Match;
	using CaptureCollection = System.Text.RegularExpressions.CaptureCollection;
	using Capture           = System.Text.RegularExpressions.Capture;
	
	/// <summary>
	/// La classe Clipboard donne acc�s au presse-papier.
	/// </summary>
	public class Clipboard
	{
		public static Data GetData()
		{
			return new Data (System.Windows.Forms.Clipboard.GetDataObject ());
		}
		
		public static string ConvertBrokenUtf8ToString(string value)
		{
			//	Le presse-papier peut contenir des donn�es encod�es en UTF-8 que l'environnement
			//	.NET traite comme s'il s'agissait de texte normal, ce qui r�sulte en une conversion
			//	incorrecte dans System.String (les donn�es en entr�e sont trait�s comme des 'char',
			//	alors que ce sont des 'byte').
			//	
			//	ConvertBrokenUtf8ToString permet de corriger cela en r�-encodant la string avec le
			//	bon encodage; le hic, c'est que .NET ne fournit pas de moyen d'acc�der sous une
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
		
		public static string ConvertToSimpleXml(string value)
		{
			System.Text.StringBuilder buffer = new System.Text.StringBuilder ();
			
			int last_tag  = -1;
			int last_elem = -1;
			
			bool is_space        = true;
			bool preserve_spaces = false;
			
			for (int i = 0; i < value.Length; i++)
			{
				char c = value[i];
				
				//	Tous les carat�res "blancs" sont consid�r�s comme des espaces :
				
				if ((c == '\t') || (c == '\n') || (c == '\r'))
				{
					c = ' ';
				}
				
				//	Traite de mani�re diff�renci�e les d�buts de tags "<x>", les d�buts
				//	d'entit�s "&amp;" et le texte normal :
				
				if (c == '<')
				{
					last_tag        = buffer.Length;
					preserve_spaces = false;
				}
				else if (c == '&')
				{
					last_elem = buffer.Length;
				}
				else if (last_tag == -1)
				{
					if (c == ' ')
					{
						if (is_space && !preserve_spaces)
						{
							//	Si plusieurs espaces se suivent, on n'en conserve que le
							//	premier, sauf si on est dans le mode sp�cial dict� par
							//	Office (<span style="mso-spacerun: yes">..</span>) :
							
							continue;
						}
						
						is_space = true;
					}
					else
					{
						is_space = false;
					}
				}
				
				buffer.Append (c);
				
				//	Traite la fin des tags et �l�ments :
				
				if (c == '>')
				{
					//	Si on a trouv� une fin de tag ">", il y a forc�ment eu un d�but de tag
					//	auparavant. On peut maintenant analyser ce tag et d�terminer ce qu'il
					//	faut en faire :
					
					System.Diagnostics.Debug.Assert (last_tag >= 0);
					
					string tag = buffer.ToString (last_tag, buffer.Length - last_tag);
					
					//	Supprime le tag du buffer; s'il est compatible avec la version simplifi�e
					//	il sera rajout� par la suite :
					
					buffer.Length = last_tag;
					last_tag      = -1;
					
					switch (tag)
					{
						case  "<b>": case  "<B>": buffer.Append ("<b>");  continue;
						case "</b>": case "</B>": buffer.Append ("</b>"); continue;
						case  "<i>": case  "<I>": buffer.Append ("<i>");  continue;
						case "</i>": case "</I>": buffer.Append ("</i>"); continue;
						
						case "</p>": case "</P>":
						case "<br>": case "<BR>": case "<br/>":
							buffer.Append ("<br/>");
							is_space = true;
							continue;
					}
					
					Match match_style = Clipboard.regex_style.Match (tag);
					
					if (match_style.Success)
					{
						CaptureCollection style_captures = match_style.Groups["style"].Captures;
						
						foreach (Capture style_capture in style_captures)
						{
							string style     = style_capture.Value;
							Match  match_opt = Clipboard.regex_mso_spacerun.Match (style);
							
							if ((match_opt.Success) &&
								(match_opt.Groups["opt"].Captures.Count == 1) &&
								(match_opt.Groups["opt"].Captures[0].Value == "yes"))
							{
								preserve_spaces = true;
							}
						}
					}
				}
				else if ((c == ';') && (last_elem >= 0))
				{
					//	Si on a trouv� une fin d'entit�, il faut v�rifier si c'est l'une des
					//	entit�s de base valides pour du XML, ou au contraire, s'il s'agit de
					//	particularit�s li�es � HTML :
					
					string elem = buffer.ToString (last_elem, buffer.Length - last_elem);
					
					buffer.Length = last_elem;
					last_elem     = -1;
					
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
					
					if (Clipboard.map_entities.Contains (elem))
					{
						elem = Clipboard.map_entities[elem] as string;
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
		
		
		public class Data
		{
			internal Data(IDataObject data)
			{
				this.data = data;
			}
			
			
			public string[]						NativeFormats
			{
				get
				{
					return data.GetFormats (false);
				}
			}
			
			public string[]						AllPossibleFormats
			{
				get
				{
					return data.GetFormats (true);
				}
			}
			
			
			public object Read(string format)
			{
				return data.GetData (format, true);
			}
			
			public string ReadAsString(string format)
			{
				return this.Read (format) as string;
			}
			
			public string ReadHtmlFragment()
			{
				string raw_html = this.ReadAsString ("HTML Format");
				
				if (raw_html == null)
				{
					return null;
				}
				
				//	V�rifie qu'il y a bien tous les tags dans le fragment HTML :
				
				int idx_version    = raw_html.IndexOf ("Version:");
				int idx_start_html = raw_html.IndexOf ("StartHTML:");
				int idx_end_html   = raw_html.IndexOf ("EndHTML:");
				int idx_start_frag = raw_html.IndexOf ("StartFragment:");
				int idx_end_frag   = raw_html.IndexOf ("EndFragment:");
				int idx_start      = raw_html.IndexOf ("<!--StartFragment");
				int idx_end        = raw_html.IndexOf ("<!--EndFragment");
				int idx_begin      = raw_html.IndexOf (">", idx_start) + 1;
				
				if ((idx_start      < idx_version) ||
					(idx_end        < idx_start) ||
					(idx_begin      < 1) ||
					(idx_version    < 0) ||
					(idx_start_html < idx_version) ||
					(idx_end_html   < idx_start_html) ||
					(idx_start_frag < idx_version) ||
					(idx_end_frag   < idx_start_frag))
				{
					return null;
				}
				
				return Clipboard.ConvertBrokenUtf8ToString (raw_html.Substring (idx_begin, idx_end - idx_begin));
			}
			
			public string ReadHtmlDocument()
			{
				string raw_html = this.ReadAsString ("HTML Format");
				
				if (raw_html == null)
				{
					return null;
				}
				
				//	V�rifie qu'il y a bien tous les tags dans le fragment HTML :
				
				int idx_version    = raw_html.IndexOf ("Version:");
				int idx_start_html = raw_html.IndexOf ("StartHTML:");
				int idx_end_html   = raw_html.IndexOf ("EndHTML:");
				int idx_start_frag = raw_html.IndexOf ("StartFragment:");
				int idx_end_frag   = raw_html.IndexOf ("EndFragment:");
				int idx_start      = raw_html.IndexOf ("<!--StartFragment");
				int idx_end        = raw_html.IndexOf ("<!--EndFragment");
				int idx_begin      = raw_html.IndexOf (">", idx_start) + 1;
				
				if ((idx_start      < idx_version) ||
					(idx_end        < idx_start) ||
					(idx_begin      < 1) ||
					(idx_version    < 0) ||
					(idx_start_html < idx_version) ||
					(idx_end_html   < idx_start_html) ||
					(idx_start_frag < idx_version) ||
					(idx_end_frag   < idx_start_frag))
				{
					return null;
				}
				
				idx_begin = System.Int32.Parse (this.ExtractDigits (raw_html, idx_start_html + 10));
				idx_end   = System.Int32.Parse (this.ExtractDigits (raw_html, idx_end_html + 8));
				
				return Clipboard.ConvertBrokenUtf8ToString (raw_html.Substring (idx_begin, idx_end - idx_begin));
			}
			
			
			protected string ExtractDigits(string text, int pos)
			{
				System.Text.StringBuilder buffer = new System.Text.StringBuilder ();
				
				for (int i = pos; i < text.Length; i++)
				{
					char c = text[i];
					
					if ((c < '0') || (c > '9'))
					{
						break;
					}
					
					buffer.Append (c);
				}
				
				return buffer.ToString ();
			}
			
			public bool IsCompatible(Format format)
			{
				switch (format)
				{
					case Format.Text:
						return this.data.GetDataPresent ("System.String", true);
					case Format.Image:
						return this.data.GetDataPresent ("System.Drawing.Bitmap", true);
					case Format.MicrosoftHtml:
						return this.data.GetDataPresent ("HTML Format", false);
				}
				
				return false;
			}
			
			
			protected IDataObject				data;
		}
		
		public enum Format
		{
			None,
			Unsupported,
			
			Text,
			Image,
			MicrosoftHtml
		}
		
		#region Clipboard setup
		static Clipboard()
		{
			//	R�f�rence: http://www.w3.org/TR/REC-html40/sgml/entities.html#iso-88591
			
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
			
			Clipboard.map_entities = new System.Collections.Hashtable ();
			
			for (int i = 0; i < pairs.Length; i += 2)
			{
				Clipboard.map_entities[pairs[i+0]] = pairs[i+1];
			}
			
			Clipboard.regex_style        = new Regex (@"\A<span((\s*style\s*=\s*(?<a>['""])(?<style>.*?)\k<a>)|(\s*\w*)|(\s*\w*\s*=\s*[^\s>]*?\s*))*\s*\>\Z", RegexOptions.Compiled | RegexOptions.Multiline);
			Clipboard.regex_mso_spacerun = new Regex (@"\Amso\-spacerun\:\s*(?<opt>\w*)\s*\Z", RegexOptions.Compiled | RegexOptions.Multiline);
		}
		#endregion
		
		static System.Collections.Hashtable		map_entities;
		static Regex							regex_style;
		static Regex							regex_mso_spacerun;
	}
}
