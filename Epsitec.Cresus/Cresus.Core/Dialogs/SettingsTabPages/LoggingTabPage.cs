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
		static LoggingTabPage()
		{
			LoggingTabPage.globalNumberOfEntriesPerPage = 200;

			LoggingTabPage.globalLogResult     = true;
			LoggingTabPage.globalLogThreadName = true;
			LoggingTabPage.globalLogStackTrace = false;

			LoggingTabPage.globalSearchMode.CaseSensitive      = true;
			LoggingTabPage.globalSearchMode.SearchInQuery      = true;
			LoggingTabPage.globalSearchMode.SearchInParameters = true;
			LoggingTabPage.globalSearchMode.SearchInResults    = true;
			LoggingTabPage.globalSearchMode.SearchInCallStack  = false;

			LoggingTabPage.globalSubstitute = true;
			LoggingTabPage.globalColorize   = true;
			LoggingTabPage.globalAutoBreak  = true;
		}

		public LoggingTabPage(CoreApplication application)
			: base (application)
		{
			this.taggedText = new TaggedText ();
		}


		public override void AcceptChangings()
		{
		}

		public override void CreateUI(Widget parent)
		{
			this.recordLabel = new StaticText
			{
				Parent = parent,
				ContentAlignment = Common.Drawing.ContentAlignment.MiddleCenter,
				FormattedText = Misc.FontColorize (Misc.FontSize (Misc.Bold ("Enregistrement en cours..."), 36), Color.FromBrightness (0.8)),
				Anchor = AnchorStyles.All,
			};

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
			this.startButton.Clicked += delegate
			{
				this.LogEnable = !this.LogEnable;
				this.UpdateWidgets ();
			};

			this.logMenuButton.Clicked += delegate
			{
				this.LogMenu ();
			};

			this.showedMenuButton.Clicked += delegate
			{
				this.ShowedMenu ();
			};

			this.secondaryButton.Clicked += delegate
			{
				this.SecondarySwap ();
			};

			this.queryMenuButton.Clicked += delegate
			{
				this.QueryMenu ();
			};

			this.searchField.TextChanged += delegate
			{
				this.UpdateWidgets ();
			};

			this.searchMenuButton.Clicked += delegate
			{
				this.SearchMenu ();
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

			this.logSlider.ValueChanged += delegate
			{
				this.firstQueryShowed = (int) this.logSlider.Value * LoggingTabPage.globalNumberOfEntriesPerPage;

				this.UpdateTable ();
				this.UpdateDetails ();
				this.UpdateWidgets ();
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

			this.UpdateSlider ();
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

			this.startButton = new Button
			{
				Parent = header,
				PreferredWidth = 100,
				Dock = DockStyle.Left,
			};

			this.logMenuButton = new GlyphButton
			{
				Parent = header,
				GlyphShape = Common.Widgets.GlyphShape.Menu,
				Dock = DockStyle.Left,
				Margins = new Margins (-1, 10, 0, 0),
			};

			this.firstTime = new StaticText
			{
				Parent = header,
				PreferredWidth = 50,
				ContentAlignment = Common.Drawing.ContentAlignment.MiddleCenter,
				Dock = DockStyle.Left,
				Margins = new Margins (0, 0, 0, 0),
			};

			this.logSlider = new HSlider
			{
				Parent = header,
				Dock = DockStyle.Fill,
				Margins = new Margins (0, 0, 3, 3),
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

			this.showedMenuButton = new GlyphButton
			{
				Parent = header,
				GlyphShape = Common.Widgets.GlyphShape.Menu,
				Dock = DockStyle.Right,
				Margins = new Margins (10, 0, 0, 0),
			};

			this.lastTime = new StaticText
			{
				Parent = header,
				PreferredWidth = 50,
				ContentAlignment = Common.Drawing.ContentAlignment.MiddleCenter,
				Dock = DockStyle.Right,
				Margins = new Margins (0, 0, 0, 0),
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


			var box1 = new FrameBox
			{
				Parent = this.secondaryToolbar,
				Dock = DockStyle.Top,
			};

			this.searchClearButton = new GlyphButton
			{
				Parent = box1,
				GlyphShape = Common.Widgets.GlyphShape.Close,
				Dock = DockStyle.Left,
				Margins = new Margins (0, 10, 2, 2),
			};

			var label = new StaticText
			{
				Parent = box1,
				Text = "Rechercher",
				PreferredWidth = 64,
				Dock = DockStyle.Left,
			};

			this.searchField = new TextField
			{
				Parent = box1,
				Dock = DockStyle.Fill,
				Margins = new Margins (0, 0, 0, 0),
			};

			this.importButton = new Button
			{
				Parent = box1,
				Text = "Importer...",
				PreferredWidth = 70,
				Dock = DockStyle.Right,
				Margins = new Margins (1, 0, 0, 0),
			};

			this.exportButton = new Button
			{
				Parent = box1,
				Text = "Exporter...",
				PreferredWidth = 70,
				Dock = DockStyle.Right,
				Margins = new Margins (10, 0, 0, 0),
			};

			this.searchCounterInfo = new StaticText
			{
				Parent = box1,
				PreferredWidth = 150,
				Dock = DockStyle.Right,
				Margins = new Margins (15, 0, 0, 0),
			};

			this.searchMenuButton = new GlyphButton
			{
				Parent = box1,
				GlyphShape = Common.Widgets.GlyphShape.Menu,
				Dock = DockStyle.Right,
				Margins = new Margins (1, 0, 0, 0),
			};

			this.searchNextButton = new Button
			{
				Parent = box1,
				Text = "Suivant",
				PreferredWidth = 60,
				Dock = DockStyle.Right,
				Margins = new Margins (1, 0, 0, 0),
			};

			this.searchPrevButton = new Button
			{
				Parent = box1,
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
				PreferredWidth = 16,
				Dock = DockStyle.Left,
				Margins = new Margins (0, 1, 0, 0),
			};

			var rightBox = new FrameBox
			{
				Parent = box,
				Dock = DockStyle.Fill,
			};

			this.queryMenuButton = new GlyphButton
			{
				Parent = leftBox,
				GlyphShape = Common.Widgets.GlyphShape.Menu,
				PreferredSize = new Size (16, 24),
				Dock = DockStyle.Bottom,
			};

			this.queryField = new TextFieldMulti
			{
				Parent = rightBox,
				MaxLength = 100000,
				IsReadOnly = true,
				Dock = DockStyle.Fill,
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
				Margins = new Margins (10, 0, 10, 10),
			};

			this.stackField = new TextFieldMulti
			{
				Parent = parent,
				MaxLength = 100000,
				IsReadOnly = true,
				Dock = DockStyle.Bottom,
				Margins = new Margins (10, 10, 10, 0),
			};

			this.splitter3 = new HSplitter
			{
				Parent = parent,
				Dock = DockStyle.Bottom,
			};
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


		private void UpdateSlider()
		{
			int count = this.QueryTotalCount;

			if (count == 0)
			{
				this.logSlider.Enable = false;
				this.firstTime.Text = null;
				this.lastTime.Text = null;
			}
			else
			{
				this.logSlider.Enable = true;

				this.logSlider.Resolution  = 1;
				this.logSlider.MinValue    = 0;
				this.logSlider.MaxValue    = (count-1) / LoggingTabPage.globalNumberOfEntriesPerPage;
				this.logSlider.SmallChange = 1;
				this.logSlider.LargeChange = 1;
				this.logSlider.Value       = this.firstQueryShowed;

				var firstQuery = this.GetAbsoluteQuery (0);
				var lastQuery  = this.GetAbsoluteQuery (count-1);

				if (firstQuery != null)
				{
					this.firstTime.Text = firstQuery.GetShortStartTime ();
				}

				if (lastQuery != null)
				{
					this.lastTime.Text = lastQuery.GetShortStartTime ();
				}
			}
		}

		private void UpdateTable()
		{
			int rows = this.QueryShowedCount;
			this.mainTable.SetArraySize (8, rows);

			this.mainTable.SetWidthColumn (0,  40);
			this.mainTable.SetWidthColumn (1,  70);
			this.mainTable.SetWidthColumn (2,  60);
			this.mainTable.SetWidthColumn (3,  60);
			this.mainTable.SetWidthColumn (4, 790-40-70-60-60-120-120);
			this.mainTable.SetWidthColumn (5, 120);
			this.mainTable.SetWidthColumn (6, 120);
			this.mainTable.SetWidthColumn (7, 120);  // dépasse volontairement

			this.mainTable.SetHeaderTextH (0, "N°");
			this.mainTable.SetHeaderTextH (1, "Début");
			this.mainTable.SetHeaderTextH (2, "Temps");
			this.mainTable.SetHeaderTextH (3, "Durée");
			this.mainTable.SetHeaderTextH (4, "Requête");
			this.mainTable.SetHeaderTextH (5, "Paramètres");
			this.mainTable.SetHeaderTextH (6, "Résultats");
			this.mainTable.SetHeaderTextH (7, "Processus");

			ContentAlignment[] alignments =
			{
				ContentAlignment.MiddleRight,
				ContentAlignment.MiddleLeft,
				ContentAlignment.MiddleRight,
				ContentAlignment.MiddleRight,
				ContentAlignment.MiddleLeft,
				ContentAlignment.MiddleLeft,
				ContentAlignment.MiddleLeft,
				ContentAlignment.MiddleLeft,
			};

			string search = this.SearchText;
			bool caseSensitive = LoggingTabPage.globalSearchMode.CaseSensitive;

			int counter = 0;
			int lines = 0;

			for (int row=0; row<rows; row++)
			{
				var query = this.GetQuery (row);

				var values = QueryAccessor.GetMainContent (query, row, this.SearchText, LoggingTabPage.globalSearchMode);

				this.mainTable.FillRow (row, alignments);
				this.mainTable.UpdateRow (row, values);

				if (query != null)
				{
					if (!string.IsNullOrEmpty (search))
					{
						int n = query.Count (search, LoggingTabPage.globalSearchMode);
						if (n > 0)
						{
							counter += n;
							lines++;
						}
					}
				}
			}

			this.UpdateRelativeTime ();
			this.UpdateSearchCounter (counter, lines);
		}

		private void UpdateRelativeTime()
		{
			int rows = this.QueryShowedCount;
			int sel = this.mainTable.SelectedRow;
			
			if (sel == -1)
			{
				sel = 0;
			}

			if (sel < rows)
			{
				System.DateTime reference;

				var query = this.GetQuery (sel);

				if (query == null)
				{
					reference = System.DateTime.Now;
				}
				else
				{
					reference = query.StartTime;
				}

				for (int row=0; row<rows; row++)
				{
					var widget = this.mainTable.GetStaticText (row, 2);

					if (widget != null)
					{
						query = this.GetQuery (row);

						if (query == null)
						{
							widget.Text = null;
						}
						else
						{
							System.TimeSpan time = query.StartTime.Subtract (reference);
							widget.Text = LoggingTabPage.ToNiceString (time);
						}
					}
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

			if (sel == -1 || this.QueryShowedCount == 0)
			{
				this.queryField.FormattedText = null;
				this.stackField.FormattedText = null;
			}
			else
			{
				var query = this.GetQuery (sel);

				if (query == null)
				{
					this.queryField.FormattedText = null;
					this.stackField.FormattedText = null;
				}
				else
				{
					bool substitute = LoggingTabPage.globalSubstitute;
					bool colorize   = LoggingTabPage.globalColorize;
					bool autoBreak  = LoggingTabPage.globalAutoBreak;
					FormattedText content = query.GetQuery (substitute, colorize, autoBreak);
					content = QueryAccessor.GetTaggedText (content, this.SearchText, LoggingTabPage.globalSearchMode, LoggingTabPage.globalSearchMode.SearchInQuery);

					if (content.ToString ().Length >= this.queryField.MaxLength)
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

					content = QueryAccessor.GetTaggedText (query.GetStackTrace (), this.SearchText, LoggingTabPage.globalSearchMode, LoggingTabPage.globalSearchMode.SearchInCallStack);
					this.stackField.FormattedText = content;
				}
			}
		}

		private void UpdateWidgets()
		{
			bool record = this.LogEnable;
			bool enable = !string.IsNullOrEmpty (this.searchField.Text);
			bool empty = this.mainTable.Rows == 0;

			this.startButton.FormattedText = Misc.Bold (record ? "Stopper" : "Démarrer");

			this.recordLabel.Visibility = record;

			this.logMenuButton.Enable = !record;
			this.firstTime.Visibility = !record;
			this.logSlider.Visibility = !record;
			this.lastTime.Visibility = !record;
			this.showedMenuButton.Visibility = !record;
			this.clearButton.Visibility = !record;
			this.secondaryButton.Visibility = !record;

			this.secondaryToolbar.Visibility = !record;
			this.mainTable.Visibility = !record;
			this.splitter1.Visibility = !record;
			this.splitter2.Visibility = !record;
			this.splitter3.Visibility = !record;

			this.detailsFrame.Visibility = !record;
			this.queryMenuButton.Visibility = !record;
			this.queryField.Visibility = !record;
			this.detailsBox.Visibility = !record;
			this.stackField.Visibility = !record;

			this.searchClearButton.Enable = enable;
			this.searchPrevButton.Enable  = enable;
			this.searchNextButton.Enable  = enable;

			this.clearButton.Enable  = !empty;
			this.exportButton.Enable = !empty;

			this.secondaryToolbar.Visibility = LoggingTabPage.globalSecondaryVisibility;
			this.secondaryButton.GlyphShape  = LoggingTabPage.globalSecondaryVisibility ? Common.Widgets.GlyphShape.TriangleUp : Common.Widgets.GlyphShape.TriangleDown;
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
			if (this.QueryShowedCount == 0)
			{
				return;
			}

			string search = this.SearchText;

			if (this.lastSearching != search || !this.lastSearchMode.Compare (LoggingTabPage.globalSearchMode))
			{
				this.lastSearching = search;
				this.lastSearchMode.CopyFrom (LoggingTabPage.globalSearchMode);

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

					var query = this.GetQuery (sel);

					if (query != null && query.ContainsString (search, LoggingTabPage.globalSearchMode))
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
			this.queryLog = null;

			this.UpdateSlider ();
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
				var values = QueryAccessor.GetParameterContent (parameters[row], this.SearchText, LoggingTabPage.globalSearchMode);

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
				var values = QueryAccessor.GetTableResultsContent (table.Rows[row].Values, this.SearchText, LoggingTabPage.globalSearchMode);

				cellTable.FillRow (row, alignments.ToArray ());
				cellTable.UpdateRow (row, values);
			}

			return frame;
		}
		#endregion


		#region Menu
		private void LogMenu()
		{
			var menu = new VMenu ();

			this.AddItemToMenu (menu, LoggingTabPage.globalLogResult,     "LogResult",     "Enregistrer les résultats");
			this.AddItemToMenu (menu, LoggingTabPage.globalLogThreadName, "LogThreadName", "Enregistrer les noms de processus");
			this.AddItemToMenu (menu, LoggingTabPage.globalLogStackTrace, "LogStackTrace", "Enregistrer la pile complète");

			this.ShowMenu (menu, this.logMenuButton);
		}

		private void ShowedMenu()
		{
			var menu = new VMenu ();

			//?this.AddItemToMenu (menu, LoggingTabPage.globalNumberOfEntriesPerPage ==   10, "EntriesShowed.10",   "Montre 10 entrées à la fois (debug)");
			this.AddItemToMenu (menu, LoggingTabPage.globalNumberOfEntriesPerPage ==  100, "EntriesShowed.100",  "Montre 100 entrées à la fois");
			this.AddItemToMenu (menu, LoggingTabPage.globalNumberOfEntriesPerPage ==  200, "EntriesShowed.200",  "Montre 200 entrées à la fois (*)");
			this.AddItemToMenu (menu, LoggingTabPage.globalNumberOfEntriesPerPage ==  500, "EntriesShowed.500",  "Montre 500 entrées à la fois");
			//?this.AddItemToMenu (menu, LoggingTabPage.globalNumberOfEntriesPerPage == 1000, "EntriesShowed.1000", "Montre 1000 entrées à la fois");
			//?this.AddItemToMenu (menu, LoggingTabPage.globalNumberOfEntriesPerPage == 2000, "EntriesShowed.2000", "Montre 2000 entrées à la fois");

			this.ShowMenu (menu, this.showedMenuButton);
		}

		private void SearchMenu()
		{
			var menu = new VMenu ();

			this.AddItemToMenu (menu, LoggingTabPage.globalSearchMode.CaseSensitive,      "CaseSensitive",      "Respecter la casse");
			menu.Items.Add (new MenuSeparator ());
			this.AddItemToMenu (menu, LoggingTabPage.globalSearchMode.SearchInQuery,      "SearchInQuery",      "Rechercher dans les requêtes");
			this.AddItemToMenu (menu, LoggingTabPage.globalSearchMode.SearchInParameters, "SearchInParameters", "Rechercher dans les paramètres");
			this.AddItemToMenu (menu, LoggingTabPage.globalSearchMode.SearchInResults,    "SearchInResults",    "Rechercher dans les résultats");
			this.AddItemToMenu (menu, LoggingTabPage.globalSearchMode.SearchInCallStack,  "SearchInCallStack",  "Rechercher dans la pile");

			this.ShowMenu (menu, this.searchMenuButton);
		}

		private void QueryMenu()
		{
			var menu = new VMenu ();

			this.AddItemToMenu (menu, LoggingTabPage.globalSubstitute, "Substitute", "Substituer les paramètres");
			this.AddItemToMenu (menu, LoggingTabPage.globalColorize,   "Colorize",   "Coloriage syntaxique");
			this.AddItemToMenu (menu, LoggingTabPage.globalAutoBreak,  "AutoBreak",  "Retours à la ligne automatiques");

			this.ShowMenu (menu, this.queryMenuButton);
		}

		private void ShowMenu(VMenu menu, Widget parent)
		{
			TextFieldCombo.AdjustComboSize (parent, menu, false);

			menu.Host = parent;
			menu.ShowAsComboList (parent, Point.Zero, parent);
		}

		private void AddItemToMenu(VMenu menu, bool check, string name, string text)
		{
			string icon = check ? "ActiveYes" : "ActiveNo";
			var item = new MenuItem (name, Misc.GetResourceIconUri (icon), text, null, name);

			item.Clicked += delegate
			{
				this.MenuAction (name);
			};

			menu.Items.Add (item);
		}

		private void MenuAction(string name)
		{
			if (name.StartsWith ("EntriesShowed."))
			{
				LoggingTabPage.globalNumberOfEntriesPerPage = int.Parse (name.Substring (14));
				this.UpdateSlider ();
				this.UpdateTable ();
				this.UpdateWidgets ();
			}
			else
			{
				switch (name)
				{
					case "LogResult":
						LoggingTabPage.globalLogResult = !LoggingTabPage.globalLogResult;
						break;

					case "LogThreadName":
						LoggingTabPage.globalLogThreadName = !LoggingTabPage.globalLogThreadName;
						break;

					case "LogStackTrace":
						LoggingTabPage.globalLogStackTrace = !LoggingTabPage.globalLogStackTrace;
						break;

					case "CaseSensitive":
						LoggingTabPage.globalSearchMode.CaseSensitive = !LoggingTabPage.globalSearchMode.CaseSensitive;
						this.Search (0);
						break;


					case "SearchInQuery":
						LoggingTabPage.globalSearchMode.SearchInQuery = !LoggingTabPage.globalSearchMode.SearchInQuery;
						this.Search (0);
						break;

					case "SearchInParameters":
						LoggingTabPage.globalSearchMode.SearchInParameters = !LoggingTabPage.globalSearchMode.SearchInParameters;
						this.Search (0);
						break;

					case "SearchInResults":
						LoggingTabPage.globalSearchMode.SearchInResults = !LoggingTabPage.globalSearchMode.SearchInResults;
						this.Search (0);
						break;

					case "SearchInCallStack":
						LoggingTabPage.globalSearchMode.SearchInCallStack = !LoggingTabPage.globalSearchMode.SearchInCallStack;
						this.Search (0);
						break;


					case "Substitute":
						LoggingTabPage.globalSubstitute = !LoggingTabPage.globalSubstitute;
						this.UpdateDetails ();
						this.UpdateWidgets ();
						break;

					case "Colorize":
						LoggingTabPage.globalColorize = !LoggingTabPage.globalColorize;
						this.UpdateDetails ();
						this.UpdateWidgets ();
						break;

					case "AutoBreak":
						LoggingTabPage.globalAutoBreak = !LoggingTabPage.globalAutoBreak;
						this.UpdateDetails ();
						this.UpdateWidgets ();
						break;
				}

				this.UpdateWidgets ();
			}
		}
		#endregion


		private string SearchText
		{
			get
			{
				string search = this.searchField.Text;

				if (!string.IsNullOrEmpty (search) && !LoggingTabPage.globalSearchMode.CaseSensitive)
				{
					search = Misc.RemoveAccentsToLower (search);
				}

				return search;
			}
		}


		private bool LogEnable
		{
			get
			{
				var db = this.application.Data.DataInfrastructure.DbInfrastructure;

				return db.QueryLog != null;
			}
			set
			{
				var db = this.application.Data.DataInfrastructure.DbInfrastructure;

				if (value)  // démarre l'enregistrement ?
				{
					db.EnableLogging ();

					db.QueryLog.LogResult     = LoggingTabPage.globalLogResult;
					db.QueryLog.LogStackTrace = LoggingTabPage.globalLogStackTrace;
					db.QueryLog.LogThreadName = LoggingTabPage.globalLogThreadName;

					this.UpdateWidgets ();
				}
				else  // stoppe l'enregistrement ?
				{
					this.queryLog = db.QueryLog;

					db.DisableLogging ();

					this.UpdateSlider ();
					this.UpdateTable ();
					this.UpdateDetails ();
					this.UpdateWidgets ();
				}
			}
		}


		private int QueryTotalCount
		{
			get
			{
				if (this.queryLog != null)
				{
					return this.queryLog.GetNbEntries ();
				}

				return 0;
			}
		}

		private int QueryShowedCount
		{
			get
			{
				return System.Math.Min (this.QueryTotalCount-this.firstQueryShowed, LoggingTabPage.globalNumberOfEntriesPerPage);
			}
		}

		private Query GetQuery(int index)
		{
			return this.GetAbsoluteQuery (this.firstQueryShowed+index);
		}

		private Query GetAbsoluteQuery(int index)
		{
			if (this.queryLog != null &&
				index >= 0 && index < this.QueryTotalCount)
			{
				return this.queryLog.GetEntry (index);
			}

			return null;
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


		private static int					globalNumberOfEntriesPerPage;
		private static bool					globalLogResult;
		private static bool					globalLogThreadName;
		private static bool					globalLogStackTrace;
		private static bool					globalSecondaryVisibility;
		private static SearchMode			globalSearchMode;
		private static bool					globalSubstitute;
		private static bool					globalColorize;
		private static bool					globalAutoBreak;

		private readonly TaggedText			taggedText;

		private AbstractLog					queryLog;
		private int							firstQueryShowed;

		private StaticText					recordLabel;

		private Button						startButton;
		private GlyphButton					logMenuButton;
		private StaticText					firstTime;
		private HSlider						logSlider;
		private StaticText					lastTime;
		private GlyphButton					showedMenuButton;
		private Button						clearButton;
		private GlyphButton					secondaryButton;

		private FrameBox					secondaryToolbar;
		private GlyphButton					searchClearButton;
		private TextField					searchField;
		private Button						searchPrevButton;
		private Button						searchNextButton;
		private GlyphButton					searchMenuButton;
		private StaticText					searchCounterInfo;
		private Button						exportButton;
		private Button						importButton;

		private CellTable					mainTable;
		private HSplitter					splitter1;
		private HSplitter					splitter2;
		private HSplitter					splitter3;

		private FrameBox					detailsFrame;
		private GlyphButton					queryMenuButton;
		private TextFieldMulti				queryField;
		private FrameBox					detailsBox;
		private TextFieldMulti				stackField;

		private string						lastSearching;
		private SearchMode					lastSearchMode;
	}
}
