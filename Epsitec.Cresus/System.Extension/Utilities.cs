namespace System
{
	/// <summary>
	/// La classe Utilities propose quelques fonctions d'utilité générale
	/// </summary>

	public class Utilities
	{
		public static bool StringSimplify(ref string value)
		{
			//	Convertie une chaîne entre guillemets en une chaîne sans guillemets
			//	retourne "true" si la chaîne avait des guillemets "false" sinon
			//	une exception est levée si des guillemets non apparentés sont trouvés
			//	enlève les doubles guillemets en milieu de chaîne pour n'en mettre qu'un.
			//	Faudrait-il traiter aussi les séquences BOA genre \t ?

			if (value.IndexOf ('"') == 0)
			{
				if ((value.Length > 1) && (value.LastIndexOf ('"') == value.Length-1))
				{
					System.Text.StringBuilder buffer = new System.Text.StringBuilder ();

					for (int i = 1 ; i < value.Length-1 ; i++)
					{
						char	c = value[i];
						if (c == '"')
						{
							i++;
							c = value[i];
							if ((i == value.Length-1) || (c != '"'))
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

		//	?DD	est-ce que cela vaudrait la peine de définir une classe StringArray 
		//		dérivée de System.Collections.ArrayList ?

		public static int StringToTokens(string value, char sep, out System.Collections.ArrayList list)
		{
			return StringToTokens (value, sep, null, out list);
		}
	
		public static int StringToTokens(string value, char sep, string trimchars, out System.Collections.ArrayList list)
		{
			//	DD: Cette routine peut servir dans tous contextes
			//		elle devrait être dans une librairie à part
			//		par exemple Epsitec.Cresus.Utilities
			//		ou même Epsitec.Utilities ?

			//	transforme une chaîne en une série de Tokens strings
			//	en fonction du séparateur donné
			//	rend le nombre de tokens trouvé en sortie
			//	les chaînes entre guillemets sont conservées
			//	une exception est levée s'il y a un nombre impair de guillemets

			//	trimchars permet de supprimer les espaces (et autres) excédentaires 
			//	devant et derrière les strings
			//			par exemple pour accepter
			//			un, deux,    trois , quatre

			list = new System.Collections.ArrayList ();
			System.Text.StringBuilder buffer = new System.Text.StringBuilder ();

			string	s;
			int		nb = 0;
			int		lv = 0;

			for (int i = 0 ; i < value.Length ; i++)
			{
				char	c = value[i];
				if (c == '"')
				{
					lv = 1-lv;
				}

				if ( (lv == 0) && (c == sep) )
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
			if ( lv > 0 )
			{
				throw new System.Exception (string.Format ("Quotes mismatch in {0}", value));
			}
			return nb;
		}
	}
}
