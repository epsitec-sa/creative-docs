using System.Text.RegularExpressions;

namespace Epsitec.Cresus.Database
{
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
		
		
		
		protected static Regex		regex_name;
		protected static Regex		regex_string;
		protected static Regex		regex_number;
	}
}
