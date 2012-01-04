//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Common.Types;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.DataAccessors;
using Epsitec.Cresus.Core.Controllers.ComptabilitéControllers;
using Epsitec.Cresus.Core.Widgets;
using Epsitec.Cresus.Core.Widgets.Tiles;
using Epsitec.Cresus.Core.Helpers;

using Epsitec.Cresus.DataLayer.Context;

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Core.Factories;

namespace Epsitec.Cresus.Core.Controllers.SpecialControllers
{
	/// <summary>
	/// Ce contrôleur permet de voir un document déjà imprimé.
	/// </summary>
	public class ComptabilitéController : IEntitySpecialController
	{
		public ComptabilitéController(TileContainer tileContainer, ComptabilitéEntity comptabilitéEntity, int mode)
		{
			this.tileContainer = tileContainer;
			this.comptabilitéEntity = comptabilitéEntity;
			this.mode = mode;
		}


		public void CreateUI(Widget parent, UIBuilder builder, bool isReadOnly)
		{
			this.isReadOnly = isReadOnly;

			var frameBox = parent as FrameBox;
			System.Diagnostics.Debug.Assert (frameBox != null);

#if false
			if (this.mode == 0)  // plan comptable ?
			{
				var controller = new PlanComptableController (this.tileContainer, this.comptabilitéEntity);
				controller.CreateUI (frameBox);
			}

			if (this.mode == 1)  // journal ?
			{
				var controller = new JournalController (this.tileContainer, this.comptabilitéEntity);
				controller.CreateUI (frameBox);
			}

			if (this.mode == 2)  // balance ?
			{
				var controller = new BalanceController (this.tileContainer, this.comptabilitéEntity);
				controller.CreateUI (frameBox);
			}

			if (this.mode == 3)  // extrait de compte ?
			{
				var controller = new ExtraitDeCompteController (this.tileContainer, this.comptabilitéEntity);
				controller.CreateUI (frameBox);
			}

			if (this.mode == 4)  // bilan ?
			{
				var controller = new BilanController (this.tileContainer, this.comptabilitéEntity);
				controller.CreateUI (frameBox);
			}

			if (this.mode == 5)  // pertes et profits ?
			{
				var controller = new PPController (this.tileContainer, this.comptabilitéEntity);
				controller.CreateUI (frameBox);
			}

			if (this.mode == 6)  // exploitation ?
			{
				var controller = new ExploitationController (this.tileContainer, this.comptabilitéEntity);
				controller.CreateUI (frameBox);
			}
#endif
		}


		private class Factory : DefaultEntitySpecialControllerFactory<ComptabilitéEntity>
		{
			protected override IEntitySpecialController Create(TileContainer container, ComptabilitéEntity entity, int mode)
			{
				return new ComptabilitéController (container, entity, mode);
			}
		}

	
		private readonly TileContainer			tileContainer;
		private readonly ComptabilitéEntity		comptabilitéEntity;
		private readonly int					mode;

		private bool							isReadOnly;
	}
}
