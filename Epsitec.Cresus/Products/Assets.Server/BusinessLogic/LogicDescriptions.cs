//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.Server.BusinessLogic
{
	public static class LogicDescriptions
	{
		public static string GetTooltip(DataAccessor accessor, DataObject obj, Timestamp timestamp, EventType eventType, int maxLines = int.MaxValue)
		{
			var list = new List<string> ();

			list.Add (LogicDescriptions.GetEventDescription (timestamp, eventType));

			foreach (var field in LogicDescriptions.ObjectFields)
			{
				string line = null;

				switch (DataAccessor.GetFieldType (field))
				{
					case FieldType.String:
						line = ObjectCalculator.GetObjectPropertyString (obj, timestamp, field, false);
						break;

					case FieldType.Decimal:
						var d = ObjectCalculator.GetObjectPropertyDecimal (obj, timestamp, field, false);
						if (d.HasValue)
						{
							switch (Format.GetFieldFormat (field))
							{
								case DecimalFormat.Rate:
									line = Helpers.TypeConverters.RateToString (d);
									break;

								case DecimalFormat.Amount:
									line = Helpers.TypeConverters.AmountToString (d);
									break;

								case DecimalFormat.Real:
									line = Helpers.TypeConverters.DecimalToString (d);
									break;
							}
						}
						break;

					case FieldType.ComputedAmount:
						var ca = ObjectCalculator.GetObjectPropertyComputedAmount (obj, timestamp, field, false);
						if (ca.HasValue)
						{
							line = Helpers.TypeConverters.AmountToString (ca.Value.FinalAmount);
						}
						break;

					case FieldType.Int:
						var i = ObjectCalculator.GetObjectPropertyInt (obj, timestamp, field, false);
						if (i.HasValue)
						{
							line = Helpers.TypeConverters.IntToString (i);
						}
						break;

					case FieldType.Date:
						var da = ObjectCalculator.GetObjectPropertyDate (obj, timestamp, field, false);
						if (da.HasValue)
						{
							line = Helpers.TypeConverters.DateToString (da);
						}
						break;

					case FieldType.GuidGroup:
						var gg = ObjectCalculator.GetObjectPropertyGuid (obj, timestamp, field, false);
						var tg = GroupsLogic.GetShortName (accessor, gg);
						if (!string.IsNullOrEmpty (tg))
						{
							line = tg;
						}
						break;

					case FieldType.GuidPerson:
						var gp = ObjectCalculator.GetObjectPropertyGuid (obj, timestamp, field, false);
						var tp = PersonsLogic.GetShortName (accessor, gp);
						if (!string.IsNullOrEmpty (tp))
						{
							line = tp;
						}
						break;

					case FieldType.GuidRatio:
						var gr = ObjectCalculator.GetObjectPropertyGuidRatio (obj, timestamp, field, false);
						var tr = GroupsLogic.GetShortName (accessor, gr);
						if (!string.IsNullOrEmpty (tr))
						{
							line = tr;
						}
						break;
				}

				if (!string.IsNullOrEmpty (line))
				{
					if (list.Count >= maxLines)
					{
						list.Add ("...");
						break;
					}

					var desc = DataDescriptions.GetObjectFieldDescription (field);
					list.Add (LogicDescriptions.GetTooltipLine (desc, line));
				}
			}

			return string.Join ("<br/>", list);
		}

		private static string GetTooltipLine(string desc, string text, int maxLength = 50)
		{
			if (!string.IsNullOrEmpty (text))
			{
				text = text.Replace ("<br/>", ", ");

				if (text.Length > maxLength)
				{
					text = text.Substring (0, maxLength) + "...";
				}

				return string.Concat (desc, " :   ", text);
			}

			return null;
		}

		public static string GetEventDescription(Timestamp timestamp, EventType eventType)
		{
			//	Retourne un texte décrivant l'événement, composé de la date
			//	et du type de l'événement.
			//	Par exemple "Evénement du 31.03.2014 — Amortissement"
			var list = new List<string> ();

			//	Met la date de l'événement, si elle est connue.
			if (timestamp == Timestamp.MaxValue)
			{
				list.Add ("Etat final");
			}
			else
			{
				var d = Helpers.TypeConverters.DateToString (timestamp.Date);

				if (eventType == EventType.Unknown)
				{
					list.Add ("Etat au " + d);
				}
				else
				{
					list.Add ("Evénement du " + d);
				}
			}

			//	Met le type de l'événement, s'il est connu.
			var ed = DataDescriptions.GetEventDescription (eventType);
			if (!string.IsNullOrEmpty (ed))
			{
				list.Add (ed);
			}

			return string.Join (" — ", list);
		}


		private static IEnumerable<ObjectField> ObjectFields
		{
			get
			{
				yield return ObjectField.OneShotNuméro;
				yield return ObjectField.OneShotDateOpération;
				yield return ObjectField.OneShotCommentaire;
				yield return ObjectField.OneShotDocuments;

				yield return ObjectField.Parent;
				yield return ObjectField.Numéro;
				yield return ObjectField.Nom;
				yield return ObjectField.Description;
				yield return ObjectField.ValeurComptable;
				yield return ObjectField.Valeur1;
				yield return ObjectField.Valeur2;
				yield return ObjectField.Valeur3;
				yield return ObjectField.Valeur4;
				yield return ObjectField.Valeur5;
				yield return ObjectField.Valeur6;
				yield return ObjectField.Valeur7;
				yield return ObjectField.Valeur8;
				yield return ObjectField.Valeur9;
				yield return ObjectField.Valeur10;
				yield return ObjectField.Maintenance;
				yield return ObjectField.Couleur;
				yield return ObjectField.NuméroSérie;
				yield return ObjectField.Personne1;
				yield return ObjectField.Personne2;
				yield return ObjectField.Personne3;
				yield return ObjectField.Personne4;
				yield return ObjectField.Personne5;

				yield return ObjectField.GroupGuidRatio+0;
				yield return ObjectField.GroupGuidRatio+1;
				yield return ObjectField.GroupGuidRatio+2;
				yield return ObjectField.GroupGuidRatio+3;
				yield return ObjectField.GroupGuidRatio+4;
				yield return ObjectField.GroupGuidRatio+5;
				yield return ObjectField.GroupGuidRatio+6;
				yield return ObjectField.GroupGuidRatio+7;
				yield return ObjectField.GroupGuidRatio+8;
				yield return ObjectField.GroupGuidRatio+9;

				yield return ObjectField.NomCatégorie;

				yield return ObjectField.TauxAmortissement;
				yield return ObjectField.TypeAmortissement;
				yield return ObjectField.Périodicité;
				yield return ObjectField.ValeurRésiduelle;

				yield return ObjectField.Compte1;
				yield return ObjectField.Compte2;
				yield return ObjectField.Compte3;
				yield return ObjectField.Compte4;
				yield return ObjectField.Compte5;
				yield return ObjectField.Compte6;
				yield return ObjectField.Compte7;
				yield return ObjectField.Compte8;
			}
		}
	}
}
