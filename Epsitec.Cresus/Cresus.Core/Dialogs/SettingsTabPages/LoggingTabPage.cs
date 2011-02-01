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

			var bottomFrame = new FrameBox
			{
				Parent = parent,
				PreferredHeight = 200,
				Dock = DockStyle.Bottom,
				Margins = new Margins (10, 0, 10, 10),
			};

			this.splitter = new HSplitter
			{
				Parent = parent,
				Dock = DockStyle.Bottom,
			};

			{
				var header = new FrameBox
				{
					Parent = topFrame,
					Dock = DockStyle.Top,
					Margins = new Margins (0, 0, 0, 10),
				};

				this.extendedButton = new RadioButton
				{
					Parent = header,
					Text = "Trace complète",
					PreferredWidth = 100,
					AutoToggle = false,
					Dock = DockStyle.Left,
				};

				this.basicButton = new RadioButton
				{
					Parent = header,
					Text = "Trace réduite",
					PreferredWidth = 90,
					AutoToggle = false,
					Dock = DockStyle.Left,
				};

				this.offButton = new RadioButton
				{
					Parent = header,
					Text = "Pas de trace",
					PreferredWidth = 80,
					AutoToggle = false,
					Dock = DockStyle.Left,
				};

				this.importButton = new Button
				{
					Parent = header,
					Text = "Importer...",
					PreferredWidth = 70,
					Dock = DockStyle.Right,
					Margins = new Margins (1, 0, 0, 0),
				};

				this.exportButton = new Button
				{
					Parent = header,
					Text = "Exporter...",
					PreferredWidth = 70,
					Dock = DockStyle.Right,
					Margins = new Margins (10, 0, 0, 0),
				};

				this.clearButton = new Button
				{
					Parent = header,
					Text = "Vider",
					PreferredWidth = 50,
					Dock = DockStyle.Right,
					Margins = new Margins (10, 0, 0, 0),
				};

				this.nextButton = new Button
				{
					Parent = header,
					Text = "Suivant",
					PreferredWidth = 60,
					Dock = DockStyle.Right,
					Margins = new Margins (1, 0, 0, 0),
				};

				this.prevButton = new Button
				{
					Parent = header,
					Text = "Précédent",
					PreferredWidth = 60,
					Dock = DockStyle.Right,
					Margins = new Margins (1, 0, 0, 0),
				};

				this.searchField = new TextField
				{
					Parent = header,
					Dock = DockStyle.Fill,
					Margins = new Margins (10, 0, 0, 0),
				};
			}

			this.table = new CellTable
			{
				Parent = topFrame,
				StyleH = CellArrayStyles.ScrollNorm | CellArrayStyles.Separator | CellArrayStyles.Header | CellArrayStyles.Mobile,
				StyleV = CellArrayStyles.ScrollNorm | CellArrayStyles.Separator | CellArrayStyles.SelectLine,
				Dock = DockStyle.Fill,
			};

			{
				this.queryField = new TextFieldMulti
				{
					Parent = bottomFrame,
					IsReadOnly = true,
					PreferredHeight = 66,
					Dock = DockStyle.Top,
					Margins = new Margins (0, 10, 0, 10),
				};

				this.detailsBox = new FrameBox
				{
					Parent = bottomFrame,
					PreferredHeight = 100,
					Dock = DockStyle.Fill,
				};
			}

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

			this.searchField.TextChanged += delegate
			{
				this.UpdateWidgets ();
			};

			this.prevButton.Clicked += delegate
			{
				this.Search (-1);
			};

			this.nextButton.Clicked += delegate
			{
				this.Search (1);
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

			this.table.SelectionChanged += delegate
			{
				this.UpdateDetails ();
			};

			this.UpdateRadio ();
			this.UpdateTable ();
			this.UpdateDetails ();
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

			int count = (db.QueryLog == null) ? 0 : db.QueryLog.GetNbEntries ();

			this.table.SetArraySize (6, count);

			this.table.SetWidthColumn (0,  40);
			this.table.SetWidthColumn (1, 110);
			this.table.SetWidthColumn (2,  70);
			this.table.SetWidthColumn (3, 350);
			this.table.SetWidthColumn (4, 110);
			this.table.SetWidthColumn (5, 110);

			this.table.SetHeaderTextH (0, "N°");
			this.table.SetHeaderTextH (1, "Début");
			this.table.SetHeaderTextH (2, "Durée");
			this.table.SetHeaderTextH (3, "Requête");
			this.table.SetHeaderTextH (4, "Paramètres");
			this.table.SetHeaderTextH (5, "Résultats");

			ContentAlignment[] alignments =
			{
				ContentAlignment.MiddleRight,
				ContentAlignment.MiddleLeft,
				ContentAlignment.MiddleRight,
				ContentAlignment.MiddleLeft,
				ContentAlignment.MiddleLeft,
				ContentAlignment.MiddleLeft
			};

			for (int row=0; row<count; row++)
			{
				var values = LoggingTabPage.GetQueryStrings (db.QueryLog.GetEntry (row), row);

				LoggingTabPage.TableFillRow (this.table, row, alignments);
				LoggingTabPage.TableUpdateRow (this.table, row, values);
			}
		}

		private void UpdateDetails()
		{
			this.detailsBox.Children.Clear ();

			int sel = this.table.SelectedRow;

			if (sel == -1)
			{
				this.queryField.Visibility = false;
				this.queryField.Text = null;
			}
			else
			{
				var db = this.application.Data.DataInfrastructure.DbInfrastructure;
				var query = db.QueryLog.GetEntry (sel);

				this.queryField.Visibility = true;
				this.queryField.FormattedText = LoggingTabPage.GetQuery (query);

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
			bool empty = this.table.Rows == 0;

			this.prevButton.Enable = enable;
			this.nextButton.Enable = enable;

			this.clearButton.Enable  = !empty;
			this.exportButton.Enable = !empty;
		}


		private void Search(int direction)
		{
			int count = this.table.Rows;
			int sel = this.table.SelectedRow;

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

				if (this.ContainsString (sel, this.searchField.Text))
				{
					this.table.DeselectAll ();
					this.table.SelectRow (sel, true);
					this.table.ShowSelect ();
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


		#region Create shower
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
				var values = LoggingTabPage.GetParameterStrings (parameters[row]);

				LoggingTabPage.TableFillRow (cellTable, row, alignments);
				LoggingTabPage.TableUpdateRow (cellTable, row, values);
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
				var values = LoggingTabPage.GetTableResultsStrings (table.Rows[row].Values);

				LoggingTabPage.TableFillRow (cellTable, row, alignments.ToArray ());
				LoggingTabPage.TableUpdateRow (cellTable, row, values);
			}

			return frame;
		}
		#endregion


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


		#region Search engine
		private bool ContainsString(int row, string search)
		{
			//	Retourne true si le texte à chercher se trouve dans une ligne donnée.
			foreach (var text in this.GetSearchableStrings (row))
			{
				if (text.Contains (search))
				{
					return true;
				}
			}

			return false;
		}

		private IEnumerable<string> GetSearchableStrings(int row)
		{
			//	Retourne tous les textes où chercher pour une ligne donnée.
			var db = this.application.Data.DataInfrastructure.DbInfrastructure;

			if (db.QueryLog != null)
			{
				var query = db.QueryLog.GetEntry (row);

				yield return query.SourceCode;
				yield return LoggingTabPage.GetCompactQueryParameters (query);
				yield return LoggingTabPage.GetCompactQueryResults (query);
			}
		}
		#endregion


		#region Query reader
		private static string[] GetQueryStrings(Query query, int row)
		{
			//	Retourne les textes pour peupler une ligne du tableau supérieur principal.
			var values = new List<string> ();

			values.Add ((row+1).ToString ());
			values.Add (query.StartTime.ToString ());
			values.Add (LoggingTabPage.GetNiceDuration (query.Duration));
			values.Add (LoggingTabPage.GetQuery (query).ToString ());
			values.Add (LoggingTabPage.GetCompactQueryParameters (query));
			values.Add (LoggingTabPage.GetCompactQueryResults (query));

			return values.ToArray ();
		}

		private static string GetCompactQueryParameters(Query query)
		{
			//	Retourne tous les paramètres sous une forme compacte.
			return string.Join (", ", query.Parameters.Select (x => x.Value));
		}

		private static string GetCompactQueryResults(Query query)
		{
			//	Retourne tous les réaultats sous une forme compacte.
			if (query.Result == null)
			{
				return "";
			}

			var list = new List<string> ();

			foreach (var table in query.Result.Tables)
			{
				foreach (var row in table.Rows)
				{
					foreach (var value in row.Values)
					{
						if (value != null)
						{
							string s = value.ToString ();

							if (!string.IsNullOrWhiteSpace (s))
							{
								list.Add (s);
							}
						}
					}
				}
			}

			return string.Join (", ", list);
		}

		private static string[] GetParameterStrings(Parameter parameter)
		{
			//	Retourne les textes pour peupler une ligne du tableau des paramètres.
			var values = new List<string> ();

			values.Add (parameter.Name);
			values.Add (parameter.Value.ToString ());

			return values.ToArray ();
		}

		private static string[] GetTableResultsStrings(ReadOnlyCollection<object> objects)
		{
			//	Retourne les textes pour peupler une ligne du tableau des résultats.
			var values = new List<string> ();

			foreach (var obj in objects)
			{
				values.Add (obj.ToString ());
			}

			return values.ToArray ();
		}


		private static FormattedText GetQuery(Query query, bool substitution = true)
		{
			//	Retourne le texte de la requête sql, avec ou sans substitution des paramètres.
			var text = query.SourceCode.Replace ("\n", "");

			if (substitution)
			{
				foreach (var parameter in query.Parameters)
				{
					var value = parameter.Value.ToString ();

					if (!string.IsNullOrEmpty (value))
					{
						text = text.Replace (parameter.Name, Misc.Bold (value).ToString ());
					}
				}
			}

			return text;
		}


		private static string GetNiceDuration(System.TimeSpan duration)
		{
			//	Retourne une durée sous une jolie forme.
			return string.Concat ((duration.Ticks/10).ToString (), " μs");  // un Tick vaut 100 nanosecondes
		}
		#endregion


		#region CellTable helpers
		private static void TableFillRow(CellTable table, int row, params ContentAlignment[] alignments)
		{
			//	Peuple une ligne d'une table, si nécessaire.
			for (int column = 0; column < alignments.Count (); column++)
			{
				if (table[column, row].IsEmpty)
				{
					var text = new StaticText
					{
						ContentAlignment = alignments[column],
						TextBreakMode = Common.Drawing.TextBreakMode.Ellipsis | Common.Drawing.TextBreakMode.Split | Common.Drawing.TextBreakMode.SingleLine,
						Dock = DockStyle.Fill,
						Margins = new Margins (4, 4, 0, 0),
					};

					table[column, row].Insert (text);
				}
			}
		}

		private static void TableUpdateRow(CellTable table, int row, params string[] values)
		{
			//	Met à jour le contenu d'une ligne d'une table.
			for (int column = 0; column < values.Count (); column++)
			{
				var text = table[column, row].Children[0] as StaticText;
				text.Text = values[column];
			}
		}
		#endregion


		private RadioButton			extendedButton;
		private RadioButton			basicButton;
		private RadioButton			offButton;
		private TextField			searchField;
		private Button				prevButton;
		private Button				nextButton;
		private Button				clearButton;
		private Button				exportButton;
		private Button				importButton;
		private CellTable			table;
		private HSplitter			splitter;
		private TextFieldMulti		queryField;
		private FrameBox			detailsBox;
	}
}
