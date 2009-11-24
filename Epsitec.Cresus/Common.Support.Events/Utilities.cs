//	Copyright © 2003-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Denis DUMOULIN & Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Common.Support
{
	/// <summary>
	/// La classe Utilities propose quelques fonctions d'utilité générale
	/// </summary>

	public static class Utilities
	{
		public static string StringSimplify(string value)
		{
			Utilities.StringSimplify (ref value);
			return value;
		}
		
		public static bool StringSimplify(ref string value)
		{
			if (value.Length > 1)
			{
				char quote = value[0];
				
				switch (quote)
				{
					case '\'':
					case '\"':
						return Utilities.StringSimplify (ref value, quote, quote);
				}
			}
			
			return false;
		}
		
		public static bool StringSimplify(ref string value, char quote)
		{
			return Utilities.StringSimplify (ref value, quote, quote);
		}
		
		public static bool StringSimplify(ref string value, char startQuote, char endQuote)
		{
			//	Convertit une chaîne entre guillemets en une chaîne sans guillemets
			//	retourne "true" si la chaîne avait des guillemets "false" sinon
			//	une exception est levée si des guillemets non apparentés sont trouvés
			//	enlève les doubles guillemets en milieu de chaîne pour n'en mettre qu'un.
			
			if (value.IndexOf (startQuote) == 0)
			{
				if ((value.Length > 1) && (value.LastIndexOf (endQuote) == value.Length-1))
				{
					System.Text.StringBuilder buffer = new System.Text.StringBuilder ();
					
					for (int i = 1 ; i < value.Length-1 ; i++)
					{
						char c = value[i];
						
						if ((c == startQuote) &&
							(startQuote == endQuote))
						{
							i++;
							c = value[i];
							if ((i == value.Length-1) ||
								(c != startQuote))
							{
								throw new System.Exception (string.Format ("Quotes mismatch in {0}", value));
							}
						}
						buffer.Append (c);
					}
					value = buffer.ToString ();
					return true;
				}
				throw new System.Exception (string.Format ("Quotes mismatch in {0}", value));
			}
			
			return false;
		}

		
		public static int StringToTokens(string value, char sep, out string[] tokens)
		{
			var list = new List<string> (Utilities.StringToTokens (value, sep, null));
			tokens = list.ToArray ();
			return list.Count;
		}

		public static IEnumerable<string> StringToTokens(string value, char sep, System.StringSplitOptions options)
		{
			switch (options)
			{
				case System.StringSplitOptions.None:
					return Utilities.StringToTokens (value, sep, null);

				case System.StringSplitOptions.RemoveEmptyEntries:
					return Utilities.StringToTokens (value, sep, null).Where (x => !string.IsNullOrEmpty (x));

				default:
					throw new System.ArgumentException ("Invalid options");
			}
		}

		public static IEnumerable<string> StringToTokens(string value, char sep, string trimchars)
		{
			//	transforme une chaîne en une série de Tokens strings
			//	en fonction du séparateur donné.
			//	Rend le nombre de tokens trouvé en sortie,
			//	les chaînes entre guillemets ou apostrophes sont conservées.
			//	Une exception est levée s'il y a un nombre impair de guillemets.

			//	trimchars permet de supprimer les espaces (et autres) excédentaires 
			//	devant et derrière les strings
			//			par exemple pour accepter
			//			un, deux,    trois , quatre

			var buffer = new System.Text.StringBuilder ();
			var trim   = string.IsNullOrEmpty (trimchars) ? new char[0] : trimchars.ToCharArray ();

			int	n1 = 0;
			int	n2 = 0;

			System.Func<string> getToken =
				delegate
				{
					string s = buffer.ToString ();
					
					buffer.Length = 0;

					if (trim.Length > 0)
					{
						return s.Trim (trim);
					}
					else
					{
						return s;
					}
				};

			for (int i = 0; i < value.Length; i++)
			{
				char c = value[i];

				if ((c == '"') && (n2 == 0))
				{
					n1 = 1-n1;
				}
				if ((c == '\'') && (n1 == 0))
				{
					n2 = 1-n2;
				}

				if ((n1 == 0) && (n2 == 0) && (c == sep))
				{
					yield return getToken ();
				}
				else
				{
					buffer.Append (c);
				}
			}

			yield return getToken ();
			
			if ((n1 > 0) || (n2 > 0))
			{
				throw new System.Exception (string.Format ("Quotes mismatch in {0}", value));
			}
		}
		
		
		public static bool CheckForDuplicates(System.Collections.ICollection data)
		{
			return Utilities.CheckForDuplicates (data, true);
		}
		
		public static bool CheckForDuplicates(System.Collections.ICollection data, bool sort)
		{
			object[] temp = new object[data.Count];
			data.CopyTo (temp, 0);

			if (sort) System.Array.Sort (temp);
			
			for (int i = 1; i < temp.Length; i++)
			{
				if (temp[i-1] == temp[i])
				{
					return true;
				}
			}
			
			return false;
		}
		
		public static bool CheckForDuplicates(System.Collections.ICollection data, System.Collections.IComparer comparer)
		{
			return Utilities.CheckForDuplicates (data, comparer, true);
		}
		
		public static bool CheckForDuplicates(System.Collections.ICollection data, System.Collections.IComparer comparer, bool sort)
		{
			object[] temp = new object[data.Count];
			data.CopyTo (temp, 0);
			
			if (sort) System.Array.Sort (temp, comparer);
			
			for (int i = 1; i < temp.Length; i++)
			{
				if (comparer.Compare (temp[i-1], temp[i]) == 0)
				{
					return true;
				}
			}
			
			return false;
		}
		
		
		public static string TextToXml(string text)
		{
			text = text.Replace (@"&",  "&amp;");
			text = text.Replace (@"<",  "&lt;");
			text = text.Replace (@">",  "&gt;");
			text = text.Replace (@"""", "&quot;");
			text = text.Replace (@"'",  "&apos;");
			
			return text;
		}
		
		public static string TextToXmlBreak(string text)
		{
			text = text.Replace (@"&",  "&amp;");
			text = text.Replace (@"<",  "&lt;");
			text = text.Replace (@">",  "&gt;");
			text = text.Replace (@"""", "&quot;");
			text = text.Replace (@"'",  "&apos;");
			text = text.Replace ("\n",  "<br/>");
			
			return text;
		}
		
		public static string XmlToText(string text)
		{
			System.Text.StringBuilder buffer = new System.Text.StringBuilder ();
			
			for (int offset = 0; offset < text.Length; )
			{
				char c = text[offset];
				
				if (c == '&')
				{
					buffer.Append (Utilities.ParseCharOrXmlEntity (text, ref offset));
				}
				else if (c == '<')
				{
					int pos = text.IndexOf ('>', offset);
					
					if (pos < offset)
					{
						throw new System.FormatException (string.Format ("Invalid XML cannot be converted to text."));
					}
					
					offset = pos+1;
				}
				else
				{
					buffer.Append (c);
					offset++;
				}
			}
			
			return buffer.ToString();
		}
		
		public static string XmlBreakToText(string text)
		{
			return Utilities.XmlToText (text.Replace ("<br/>", "\n").Replace ("<tab/>", "\t"));
		}
		
		
		public static int SkipXmlChars(string text, int n)
		{
			//	Saute le nombre de caractères "logiques" indiqué et retourne le nombre
			//	de caractères réels dans 'text' qui ont été sautés.
			
			//	Une entité XML (par ex. "&quot;") compte comme 1 caractère, tout comme
			//	les tags <br/>, <tab/> et <img>.
			
			int offset = 0;
					
			for (int i = 0; (i < n) && (offset < text.Length); i++)
			{
				while (text[offset] == '<')
				{
					int pos = text.IndexOf ('>', offset);
					
					if ((text.IndexOf ("<br/", offset, 1) == 0) ||
						(text.IndexOf ("<br ", offset, 1) == 0) ||
						(text.IndexOf ("<img ", offset, 1) == 0) ||
						(text.IndexOf ("<tab/", offset, 1) == 0) ||
						(text.IndexOf ("<tab ", offset, 1) == 0))
					{
						i++;
					}
					
					System.Diagnostics.Debug.Assert (pos > 0);
					
					offset = pos + 1;
					
					if ((i >= n) || (offset >= text.Length))
					{
						return offset;
					}
				}
						
				Utilities.ParseCharOrXmlEntity (text, ref offset);
			}
			
			return offset;
		}
		
		public static int CountXmlChars(string text)
		{
			//	Compte le nombre de caractères "logiques" dans le texte.
			
			//	Une entité XML (par ex. "&quot;") compte comme 1 caractère, tout comme
			//	les tags <br/>, <tab/> et <img>.
			
			int i      = 0;
			int offset = 0;
					
			for (; offset < text.Length; i++)
			{
				while (text[offset] == '<')
				{
					int pos = text.IndexOf ('>', offset);
					
					if ((text.IndexOf ("<br/", offset, 1) == 0) ||
						(text.IndexOf ("<br ", offset, 1) == 0) ||
						(text.IndexOf ("<img ", offset, 1) == 0) ||
						(text.IndexOf ("<tab/", offset, 1) == 0) ||
						(text.IndexOf ("<tab ", offset, 1) == 0))
					{
						i++;
					}
					
					System.Diagnostics.Debug.Assert (pos > 0);
					
					offset = pos + 1;
					
					if (offset >= text.Length)
					{
						return i;
					}
				}
						
				Utilities.ParseCharOrXmlEntity (text, ref offset);
			}
			
			return i;
		}
		
		public static char ParseCharOrXmlEntity(string text, ref int offset)
		{
			if (text[offset] == '&')
			{
				int length = text.IndexOf (";", offset) - offset + 1;
				
				if (length < 3)
				{
					throw new System.FormatException (string.Format ("Bad entity syntax in '{0}'.", text.Substring (offset)));
				}
				
				char   code;
				string entity = text.Substring (offset, length).ToLower ();
				
				switch (entity)
				{
					case "&lt;":	code = '<';			break;
					case "&gt;":	code = '>';			break;
					case "&amp;":	code = '&';			break;
					case "&quot;":	code = '"';			break;
					case "&apos;":	code = '\'';		break;
					
					default:
						if (entity.StartsWith ("&#"))
						{
							entity = entity.Substring (2, entity.Length - 3);
							code   = (char) System.Int32.Parse (entity, System.Globalization.CultureInfo.InvariantCulture);
						}
						else
						{
							throw new System.FormatException (string.Format ("Unrecognized entity '{0}'.", entity));
						}
						break;
				}
				
				offset += length;
				
				return code;
			}
			
			return text[offset++];
		}
		
		
		private enum SplitState
		{
			Normal, DoubleQuote, SingleQuote
		}
		
		
		public static string[] Split(string text, char sep)
		{
			//	Comme string.Split, mais en mieux, en ce sens que cette méthode gère les
			//	guillemets simples (') et doubles ("), ainsi que les tags <>. Voir aussi
			//	la méthode StringToTokens qui fait la même chose sans gérer les tags.
			//
			//	Si un séparateur se trouve entre guillemets ou dans un tag, il est sauté.
			
			if (text == null)
			{
				return new string[0];
			}
			
			if (text.IndexOf (sep) < 0)
			{
				//	Optimisation pour le cas le plus fréquent : il n'y a qu'un seul élément
				//	et aucun séparateur.
				
				return new string[1] { text };
			}
			
			const int max = 50;
			int[] sepPos  = new int[max];
			int argCount  = 1;
			
			SplitState state  = SplitState.Normal;
			int        depth  = 0;
			bool       escape = false;
			
			for (int i = 0; i < text.Length; i++)
			{
				char c = text[i];
				
				if (escape)
				{
					escape = false;
					continue;
				}
				
				switch (state)
				{
					case SplitState.Normal:
						switch (c)
						{
							case '\"':	state = SplitState.DoubleQuote; break;
							case '\'':	state = SplitState.SingleQuote; break;
							case '<':	depth++;						break;
							case '>':	depth--;						break;
							case '\\':	escape = true;					break;
							
							default:
								if ((depth == 0) && (c == sep))
								{
									sepPos[argCount] = i;
									argCount++;
									if (argCount >= max-1)
									{
										throw new System.ArgumentException (string.Format ("Text too complex to split: {0}.", text));
									}
								}
								break;
						}
						break;
					
					case SplitState.SingleQuote:
						if (c == '\'')
						{
							state = SplitState.Normal;
						}
						break;
					
					case SplitState.DoubleQuote:
						if (c == '\"')
						{
							state = SplitState.Normal;
						}
						break;
				}
			}
			
			sepPos[0]         = -1;
			sepPos[argCount] = text.Length;
			
			string[] args = new string[argCount];
			
			for (int i = 0; i < argCount; i++)
			{
				args[i] = text.Substring (sepPos[i]+1, sepPos[i+1]-sepPos[i]-1);
			}
			
			return args;
		}

		public static string[] Split(string text, string separator)
		{
			//	Comme ci-dessus, avec une chaîne de caractères comme séparateur
			//	Tous texte entre guillemets ou dans un tag est sauté.

			if (text == null)
			{
				return new string[0];
			}
			
			if ((separator == null) ||
				(separator.Length == 0))
			{
				throw new System.ArgumentException (string.Format ("Empty separator for split: {0}.", text));
			}

			if (text.IndexOf (separator) < 0)
			{
				//	Optimisation pour le cas le plus fréquent : il n'y a qu'un seul élément
				//	et aucun séparateur.
				
				return new string[1] { text };
			}
			
			const int max = 50;
			int[] sepPos  = new int[max];
			int argCount  = 1;

			SplitState state  = SplitState.Normal;
			int        depth  = 0;
			bool       escape = false;
			char	   sep	  = separator[0];
			
			for (int i = 0; i <= text.Length - separator.Length; i++)
			{
				char c = text[i];
				
				if (escape)
				{
					escape = false;
					continue;
				}
				
				switch (state)
				{
					case SplitState.Normal:
					switch (c)
					{
						case '\"':	state = SplitState.DoubleQuote; break;
						case '\'':	state = SplitState.SingleQuote; break;
						case '<':	depth++;						break;
						case '>':	depth--;						break;
						case '\\':	escape = true;					break;
							
						default:
							if ((depth == 0) && (c == sep))
							{
								if (text.Substring (i, separator.Length) != separator) break;
								sepPos[argCount] = i;
								argCount++;
								if (argCount >= max-1)
								{
									throw new System.ArgumentException (string.Format ("Text too complex to split: {0}.", text));
								}
								i += separator.Length-1;
							}
							break;
					}
					break;
				
					case SplitState.SingleQuote:
						if (c == '\'')
						{
							state = SplitState.Normal;
						}
						break;
					
					case SplitState.DoubleQuote:
						if (c == '\"')
						{
							state = SplitState.Normal;
						}
						break;
				}
			}
			
			sepPos[0]         = -separator.Length;
			sepPos[argCount] = text.Length;
			
			string[] args = new string[argCount];
			
			for (int i = 0; i < argCount; i++)
			{
				args[i] = text.Substring (sepPos[i]+separator.Length, sepPos[i+1]-sepPos[i]-separator.Length);
			}
			
			return args;
		}
	}
}
