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
	/// Ce contrôleur gère les options d'affichage de la balance de vérification de la comptabilité.
	/// </summary>
	public class BalanceOptionsController : AbstractOptionsController
	{
		public BalanceOptionsController(AbstractController controller)
			: base (controller)
		{
		}


		public override void UpdateContent()
		{
			base.UpdateContent ();

			if (this.showPanel)
			{
				this.UpdateWidgets ();
			}
		}


		public override void CreateUI(FrameBox parent, System.Action optionsChanged)
		{
			base.CreateUI (parent, optionsChanged);

			this.CreateCheckUI (this.mainFrame);
			this.CreateComparisonUI (this.mainFrame, ComparisonShowed.All);

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

			this.CreateGraphUI (frame);

			this.zeroButton = new CheckButton
			{
				Parent         = frame,
				FormattedText  = "Affiche en blanc les montants nuls",
				PreferredWidth = 200,
				Dock           = DockStyle.Left,
				TabIndex       = ++this.tabIndex,
			};

			this.graphicsButton = new CheckButton
			{
				Parent         = frame,
				Text           = "Graphique du solde",
				PreferredWidth = 120,
				Dock           = DockStyle.Left,
				TabIndex        = ++this.tabIndex,
			};

			this.zeroButton.ActiveStateChanged += delegate
			{
				if (this.ignoreChanges.IsZero)
				{
					this.Options.ZeroDisplayedInWhite = !this.Options.ZeroDisplayedInWhite;
					this.OptionsChanged ();
				}
			};

			this.graphicsButton.ActiveStateChanged += delegate
			{
				if (this.ignoreChanges.IsZero)
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
			this.UpdateGraphWidgets ();
			this.UpdateComparison ();

			using (this.ignoreChanges.Enter ())
			{
				this.zeroButton.Visibility     = !this.options.ViewGraph;
				this.graphicsButton.Visibility = !this.options.ViewGraph;

				this.zeroButton.ActiveState     = this.Options.ZeroDisplayedInWhite ? ActiveState.Yes : ActiveState.No;
				this.graphicsButton.ActiveState = this.Options.HasGraphics          ? ActiveState.Yes : ActiveState.No;
			}

			base.UpdateWidgets ();
		}

		private BalanceOptions Options
		{
			get
			{
				return this.options as BalanceOptions;
			}
		}


		private CheckButton			zeroButton;
		private CheckButton			graphicsButton;
	}
}
