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

namespace Epsitec.Cresus.Core.Controllers.ComplexControllers
{
	/// <summary>
	/// Permet de choisir les plans comptables.
	/// </summary>
	public class ChartsOfAccountsController
	{
		public ChartsOfAccountsController(BusinessContext businessContext, FinanceSettingsEntity financeSettingsEntity)
		{
			//	Il faut utiliser un BusinessContext propre.
			System.Diagnostics.Debug.Assert (financeSettingsEntity != null);

			this.businessContext       = businessContext;
			this.financeSettingsEntity = financeSettingsEntity;
		}


		public void CreateUI(Widget parent)
		{
			int tabIndex = 0;

			var frame = new FrameBox
			{
				Parent = parent,
				PreferredHeight = 200,  // hauteur arbitraire permettant de voir environ 5 plans comptables sans scrolling
				Dock = DockStyle.Top,
				Margins = new Margins (0, Library.UI.Constants.RightMargin, 0, 0),
				TabIndex = tabIndex++,
			};

			//	Crée le panneau de gauche.
			new StaticText
			{
				Parent = frame,
				Dock = DockStyle.Top,
				Text = "Liste des plans comptables :",
				Margins = new Margins (0, Library.UI.Constants.RightMargin, 0, Library.UI.Constants.MarginUnderLabel),
			};

			{
				//	Crée la toolbar.
				double buttonSize = Library.UI.Constants.TinyButtonSize;

				var toolbar = UIBuilder.CreateMiniToolbar (frame, buttonSize);
				toolbar.Dock = DockStyle.Top;
				toolbar.Margins = new Margins (0, 0, 0, -1);
				toolbar.TabIndex = tabIndex++;

				this.addButton = new GlyphButton
				{
					Parent = toolbar,
					PreferredSize = new Size (buttonSize*2+1, buttonSize),
					GlyphShape = GlyphShape.Plus,
					Margins = new Margins (0, 0, 0, 0),
					Dock = DockStyle.Left,
					TabIndex = tabIndex++,
				};

				this.removeButton = new GlyphButton
				{
					Parent = toolbar,
					PreferredSize = new Size (buttonSize, buttonSize),
					GlyphShape = GlyphShape.Minus,
					Margins = new Margins (1, 0, 0, 0),
					Dock = DockStyle.Left,
					TabIndex = tabIndex++,
				};

				ToolTip.Default.SetToolTip (this.addButton,    "Ajoute un plan comptable");
				ToolTip.Default.SetToolTip (this.removeButton, "Supprime le plan comptable sélectionné");
			}

			{
				//	Crée la liste.
				var tile = new FrameBox
				{
					Parent = frame,
					Dock = DockStyle.Fill,
					DrawFullFrame = true,
					TabIndex = tabIndex++,
				};

				this.table = new CellTable
				{
					Parent = tile,
					StyleH = CellArrayStyles.ScrollNorm | CellArrayStyles.Separator | CellArrayStyles.Header,
					StyleV = CellArrayStyles.ScrollNorm | CellArrayStyles.Separator | CellArrayStyles.SelectLine,
					Margins = new Margins (2),
					Dock = DockStyle.Fill,
					TabIndex = tabIndex++,
				};
			}

			new StaticText
			{
				Parent = frame,
				PreferredHeight = 30,
				Dock = DockStyle.Bottom,
				Text = "<i><b>Remarque:</b> Il ne peut pas y avoir plus d'un plan comptable par période.</i>",
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
			int rows = this.ChartOfAccounts.Count;
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
			var chartOfAccount = this.ChartOfAccounts[row];

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
			string[] filenames = this.OpenFileDialog ();  // choix des fichiers à ouvrir...

			if (filenames != null)
			{
				if (this.businessContext.AcquireLock ())
				{
					foreach (var filename in filenames)
					{
						CresusChartOfAccounts chart = CresusChartOfAccountsConnector.Load (filename);

						string err = this.CheckChart (chart);

						if (string.IsNullOrEmpty (err))  // ok ?
						{
							this.financeSettingsEntity.AddChartOfAccounts (this.businessContext, chart);

							this.UpdateTable (this.ChartOfAccounts.Count-1);
							this.UpdateWidgets ();
						}
						else  // erreur ?
						{
							MessageDialog.CreateOk ("Erreur", DialogIcon.Warning, err).OpenDialog ();
							break;
						}
					}
				}
				else
				{
					//	TODO: lock n'a pas pu être obtenu...
				}
			}
		}

		private void RemoveAction()
		{
			//	Bouton [-] cliqué.
			int sel = this.table.SelectedRow;

			if (this.businessContext.AcquireLock ())
			{
				if (sel >= 0 && sel < this.ChartOfAccounts.Count)
				{
					var chart = this.ChartOfAccounts[sel];
					this.financeSettingsEntity.RemoveChartOfAccounts (this.businessContext, chart);

					this.UpdateTable ();
					this.UpdateWidgets ();
				}
			}
			else
			{
				//	TODO: lock n'a pas pu être obtenu...
			}
		}


		private string[] OpenFileDialog()
		{
			//	Demande quels plans comptables ouvrir.
			var dialog = new FileOpenDialog ();

			dialog.Title = "Importation d'un plan comptable \"CRP\"";
			dialog.InitialDirectory = ChartsOfAccountsController.initialDirectory;

			dialog.Filters.Add ("crp", "Plan comptable", "*.crp");
			dialog.Filters.Add ("any", "Tous les fichiers", "*.*");

			dialog.AcceptMultipleSelection = true;
			dialog.OwnerWindow = this.table.Window;
			dialog.OpenDialog ();
			if (dialog.Result != Common.Dialogs.DialogResult.Accept)
			{
				return null;
			}

			ChartsOfAccountsController.initialDirectory = dialog.InitialDirectory;

			return dialog.FileNames;
		}

		private string CheckChart(CresusChartOfAccounts chart)
		{
			//	Vérifie si un plan comptable peut être ouvert.
			//	Retourne null si l'ouverture est possible, ou un message en cas d'erreur.
			foreach (var c in this.ChartOfAccounts)
			{
				if (chart.BeginDate < c.EndDate && chart.EndDate > c.BeginDate)
				{
					return string.Format ("Il y a déjà un plan comptable couvrant la période du {0} au {1}.", chart.BeginDate.ToString (), chart.EndDate.ToString ());
				}
			}

			return null;  // ok
		}

		private IList<CresusChartOfAccounts> ChartOfAccounts
		{
			//	Retourne la liste des plans comptables.
			get
			{
				return this.financeSettingsEntity.GetAllChartsOfAccounts ();
			}
		}


		private static string							initialDirectory;

		private readonly BusinessContext				businessContext;
		private readonly FinanceSettingsEntity			financeSettingsEntity;

		private GlyphButton								addButton;
		private GlyphButton								removeButton;
		private CellTable								table;
	}
}
