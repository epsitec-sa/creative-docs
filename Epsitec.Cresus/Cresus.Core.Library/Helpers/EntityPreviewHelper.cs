//	Copyright © 2010-2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;

using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Library;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Data;

using Epsitec.Cresus.DataLayer.Context;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Helpers
{
	public static class EntityPreviewHelper
	{
		public static Widget CreateSummaryUI(AbstractEntity entity, Widget parent, CoreData coreData)
		{
			//	Construit l'interface permettant de résumer une entité.
			if (entity is ImageEntity)
			{
				var imageEntity = entity as ImageEntity;
				if (imageEntity.ImageBlob.IsNotNull ())
				{
					return EntityPreviewHelper.CreateBlobSummaryUI (imageEntity.ImageBlob, parent, coreData);
				}
			}
			else if (entity is ImageBlobEntity)
			{
				return EntityPreviewHelper.CreateBlobSummaryUI (entity as ImageBlobEntity, parent, coreData);
			}

			//	Construit un résumé textuel générique.
			return new StaticText
			{
				Parent           = parent,
				FormattedText    = entity.GetSummary (),
				ContentAlignment = ContentAlignment.TopLeft,
				Dock             = DockStyle.Fill,
			};
		}

		private static Widget CreateBlobSummaryUI(ImageBlobEntity blob, Widget parent, CoreData coreData)
		{
			//	Construit l'interface permettant de voir une miniature d'une image.
			var store = coreData.GetComponent<ImageDataStore> ();
			System.Diagnostics.Debug.Assert (store != null);

			var data = store.GetImageData (blob.Code, 300);

			Image image = data == null ? null : data.GetImage ();

			var box = new FrameBox(parent);

			new Widgets.Miniature ()
			{
				Parent = box,
				Image  = image,
				Dock   = DockStyle.Fill,
			};

			return box;
		}
	}
}
