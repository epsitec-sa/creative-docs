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
			this.queries = new List<Query> ();
			this.CopyQueries ();
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
				Margins = new Margins (0, 0, 10, 10),
			};

			this.splitter1 = new HSplitter
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

			this.queryOptionsButton.Clicked += delegate
			{
				this.QueryOptionsSwap ();
			};

			this.searchField.TextChanged += delegate
			{
				this.UpdateWidgets ();
			};

			this.searchClearButton.Clicked += delegate
			{
				this.ClearSearch ();
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
				this.CopyQueries ();
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
				this.UpdateRelativeTime ();
			};

			this.caseSensitiveButton.Clicked += delegate
			{
				LoggingTabPage.globalCaseSensitive = !LoggingTabPage.globalCaseSensitive;
				this.Search (0);
				this.UpdateWidgets ();
			};

			this.substituteButton.Clicked += delegate
			{
				LoggingTabPage.globalSubstitute = !LoggingTabPage.globalSubstitute;
				this.UpdateDetails ();
				this.UpdateWidgets ();
			};

			this.colorizeButton.Clicked += delegate
			{
				LoggingTabPage.globalColorize = !LoggingTabPage.globalColorize;
				this.UpdateDetails ();
				this.UpdateWidgets ();
			};

			this.autoBreakButton.Clicked += delegate
			{
				LoggingTabPage.globalAutoBreak = !LoggingTabPage.globalAutoBreak;
				this.UpdateDetails ();
				this.UpdateWidgets ();
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

			this.searchClearButton = new GlyphButton
			{
				Parent = box,
				GlyphShape = Common.Widgets.GlyphShape.Close,
				Dock = DockStyle.Left,
				Margins = new Margins (0, 10, 2, 2),
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
				Margins = new Margins (10, 0, 0, 0),
			};

			this.caseSensitiveButton = new CheckButton
			{
				Parent = box,
				Text = "Respecter la casse",
				PreferredWidth = 120,
				AutoToggle = false,
				Dock = DockStyle.Right,
				Margins = new Margins (5, 0, 0, 0),
			};

			this.searchCounterInfo = new StaticText
			{
				Parent = box,
				PreferredWidth = 150,
				Dock = DockStyle.Right,
				Margins = new Margins (15, 0, 0, 0),
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
			var box = new FrameBox
			{
				Parent = parent,
				PreferredHeight = 80,
				Dock = DockStyle.Top,
				Margins = new Margins (10, 10, 0, 10),
			};

			var leftBox = new FrameBox
			{
				Parent = box,
				Dock = DockStyle.Fill,
			};

			var rightBox = new FrameBox
			{
				Parent = box,
				PreferredWidth = 18,
				Dock = DockStyle.Right,
				Margins = new Margins (5, 0, 0, 0),
			};

			this.CreateUIQueryOptionsToolbar (leftBox);

			this.queryField = new TextFieldMulti
			{
				Parent = leftBox,
				MaxLength = 100000,
				IsReadOnly = true,
				Dock = DockStyle.Fill,
			};

			this.queryOptionsButton = new GlyphButton
			{
				Parent = rightBox,
				PreferredSize = new Size (18, 18),
				ButtonStyle = ButtonStyle.Slider,
				Dock = DockStyle.Top,
			};

			this.splitter2 = new HSplitter
			{
				Parent = parent,
				Dock = DockStyle.Top,
			};

			this.detailsBox = new FrameBox
			{
				Parent = parent,
				PreferredHeight = 100,
				Dock = DockStyle.Fill,
				Margins = new Margins (10, 0, 10, 0),
			};
		}

		private void CreateUIQueryOptionsToolbar(Widget parent)
		{
			this.queryOptionsToolbar = new FrameBox
			{
				Parent = parent,
				Dock = DockStyle.Top,
				Margins = new Margins (0, 10, 0, 5),
			};

			this.substituteButton = new CheckButton
			{
				Parent = this.queryOptionsToolbar,
				Text = "Substituer les paramètres",
				PreferredWidth = 150,
				AutoToggle = false,
				Dock = DockStyle.Left,
				Margins = new Margins (0, 0, 0, 0),
			};

			this.colorizeButton = new CheckButton
			{
				Parent = this.queryOptionsToolbar,
				Text = "Coloriage syntaxique",
				PreferredWidth = 140,
				AutoToggle = false,
				Dock = DockStyle.Left,
				Margins = new Margins (0, 0, 0, 0),
			};

			this.autoBreakButton = new CheckButton
			{
				Parent = this.queryOptionsToolbar,
				Text = "Retours à la ligne automatiques",
				PreferredWidth = 200,
				AutoToggle = false,
				Dock = DockStyle.Left,
				Margins = new Margins (0, 0, 0, 0),
			};
		}


		private void CopyQueries()
		{
			//	Copie toutes les queries sur lesquelles on va travailler.
			var db = this.application.Data.DataInfrastructure.DbInfrastructure;

			this.queries.Clear ();

			if (db.QueryLog != null)
			{
				int count = db.QueryLog.GetNbEntries ();

				for (int i = 0; i < count; i++)
				{
					var query = db.QueryLog.GetEntry (i);
					this.queries.Add (query);
				}
			}
		}


		private void SecondarySwap()
		{
			LoggingTabPage.globalSecondaryVisibility = !LoggingTabPage.globalSecondaryVisibility;

			if (!LoggingTabPage.globalSecondaryVisibility && !string.IsNullOrEmpty (this.searchField.Text))
			{
				this.ClearSearch ();
			}

			this.UpdateWidgets ();
		}

		private void QueryOptionsSwap()
		{
			LoggingTabPage.globalQueryOptionsVisibility = !LoggingTabPage.globalQueryOptionsVisibility;
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
			int rows = this.queries.Count;
			this.mainTable.SetArraySize (7, rows);

			this.mainTable.SetWidthColumn (0,  40);
			this.mainTable.SetWidthColumn (1,  70);
			this.mainTable.SetWidthColumn (2,  60);
			this.mainTable.SetWidthColumn (3,  60);
			this.mainTable.SetWidthColumn (4, 790-40-70-60-60-120-120);
			this.mainTable.SetWidthColumn (5, 120);
			this.mainTable.SetWidthColumn (6, 120);

			this.mainTable.SetHeaderTextH (0, "N°");
			this.mainTable.SetHeaderTextH (1, "Début");
			this.mainTable.SetHeaderTextH (2, "Temps");
			this.mainTable.SetHeaderTextH (3, "Durée");
			this.mainTable.SetHeaderTextH (4, "Requête");
			this.mainTable.SetHeaderTextH (5, "Paramètres");
			this.mainTable.SetHeaderTextH (6, "Résultats");

			ContentAlignment[] alignments =
			{
				ContentAlignment.MiddleRight,
				ContentAlignment.MiddleLeft,
				ContentAlignment.MiddleRight,
				ContentAlignment.MiddleRight,
				ContentAlignment.MiddleLeft,
				ContentAlignment.MiddleLeft,
				ContentAlignment.MiddleLeft
			};

			string search = this.SearchText;
			bool caseSensitive = LoggingTabPage.globalCaseSensitive;

			int counter = 0;
			int lines = 0;

			for (int row=0; row<rows; row++)
			{
				var query = this.queries[row];
				var values = query.GetMainStrings (row);
				this.ColorizeSearchingString (values);

				this.mainTable.FillRow (row, alignments);
				this.mainTable.UpdateRow (row, values);

				if (!string.IsNullOrEmpty (search))
				{
					int n = query.Count (search, caseSensitive);
					if (n > 0)
					{
						counter += n;
						lines++;
					}
				}
			}

			this.UpdateRelativeTime ();
			this.UpdateSearchCounter (counter, lines);
		}

		private void UpdateRelativeTime()
		{
			int rows = this.queries.Count;
			int sel = this.mainTable.SelectedRow;
			
			if (sel == -1)
			{
				sel = 0;
			}

			if (sel < rows)
			{
				var reference = this.queries[sel].StartTime;

				for (int row=0; row<rows; row++)
				{
					System.TimeSpan time = this.queries[row].StartTime.Subtract (reference);

					var widget = this.mainTable.GetStaticText (row, 2);
					widget.Text = LoggingTabPage.ToNiceString (time);
				}
			}
		}

		private void UpdateSearchCounter(int counter, int lines)
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
				if (counter == lines)
				{
					this.searchCounterInfo.Text = string.Format ("{0} résultats", counter.ToString ());
				}
				else
				{
					if (lines == 1)
					{
						this.searchCounterInfo.Text = string.Format ("{0} résultats dans 1 ligne", counter.ToString ());
					}
					else
					{
						this.searchCounterInfo.Text = string.Format ("{0} résultats dans {1} lignes", counter.ToString (), lines.ToString ());
					}
				}
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
				var query = this.queries[sel];

				bool substitute = LoggingTabPage.globalSubstitute;
				bool colorize   = LoggingTabPage.globalColorize;
				bool autoBreak  = LoggingTabPage.globalAutoBreak;
				string content = query.GetQuery (substitute, colorize, autoBreak).ToString ();
				content = this.ColorizeSearchingString (content);

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

			this.secondaryToolbar.Visibility = LoggingTabPage.globalSecondaryVisibility;
			this.secondaryButton.GlyphShape  = LoggingTabPage.globalSecondaryVisibility ? Common.Widgets.GlyphShape.TriangleUp : Common.Widgets.GlyphShape.TriangleDown;

			this.queryOptionsToolbar.Visibility = LoggingTabPage.globalQueryOptionsVisibility;
			this.queryOptionsButton.GlyphShape  = LoggingTabPage.globalQueryOptionsVisibility ? Common.Widgets.GlyphShape.TriangleUp : Common.Widgets.GlyphShape.TriangleDown;

			this.caseSensitiveButton.ActiveState = LoggingTabPage.globalCaseSensitive ? ActiveState.Yes : ActiveState.No;
			this.substituteButton.ActiveState    = LoggingTabPage.globalSubstitute    ? ActiveState.Yes : ActiveState.No;
			this.colorizeButton.ActiveState      = LoggingTabPage.globalColorize      ? ActiveState.Yes : ActiveState.No;
			this.autoBreakButton.ActiveState     = LoggingTabPage.globalAutoBreak     ? ActiveState.Yes : ActiveState.No;
		}


		private void ClearSearch()
		{
			//	Annule la recherche en cours.
			this.searchField.Text = null;
			this.lastSearching = null;

			this.UpdateTable ();
			this.UpdateDetails ();
		}

		private void Search(int direction)
		{
			if (this.queries.Count == 0)
			{
				return;
			}

			string search = this.SearchText;
			bool caseSensitive = LoggingTabPage.globalCaseSensitive;

			if (this.lastSearching != search || this.lastCaseSensitive != caseSensitive)
			{
				this.lastSearching = search;
				this.lastCaseSensitive = caseSensitive;

				this.UpdateTable ();
				this.UpdateDetails ();
			}

			if (direction != 0)
			{
				int count = this.mainTable.Rows;
				int sel = this.mainTable.SelectedRow;

				if (sel == -1)
				{
					sel = 0;
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

					var query = this.queries[sel];

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
		}

		private void ClearTable()
		{
			var db = this.application.Data.DataInfrastructure.DbInfrastructure;
			db.QueryLog.Clear ();

			this.CopyQueries ();
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
				this.ColorizeSearchingString (values);

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
				this.ColorizeSearchingString (values);

				cellTable.FillRow (row, alignments.ToArray ());
				cellTable.UpdateRow (row, values);
			}

			return frame;
		}
		#endregion


		private void ColorizeSearchingString(string[] values)
		{
			for (int i = 0; i < values.Length; i++)
			{
				values[i] = this.ColorizeSearchingString (values[i]);
			}
		}

		private string ColorizeSearchingString(string text)
		{
			string searching = this.SearchText;
			bool caseSensitive = LoggingTabPage.globalCaseSensitive;

			if (string.IsNullOrEmpty (searching))
			{
				return text;
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
				}

				return text;
			}
		}


		private string SearchText
		{
			get
			{
				string search = this.searchField.Text;

				if (!string.IsNullOrEmpty (search) && !LoggingTabPage.globalCaseSensitive)
				{
					search = Misc.RemoveAccentsToLower (search);
				}

				return search;
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


		private static string ToNiceString(System.TimeSpan time)
		{
			long t = time.Ticks / 10L;  // en us
			long abs = System.Math.Abs (t);

			if (abs >= 60L*60L*1000L*1000L)  // heures ?
			{
				int h = (int) (t/60/60/1000/1000)%24;
				int m = (int) (t/60/1000/1000)%60;
				int s = (int) (t/1000/1000)%60;
				return string.Concat (m.ToString (), " h ", m.ToString (), " m ", s.ToString (), " s");
			}
			else if (abs >= 60L*1000L*1000L)  // minutes ?
			{
				int m = (int) t/60/1000/1000;
				int s = (int) (t/1000/1000)%60;
				return string.Concat (m.ToString (), " m ", s.ToString (), " s");
			}
			else if (abs >= 1000L*1000L)  // secondes ?
			{
				double s = t/1000.0/1000.0;
				return string.Concat (s.ToString ("0.00"), " s");
			}
			else if (abs >= 1000L)  // millisecondes ?
			{
				int m = (int) t/1000;
				return string.Concat (m.ToString (), " ms");
			}
			else  // microsecondes ?
			{
				return string.Concat (t.ToString (), " µs");
			}
		}


		private static bool				globalSecondaryVisibility    = false;
		private static bool				globalQueryOptionsVisibility = false;
		private static bool				globalCaseSensitive          = true;
		private static bool				globalSubstitute             = true;
		private static bool				globalColorize               = true;
		private static bool				globalAutoBreak              = true;

		private readonly List<Query>	queries;

		private RadioButton				extendedButton;
		private RadioButton				basicButton;
		private RadioButton				offButton;
		private Button					updateButton;
		private Button					clearButton;
		private GlyphButton				secondaryButton;

		private FrameBox				secondaryToolbar;
		private GlyphButton				searchClearButton;
		private TextField				searchField;
		private Button					searchPrevButton;
		private Button					searchNextButton;
		private StaticText				searchCounterInfo;
		private CheckButton				caseSensitiveButton;
		private Button					exportButton;
		private Button					importButton;

		private CellTable				mainTable;
		private HSplitter				splitter1;
		private HSplitter				splitter2;

		private FrameBox				detailsFrame;
		private GlyphButton				queryOptionsButton;
		private FrameBox				queryOptionsToolbar;
		private CheckButton				substituteButton;
		private CheckButton				colorizeButton;
		private CheckButton				autoBreakButton;
		private TextFieldMulti			queryField;
		private FrameBox				detailsBox;

		private string					lastSearching;
		private bool					lastCaseSensitive;
	}
}
