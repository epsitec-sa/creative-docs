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
			
			throw new DbFormatException (string.Format ("Cannot make qualified name from {0} and {1}", qualifier, name));
		}
		
		public static void SplitQualifiedName(string value, out string qualifier, out string name)
		{
			if (DbSqlStandard.ValidateQualifiedName (value))
			{
				//	Si le qualifier est entre guillemets, il peut lui aussi contenir un "."...
				//	Exemple valide "A.B".C où le résultat doit être qualifier="A.B" et name="C".

				string[] tokens;
				int nb = System.Utilities.StringToTokens (value, '.', out tokens);

				if ( nb != 2 )
				{
					throw new DbFormatException (string.Format ("{0} is not a qualified name", value));
				}

				qualifier = tokens [0];
				name      = tokens [1];
				
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
						nb = System.Utilities.StringToTokens (value, '.', out tokens);
					}
					catch
					{
						// en cas d'erreur dans le nombre de guillemets
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
		
		
		protected static Regex		regex_name;
		protected static Regex		regex_string;
		protected static Regex		regex_number;
	}
}
