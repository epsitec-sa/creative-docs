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
	public class EditionTelecomContactViewController : EditionViewController<Entities.TelecomContactEntity>
	{
#if true
		protected override void CreateBricks(BrickWall<TelecomContactEntity> wall)
		{
			wall.AddBrick ()
				.Input ()
				  .Field (x => x.ContactGroups)
				.End ()
				.Separator ()
				.Input ()
				  .Field (x => x.TelecomType)
				  .Field (x => x.Number)
				  .Field (x => x.Extension)
				.End ()
				;
		}
#else
		protected override void CreateUI()
		{
			using (var data = TileContainerController.Setup (this))
			{
				this.CreateUIRoles (data);
				this.CreateUITelecomType (data);
				this.CreateUIPhoneNumber (data);

				this.CreateUIComments (data);
			}
		}


		private void CreateUIRoles(TileDataItems data)
		{
			var tileData = new TileDataItem
			{
				Name            = "TelecomContactGroups",
				IconUri	        = "Data.TelecomContact",
				Title	        = TextFormatter.FormatText ("Téléphone"),
				CompactTitle    = TextFormatter.FormatText ("Téléphone"),
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

		private void CreateUITelecomType(TileDataItems data)
		{
			var tileData = new TileDataItem
			{
				Name            = "TelecomContactType",
				Frameless       = true,
				CreateEditionUI = (tile, builder) =>
				{
					var controller = new SelectionController<Entities.TelecomTypeEntity> (this.BusinessContext)
					{
						ValueGetter              = () => this.Entity.TelecomType,
						ValueSetter              = x => this.Entity.TelecomType = x,
						ToFormattedTextConverter = x => TextFormatter.FormatText (x.Name)
					};

					builder.CreateEditionDetailedItemPicker (tile, "Type du numéro de téléphone", controller, EnumValueCardinality.ExactlyOne);
				}
			};

			data.Add (tileData);
		}

		private void CreateUIPhoneNumber(TileDataItems data)
		{
			var tileData = new TileDataItem
			{
				Name            = "TelecomContactNumber",
				Frameless       = true,
				CreateEditionUI = (tile, builder) =>
				{
					builder.CreateTextField (tile, 150, "Numéro de téléphone", Marshaler.Create (() => this.Entity.Number,    x => this.Entity.Number = x));
					builder.CreateTextField (tile, 100, "Numéro interne",      Marshaler.Create (() => this.Entity.Extension, x => this.Entity.Extension = x));
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
