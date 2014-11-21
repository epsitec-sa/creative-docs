//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Core.Helpers;
using Epsitec.Cresus.Assets.Data;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.Server.BusinessLogic
{
	public static class LogicDescriptions
	{
		public static string GetTooltip(DataAccessor accessor, DataObject obj, Timestamp timestamp, EventType eventType, int maxLines = int.MaxValue)
		{
			var list = new List<string> ();

			list.Add (LogicDescriptions.GetEventDescription (timestamp, eventType));

			foreach (var field in LogicDescriptions.GetObjectFields (accessor))
			{
				string line = null;

				switch (accessor.GetFieldType (field))
				{
					case FieldType.String:
						line = ObjectProperties.GetObjectPropertyString (obj, timestamp, field, false);
						break;

					case FieldType.Decimal:
						var d = ObjectProperties.GetObjectPropertyDecimal (obj, timestamp, field, false);
						if (d.HasValue)
						{
							switch (Format.GetFieldFormat (field))
							{
								case DecimalFormat.Rate:
									line = TypeConverters.RateToString (d);
									break;

								case DecimalFormat.Amount:
									line = TypeConverters.AmountToString (d);
									break;

								case DecimalFormat.Real:
									line = TypeConverters.DecimalToString (d);
									break;
							}
						}
						break;

					case FieldType.ComputedAmount:
						var ca = ObjectProperties.GetObjectPropertyComputedAmount (obj, timestamp, field, false);
						if (ca.HasValue)
						{
							line = TypeConverters.AmountToString (ca.Value.FinalAmount);
						}
						break;

					case FieldType.AmortizedAmount:
						var aa = ObjectProperties.GetObjectPropertyAmortizedAmount (obj, timestamp, field, false);
						if (aa.HasValue)
						{
							line = TypeConverters.AmountToString (accessor.GetAmortizedAmount (aa));
						}
						break;

					case FieldType.Int:
						var i = ObjectProperties.GetObjectPropertyInt (obj, timestamp, field, false);
						if (i.HasValue)
						{
							line = TypeConverters.IntToString (i);
						}
						break;

					case FieldType.Date:
						var da = ObjectProperties.GetObjectPropertyDate (obj, timestamp, field, false);
						if (da.HasValue)
						{
							line = TypeConverters.DateToString (da);
						}
						break;

					case FieldType.GuidGroup:
						var gg = ObjectProperties.GetObjectPropertyGuid (obj, timestamp, field, false);
						var tg = GroupsLogic.GetShortName (accessor, gg);
						if (!string.IsNullOrEmpty (tg))
						{
							line = tg;
						}
						break;

					case FieldType.GuidPerson:
						var gp = ObjectProperties.GetObjectPropertyGuid (obj, timestamp, field, false);
						var tp = PersonsLogic.GetSummary (accessor, gp);
						if (!string.IsNullOrEmpty (tp))
						{
							line = tp;
						}
						break;

					case FieldType.GuidRatio:
						var gr = ObjectProperties.GetObjectPropertyGuidRatio (obj, timestamp, field, false);
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

					var name = accessor.GetFieldName (field);
					list.Add (LogicDescriptions.GetTooltipLine (name, line));
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
				list.Add (Res.Strings.LogicDescription.Event.Final.ToString ());
			}
			else
			{
				var d = TypeConverters.DateToString (timestamp.Date);

				if (eventType == EventType.Unknown)
				{
					list.Add (string.Format (Res.Strings.LogicDescription.Event.State.ToString (), d));
				}
				else
				{
					list.Add (string.Format (Res.Strings.LogicDescription.Event.Date.ToString (), d));
				}
			}

			//	Met le type de l'événement, s'il est connu.
			var ed = DataDescriptions.GetEventDescription (eventType);
			if (!string.IsNullOrEmpty (ed))
			{
				list.Add (ed);
			}

			return UniversalLogic.NiceJoin (list.ToArray ());
		}


		private static IEnumerable<ObjectField> GetObjectFields(DataAccessor accessor)
		{
			yield return ObjectField.OneShotNumber;
			yield return ObjectField.OneShotUser;
			yield return ObjectField.OneShotDateOperation;
			yield return ObjectField.OneShotComment;
			yield return ObjectField.OneShotDocuments;

			foreach (var userField in AssetsLogic.GetUserFields (accessor))
			{
				yield return userField.Field;
			}

			//?for (int i=0; i<=ObjectField.GroupGuidRatioLast-ObjectField.GroupGuidRatioFirst; i++)
			//?{
			//?	yield return ObjectField.GroupGuidRatioFirst+i;
			//?}

			yield return ObjectField.CategoryName;
			yield return ObjectField.AmortizationMethod;
			yield return ObjectField.AmortizationRate;
			yield return ObjectField.AmortizationYearCount;
			yield return ObjectField.AmortizationType;
			yield return ObjectField.Periodicity;
			yield return ObjectField.Prorata;
			yield return ObjectField.Round;
			yield return ObjectField.ResidualValue;

			foreach (var field in DataAccessor.AccountFields)
			{
				yield return field;
			}
		}
	}
}
