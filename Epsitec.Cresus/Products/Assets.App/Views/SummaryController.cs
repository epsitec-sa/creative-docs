//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Widgets;
using Epsitec.Cresus.Assets.Server.NaiveEngine;

namespace Epsitec.Cresus.Assets.App.Views
{
	public class SummaryController
	{
		public SummaryController(DataAccessor accessor, List<List<int>> fields)
		{
			this.accessor = accessor;
			this.fields = fields;
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

			this.frameBox = new FrameBox
			{
				Parent = parent,
				Dock   = DockStyle.Fill,
			};

			int columnsCount = this.ColumnsCount;
			int rowsCount    = this.RowsCount;

			for (int column = 0; column < columnsCount; column++ )
			{
				var columnFrame = new FrameBox
				{
					Parent         = this.frameBox,
					Dock           = DockStyle.Left,
					PreferredWidth = 120,
				};

				for (int row = 0; row < rowsCount; row++)
				{
					var button = new ColoredButton
					{
						Parent        = columnFrame,
						Name          = SummaryController.PutRowColumn (row, column),
						Dock          = DockStyle.Top,
						PreferredSize = new Size (100, 20),
						Margins       = new Margins (1),
						TextBreakMode = TextBreakMode.Ellipsis | TextBreakMode.Split | TextBreakMode.SingleLine,
					};

					var field = this.GetField (column, row);
					var desc = StaticDescriptions.GetObjectFieldDescription ((ObjectField) field.GetValueOrDefault (-1));
					if (!string.IsNullOrEmpty (desc))
					{
						ToolTip.Default.SetToolTip (button, desc);
					}

					this.UpdateButton (button, this.GetField (column, row));

					button.Clicked += delegate
					{
						int r, c;
						SummaryController.GetRowColumn (button.Name, out r, out c);
						this.OnTileClicked (r, c);
					};
				}
			}
		}

		public void UpdateFields(Guid objectGuid, Timestamp? timestamp)
		{
			this.timestamp = timestamp;

			if (objectGuid.IsEmpty)
			{
				this.hasEvent = false;
				this.eventType = EventType.Unknown;
				this.properties = null;
			}
			else
			{
				var ts = timestamp.GetValueOrDefault (new Timestamp (System.DateTime.MaxValue, 0));
				this.hasEvent = this.accessor.HasObjectEvent (objectGuid, ts);
				this.eventType = this.accessor.GetObjectEventType (objectGuid, ts).GetValueOrDefault (EventType.Unknown);
				this.properties = this.accessor.GetObjectSyntheticProperties (objectGuid, ts);
			}

			this.UpdateInformations ();

			for (int column = 0; column < this.frameBox.Children.Count; column++)
			{
				var columnFrame = this.frameBox.Children[column] as FrameBox;

				for (int row = 0; row < columnFrame.Children.Count; row++)
				{
					var button = columnFrame.Children[row] as ColoredButton;
					this.UpdateButton (button, this.GetField (column, row));
				}
			}
		}


		private void UpdateInformations()
		{
			this.informations.Text = string.Concat (this.Informations1, "<br/>", this.Informations2);
		}

		private string Informations1
		{
			get
			{
				if (this.hasEvent && this.properties != null)
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
				else if (!this.timestamp.HasValue)
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
				if (this.timestamp.HasValue)
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


		private void UpdateButton(ColoredButton button, int? field)
		{
			var a = this.GetActiveState (field);

			if (a == ActiveState.Maybe)
			{
				button.NormalColor   = ColorManager.EditBackgroundColor;
				button.SelectedColor = ColorManager.EditBackgroundColor;
				button.HoverColor    = ColorManager.EditBackgroundColor;

				button.ActiveState = ActiveState.No;
			}
			else
			{
				button.NormalColor   = ColorManager.WindowBackgroundColor;
				button.SelectedColor = ColorManager.EditSinglePropertyColor;
				button.HoverColor    = ColorManager.SelectionColor;

				button.ActiveState = a;
			}

			if (field.HasValue && this.properties != null)
			{
				var type = this.accessor.GetFieldType ((ObjectField) field.Value);

				switch (type)
				{
					case FieldType.Amount:
						var d = DataAccessor.GetDecimalProperty (this.properties, field.Value);
						if (d.HasValue)
						{
							button.Text = Helpers.Converters.AmountToString (d) + " ";
							button.ContentAlignment = ContentAlignment.MiddleRight;
						}
						break;

					case FieldType.Rate:
						var p = DataAccessor.GetDecimalProperty (this.properties, field.Value);
						if (p.HasValue)
						{
							button.Text = Helpers.Converters.RateToString (p) + " ";
							button.ContentAlignment = ContentAlignment.MiddleRight;
						}
						break;

					case FieldType.Int:
						var i = DataAccessor.GetIntProperty (this.properties, field.Value);
						if (i.HasValue)
						{
							button.Text = Helpers.Converters.IntToString (i) + " ";
							button.ContentAlignment = ContentAlignment.MiddleRight;
						}
						break;

					case FieldType.ComputedAmount:
						var ca = DataAccessor.GetComputedAmountProperty (this.properties, field.Value);
						if (ca.HasValue)
						{
							button.Text = Helpers.Converters.AmountToString (ca.Value.FinalAmount) + " ";
							button.ContentAlignment = ContentAlignment.MiddleRight;
						}
						break;

					default:
						string s = DataAccessor.GetStringProperty (this.properties, field.Value);
						if (!string.IsNullOrEmpty (s))
						{
							button.Text = " " + s;
							button.ContentAlignment = ContentAlignment.MiddleLeft;
						}
						break;
				}
			}
			else
			{
				button.Text = null;
			}
		}

		private ActiveState GetActiveState(int? field)
		{
			if (field.HasValue)
			{
				switch (this.GetPropertyState (field.Value))
				{
					case PropertyState.Single:
						return ActiveState.Yes;

					default:
						return ActiveState.No;
				}
			}
			else
			{
				return ActiveState.Maybe;
			}
		}

		private Color GetBackgroundColor(int? field)
		{
			if (field.HasValue)
			{
				switch (this.GetPropertyState (field.Value))
				{
					case PropertyState.Single:
						return ColorManager.EditSinglePropertyColor;

					default:
						return ColorManager.WindowBackgroundColor;
				}
			}
			else
			{
				return Color.Empty;
			}
		}

		private PropertyState GetPropertyState(int field)
		{
			if (this.hasEvent)
			{
				return DataAccessor.GetPropertyState (this.properties, field);
			}
			else
			{
				return PropertyState.Readonly;
			}
		}

		private int SinglePropertiesCount
		{
			get
			{
				if (this.properties == null)
				{
					return 0;
				}
				else
				{
					return this.properties.Where (x => x.State == PropertyState.Single).Count ();
				}
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


		private static string PutRowColumn(int row, int column)
		{
			return string.Concat
			(
				row.ToString (System.Globalization.CultureInfo.InstalledUICulture),
				"/",
				column.ToString (System.Globalization.CultureInfo.InstalledUICulture)
			);
		}

		private static void GetRowColumn(string text, out int row, out int column)
		{
			var p = text.Split ('/');
			row    = int.Parse (p[0], System.Globalization.CultureInfo.InstalledUICulture);
			column = int.Parse (p[1], System.Globalization.CultureInfo.InstalledUICulture);
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
		private readonly List<List<int>>			fields;

		private StaticText							informations;
		private FrameBox							frameBox;
		private Timestamp?							timestamp;
		private bool								hasEvent;
		private EventType							eventType;
		private IEnumerable<AbstractDataProperty>	properties;
	}
}
