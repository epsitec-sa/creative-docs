//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types.Converters;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Widgets;
using Epsitec.Cresus.Core.Widgets.Tiles;
using Epsitec.Cresus.DataLayer;
using Epsitec.Cresus.DataLayer.Context;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers.EditionControllers
{
	public class EditionNaturalPersonViewController : EditionViewController<NaturalPersonEntity>
	{
		public EditionNaturalPersonViewController(string name, NaturalPersonEntity entity)
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

		protected override void UpdateEmptyEntityStatus(DataLayer.Context.DataContext context, bool isEmpty)
		{
			var entity = this.Entity;

			context.UpdateEmptyEntityStatus (entity, isEmpty);
		}

		
		private void CreateUITitle(UIBuilder builder)
		{
			builder.CreateAutoCompleteTextField ("Titre",
				new SelectionController<PersonTitleEntity>
				{
					ValueGetter         = () => this.Entity.Title,
					ValueSetter         = x => this.Entity.Title = x.WrapNullEntity (),
					ReferenceController = new ReferenceController (() => this.Entity.Title, creator: this.CreateNewTitle),
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
				new SelectionController<PersonGenderEntity>
				{
					ValueGetter         = () => this.Entity.Gender,
					ValueSetter         = x => this.Entity.Gender = x.WrapNullEntity (),
					ReferenceController = new ReferenceController (() => this.Entity.Gender, mode: ViewControllerMode.None),
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

		private NewEntityReference CreateNewTitle(DataContext context)
		{
			var title = context.CreateEmptyEntity<PersonTitleEntity> ();
			return title;
		}
	}
}
