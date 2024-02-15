//	Copyright © 2010-2019, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Widgets;

using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Factories;
using Epsitec.Cresus.Core.Widgets;

namespace Epsitec.Cresus.Core.Controllers.SpecialControllers
{
	/// <summary>
	/// Ce contrôleur gère l'ensemble de la saisie des valeurs des articles paramétrés (donc pour tous
	/// les paramètres de l'article concerné).
	/// </summary>
	public class SpecialValuesArticleParameterController : IEntitySpecialController
	{
		public SpecialValuesArticleParameterController(TileContainer tileContainer, OptionValueEntity optionValueEntity)
		{
			this.tileContainer     = tileContainer;
			this.optionValueEntity = optionValueEntity;
		}


		public void CreateUI(Widget parent, UIBuilder builder, bool isReadOnly)
		{
			this.isReadOnly = isReadOnly;

			var frameBox = parent as FrameBox;
			System.Diagnostics.Debug.Assert (frameBox != null);

			var c = new ArticleParameterControllers.ValuesArticleParameterController (this.tileContainer, frameBox);
			c.CreateUI (frameBox);
			c.UpdateUI (this.optionValueEntity);
		}


		private class Factory : DefaultEntitySpecialControllerFactory<OptionValueEntity>
		{
			protected override IEntitySpecialController Create(TileContainer container, OptionValueEntity entity, ViewId mode)
			{
				return new SpecialValuesArticleParameterController (container, entity);
			}
		}

	
		private readonly TileContainer			tileContainer;
		private readonly OptionValueEntity		optionValueEntity;

		private bool							isReadOnly;
	}
}
