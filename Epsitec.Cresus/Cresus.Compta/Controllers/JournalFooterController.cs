﻿//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Common.Types;
using Epsitec.Common.Types.Converters;
using Epsitec.Common.Support;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Library;
using Epsitec.Cresus.Core.Business;

using Epsitec.Cresus.Compta.Accessors;
using Epsitec.Cresus.Compta.Entities;
using Epsitec.Cresus.Compta.Widgets;

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
			this.CreateLineUI ();

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

		private void CreateLineUI()
		{
			this.fieldControllers.Add (new List<AbstractFieldController> ());

			var footerFrame = new FrameBox
			{
				Parent          = this.linesContainer,
				PreferredHeight = 20,
				Dock            = DockStyle.Bottom,
				Margins         = new Margins (0, 0, 1, 0),
			};

			this.linesFrames.Add (footerFrame);
			int line = this.linesFrames.Count - 1;
			int tabIndex = 0;

			var comptes = this.comptaEntity.PlanComptable.Where (x => x.Type == TypeDeCompte.Normal);

			foreach (var mapper in this.columnMappers.Where (x => x.Show))
			{
				AbstractFieldController field;

				if (mapper.Column == ColumnType.Débit)
				{
					field = new AutoCompleteFieldController (this.controller, line, mapper, this.HandleSetFocus, this.FooterTextChanged);
					field.CreateUI (footerFrame);

					UIBuilder.UpdateAutoCompleteTextField (field.EditWidget as AutoCompleteTextField, comptes);
				}
				else if (mapper.Column == ColumnType.Crédit)
				{
					field = new AutoCompleteFieldController (this.controller, line, mapper, this.HandleSetFocus, this.FooterTextChanged);
					field.CreateUI (footerFrame);

					UIBuilder.UpdateAutoCompleteTextField (field.EditWidget as AutoCompleteTextField, comptes);
				}
				else
				{
					field = new TextFieldController (this.controller, line, mapper, this.HandleSetFocus, this.FooterTextChanged);
					field.CreateUI (footerFrame);
				}

				field.Box.TabIndex = ++tabIndex;

				this.fieldControllers[line].Add (field);

#if false
				var box = new FrameBox
				{
					Parent        = footerFrame,
					DrawFullFrame = true,
					Dock          = DockStyle.Left,
					Margins       = new Margins (0, 1, 0, 0),
					TabIndex      = ++tabIndex,
				};

				FrameBox container;
				AbstractTextField field;

				if (mapper.Column == ColumnType.Débit)
				{
					UIBuilder.CreateAutoCompleteTextField (box, out container, out field);
					UIBuilder.UpdateAutoCompleteTextField (field, comptes);
					field.Name = this.GetWidgetName (mapper.Column, line);

					field.TextChanged += delegate
					{
						this.FooterTextChanged (field);
					};
				}
				else if (mapper.Column == ColumnType.Crédit)
				{
					UIBuilder.CreateAutoCompleteTextField (box, out container, out field);
					UIBuilder.UpdateAutoCompleteTextField (field, comptes);
					field.Name = this.GetWidgetName (mapper.Column, line);

					field.TextChanged += delegate
					{
						this.FooterTextChanged (field);
					};
				}
				else
				{
					container = new FrameBox
					{
						Parent   = box,
						Dock     = DockStyle.Fill,
						TabIndex = 1,
					};

					field = new TextField
					{
						Parent   = container,
						Dock     = DockStyle.Fill,
						Name     = this.GetWidgetName (mapper.Column, line),
						TabIndex = 1,
					};

					if (mapper.Column == ColumnType.Date)
					{
						field.TextDisplayMode = TextFieldDisplayMode.ActiveHint;
					}

					field.TextChanged += delegate
					{
						this.FooterTextChanged (field);
					};
				}

				this.footerBoxes     [line].Add (box);
				this.footerContainers[line].Add (container);
				this.footerFields    [line].Add (field);
#endif
			}
		}

		private void ResetLineUI()
		{
			//	Recrée une seule ligne.
			this.linesContainer.Children.Clear ();
			this.fieldControllers.Clear ();
			this.linesFrames.Clear ();

			//?this.selectedLine = 0;

			this.CreateLineUI ();
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


		public override void InsertLineAction()
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

		public override void DeleteLineAction()
		{
			//	Supprime la ligne courante.
			this.dataAccessor.RemoveAtEditionLine (this.selectedLine);

			this.dirty = true;
			this.UpdateFooterContent ();
			this.FooterSelect (this.selectedColumn);
		}

		public override void LineUpAction()
		{
			//	Monte la ligne courante.
			this.SwapLine (this.selectedLine, this.selectedLine-1);
			this.selectedLine--;

			this.dirty = true;
			this.UpdateFooterContent ();
			this.FooterSelect (this.selectedColumn);
		}

		public override void LineDownAction()
		{
			//	Descend la ligne courante.
			this.SwapLine (this.selectedLine, this.selectedLine+1);
			this.selectedLine++;

			this.dirty = true;
			this.UpdateFooterContent ();
			this.FooterSelect (this.selectedColumn);
		}

		public override void LineSwapAction()
		{
			//	Permute le débit et le crédit dans la ligne courante.
			var débit  = this.dataAccessor.EditionLine[this.selectedLine].GetText (ColumnType.Débit);
			var crédit = this.dataAccessor.EditionLine[this.selectedLine].GetText (ColumnType.Crédit);

			this.dataAccessor.EditionLine[this.selectedLine].SetText (ColumnType.Débit,  crédit);
			this.dataAccessor.EditionLine[this.selectedLine].SetText (ColumnType.Crédit, débit);

			this.dirty = true;
			this.UpdateFooterContent ();
		}

		public override void LineAutoAction()
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
					this.CreateLineUI ();
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
