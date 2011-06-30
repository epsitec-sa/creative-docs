//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Common.Types;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.DataAccessors;
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
	/// Ce contrôleur permet de choisir les plans comptables.
	/// </summary>
	public class ChartsOfAccountsController : IEntitySpecialController
	{
		public ChartsOfAccountsController(TileContainer tileContainer, FinanceSettingsEntity financeSettingsEntity)
		{
			this.tileContainer = tileContainer;
			this.financeSettingsEntity = financeSettingsEntity;
		}


		public void CreateUI(Widget parent, UIBuilder builder, bool isReadOnly)
		{
			this.isReadOnly = isReadOnly;

			var controller = this.tileContainer.Controller as EntityViewController;
			var businessContext = controller.BusinessContext;

			var charts = new ComplexControllers.ChartsOfAccountsController (businessContext, this.financeSettingsEntity);
			charts.CreateUI (parent);
		}


		private class Factory : DefaultEntitySpecialControllerFactory<FinanceSettingsEntity>
		{
			protected override IEntitySpecialController Create(TileContainer container, FinanceSettingsEntity entity, int mode)
			{
				return new ChartsOfAccountsController (container, entity);
			}
		}

	
		private readonly TileContainer tileContainer;
		private readonly FinanceSettingsEntity financeSettingsEntity;

		private bool isReadOnly;
	}
}
