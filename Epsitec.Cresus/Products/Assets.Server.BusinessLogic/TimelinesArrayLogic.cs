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


		public void Update(DataArray dataArray, IObjectsNodeGetter nodeGetter, TimelinesMode mode, System.Func<DataEvent, bool> filter = null)
		{
			//	Met à jour this.dataArray en fonction de l'ensemble des événements de
			//	tous les objets. Cela nécessite d'accéder à l'ensemble des données, ce
			//	qui peut être long. Néanmoins, cela est nécessaire, même si la timeline
			//	n'affiche qu'un nombre limité de lignes. En effet, il faut allouer toutes
			//	les colonnes pour lesquelles il existe un événement.
			dataArray.Clear (nodeGetter.Count, mode);

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

							if (mode == TimelinesMode.Multi)
							{
								if (column[row].IsEmpty)
								{
									column[row] = this.EventToCell (obj, e);
								}
								else
								{
									//	Effectue un merge.
									column[row] = new DataCell(column[row], this.EventToCell (obj, e));
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
							column[row] = new DataCell (column[row].Glyphs, flags, column[row].Tooltip);
						}
					}
				}
				else
				{
					for (int c=0; c<dataArray.ColumnsCount; c++)
					{
						var column = dataArray.GetColumn (c);
						column[row] = new DataCell (column[row].Glyphs, DataCellFlags.Group, null);
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

			public void Clear(int rowsCount, TimelinesMode mode)
			{
				this.rowsCount = rowsCount;
				this.mode = mode;
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
				var adjusted = this.Adjust (timestamp);
				var column = this.columns.Where (x => this.Adjust (x.Timestamp) == adjusted).FirstOrDefault ();

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

			private Timestamp Adjust(Timestamp timestamp)
			{
				if (this.mode == TimelinesMode.Multi)
				{
					var date = new System.DateTime(timestamp.Date.Year, timestamp.Date.Month, 1);
					timestamp = new Timestamp (date, 0);
				}

				return timestamp;
			}

			private readonly List<DataColumn>	columns;
			private readonly List<string>		rowsLabel;
			private int							rowsCount;
			private TimelinesMode				mode;
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
				this.Glyphs = new List<TimelineGlyph> ();
				this.Glyphs.Add (glyph);

				this.Flags   = flags;
				this.Tooltip = tooltip;
			}

			public DataCell(IEnumerable<TimelineGlyph> glyphs, DataCellFlags flags = DataCellFlags.None, string tooltip = null)
			{
				this.Glyphs = new List<TimelineGlyph> ();
				this.Glyphs.AddRange (glyphs);

				this.Flags   = flags;
				this.Tooltip = tooltip;
			}

			public DataCell(DataCell cell1, DataCell cell2)
			{
				this.Glyphs = new List<TimelineGlyph> ();

				if (!cell1.IsEmpty)
				{
					this.Glyphs.AddRange (cell1.Glyphs);
				}

				if (!cell2.IsEmpty)
				{
					this.Glyphs.AddRange (cell2.Glyphs);
				}

				this.Flags = cell1.Flags | cell2.Flags;

				string t1, t2;

				if (cell1.Glyphs.Count == 1)
				{
					t1 = DataCell.GetFirstLine (cell1.Tooltip);
				}
				else
				{
					t1 = cell1.Tooltip;
				}

				t2 = DataCell.GetFirstLine (cell2.Tooltip);

				this.Tooltip = string.Concat (t1, "<br/>", t2);
			}

			public bool IsEmpty
			{
				get
				{
					return this.Glyphs.Count == 1 && this.Glyphs[0] == TimelineGlyph.Empty;
				}
			}

			public static DataCell Empty = new DataCell (TimelineGlyph.Empty);

			private static string GetFirstLine(string text)
			{
				if (!string.IsNullOrEmpty (text))
				{
					var i = text.IndexOf ("<br/>");
					if (i != -1)
					{
						text = text.Substring (0, i);
					}
				}

				return text;
			}

			public readonly List<TimelineGlyph>	Glyphs;
			public readonly DataCellFlags		Flags;
			public readonly string				Tooltip;
		}
		#endregion



		private readonly DataAccessor			accessor;
	}
}