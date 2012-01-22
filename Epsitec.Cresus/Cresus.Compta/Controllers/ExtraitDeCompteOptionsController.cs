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

using Epsitec.Cresus.Compta.Accessors;
using Epsitec.Cresus.Compta.Entities;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.Controllers
{
	/// <summary>
	/// Ce contrôleur gère les options d'affichage d'un extrait de compte de la comptabilité.
	/// </summary>
	public class ExtraitDeCompteOptionsController : AbstractOptionsController
	{
		public ExtraitDeCompteOptionsController(ComptaEntity comptaEntity, ExtraitDeCompteOptions options)
			: base (comptaEntity, options)
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
				Dock                = DockStyle.Top,
				Margins             = new Margins (0, 0, 0, 6),
				Padding             = new Margins (5),
			};

			this.CreateEditionUI (this.toolbar, optionsChanged);
			this.CreateDatesUI (this.toolbar, optionsChanged);
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
			var comptes = this.comptaEntity.PlanComptable.Where (x => this.CompteFilter (x)).OrderBy (x => x.Numéro);
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
				optionsChanged ();
			};

			graphicsButton.ActiveStateChanged += delegate
			{
				this.Options.HasGraphics = (graphicsButton.ActiveState == ActiveState.Yes);
				optionsChanged ();
			};
		}


		private bool CompteFilter(ComptaCompteEntity compte)
		{
			if (compte.Type != TypeDeCompte.Normal &&
				compte.Type != TypeDeCompte.Groupe)
			{
				return false;
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

		private new ExtraitDeCompteOptions Options
		{
			get
			{
				return this.options as ExtraitDeCompteOptions;
			}
		}
	}
}
