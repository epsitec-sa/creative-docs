//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;
using Epsitec.Common.Types.Converters;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Widgets;
using Epsitec.Cresus.Core.Widgets.Tiles;
using Epsitec.Cresus.Core.Business;

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Core.Library;

namespace Epsitec.Cresus.Core.Controllers.EditionControllers
{
	public class EditionUnitOfMeasureGroupViewController : EditionViewController<Entities.UnitOfMeasureGroupEntity>
	{
		protected override void CreateUI()
		{
			using (var builder = new UIBuilder (this))
			{
				builder.CreateHeaderEditorTile ();
				builder.CreateEditionTitleTile ("Data.UnitOfMeasureGroup", "Groupe d'unités de mesure");

				this.CreateUIWarning (builder);
				this.CreateUIMain    (builder);

				builder.CreateFooterEditorTile ();
			}
		}


		private void CreateUIWarning(UIBuilder builder)
		{
			var tile = builder.CreateEditionTile ();

			builder.CreateWarning (tile);
		}

		private void CreateUIMain(UIBuilder builder)
		{
			var tile = builder.CreateEditionTile ();

			builder.CreateTextField             (tile,   0, "Nom du groupe",         Marshaler.Create (() => this.Entity.Name,        x => this.Entity.Name = x));
			builder.CreateTextFieldMulti        (tile,  70, "Description du groupe", Marshaler.Create (() => this.Entity.Description, x => this.Entity.Description = x));
			builder.CreateAutoCompleteTextField (tile, 100, "Catégorie du groupe",   Marshaler.Create (() => this.Entity.Category,    x => this.Entity.Category = x), EnumKeyValues.FromEnum<UnitOfMeasureCategory> ());
		}
	}
}
