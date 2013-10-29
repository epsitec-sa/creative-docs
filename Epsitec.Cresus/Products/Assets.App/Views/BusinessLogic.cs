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
			//	Première ébauche totalement naïve et fausse !
			//	TODO: ...
			this.accessor.RemoveAmortissementsAuto (objectGuid);

			System.DateTime? date1, date2;
			decimal? taux, rest;
			string type;
			int? freq;
			if (!this.GetAmortissement (objectGuid, out date1, out date2, out taux, out type, out freq, out rest))
			{
				return;
			}

			for (int j=0; j<100; j++)
			{
				System.DateTime date;

				if (j == 0)
				{
					if (!date1.HasValue)
					{
						continue;
					}
					date = date1.Value;
				}
				else
				{
					var d = date2.Value.Day;
					var m = date2.Value.Month;
					var y = date2.Value.Year;

					m += (j-1)*freq.Value;

					while (m > 12)
					{
						m -=12;
						y++;
					}

					date = new System.DateTime (y, m, d);
				}

				var currentValues = this.GetValeur (objectGuid, date);
				var newValues = new List<decimal?> ();

				for (int i=0; i<3; i++)
				{
					var v = currentValues[i].GetValueOrDefault (0);

					v -= v*taux.Value;

					if (v < rest.Value)
					{
						newValues.Add (null);
					}
					else
					{
						newValues.Add (v);
					}
				}

				this.CreateAmortissementAuto (objectGuid, date, currentValues, newValues);
			}
		}

		private bool GetAmortissement(Guid objectGuid,
			out System.DateTime? date1, out System.DateTime? date2,
			out decimal? taux, out string type, out int? freq, out decimal? rest)
		{
			date1 = null;
			date2 = null;
			taux = null;
			type = null;
			freq = null;
			rest = null;

			var properties = this.accessor.GetObjectSyntheticProperties (objectGuid, null);

			if (properties != null)
			{
				date1 = DataAccessor.GetDateProperty    (properties, (int) ObjectField.DateAmortissement1);
				date2 = DataAccessor.GetDateProperty    (properties, (int) ObjectField.DateAmortissement2);
				taux  = DataAccessor.GetDecimalProperty (properties, (int) ObjectField.TauxAmortissement);
				type  = DataAccessor.GetStringProperty  (properties, (int) ObjectField.TypeAmortissement);
				freq  = DataAccessor.GetIntProperty     (properties, (int) ObjectField.FréquenceAmortissement);
				rest  = DataAccessor.GetDecimalProperty (properties, (int) ObjectField.ValeurRésiduelle);
			}

			if (!date2.HasValue && date1.HasValue)
			{
				date2 = date1;
				date1 = null;
			}

			if (string.IsNullOrEmpty (type))
			{
				type = "Linéaire";
			}

			if (!rest.HasValue)
			{
				rest = 1.0m;
			}

			if (!freq.HasValue)
			{
				freq = 1;
			}

			return (date2.HasValue && taux.HasValue);
		}

		private List<decimal?> GetValeur(Guid objectGuid, System.DateTime date)
		{
			var list = new List<decimal?> ();

			var timestamp = new Timestamp(date, 0);
			var properties = this.accessor.GetObjectSyntheticProperties (objectGuid, timestamp);

			if (properties != null)
			{
				for (int i=0; i<3; i++)  // Valeur1..3
				{
					ComputedAmount? m = null;
					switch (i)
					{
						case 0:
							m = DataAccessor.GetComputedAmountProperty (properties, (int) ObjectField.Valeur1);
							break;

						case 1:
							m = DataAccessor.GetComputedAmountProperty (properties, (int) ObjectField.Valeur2);
							break;

						case 2:
							m = DataAccessor.GetComputedAmountProperty (properties, (int) ObjectField.Valeur3);
							break;
					}

					if (m.HasValue)
					{
						list.Add (m.Value.FinalAmount);
					}
					else
					{
						list.Add (null);
					}
				}
			}

			return list;
		}

		private void CreateAmortissementAuto(Guid objectGuid, System.DateTime date, List<decimal?> currentValues, List<decimal?> newValues)
		{
			var timestamp = this.accessor.CreateObjectEvent (objectGuid, date, EventType.AmortissementAuto);

			if (timestamp.HasValue)
			{
				for (int i=0; i<3; i++)  // Valeur1..3
				{
					if (newValues[i].HasValue)
					{
						var v = new ComputedAmount (currentValues[i].GetValueOrDefault (0), newValues[i].GetValueOrDefault (0), true);
						DataComputedAmountProperty p = null;

						switch (i)
						{
							case 0:
								p = new DataComputedAmountProperty ((int) ObjectField.Valeur1, v);
								break;

							case 1:
								p = new DataComputedAmountProperty ((int) ObjectField.Valeur2, v);
								break;

							case 2:
								p = new DataComputedAmountProperty ((int) ObjectField.Valeur3, v);
								break;
						}

						if (p != null)
						{
							this.accessor.AddObjectEventProperty (objectGuid, timestamp.Value, p);
						}
					}
				}
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
			if (timestamp.Date == System.DateTime.MaxValue)
			{
				list.Add ("Etat final");
			}
			else
			{
				var d = Helpers.Converters.DateToString (timestamp.Date);

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
