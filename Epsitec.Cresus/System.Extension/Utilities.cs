namespace System
{
	/// <summary>
	/// La classe Utilities propose quelques fonctions d'utilit� g�n�rale
	/// </summary>

	public class Utilities
	{
		public static bool StringSimplify(ref string value)
		{
			return Utilities.StringSimplify(ref value, '"');
		}

		public static bool StringSimplify(ref string value, char quote)
		{
			//	Converti une cha�ne entre guillemets en une cha�ne sans guillemets
			//	retourne "true" si la cha�ne avait des guillemets "false" sinon
			//	une exception est lev�e si des guillemets non apparent�s sont trouv�s
			//	enl�ve les doubles guillemets en milieu de cha�ne pour n'en mettre qu'un.
			//	Faudrait-il traiter aussi les s�quences BOA genre \t ?

			if (value.IndexOf (quote) == 0)
			{
				if ((value.Length > 1) && (value.LastIndexOf (quote) == value.Length-1))
				{
					System.Text.StringBuilder buffer = new System.Text.StringBuilder ();

					for (int i = 1 ; i < value.Length-1 ; i++)
					{
						char	c = value[i];
						if (c == quote)
						{
							i++;
							c = value[i];
							if ((i == value.Length-1) || (c != quote))
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
			return StringToTokens (value, sep, null, out tokens);
		}
	
		public static int StringToTokens(string value, char sep, string trimchars, out string [] tokens)
		{
			//	transforme une cha�ne en une s�rie de Tokens strings
			//	en fonction du s�parateur donn�.
			//	Rend le nombre de tokens trouv� en sortie,
			//	les cha�nes entre guillemets ou apostrophes sont conserv�es.
			//	Une exception est lev�e s'il y a un nombre impair de guillemets.

			//	trimchars permet de supprimer les espaces (et autres) exc�dentaires 
			//	devant et derri�re les strings
			//			par exemple pour accepter
			//			un, deux,    trois , quatre

			System.Collections.ArrayList list = new System.Collections.ArrayList ();
			System.Text.StringBuilder buffer = new System.Text.StringBuilder ();

			string	s;
			int		nb = 0;
			int		n1 = 0;
			int		n2 = 0;

			for (int i = 0 ; i < value.Length ; i++)
			{
				char	c = value[i];
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
					s = buffer.ToString ();
					if ( trimchars != null ) s.Trim (trimchars.ToCharArray ());
					list.Add (s);
					buffer.Length = 0;
					nb++;
					continue;
				}
				buffer.Append (c);
			}
			s = buffer.ToString ();
			if ( trimchars != null ) s.Trim (trimchars.ToCharArray ());
			list.Add (s);
			nb++;
			if ( (n1 > 0) || (n2 > 0))
			{
				throw new System.Exception (string.Format ("Quotes mismatch in {0}", value));
			}
			tokens = new string [list.Count];
			list.CopyTo (tokens, 0);
			return nb;
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
		
		
		
		private enum SplitState
		{
			Normal, DoubleQuote, SingleQuote
		}
		
		
		public static string[] Split(string text, char sep)
		{
			if (text.IndexOf (sep) < 0)
			{
				//	Optimisation pour le cas le plus fr�quent : il n'y a qu'un seul �l�ment
				//	et aucun s�parateur.
				
				return new string[1] { text };
			}
			
			const int max = 50;
			int[] sep_pos = new int[max];
			int arg_count = 1;
			
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
									sep_pos[arg_count] = i;
									arg_count++;
									if (arg_count >= max-1)
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
			
			sep_pos[0]         = -1;
			sep_pos[arg_count] = text.Length;
			
			string[] args = new string[arg_count];
			
			for (int i = 0; i < arg_count; i++)
			{
				args[i] = text.Substring (sep_pos[i]+1, sep_pos[i+1]-sep_pos[i]-1);
			}
			
			return args;
		}
	}
}
