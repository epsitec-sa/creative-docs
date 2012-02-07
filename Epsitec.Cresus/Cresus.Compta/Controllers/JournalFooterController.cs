//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Common.Types;
using Epsitec.Common.Types.Converters;
using Epsitec.Common.Support;

using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Library;
using Epsitec.Cresus.Core.Business;

using Epsitec.Cresus.Compta.Accessors;
using Epsitec.Cresus.Compta.Entities;
using Epsitec.Cresus.Compta.Widgets;
using Epsitec.Cresus.Compta.Helpers;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.Controllers
{
	/// <summary>
	/// Ce contrôleur gère le pied de page pour l'édition de la comptabilité.
	/// </summary>
	public class JournalFooterController : AbstractFooterController
	{
		public JournalFooterController(AbstractController controller)
			: base (controller)
		{
		}


		public override void CreateUI(FrameBox parent, System.Action updateArrayContentAction)
		{
			this.fieldControllers.Clear ();

			//	Crée les boîtes.
			this.infoFrameBox = new FrameBox
			{
				Parent          = parent,
				PreferredHeight = 39,
				Dock            = DockStyle.Bottom,
				Margins         = new Margins (0, 0, 0, 0),
			};

			this.infoFrameSeparator = new FrameBox
			{
				Parent          = parent,
				PreferredHeight = 5,
				Dock            = DockStyle.Bottom,
				Margins         = new Margins (0, 0, 0, 0),
			};

			this.linesContainer = new FrameBox
			{
				Parent  = parent,
				Dock    = DockStyle.Bottom,
				Margins = new Margins (0, 0, 0, 0),
			};

			//	Crée les lignes éditables.
			this.CreateLineUI (this.linesContainer);

			//	Crée les informations.
			this.débitInfoSeparator = new Separator
			{
				Parent          = this.infoFrameSeparator,
				IsVerticalLine  = true,
				DrawFrameWidth  = 3,
				PreferredHeight = 5,
				Dock            = DockStyle.Left,
			};

			this.créditInfoSeparator = new Separator
			{
				Parent          = this.infoFrameSeparator,
				IsVerticalLine  = true,
				DrawFrameWidth  = 3,
				PreferredHeight = 5,
				Dock            = DockStyle.Left,
			};

			this.débitInfoFrame = new FrameBox
			{
				Parent        = this.infoFrameBox,
				DrawFullFrame = true,
				BackColor     = Color.FromHexa ("ffffe1"),  // jaune pâle
				Dock          = DockStyle.Left,
				Margins       = new Margins (0, 1, 0, 0),
			};

			this.créditInfoFrame = new FrameBox
			{
				Parent        = this.infoFrameBox,
				DrawFullFrame = true,
				BackColor     = Color.FromHexa ("ffffe1"),  // jaune pâle
				Dock          = DockStyle.Left,
				Margins       = new Margins (0, 1, 0, 0),
			};

			UIBuilder.CreateInfoCompte (this.débitInfoFrame);
			UIBuilder.CreateInfoCompte (this.créditInfoFrame);

			base.CreateUI (parent, updateArrayContentAction);
		}

		private void CreateLineUI(Widget parent)
		{
			this.fieldControllers.Add (new List<AbstractFieldController> ());

			var footerFrame = new FrameBox
			{
				Parent          = parent,
				PreferredHeight = 20,
				Dock            = DockStyle.Top,
				Margins         = new Margins (0, 0, 1, 0),
			};

			this.linesFrames.Add (footerFrame);
			int line = this.linesFrames.Count - 1;
			int tabIndex = 0;

			footerFrame.TabIndex = line+1;

			var comptes = this.comptaEntity.PlanComptable.Where (x => x.Type == TypeDeCompte.Normal);

			foreach (var mapper in this.columnMappers.Where (x => x.Show))
			{
				AbstractFieldController field;

				if (mapper.Column == ColumnType.Date)
				{
					field = new DateFieldController (this.controller, line, mapper, this.HandleSetFocus, this.FooterTextChanged);
					field.CreateUI (footerFrame);
				}
				else if (mapper.Column == ColumnType.Débit || mapper.Column == ColumnType.Crédit)
				{
					field = new AutoCompleteFieldController (this.controller, line, mapper, this.HandleSetFocus, this.FooterTextChanged);
					field.CreateUI (footerFrame);

					UIBuilder.UpdateAutoCompleteTextField (field.EditWidget as AutoCompleteTextField, comptes);
				}
				else if (mapper.Column == ColumnType.Libellé)
				{
					field = new AutoCompleteFieldController (this.controller, line, mapper, this.HandleSetFocus, this.FooterTextChanged);
					field.CreateUI (footerFrame);

					UIBuilder.UpdateAutoCompleteTextField (field.EditWidget as AutoCompleteTextField, this.comptaEntity.GetLibellésDescriptions (this.périodeEntity).ToArray ());

					this.CreateButtonMedèleUI (field, line);
				}
				else
				{
					field = new TextFieldController (this.controller, line, mapper, this.HandleSetFocus, this.FooterTextChanged);
					field.CreateUI (footerFrame);
				}

				if (mapper.Column == ColumnType.Montant)
				{
					field.EditWidget.ContentAlignment = ContentAlignment.MiddleRight;
				}

				field.Box.TabIndex = ++tabIndex;

				this.fieldControllers[line].Add (field);
			}
		}

		private void ResetLineUI()
		{
			//	Recrée une seule ligne.
			this.linesContainer.Children.Clear ();
			this.fieldControllers.Clear ();
			this.linesFrames.Clear ();

			//?this.selectedLine = 0;

			this.CreateLineUI (this.linesContainer);
			//?this.FooterSelect (0);
		}

		protected override FormattedText GetOperationDescription(bool modify)
		{
			return modify ? "Modification d'une écriture :" : "Création d'une écriture :";
		}


		public override void AcceptAction()
		{
			if (!this.controller.GetCommandEnable (Res.Commands.Edit.Accept))
			{
				return;
			}

			if (!this.isMulti && !this.dataAccessor.IsModification && this.dataAccessor.CountEditedRow == 1)
			{
				if (this.IsDébitMulti (0) || this.IsCréditMulti (0))
				{
					this.isMulti = true;
					this.PrepareFirstMulti ();
					return;
				}
			}

			base.AcceptAction();

			this.UpdateLibellés ();
		}

		private void UpdateLibellés()
		{
			//	Met à jour les libellés usuels dans les widgets combo pour les libellés.
			var libellés = this.comptaEntity.GetLibellésDescriptions (this.périodeEntity).ToArray ();

			for (int line = 0; line < this.fieldControllers.Count; line++)
			{
				var field = this.GetFieldController (ColumnType.Libellé, line);
				UIBuilder.UpdateAutoCompleteTextField (field.EditWidget as AutoCompleteTextField, libellés);
			}
		}

		private void PrepareFirstMulti()
		{
			//	Met à jour l'interface pour permettre de créer une écriture multiple, lorsqu'on passe de 1 ligne à 3.
			bool isDébitMulti = this.IsDébitMulti (0);
			var multiActiveColumn   =  isDébitMulti ? ColumnType.Crédit : ColumnType.Débit;
			var multiInactiveColumn = !isDébitMulti ? ColumnType.Crédit : ColumnType.Débit;

			this.dataAccessor.InsertEditionLine (-1);  // 2ème ligne
			this.dataAccessor.InsertEditionLine (-1);  // contrepartie

			//	Met à jour les données de la 2ème ligne.
			this.dataAccessor.EditionLine[1].SetText (ColumnType.Date,             this.dataAccessor.EditionLine[0].GetText (ColumnType.Date));
			this.dataAccessor.EditionLine[1].SetText (multiInactiveColumn,         JournalDataAccessor.multi);
			this.dataAccessor.EditionLine[1].SetText (ColumnType.Pièce,            this.dataAccessor.EditionLine[0].GetText (ColumnType.Pièce));
			this.dataAccessor.EditionLine[1].SetText (ColumnType.Montant,          "0.00");
			this.dataAccessor.EditionLine[1].SetText (ColumnType.Journal,          this.dataAccessor.EditionLine[0].GetText (ColumnType.Journal));
																				   
			//	Met à jour les données de la contrepartie.						   
			this.dataAccessor.EditionLine[2].SetText (ColumnType.Date,             this.dataAccessor.EditionLine[0].GetText (ColumnType.Date));
			this.dataAccessor.EditionLine[2].SetText (multiActiveColumn,           JournalDataAccessor.multi);
			this.dataAccessor.EditionLine[2].SetText (ColumnType.Pièce,            this.dataAccessor.EditionLine[0].GetText (ColumnType.Pièce));
			this.dataAccessor.EditionLine[2].SetText (ColumnType.Libellé,          "Total");
			this.dataAccessor.EditionLine[2].SetText (ColumnType.Journal,          this.dataAccessor.EditionLine[0].GetText (ColumnType.Journal));
			this.dataAccessor.EditionLine[2].SetText (ColumnType.TotalAutomatique, "True");

			this.UpdateFooterContent ();

			this.selectedLine = 1;  // dans la 2ème ligne
			this.FooterSelect (multiActiveColumn);
		}


		public override void MultiInsertLineAction()
		{
			//	Insère une nouvelle ligne après la ligne courante.
			bool isDébitMulti = this.IsDébitMulti (this.selectedLine);
			var multiActiveColumn   =  isDébitMulti ? ColumnType.Crédit : ColumnType.Débit;
			var multiInactiveColumn = !isDébitMulti ? ColumnType.Crédit : ColumnType.Débit;

			this.selectedLine++;
			this.dataAccessor.InsertEditionLine (this.selectedLine);

			this.dataAccessor.EditionLine[this.selectedLine].SetText (multiInactiveColumn, JournalDataAccessor.multi);

			int cp = this.IndexTotalAutomatique;
			if (cp != -1)
			{
				this.dataAccessor.EditionLine[this.selectedLine].SetText (ColumnType.Date,    this.dataAccessor.EditionLine[cp].GetText (ColumnType.Date));
				this.dataAccessor.EditionLine[this.selectedLine].SetText (ColumnType.Pièce,   this.dataAccessor.EditionLine[cp].GetText (ColumnType.Pièce));
				this.dataAccessor.EditionLine[this.selectedLine].SetText (ColumnType.Journal, this.dataAccessor.EditionLine[cp].GetText (ColumnType.Journal));
			}

			this.dirty = true;
			this.UpdateFooterContent ();
			this.FooterSelect (multiActiveColumn);
		}

		public override void MultiDeleteLineAction()
		{
			//	Supprime la ligne courante.
			this.dataAccessor.RemoveAtEditionLine (this.selectedLine);

			this.dirty = true;
			this.UpdateFooterContent ();
			this.FooterSelect (this.selectedColumn);
		}

		public override void MultiMoveLineAction(int direction)
		{
			//	Monte ou descend la ligne courante.
			this.SwapLine (this.selectedLine, this.selectedLine+direction);
			this.selectedLine += direction;

			this.dirty = true;
			this.UpdateFooterContent ();
			this.FooterSelect (this.selectedColumn);
		}

		public override void MultiLineSwapAction()
		{
			//	Permute le débit et le crédit dans la ligne courante.
			var débit  = this.dataAccessor.EditionLine[this.selectedLine].GetText (ColumnType.Débit);
			var crédit = this.dataAccessor.EditionLine[this.selectedLine].GetText (ColumnType.Crédit);

			this.dataAccessor.EditionLine[this.selectedLine].SetText (ColumnType.Débit,  crédit);
			this.dataAccessor.EditionLine[this.selectedLine].SetText (ColumnType.Crédit, débit);

			this.dirty = true;
			this.UpdateFooterContent ();
		}

		public override void MultiLineAutoAction()
		{
			//	Met le total automatique dans la ligne courante.
			for (int line = 0; line < this.dataAccessor.EditionLine.Count; line++)
			{
				this.dataAccessor.EditionLine[line].SetText (ColumnType.TotalAutomatique, (line == this.selectedLine) ? "True" : "False");
			}

			this.dirty = true;
			this.UpdateFooterContent ();
		}

		private void SwapLine(int line1, int line2)
		{
			//	Permute deux lignes (pour les opérations monte/descend).
			var t                                = this.dataAccessor.EditionLine[line2];
			this.dataAccessor.EditionLine[line2] = this.dataAccessor.EditionLine[line1];
			this.dataAccessor.EditionLine[line1] = t;
		}

		public override void InsertModèle(int n)
		{
			RaccourciModèle rm = RaccourciModèle.Ctrl0 + n;
			string srm = Converters.RaccourciToString (rm);
			var modèle = this.comptaEntity.Modèles.Where (x => x.Raccourci == srm).FirstOrDefault ();

			if (modèle != null)
			{
				this.InsertModèle (modèle, this.selectedLine);
			}
		}


		protected override void UpdateEditionWidgets()
		{
			//	Met à jour toutes les données en édition d'une écriture multiple.
			if (!this.isMulti || this.controller.IgnoreChanged)
			{
				return;
			}

			for (int line = 0; line < this.dataAccessor.EditionLine.Count; line++)
			{
				if (this.IsDébitMulti (line))
				{
					this.SetWidgetVisibility (ColumnType.Débit,  line, false);
					this.SetWidgetVisibility (ColumnType.Crédit, line, true);
				}

				if (this.IsCréditMulti (line))
				{
					this.SetWidgetVisibility (ColumnType.Débit,  line, true);
					this.SetWidgetVisibility (ColumnType.Crédit, line, false);
				}

				bool totalAutomatique = (this.dataAccessor.EditionLine[line].GetText (ColumnType.TotalAutomatique) == "True");

				this.SetWidgetVisibility (ColumnType.Date,    line, totalAutomatique);
				this.SetWidgetVisibility (ColumnType.Pièce,   line, totalAutomatique);
				this.SetWidgetVisibility (ColumnType.Journal, line, totalAutomatique);

				this.SetTextFieldReadonly (ColumnType.Montant, line, totalAutomatique);
			}

			this.UpdateMultiEditionData ();
		}

		private void UpdateMultiEditionData()
		{
			//	Recalcule le total de l'écriture multiple.
			int cp = this.IndexTotalAutomatique;
			if (cp != -1)
			{
				decimal totalDébit  = 0;
				decimal totalCrédit = 0;

				for (int line = 0; line < this.dataAccessor.EditionLine.Count; line++)
				{
					if (line != cp)
					{
						this.dataAccessor.EditionLine[line].SetText (ColumnType.Date,    this.dataAccessor.EditionLine[cp].GetText (ColumnType.Date));
						this.dataAccessor.EditionLine[line].SetText (ColumnType.Pièce,   this.dataAccessor.EditionLine[cp].GetText (ColumnType.Pièce));
						this.dataAccessor.EditionLine[line].SetText (ColumnType.Journal, this.dataAccessor.EditionLine[cp].GetText (ColumnType.Journal));

						var text = this.dataAccessor.EditionLine[line].GetText (ColumnType.Montant).ToSimpleText ();
						decimal montant = 0;
						decimal.TryParse (text, out montant);

						if (this.IsDébitMulti (line))
						{
							totalCrédit += montant;
						}

						if (this.IsCréditMulti (line))
						{
							totalDébit += montant;
						}
					}
				}

				if (this.IsDébitMulti (cp))
				{
					decimal montant = totalDébit - totalCrédit;
					this.dataAccessor.EditionLine[cp].SetText (ColumnType.Montant, montant.ToString ("0.00"));
				}

				if (this.IsCréditMulti (cp))
				{
					decimal montant = totalCrédit - totalDébit;
					this.dataAccessor.EditionLine[cp].SetText (ColumnType.Montant, montant.ToString ("0.00"));
				}
			}
		}

		private bool IsDébitMulti(int line)
		{
			return this.dataAccessor.EditionLine[line].GetText (ColumnType.Débit) == JournalDataAccessor.multi;
		}

		private bool IsCréditMulti(int line)
		{
			return this.dataAccessor.EditionLine[line].GetText (ColumnType.Crédit) == JournalDataAccessor.multi;
		}

		private int IndexTotalAutomatique
		{
			//	Retourne l'index de la ligne qui contient le total automatique.
			get
			{
				return this.dataAccessor.EditionLine.FindIndex (x => x.GetText (ColumnType.TotalAutomatique) == "True");
			}
		}


		protected override void UpdateAfterSelectedLineChanged()
		{
#if false
			int sel = this.multiFirstRow + this.selectedLine;

			if (this.arrayController.SelectedRow != sel)
			{
				this.controller.IgnoreChanged = true;
				this.arrayController.SelectedRow = sel;
				this.controller.IgnoreChanged = false;
			}
#endif

			this.UpdateToolbar ();
			this.UpdateInsertionRow ();
			this.UpdateFooterInfo ();
		}


		public override void UpdateFooterContent()
		{
			this.UpdateArrayColumns ();

			int count = this.dataAccessor.CountEditedRow;
			this.selectedLine = System.Math.Min (this.selectedLine, count-1);
			this.isMulti = (count > 1);

			base.UpdateFooterContent ();
		}


		public override void UpdateFooterGeometry()
		{
			base.UpdateFooterGeometry ();

			double w1 = this.arrayController.GetColumnsAbsoluteWidth (0);
			double w2 = this.arrayController.GetColumnsAbsoluteWidth (1);
			double w3 = this.arrayController.GetColumnsAbsoluteWidth (2);

			this.débitInfoSeparator.Margins  = new Margins (w1, 0, 0, 0);
			this.débitInfoSeparator.PreferredWidth  = w2-1;
			this.créditInfoSeparator.PreferredWidth = w3-1;

			this.débitInfoFrame.PreferredWidth  = w1+w2;
			this.créditInfoFrame.PreferredWidth = w1+w2-1;
		}

		protected override void UpdateArrayColumns()
		{
			//	Si nécessaire, adapte l'interface pour accueillir le nombre de lignes et de colonnes requis.
			int count = this.dataAccessor.CountEditedRow;

			if (this.linesFrames.Count != count ||
				this.fieldControllers[0].Count != this.columnMappers.Where (x => x.Show).Count ())
			{
				this.ResetLineUI ();

				for (int i = 0; i < count-1; i++)
				{
					this.CreateLineUI (this.linesContainer);
				}
			}
		}


		protected override void UpdateFooterInfo()
		{
			this.infoFrameSeparator.Visibility = this.ShowInfoPanel;
			this.infoFrameBox.Visibility       = this.ShowInfoPanel;

			if (this.ShowInfoPanel)
			{
				this.UpdateFooterInfo (this.dataAccessor.GetEditionText (this.selectedLine, ColumnType.Débit ), isDébit: true);
				this.UpdateFooterInfo (this.dataAccessor.GetEditionText (this.selectedLine, ColumnType.Crédit), isDébit: false);
			}
		}

		private void UpdateFooterInfo(FormattedText numéro, bool isDébit)
		{
			FormattedText title;
			decimal? solde;

			this.GetInfoCompte (numéro, out title, out solde);
			UIBuilder.UpdateInfoCompte (isDébit ? this.débitInfoFrame : this.créditInfoFrame, title, solde);
		}

		public override void UpdateToolbar()
		{
			base.UpdateToolbar ();

			int count = this.linesFrames.Count;
			int cp = this.IndexTotalAutomatique;

			this.controller.SetCommandEnable (Res.Commands.Multi.Insert, count > 1);
			this.controller.SetCommandEnable (Res.Commands.Multi.Delete, count > 2 && this.selectedLine != cp);
			this.controller.SetCommandEnable (Res.Commands.Multi.Up,     count > 1 && this.selectedLine > 0);
			this.controller.SetCommandEnable (Res.Commands.Multi.Down,   count > 1 && this.selectedLine < count-1);
			this.controller.SetCommandEnable (Res.Commands.Multi.Swap,   count != 0);
			this.controller.SetCommandEnable (Res.Commands.Multi.Auto,   count > 1 && this.selectedLine != cp);
		}


		private void GetInfoCompte(FormattedText numéro, out FormattedText titre, out decimal? solde)
		{
			numéro = PlanComptableDataAccessor.GetCompteNuméro (numéro);
			var compte = this.comptaEntity.PlanComptable.Where (x => x.Numéro == numéro).FirstOrDefault ();

			if (compte == null)
			{
				titre = FormattedText.Empty;
				solde = null;
			}
			else
			{
				titre = compte.Titre;
				solde = this.dataAccessor.SoldesJournalManager.GetSolde (compte);
			}
		}


		#region Menu des écritures modèles
		private void CreateButtonMedèleUI(AbstractFieldController fieldController, int line)
		{
			if (this.comptaEntity.Modèles.Any ())
			{
				var button = new Button
				{
					Parent          = fieldController.Box,
					ButtonStyle     = ButtonStyle.Icon,
					Text            = "M",
					Index           = line,
					PreferredWidth  = UIBuilder.ComboButtonWidth,
					PreferredHeight = 20,
					Dock            = DockStyle.Right,
					Margins         = new Margins (-1, 0, 0, 0),
				};

				button.Clicked += delegate
				{
					this.ShowMenuModèles (button, button.Index);
				};

				ToolTip.Default.SetToolTip (button, "Choix d'une écriture modèle");
			}
		}

		private void ShowMenuModèles(Widget parentButton, int line)
		{
			//	Affiche le menu permettant de choisir le mode.
			var menu = new VMenu ();

			int index = 0;
			foreach (var modèle in this.comptaEntity.Modèles)
			{
				var item = new MenuItem ()
				{
					FormattedText = modèle.ShortSummary,
					Index         = index++,
				};

				item.Clicked += delegate
				{
					this.InsertModèle (this.comptaEntity.Modèles[item.Index], line);
				};

				menu.Items.Add (item);
			}

			TextFieldCombo.AdjustComboSize (parentButton, menu, false);

			menu.Host = parentButton.Window;
			menu.ShowAsComboList (parentButton, Point.Zero, parentButton);
		}

		private void InsertModèle(ComptaModèleEntity modèle, int line)
		{
			if (modèle.Débit != null)
			{
				this.dataAccessor.EditionLine[line].SetText (ColumnType.Débit, modèle.Débit.Numéro);
			}

			if (modèle.Crédit != null)
			{
				this.dataAccessor.EditionLine[line].SetText (ColumnType.Crédit, modèle.Crédit.Numéro);
			}

			if (!modèle.Pièce.IsNullOrEmpty)
			{
				this.dataAccessor.EditionLine[line].SetText (ColumnType.Pièce, modèle.Pièce);
			}

			int cursor = -1;
			if (!modèle.Libellé.IsNullOrEmpty)
			{
				cursor = modèle.Libellé.ToString ().IndexOf ("@");
				var m = modèle.Libellé.ToString ().Replace ("@", "");
				this.dataAccessor.EditionLine[line].SetText (ColumnType.Libellé, m);
			}

			if (modèle.Montant.HasValue)
			{
				this.dataAccessor.EditionLine[line].SetText (ColumnType.Montant, modèle.Montant.Value.ToString ("0.00"));
			}

			this.UpdateFooterContent ();
			this.FooterSelect (ColumnType.Libellé);

			var fc = this.GetFieldController (ColumnType.Libellé, line);
			var field = fc.EditWidget as AbstractTextField;
			field.Focus ();
			field.Cursor = (cursor == -1) ? field.Text.Length : cursor;
		}
		#endregion


		private FrameBox							linesContainer;
		private FrameBox							infoFrameSeparator;
		private FrameBox							infoFrameBox;
		private Separator							débitInfoSeparator;
		private Separator							créditInfoSeparator;
		private FrameBox							débitInfoFrame;
		private FrameBox							créditInfoFrame;

		private bool								isMulti;
	}
}
