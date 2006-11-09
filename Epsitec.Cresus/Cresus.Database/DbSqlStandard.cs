//	Copyright © 2003-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

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
			
			throw new Exceptions.FormatException (string.Format ("Expected two names: {0} and {1}", a, b));
		}
		
		public static string ConcatStrings(string a, string b)
		{
			if (DbSqlStandard.ValidateString (a) &&
				DbSqlStandard.ValidateString (b))
			{
				return "'" + a.Substring (1, a.Length-2) + b.Substring (1, b.Length-2) + "'";
			}
			
			throw new Exceptions.FormatException (string.Format ("Expected two strings: {0} and {1}", a, b));
		}
		
		
		public static string QuoteString(string value)
		{
			return "'" + value.Replace ("'", "''") + "'";
		}
		
		
		public static string QualifyName(string qualifier, string name)
		{
			if (DbSqlStandard.ValidateName (qualifier) &&
				DbSqlStandard.ValidateName (name))
			{
				return qualifier + "." + name;
			}
			
			throw new Exceptions.FormatException (string.Format ("Cannot make qualified name from {0} and {1}", qualifier, name));
		}
		
		public static void SplitQualifiedName(string value, out string qualifier, out string name)
		{
			if (DbSqlStandard.ValidateQualifiedName (value))
			{
				//	Si le qualifier est entre guillemets, il peut lui aussi contenir un "."...
				//	Exemple valide "A.B".C où le résultat doit être qualifier="A.B" et name="C".

				string[] tokens;
				int nb = Common.Support.Utilities.StringToTokens (value, '.', out tokens);

				if ( nb != 2 )
				{
					throw new Exceptions.FormatException (string.Format ("{0} is not a qualified name", value));
				}

				qualifier = tokens [0];
				name      = tokens [1];
				
				return;
			}
			
			throw new Exceptions.FormatException (string.Format ("{0} is not a qualified name", value));
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
					return DbSqlStandard.regex_number.IsMatch (value);
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
					string[] tokens;
					int		nb;

					//	Valide un nom qualifié de type N1.N2 où N1 est un nom
					//	bien formé (pouvant être entre guillemets).

					try
					{
						nb = Common.Support.Utilities.StringToTokens (value, '.', out tokens);
					}
					catch
					{
						//	en cas d'erreur dans le nombre de guillemets
						return false;
					}

					if ( nb != 2 )
					{
						return false;
					}

					return (ValidateName (tokens [0]) && ValidateName (tokens [1]));
				}
			}
			
			return false;
		}
		
		
		public static string MakeSqlTableName(string name, DbElementCat category, DbKey key)
		{
			//	Crée un nom de table SQL à partir du nom "haut niveau" ainsi que de la catégorie
			//	de la table (les tables de l'utilisateur sont préfixées par "U_" et se terminent
			//	par l'ID de la table, pour permettre d'avoir plusieurs tables avec le même nom
			//	dans la durée de vie de la base).
			
			System.Text.StringBuilder buffer;
			
			switch (category)
			{
				case DbElementCat.Internal:
					if ((DbSqlStandard.ValidateName (name)) &&
						(name.StartsWith ("CR_")))
					{
						return name;
					}
					throw new Exceptions.GenericException (DbAccess.Empty, string.Format ("'{0}' is an invalid internal table name.", name));
				
				case DbElementCat.ManagedUserData:
					buffer = new System.Text.StringBuilder ();
					buffer.Append ("U_");
					break;
				
				default:
					throw new System.NotImplementedException (string.Format ("Support for category {0} is not implemented.", category));
			}
			
			DbSqlStandard.CreateSimpleSqlName (name, buffer);
			
			//	On limite la taille des noms de tables à 30 caractères maximum, car certaines bases
			//	sont assez restrictives en la matière. Et comme on rajoute 18 digits (au plus), il
			//	ne reste plus que 12 caractères effectifs pour le nom :
			
			if (buffer.Length > 30-18)
			{
				buffer.Length = 30-18;
			}
			
			buffer.AppendFormat (System.Globalization.CultureInfo.InvariantCulture, "_{0}", key.Id.Value);
			
			return buffer.ToString ();
		}
		
		public static string MakeSimpleSqlName(string name)
		{
			//	Simplifie le nom passé en entrée pour ne conserver plus que des
			//	majuscules, des chiffres et le "_". Les caractères non reconnus
			//	sont remplacés par des "_".
			
			//	En début de nom, ni "_", ni un chiffre n'est permis; ces caractères
			//	sont simplement supprimés.
			
			System.Text.StringBuilder buffer = new System.Text.StringBuilder ();
			DbSqlStandard.CreateSimpleSqlName (name, buffer);
			return buffer.ToString ();
		}
		
		public static string MakeSimpleSqlName(string name, string prefix, string suffix)
		{
			System.Text.StringBuilder buffer = new System.Text.StringBuilder ();
			
			buffer.Append (prefix);
			buffer.Append ('_');
			DbSqlStandard.CreateSimpleSqlName (name, buffer);
			buffer.Append ('_');
			buffer.Append (suffix);
			
			return buffer.ToString ();
		}
		
		public static string MakeSimpleSqlName(string name, DbElementCat category)
		{
			System.Text.StringBuilder buffer;
			
			switch (category)
			{
				case DbElementCat.Internal:
					if ((DbSqlStandard.ValidateName (name)) &&
						((name.StartsWith ("CR_") || name.StartsWith ("CREF_"))))
					{
						return name;
					}
					throw new Exceptions.GenericException (DbAccess.Empty, string.Format ("'{0}' is an invalid internal name.", name));
				
				case DbElementCat.ManagedUserData:
					buffer = new System.Text.StringBuilder ();
					buffer.Append ("U_");
					break;
				
				default:
					throw new System.NotImplementedException (string.Format ("Support for category {0} is not implemented.", category));
			}
			
			DbSqlStandard.CreateSimpleSqlName (name, buffer);
			return buffer.ToString ();
		}
		
		
		public static string MakeDelimitedIdentifier(string name)
		{
			bool ok  = true;
			bool bad = false;
			
			for (int i = 0; i < name.Length; i++)
			{
				char c = name[i];
				
				if (((c >= 'a') && (c <= 'z')) ||
					((c >= 'A') && (c <= 'Z')) ||
					((c == '_')))
				{
					continue;
				}
				if ((i > 0) &&
					(c >= '0') && (c <= '9'))
				{
					continue;
				}
				if (c == '"')
				{
					bad = true;
				}
				ok = false;
			}
			
			if (ok)
			{
				return name;
			}
			
			if (bad)
			{
				throw new Exceptions.FormatException (string.Format ("Expected valid name: {0}", name));
			}
			
			return string.Concat (@"""", name, @"""");
		}
		
		
		protected static void CreateSimpleSqlName(string name, System.Text.StringBuilder buffer)
		{
			for (int i = 0; i < name.Length; i++)
			{
				char c = name[i];
				
				if ((c >= 'a') && (c <= 'z'))
				{
					buffer.Append ((char)(c + 'A' - 'a'));
					continue;
				}
				
				if ((c >= 'A') && (c <= 'Z'))
				{
					buffer.Append (c);
					continue;
				}
				
				if ((c >= '0') && (c <= '9'))
				{
					if (buffer.Length > 0)
					{
						buffer.Append (c);
					}
					continue;
				}
				
				//	Remplace les caractères inconnus par des "_".
				
				if (buffer.Length > 0)
				{
					buffer.Append ('_');
				}
			}
		}
		
		
		protected static Regex					regex_name;
		protected static Regex					regex_string;
		protected static Regex					regex_number;
	}
}
