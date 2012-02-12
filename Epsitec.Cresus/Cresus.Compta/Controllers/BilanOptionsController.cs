//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Common.Types;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Controllers;

using Epsitec.Cresus.Compta.Accessors;
using Epsitec.Cresus.Compta.Entities;
using Epsitec.Cresus.Compta.Options.Data;
using Epsitec.Cresus.Compta.Options.Controllers;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.Controllers
{
	/// <summary>
	/// Ce contrôleur gère les options d'affichage du bilan de la comptabilité.
	/// </summary>
	public class BilanOptionsController : AbstractOptionsController
	{
		public BilanOptionsController(AbstractController controller)
			: base (controller)
		{
		}


		public override void CreateUI(FrameBox parent, System.Action optionsChanged)
		{
			base.CreateUI (parent, optionsChanged);

			this.CreateCheckUI (this.mainFrame);
			this.CreateComparisonUI (this.mainFrame, ComparisonShowed.PériodePrécédente | ComparisonShowed.PériodePénultième);

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

			this.nullButton = new CheckButton
			{
				Parent         = frame,
				FormattedText  = "Affiche en blanc les montants nuls",
				PreferredWidth = 200,
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
				if (this.ignoreChanges.IsZero)
				{
					this.Options.HideZero = (nullButton.ActiveState == ActiveState.Yes);
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
			this.UpdateComparison ();

			using (this.ignoreChanges.Enter ())
			{
				this.nullButton.ActiveState = this.Options.HideZero ? ActiveState.Yes : ActiveState.No;
				this.graphicsButton.ActiveState = this.Options.HasGraphics ? ActiveState.Yes : ActiveState.No;
			}

			base.UpdateWidgets ();
		}

		private DoubleOptions Options
		{
			get
			{
				return this.options as DoubleOptions;
			}
		}


		private CheckButton			nullButton;
		private CheckButton			graphicsButton;
	}
}
