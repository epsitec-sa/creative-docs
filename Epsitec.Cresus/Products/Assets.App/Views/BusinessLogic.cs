//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Server.NaiveEngine;

namespace Epsitec.Cresus.Assets.App.Views
{
	public class BusinessLogic
	{
		public BusinessLogic(DataAccessor accessor)
		{
			this.accessor = accessor;
		}


		public void GeneratesAmortissementsAuto()
		{
			int count = this.accessor.ObjectsCount;
			for (int i=0; i<count; i++)
			{
				var guid = this.accessor.GetObjectGuid (i);
				this.GeneratesAmortissementsAuto (guid);
			}
		}

		public void GeneratesAmortissementsAuto(Guid objectGuid)
		{
			//	TODO: ...
			this.accessor.RemoveAmortissementsAuto (objectGuid);

			this.CreateEvent (objectGuid, new System.DateTime (2013, 3, 1));
			this.CreateEvent (objectGuid, new System.DateTime (2013, 6, 1));
			this.CreateEvent (objectGuid, new System.DateTime (2013, 9, 1));
		}

		private void CreateEvent(Guid objectGuid, System.DateTime date)
		{
			//	TODO: ...
			var timestamp = this.accessor.CreateObjectEvent (objectGuid, date, EventType.AmortissementAuto);

			if (timestamp.HasValue)
			{
				var v = new ComputedAmount(123.0m);
				var p = new DataComputedAmountProperty((int) ObjectField.Valeur1, v);

				this.accessor.AddObjectEventProperty(objectGuid, timestamp.Value, p);
			}
		}


		public static string GetTooltip(Timestamp timestamp, EventType eventType, IEnumerable<AbstractDataProperty> properties, int maxLines = int.MaxValue)
		{
			var list = new List<string> ();

			list.Add (BusinessLogic.GetEventDescription (timestamp, eventType));

			foreach (var field in DataAccessor.ObjectFields)
			{
				if (field == ObjectField.Level)
				{
					continue;
				}

				string line = null;

				switch (DataAccessor.GetFieldType (field))
				{
					case FieldType.String:
						line = DataAccessor.GetStringProperty (properties, (int) field);
						break;

					case FieldType.Decimal:
						var d = DataAccessor.GetDecimalProperty (properties, (int) field);
						if (d.HasValue)
						{
							switch (Format.GetFieldFormat (field))
							{
								case DecimalFormat.Rate:
									line = Helpers.Converters.RateToString (d);
									break;

								case DecimalFormat.Amount:
									line = Helpers.Converters.AmountToString (d);
									break;

								case DecimalFormat.Real:
									line = Helpers.Converters.DecimalToString (d);
									break;
							}
						}
						break;

					case FieldType.ComputedAmount:
						var ca = DataAccessor.GetComputedAmountProperty (properties, (int) field);
						if (ca.HasValue)
						{
							line = Helpers.Converters.AmountToString (ca.Value.FinalAmount);
						}
						break;

					case FieldType.Int:
						var i = DataAccessor.GetIntProperty (properties, (int) field);
						if (i.HasValue)
						{
							line = Helpers.Converters.IntToString (i);
						}
						break;

					case FieldType.Date:
						var da = DataAccessor.GetDateProperty (properties, (int) field);
						if (da.HasValue)
						{
							line = Helpers.Converters.DateToString (da);
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

					var desc = StaticDescriptions.GetObjectFieldDescription (field);
					list.Add (BusinessLogic.GetTooltipLine (desc, line));
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
			if (timestamp.Date != System.DateTime.MaxValue)
			{
				var d = Helpers.Converters.DateToString (timestamp.Date);

				if (eventType == EventType.Unknown)
				{
					list.Add (d);
				}
				else
				{
					list.Add ("Evénement du " + d);
				}
			}

			//	Met le type de l'événement, s'il est connu.
			var ed = StaticDescriptions.GetEventDescription (eventType);
			if (!string.IsNullOrEmpty (ed))
			{
				list.Add (ed);
			}

			return string.Join (" — ", list);
		}


		private readonly DataAccessor accessor;
	}
}
