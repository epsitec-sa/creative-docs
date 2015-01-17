//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Data;
using Epsitec.Cresus.Assets.Server.NodeGetters;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.Server.BusinessLogic
{
	public class TimelinesArrayLogic
	{
		public TimelinesArrayLogic(DataAccessor accessor)
		{
			this.accessor = accessor;
		}


		public void Update(TimelineArray dataArray, IObjectsNodeGetter nodeGetter, TimelinesMode mode, DateRange groupedExcludeRange, System.Func<DataEvent, bool> filter = null)
		{
			//	Met à jour this.dataArray en fonction de l'ensemble des événements de
			//	tous les objets. Cela nécessite d'accéder à l'ensemble des données, ce
			//	qui peut être long. Néanmoins, cela est nécessaire, même si la timeline
			//	n'affiche qu'un nombre limité de lignes. En effet, il faut allouer toutes
			//	les colonnes pour lesquelles il existe un événement.
			dataArray.Clear (nodeGetter.Count, mode, groupedExcludeRange);

			for (int row=0; row<nodeGetter.Count; row++)
			{
				var node = nodeGetter[row];
				var obj = this.accessor.GetObject (node.BaseType, node.Guid);
				var field = this.accessor.GetMainStringField (node.BaseType);

				var label = ObjectProperties.GetObjectPropertyString (obj, null, field);
				dataArray.RowsLabel.Add (label);

				if (node.BaseType == BaseType.Assets)
				{
					foreach (var e in obj.Events)
					{
						if (filter == null || filter (e))
						{
							var groupedMode = TimelineGroupedMode.None;

							if (TimelinesArrayLogic.IsGrouped (mode) &&
								!groupedExcludeRange.IsInside (e.Timestamp.Date))
							{
								groupedMode = TimelinesArrayLogic.GetGroupedMode (mode);
							}

							//	Retourne la colonne, qui est créée si nécessaire.
							var column = dataArray.GetColumn (e.Timestamp, groupedMode);

							if (TimelinesArrayLogic.IsGrouped (mode))
							{
								if (column[row].IsEmpty)
								{
									column[row] = this.EventToCell (obj, e);
								}
								else
								{
									//	Effectue un merge.
									column[row] = new TimelineCell(column[row], this.EventToCell (obj, e));
								}
							}
							else
							{
								column[row] = this.EventToCell (obj, e);
							}
						}
					}
				}
			}

			//	Marque les intervalles bloqués, qui seront hachurés.
			for (int row=0; row<nodeGetter.Count; row++)
			{
				var node = nodeGetter[row];

				if (node.BaseType == BaseType.Assets)
				{
					var obj = this.accessor.GetObject (node.BaseType, node.Guid);
					var outOfBoundsIntervals = AssetCalculator.GetOutOfBoundsIntervals (obj);
					var lockedTimestamp = AssetCalculator.GetLockedTimestamp (obj);

					for (int c=0; c<dataArray.ColumnsCount; c++)
					{
						var column = dataArray.GetColumn (c);
						var flags = column[row].Flags;

						if (column.Timestamp < lockedTimestamp)
						{
							flags |= DataCellFlags.Locked;
						}

						if (AssetCalculator.IsOutOfBounds (outOfBoundsIntervals, column.Timestamp))
						{
							flags |= DataCellFlags.OutOfBounds;
						}

						if (column[row].Flags != flags)
						{
							column[row] = new TimelineCell (column[row].Glyphs, flags, column[row].Tooltip);
						}
					}
				}
				else
				{
					for (int c=0; c<dataArray.ColumnsCount; c++)
					{
						var column = dataArray.GetColumn (c);
						column[row] = new TimelineCell (column[row].Glyphs, DataCellFlags.Group, null);
					}
				}
			}
		}

		private TimelineCell EventToCell(DataObject obj, DataEvent e)
		{
			var mode  = AssetsLogic.IsAmortizationEnded (obj, e);
			var glyph = TimelineData.TypeToGlyph (e.Type, mode);
			string tooltip = LogicDescriptions.GetTooltip (this.accessor, obj, e.Timestamp, e.Type, 8);

			return new TimelineCell (glyph, DataCellFlags.None, tooltip);
		}


		#region General static helpers
		public static System.DateTime Adjust(System.DateTime date, TimelineGroupedMode mode)
		{
			//	Ajuste une date au début de la période de regroupement.
			switch (mode)
			{
				case TimelineGroupedMode.ByMonth:
					return new System.DateTime (date.Year, date.Month, 1);  // le 1er du mois

				case TimelineGroupedMode.ByTrim:
					int month = (((date.Month-1)/3)*3)+1;  // 1, 4, 7 ou 10
					return new System.DateTime (date.Year, month, 1);  // le 1er du trimestre

				case TimelineGroupedMode.ByYear:
					return new System.DateTime (date.Year, 1, 1);  // le 1er janvier de l'année

				default:
					return date;
			}
		}

		public static bool IsGrouped(TimelinesMode mode)
		{
			return TimelinesArrayLogic.GetGroupedMode (mode) != TimelineGroupedMode.None;
		}

		public static TimelineGroupedMode GetGroupedMode(TimelinesMode mode)
		{
			switch (mode)
			{
				case TimelinesMode.GroupedByMonth:
					return TimelineGroupedMode.ByMonth;

				case TimelinesMode.GroupedByTrim:
					return TimelineGroupedMode.ByTrim;

				case TimelinesMode.GroupedByYear:
					return TimelineGroupedMode.ByYear;

				default:
					return TimelineGroupedMode.None;
			}
		}
		#endregion


		private readonly DataAccessor			accessor;
	}
}