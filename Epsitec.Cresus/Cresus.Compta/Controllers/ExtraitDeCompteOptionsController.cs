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

using Epsitec.Cresus.Compta.Entities;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta
{
	/// <summary>
	/// Ce contrôleur gère les options d'affichage d'un extrait de compte de la comptabilité.
	/// </summary>
	public class ExtraitDeCompteOptionsController : AbstractOptionsController
	{
		public ExtraitDeCompteOptionsController(ComptabilitéEntity comptabilitéEntity, ExtraitDeCompteOptions options)
			: base (comptabilitéEntity, options)
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
				PreferredHeight     = 81,
				Dock                = DockStyle.Top,
				Margins             = new Margins (0, 20, 0, 6),
				Padding             = new Margins (5),
			};

			this.CreateEditionUI (this.toolbar, optionsChanged);
			this.CreateDatesUI (this.toolbar, optionsChanged);
			this.CreateTitleUI (this.toolbar);

			this.UpdateTitle ();
		}

		private void CreateEditionUI(FrameBox parent, System.Action optionsChanged)
		{
			var frame = new FrameBox
			{
				Parent   = parent,
				Dock     = DockStyle.Top,
				TabIndex = ++this.tabIndex,
			};

			new StaticText
			{
				Parent         = frame,
				FormattedText  = "Compte",
				PreferredWidth = 64,
				Dock           = DockStyle.Left,
			};

			FrameBox container;
			AbstractTextField field;
			var comptes = this.comptabilitéEntity.PlanComptable.Where (x => this.CompteFilter (x)).OrderBy (x => x.Numéro);
			//?var marshaler = Marshaler.Create<FormattedText> (() => this.NuméroCompte, x => this.NuméroCompte = x);
			UIBuilder.CreateAutoCompleteTextField (frame, comptes, out container, out field);
			container.PreferredWidth = 100;
			container.Dock = DockStyle.Left;
			container.Margins = new Margins (0, 20, 0, 0);
			container.TabIndex = ++this.tabIndex;

			var graphicsButton = new CheckButton
			{
				Parent         = frame,
				Text           = "Graphique du solde",
				PreferredWidth = 120,
				ActiveState    = this.Options.HasGraphics ? ActiveState.Yes : ActiveState.No,
				Dock           = DockStyle.Left,
			};

			field.TextChanged += delegate
			{
				this.NuméroCompte = field.FormattedText;
				this.UpdateTitle ();
				optionsChanged ();
			};

			graphicsButton.ActiveStateChanged += delegate
			{
				this.Options.HasGraphics = (graphicsButton.ActiveState == ActiveState.Yes);
				optionsChanged ();
			};
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
			var compte = this.comptabilitéEntity.PlanComptable.Where (x => x.Numéro == this.Options.NuméroCompte).FirstOrDefault ();

			if (compte == null)
			{
				this.titleLabel.FormattedText = null;
			}
			else
			{
				this.titleLabel.FormattedText = TextFormatter.FormatText ("Compte", compte.Numéro, compte.Titre).ApplyBold ().ApplyFontSize (14.0);
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
				return this.Options.NuméroCompte;
			}
			set
			{
				this.Options.NuméroCompte = PlanComptableDataAccessor.GetCompteNuméro (value);
			}
		}

		private ExtraitDeCompteOptions Options
		{
			get
			{
				return this.options as ExtraitDeCompteOptions;
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
