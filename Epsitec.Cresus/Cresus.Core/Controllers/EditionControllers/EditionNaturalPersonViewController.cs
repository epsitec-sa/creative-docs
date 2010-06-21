//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;
using Epsitec.Common.Types.Converters;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Widgets;
using Epsitec.Cresus.Core.Widgets.Tiles;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers.EditionControllers
{
	public class EditionNaturalPersonViewController : EditionViewController<Entities.NaturalPersonEntity>
	{
		public EditionNaturalPersonViewController(string name, Entities.NaturalPersonEntity entity)
			: base (name, entity)
		{
		}

		protected override void CreateUI(TileContainer container)
		{
			using (var builder = new UIBuilder (container, this))
			{
				builder.CreateHeaderEditorTile ();
				builder.CreateEditionTitleTile ("Data.NaturalPerson", "Personne physique");

				this.CreateUITitle (builder);
				this.CreateUIFirstnameAndLastname (builder);
				this.CreateUIGender (builder);
				this.CreateUIBirthDate (builder);

				builder.CreateFooterEditorTile ();
			}
		}
		
		
		private void CreateUITitle(UIBuilder builder)
		{
			builder.CreateAutoCompleteTextField ("Titre",
				new SelectionController<Entities.PersonTitleEntity>
				{
					ValueGetter = () => this.Entity.Title,
					ValueSetter = x => this.Entity.Title = x,
					PossibleItemsGetter = () => CoreProgram.Application.Data.GetTitles (),

					ToTextArrayConverter     = x => new string[] { x.ShortName, x.Name },
					ToFormattedTextConverter = x => UIBuilder.FormatText (x.Name)
				});
		}
		
		private void CreateUIFirstnameAndLastname(UIBuilder builder)
		{
			var tile = builder.CreateEditionTile ();

			builder.CreateTextField (tile, 0, "Prénom", Marshaler.Create (() => this.Entity.Firstname, x => this.Entity.Firstname = x));
			builder.CreateTextField (tile, 0, "Nom",    Marshaler.Create (() => this.Entity.Lastname,  x => this.Entity.Lastname = x));
			builder.CreateMargin    (tile, horizontalSeparator: true);
		}

		private void CreateUIGender(UIBuilder builder)
		{
			builder.CreateAutoCompleteTextField ("Sexe",
				new SelectionController<Entities.PersonGenderEntity>
				{
					ValueGetter = () => this.Entity.Gender,
					ValueSetter = x => this.Entity.Gender = x,
					PossibleItemsGetter = () => CoreProgram.Application.Data.GetGenders (),
				
					ToTextArrayConverter     = x => new string[] { x.Name },
					ToFormattedTextConverter = x => UIBuilder.FormatText (x.Name)
				});
		}

		private void CreateUIBirthDate(UIBuilder builder)
		{
			var tile = builder.CreateEditionTile ();
			builder.CreateTextField (tile, 90, "Date de naissance", Marshaler.Create (() => this.Entity.BirthDate, x => this.Entity.BirthDate = x));
		}
	}
}
