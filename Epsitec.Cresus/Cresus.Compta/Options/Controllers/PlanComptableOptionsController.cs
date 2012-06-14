//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Common.Types;

using Epsitec.Cresus.Compta.Accessors;
using Epsitec.Cresus.Compta.Entities;
using Epsitec.Cresus.Compta.Controllers;
using Epsitec.Cresus.Compta.Options.Data;
using Epsitec.Cresus.Compta.Helpers;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.Options.Controllers
{
	/// <summary>
	/// Ce contrôleur gère les options d'affichage du plan comptable de la comptabilité.
	/// </summary>
	public class PlanComptableOptionsController : AbstractOptionsController
	{
		public PlanComptableOptionsController(AbstractController controller)
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

			this.CreateMainUI (this.firstFrame);

			this.UpdateWidgets ();
		}

		protected void CreateMainUI(FrameBox parent)
		{
			this.CreateCatégoriesUI (parent);
			this.CreateDeepUI (parent);
		}


		protected override bool HasBeginnerSpecialist
		{
			get
			{
				return false;
			}
		}

		protected override void OptionsChanged()
		{
			this.UpdateWidgets ();
			base.OptionsChanged ();
		}

		protected override void LevelChangedAction()
		{
			base.LevelChangedAction ();
		}

		protected override void UpdateWidgets()
		{
			this.UpdateCatégories ();
			this.UpdateDeep ();

			base.UpdateWidgets ();
		}


		private PlanComptableOptions Options
		{
			get
			{
				return this.options as PlanComptableOptions;
			}
		}
	}
}
