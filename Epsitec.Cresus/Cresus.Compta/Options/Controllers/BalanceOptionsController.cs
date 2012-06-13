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

			var line = this.CreateSpecialistFrameUI (this.mainFrame);
			this.CreateDeepUI (line);
			this.CreateSeparator (line);
			this.CreateZeroFilteredUI (line);
			this.CreateZeroDisplayedInWhiteUI (line);

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
			this.CreateHasGraphicColumnUI (frame);
		}

		protected override bool HasBeginnerSpecialist
		{
			get
			{
				return true;
			}
		}

		protected override void OptionsChanged()
		{
			this.UpdateWidgets ();
			base.OptionsChanged ();
		}

		protected override void UpdateWidgets()
		{
			this.UpdateGraphWidgets ();
			this.UpdateDeep ();
			this.UpdateZeroFiltered ();
			this.UpdateZeroDisplayedInWhite ();
			this.UpdateHasGraphicColumn ();
			this.UpdateComparison ();

			this.zeroFilteredButton.Visibility         = !this.options.ViewGraph;
			this.zeroDisplayedInWhiteButton.Visibility = !this.options.ViewGraph;
			this.hasGraphicColumnButton.Visibility     = !this.options.ViewGraph;

			base.UpdateWidgets ();
		}

		private BalanceOptions Options
		{
			get
			{
				return this.options as BalanceOptions;
			}
		}
	}
}
