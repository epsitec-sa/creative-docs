//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.Server.NaiveEngine;

namespace Epsitec.Cresus.Assets.App.Views
{
	public class ObjectSummaryController
	{
		public ObjectSummaryController(DataAccessor accessor, List<string> columnsTitle, List<List<int>> fields)
		{
			this.accessor     = accessor;
			this.columnsTitle = columnsTitle;
			this.fields       = fields;

			this.controller = new SummaryController ();

			this.controller.TileClicked += delegate (object sender, int row, int column)
			{
				this.OnTileClicked (row, column);
			};
		}


		public void CreateUI(Widget parent)
		{
			this.informations = new StaticText
			{
				Parent          = parent,
				Dock            = DockStyle.Top,
				PreferredHeight = 30,
				Margins         = new Margins (0, 0, 0, 20),
			};

			this.header = new FrameBox
			{
				Parent          = parent,
				Dock            = DockStyle.Top,
				PreferredHeight = 17,
			};

			this.CreateHeader ();

			this.controller.CreateUI (parent);
		}

		private void CreateHeader()
		{
			foreach (var title in this.columnsTitle)
			{
				new StaticText
				{
					Parent           = this.header,
					Text             = title,
					ContentAlignment = ContentAlignment.MiddleCenter,
					Dock             = DockStyle.Left,
					PreferredSize    = new Size (120, 20),
				};
			}
		}

		public void UpdateFields(Guid objectGuid, Timestamp? timestamp)
		{
			this.obj       = this.accessor.GetObject (objectGuid);
			this.timestamp = timestamp;
			this.hasEvent  = false;
			this.eventType = EventType.Unknown;

			if (!objectGuid.IsEmpty && this.obj != null)
			{
				var e = this.obj.GetEvent (this.timestamp.Value);

				if (e != null)
				{
					this.eventType = e.Type;
					this.hasEvent  = true;
				}
			}

			this.UpdateInformations ();

			var cells = new List<List<SummaryControllerTile?>> ();

			int columnsCount = this.ColumnsCount;
			int rowsCount    = this.RowsCount;

			for (int column = 0; column < columnsCount; column++)
			{
				var columns = new List<SummaryControllerTile?> ();

				for (int row = 0; row < rowsCount; row++)
				{
					var field = this.GetField (column, row);
					var cell = this.GetCell (field);
					columns.Add (cell);
				}

				cells.Add (columns);
			}

			this.controller.SetTiles (cells);
		}


		private void UpdateInformations()
		{
			this.informations.Text = string.Concat (this.Informations1, "<br/>", this.Informations2);
		}

		private string Informations1
		{
			get
			{
				if (this.hasEvent && this.obj != null)
				{
					var s1 = StaticDescriptions.GetEventDescription (this.eventType);
					var s2 = this.SinglePropertiesCount;

					if (s2 <= 1)
					{
						return string.Format ("Cet événement de type \"{0}\" définit {1} champ.", s1, s2.ToString ());
					}
					else
					{
						return string.Format ("Cet événement de type \"{0}\" définit {1} champs.", s1, s2.ToString ());
					}
				}
				else if (!ObjectSummaryController.IsDefined (this.timestamp))
				{
					return "Il n'y a pas de date sélectionnée.";
				}
				else
				{
					return "Il n'y a pas d'événement à cette date.";
				}
			}
		}

		private string Informations2
		{
			get
			{
				if (ObjectSummaryController.IsDefined (this.timestamp))
				{
					string d = Helpers.Converters.DateToString (this.timestamp.Value.Date);
					return string.Format ("Le tableau ci-dessous montre l'état de l'objet en date du {0} :", d);
				}
				else
				{
					return "Le tableau ci-dessous montre l'état final de l'objet :";
				}
			}
		}

		private static bool IsDefined(Timestamp? timestamp)
		{
			return timestamp != null && timestamp.Value.Date != System.DateTime.MaxValue;
		}


		private SummaryControllerTile? GetCell(int? field)
		{
			if (!field.HasValue || this.obj == null)
			{
				return null;
			}

			string text = null;
			var alignment = ContentAlignment.MiddleCenter;

			switch (DataAccessor.GetFieldType ((ObjectField) field.Value))
			{
				case FieldType.Decimal:
					var d = ObjectCalculator.GetObjectPropertyDecimal (this.obj, this.timestamp, (ObjectField) field.Value);
					if (d.HasValue)
					{
						switch (Format.GetFieldFormat ((ObjectField) field.Value))
						{
							case DecimalFormat.Rate:
								text = Helpers.Converters.RateToString (d);
								alignment = ContentAlignment.MiddleRight;
								break;

							case DecimalFormat.Amount:
								text = Helpers.Converters.AmountToString (d);
								alignment = ContentAlignment.MiddleRight;
								break;

							case DecimalFormat.Real:
								text = Helpers.Converters.DecimalToString (d);
								alignment = ContentAlignment.MiddleRight;
								break;
						}
					}
					break;

				case FieldType.Int:
					var i = ObjectCalculator.GetObjectPropertyInt (this.obj, this.timestamp, (ObjectField) field.Value);
					if (i.HasValue)
					{
						text = Helpers.Converters.IntToString (i);
						alignment = ContentAlignment.MiddleRight;
					}
					break;

				case FieldType.ComputedAmount:
					var ca = ObjectCalculator.GetObjectPropertyComputedAmount (this.obj, this.timestamp, (ObjectField) field.Value);
					if (ca.HasValue)
					{
						text = Helpers.Converters.AmountToString (ca.Value.FinalAmount);
						alignment = ContentAlignment.MiddleRight;
					}
					break;

				case FieldType.Date:
					var da = ObjectCalculator.GetObjectPropertyDate (this.obj, this.timestamp, (ObjectField) field.Value);
					if (da.HasValue)
					{
						text = Helpers.Converters.DateToString (da.Value);
						alignment = ContentAlignment.MiddleLeft;
					}
					break;

				default:
					string s = ObjectCalculator.GetObjectPropertyString (this.obj, this.timestamp, (ObjectField) field.Value);
					if (!string.IsNullOrEmpty (s))
					{
						text = s;
						alignment = ContentAlignment.MiddleLeft;
					}
					break;
			}

			string tooltip = StaticDescriptions.GetObjectFieldDescription ((ObjectField) field.GetValueOrDefault (-1));
			bool hilited = this.IsHilited (field);
			bool readOnly = this.IsReadonly (field);

			return new SummaryControllerTile (text, tooltip, alignment, hilited, readOnly);
		}

		private bool IsHilited(int? field)
		{
			if (field.HasValue)
			{
				return this.GetPropertyState (field.Value) == PropertyState.Single;
			}
			else
			{
				return false;
			}
		}

		private bool IsReadonly(int? field)
		{
			//	Un champ est non modifiable s'il appartient à une page interdite
			//	pour le type de l'événement en cours.
			if (this.hasEvent && field.HasValue)
			{
				var type = ObjectEditorPageSummary.GetPageType ((ObjectField) field.Value);
				var availables = ObjectEditor.GetAvailablePages (this.hasEvent, this.eventType).ToArray ();
				return !availables.Contains (type);
			}
			else
			{
				return true;
			}
		}

		private PropertyState GetPropertyState(int field)
		{
			if (this.hasEvent)
			{
				var p = ObjectCalculator.GetObjectSyntheticProperty (this.obj, this.timestamp, (ObjectField) field);

				if (p != null)
				{
					return p.State;
				}
			}

			return PropertyState.Readonly;
		}

		private int SinglePropertiesCount
		{
			get
			{
				if (this.timestamp.HasValue)
				{
					var e = obj.GetEvent (this.timestamp.Value);
					if (e != null)
					{
						return e.PropertiesCount;
					}
				}

				return 0;
			}
		}

		private int? GetField(int column, int row)
		{
			if (column < this.fields.Count)
			{
				var rows = this.fields[column];

				if (row < rows.Count)
				{
					return rows[row];
				}
			}

			return null;
		}

		private int ColumnsCount
		{
			get
			{
				return this.fields.Count;
			}
		}

		private int RowsCount
		{
			get
			{
				int count = 0;

				foreach (var columns in this.fields)
				{
					count = System.Math.Max (count, columns.Count);
				}

				return count;
			}
		}


		#region Events handler
		private void OnTileClicked(int row, int column)
		{
			if (this.TileClicked != null)
			{
				this.TileClicked (this, row, column);
			}
		}

		public delegate void TileClickedEventHandler(object sender, int row, int column);
		public event TileClickedEventHandler TileClicked;
		#endregion


		private readonly DataAccessor				accessor;
		private readonly List<string>				columnsTitle;
		private readonly List<List<int>>			fields;
		private readonly SummaryController			controller;

		private StaticText							informations;
		private FrameBox							header;
		private DataObject							obj;
		private Timestamp?							timestamp;
		private bool								hasEvent;
		private EventType							eventType;
	}
}
