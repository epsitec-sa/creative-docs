//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types.Converters;

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

			var group = builder.CreateEditionGroupingTile ("Data.NaturalPerson", "Personne physique");
			
			var titleTile  = builder.CreateEditionTile (group, this.Entity);
			var mainTile1  = builder.CreateEditionTile (group, this.Entity);
			var genderTile = builder.CreateEditionTile (group, this.Entity);
			var mainTile2  = builder.CreateEditionTile (group, this.Entity);

			builder.CreateFooterEditorTile ();

			var titleHint = builder.CreateHintEditor (titleTile, "Titre", this.Entity.Title, null, x => this.Entity.Title = x as Entities.PersonTitleEntity);
			var titleCtrl = new HintEditorController<Entities.PersonTitleEntity>
			{
				ValueGetter = () => this.Entity.Title,
				ValueSetter = x => this.Entity.Title = x,
				
				ItemsGetter = () => CoreProgram.Application.Data.GetTitles (),
				
				ToTextArrayConverter     = x => new string[] { x.ShortName, x.Name },
				ToFormattedTextConverter = x => UIBuilder.FormatText (x.Name)
			};
			titleCtrl.Attach (titleHint);

			builder.CreateTextField (mainTile1.Container, 0, "Prénom", this.Entity.Firstname, x => this.Entity.Firstname = x, Validators.StringValidator.Validate);
			builder.CreateTextField (mainTile1.Container, 0, "Nom", this.Entity.Lastname, x => this.Entity.Lastname = x, Validators.StringValidator.Validate);
			builder.CreateMargin (mainTile1.Container, true);

			var genderHint = builder.CreateHintEditor (genderTile, "Sexe", this.Entity.Gender, null, x => this.Entity.Gender = x as Entities.PersonGenderEntity);
			var genderCtrl = new HintEditorController<Entities.PersonGenderEntity>
			{
				ValueGetter = () => this.Entity.Gender,
				ValueSetter = x => this.Entity.Gender = x,
				ItemsGetter = () => CoreProgram.Application.Data.GetGenders (),
				ToTextArrayConverter = x => new string[] { x.Name },
				ToFormattedTextConverter = x => UIBuilder.FormatText (x.Name)
			};
			genderCtrl.Attach (genderHint);

			builder.CreateTextField (mainTile2.Container, 90, "Date de naissance", Marshaler.Create (() => this.Entity.BirthDate, x => this.Entity.BirthDate = x));

			UI.SetInitialFocus (container);
#endif
		}
	}
}
