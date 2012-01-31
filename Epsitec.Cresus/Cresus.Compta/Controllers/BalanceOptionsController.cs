//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Common.Types;

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
	/// Ce contrôleur gère les options d'affichage de la balance de vérification de la comptabilité.
	/// </summary>
	public class BalanceOptionsController : AbstractOptionsController
	{
		public BalanceOptionsController(ComptaEntity comptaEntity, BalanceOptions options)
			: base (comptaEntity, options)
		{
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

			this.CreateProfondeurUI (frame);

			this.buttonComptesNuls = new CheckButton
			{
				Parent         = frame,
				FormattedText  = "Affiche les comptes dont le solde est nul",
				PreferredWidth = 220,
				Dock           = DockStyle.Left,
				TabIndex       = ++this.tabIndex,
			};

			this.UpdateWidgets ();

			this.buttonComptesNuls.ActiveStateChanged += delegate
			{
				if (!this.ignoreChange)
				{
					this.Options.ComptesNuls = !this.Options.ComptesNuls;
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
			this.UpdateProfondeur ();

			this.ignoreChange = true;
			this.buttonComptesNuls.ActiveState = this.Options.ComptesNuls ? ActiveState.Yes : ActiveState.No;
			this.ignoreChange = false;

			base.UpdateWidgets ();
		}

		private BalanceOptions Options
		{
			get
			{
				return this.options as BalanceOptions;
			}
		}


		private CheckButton			buttonComptesNuls;
	}
}
