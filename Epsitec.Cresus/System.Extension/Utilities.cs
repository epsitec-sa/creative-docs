namespace System
{
	/// <summary>
	/// La classe Utilities propose quelques fonctions d'utilit� g�n�rale
	/// </summary>

	public class Utilities
	{
		public static bool StringSimplify(ref string value)
		{
			//	Convertie une cha�ne entre guillemets en une cha�ne sans guillemets
			//	retourne "true" si la cha�ne avait des guillemets "false" sinon
			//	une exception est lev�e si des guillemets non apparent�s sont trouv�s
			//	enl�ve les doubles guillemets en milieu de cha�ne pour n'en mettre qu'un.
			//	Faudrait-il traiter aussi les s�quences BOA genre \t ?

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

		//	?DD	est-ce que cela vaudrait la peine de d�finir une classe StringArray 
		//		d�riv�e de System.Collections.ArrayList ?

		public static int StringToTokens(string value, char sep, out System.Collections.ArrayList list)
		{
			return StringToTokens (value, sep, null, out list);
		}
	
		public static int StringToTokens(string value, char sep, string trimchars, out System.Collections.ArrayList list)
		{
			//	DD: Cette routine peut servir dans tous contextes
			//		elle devrait �tre dans une librairie � part
			//		par exemple Epsitec.Cresus.Utilities
			//		ou m�me Epsitec.Utilities ?

			//	transforme une cha�ne en une s�rie de Tokens strings
			//	en fonction du s�parateur donn�
			//	rend le nombre de tokens trouv� en sortie
			//	les cha�nes entre guillemets sont conserv�es
			//	une exception est lev�e s'il y a un nombre impair de guillemets

			//	trimchars permet de supprimer les espaces (et autres) exc�dentaires 
			//	devant et derri�re les strings
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
