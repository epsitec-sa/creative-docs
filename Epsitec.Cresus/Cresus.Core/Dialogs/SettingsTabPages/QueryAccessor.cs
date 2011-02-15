//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.IO;
using Epsitec.Common.Support;
using Epsitec.Common.Types;
using Epsitec.Common.Widgets;
using Epsitec.Common.Drawing;

using Epsitec.Common.Support.EntityEngine;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Database.Logging;

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;

namespace Epsitec.Cresus.Core.Dialogs.SettingsTabPages
{
	/// <summary>
	/// Classe d'extension pour Epsitec.Cresus.Database.Logging.Query.
	/// </summary>
	public static class QueryAccessor
	{
		public static int Count(this Query query, string search, SearchMode mode)
		{
			if (string.IsNullOrEmpty (search))
			{
				return 0;
			}

			int count = 0;

			foreach (var t in query.GetSearchableStrings (mode))
			{
				string text = t;

				if (!mode.CaseSensitive)
				{
					text = Misc.RemoveAccentsToLower (t);
				}

				count += text.Count (search);
			}

			return count;
		}

		public static bool ContainsString(this Query query, string search, SearchMode mode)
		{
			//	Retourne true si le texte à chercher se trouve dans une ligne donnée.
			foreach (var t in query.GetSearchableStrings (mode))
			{
				string text = t;

				if (!mode.CaseSensitive)
				{
					text = Misc.RemoveAccentsToLower (t);
				}

				if (text.Contains (search))
				{
					return true;
				}
			}

			return false;
		}

		private static IEnumerable<string> GetSearchableStrings(this Query query, SearchMode mode)
		{
			//	Retourne tous les textes où chercher pour une ligne donnée.
			if (mode.SearchInQuery)
			{
				yield return query.SourceCode;
			}

			if (mode.SearchInParameters)
			{
				yield return query.GetCompactParameters ();
			}

			if (mode.SearchInResults)
			{
				yield return query.GetCompactResults ();
			}

			if (mode.SearchInCallStack)
			{
				yield return query.GetStackTrace ();
			}
		}


		public static FormattedText[] GetMainContent(this Query query, int row, string search, SearchMode mode)
		{
			//	Retourne les textes pour peupler les 6 colonnes d'une ligne du tableau principal.
			var values = new List<FormattedText> ();

			if (query == null)
			{
				values.Add ("");
				values.Add ("");
				values.Add ("");  // colonne pour le temps relatif
				values.Add ("");
				values.Add (Misc.Italic ("Données effacées"));
				values.Add ("");
				values.Add ("");
				values.Add ("");
			}
			else
			{
				values.Add (query.Number.ToString ());
				values.Add (query.GetNiceStartTime ());
				values.Add ("");  // colonne pour le temps relatif
				values.Add (query.GetNiceDuration ());
				values.Add (QueryAccessor.GetTaggedText (query.GetQuerySummary (), search, mode, mode.SearchInQuery));
				values.Add (QueryAccessor.GetTaggedText (query.GetCompactParameters (), search, mode, mode.SearchInParameters));
				values.Add (QueryAccessor.GetTaggedText (query.GetCompactResults (), search, mode, mode.SearchInResults));
				values.Add (query.GetNiceThreadName ());
			}

			return values.ToArray ();
		}

		private static string GetCompactParameters(this Query query)
		{
			//	Retourne tous les paramètres sous une forme compacte.
			return string.Join (", ", query.Parameters.Select (x => QueryAccessor.GetString (x.Value)));
		}

		private static string GetCompactResults(this Query query)
		{
			//	Retourne tous les résultats sous une forme compacte.
			if (query.Result == null)
			{
				return "";
			}

			var list = new List<string> ();

			foreach (var table in query.Result.Tables)
			{
				foreach (var row in table.Rows)
				{
					foreach (var value in row.Values)
					{
						if (value != null)
						{
							string s = QueryAccessor.GetString (value);

							if (!string.IsNullOrWhiteSpace (s))
							{
								list.Add (s);
							}
						}
					}
				}
			}

			return string.Join (", ", list);
		}

		public static FormattedText[] GetParameterContent(Parameter parameter, string search, SearchMode mode)
		{
			//	Retourne les textes pour peupler les 2 colonnes d'une ligne du tableau des paramètres.
			var values = new List<FormattedText> ();

			values.Add (parameter.Name);
			values.Add (QueryAccessor.GetTaggedText (QueryAccessor.GetString (parameter.Value), search, mode, mode.SearchInParameters));

			return values.ToArray ();
		}

		public static FormattedText[] GetTableResultsContent(ReadOnlyCollection<object> objects, string search, SearchMode mode)
		{
			//	Retourne les textes pour peupler les colonnes d'une ligne du tableau des résultats.
			var values = new List<FormattedText> ();

			foreach (var obj in objects)
			{
				values.Add (QueryAccessor.GetTaggedText (QueryAccessor.GetString (obj), search, mode, mode.SearchInResults));
			}

			return values.ToArray ();
		}


		public static string GetStackTrace(this Query query)
		{
			if (query.StackTrace == null)
			{
				return "";
			}
			else
			{
				return query.StackTrace.ToString ().Replace ("\r", "");
			}
		}


		public static FormattedText GetQuerySummary(this Query query)
		{
			//	Retourne le texte résumé de la requête SQL.
			//	Pour l'instant, je ne garde que les mots-clés, mais cela pourrait être amélioré.
			var text = query.SourceCode.Replace ("\n", "");

			foreach (var word in QueryAccessor.SqlKeywords)
			{
				int index = 0;
				while (index < text.Length)
				{
					index = text.IndexOf (word, index);

					if (index == -1)
					{
						break;
					}

					if (QueryAccessor.IsSqlWordSeparator (text, index-1) &&
						QueryAccessor.IsSqlWordSeparator (text, index+word.Length))
					{
						string subst = string.Concat ("∆", word, "∆");

						text = text.Remove (index, word.Length);
						text = text.Insert (index, subst);

						index += subst.Length;
					}
					else
					{
						index++;
					}
				}
			}

			var builder = new System.Text.StringBuilder ();

			int i = 0;
			foreach (var c in text)
			{
				if (c == '∆')
				{
					if (i%2 != 0)
					{
						builder.Append (' ');
					}

					i++;
				}
				else
				{
					if (i%2 != 0)
					{
						builder.Append (c);
					}
				}
			}

			return builder.ToString ();
		}

		public static FormattedText GetQuery(this Query query, bool substitution, bool syntaxColorized, bool autoBreak)
		{
			//	Retourne le texte de la requête SQL, avec ou sans substitution des paramètres (en vert)
			//	et coloriage syntaxique (en bleu).
			var text = query.SourceCode.Replace ("\n", "");
			text = TextLayout.ConvertToTaggedText (text);

			if (autoBreak)
			{
				text = QueryAccessor.GetSqlAutoBreakedText (text);
			}

			if (substitution || syntaxColorized)
			{
				foreach (var parameter in query.Parameters.OrderByDescending (p => p.Name.Length))
				{
					if (substitution)
					{
						var value = parameter.Value.ToString ();

						if (!string.IsNullOrEmpty (value))
						{
							text = text.Replace (parameter.Name, Misc.FontColorize (Misc.Bold (value), Color.FromName ("Green")).ToString ());
						}
					}
					else
					{
						text = text.Replace (parameter.Name, Misc.FontColorize (Misc.Bold (parameter.Name), Color.FromName ("Green")).ToString ());
					}
				}

				if (syntaxColorized)
				{
					text = QueryAccessor.GetSqlSyntaxColorizedText (text);
				}
			}

			return text;
		}

		private static string GetSqlAutoBreakedText(string text)
		{
			foreach (var word in QueryAccessor.SqlAutoBreakableWords)
			{
				text = text.Replace (word, string.Concat ("<br/>", word));
			}

			return text;
		}

		private static string GetSqlSyntaxColorizedText(string text)
		{
			//	Colorie en bleu tous les mots-clés SQL dans le texte d'une requête.
			//	Ils doivent être en majuscule !
			foreach (var word in QueryAccessor.SqlKeywords)
			{
				int index = 0;
				while (index < text.Length)
				{
					index = text.IndexOf (word, index);

					if (index == -1)
					{
						break;
					}

					if (QueryAccessor.IsSqlWordSeparator (text, index-1) &&
						QueryAccessor.IsSqlWordSeparator (text, index+word.Length))
					{
						string subst = Misc.FontColorize (Misc.Bold (word), Color.FromName ("Blue")).ToString ();

						text = text.Remove (index, word.Length);
						text = text.Insert (index, subst);

						index += subst.Length;
					}
					else
					{
						index++;
					}
				}
			}

			return text;
		}

		private static bool IsSqlWordSeparator(string text, int index)
		{
			if (index < 0 || index >= text.Length)
			{
				return true;
			}

			return !QueryAccessor.IsSqlWordCharacter (text[index]);
		}

		private static bool IsSqlWordCharacter(char c)
		{
			return (c >= 'A' && c <= 'Z') || (c >= '0' && c <= '9') || c == '_';
		}

		private static IEnumerable<string> SqlAutoBreakableWords
		{
			get
			{
				yield return "FROM";
				yield return "ORDER";
				yield return "SET";
				yield return "VALUES";
				yield return "WHERE";
			}
		}

		private static IEnumerable<string> SqlKeywords
		{
			//	Retourne la liste de tous les mots-clés réservés pour SQL Server 2000.
			//	Source: http://msdn.microsoft.com/en-us/library/aa238507(v=sql.80).aspx
			get
			{
				yield return "ABSOLUTE";
				yield return "ACTION";
				yield return "ADA";
				yield return "ADD";
				yield return "ADMIN";
				yield return "AFTER";
				yield return "AGGREGATE";
				yield return "ALIAS";
				yield return "ALL";
				yield return "ALLOCATE";
				yield return "ALTER";
				yield return "AND";
				yield return "ANY";
				yield return "ARE";
				yield return "ARRAY";
				yield return "AS";
				yield return "ASC";
				yield return "ASSERTION";
				yield return "AT";
				yield return "AUTHORIZATION";
				yield return "AVG";
				yield return "BACKUP";
				yield return "BEFORE";
				yield return "BEGIN";
				yield return "BETWEEN";
				yield return "BINARY";
				yield return "BIT";
				yield return "BIT_LENGTH";
				yield return "BLOB";
				yield return "BOOLEAN";
				yield return "BOTH";
				yield return "BREADTH";
				yield return "BREAK";
				yield return "BROWSE";
				yield return "BULK";
				yield return "BY";
				yield return "CALL";
				yield return "CASCADE";
				yield return "CASCADED";
				yield return "CASE";
				yield return "CAST";
				yield return "CATALOG";
				yield return "CHAR";
				yield return "CHAR_LENGTH";
				yield return "CHARACTER";
				yield return "CHARACTER_LENGTH";
				yield return "CHECK";
				yield return "CHECKPOINT";
				yield return "CLASS";
				yield return "CLOB";
				yield return "CLOSE";
				yield return "CLUSTERED";
				yield return "COALESCE";
				yield return "COLLATE";
				yield return "COLLATION";
				yield return "COLUMN";
				yield return "COMMIT";
				yield return "COMPLETION";
				yield return "COMPUTE";
				yield return "CONNECT";
				yield return "CONNECTION";
				yield return "CONSTRAINT";
				yield return "CONSTRAINTS";
				yield return "CONSTRUCTOR";
				yield return "CONTAINS";
				yield return "CONTAINSTABLE";
				yield return "CONTINUE";
				yield return "CONVERT";
				yield return "CORRESPONDING";
				yield return "COUNT";
				yield return "CREATE";
				yield return "CROSS";
				yield return "CUBE";
				yield return "CURRENT";
				yield return "CURRENT_DATE";
				yield return "CURRENT_PATH";
				yield return "CURRENT_ROLE";
				yield return "CURRENT_TIME";
				yield return "CURRENT_TIMESTAMP";
				yield return "CURRENT_USER";
				yield return "CURSOR";
				yield return "CYCLE";
				yield return "DATA";
				yield return "DATABASE";
				yield return "DATE";
				yield return "DAY";
				yield return "DBCC";
				yield return "DEALLOCATE";
				yield return "DEC";
				yield return "DECIMAL";
				yield return "DECLARE";
				yield return "DEFAULT";
				yield return "DEFERRABLE";
				yield return "DEFERRED";
				yield return "DELETE";
				yield return "DENY";
				yield return "DEPTH";
				yield return "DEREF";
				yield return "DESC";
				yield return "DESCRIBE";
				yield return "DESCRIPTOR";
				yield return "DESTROY";
				yield return "DESTRUCTOR";
				yield return "DETERMINISTIC";
				yield return "DIAGNOSTICS";
				yield return "DICTIONARY";
				yield return "DISCONNECT";
				yield return "DISK";
				yield return "DISTINCT";
				yield return "DISTRIBUTED";
				yield return "DOMAIN";
				yield return "DOUBLE";
				yield return "DROP";
				yield return "DUMMY";
				yield return "DUMP";
				yield return "DYNAMIC";
				yield return "EACH";
				yield return "ELSE";
				yield return "END";
				yield return "END-EXEC";
				yield return "EQUALS";
				yield return "ERRLVL";
				yield return "ESCAPE";
				yield return "EVERY";
				yield return "EXCEPT";
				yield return "EXCEPTION";
				yield return "EXEC";
				yield return "EXECUTE";
				yield return "EXISTS";
				yield return "EXIT";
				yield return "EXTERNAL";
				yield return "EXTRACT";
				yield return "FALSE";
				yield return "FETCH";
				yield return "FILE";
				yield return "FILLFACTOR";
				yield return "FIRST";
				yield return "FLOAT";
				yield return "FOR";
				yield return "FOREIGN";
				yield return "FORTRAN";
				yield return "FOUND";
				yield return "FREE";
				yield return "FREETEXT";
				yield return "FREETEXTTABLE";
				yield return "FROM";
				yield return "FULL";
				yield return "FUNCTION";
				yield return "GENERAL";
				yield return "GET";
				yield return "GLOBAL";
				yield return "GO";
				yield return "GOTO";
				yield return "GRANT";
				yield return "GROUP";
				yield return "GROUPING";
				yield return "HAVING";
				yield return "HOLDLOCK";
				yield return "HOST";
				yield return "HOUR";
				yield return "IDENTITY";
				yield return "IDENTITY_INSERT";
				yield return "IDENTITYCOL";
				yield return "IF";
				yield return "IGNORE";
				yield return "IMMEDIATE";
				yield return "IN";
				yield return "INCLUDE";
				yield return "INDEX";
				yield return "INDICATOR";
				yield return "INITIALIZE";
				yield return "INITIALLY";
				yield return "INNER";
				yield return "INOUT";
				yield return "INPUT";
				yield return "INSENSITIVE";
				yield return "INSERT";
				yield return "INT";
				yield return "INTEGER";
				yield return "INTERSECT";
				yield return "INTERVAL";
				yield return "INTO";
				yield return "IS";
				yield return "ISOLATION";
				yield return "ITERATE";
				yield return "JOIN";
				yield return "KEY";
				yield return "KILL";
				yield return "LANGUAGE";
				yield return "LARGE";
				yield return "LAST";
				yield return "LATERAL";
				yield return "LEADING";
				yield return "LEFT";
				yield return "LESS";
				yield return "LEVEL";
				yield return "LIKE";
				yield return "LIMIT";
				yield return "LINENO";
				yield return "LOAD";
				yield return "LOCAL";
				yield return "LOCALTIME";
				yield return "LOCALTIMESTAMP";
				yield return "LOCATOR";
				yield return "LOWER";
				yield return "MAP";
				yield return "MATCH";
				yield return "MAX";
				yield return "MIN";
				yield return "MINUTE";
				yield return "MODIFIES";
				yield return "MODIFY";
				yield return "MODULE";
				yield return "MONTH";
				yield return "NAMES";
				yield return "NATIONAL";
				yield return "NATURAL";
				yield return "NCHAR";
				yield return "NCLOB";
				yield return "NEW";
				yield return "NEXT";
				yield return "NO";
				yield return "NOCHECK";
				yield return "NONCLUSTERED";
				yield return "NONE";
				yield return "NOT";
				yield return "NULL";
				yield return "NULLIF";
				yield return "NUMERIC";
				yield return "OBJECT";
				yield return "OCTET_LENGTH";
				yield return "OF";
				yield return "OFF";
				yield return "OFFSETS";
				yield return "OLD";
				yield return "ON";
				yield return "ONLY";
				yield return "OPEN";
				yield return "OPENDATASOURCE";
				yield return "OPENQUERY";
				yield return "OPENROWSET";
				yield return "OPENXML";
				yield return "OPERATION";
				yield return "OPTION";
				yield return "OR";
				yield return "ORDER";
				yield return "ORDINALITY";
				yield return "OUT";
				yield return "OUTER";
				yield return "OUTPUT";
				yield return "OVER";
				yield return "OVERLAPS";
				yield return "PAD";
				yield return "PARAMETER";
				yield return "PARAMETERS";
				yield return "PARTIAL";
				yield return "PASCAL";
				yield return "PATH";
				yield return "PERCENT";
				yield return "PLAN";
				yield return "POSITION";
				yield return "POSTFIX";
				yield return "PRECISION";
				yield return "PREFIX";
				yield return "PREORDER";
				yield return "PREPARE";
				yield return "PRESERVE";
				yield return "PRIMARY";
				yield return "PRINT";
				yield return "PRIOR";
				yield return "PRIVILEGES";
				yield return "PROC";
				yield return "PROCEDURE";
				yield return "PUBLIC";
				yield return "RAISERROR";
				yield return "READ";
				yield return "READS";
				yield return "READTEXT";
				yield return "REAL";
				yield return "RECONFIGURE";
				yield return "RECURSIVE";
				yield return "REF";
				yield return "REFERENCES";
				yield return "REFERENCING";
				yield return "RELATIVE";
				yield return "REPLICATION";
				yield return "RESTORE";
				yield return "RESTRICT";
				yield return "RESULT";
				yield return "RETURN";
				yield return "RETURNS";
				yield return "REVOKE";
				yield return "RIGHT";
				yield return "ROLE";
				yield return "ROLLBACK";
				yield return "ROLLUP";
				yield return "ROUTINE";
				yield return "ROW";
				yield return "ROWCOUNT";
				yield return "ROWGUIDCOL";
				yield return "ROWS";
				yield return "RULE";
				yield return "SAVE";
				yield return "SAVEPOINT";
				yield return "SCHEMA";
				yield return "SCOPE";
				yield return "SCROLL";
				yield return "SEARCH";
				yield return "SECOND";
				yield return "SECTION";
				yield return "SELECT";
				yield return "SEQUENCE";
				yield return "SESSION";
				yield return "SESSION_USER";
				yield return "SET";
				yield return "SETS";
				yield return "SETUSER";
				yield return "SHUTDOWN";
				yield return "SIZE";
				yield return "SMALLINT";
				yield return "SOME";
				yield return "SPACE";
				yield return "SPECIFIC";
				yield return "SPECIFICTYPE";
				yield return "SQL";
				yield return "SQLCA";
				yield return "SQLCODE";
				yield return "SQLERROR";
				yield return "SQLEXCEPTION";
				yield return "SQLSTATE";
				yield return "SQLWARNING";
				yield return "START";
				yield return "STATE";
				yield return "STATEMENT";
				yield return "STATIC";
				yield return "STATISTICS";
				yield return "STRUCTURE";
				yield return "SUBSTRING";
				yield return "SUM";
				yield return "SYSTEM_USER";
				yield return "TABLE";
				yield return "TEMPORARY";
				yield return "TERMINATE";
				yield return "TEXTSIZE";
				yield return "THAN";
				yield return "THEN";
				yield return "TIME";
				yield return "TIMESTAMP";
				yield return "TIMEZONE_HOUR";
				yield return "TIMEZONE_MINUTE";
				yield return "TO";
				yield return "TOP";
				yield return "TRAILING";
				yield return "TRAN";
				yield return "TRANSACTION";
				yield return "TRANSLATE";
				yield return "TRANSLATION";
				yield return "TREAT";
				yield return "TRIGGER";
				yield return "TRIM";
				yield return "TRUE";
				yield return "TRUNCATE";
				yield return "TSEQUAL";
				yield return "UNDER";
				yield return "UNION";
				yield return "UNIQUE";
				yield return "UNKNOWN";
				yield return "UNNEST";
				yield return "UPDATE";
				yield return "UPDATETEXT";
				yield return "UPPER";
				yield return "USAGE";
				yield return "USE";
				yield return "USER";
				yield return "USING";
				yield return "VALUE";
				yield return "VALUES";
				yield return "VARCHAR";
				yield return "VARIABLE";
				yield return "VARYING";
				yield return "VIEW";
				yield return "WAITFOR";
				yield return "WHEN";
				yield return "WHENEVER";
				yield return "WHERE";
				yield return "WHILE";
				yield return "WITH";
				yield return "WITHOUT";
				yield return "WORK";
				yield return "WRITE";
				yield return "WRITETEXT";
				yield return "YEAR";
				yield return "ZONE";
			}
		}


		private static int Count(this string text, string pattern)
		{
			//	Retourne le nombre d'occurences de pattern dans text.
			if (string.IsNullOrEmpty (pattern))
			{
				return 0;
			}

			int count = 0;
			int index = 0;

			while (index < text.Length)
			{
				index = text.IndexOf (pattern, index);

				if (index == -1)
				{
					break;
				}

				count++;
				index += pattern.Length;
			}

			return count;
		}


		public static FormattedText GetTaggedText(string text, string search, SearchMode mode, bool searchingEnabled)
		{
			if (string.IsNullOrEmpty (search) || !searchingEnabled)
			{
				return TextLayout.ConvertToTaggedText (text);
			}

			QueryAccessor.taggedText.SetSimpleText (text);

			if (!mode.CaseSensitive)
			{
				text = Misc.RemoveAccentsToLower (text);
			}

			var color = Color.FromName ("Red");
			var tag1 = string.Concat ("<font color=\"#", Color.ToHexa (color), "\"><b>");
			var tag2 = "</b></font>";

			int index = 0;
			while (index < text.Length)
			{
				index = text.IndexOf (search, index);

				if (index == -1)
				{
					break;
				}

				QueryAccessor.taggedText.InsertTags (index, index+search.Length, tag1, tag2);

				index += search.Length;
			}

			return QueryAccessor.taggedText.GetTaggedText ();
		}

		public static FormattedText GetTaggedText(FormattedText formattedText, string search, SearchMode mode, bool searchingEnabled)
		{
			if (string.IsNullOrEmpty (search) || !searchingEnabled)
			{
				return formattedText;
			}

			QueryAccessor.taggedText.SetTaggedText (formattedText);
			string text = QueryAccessor.taggedText.GetSimpleText ();

			if (!mode.CaseSensitive)
			{
				text = Misc.RemoveAccentsToLower (text);
			}

			var color = Color.FromName ("Red");
			var tag1 = string.Concat ("<font color=\"#", Color.ToHexa (color), "\"><b>");
			var tag2 = "</b></font>";

			int index = 0;
			while (index < text.Length)
			{
				index = text.IndexOf (search, index);

				if (index == -1)
				{
					break;
				}

				QueryAccessor.taggedText.InsertTags (index, index+search.Length, tag1, tag2);

				index += search.Length;
			}

			return QueryAccessor.taggedText.GetTaggedText ();
		}

	
		private static string GetString(object value)
		{
			if (value is string)
			{
				return value as string;
			}
			else
			{
				return value.ToString ();
			}
		}

		public static string GetShortStartTime(this Query query)
		{
			//	Retourne le temps sous une forme courte.
			return query.StartTime.ToString ("HH:mm:ss");
		}

		private static string GetNiceStartTime(this Query query)
		{
			//	Retourne le temps sous une jolie forme.
			return query.StartTime.ToString ("HH:mm:ss fff");
		}

		private static string GetNiceDuration(this Query query)
		{
			//	Retourne la durée sous une jolie forme.
			return string.Concat ((query.Duration.Ticks/10).ToString (), " µs");  // un Tick vaut 100 nanosecondes
		}

		private static string GetNiceThreadName(this Query query)
		{
			//	Retourne le nom du processus sous une jolie forme.
			if (string.IsNullOrEmpty (query.ThreadName))
			{
				return "Inconnu";
			}
			else
			{
				return query.ThreadName;
			}
		}


		private static readonly Widgets.TaggedText taggedText = new Widgets.TaggedText ();
	}
}
