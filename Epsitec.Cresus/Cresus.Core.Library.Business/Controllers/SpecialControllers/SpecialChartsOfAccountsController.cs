//	Copyright © 2011-2019, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Widgets;

using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Factories;
using Epsitec.Cresus.Core.Widgets;

namespace Epsitec.Cresus.Core.Controllers.SpecialControllers
{
	/// <summary>
	/// Ce contrôleur permet de choisir les plans comptables.
	/// </summary>
	public class SpecialChartsOfAccountsController : IEntitySpecialController
	{
		public SpecialChartsOfAccountsController(TileContainer tileContainer, FinanceSettingsEntity financeSettingsEntity)
		{
			this.tileContainer = tileContainer;
			this.financeSettingsEntity = financeSettingsEntity;
		}


		public void CreateUI(Widget parent, UIBuilder builder, bool isReadOnly)
		{
			this.isReadOnly = isReadOnly;

			var controller = this.tileContainer.EntityViewController;
			var businessContext = controller.BusinessContext;

			var charts = new ComplexControllers.ChartsOfAccountsController (businessContext, this.financeSettingsEntity);
			charts.CreateUI (parent);
		}


		private class Factory : DefaultEntitySpecialControllerFactory<FinanceSettingsEntity>
		{
			protected override IEntitySpecialController Create(TileContainer container, FinanceSettingsEntity entity, ViewId mode)
			{
				return new SpecialChartsOfAccountsController (container, entity);
			}
		}

	
		private readonly TileContainer			tileContainer;
		private readonly FinanceSettingsEntity	financeSettingsEntity;

		private bool							isReadOnly;
	}
}
