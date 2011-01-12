//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Debug;
using Epsitec.Common.Dialogs;
using Epsitec.Common.Drawing;
using Epsitec.Common.IO;
using Epsitec.Common.Printing;
using Epsitec.Common.Support;
using Epsitec.Common.Types;
using Epsitec.Common.Widgets;

using Epsitec.Common.Support.EntityEngine;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Widgets;
using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Business.Accounting;

using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Epsitec.Cresus.Core.Dialogs.SettingsTabPages
{
	/// <summary>
	/// Onglet de SettingsDialog pour choisir les plans comptables.
	/// </summary>
	public class ChartOfAccountsTabPage : AbstractSettingsTabPage
	{
		public ChartOfAccountsTabPage(CoreApplication application)
			: base (application)
		{
			this.businessContext = this.application.Data.CreateBusinessContext ();

			//?this. = Logic.Current.FinanceSettings;  // TODO: Logic.Current est null !
			this.financeSettingsEntity = this.application.FinanceSettings;

			if (this.financeSettingsEntity == null)
			{
				this.financeSettingsEntity = this.businessContext.CreateEntity<FinanceSettingsEntity> ();
			}
		}


		public override void AcceptChangings()
		{
			this.businessContext.SaveChanges ();
		}

		public override void CreateUI(Widget parent)
		{
			int tabIndex = 0;

			var frame = new FrameBox
			{
				Parent = parent,
				Dock = DockStyle.Fill,
				Margins = new Margins (10),
			};

			var leftPane = new FrameBox
			{
				Parent = frame,
				PreferredWidth = 160+70+70+90 + 20,
				Dock = DockStyle.Left,
				Margins = new Margins (0, 0, 0, 0),
				TabIndex = tabIndex++,
			};

			//	Crée le panneau de gauche.
			new StaticText
			{
				Parent = leftPane,
				PreferredHeight = 20,
				Dock = DockStyle.Top,
				Text = "<font size=\"16\">Liste des plans comptables</font>",
				Margins = new Margins (0, 0, 0, 10),
			};

			{
				//	Crée la toolbar.
				double buttonSize = 19;

				this.toolbar = UIBuilder.CreateMiniToolbar (leftPane, buttonSize);
				this.toolbar.Margins = new Margins (0, 0, 0, -1);
				this.toolbar.TabIndex = tabIndex++;

				this.addButton = new GlyphButton
				{
					Parent = this.toolbar,
					PreferredSize = new Size (buttonSize*2+1, buttonSize),
					GlyphShape = GlyphShape.Plus,
					Margins = new Margins (0, 0, 0, 0),
					Dock = DockStyle.Left,
				};

				this.removeButton = new GlyphButton
				{
					Parent = this.toolbar,
					PreferredSize = new Size (buttonSize, buttonSize),
					GlyphShape = GlyphShape.Minus,
					Margins = new Margins (1, 0, 0, 0),
					Dock = DockStyle.Left,
				};

				ToolTip.Default.SetToolTip (this.addButton,    "Ajoute un plan comptable");
				ToolTip.Default.SetToolTip (this.removeButton, "Supprime le un plan comptable sélectionné");
			}

			{
				//	Crée la liste.
				var tile = new FrameBox
				{
					Parent = leftPane,
					Dock = DockStyle.Fill,
					DrawFullFrame = true,
					TabIndex = tabIndex++,
				};

				this.table = new CellTable
				{
					Parent = tile,
					StyleH = CellArrayStyles.Separator,
					StyleV = CellArrayStyles.ScrollNorm | CellArrayStyles.Separator | CellArrayStyles.SelectLine,
					Margins = new Margins (2),
					Dock = DockStyle.Fill,
					TabIndex = tabIndex++,
				};

				this.table.SetArraySize (4, 0);
				this.table.SetWidthColumn (0, 160);
				this.table.SetWidthColumn (1, 70);
				this.table.SetWidthColumn (2, 70);
				this.table.SetWidthColumn (3, 90);
			}

			//	Connexion des événements.
			this.addButton.Clicked += delegate
			{
				this.AddAction ();
			};

			this.removeButton.Clicked += delegate
			{
				this.RemoveAction ();
			};

			this.table.SelectionChanged += delegate
			{
				this.UpdateWidgets ();
			};

			this.UpdateTable ();
			this.UpdateWidgets ();
		}

		private void UpdateWidgets()
		{
			int sel = this.table.SelectedRow;

			this.removeButton.Enable = (sel != -1);
		}


		private void UpdateTable(int? sel = null)
		{
			//	Met à jour le contenu de la table.
			this.ignoreChange = true;

			int rows = this.financeSettingsEntity.GetChartsOfAccounts ().Count;
			this.table.SetArraySize (4, rows);

			if (sel == null)
			{
				sel = this.table.SelectedRow;
			}

			for (int row=0; row<rows; row++)
			{
				this.TableFillRow (row);
				this.TableUpdateRow (row);
			}

			sel = System.Math.Min (sel.Value, rows-1);
			if (sel != -1)
			{
				this.table.SelectRow (sel.Value, true);
			}

			this.ignoreChange = false;
		}

		private void TableFillRow(int row)
		{
			//	Peuple une ligne de la table, si nécessaire.
			if (this.table[0, row].IsEmpty)
			{
				var text = new StaticText
				{
					ContentAlignment = ContentAlignment.MiddleLeft,
					Dock = DockStyle.Fill,
					Margins = new Margins (4, 4, 0, 0),
				};

				this.table[0, row].Insert (text);
			}

			if (this.table[1, row].IsEmpty)
			{
				var text = new StaticText
				{
					ContentAlignment = ContentAlignment.MiddleLeft,
					Dock = DockStyle.Fill,
					Margins = new Margins (4, 4, 0, 0),
				};

				this.table[1, row].Insert (text);
			}

			if (this.table[2, row].IsEmpty)
			{
				var text = new StaticText
				{
					ContentAlignment = ContentAlignment.MiddleLeft,
					Dock = DockStyle.Fill,
					Margins = new Margins (4, 4, 0, 0),
				};

				this.table[2, row].Insert (text);
			}

			if (this.table[3, row].IsEmpty)
			{
				var text = new StaticText
				{
					ContentAlignment = ContentAlignment.MiddleRight,
					Dock = DockStyle.Fill,
					Margins = new Margins (4, 4, 0, 0),
				};

				this.table[3, row].Insert (text);
			}
		}

		private void TableUpdateRow(int row)
		{
			//	Met à jour le contenu d'une ligne de la table.
			var chartOfAccount = this.financeSettingsEntity.GetChartsOfAccounts ().ElementAt (row);

			{
				var text = this.table[0, row].Children[0] as StaticText;
				text.FormattedText = chartOfAccount.Title;
			}

			{
				var text = this.table[1, row].Children[0] as StaticText;
				text.FormattedText = chartOfAccount.BeginDate.ToString ();
			}

			{
				var text = this.table[2, row].Children[0] as StaticText;
				text.FormattedText = chartOfAccount.EndDate.ToString ();
			}

			{
				var text = this.table[3, row].Children[0] as StaticText;
				text.FormattedText = TextFormatter.FormatText (chartOfAccount.Items.Count.ToString (), "comptes");
			}

			this.table.SelectRow (row, false);
		}


		private void AddAction()
		{
			string filename = this.OpenFileDialog (this.application);

			if (!string.IsNullOrEmpty (filename))
			{
				CresusChartOfAccounts chart = CresusChartOfAccountsConnector.Load (filename);

				string err = this.CheckChart (chart);

				if (string.IsNullOrEmpty (err))  // ok ?
				{
					this.financeSettingsEntity.AddChartOfAccounts (this.businessContext, chart);

					this.UpdateTable ();
					this.UpdateWidgets ();
				}
				else  // erreur ?
				{
					MessageDialog.CreateOk ("Erreur", DialogIcon.Warning, err).OpenDialog ();
				}
			}
		}

		private void RemoveAction()
		{
		}


		private string OpenFileDialog(CoreApplication application)
		{
			var dialog = new FileOpenDialog ();

			dialog.Title = "Importation d'un plan comptable \"CRP\"";
			dialog.InitialDirectory = this.initialDirectory;
			//?dialog.FileName = "";

			dialog.Filters.Add ("crp", "Plan comptable", "*.crp");
			dialog.Filters.Add ("any", "Tous les fichiers", "*.*");

			dialog.AcceptMultipleSelection = false;
			dialog.Owner = application.Window;
			dialog.OpenDialog ();
			if (dialog.Result != Common.Dialogs.DialogResult.Accept)
			{
				return null;
			}

			this.initialDirectory = dialog.InitialDirectory;

			return dialog.FileName;
		}

		private string CheckChart(CresusChartOfAccounts chart)
		{
			foreach (var c in this.financeSettingsEntity.GetChartsOfAccounts ())
			{
				if (chart.BeginDate < c.EndDate && chart.EndDate > c.BeginDate)
				{
					return string.Format ("Il y a déjà un plan comptable couvrant la période du {0} au {1}.", chart.BeginDate.ToString (), chart.EndDate.ToString ());
				}
			}

			return null;  // ok
		}


		private readonly BusinessContext				businessContext;
		private readonly FinanceSettingsEntity			financeSettingsEntity;

		private FrameBox								toolbar;
		private GlyphButton								addButton;
		private GlyphButton								removeButton;
		private CellTable								table;
		private string									initialDirectory;
		private bool									ignoreChange;
	}
}
