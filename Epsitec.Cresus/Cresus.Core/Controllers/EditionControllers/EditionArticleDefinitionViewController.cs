//	Copyright © 2010-2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;
using Epsitec.Common.Types.Converters;
using Epsitec.Common.Widgets;

using Epsitec.Cresus.Bricks;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.DataAccessors;
using Epsitec.Cresus.Core.Library;
using Epsitec.Cresus.Core.Widgets;
using Epsitec.Cresus.Core.Widgets.Tiles;
using Epsitec.Cresus.Core.Helpers;

using Epsitec.Cresus.DataLayer.Context;
using Epsitec.Cresus.DataLayer.Loader;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers.EditionControllers
{
	public class EditionArticleDefinitionViewController : EditionViewController<Entities.ArticleDefinitionEntity>
	{
#if true
		protected override void CreateBricks(BrickWall<ArticleDefinitionEntity> wall)
		{
			wall.AddBrick ()
				.Input ()
				  .Title ("N° de l'article")
				  .Field (x => x.IdA)

				  .Field (x => x.ArticleParameterDefinitions)

				  // TODO: ne pas utiliser un 'special controller' pour ça !

				  .Field (x => x).WithSpecialController (0)  // description courte (x => x.Name)
				  .Field (x => x).WithSpecialController (1)  // description longue (x => x.Description)
				  .Field (x => x.Pictures)
				  .Field (x => x.ArticleGroups)
				  .Field (x => x.ArticleCategory)
				  .Field (x => x.Units)
				.End ()
				;
		}
#else
		protected override void CreateUI()
		{
			using (var builder = new UIBuilder (this))
			{
				builder.CreateHeaderEditorTile ();
				builder.CreateEditionTitleTile ("Data.ArticleDefinition", "Article");

				this.CreateUIMain1              (builder);
				this.CreateUIGroup              (builder);
				this.CreateUIMain2              (builder);
				this.CreateUIUnitOfMeasure      (builder);
				this.CreateUIUnitOfMeasureGroup (builder);
				this.CreateUICategory           (builder);

				builder.CreateFooterEditorTile ();
			}
		}


		private void CreateUIMain1(UIBuilder builder)
		{
			var tile = builder.CreateEditionTile ();

			FrameBox group = builder.CreateGroup (tile, "N° de l'article (principal, externe et interne)");
			builder.CreateTextField (group, DockStyle.Left, 74, Marshaler.Create (() => this.Entity.IdA, x => this.Entity.IdA = x));
			builder.CreateTextField (group, DockStyle.Left, 74, Marshaler.Create (() => this.Entity.IdB, x => this.Entity.IdB = x));
			builder.CreateTextField (group, DockStyle.Left, 74, Marshaler.Create (() => this.Entity.IdC, x => this.Entity.IdC = x));

			builder.CreateMargin (tile, horizontalSeparator: true);
		}

		private void CreateUIGroup(UIBuilder builder)
		{
			var controller = new SelectionController<ArticleGroupEntity> (this.BusinessContext)
			{
				CollectionValueGetter    = () => this.Entity.ArticleGroups,
				ToFormattedTextConverter = x => TextFormatter.FormatText (x.Name).IfNullOrEmptyReplaceWith (CollectionTemplate.DefaultEmptyText),
			};

			builder.CreateEditionDetailedItemPicker ("ArticleGroups", this.Entity, "Groupes auxquels l'article appartient", controller, EnumValueCardinality.Any, ViewControllerMode.Summary, 2);
		}

		private void CreateUIMain2(UIBuilder builder)
		{
			var tile = builder.CreateEditionTile ();

			builder.CreateMargin (tile, horizontalSeparator: true);

			var shortToolbarController = new ArticleParameterControllers.ArticleParameterToolbarController (this.TileContainer);
			shortToolbarController.CreateUI (tile.Container, "Description courte");
			var shortTextField = builder.CreateTextField (tile, 0, null, Marshaler.Create (() => this.Entity.Name, x => this.Entity.Name = x));
			shortToolbarController.UpdateUI (this.Entity, shortTextField);

			var longToolbarController = new ArticleParameterControllers.ArticleParameterToolbarController (this.TileContainer);
			longToolbarController.CreateUI (tile.Container, "Description longue");
			var longTextField = builder.CreateTextFieldMulti (tile, 68, null, Marshaler.Create (() => this.Entity.Description, x => this.Entity.Description = x));
			longToolbarController.UpdateUI (this.Entity, longTextField);

			builder.CreateMargin (tile, horizontalSeparator: true);

			builder.CreateAutoCompleteTextField (tile, 0, "Mode de TVA (acquisition)", Marshaler.Create (() => this.Entity.InputVatCode,  x => this.Entity.InputVatCode = x),  Business.Enumerations.GetInputVatCodes ());
			builder.CreateAutoCompleteTextField (tile, 0, "Mode de TVA (vente)",       Marshaler.Create (() => this.Entity.OutputVatCode, x => this.Entity.OutputVatCode = x), Business.Enumerations.GetOutputVatCodes ());
		}

		private void CreateUIUnitOfMeasure(UIBuilder builder)
		{
			var controller = new SelectionController<UnitOfMeasureEntity> (this.BusinessContext)
			{
				ValueGetter         = () => this.Entity.BillingUnit,
				ValueSetter         = x => this.Entity.BillingUnit = x,
				ReferenceController = new ReferenceController (() => this.Entity.BillingUnit, creator: this.CreateNewUnitOfMeasure),
			};

			builder.CreateAutoCompleteTextField ("Unité", controller);
		}

		private void CreateUIUnitOfMeasureGroup(UIBuilder builder)
		{
			var controller = new SelectionController<UnitOfMeasureGroupEntity> (this.BusinessContext)
			{
				ValueGetter         = () => this.Entity.Units,
				ValueSetter         = x => this.Entity.Units = x,
				ReferenceController = new ReferenceController (() => this.Entity.Units, creator: this.CreateNewUnitOfMeasureGroup),
			};

			builder.CreateAutoCompleteTextField ("Groupe d'unités", controller);
		}

		private void CreateUICategory(UIBuilder builder)
		{
			var controller = new SelectionController<ArticleCategoryEntity> (this.BusinessContext)
			{
				ValueGetter         = () => this.Entity.ArticleCategory,
				ValueSetter         = x => this.Entity.ArticleCategory = x,
				ReferenceController = new ReferenceController (() => this.Entity.ArticleCategory, creator: this.CreateNewCategory),
			};

			builder.CreateAutoCompleteTextField ("Catégorie", controller);
		}


		private NewEntityReference CreateNewUnitOfMeasure(DataContext context)
		{
			var entity = context.CreateEntityAndRegisterAsEmpty<UnitOfMeasureEntity> ();
			return entity;
		}

		private NewEntityReference CreateNewUnitOfMeasureGroup(DataContext context)
		{
			var entity = context.CreateEntityAndRegisterAsEmpty<UnitOfMeasureGroupEntity> ();

			entity.Category = Business.UnitOfMeasureCategory.Unit;
			
			return entity;
		}

		private NewEntityReference CreateNewCategory(DataContext context)
		{
			return context.CreateEntityAndRegisterAsEmpty<ArticleCategoryEntity> ();
		}
#endif
	}
}
