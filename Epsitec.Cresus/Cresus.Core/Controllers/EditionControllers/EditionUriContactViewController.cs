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
	public class EditionUriContactViewController : EditionViewController<Entities.UriContactEntity>
	{
		public EditionUriContactViewController(string name, Entities.UriContactEntity entity)
			: base (name, entity)
		{
		}

		protected override void CreateUI()
		{
			using (var builder = new UIBuilder (this))
			{
				builder.CreateHeaderEditorTile ();
				builder.CreateEditionTitleTile ("Data.Uri", "Email");

				this.CreateUIRoles (builder);
				this.CreateUIMail (builder);

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
			var controller = new SelectionController<Entities.ContactRoleEntity> (this.BusinessContext)
			{
				CollectionValueGetter    = () => this.Entity.Roles,
				ToFormattedTextConverter = x => TextFormatter.FormatText (x.Name)
			};

			builder.CreateEditionDetailedItemPicker ("ContactRoles", this.Entity, "Rôles souhaités", controller, Business.EnumValueCardinality.Any, ViewControllerMode.Summary, 3);
		}

		private void CreateUIMail(Epsitec.Cresus.Core.UIBuilder builder)
		{
			var tile = builder.CreateEditionTile ();
			builder.CreateTextField (tile, 0, "Adresse mail", Marshaler.Create (() => this.Entity.Uri, x => this.Entity.Uri = x));
		}


		private void CreateUIComments(SummaryDataItems data)
		{
			SummaryControllers.Common.CreateUIComments (this.Data, data, this.EntityGetter, x => x.Comments);
		}

	}
}
