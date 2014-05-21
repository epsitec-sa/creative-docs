﻿//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
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


		public void Update(DataArray dataArray, IObjectsNodeGetter nodeGetter, System.Func<DataEvent, bool> filter = null)
		{
			//	Met à jour this.dataArray en fonction de l'ensemble des événements de
			//	tous les objets. Cela nécessite d'accéder à l'ensemble des données, ce
			//	qui peut être long. Néanmoins, cela est nécessaire, même si la timeline
			//	n'affiche qu'un nombre limité de lignes. En effet, il faut allouer toutes
			//	les colonnes pour lesquelles il existe un événement.
			dataArray.Clear (nodeGetter.Count);

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
							var column = dataArray.GetColumn (e.Timestamp);
							column[row] = this.EventToCell (obj, e);
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
							column[row] = new DataCell (column[row].Glyph, flags, column[row].Tooltip);
						}
					}
				}
				else
				{
					for (int c=0; c<dataArray.ColumnsCount; c++)
					{
						var column = dataArray.GetColumn (c);
						column[row] = new DataCell (column[row].Glyph, DataCellFlags.Group, null);
					}
				}
			}
		}

		private DataCell EventToCell(DataObject obj, DataEvent e)
		{
			var glyph      = TimelineData.TypeToGlyph (e.Type);
			string tooltip = LogicDescriptions.GetTooltip (this.accessor, obj, e.Timestamp, e.Type, 8);

			return new DataCell (glyph, DataCellFlags.None, tooltip);
		}


		#region Data manager
		/// <summary>
		/// Tableau des cellules. Il y a une colonne par Timestamp existant et
		/// une ligne par objet.
		/// </summary>
		public class DataArray
		{
			public DataArray()
			{
				this.columns = new List<DataColumn> ();
				this.rowsLabel = new List<string> ();
			}

			public List<string> RowsLabel
			{
				get
				{
					return this.rowsLabel;
				}
			}

			public int RowsCount
			{
				get
				{
					return this.rowsCount;
				}
			}

			public int ColumnsCount
			{
				get
				{
					return this.columns.Count;
				}
			}

			public DataCell GetCell(int row, int column)
			{
				if (column < this.ColumnsCount && row < this.RowsCount)
				{
					return this.columns[column][row];
				}
				else
				{
					return DataCell.Empty;
				}
			}

			public void Clear(int rowsCount)
			{
				this.rowsCount = rowsCount;
				this.columns.Clear ();
			}

			public int FindColumnIndex(Timestamp timestamp)
			{
				return this.columns.FindIndex (x => x.Timestamp == timestamp);
			}

			public IEnumerable<DataColumn> Columns
			{
				get
				{
					return this.columns;
				}
			}

			public DataColumn GetColumn(int column)
			{
				if (column >= 0 && column < this.columns.Count)
				{
					return this.columns[column];
				}
				else
				{
					return null;
				}
			}

			public DataColumn GetColumn(Timestamp timestamp)
			{
				//	Retourne la colonne à utiliser pour un Timestamp donné.
				//	Si elle n'existe pas, elle est créée.
				var column = this.columns.Where (x => x.Timestamp == timestamp).FirstOrDefault ();

				if (column == null)
				{
					column = new DataColumn (this.rowsCount, timestamp);

					//	Les colonnes sont triées chronologiquement. Il faut donc insérer
					//	la nouvelle colonne à la bonne place.
					int i = this.columns.Where (x => x.Timestamp < timestamp).Count ();
					this.columns.Insert (i, column);
				}

				return column;
			}

			private readonly List<DataColumn> columns;
			private readonly List<string> rowsLabel;
			private int rowsCount;
		}

		/// <summary>
		/// Une colonne pour un Timestamp donné, avec une ligne par objet.
		/// </summary>
		public class DataColumn
		{
			public DataColumn(int rowsCount, Timestamp timestamp)
			{
				this.Timestamp = timestamp;

				this.cells = new DataCell[rowsCount];

				for (int i=0; i<rowsCount; i++)
				{
					this.cells[i] = DataCell.Empty;
				}
			}

			public readonly Timestamp Timestamp;

			public DataCell this[int index]
			{
				get
				{
					return this.cells[index];
				}
				set
				{
					this.cells[index] = value;
				}
			}

			private readonly DataCell[] cells;
		}

		/// <summary>
		/// Une cellule, correspondant à un événement d'un objet.
		/// </summary>
		public class DataCell
		{
			public DataCell(TimelineGlyph glyph, DataCellFlags flags = DataCellFlags.None, string tooltip = null)
			{
				this.Glyph   = glyph;
				this.Flags   = flags;
				this.Tooltip = tooltip;
			}

			public bool IsEmpty
			{
				get
				{
					return this.Glyph == TimelineGlyph.Empty;
				}
			}

			public static DataCell Empty = new DataCell (TimelineGlyph.Empty);

			public readonly TimelineGlyph	Glyph;
			public readonly DataCellFlags	Flags;
			public readonly string			Tooltip;
		}
		#endregion



		private readonly DataAccessor			accessor;
	}
}