//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : en chantier/PA

namespace Epsitec.Common.Support
{
	using IDataObject = System.Windows.Forms.IDataObject;
	
	/// <summary>
	/// La classe Clipboard donne accès au presse-papier.
	/// </summary>
	public class Clipboard
	{
		public static Data GetData()
		{
			return new Data (System.Windows.Forms.Clipboard.GetDataObject ());
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
		
		public static string ConvertToSimpleXml(string value)
		{
			System.Text.StringBuilder buffer = new System.Text.StringBuilder ();
			
			int last_tag  = -1;
			int last_elem = -1;
			
			bool is_space = true;
			
			for (int i = 0; i < value.Length; i++)
			{
				char c = value[i];
				
				if (c == '<')
				{
					last_tag = buffer.Length;
				}
				else if (c == '&')
				{
					last_elem = buffer.Length;
				}
				else if (last_tag == -1)
				{
					if ((c == ' ') ||
						(c == '\t') ||
						(c == '\n') ||
						(c == '\r'))
					{
						if (is_space)
						{
							continue;
						}
						is_space = true;
						c = ' ';
					}
				}
				
				if (c == 160)
				{
					c = ' ';
				}
				
				buffer.Append (c);
				
				if (c == '>')
				{
					System.Diagnostics.Debug.Assert (last_tag >= 0);
					
					string tag = buffer.ToString (last_tag, buffer.Length - last_tag);
					
					switch (tag)
					{
						case "<b>": case "</b>":
						case "<i>": case "</i>":
							break;
						case "</p>":
							buffer.Length = last_tag;
							buffer.Append ("</br>");
							break;
						
						default:
							buffer.Length = last_tag;
							break;
					}
					
					last_tag = -1;
				}
				else if ((c == ';') && (last_elem >= 0))
				{
					string elem = buffer.ToString (last_elem, buffer.Length - last_elem);
					
					switch (elem)
					{
						case "&lt;":
						case "&gt;":
						case "&amp;":
						case "&quot;":
						case "&apos;":
						case "&#160;":
						case "&#8212;":
							break;
						
						default:
							elem = elem.Substring (1, elem.Length - 2);
							
							if (Clipboard.map_entities.Contains (elem))
							{
								elem = Clipboard.map_entities[elem] as string;
							}
							
							if (elem.StartsWith ("#"))
							{
								int num = System.Int32.Parse (elem.Substring (1), System.Globalization.CultureInfo.InvariantCulture);
								buffer.Length = last_elem;
								buffer.Append ((char) num);
							}
							else
							{
								throw new System.FormatException (string.Format ("Illegal entity {0} found.", elem));
							}
							
							break;
					}
					
					
					last_elem = -1;
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
			
			public System.IO.MemoryStream ReadAsStream(string format)
			{
				return this.Read (format) as System.IO.MemoryStream;
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
			
			Clipboard.map_entities = new System.Collections.Hashtable ();
			
			for (int i = 0; i < pairs.Length; i += 2)
			{
				Clipboard.map_entities[pairs[i+0]] = pairs[i+1];
			}
		}
		
		static System.Collections.Hashtable		map_entities;
	}
}
