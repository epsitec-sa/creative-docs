namespace System
{
	/// <summary>
	/// La classe Utilities propose quelques fonctions d'utilité générale
	/// </summary>

	public class Utilities
	{
		public static bool StringSimplify(ref string value)
		{
			return Utilities.StringSimplify(ref value, '"');
		}

		public static bool StringSimplify(ref string value, char quote)
		{
			//	Converti une chaîne entre guillemets en une chaîne sans guillemets
			//	retourne "true" si la chaîne avait des guillemets "false" sinon
			//	une exception est levée si des guillemets non apparentés sont trouvés
			//	enlève les doubles guillemets en milieu de chaîne pour n'en mettre qu'un.
			//	Faudrait-il traiter aussi les séquences BOA genre \t ?

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
			//	transforme une chaîne en une série de Tokens strings
			//	en fonction du séparateur donné.
			//	Rend le nombre de tokens trouvé en sortie,
			//	les chaînes entre guillemets ou apostrophes sont conservées.
			//	Une exception est levée s'il y a un nombre impair de guillemets.

			//	trimchars permet de supprimer les espaces (et autres) excédentaires 
			//	devant et derrière les strings
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
	}
}
