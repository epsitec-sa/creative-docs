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
	/// Ce contrôleur gère les options d'affichage du bilan de la comptabilité.
	/// </summary>
	public class BilanOptionsController : AbstractOptionsController
	{
		public BilanOptionsController(ComptaEntity comptaEntity, BilanOptions options)
			: base (comptaEntity, options)
		{
		}


		public override void CreateUI(FrameBox parent, System.Action optionsChanged)
		{
			base.CreateUI (parent, optionsChanged);

			this.CreateCheckUI (this.mainFrame);
			this.CreateBudgetUI (this.mainFrame);

			this.UpdateWidgets ();
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

			this.nullButton = new CheckButton
			{
				Parent         = frame,
				FormattedText  = "Affiche les comptes dont le solde est nul",
				PreferredWidth = 230,
				Dock           = DockStyle.Left,
				TabIndex        = ++this.tabIndex,
			};

			this.graphicsButton = new CheckButton
			{
				Parent         = frame,
				Text           = "Graphique du solde",
				PreferredWidth = 120,
				Dock           = DockStyle.Left,
				TabIndex        = ++this.tabIndex,
			};

			this.nullButton.ActiveStateChanged += delegate
			{
				if (!this.ignoreChange)
				{
					this.Options.ComptesNuls = (nullButton.ActiveState == ActiveState.Yes);
					this.OptionsChanged ();
				}
			};

			this.graphicsButton.ActiveStateChanged += delegate
			{
				if (!this.ignoreChange)
				{
					this.Options.HasGraphics = (graphicsButton.ActiveState == ActiveState.Yes);
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
			this.UpdateBudget ();

			this.ignoreChange = true;
			this.nullButton    .ActiveState = this.Options.ComptesNuls ? ActiveState.Yes : ActiveState.No;
			this.graphicsButton.ActiveState = this.Options.HasGraphics ? ActiveState.Yes : ActiveState.No;
			this.ignoreChange = false;

			base.UpdateWidgets ();
		}

		private BilanOptions Options
		{
			get
			{
				return this.options as BilanOptions;
			}
		}


		private CheckButton			nullButton;
		private CheckButton			graphicsButton;
	}
}
