//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Types;
using Epsitec.Common.Types.Converters;

using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.DataAccessors;
using Epsitec.Cresus.Core.Widgets;
using Epsitec.Cresus.Core.Widgets.Tiles;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers.EditionControllers
{
	public class EditionTelecomContactViewController : EditionViewController<Entities.TelecomContactEntity>
	{
		public EditionTelecomContactViewController(string name, Entities.TelecomContactEntity entity)
			: base (name, entity)
		{
		}

#if false
		protected override void CreateUI()
		{
			using (var builder = new UIBuilder (this))
			{
				builder.CreateHeaderEditorTile ();
				builder.CreateEditionTitleTile ("Data.Telecom", "Téléphone");

				this.CreateUIRoles (builder);
				this.CreateUITelecomType (builder);
				this.CreateUIPhoneNumber (builder);

				builder.CreateFooterEditorTile ();
			}

			//	Summary:
			using (var data = TileContainerController.Setup (this))
			{
				this.CreateUIComments (data);
			}
		}


		private void CreateUIRoles(Epsitec.Cresus.Core.UIBuilder builder)
		{
			var controller = new SelectionController<Entities.ContactGroupEntity> (this.BusinessContext)
			{
				CollectionValueGetter    = () => this.Entity.ContactGroups,
				ToFormattedTextConverter = x => TextFormatter.FormatText (x.Name)
			};

			builder.CreateEditionDetailedItemPicker ("ContactRoles", this.Entity, "Rôles souhaités", controller, Business.EnumValueCardinality.Any, ViewControllerMode.Summary, 3);
		}
		
		private void CreateUITelecomType(Epsitec.Cresus.Core.UIBuilder builder)
		{
			var controller = new SelectionController<Entities.TelecomTypeEntity> (this.BusinessContext)
			{
				ValueGetter              = () => this.Entity.TelecomType,
				ValueSetter              = x => this.Entity.TelecomType = x,
				ToFormattedTextConverter = x => TextFormatter.FormatText (x.Name)
			};

			builder.CreateEditionDetailedItemPicker ("Type du numéro de téléphone", controller, Business.EnumValueCardinality.ExactlyOne);
		}
		
		private void CreateUIPhoneNumber(Epsitec.Cresus.Core.UIBuilder builder)
		{
			var tile = builder.CreateEditionTile ();

			builder.CreateTextField (tile, 150, "Numéro de téléphone", Marshaler.Create (() => this.Entity.Number,    x => this.Entity.Number = x));
			builder.CreateTextField (tile, 100, "Numéro interne",      Marshaler.Create (() => this.Entity.Extension, x => this.Entity.Extension = x));
		}


		private void CreateUIComments(TileDataItems data)
		{
			SummaryControllers.Common.CreateUIComments (this.BusinessContext, data, this.EntityGetter, x => x.Comments);
		}
#else
		protected override void CreateUI()
		{
			using (var data = TileContainerController.Setup (this))
			{
				this.CreateUIPhoneNumber (data);
				this.CreateUIRoles (data);
				this.CreateUITelecomType (data);
				this.CreateUIComments (data);
			}
		}


		private void CreateUIRoles(TileDataItems data)
		{
			var tileData = new TileDataItem
			{
				Name            = "TelecomContactRoles",
				//?IconUri	        = "Data.Roles",
				//?Title	        = TextFormatter.FormatText ("Rôles souhaités"),
				//?CompactTitle    = TextFormatter.FormatText ("Rôles"),
				CreateEditionUI = this.CreateEditionUIRoles,
			};

			data.Add (tileData);
		}

		private void CreateEditionUIRoles(EditionTile tile, UIBuilder builder)
		{
			var controller = new SelectionController<Entities.ContactGroupEntity> (this.BusinessContext)
			{
				CollectionValueGetter    = () => this.Entity.ContactGroups,
				ToFormattedTextConverter = x => TextFormatter.FormatText (x.Name)
			};

			//?builder.CreateEditionDetailedItemPicker (tile, "ContactRoles", this.Entity, null, controller, Business.EnumValueCardinality.Any, ViewControllerMode.Summary, 3);
			builder.CreateEditionDetailedItemPicker (tile, "ContactRoles", this.Entity, "Rôles souhaités", controller, Business.EnumValueCardinality.Any, ViewControllerMode.Summary, 3);
		}


		private void CreateUITelecomType(TileDataItems data)
		{
			var tileData = new TileDataItem
			{
				Name            = "TelecomContactType",
				//?IconUri	        = "Data.Type",
				//?Title	        = TextFormatter.FormatText ("Type du numéro de téléphone"),
				//?CompactTitle    = TextFormatter.FormatText ("Type"),
				CreateEditionUI = this.CreateEditionUITelecomType,
			};

			data.Add (tileData);
		}

		private void CreateEditionUITelecomType(EditionTile tile, UIBuilder builder)
		{
			var controller = new SelectionController<Entities.TelecomTypeEntity> (this.BusinessContext)
			{
				ValueGetter              = () => this.Entity.TelecomType,
				ValueSetter              = x => this.Entity.TelecomType = x,
				ToFormattedTextConverter = x => TextFormatter.FormatText (x.Name)
			};

			//?builder.CreateEditionDetailedItemPicker (tile, null, controller, Business.EnumValueCardinality.ExactlyOne);
			builder.CreateEditionDetailedItemPicker (tile, "Type du numéro de téléphone", controller, Business.EnumValueCardinality.ExactlyOne);
		}


		private void CreateUIPhoneNumber(TileDataItems data)
		{
			var tileData = new TileDataItem
			{
				Name            = "TelecomContactNumber",
				IconUri	        = "Data.Telecom",
				Title	        = TextFormatter.FormatText ("Téléphone"),
				CompactTitle    = TextFormatter.FormatText ("Téléphone"),
				CreateEditionUI = this.CreateEditionUIPhoneNumber,
			};

			data.Add (tileData);
		}

		private void CreateEditionUIPhoneNumber(EditionTile tile, UIBuilder builder)
		{
			builder.CreateTextField (tile, 150, "Numéro de téléphone", Marshaler.Create (() => this.Entity.Number,    x => this.Entity.Number = x));
			builder.CreateTextField (tile, 100, "Numéro interne",      Marshaler.Create (() => this.Entity.Extension, x => this.Entity.Extension = x));
		}


		private void CreateUIComments(TileDataItems data)
		{
			SummaryControllers.Common.CreateUIComments (this.BusinessContext, data, this.EntityGetter, x => x.Comments);
		}
#endif
	}
}
