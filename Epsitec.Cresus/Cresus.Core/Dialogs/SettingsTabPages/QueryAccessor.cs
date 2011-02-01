//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.IO;
using Epsitec.Common.Support;
using Epsitec.Common.Types;
using Epsitec.Common.Widgets;

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
		public static bool ContainsString(this Query query, string search, bool caseSensitive)
		{
			//	Retourne true si le texte à chercher se trouve dans une ligne donnée.
			foreach (var t in query.GetSearchableStrings ())
			{
				string text = t;

				if (!caseSensitive)
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

		private static IEnumerable<string> GetSearchableStrings(this Query query)
		{
			//	Retourne tous les textes où chercher pour une ligne donnée.
			yield return query.SourceCode;
			yield return query.GetCompactParameters ();
			yield return query.GetCompactResults ();
		}


		public static string[] GetStrings(this Query query, int row)
		{
			//	Retourne les textes pour peupler une ligne du tableau supérieur principal.
			var values = new List<string> ();

			values.Add ((row+1).ToString ());
			values.Add (query.StartTime.ToString ());
			values.Add (QueryAccessor.GetNiceDuration (query.Duration));
			values.Add (query.GetQuery ());
			values.Add (query.GetCompactParameters ());
			values.Add (query.GetCompactResults ());

			return values.ToArray ();
		}

		public static string GetCompactParameters(this Query query)
		{
			//	Retourne tous les paramètres sous une forme compacte.
			return string.Join (", ", query.Parameters.Select (x => x.Value));
		}

		public static string GetCompactResults(this Query query)
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
							string s = value.ToString ();

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

		public static string[] GetParameterStrings(Parameter parameter)
		{
			//	Retourne les textes pour peupler une ligne du tableau des paramètres.
			var values = new List<string> ();

			values.Add (parameter.Name);
			values.Add (parameter.Value.ToString ());

			return values.ToArray ();
		}

		public static string[] GetTableResultsStrings(ReadOnlyCollection<object> objects)
		{
			//	Retourne les textes pour peupler une ligne du tableau des résultats.
			var values = new List<string> ();

			foreach (var obj in objects)
			{
				values.Add (obj.ToString ());
			}

			return values.ToArray ();
		}


		public static string GetQuery(this Query query, bool substitution = false)
		{
			//	Retourne le texte de la requête sql, avec ou sans substitution des paramètres.
			var text = query.SourceCode.Replace ("\n", "");

			if (substitution)
			{
				foreach (var parameter in query.Parameters)
				{
					var value = parameter.Value.ToString ();

					if (!string.IsNullOrEmpty (value))
					{
						text = text.Replace (parameter.Name, Misc.Bold (value).ToString ());
					}
				}
			}

			return text;
		}


		private static string GetNiceDuration(System.TimeSpan duration)
		{
			//	Retourne une durée sous une jolie forme.
			return string.Concat ((duration.Ticks/10).ToString (), " µs");  // un Tick vaut 100 nanosecondes
		}
	}
}
