//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types.Converters;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Widgets;
using Epsitec.Cresus.Core.Widgets.Tiles;

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Types;

namespace Epsitec.Cresus.Core.Controllers.EditionControllers
{
	public class EditionNaturalPersonViewController : EntityViewController<Entities.NaturalPersonEntity>
	{
		public EditionNaturalPersonViewController(string name, Entities.NaturalPersonEntity entity)
			: base (name, entity)
		{
		}

		protected override void CreateUI(TileContainer container)
		{
			var builder = new UIBuilder (container, this);

			builder.CreateHeaderEditorTile ();
			builder.CreateEditionGroupingTile ("Data.NaturalPerson", "Personne physique");

			this.CreateUITitle (builder);
			this.CreateUIFirstnameAndLastname (builder);
			this.CreateUIGender (builder);
			this.CreateUIBirthDate (builder);
			
			builder.CreateFooterEditorTile ();

			UI.SetInitialFocus (container);
		}
		
		
		private void CreateUITitle(UIBuilder builder)
		{
			builder.CreateEditionHintEditor ("Titre",
				new HintEditorController<Entities.PersonTitleEntity>
				{
					ValueGetter = () => this.Entity.Title,
					ValueSetter = x => this.Entity.Title = x,
					ItemsGetter = () => CoreProgram.Application.Data.GetTitles (),

					ToTextArrayConverter     = x => new string[] { x.ShortName, x.Name },
					ToFormattedTextConverter = x => UIBuilder.FormatText (x.Name)
				});
		}
		
		private void CreateUIFirstnameAndLastname(UIBuilder builder)
		{
			var tile = builder.CreateEditionTile ();
			builder.CreateTextField (tile, 0, "Prénom", Marshaler.Create (() => this.Entity.Firstname, x => this.Entity.Firstname = x));
			builder.CreateTextField (tile, 0, "Nom",    Marshaler.Create (() => this.Entity.Lastname,  x => this.Entity.Lastname = x));
			builder.CreateMargin (tile, horizontalSeparator: true);
		}

		private void CreateUIGender(UIBuilder builder)
		{
			builder.CreateEditionHintEditor ("Sexe",
				new HintEditorController<Entities.PersonGenderEntity>
				{
					ValueGetter = () => this.Entity.Gender,
					ValueSetter = x => this.Entity.Gender = x,
					ItemsGetter = () => CoreProgram.Application.Data.GetGenders (),
				
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
