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
						line = AssetCalculator.GetObjectPropertyString (obj, timestamp, field, false);
						break;

					case FieldType.Decimal:
						var d = AssetCalculator.GetObjectPropertyDecimal (obj, timestamp, field, false);
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
						var ca = AssetCalculator.GetObjectPropertyComputedAmount (obj, timestamp, field, false);
						if (ca.HasValue)
						{
							line = Helpers.TypeConverters.AmountToString (ca.Value.FinalAmount);
						}
						break;

					case FieldType.Int:
						var i = AssetCalculator.GetObjectPropertyInt (obj, timestamp, field, false);
						if (i.HasValue)
						{
							line = Helpers.TypeConverters.IntToString (i);
						}
						break;

					case FieldType.Date:
						var da = AssetCalculator.GetObjectPropertyDate (obj, timestamp, field, false);
						if (da.HasValue)
						{
							line = Helpers.TypeConverters.DateToString (da);
						}
						break;

					case FieldType.GuidGroup:
						var gg = AssetCalculator.GetObjectPropertyGuid (obj, timestamp, field, false);
						var tg = GroupsLogic.GetShortName (accessor, gg);
						if (!string.IsNullOrEmpty (tg))
						{
							line = tg;
						}
						break;

					case FieldType.GuidPerson:
						var gp = AssetCalculator.GetObjectPropertyGuid (obj, timestamp, field, false);
						var tp = PersonsLogic.GetShortName (accessor, gp);
						if (!string.IsNullOrEmpty (tp))
						{
							line = tp;
						}
						break;

					case FieldType.GuidRatio:
						var gr = AssetCalculator.GetObjectPropertyGuidRatio (obj, timestamp, field, false);
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
				yield return ObjectField.OneShotNumber;
				yield return ObjectField.OneShotDateOperation;
				yield return ObjectField.OneShotComment;
				yield return ObjectField.OneShotDocuments;

				yield return ObjectField.GroupParent;
				yield return ObjectField.Number;
				yield return ObjectField.Name;
				yield return ObjectField.Description;
				yield return ObjectField.MainValue;
				yield return ObjectField.Value1;
				yield return ObjectField.Value2;
				yield return ObjectField.Value3;
				yield return ObjectField.Value4;
				yield return ObjectField.Value5;
				yield return ObjectField.Value6;
				yield return ObjectField.Value7;
				yield return ObjectField.Value8;
				yield return ObjectField.Value9;
				yield return ObjectField.Value10;
				yield return ObjectField.Maintenance;
				yield return ObjectField.Color;
				yield return ObjectField.SerialNumber;
				yield return ObjectField.Person1;
				yield return ObjectField.Person2;
				yield return ObjectField.Person3;
				yield return ObjectField.Person4;
				yield return ObjectField.Person5;

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

				yield return ObjectField.CategoryName;

				yield return ObjectField.AmortizationRate;
				yield return ObjectField.AmortizationType;
				yield return ObjectField.Periodicity;
				yield return ObjectField.Prorata;
				yield return ObjectField.Round;
				yield return ObjectField.ResidualValue;

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
