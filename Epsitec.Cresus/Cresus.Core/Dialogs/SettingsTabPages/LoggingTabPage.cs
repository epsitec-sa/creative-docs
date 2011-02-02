//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Debug;
using Epsitec.Common.Dialogs;
using Epsitec.Common.Drawing;
using Epsitec.Common.IO;
using Epsitec.Common.Support;
using Epsitec.Common.Types;
using Epsitec.Common.Widgets;

using Epsitec.Common.Support.EntityEngine;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Database.Logging;
using Epsitec.Cresus.Core.Widgets;

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;

namespace Epsitec.Cresus.Core.Dialogs.SettingsTabPages
{
	/// <summary>
	/// Onglet de SettingsDialog pour gérer le DbInfrastructure.Logging de la base de données.
	/// </summary>
	public class LoggingTabPage : AbstractSettingsTabPage
	{
		public LoggingTabPage(CoreApplication application)
			: base (application)
		{
		}


		public override void AcceptChangings()
		{
		}

		public override void CreateUI(Widget parent)
		{
			var topFrame = new FrameBox
			{
				Parent = parent,
				Dock = DockStyle.Fill,
				Margins = new Margins (10),
			};

			this.detailsFrame = new FrameBox
			{
				Parent = parent,
				PreferredHeight = 220,
				Dock = DockStyle.Bottom,
				Margins = new Margins (10, 0, 10, 10),
			};

			this.splitter = new HSplitter
			{
				Parent = parent,
				Dock = DockStyle.Bottom,
			};

			this.CreateUIMainToolbar (topFrame);
			this.CreateUISecondaryToolbar (topFrame);

			this.mainTable = new CellTable
			{
				Parent = topFrame,
				StyleH = CellArrayStyles.ScrollNorm | CellArrayStyles.Separator | CellArrayStyles.Header | CellArrayStyles.Mobile,
				StyleV = CellArrayStyles.ScrollNorm | CellArrayStyles.Separator | CellArrayStyles.SelectLine,
				Dock = DockStyle.Fill,
			};

			this.CreateUIDetails (this.detailsFrame);

			//	Connection des événements.
			this.extendedButton.Clicked += delegate
			{
				this.LogMode = Database.Logging.LogMode.Extended;
			};

			this.basicButton.Clicked += delegate
			{
				this.LogMode = Database.Logging.LogMode.Basic;
			};

			this.offButton.Clicked += delegate
			{
				this.LogMode = Database.Logging.LogMode.Off;
			};

			this.secondaryButton.Clicked += delegate
			{
				this.SecondarySwap ();
			};

			this.searchField.TextChanged += delegate
			{
				this.UpdateWidgets ();
			};

			this.searchClearButton.Clicked += delegate
			{
				this.searchField.Text = null;
				this.UpdateTable();
			};

			this.searchPrevButton.Clicked += delegate
			{
				this.Search (-1);
			};

			this.searchNextButton.Clicked += delegate
			{
				this.Search (1);
			};

			this.updateButton.Clicked += delegate
			{
				this.UpdateTable ();
				this.UpdateDetails ();
			};

			this.clearButton.Clicked += delegate
			{
				this.ClearTable ();
			};

			this.exportButton.Clicked += delegate
			{
				this.Export ();
			};

			this.importButton.Clicked += delegate
			{
				this.Import ();
			};

			this.mainTable.SelectionChanged += delegate
			{
				this.UpdateDetails ();
			};

			this.substituteButton.ActiveStateChanged += delegate
			{
				this.UpdateDetails ();
			};

			this.colorizeButton.ActiveStateChanged += delegate
			{
				this.UpdateDetails ();
			};

			this.UpdateRadio ();
			this.UpdateTable ();
			this.UpdateDetails ();
			this.UpdateWidgets ();
		}

		private void CreateUIMainToolbar(Widget parent)
		{
			var header = new FrameBox
			{
				Parent = parent,
				Dock = DockStyle.Top,
				Margins = new Margins (0, 0, 0, 10),
			};

			this.extendedButton = new RadioButton
			{
				Parent = header,
				Text = "Trace complète",
				PreferredWidth = 110,
				AutoToggle = false,
				Dock = DockStyle.Left,
			};

			this.basicButton = new RadioButton
			{
				Parent = header,
				Text = "Trace réduite",
				PreferredWidth = 100,
				AutoToggle = false,
				Dock = DockStyle.Left,
			};

			this.offButton = new RadioButton
			{
				Parent = header,
				Text = "Pas de trace",
				PreferredWidth = 90,
				AutoToggle = false,
				Dock = DockStyle.Left,
			};

			this.secondaryButton = new GlyphButton
			{
				Parent = header,
				ButtonStyle = ButtonStyle.Slider,
				Dock = DockStyle.Right,
				Margins = new Margins (10, 0, 3, 3),
			};

			this.clearButton = new Button
			{
				Parent = header,
				Text = "Vider",
				PreferredWidth = 100,
				Dock = DockStyle.Right,
				Margins = new Margins (10, 0, 0, 0),
			};

			this.updateButton = new Button
			{
				Parent = header,
				Text = "Mettre à jour",
				PreferredWidth = 100,
				Dock = DockStyle.Right,
				Margins = new Margins (10, 0, 0, 0),
			};
		}

		private void CreateUISecondaryToolbar(Widget parent)
		{
			this.secondaryToolbar = new FrameBox
			{
				Parent = parent,
				Dock = DockStyle.Top,
				Margins = new Margins (0, 0, 0, 10),
				Visibility = false,
			};

			new Separator
			{
				Parent = this.secondaryToolbar,
				PreferredHeight = 1,
				Dock = DockStyle.Top,
				Margins = new Margins (0, 0, 0, 10),
			};

			var box = new FrameBox
			{
				Parent = this.secondaryToolbar,
				Dock = DockStyle.Fill,
			};

			var label = new StaticText
			{
				Parent = box,
				Text = "Rechercher",
				PreferredWidth = 64,
				Dock = DockStyle.Left,
			};

			this.searchField = new TextField
			{
				Parent = box,
				Dock = DockStyle.Fill,
				Margins = new Margins (0, 0, 0, 0),
			};

			this.importButton = new Button
			{
				Parent = box,
				Text = "Importer...",
				PreferredWidth = 70,
				Dock = DockStyle.Right,
				Margins = new Margins (1, 0, 0, 0),
			};

			this.exportButton = new Button
			{
				Parent = box,
				Text = "Exporter...",
				PreferredWidth = 70,
				Dock = DockStyle.Right,
				Margins = new Margins (20, 0, 0, 0),
			};

			this.caseButton = new CheckButton
			{
				Parent = box,
				Text = "Respecter la casse",
				PreferredWidth = 130,
				ActiveState = Common.Widgets.ActiveState.Yes,
				Dock = DockStyle.Right,
				Margins = new Margins (5, 0, 0, 0),
			};

			this.searchCounterInfo = new StaticText
			{
				Parent = box,
				PreferredWidth = 100,
				Dock = DockStyle.Right,
				Margins = new Margins (15, 0, 0, 0),
			};

			this.searchClearButton = new GlyphButton
			{
				Parent = box,
				GlyphShape = Common.Widgets.GlyphShape.Close,
				Dock = DockStyle.Right,
				Margins = new Margins (1, 0, 0, 0),
			};

			this.searchNextButton = new Button
			{
				Parent = box,
				Text = "Suivant",
				PreferredWidth = 60,
				Dock = DockStyle.Right,
				Margins = new Margins (1, 0, 0, 0),
			};

			this.searchPrevButton = new Button
			{
				Parent = box,
				Text = "Précédent",
				PreferredWidth = 60,
				Dock = DockStyle.Right,
				Margins = new Margins (1, 0, 0, 0),
			};
		}

		private void CreateUIDetails(Widget parent)
		{
			this.CreateUIQueryToolbar (parent);

			this.queryField = new TextFieldMulti
			{
				Parent = parent,
				MaxLength = 100000,
				IsReadOnly = true,
				PreferredHeight = 66,
				Dock = DockStyle.Top,
				Margins = new Margins (0, 10, 0, 10),
			};

			this.detailsBox = new FrameBox
			{
				Parent = parent,
				PreferredHeight = 100,
				Dock = DockStyle.Fill,
			};
		}

		private void CreateUIQueryToolbar(Widget parent)
		{
			var toolbar = new FrameBox
			{
				Parent = parent,
				Dock = DockStyle.Top,
				Margins = new Margins (0, 10, 0, 5),
			};

			this.substituteButton = new CheckButton
			{
				Parent = toolbar,
				Text = "Substituer les paramètres",
				PreferredWidth = 150,
				ActiveState = Common.Widgets.ActiveState.Yes,
				Dock = DockStyle.Left,
				Margins = new Margins (0, 0, 0, 0),
			};

			this.colorizeButton = new CheckButton
			{
				Parent = toolbar,
				Text = "Coloriage syntaxique",
				PreferredWidth = 150,
				ActiveState = Common.Widgets.ActiveState.Yes,
				Dock = DockStyle.Left,
				Margins = new Margins (10, 0, 0, 0),
			};
		}


		private void SecondarySwap()
		{
			LoggingTabPage.secondaryVisibility = !LoggingTabPage.secondaryVisibility;

			if (!LoggingTabPage.secondaryVisibility && !string.IsNullOrEmpty (this.searchField.Text))
			{
				this.searchField.Text = null;
				this.UpdateTable ();
			}

			this.UpdateWidgets ();
		}

		private void UpdateRadio()
		{
			this.extendedButton.ActiveState = (this.LogMode == Database.Logging.LogMode.Extended) ? ActiveState.Yes : ActiveState.No;
			this.basicButton.ActiveState    = (this.LogMode == Database.Logging.LogMode.Basic   ) ? ActiveState.Yes : ActiveState.No;
			this.offButton.ActiveState      = (this.LogMode == Database.Logging.LogMode.Off     ) ? ActiveState.Yes : ActiveState.No;
		}


		private void UpdateTable()
		{
			var db = this.application.Data.DataInfrastructure.DbInfrastructure;
			int rows = (db.QueryLog == null) ? 0 : db.QueryLog.GetNbEntries ();

			this.mainTable.SetArraySize (6, rows);

			this.mainTable.SetWidthColumn (0,  40);
			this.mainTable.SetWidthColumn (1, 110);
			this.mainTable.SetWidthColumn (2,  70);
			this.mainTable.SetWidthColumn (3, 350);
			this.mainTable.SetWidthColumn (4, 110);
			this.mainTable.SetWidthColumn (5, 110);

			this.mainTable.SetHeaderTextH (0, "N°");
			this.mainTable.SetHeaderTextH (1, "Début");
			this.mainTable.SetHeaderTextH (2, "Durée");
			this.mainTable.SetHeaderTextH (3, "Requête");
			this.mainTable.SetHeaderTextH (4, "Paramètres");
			this.mainTable.SetHeaderTextH (5, "Résultats");

			ContentAlignment[] alignments =
			{
				ContentAlignment.MiddleRight,
				ContentAlignment.MiddleLeft,
				ContentAlignment.MiddleRight,
				ContentAlignment.MiddleLeft,
				ContentAlignment.MiddleLeft,
				ContentAlignment.MiddleLeft
			};

			int counter = 0;
			for (int row=0; row<rows; row++)
			{
				var query = db.QueryLog.GetEntry (row);
				var values = query.GetMainStrings (row);
				this.ColorizeSearchingString (values, ref counter);

				this.mainTable.FillRow (row, alignments);
				this.mainTable.UpdateRow (row, values);
			}

			this.UpdateSearchCounter (counter);
		}

		private void UpdateSearchCounter(int counter)
		{
			if (counter == 0)
			{
				if (string.IsNullOrEmpty (this.searchField.Text))
				{
					this.searchCounterInfo.Text = null;
				}
				else
				{
					this.searchCounterInfo.Text = "pas trouvé";
				}
			}
			else if (counter == 1)
			{
				this.searchCounterInfo.Text = "1 résultat";
			}
			else
			{
				this.searchCounterInfo.Text = string.Format ("{0} résultats", counter.ToString ());
			}
		}

		private void UpdateDetails()
		{
			this.detailsBox.Children.Clear ();

			int sel = this.mainTable.SelectedRow;

			if (sel == -1)
			{
				this.queryField.FormattedText = null;
			}
			else
			{
				var db = this.application.Data.DataInfrastructure.DbInfrastructure;
				var query = db.QueryLog.GetEntry (sel);

				bool substitute = this.substituteButton.ActiveState == ActiveState.Yes;
				bool colorize   = this.colorizeButton.ActiveState   == ActiveState.Yes;
				string content = query.GetQuery (substitute, colorize).ToString ();
				int counter = 0;
				content = this.ColorizeSearchingString (content, ref counter);

				if (content.Length >= this.queryField.MaxLength)
				{
					this.queryField.FormattedText = "Trop long...";
				}
				else
				{
					this.queryField.FormattedText = content;
				}

				var parameters = this.CreateParametersShower (query.Parameters);
				parameters.Parent = this.detailsBox;
				parameters.Dock = DockStyle.Left;
				parameters.Margins = new Margins (0, 10, 0, 0);

				if (query.Result != null)
				{
					foreach (var table in query.Result.Tables)
					{
						var cellTable = this.CreateTableResultsShower (table);
						cellTable.Parent = this.detailsBox;
						cellTable.Dock = DockStyle.Fill;
						cellTable.Margins = new Margins (0, 10, 0, 0);
					}
				}
			}
		}

		private void UpdateWidgets()
		{
			bool enable = !string.IsNullOrEmpty (this.searchField.Text);
			bool empty = this.mainTable.Rows == 0;

			this.searchClearButton.Enable = enable;
			this.searchPrevButton.Enable  = enable;
			this.searchNextButton.Enable  = enable;

			this.clearButton.Enable  = !empty;
			this.exportButton.Enable = !empty;

			this.secondaryToolbar.Visibility = LoggingTabPage.secondaryVisibility;
			this.secondaryButton.GlyphShape = LoggingTabPage.secondaryVisibility ? Common.Widgets.GlyphShape.TriangleUp : Common.Widgets.GlyphShape.TriangleDown;
		}


		private void Search(int direction)
		{
			var db = this.application.Data.DataInfrastructure.DbInfrastructure;

			if (db.QueryLog == null)
			{
				return;
			}

			string search = this.searchField.Text;
			bool caseSensitive = this.caseButton.ActiveState == ActiveState.Yes;

			if (this.lastSearching != search || this.lastCaseSensitive != caseSensitive)
			{
				this.lastSearching = search;
				this.lastCaseSensitive = caseSensitive;

				this.UpdateTable ();
				this.UpdateDetails ();
			}

			int count = this.mainTable.Rows;
			int sel = this.mainTable.SelectedRow;

			if (sel == -1)
			{
				sel = 0;
			}

			if (!caseSensitive)
			{
				search = Misc.RemoveAccentsToLower (search);
			}

			for (int i = 0; i < count; i++)  // une boucle complète
			{
				sel += direction;  // suivant ou précédent

				if (sel < 0)
				{
					sel = count-1;  // début -> fin
				}

				if (sel >= count)
				{
					sel = 0;  // fin -> début
				}

				var query = db.QueryLog.GetEntry (sel);

				if (query.ContainsString (search, caseSensitive))
				{
					this.mainTable.DeselectAll ();
					this.mainTable.SelectRow (sel, true);
					this.mainTable.ShowSelect ();
					this.UpdateDetails ();
					return;
				}
			}
		}

		private void ClearTable()
		{
			var db = this.application.Data.DataInfrastructure.DbInfrastructure;
			db.QueryLog.Clear ();

			this.UpdateTable ();
			this.UpdateDetails ();
			this.UpdateWidgets ();
		}


		private void Export()
		{
			//	TODO: ...
		}

		private void Import()
		{
			//	TODO: ...
		}


		#region Create showers
		private Widget CreateParametersShower(ReadOnlyCollection<Parameter> parameters)
		{
			//	Retourne le widget permettant de représenter les paramètres.
			var frame = new FrameBox
			{
				PreferredWidth = 80+80+20,
				DrawFullFrame = true,
				Padding = new Margins (2),
			};

			var title = new StaticText
			{
				Parent = frame,
				FormattedText = Misc.Bold ("Paramètres"),
				ContentAlignment = Common.Drawing.ContentAlignment.MiddleCenter,
				Dock = DockStyle.Top,
			};

			var cellTable = new CellTable
			{
				Parent = frame,
				Dock = DockStyle.Fill,
				StyleH = CellArrayStyles.ScrollNorm | CellArrayStyles.Separator | CellArrayStyles.Header | CellArrayStyles.Mobile,
				StyleV = CellArrayStyles.ScrollNorm | CellArrayStyles.Separator | CellArrayStyles.SelectLine,
			};

			cellTable.SetArraySize (2, parameters.Count);

			cellTable.SetWidthColumn (0, 80);
			cellTable.SetWidthColumn (1, 80);

			cellTable.SetHeaderTextH (0, "Nom");
			cellTable.SetHeaderTextH (1, "Valeur");

			ContentAlignment[] alignments = { ContentAlignment.MiddleLeft, ContentAlignment.MiddleRight };

			for (int row=0; row<parameters.Count; row++)
			{
				var values = QueryAccessor.GetParameterStrings (parameters[row]);
				int counter = 0;
				this.ColorizeSearchingString (values, ref counter);

				cellTable.FillRow (row, alignments);
				cellTable.UpdateRow (row, values);
			}

			return frame;
		}

		private Widget CreateTableResultsShower(Table table)
		{
			//	Retourne le widget permettant de représenter les résultats d'une table.
			var frame = new FrameBox
			{
				PreferredWidth = 300,
				DrawFullFrame = true,
				Padding = new Margins (2),
			};

			var title = new StaticText
			{
				Parent = frame,
				FormattedText = Misc.Bold (table.Name),
				ContentAlignment = Common.Drawing.ContentAlignment.MiddleCenter,
				Dock = DockStyle.Top,
			};

			var cellTable = new CellTable
			{
				Parent = frame,
				Dock = DockStyle.Fill,
				StyleH = CellArrayStyles.ScrollNorm | CellArrayStyles.Separator | CellArrayStyles.Header | CellArrayStyles.Mobile,
				StyleV = CellArrayStyles.ScrollNorm | CellArrayStyles.Separator | CellArrayStyles.SelectLine,
			};

			int columnsCount = table.Columns.Count;
			int rowsCount = table.Rows.Count;

			cellTable.SetArraySize (columnsCount, rowsCount);

			var alignments = new List<ContentAlignment> ();

			for (int column = 0; column < columnsCount; column++)
			{
				cellTable.SetWidthColumn (column, 100);
				cellTable.SetHeaderTextH (column, table.Columns[column].Name);
				alignments.Add (ContentAlignment.MiddleLeft);
			}

			for (int row=0; row<rowsCount; row++)
			{
				var values = QueryAccessor.GetTableResultsStrings (table.Rows[row].Values);
				int counter = 0;
				this.ColorizeSearchingString (values, ref counter);

				cellTable.FillRow (row, alignments.ToArray ());
				cellTable.UpdateRow (row, values);
			}

			return frame;
		}
		#endregion


		private void ColorizeSearchingString(string[] values, ref int counter)
		{
			for (int i = 0; i < values.Length; i++)
			{
				values[i] = this.ColorizeSearchingString (values[i], ref counter);
			}
		}

		private string ColorizeSearchingString(string text, ref int counter)
		{
			string searching = this.searchField.Text;
			bool caseSensitive = this.caseButton.ActiveState == ActiveState.Yes;

			if (string.IsNullOrEmpty (searching))
			{
				return text;
			}

			if (!caseSensitive)
			{
				searching = Misc.RemoveAccentsToLower (searching);
			}

			var color = Color.FromName ("Green");
			var tag1 = string.Concat ("<font color=\"#", Color.ToHexa (color), "\"><b>");
			var tag2 = "</b></font>";

			if (caseSensitive)
			{
				int index = 0;
				while (index < text.Length)
				{
					index = text.IndexOf (searching, index);

					if (index == -1)
					{
						break;
					}

					text = text.Insert (index+searching.Length, tag2);
					text = text.Insert (index, tag1);

					index += tag1.Length;
					index += tag2.Length;

					counter++;
				}

				return text;
			}
			else
			{
				var lowerText = Misc.RemoveAccentsToLower (text);

				int index = 0;
				while (index < text.Length)
				{
					index = lowerText.IndexOf (searching, index);

					if (index == -1)
					{
						break;
					}

					text = text.Insert (index+searching.Length, tag2);
					text = text.Insert (index, tag1);

					lowerText = lowerText.Insert (index+searching.Length, tag2);
					lowerText = lowerText.Insert (index, tag1);

					index += tag1.Length;
					index += tag2.Length;

					counter++;
				}

				return text;
			}
		}


		private LogMode LogMode
		{
			//	Mode de trace.
			get
			{
				var db = this.application.Data.DataInfrastructure.DbInfrastructure;

				if (db.QueryLog == null)
				{
					return Database.Logging.LogMode.Off;
				}
				else
				{
					return db.QueryLog.Mode;
				}
			}
			set
			{
				var db = this.application.Data.DataInfrastructure.DbInfrastructure;

				if (value == Database.Logging.LogMode.Off)
				{
					db.DisableLogging ();
				}
				else
				{
					db.EnableLogging ();
					db.QueryLog.Mode = value;
				}

				this.UpdateRadio ();
			}
		}


		private static bool			secondaryVisibility;

		private RadioButton			extendedButton;
		private RadioButton			basicButton;
		private RadioButton			offButton;
		private Button				updateButton;
		private Button				clearButton;
		private GlyphButton			secondaryButton;

		private FrameBox			secondaryToolbar;
		private TextField			searchField;
		private Button				searchPrevButton;
		private Button				searchNextButton;
		private GlyphButton			searchClearButton;
		private StaticText			searchCounterInfo;
		private CheckButton			caseButton;
		private Button				exportButton;
		private Button				importButton;

		private CellTable			mainTable;
		private HSplitter			splitter;

		private FrameBox			detailsFrame;
		private CheckButton			substituteButton;
		private CheckButton			colorizeButton;
		private TextFieldMulti		queryField;
		private FrameBox			detailsBox;

		private string				lastSearching;
		private bool				lastCaseSensitive;
	}
}
