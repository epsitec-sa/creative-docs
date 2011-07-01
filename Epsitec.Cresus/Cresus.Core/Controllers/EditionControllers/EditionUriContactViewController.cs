//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Types;
using Epsitec.Common.Types.Converters;

using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.DataAccessors;
using Epsitec.Cresus.Core.Library;
using Epsitec.Cresus.Core.Widgets;
using Epsitec.Cresus.Core.Widgets.Tiles;
using Epsitec.Cresus.Bricks;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers.EditionControllers
{
	public class EditionUriContactViewController : EditionViewController<Entities.UriContactEntity>
	{
#if true
		protected override void CreateBricks(BrickWall<UriContactEntity> wall)
		{
			wall.AddBrick ()
				.Input ()
				  .Field (x => x.ContactGroups)
				.End ()
				.Separator ()
				.Input ()
				  .Field (x => x.Uri)
				  .Field (x => x.UriType)
				.End ()
				;
		}
#else
		protected override void CreateUI()
		{
			using (var data = TileContainerController.Setup (this))
			{
				this.CreateUIRoles (data);
				this.CreateUIMail (data);

				this.CreateUIComments (data);
			}
		}


		private void CreateUIRoles(TileDataItems data)
		{
			var tileData = new TileDataItem
			{
				Name            = "UriContactGroups",
				IconUri	        = "Data.UriContact",
				Title	        = TextFormatter.FormatText ("Email"),
				CompactTitle    = TextFormatter.FormatText ("Email"),
				Frameless       = true,
				CreateEditionUI = (tile, builder) =>
				{
					var controller = new SelectionController<Entities.ContactGroupEntity> (this.BusinessContext)
					{
						CollectionValueGetter    = () => this.Entity.ContactGroups,
						ToFormattedTextConverter = x => TextFormatter.FormatText (x.Name)
					};

					builder.CreateEditionDetailedItemPicker (tile, "ContactGroups", this.Entity, "Rôles souhaités", controller, EnumValueCardinality.Any, ViewControllerMode.Summary, 3);
				}
			};

			data.Add (tileData);
		}

		private void CreateUIMail(TileDataItems data)
		{
			var tileData = new TileDataItem
			{
				Name            = "UriContactMail",
				Frameless       = true,
				CreateEditionUI = (tile, builder) =>
				{
					builder.CreateTextField (tile, 0, "Adresse mail", Marshaler.Create (() => this.Entity.Uri, x => this.Entity.Uri = x));
				}
			};

			data.Add (tileData);
		}


		private void CreateUIComments(TileDataItems data)
		{
			SummaryControllers.Common.CreateUIComments (this.BusinessContext, data, this.EntityGetter, x => x.Comments);
		}
#endif
	}
}
