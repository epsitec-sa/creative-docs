//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;
using Epsitec.Common.Types.Converters;
using Epsitec.Common.Widgets;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.DataAccessors;
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
		public EditionArticleDefinitionViewController(string name, Entities.ArticleDefinitionEntity entity)
			: base (name, entity)
		{
		}

		protected override void CreateUI()
		{
			using (var builder = new UIBuilder (this))
			{
				builder.CreateHeaderEditorTile ();
				builder.CreateEditionTitleTile ("Data.ArticleDefinition", "Article");

				this.CreateUIMain1         (builder);
				this.CreateUIGroup         (builder);
				this.CreateUIMain2         (builder);
				this.CreateUIUnitOfMeasure (builder);
				this.CreateUICategory      (builder);

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
			var controller = new SelectionController<ArticleGroupEntity>
			{
				CollectionValueGetter    = () => this.Entity.ArticleGroups,
				PossibleItemsGetter      = () => CoreProgram.Application.Data.GetArticleGroups (),
				ToFormattedTextConverter = x => TextFormatter.FormatText (x.Name)
			};

			builder.CreateEditionDetailedItemPicker ("ArticleGroups", this.Entity, "Groupes auxquels l'article appartient", controller, BusinessLogic.EnumValueCardinality.Any, ViewControllerMode.Summary, 2);
		}

		private void CreateUIMain2(UIBuilder builder)
		{
			var tile = builder.CreateEditionTile ();

			builder.CreateMargin (tile, horizontalSeparator: true);

			var shortToolbarController = new ArticleParameterControllers.ArticleParameterToolbarController (this.TileContainer);
			shortToolbarController.CreateUI (tile.Container, "Description courte");
			var shortTextField = builder.CreateTextField (tile, 0, null, Marshaler.Create (() => this.Entity.ShortDescription, x => this.Entity.ShortDescription = x));
			shortToolbarController.UpdateUI (this.Entity, null, shortTextField);

			var longToolbarController = new ArticleParameterControllers.ArticleParameterToolbarController (this.TileContainer);
			longToolbarController.CreateUI (tile.Container, "Description longue");
			var longTextField = builder.CreateTextFieldMulti (tile, 68, null, Marshaler.Create (() => this.Entity.LongDescription, x => this.Entity.LongDescription = x));
			longToolbarController.UpdateUI (this.Entity, null, longTextField);

			builder.CreateMargin (tile, horizontalSeparator: true);

			builder.CreateAutoCompleteTextField (tile, 0, "Mode de TVA (acquisition)", Marshaler.Create (this.Entity, x => x.InputVatCode,  (x, v) => x.InputVatCode = v),  BusinessLogic.Enumerations.GetInputVatCodes  (), x => TextFormatter.FormatText (x.Values[0], "-", x.Values[1]));
			builder.CreateAutoCompleteTextField (tile, 0, "Mode de TVA (vente)",       Marshaler.Create (this.Entity, x => x.OutputVatCode, (x, v) => x.OutputVatCode = v), BusinessLogic.Enumerations.GetOutputVatCodes (), x => TextFormatter.FormatText (x.Values[0], "-", x.Values[1]));
		}

		private void CreateUIUnitOfMeasure(UIBuilder builder)
		{
			var controller = new SelectionController<UnitOfMeasureEntity>
			{
				ValueGetter         = () => this.Entity.BillingUnit,
				ValueSetter         = x => this.Entity.BillingUnit = x.WrapNullEntity (),
				ReferenceController = new ReferenceController (() => this.Entity.BillingUnit, creator: this.CreateNewUnitOfMeasure),
				PossibleItemsGetter = () => CoreProgram.Application.Data.GetUnitOfMeasure (),

				ToTextArrayConverter     = x => new string[] { x.Name.ToSimpleText (), x.Code },
				ToFormattedTextConverter = x => TextFormatter.FormatText (x.Name, "(", x.Code, ")")
			};

			builder.CreateAutoCompleteTextField ("Unité", controller);
		}

		private void CreateUICategory(UIBuilder builder)
		{
			var controller = new SelectionController<ArticleCategoryEntity>
			{
				ValueGetter         = () => this.Entity.ArticleCategory,
				ValueSetter         = x => this.Entity.ArticleCategory = x.WrapNullEntity (),
				ReferenceController = new ReferenceController (() => this.Entity.ArticleCategory, creator: this.CreateNewCategory),
				PossibleItemsGetter = () => CoreProgram.Application.Data.GetArticleCategories (),

				ToTextArrayConverter     = x => new string[] { x.Name },
				ToFormattedTextConverter = x => TextFormatter.FormatText (x.Name)
			};

			builder.CreateAutoCompleteTextField ("Catégorie", controller);
		}


		private NewEntityReference CreateNewUnitOfMeasure(DataContext context)
		{
			var title = context.CreateEmptyEntity<UnitOfMeasureEntity> ();
			return title;
		}

		private NewEntityReference CreateNewCategory(DataContext context)
		{
			return context.CreateEmptyEntity<ArticleCategoryEntity> ();
		}
	}


	/// <summary>
	/// Méthode d'extension pour l'entité ArticleDefinitionEntity, qui ajoute une méthode AllArticleGroups()
	/// contenant toutes les définitions de groupe (et pas seulement celles utilisées par l'article).
	/// Il n'est hélas pas possible de définir une propriété d'extension, mais seulement une méthode.
	/// D'où l'écriture AllArticleGroups() avec les parenthèses.
	/// Donc:
	/// article.ArticleGroups       -> liste des ArticleGroupEntity de cet article
	/// article.AllArticleGroups () -> liste de tous les ArticleGroupEntity connus
	/// </summary>
	public static class ArticleDefinitionEntityExtension
	{
		public static IList<ArticleGroupEntity> AllArticleGroups(this ArticleDefinitionEntity entity)
		{
			return ArticleDefinitionEntityExtension.allArticleGroups;
		}

		private static readonly IList<ArticleGroupEntity> allArticleGroups = CoreProgram.Application.Data.GetArticleGroups ().ToList ();
	}
}
