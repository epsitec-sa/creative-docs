﻿//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
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
	public class EditionCountryViewController : EditionViewController<Entities.CountryEntity>
	{
		public EditionCountryViewController(string name, Entities.CountryEntity entity)
			: base (name, entity)
		{
		}

		protected override void CreateUI(TileContainer container)
		{
			using (var builder = new UIBuilder (container, this))
			{
				builder.CreateHeaderEditorTile ();
				builder.CreateEditionTitleTile ("Data.Mail", "Pays");

				this.CreateUIMain (builder);

				builder.CreateFooterEditorTile ();
			}
		}

		protected override EditionStatus GetEditionStatus()
		{
			if (this.Entity.IsEmpty ())
			{
				return EditionStatus.Empty;
			}
			else
			{
				return EditionStatus.Valid;
			}
		}

		protected override void UpdateEmptyEntityStatus(DataLayer.DataContext context, bool isEmpty)
		{
			var entity = this.Entity;

			context.UpdateEmptyEntityStatus (entity, isEmpty);
		}

		private void CreateUIMain(UIBuilder builder)
		{
			var tile = builder.CreateEditionTile ();

			builder.CreateWarning   (tile);
			builder.CreateTextField (tile,  0, "Pays",                    Marshaler.Create (() => this.Entity.Name, x => this.Entity.Name = x));
			builder.CreateTextField (tile, 80, "Code ISO à deux lettres", Marshaler.Create (() => this.Entity.Code, x => this.Entity.Code = x));
		}
	}
}
