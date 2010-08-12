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

				this.CreateUIMain (builder);
				this.CreateUIUnitOfMeasure (builder);
				this.CreateUICategory (builder);

				builder.CreateFooterEditorTile ();
			}

			//	Summary:
			using (var data = TileContainerController.Setup (this))
			{
				this.CreateUIGroup (data);
				this.CreateUIParameters (data);
			}
		}


		private void CreateUIMain(UIBuilder builder)
		{
			var tile = builder.CreateEditionTile ();

			FrameBox group = builder.CreateGroup (tile, "N° de l'article (principal, externe et interne)");
			builder.CreateTextField (group, DockStyle.Left, 74, Marshaler.Create (() => this.Entity.IdA, x => this.Entity.IdA = x));
			builder.CreateTextField (group, DockStyle.Left, 74, Marshaler.Create (() => this.Entity.IdB, x => this.Entity.IdB = x));
			builder.CreateTextField (group, DockStyle.Left, 74, Marshaler.Create (() => this.Entity.IdC, x => this.Entity.IdC = x));

			builder.CreateMargin (tile, horizontalSeparator: true);

			builder.CreateTextField (tile,  0, "Description courte", Marshaler.Create (() => this.Entity.ShortDescription, x => this.Entity.ShortDescription = x));

			var toolbarController = new ArticleParameterControllers.ArticleParameterToolbarController (this.TileContainer);
			toolbarController.CreateUI (tile.Container, "Description longue");
			var textField = builder.CreateTextFieldMulti (tile, 68, null, Marshaler.Create (() => this.Entity.LongDescription, x => this.Entity.LongDescription = x));
			toolbarController.UpdateUI (this.Entity, textField);

			builder.CreateMargin (tile, horizontalSeparator: true);

			builder.CreateAutoCompleteTextField (tile, 0, "Mode de TVA", Marshaler.Create (this.Entity, x => x.OutputVatCode, (x, v) => x.OutputVatCode = v), BusinessLogic.Enumerations.GetAllPossibleVatCodes (), x => TextFormatter.FormatText (x.Values[0], "-", x.Values[1]));
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
					ToFormattedTextConverter = x => TextFormatter.FormatText (x.Name, "(", x.Code, ")")
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
					ToFormattedTextConverter = x => TextFormatter.FormatText (x.Name)
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
					Title		 = TextFormatter.FormatText ("Groupes d'articles"),
					CompactTitle = TextFormatter.FormatText ("Groupes"),
					Text		 = CollectionTemplate.DefaultEmptyText
				});

			var template = new CollectionTemplate<ArticleGroupEntity> ("ArticleGroup", data.Controller, this.DataContext);

			template.DefineText (x => TextFormatter.FormatText (x.Name));
			template.DefineCompactText (x => TextFormatter.FormatText (x.Name));

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
					Title		 = TextFormatter.FormatText ("Paramètres"),
					CompactTitle = TextFormatter.FormatText ("Paramètres"),
					Text		 = CollectionTemplate.DefaultEmptyText,
				});

			var template = new CollectionTemplate<AbstractArticleParameterDefinitionEntity> ("ArticleParameterDefinition", data.Controller, this.DataContext);

			template.DefineText (x => TextFormatter.FormatText (GetParameterSummary (x)));
			template.DefineCompactText (x => TextFormatter.FormatText (GetParameterSummary (x)));
			template.DefineCreateItem (this.CreateParameter);  // le bouton [+] crée une ligne d'article

			data.Add (CollectionAccessor.Create (this.EntityGetter, x => x.ArticleParameterDefinitions, template));
		}


		private static string GetParameterSummary(AbstractArticleParameterDefinitionEntity parameter)
		{
			var builder = new System.Text.StringBuilder ();

			if (!string.IsNullOrEmpty (parameter.Name))
			{
				builder.Append (parameter.Name);
				builder.Append (": ");
			}

			if (parameter is NumericValueArticleParameterDefinitionEntity)
			{
				var value = parameter as NumericValueArticleParameterDefinitionEntity;

				if (value.DefaultValue.HasValue ||
					value.MinValue.HasValue     ||
					value.MaxValue.HasValue     )
				{
					builder.Append (value.DefaultValue.ToString ());
					builder.Append (" (");
					builder.Append (value.MinValue.ToString ());
					builder.Append ("..");
					builder.Append (value.MaxValue.ToString ());
					builder.Append (")");
				}
				else
				{
					builder.Append ("<i>Vide</i>");
				}
			}

			if (parameter is EnumValueArticleParameterDefinitionEntity)
			{
				var value = parameter as EnumValueArticleParameterDefinitionEntity;

				if (!string.IsNullOrWhiteSpace (value.DefaultValue) ||
					!string.IsNullOrWhiteSpace (value.Values)       )
				{
					builder.Append (value.DefaultValue);
					builder.Append (" (");
					builder.Append (Common.EnumInternalToSingleLine (value.Values));
					builder.Append (")");
				}
				else
				{
					builder.Append ("<i>Vide</i>");
				}
			}

			return builder.ToString ();
			;
		}

		private NumericValueArticleParameterDefinitionEntity CreateParameter()
		{
			//	Crée un nouvelle ligne dans la facture du type le plus courant, c'est-à-dire ArticleDocumentItemEntity.
			return this.DataContext.CreateEmptyEntity<NumericValueArticleParameterDefinitionEntity> ();
		}
	}
}
