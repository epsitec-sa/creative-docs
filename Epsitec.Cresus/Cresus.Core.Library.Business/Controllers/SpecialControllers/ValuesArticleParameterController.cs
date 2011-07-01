//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Common.Types;
using Epsitec.Common.Dialogs;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.DataAccessors;
using Epsitec.Cresus.Core.Widgets;
using Epsitec.Cresus.Core.Widgets.Tiles;
using Epsitec.Cresus.Core.Helpers;
using Epsitec.Cresus.Core.Factories;
using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Data;

using Epsitec.Cresus.DataLayer.Context;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers.SpecialControllers
{
	/// <summary>
	/// Ce contrôleur gère l'ensemble de la saisie des valeurs des articles paramétrés (donc pour tous
	/// les paramètres de l'article concerné).
	/// </summary>
	public class ValuesArticleParameterController : IEntitySpecialController
	{
		public ValuesArticleParameterController(TileContainer tileContainer, OptionValueEntity optionValueEntity)
		{
			this.tileContainer = tileContainer;
			this.optionValueEntity = optionValueEntity;
		}


		public void CreateUI(Widget parent, UIBuilder builder, bool isReadOnly)
		{
			this.isReadOnly = isReadOnly;

			var frameBox = parent as FrameBox;
			System.Diagnostics.Debug.Assert (frameBox != null);

			var c = new ArticleParameterControllers.ValuesArticleParameterController (null, null);  // TODO: tester si OK
			c.CreateUI (frameBox);
			c.UpdateUI (this.optionValueEntity);
		}


		private class Factory : DefaultEntitySpecialControllerFactory<OptionValueEntity>
		{
			protected override IEntitySpecialController Create(TileContainer container, OptionValueEntity entity, int mode)
			{
				return new ValuesArticleParameterController (container, entity);
			}
		}

	
		private readonly TileContainer tileContainer;
		private readonly OptionValueEntity optionValueEntity;

		private bool isReadOnly;
	}
}
