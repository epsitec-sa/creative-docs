//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types.Converters;

using Epsitec.Cresus.Core.Widgets;
using Epsitec.Cresus.Core.Widgets.Tiles;

using System.Collections.Generic;
using System.Linq;

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
#if false
			UIBuilder builder = new UIBuilder (container, this);
			
			builder.CreateHeaderEditorTile ();

			var person = this.Entity;
			var group = builder.CreateEditionGroupingTile ("Data.NaturalPerson", "Personne physique");

			var accessor = new Accessors.NaturalPersonAccessor (person);
			var tile = builder.CreateEditionTile (group, accessor);

			builder.CreateFooterEditorTile ();

			builder.CreateCombo (tile.Container, 150, "Titre", accessor.TitleInitializer, false, false, false, accessor.NaturalTitle, x => accessor.NaturalTitle = x, null);
			builder.CreateTextField (tile.Container, 0, "Prénom", accessor.Entity.Firstname, x => accessor.Entity.Firstname = x, Validators.StringValidator.Validate);
			builder.CreateTextField (tile.Container, 0, "Nom", accessor.Entity.Lastname, x => accessor.Entity.Lastname = x, Validators.StringValidator.Validate);
			builder.CreateMargin (tile.Container, true);
			builder.CreateCombo (tile.Container, 0, "Sexe", accessor.GenderInitializer, true, false, true, accessor.Gender, x => accessor.Gender = x, null);
			builder.CreateTextField (tile.Container, 75, "Date de naissance", accessor.NaturalBirthDate, x => accessor.NaturalBirthDate = x, null);

			UI.SetInitialFocus (container);
#else
			var builder = new UIBuilder (container, this);

			builder.CreateHeaderEditorTile ();

			var person = this.Entity;
			var group = builder.CreateEditionGroupingTile ("Data.NaturalPerson", "Personne physique");
			var tile = builder.CreateEditionTile (group, this.Entity);

			builder.CreateFooterEditorTile ();

			var titleHint = builder.CreateHintEditor (tile.Container, "Titre", this.Entity.Title, null, x => this.Entity.Title = x as Entities.PersonTitleEntity);
			var titleCtrl = new HintEditorController<Entities.PersonTitleEntity>
			{
				ValueGetter = () => this.Entity.Title,
				ValueSetter = x => this.Entity.Title = x,
				Items = CoreProgram.Application.Data.GetTitles (),
				ToTextArrayConverter = x => new string[] { x.ShortName, x.Name },
				ToFormattedTextConverter = x => UIBuilder.FormatText (x.Name)
			};

			titleCtrl.Attach (titleHint);
			
			builder.CreateTextField (tile.Container, 0, "Prénom", this.Entity.Firstname, x => this.Entity.Firstname = x, Validators.StringValidator.Validate);
			builder.CreateTextField (tile.Container, 0, "Nom", this.Entity.Lastname, x => this.Entity.Lastname = x, Validators.StringValidator.Validate);
			builder.CreateMargin (tile.Container, true);

			var genderHint = builder.CreateHintEditor (tile.Container, "Sexe", this.Entity.Gender, null, x => this.Entity.Gender = x as Entities.PersonGenderEntity);
			var genderCtrl = new HintEditorController<Entities.PersonGenderEntity>
			{
				ValueGetter = () => this.Entity.Gender,
				ValueSetter = x => this.Entity.Gender = x,
				Items = CoreProgram.Application.Data.GetGenders (),
				ToTextArrayConverter = x => new string[] { x.Name },
				ToFormattedTextConverter = x => UIBuilder.FormatText (x.Name)
			};

			genderCtrl.Attach (genderHint);
			
			builder.CreateTextField (tile.Container, 75, "Date de naissance", DateConverter.ConvertToString (this.Entity.BirthDate), x => this.Entity.BirthDate = DateConverter.ConvertFromString (x), x => DateConverter.CanConvertFromString (x));

			UI.SetInitialFocus (container);
#endif
		}
	}
}
