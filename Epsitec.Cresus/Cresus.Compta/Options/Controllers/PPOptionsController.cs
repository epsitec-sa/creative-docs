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
	/// Ce contrôleur gère les options d'affichage des pertes et profits de la comptabilité.
	/// </summary>
	public class PPOptionsController : AbstractOptionsController
	{
		public PPOptionsController(AbstractController controller)
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

			this.CreateCheckUI (this.firstFrame);
			this.CreateComparisonUI (ComparisonShowed.All);

			var line = this.CreateSpecialistFrameUI ();
			this.CreateDeepUI (line);
			this.CreateZeroUI (line);

			this.UpdateWidgets ();
		}

		protected void CreateCheckUI(FrameBox parent)
		{
			this.CreateGraphUI (parent);
			this.hasGraphicColumnFrame = this.CreateHasGraphicColumnUI (parent);
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
			this.UpdateZero ();
			this.UpdateHasGraphicColumn ();
			this.UpdateComparison ();

			this.hasGraphicColumnFrame.Visibility      = !this.options.ViewGraph;
			this.zeroDisplayedInWhiteButton.Visibility = !this.options.ViewGraph;

			base.UpdateWidgets ();
		}

		private DoubleOptions Options
		{
			get
			{
				return this.options as DoubleOptions;
			}
		}


		private FrameBox			hasGraphicColumnFrame;
	}
}
