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
	/// Ce contrôleur affiche une miniature d'une image bitmap.
	/// </summary>
	public class ImageMiniatureController : IEntitySpecialController
	{
		public ImageMiniatureController(TileContainer tileContainer, ImageBlobEntity imageBlobEntity)
		{
			this.tileContainer = tileContainer;
			this.imageBlobEntity = imageBlobEntity;
		}


		public void CreateUI(Widget parent, UIBuilder builder, bool isReadOnly)
		{
			this.isReadOnly = isReadOnly;

			if (this.imageBlobEntity.IsNotNull ())
			{
				var controller = this.tileContainer.Controller as EntityViewController;
				System.Diagnostics.Debug.Assert (controller != null);

				var store = controller.Data.GetComponent<Epsitec.Cresus.Core.Data.ImageDataStore> ();
				System.Diagnostics.Debug.Assert (store != null);
				
				var data = store.GetImageData (this.imageBlobEntity.Code, 300);
				
				Image image = data == null ? null : data.GetImage ();

				var box = new FrameBox
				{
					Parent = parent,
					PreferredHeight = 300,
					Padding = new Margins (10),
					DrawFrameState = FrameState.Bottom,
					Dock = DockStyle.Top,
					Margins = Widgets.Tiles.TileArrow.GetContainerPadding (Direction.Right),
				};

				var miniature = new Widgets.Miniature ()
				{
					Parent = box,
					Image = image,
					Dock = DockStyle.Fill,
				};
			}
		}


		private class Factory : DefaultEntitySpecialControllerFactory<ImageBlobEntity>
		{
			protected override IEntitySpecialController Create(TileContainer container, ImageBlobEntity entity, int mode)
			{
				return new ImageMiniatureController (container, entity);
			}
		}

	
		private readonly TileContainer tileContainer;
		private readonly ImageBlobEntity imageBlobEntity;

		private bool isReadOnly;
	}
}
