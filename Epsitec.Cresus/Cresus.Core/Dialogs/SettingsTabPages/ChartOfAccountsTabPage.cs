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
using Epsitec.Cresus.DataLayer.Context;

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
			//	Il faut utiliser un BusinessContext propre.
			this.businessContext = this.application.Data.CreateBusinessContext ();

			//	Cherche l'entité sur le compte du BusinessContext propre.
			DataContext dataContext = this.businessContext.DataContext;
			CoreData coreData = this.businessContext.Data;
			this.financeSettingsEntity = coreData.GetAllEntities<FinanceSettingsEntity> (DataExtractionMode.Default, dataContext).FirstOrDefault ();

			//	Si elle n'existe pas, on la crée.
			if (this.financeSettingsEntity == null)
			{
				this.financeSettingsEntity = this.businessContext.CreateEntity<FinanceSettingsEntity> ();
			}

			//	Obtient une copie de la liste des plans comptables.
			this.chartOfAccounts = new List<CresusChartOfAccounts> ();
			this.chartOfAccounts.AddRange (this.financeSettingsEntity.GetChartsOfAccounts ());
		}


		public override void AcceptChangings()
		{
			this.UpdateFinanceSettingsEntity ();
			this.businessContext.Dispose ();
		}

		public override void RejectChangings()
		{
			this.businessContext.Discard ();
			this.businessContext.Dispose ();
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
				double buttonSize = 23;

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
					StyleH = CellArrayStyles.Separator | CellArrayStyles.Header,
					StyleV = CellArrayStyles.ScrollNorm | CellArrayStyles.Separator | CellArrayStyles.SelectLine,
					Margins = new Margins (2),
					Dock = DockStyle.Fill,
					TabIndex = tabIndex++,
				};
			}

			new StaticText
			{
				Parent = leftPane,
				Dock = DockStyle.Bottom,
				Text = "<i>Remarque: Il ne peut pas y avoir plus d'un plan comptable par période.</i>",
				Margins = new Margins (0, 0, 10, 0),
			};

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

			int rows = this.chartOfAccounts.Count;
			this.table.SetArraySize (4, rows);

			this.table.SetWidthColumn (0, 160);
			this.table.SetWidthColumn (1, 70);
			this.table.SetWidthColumn (2, 70);
			this.table.SetWidthColumn (3, 90);

			this.table.SetHeaderTextH (0, "Nom");
			this.table.SetHeaderTextH (1, "Du");
			this.table.SetHeaderTextH (2, "Au");
			this.table.SetHeaderTextH (3, "Nb de comptes");

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
					Margins = new Margins (4, 8, 0, 0),
				};

				this.table[3, row].Insert (text);
			}
		}

		private void TableUpdateRow(int row)
		{
			//	Met à jour le contenu d'une ligne de la table.
			var chartOfAccount = this.chartOfAccounts[row];

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
				text.FormattedText = chartOfAccount.Items.Count.ToString ();
			}

			this.table.SelectRow (row, false);
		}


		private void AddAction()
		{
			//	Bouton [+] cliqué.
			string[] filenames = this.OpenFileDialog (this.application);  // choix des fichiers à ouvrir...

			if (filenames != null)
			{
				foreach (var filename in filenames)
				{
					CresusChartOfAccounts chart = CresusChartOfAccountsConnector.Load (filename);

					string err = this.CheckChart (chart);

					if (string.IsNullOrEmpty (err))  // ok ?
					{
						this.chartOfAccounts.Add (chart);

						this.UpdateTable (this.chartOfAccounts.Count-1);
						this.UpdateWidgets ();
					}
					else  // erreur ?
					{
						MessageDialog.CreateOk ("Erreur", DialogIcon.Warning, err).OpenDialog ();
						break;
					}
				}
			}
		}

		private void RemoveAction()
		{
			//	Bouton [-] cliqué.
			int sel = this.table.SelectedRow;

			if (sel >= 0 && sel < this.chartOfAccounts.Count)
			{
				this.chartOfAccounts.RemoveAt (sel);

				this.UpdateTable ();
				this.UpdateWidgets ();
			}
		}


		private string[] OpenFileDialog(CoreApplication application)
		{
			//	Demande quels plans comptables ouvrir.
			var dialog = new FileOpenDialog ();

			dialog.Title = "Importation d'un plan comptable \"CRP\"";
			dialog.InitialDirectory = this.initialDirectory;

			dialog.Filters.Add ("crp", "Plan comptable", "*.crp");
			dialog.Filters.Add ("any", "Tous les fichiers", "*.*");

			dialog.AcceptMultipleSelection = true;
			dialog.Owner = application.Window;
			dialog.OpenDialog ();
			if (dialog.Result != Common.Dialogs.DialogResult.Accept)
			{
				return null;
			}

			this.initialDirectory = dialog.InitialDirectory;

			return dialog.FileNames;
		}

		private string CheckChart(CresusChartOfAccounts chart)
		{
			//	Vérifie si un plan comptable peut être ouvert.
			//	Retourne null si l'ouverture est possible, ou un message en cas d'erreur.
			foreach (var c in this.chartOfAccounts)
			{
				if (chart.BeginDate < c.EndDate && chart.EndDate > c.BeginDate)
				{
					return string.Format ("Il y a déjà un plan comptable couvrant la période du {0} au {1}.", chart.BeginDate.ToString (), chart.EndDate.ToString ());
				}
			}

			return null;  // ok
		}


		private void UpdateFinanceSettingsEntity()
		{
			//	Met à jour l'entité FinanceSettingsEntity en fonction de la liste des plans comptables
			//	dans this.chartOfAccounts.
			bool dirty = false;
			var originalCharts = this.financeSettingsEntity.GetChartsOfAccounts ();

			//	Supprime les plans comptables qui ne sont plus utilisés.
			bool removed;
			do
			{
				removed = false;
				for (int i=0; i<originalCharts.Count; i++)
				{
					var originalChart = originalCharts[i];

					var c = this.chartOfAccounts.Where (x => x.Id == originalChart.Id).FirstOrDefault ();
					if (c == null)  // pas utilisé dans la nouvelle liste ?
					{
						this.financeSettingsEntity.RemoveChartOfAccounts (this.businessContext, originalChart);
						dirty = true;
						removed = true;
						break;
					}
				}
			}
			while (removed);

			//	Ajoute les nouveaux plans comptables.
			foreach (var newChart in this.chartOfAccounts)
			{
				var c = originalCharts.Where (x => x.Id == newChart.Id).FirstOrDefault ();
				if (c == null)  // pas utilisé dans l'ancienne liste ?
				{
					this.financeSettingsEntity.AddChartOfAccounts (this.businessContext, newChart);
					dirty = true;
				}
			}

			if (dirty)
			{
				this.businessContext.SaveChanges ();
			}
		}


		private readonly BusinessContext				businessContext;
		private readonly FinanceSettingsEntity			financeSettingsEntity;
		private readonly List<CresusChartOfAccounts>	chartOfAccounts;

		private FrameBox								toolbar;
		private GlyphButton								addButton;
		private GlyphButton								removeButton;
		private CellTable								table;
		private string									initialDirectory;
		private bool									ignoreChange;
	}
}
