//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Common.Types;

using Epsitec.Cresus.Compta.Accessors;
using Epsitec.Cresus.Compta.Entities;
using Epsitec.Cresus.Compta.Controllers;
using Epsitec.Cresus.Compta.Options.Data;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.Options.Controllers
{
	/// <summary>
	/// Ce contrôleur gère les options d'affichage du résumé TVA de la comptabilité.
	/// </summary>
	public class RésuméTVAOptionsController : AbstractOptionsController
	{
		public RésuméTVAOptionsController(AbstractController controller)
			: base (controller)
		{
		}


		public override void UpdateContent()
		{
			if (this.showPanel)
			{
				this.UpdateWidgets ();
			}
		}


		public override void CreateUI(FrameBox parent, System.Action optionsChanged)
		{
			base.CreateUI (parent, optionsChanged);

			this.CreateCheckUI (this.mainFrame);
		}

		protected void CreateCheckUI(FrameBox parent)
		{
			var frame = new FrameBox
			{
				Parent          = parent,
				PreferredHeight = 20,
				Dock            = DockStyle.Top,
				TabIndex        = ++this.tabIndex,
			};

			this.montreEcrituresButton = new CheckButton
			{
				Parent         = frame,
				FormattedText  = "Montre les écritures",
				PreferredWidth = 160,
				ActiveState    = this.Options.MontreEcritures ? ActiveState.Yes : ActiveState.No,
				Dock           = DockStyle.Left,
				TabIndex       = ++this.tabIndex,
			};

			this.montantTTCButton = new CheckButton
			{
				Parent         = frame,
				FormattedText  = "Affiche les montants TTC",
				PreferredWidth = 170,
				ActiveState    = this.Options.MontantTTC ? ActiveState.Yes : ActiveState.No,
				Dock           = DockStyle.Left,
				TabIndex       = ++this.tabIndex,
			};

			this.parCodeTVAButton = new CheckButton
			{
				Parent         = frame,
				FormattedText  = "Résumé par code TVA",
				PreferredWidth = 160,
				ActiveState    = this.Options.ParCodeTVA ? ActiveState.Yes : ActiveState.No,
				Dock           = DockStyle.Left,
				TabIndex       = ++this.tabIndex,
			};

			this.UpdateWidgets ();

			this.montreEcrituresButton.ActiveStateChanged += delegate
			{
				if (this.ignoreChanges.IsZero)
				{
					this.Options.MontreEcritures = !this.Options.MontreEcritures;
					this.OptionsChanged ();
				}
			};

			this.montantTTCButton.ActiveStateChanged += delegate
			{
				if (this.ignoreChanges.IsZero)
				{
					this.Options.MontantTTC = !this.Options.MontantTTC;
					this.OptionsChanged ();
				}
			};

			this.parCodeTVAButton.ActiveStateChanged += delegate
			{
				if (this.ignoreChanges.IsZero)
				{
					this.Options.ParCodeTVA = !this.Options.ParCodeTVA;
					this.OptionsChanged ();
				}
			};
		}

		protected override void OptionsChanged()
		{
			this.UpdateWidgets ();
			base.OptionsChanged ();
		}

		protected override void UpdateWidgets()
		{
			using (this.ignoreChanges.Enter ())
			{
				this.montreEcrituresButton.ActiveState = this.Options.MontreEcritures ? ActiveState.Yes : ActiveState.No;
				this.montantTTCButton.ActiveState      = this.Options.MontantTTC      ? ActiveState.Yes : ActiveState.No;
				this.parCodeTVAButton.ActiveState      = this.Options.ParCodeTVA      ? ActiveState.Yes : ActiveState.No;
			}

			base.UpdateWidgets ();
		}

		private RésuméTVAOptions Options
		{
			get
			{
				return this.options as RésuméTVAOptions;
			}
		}


		private CheckButton			montreEcrituresButton;
		private CheckButton			montantTTCButton;
		private CheckButton			parCodeTVAButton;
	}
}
