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

		protected override void CreateUI(TileContainer container)
		{
			using (var builder = new UIBuilder (container, this))
			{
				builder.CreateHeaderEditorTile ();
				builder.CreateEditionTitleTile ("Data.Telecom", "Téléphone");

				this.CreateUIRoles (builder);
				this.CreateUITelecomType (builder);
				this.CreateUIPhoneNumber (builder);

				builder.CreateFooterEditorTile ();
			}

			//	Summary:
			var containerController = new TileContainerController (this, container);
			var data = containerController.DataItems;

			this.CreateUIComments (data);

			containerController.GenerateTiles ();
		}


		private void CreateUIRoles(Epsitec.Cresus.Core.UIBuilder builder)
		{
			var controller = new SelectionController<Entities.ContactRoleEntity>
			{
				CollectionValueGetter    = () => this.Entity.Roles,
				PossibleItemsGetter      = () => CoreProgram.Application.Data.GetRoles (),
				ToFormattedTextConverter = x => UIBuilder.FormatText (x.Name)
			};

			builder.CreateEditionDetailedCheck (0, "Choix du ou des rôles souhaités", controller);
		}
		
		private void CreateUITelecomType(Epsitec.Cresus.Core.UIBuilder builder)
		{
			var controller = new SelectionController<Entities.TelecomTypeEntity>
			{
				ValueGetter              = () => this.Entity.TelecomType,
				ValueSetter              = x => this.Entity.TelecomType = x,
				PossibleItemsGetter      = () => CoreProgram.Application.Data.GetTelecomTypes (),
				ToFormattedTextConverter = x => UIBuilder.FormatText (x.Name)
			};
			
			builder.CreateEditionDetailedRadio (0, "Type du numéro de téléphone", controller);
		}
		
		private void CreateUIPhoneNumber(Epsitec.Cresus.Core.UIBuilder builder)
		{
			var tile = builder.CreateEditionTile ();

			builder.CreateTextField (tile, 150, "Numéro de téléphone", Marshaler.Create (() => this.Entity.Number,    x => this.Entity.Number = x));
			builder.CreateTextField (tile, 100, "Numéro interne",      Marshaler.Create (() => this.Entity.Extension, x => this.Entity.Extension = x));
		}


		private void CreateUIComments(SummaryDataItems data)
		{
			data.Add (
				new SummaryData
				{
					AutoGroup    = true,
					Name		 = "Comment",
					IconUri		 = "Data.Comment",
					Title		 = UIBuilder.FormatText ("Commentaires"),
					CompactTitle = UIBuilder.FormatText ("Commentaires"),
					Text		 = CollectionTemplate.DefaultEmptyText
				});

			var template = new CollectionTemplate<CommentEntity> ("Comment", data.Controller)
				.DefineText (x => UIBuilder.FormatText (x.Text))
				.DefineCompactText (x => UIBuilder.FormatText (x.Text));

			data.Add (CollectionAccessor.Create (this.EntityGetter, x => x.Comments, template));
		}


		protected override EditionStatus GetEditionStatus()
		{
			if (string.IsNullOrEmpty (this.Entity.Number) &&
				string.IsNullOrEmpty (this.Entity.Extension))
			{
				return EditionStatus.Empty;
			}

			if (string.IsNullOrEmpty (this.Entity.Number) &&
				!string.IsNullOrEmpty (this.Entity.Extension))
			{
				return EditionStatus.Invalid;
			}

			// TODO: Comment implémenter un vraie validation ? Est-ce que le Marshaler sait faire cela ?

			return EditionStatus.Valid;
		}
	}
}
