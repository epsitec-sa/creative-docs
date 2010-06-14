//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Types;
using Epsitec.Common.Types.Converters;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Widgets;
using Epsitec.Cresus.Core.Widgets.Tiles;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers.EditionControllers
{
	public class EditionCustomerViewController : EditionViewController<Entities.CustomerEntity>
	{
		public EditionCustomerViewController(string name, Entities.CustomerEntity entity)
			: base (name, entity)
		{
		}

		public override IEnumerable<CoreController> GetSubControllers()
		{
			if (this.personController != null)
			{
				yield return this.personController;
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (this.personController != null)
                {
					this.personController.Dispose ();
					this.personController = null;
                }
			}
			
			base.Dispose (disposing);
		}

		protected override void CreateUI(TileContainer container)
		{
			var builder = new UIBuilder (container, this);

			builder.CreateHeaderEditorTile ();
			builder.CreateEditionTitleTile ("Data.Customer", "Client");

			this.CreateUIMain (builder);

			this.personController = EntityViewController.CreateEntityViewController (this.Name + "Person", this.Entity.Person, ViewControllerMode.Edition, this.Orchestrator);
			this.personController.DataContext = this.DataContext;
			this.personController.CreateUI (container);

			builder.CreateFooterEditorTile ();

			UI.SetInitialFocus (container);
		}


		private void CreateUIMain(UIBuilder builder)
		{
			var tile = builder.CreateEditionTile ();

			builder.CreateTextField (tile, 150, "Numéro de client", Marshaler.Create (() => this.Entity.Id,            x => this.Entity.Id = x));
			builder.CreateTextField (tile,  90, "Client depuis le", Marshaler.Create (() => this.Entity.CustomerSince, x => this.Entity.CustomerSince = x));
		}


		private EntityViewController personController;
	}
}
