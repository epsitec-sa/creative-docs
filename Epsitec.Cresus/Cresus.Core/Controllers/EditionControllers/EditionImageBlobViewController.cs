//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Types;
using Epsitec.Common.Types.Converters;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Widgets;
using Epsitec.Cresus.Core.Widgets.Tiles;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers.EditionControllers
{
	public class EditionImageBlobViewController : EditionViewController<Entities.ImageBlobEntity>
	{
		public EditionImageBlobViewController(string name, Entities.ImageBlobEntity entity)
			: base (name, entity)
		{
		}

		protected override void CreateUI()
		{
			using (var builder = new UIBuilder (this))
			{
				builder.CreateHeaderEditorTile ();
				builder.CreateEditionTitleTile ("Data.ImageBlob", "Image bitmap");

				this.CreateUIMain (builder);

				builder.CreateFooterEditorTile ();
			}
		}

		private void CreateUIMain(UIBuilder builder)
		{
			var tile = builder.CreateEditionTile ();

			builder.CreateWarning   (tile);
			builder.CreateTextField (tile, 0, "Code",           Marshaler.Create (() => this.Entity.Code,         x => this.Entity.Code = x));
			builder.CreateTextField (tile, 0, "Nom de fichier", Marshaler.Create (() => this.Entity.FileName,     x => this.Entity.FileName = x));
			builder.CreateTextField (tile, 0, "URI",            Marshaler.Create (() => this.Entity.FileUri,      x => this.Entity.FileUri = x));
			builder.CreateTextField (tile, 0, "Type MIME",      Marshaler.Create (() => this.Entity.FileMimeType, x => this.Entity.FileMimeType = x));
		}
	}
}
