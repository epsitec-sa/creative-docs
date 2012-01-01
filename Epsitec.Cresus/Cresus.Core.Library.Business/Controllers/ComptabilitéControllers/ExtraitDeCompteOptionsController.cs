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
using Epsitec.Cresus.Core.Business.Finance;
using Epsitec.Cresus.Core.Business.Finance.Comptabilité;

using Epsitec.Cresus.DataLayer.Context;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers.ComptabilitéControllers
{
	/// <summary>
	/// Ce contrôleur gère les options d'affichage d'un extrait de compte de la comptabilité.
	/// </summary>
	public class ExtraitDeCompteOptionsController : AbstractOptionsController<ExtraitDeCompteData, ExtraitDeCompteOptions>
	{
		public ExtraitDeCompteOptionsController(TileContainer tileContainer, ComptabilitéEntity comptabilitéEntity, ExtraitDeCompteOptions options)
			: base (tileContainer, comptabilitéEntity, options)
		{
		}


		public override void CreateUI(FrameBox parent, System.Action optionsChanged)
		{
			this.toolbar = new FrameBox
			{
				Parent              = parent,
				DrawFullFrame       = true,
				BackColor           = Color.FromBrightness (0.96),  // gris très clair
				ContainerLayoutMode = Common.Widgets.ContainerLayoutMode.VerticalFlow,
				PreferredHeight     = 56,
				Dock                = DockStyle.Top,
				Margins             = new Margins (0, 20, 0, 6),
				Padding             = new Margins (5),
			};

			this.CreateEditionUI (this.toolbar, optionsChanged);
			this.CreateTitleUI (this.toolbar);

			this.UpdateTitle ();
		}

		private void CreateEditionUI(FrameBox parent, System.Action optionsChanged)
		{
			var frame = new FrameBox
			{
				Parent = parent,
				Dock   = DockStyle.Top,
			};

			new StaticText
			{
				Parent         = frame,
				FormattedText  = "Compte",
				PreferredWidth = 64,
				Dock           = DockStyle.Left,
			};

			Widget container;
			AbstractTextField field;
			var comptes = this.comptabilitéEntity.PlanComptable.Where (x => this.CompteFilter (x)).OrderBy (x => x.Numéro);
			var marshaler = Marshaler.Create<FormattedText> (() => this.NuméroCompte, x => this.NuméroCompte = x);
			UIBuilder.CreateAutoCompleteTextField (frame, comptes, marshaler, out container, out field);
			container.PreferredWidth = 100;
			container.Margins = new Margins (0, 10, 0, 0);

			field.EditionAccepted += delegate
			{
				this.NuméroCompte = field.FormattedText;
				this.UpdateTitle ();
				optionsChanged ();
			};

#if false
			this.radioTous = new RadioButton
			{
				Parent         = frame,
				FormattedText  = "Tous",
				PreferredWidth = 50,
				ActiveState    = Common.Widgets.ActiveState.Yes,
				Dock           = DockStyle.Left,
			};

			this.radioActifs = new RadioButton
			{
				Parent         = frame,
				FormattedText  = "Actifs",
				PreferredWidth = 55,
				ActiveState    = Common.Widgets.ActiveState.No,
				Dock           = DockStyle.Left,
			};

			this.radioPassifs = new RadioButton
			{
				Parent         = frame,
				FormattedText  = "Passifs",
				PreferredWidth = 60,
				ActiveState    = Common.Widgets.ActiveState.No,
				Dock           = DockStyle.Left,
			};

			this.radioCharges = new RadioButton
			{
				Parent         = frame,
				FormattedText  = "Charges",
				PreferredWidth = 70,
				ActiveState    = Common.Widgets.ActiveState.No,
				Dock           = DockStyle.Left,
			};

			this.radioProduits = new RadioButton
			{
				Parent         = frame,
				FormattedText  = "Produits",
				PreferredWidth = 70,
				ActiveState    = Common.Widgets.ActiveState.No,
				Dock           = DockStyle.Left,
			};

			this.radioExploitations = new RadioButton
			{
				Parent         = frame,
				FormattedText  = "Exploitations",
				PreferredWidth = 90,
				ActiveState    = Common.Widgets.ActiveState.No,
				Dock           = DockStyle.Left,
			};

			this.checkGroupe = new CheckButton
			{
				Parent         = frame,
				FormattedText  = "Comptes centralisateurs",
				PreferredWidth = 150,
				ActiveState    = Common.Widgets.ActiveState.No,
				Dock           = DockStyle.Left,
			};
#endif
		}

		private void CreateTitleUI(FrameBox parent)
		{
			var frame = new FrameBox
			{
				Parent = parent,
				Dock   = DockStyle.Fill,
			};

			this.titleLabel = new StaticText
			{
				Parent           = frame,
				ContentAlignment = Common.Drawing.ContentAlignment.BottomLeft,
				Dock             = DockStyle.Fill,
			};
		}

		private void UpdateTitle()
		{
			var compte = this.comptabilitéEntity.PlanComptable.Where (x => x.Numéro == this.options.NuméroCompte).FirstOrDefault ();

			if (compte == null)
			{
				this.titleLabel.FormattedText = null;
			}
			else
			{
				this.titleLabel.FormattedText = TextFormatter.FormatText (compte.Numéro, compte.Titre).ApplyBold ().ApplyFontSize (14.0);
			}
		}


		private bool CompteFilter(ComptabilitéCompteEntity compte)
		{
			if (this.radioActifs == null)
			{
				return true;
			}

			if (compte.Type != TypeDeCompte.Normal &&
				compte.Type != TypeDeCompte.Groupe)
			{
				return false;
			}

			if (this.checkGroupe.ActiveState == ActiveState.No && compte.Type == TypeDeCompte.Groupe)
			{
				return false;
			}

			if (this.radioActifs.ActiveState == ActiveState.Yes)
			{
				return compte.Catégorie == CatégorieDeCompte.Actif;
			}
			else if (this.radioPassifs.ActiveState == ActiveState.Yes)
			{
				return compte.Catégorie == CatégorieDeCompte.Passif;
			}
			else if (this.radioCharges.ActiveState == ActiveState.Yes)
			{
				return compte.Catégorie == CatégorieDeCompte.Charge;
			}
			else if (this.radioProduits.ActiveState == ActiveState.Yes)
			{
				return compte.Catégorie == CatégorieDeCompte.Produit;
			}
			else if (this.radioExploitations.ActiveState == ActiveState.Yes)
			{
				return compte.Catégorie == CatégorieDeCompte.Exploitation;
			}

			return true;
		}

		private FormattedText NuméroCompte
		{
			get
			{
				return this.options.NuméroCompte;
			}
			set
			{
				this.options.NuméroCompte = PlanComptableAccessor.GetCompteNuméro (value);
			}
		}


		private RadioButton		radioTous;
		private RadioButton		radioActifs;
		private RadioButton		radioPassifs;
		private RadioButton		radioCharges;
		private RadioButton		radioProduits;
		private RadioButton		radioExploitations;
		private CheckButton		checkGroupe;
		private StaticText		titleLabel;
	}
}
