//	Copyright © 2003, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : en chantier

namespace Epsitec.Cresus.Database
{
	using System.Text.RegularExpressions;
	
	/// <summary>
	/// La classe DbSqlStandard implémente les services de base liés à la norme
	/// SQL.
	/// </summary>
	public class DbSqlStandard
	{
		private DbSqlStandard()
		{
		}
		
		static DbSqlStandard()
		{
			//	Un nom SQL valide est composé soit de:
			//
			//	- Un caractère alphabétique, suivi de 0-n caractères alphanumériques, y
			//	  compris le "_".
			//	- Un texte entre guillemets pouvant contenir tous les caractères supportés
			//	  par le jeu de caractères actif; les guillemets doivent être doublés s'ils
			//	  apparaissent dans le texte.
			
			DbSqlStandard.regex_name = new Regex (@"^([a-zA-Z][a-zA-Z0-9_]*|""([^""]|"""")*"")$", RegexOptions.Compiled | RegexOptions.ExplicitCapture);
			
			//	Une chaîne SQL valide est composée d'un texte entre apostrophes; les apostrophes
			//	doivent être doublés s'ils apparaissent dans le texte.
			
			DbSqlStandard.regex_string = new Regex (@"^'([^']|'')*'$", RegexOptions.Compiled | RegexOptions.ExplicitCapture);
			
			//	Une valeur numérique SQL valide est composée d'un signe négatif optionnel, suivi
			//	d'un ou plusieurs chiffres, et en option d'un point décimal et de un ou plusieurs
			//	chiffres.
			
			DbSqlStandard.regex_number = new Regex (@"^-?[0-9]+(\.[0-9]+)?$", RegexOptions.Compiled | RegexOptions.ExplicitCapture);
		}
		
		
		public static string ConcatNames(string a, string b)
		{
			if (DbSqlStandard.ValidateName (a) &&
				DbSqlStandard.ValidateName (b))
			{
				bool force_quotes = false;
				
				if (a.StartsWith (@""""))
				{
					force_quotes = true;
					a = a.Substring (1, a.Length-2);
				}
				if (b.StartsWith (@""""))
				{
					force_quotes = true;
					b = b.Substring (1, b.Length-2);
				}
				
				if (force_quotes)
				{
					return @"""" + a + b + @"""";
				}
				
				return a + b;
			}
			
			throw new DbFormatException (string.Format ("Expected two names: {0} and {1}", a, b));
		}
		
		public static string ConcatStrings(string a, string b)
		{
			if (DbSqlStandard.ValidateString (a) &&
				DbSqlStandard.ValidateString (b))
			{
				return "'" + a.Substring (1, a.Length-2) + b.Substring (1, b.Length-2) + "'";
			}
			
			throw new DbFormatException (string.Format ("Expected two strings: {0} and {1}", a, b));
		}
		
		
		public static string QualifyName(string qualifier, string name)
		{
			if (DbSqlStandard.ValidateName (qualifier) &&
				DbSqlStandard.ValidateName (name))
			{
				return qualifier + "." + name;
			}
			
			throw new DbFormatException (string.Format ("Cannot make qualified name from {0} and {1}", qualifier, name));
		}
		
		public static bool StringSimplify(ref string value)
		{
			//	DD: Cette routine peut servir dans tous contextes
			//		elle devrait être dans une librairie à part
			//		par exemple Epsitec.Cresus.Utilities
			//		ou même Epsitec.Utilities ?

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
								throw new DbFormatException (string.Format ("Quotes mismatch in {0}", value));
							}
						}
						buffer.Append (c);
					}
					value = buffer.ToString ();
					return true;
				}
				throw new DbFormatException (string.Format ("Quotes mismatch in {0}", value));
			}
			return false;
		}

		//	?DD	est-ce que cela vaudrait la peine de définir une classe StringArray 
		//		dérivée de System.Collections.ArrayList ?

		public static int StringToTokens(string value, char sep, out System.Collections.ArrayList list)
		{
			return StringToTokens(value, sep, null, out list);
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
				throw new DbFormatException (string.Format ("Quotes mismatch in {0}", value));
			}
			return nb;
		}

		public static void SplitQualifiedName(string value, out string qualifier, out string name)
		{
			if (DbSqlStandard.ValidateQualifiedName (value))
			{
				//	DONE: recherche le "." de manière plus intelligente, car si le qualifier est entre
				//	guillemets, il peut lui aussi contenir un "."... Exemple valide "A.B".C où le
				//	résultat doit être qualifier="A.B" et name="C".

				System.Collections.ArrayList list;
				int nb = StringToTokens (value, '.', out list);

				if ( nb != 2 )
				{
					throw new DbFormatException (string.Format ("{0} is not a qualified name", value));
				}

				qualifier = (string)list[0];
				name      = (string)list[1];
				
				return;
			}
			
			throw new DbFormatException (string.Format ("{0} is not a qualified name", value));
		}
		
		
		public static bool ValidateName(string value)
		{
			if (value != null)
			{
				int len = value.Length;
				if ((len > 0) && (len < 128))
				{
					return DbSqlStandard.regex_name.IsMatch (value);
				}
			}
			
			return false;
		}
		
		public static bool ValidateString(string value)
		{
			if (value != null)
			{
				int len = value.Length;
				if ((len > 1) && (len < 10000))
				{
					return DbSqlStandard.regex_string.IsMatch (value);
				}
			}
			
			return false;
		}
		
		public static bool ValidateNumber(string value)
		{
			if (value != null)
			{
				int len = value.Length;
				if ((len > 0) && (len < 40))
				{
					return DbSqlStandard.regex_string.IsMatch (value);
				}
			}
			
			return false;
		}
		
		public static bool ValidateQualifiedName(string value)
		{
			if (value != null)
			{
				int len = value.Length;
				if ((len > 0) && (len < 256))
				{
					//	TODO: valider un nom qualifié de type N1.N2 où N1 est un nom
					//	bien formé (pouvant être entre guillemets).
					
					return true;
				}
			}
			
			return false;
		}
		
		
		protected static Regex		regex_name;
		protected static Regex		regex_string;
		protected static Regex		regex_number;
	}
}
