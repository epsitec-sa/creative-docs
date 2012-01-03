//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Common.Types;
using Epsitec.Common.Types.Converters;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Widgets;
using Epsitec.Cresus.Core.Widgets.Tiles;
using Epsitec.Cresus.Core.Library;
using Epsitec.Cresus.Core.Business.Finance;
using Epsitec.Cresus.Core.Business.Finance.Comptabilité;

using Epsitec.Cresus.DataLayer.Context;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers.ComptabilitéControllers
{
	/// <summary>
	/// Ce contrôleur gère le pied de page pour l'édition de la comptabilité.
	/// </summary>
	public class JournalFooterController : AbstractFooterController<JournalColumn, ComptabilitéEcritureEntity>
	{
		public JournalFooterController(TileContainer tileContainer, ComptabilitéEntity comptabilitéEntity, AbstractDataAccessor<JournalColumn, ComptabilitéEcritureEntity> dataAccessor, List<AbstractColumnMapper<JournalColumn>> columnMappers, ArrayController<ComptabilitéEcritureEntity> arrayController)
			: base (tileContainer, comptabilitéEntity, dataAccessor, columnMappers, arrayController)
		{
			this.infoShowed = true;
		}


		public override void CreateUI(FrameBox parent, System.Action updateArrayContentAction)
		{
			this.footerContainers.Clear ();
			this.footerFields.Clear ();
			this.footerValidatedTexts.Clear ();

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

			var footerFrame = new FrameBox
			{
				Parent  = parent,
				Dock    = DockStyle.Bottom,
				Margins = new Margins (0, 0, 0, 0),
			};

			//	Crée les lignes éditables.
			var comptes = this.comptabilitéEntity.PlanComptable.Where (x => x.Type == TypeDeCompte.Normal).OrderBy (x => x.Numéro);

			int columnCount = this.columnMappers.Count;

			for (int column = 0; column < columnCount; column++)
			{
				var mapper = this.columnMappers[column];

				Widget container;
				AbstractTextField field;

				if (mapper.Column == JournalColumn.Débit)
				{
					var marshaler = Marshaler.Create<FormattedText> (() => this.CompteDébit, x => this.CompteDébit = x);
					UIBuilder.CreateAutoCompleteTextField (footerFrame, comptes, marshaler, out container, out field);
					container.TabIndex = column+1;
					field.Index = column;

					field.TextChanged += delegate
					{
						this.FooterTextChanged (field.Index, field.FormattedText);
					};
				}
				else if (mapper.Column == JournalColumn.Crédit)
				{
					var marshaler = Marshaler.Create<FormattedText> (() => this.CompteCrédit, x => this.CompteCrédit = x);
					UIBuilder.CreateAutoCompleteTextField (footerFrame, comptes, marshaler, out container, out field);
					container.TabIndex = column+1;
					field.Index = column;

					field.TextChanged += delegate
					{
						this.FooterTextChanged (field.Index, field.FormattedText);
					};
				}
				else
				{
					field = new TextField
					{
						Parent   = footerFrame,
						Dock     = DockStyle.Left,
						Margins  = new Margins (0, 1, 0, 0),
						Index    = column,
						TabIndex = column+1,
					};

					field.TextChanged += delegate
					{
						this.FooterTextChanged (field.Index, field.FormattedText);
					};

					container = field;
				}

				this.footerContainers.Add (container);
				this.footerFields.Add (field);
				this.footerValidatedTexts.Add (FormattedText.Empty);
			}

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
				BackColor     = Color.FromBrightness (0.90),
				Dock          = DockStyle.Left,
				Margins       = new Margins (0, 1, 0, 0),
			};

			this.créditInfoFrame = new FrameBox
			{
				Parent        = this.infoFrameBox,
				DrawFullFrame = true,
				BackColor     = Color.FromBrightness (0.90),
				Dock          = DockStyle.Left,
				Margins       = new Margins (0, 1, 0, 0),
			};

			UIBuilder.CreateInfoCompte (this.débitInfoFrame);
			UIBuilder.CreateInfoCompte (this.créditInfoFrame);

			//	Crée le bouton pour montrer/cacher.
			this.infoShowHideButton = new GlyphButton
			{
				Parent        = parent,
				Anchor        = AnchorStyles.BottomRight,
				PreferredSize = new Size (16, 16),
				ButtonStyle   = ButtonStyle.Slider,
			};

			this.infoShowHideButton.Clicked += delegate
			{
				this.infoShowed = !this.infoShowed;
				this.UpdateInfoShowHideButton ();
			};

			this.UpdateInfoShowHideButton ();

			base.CreateUI (parent, updateArrayContentAction);
		}

		public override void FinalUpdate()
		{
			this.UpdateInfoShowHideButton ();
		}

		protected override FormattedText GetOperationDescription(bool modify)
		{
			return modify ? "Modification d'une écriture :" : "Création d'une écriture :";
		}


		public override void AcceptAction()
		{
			if (this.bottomToolbarController.AcceptEnable)
			{
				this.AcceptAction (false, this.arrayController.SelectedEntity);

				if (this.JustCreate)
				{
					int column = this.GetMapperColumnRank (JournalColumn.Montant);
					this.arrayController.IgnoreChanged = true;
					this.GetTextField (column).FormattedText = "0.00";
					this.arrayController.IgnoreChanged = false;
				}
			}
		}

		protected override void CreateEntity(bool silent)
		{
			var écriture = this.tileContainer.Controller.DataContext.CreateEntity<ComptabilitéEcritureEntity> ();
			this.dataAccessor.Add (écriture);
			this.UpdateEntity (silent, écriture);
		}


		protected override void FooterTextChanged(int column, FormattedText text)
		{
			//	Appelé lorsqu'un texte éditable a changé.
			base.FooterTextChanged (column, text);

			var mapper = this.columnMappers[column];

			if (mapper.Column == JournalColumn.Débit)
			{
				this.UpdateFooterInfo (text, true);
			}
			else if (mapper.Column == JournalColumn.Crédit)
			{
				this.UpdateFooterInfo (text, false);
			}
		}

		public override void UpdateFooterContent()
		{
			this.arrayController.IgnoreChanged = true;

			int columnCount = this.columnMappers.Count;
			int row = this.arrayController.SelectedRow;

			if (row == -1 || row >= this.dataAccessor.Count)
			{
				for (int column = 0; column < columnCount; column++)
				{
					var mapper = this.columnMappers[column];

					if (mapper.Column == JournalColumn.Date)
					{
						this.footerFields[column].FormattedText = this.comptabilitéEntity.DefaultDate.ToString ();
					}
					else if (mapper.Column == JournalColumn.Montant)
					{
						this.footerFields[column].FormattedText = "0.00";
					}
					else
					{
						this.footerFields[column].FormattedText = null;
					}
				}
			}
			else
			{
				for (int column = 0; column < columnCount; column++)
				{
					var mapper = this.columnMappers[column];

					this.footerFields[column].FormattedText = this.dataAccessor.GetText (row, mapper.Column);
				}
			}

			this.FooterValidate ();
			this.UpdateFooterInfo ();

			this.arrayController.IgnoreChanged = false;
		}

		public override void UpdateFooterGeometry()
		{
			int columnCount = this.columnMappers.Count;

			for (int column = 0; column < columnCount; column++)
			{
				this.footerContainers[column].PreferredWidth = this.arrayController.GetColumnsAbsoluteWidth (column) - (column == 0 ? 0 : 1);
			}

			double w1 = this.arrayController.GetColumnsAbsoluteWidth (0);
			double w2 = this.arrayController.GetColumnsAbsoluteWidth (1);
			double w3 = this.arrayController.GetColumnsAbsoluteWidth (2);

			this.débitInfoSeparator.Margins  = new Margins (w1, 0, 0, 0);
			this.débitInfoSeparator.PreferredWidth  = w2-1;
			this.créditInfoSeparator.PreferredWidth = w3-1;

			this.débitInfoFrame.PreferredWidth  = w1+w2;
			this.créditInfoFrame.PreferredWidth = w1+w2-1;
		}


		private void UpdateFooterInfo()
		{
			if (!this.infoShowed)
			{
				return;
			}

			{
				int column = this.GetMapperColumnRank (JournalColumn.Débit);
				this.UpdateFooterInfo (this.footerFields[column].FormattedText, true);
			}

			{
				int column = this.GetMapperColumnRank (JournalColumn.Crédit);
				this.UpdateFooterInfo (this.footerFields[column].FormattedText, false);
			}
		}

		private void UpdateFooterInfo(FormattedText numéro, bool isDébit)
		{
			if (!this.infoShowed)
			{
				return;
			}

			FormattedText title;
			decimal? solde;

			this.GetInfoCompte (numéro, out title, out solde);
			UIBuilder.UpdateInfoCompte (isDébit ? this.débitInfoFrame : this.créditInfoFrame, title, solde);
		}

		private void UpdateInfoShowHideButton()
		{
			//	Met à jour le bouton pour montrer/cacher la barre d'icône.
			this.infoShowHideButton.GlyphShape = this.infoShowed ? GlyphShape.ArrowDown : GlyphShape.ArrowUp;
			this.infoShowHideButton.Margins = new Margins (0, 0, 0, this.infoShowed ? 47 : 2);

			ToolTip.Default.SetToolTip (this.infoShowHideButton, this.infoShowed ? "Cache les informations sur les comptes" : "Montre les informations sur les comptes");

			this.infoFrameSeparator.Visibility = this.infoShowed;
			this.infoFrameBox.Visibility       = this.infoShowed;

			if (this.bottomToolbarController != null)
			{
				this.bottomToolbarController.BottomOffset = this.infoShowed ? 44 : 0;
			}

			if (this.infoShowed)
			{
				this.UpdateFooterInfo ();
			}
		}


		private int GetMapperColumnRank(JournalColumn column)
		{
			var mapper = this.columnMappers.Where (x => x.Column == column).FirstOrDefault ();
			return this.columnMappers.IndexOf (mapper);
		}

		private void GetInfoCompte(FormattedText numéro, out FormattedText titre, out decimal? solde)
		{
			numéro = PlanComptableAccessor.GetCompteNuméro (numéro);
			var compte = this.comptabilitéEntity.PlanComptable.Where (x => x.Numéro == numéro).FirstOrDefault ();

			if (compte == null)
			{
				titre = FormattedText.Empty;
				solde = null;
			}
			else
			{
				titre = compte.Titre;
				solde = this.comptabilitéEntity.GetSoldeCompte (compte);
			}
		}

		private FormattedText CompteDébit
		{
			get
			{
				var écriture = this.arrayController.SelectedEntity;

				if (écriture != null && écriture.Débit != null)
				{
					return PlanComptableAccessor.GetCompteDescription (écriture.Débit);
				}

				return FormattedText.Empty;
			}
			set
			{
				var écriture = this.arrayController.SelectedEntity;

				if (écriture != null)
				{
					écriture.Débit = PlanComptableAccessor.GetCompteEntity (this.comptabilitéEntity, value);
				}
			}
		}

		private FormattedText CompteCrédit
		{
			get
			{
				var écriture = this.arrayController.SelectedEntity;

				if (écriture != null && écriture.Crédit != null)
				{
					return PlanComptableAccessor.GetCompteDescription (écriture.Crédit);
				}

				return FormattedText.Empty;
			}
			set
			{
				var écriture = this.arrayController.SelectedEntity;

				if (écriture != null)
				{
					écriture.Crédit = PlanComptableAccessor.GetCompteEntity (this.comptabilitéEntity, value);
				}
			}
		}

	
		private FrameBox							infoFrameSeparator;
		private FrameBox							infoFrameBox;
		private Separator							débitInfoSeparator;
		private Separator							créditInfoSeparator;
		private FrameBox							débitInfoFrame;
		private FrameBox							créditInfoFrame;
		private GlyphButton							infoShowHideButton;
		private bool								infoShowed;
	}
}
