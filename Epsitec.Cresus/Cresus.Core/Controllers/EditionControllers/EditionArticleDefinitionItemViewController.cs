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

		protected override void CreateUI(TileContainer container)
		{
			this.tileContainer = container;

			using (var builder = new UIBuilder (container, this))
			{
				builder.CreateHeaderEditorTile ();
				builder.CreateEditionTitleTile ("Data.ArticleDefinition", "Article");

				this.CreateUIMain (builder);
				this.CreateUIUnitOfMeasure (builder);
				this.CreateUICategory (builder);

				builder.CreateFooterEditorTile ();
			}

			//	Summary:
			this.TileContainerController = new TileContainerController (this, container);
			var data = this.TileContainerController.DataItems;

			this.CreateUIGroup (data);
			this.CreateUIParameters (data);

			this.TileContainerController.GenerateTiles ();
		}


		private void CreateUIMain(UIBuilder builder)
		{
			var tile = builder.CreateEditionTile ();

			FrameBox group = builder.CreateGroup (tile, "N° de l'article (principal, externe et interne)");
			builder.CreateTextField (group, DockStyle.Left, 74, Marshaler.Create (() => this.Entity.IdA, x => this.Entity.IdA = x));
			builder.CreateTextField (group, DockStyle.Left, 74, Marshaler.Create (() => this.Entity.IdB, x => this.Entity.IdB = x));
			builder.CreateTextField (group, DockStyle.Left, 74, Marshaler.Create (() => this.Entity.IdC, x => this.Entity.IdC = x));

			builder.CreateMargin (tile, horizontalSeparator: true);

			builder.CreateTextField      (tile,  0, "Description courte", Marshaler.Create (() => this.Entity.ShortDescription, x => this.Entity.ShortDescription = x));
			builder.CreateTextFieldMulti (tile, 68, "Description longue", Marshaler.Create (() => this.Entity.LongDescription, x => this.Entity.LongDescription = x));

			builder.CreateMargin (tile, horizontalSeparator: true);

			builder.CreateAutoCompleteTextField (tile, 0, "Mode de TVA", Marshaler.Create (this.Entity, x => x.VatCode, (x, v) => x.VatCode = v), BusinessLogic.Enumerations.GetGetAllPossibleItemsVatCode (), x => UIBuilder.FormatText (x.Values[0], "-", x.Values[1]));
		}

		private void CreateUIUnitOfMeasure(UIBuilder builder)
		{
			builder.CreateAutoCompleteTextField ("Unité",
				new SelectionController<UnitOfMeasureEntity>
				{
					ValueGetter = () => this.Entity.BillingUnit,
					ValueSetter = x => this.Entity.BillingUnit = x.WrapNullEntity (),
					ReferenceController = new ReferenceController (() => this.Entity.BillingUnit, creator: this.CreateNewUnitOfMeasure),
					PossibleItemsGetter = () => CoreProgram.Application.Data.GetUnitOfMeasure (),

					ToTextArrayConverter     = x => new string[] { x.Name, x.Code },
					ToFormattedTextConverter = x => UIBuilder.FormatText (x.Name, "(", x.Code, ")")
				});
		}

		private void CreateUICategory(UIBuilder builder)
		{
			builder.CreateAutoCompleteTextField ("Catégorie",
				new SelectionController<ArticleCategoryEntity>
				{
					ValueGetter = () => this.Entity.ArticleCategory,
					ValueSetter = x => this.Entity.ArticleCategory = x.WrapNullEntity (),
					ReferenceController = new ReferenceController (() => this.Entity.ArticleCategory, creator: this.CreateNewCategory),
					PossibleItemsGetter = () => CoreProgram.Application.Data.GetArticleCategories (),

					ToTextArrayConverter     = x => new string[] { x.Name },
					ToFormattedTextConverter = x => UIBuilder.FormatText (x.Name)
				});
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


		private void CreateUIGroup(SummaryDataItems data)
		{
			data.Add (
				new SummaryData
				{
					AutoGroup    = true,
					Name		 = "ArticleGroup",
					IconUri		 = "Data.ArticleGroup",
					Title		 = UIBuilder.FormatText ("Groupes d'articles"),
					CompactTitle = UIBuilder.FormatText ("Groupes"),
					Text		 = CollectionTemplate.DefaultEmptyText
				});

			var template = new CollectionTemplate<ArticleGroupEntity> ("ArticleGroup", data.Controller, this.DataContext);

			template.DefineText (x => UIBuilder.FormatText (x.Name));
			template.DefineCompactText (x => UIBuilder.FormatText (x.Name));

			data.Add (CollectionAccessor.Create (this.EntityGetter, x => x.ArticleGroups, template));
		}

		private void CreateUIParameters(SummaryDataItems data)
		{
			data.Add (
				new SummaryData
				{
					AutoGroup    = true,
					Name		 = "ArticleParameterDefinition",
					IconUri		 = "Data.ArticleParameter",
					Title		 = UIBuilder.FormatText ("Paramètres"),
					CompactTitle = UIBuilder.FormatText ("Paramètres"),
					Text		 = CollectionTemplate.DefaultEmptyText,
				});

			var template = new CollectionTemplate<AbstractArticleParameterDefinitionEntity> ("ArticleParameterDefinition", data.Controller, this.DataContext);

			template.DefineText (x => UIBuilder.FormatText (GetParameterSummary (x)));
			template.DefineCompactText (x => UIBuilder.FormatText (GetParameterSummary (x)));
			template.DefineCreateItem (this.CreateParameter);  // le bouton [+] crée une ligne d'article

			data.Add (CollectionAccessor.Create (this.EntityGetter, x => x.ArticleParameters, template));
		}


		private static string GetParameterSummary(AbstractArticleParameterDefinitionEntity parameter)
		{
			var builder = new System.Text.StringBuilder ();

			builder.Append (parameter.Name);
			builder.Append (": ");

			if (parameter is NumericValueArticleParameterDefinitionEntity)
			{
				var value = parameter as NumericValueArticleParameterDefinitionEntity;

				builder.Append (value.DefaultValue.ToString ());
				builder.Append (" (");
				builder.Append (value.MinValue.ToString ());
				builder.Append ("..");
				builder.Append (value.MaxValue.ToString ());
				builder.Append (")");
			}

			if (parameter is EnumValueArticleParameterDefinitionEntity)
			{
				var value = parameter as EnumValueArticleParameterDefinitionEntity;

				builder.Append (value.DefaultValue);
				builder.Append (" (");
				builder.Append (Common.EnumInternalToSingleLine (value.Values));
				builder.Append (")");
			}

			return builder.ToString ();
			;
		}

		private NumericValueArticleParameterDefinitionEntity CreateParameter()
		{
			//	Crée un nouvelle ligne dans la facture du type le plus courant, c'est-à-dire ArticleDocumentItemEntity.
			return this.DataContext.CreateEmptyEntity<NumericValueArticleParameterDefinitionEntity> ();
		}


		private TileContainer							tileContainer;
	}
}
