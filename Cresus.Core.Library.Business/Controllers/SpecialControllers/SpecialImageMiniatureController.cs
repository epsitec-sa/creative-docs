//	Copyright © 2010-2019, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;

using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Factories;
using Epsitec.Cresus.Core.Helpers;
using Epsitec.Cresus.Core.Widgets;

namespace Epsitec.Cresus.Core.Controllers.SpecialControllers
{
	/// <summary>
	/// Ce contrôleur affiche une miniature d'une image bitmap.
	/// </summary>
	public class SpecialImageMiniatureController : IEntitySpecialController
	{
		public SpecialImageMiniatureController(TileContainer tileContainer, ImageBlobEntity imageBlobEntity)
		{
			this.tileContainer = tileContainer;
			this.imageBlobEntity = imageBlobEntity;
		}


		public void CreateUI(Widget parent, UIBuilder builder, bool isReadOnly)
		{
			this.isReadOnly = isReadOnly;

			if (this.imageBlobEntity.IsNotNull ())
			{
				var box = EntityPreviewHelper.CreateSummaryUI (this.imageBlobEntity, parent, this.tileContainer.EntityViewController.Data);

				box.PreferredHeight = 300;
				box.Padding         = new Margins (10);
				box.Dock            = DockStyle.Top;
				box.Margins         = Widgets.Tiles.TileArrow.GetContainerPadding (Direction.Right);
			}
		}


		private class Factory : DefaultEntitySpecialControllerFactory<ImageBlobEntity>
		{
			protected override IEntitySpecialController Create(TileContainer container, ImageBlobEntity entity, ViewId mode)
			{
				return new SpecialImageMiniatureController (container, entity);
			}
		}

	
		private readonly TileContainer tileContainer;
		private readonly ImageBlobEntity imageBlobEntity;

		private bool isReadOnly;
	}
}
