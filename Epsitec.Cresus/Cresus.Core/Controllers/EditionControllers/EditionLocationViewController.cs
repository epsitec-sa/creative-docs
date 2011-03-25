//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Support.EntityEngine;
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
	public class EditionLocationViewController : EditionViewController<Entities.LocationEntity>
	{
		protected override void CreateUI()
		{
			using (var builder = new UIBuilder (this))
			{
				builder.CreateHeaderEditorTile ();
				builder.CreateEditionTitleTile ("Data.Location", "Ville");

				this.CreateUIWarning (builder);
				this.CreateUICountry (builder);
				this.CreateUIMain    (builder);

				builder.CreateFooterEditorTile ();
			}
		}


		private void CreateUIWarning(UIBuilder builder)
		{
			var tile = builder.CreateEditionTile ();

			builder.CreateWarning (tile);
		}

		private void CreateUICountry(UIBuilder builder)
		{
			this.selectedCountry = this.Entity.Country;

			var controller = new SelectionController<CountryEntity> (this.BusinessContext)
			{
				ValueGetter = () => this.Entity.Country,
				ValueSetter = x  => this.Entity.Country = x,
				ReferenceController = new ReferenceController (() => this.Entity.Country),
			};

			this.countryTextField = builder.CreateAutoCompleteTextField ("Nom et code du pays", controller);
		}

		private void CreateUIMain(UIBuilder builder)
		{
			var tile = builder.CreateEditionTile ();

			builder.CreateMargin    (tile, horizontalSeparator: true);
			builder.CreateTextField (tile, 100, "Numéro postal", Marshaler.Create (() => this.Entity.PostalCode, x => this.Entity.PostalCode = x));
			builder.CreateTextField (tile,   0, "Ville",         Marshaler.Create (() => this.Entity.Name,       x => this.Entity.Name = x));
		}


		private AutoCompleteTextField			countryTextField;
		private CountryEntity					selectedCountry;
	}
}
