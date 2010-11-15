﻿//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;
using Epsitec.Common.Types.Converters;
using Epsitec.Common.Widgets;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.TabIds;
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
	public class EditionArticleDocumentItemViewController : EditionViewController<Entities.ArticleDocumentItemEntity>
	{
		public EditionArticleDocumentItemViewController(string name, Entities.ArticleDocumentItemEntity entity)
			: base (name, entity)
		{
		}

		protected override void CreateUI()
		{
			using (var builder = new UIBuilder (this))
			{
				builder.CreateHeaderEditorTile ();
				builder.CreateEditionTitleTile ("Data.ArticleDocumentItem", "Ligne d'article");

				this.CreateTabBook (builder);

				this.CreateUIArticleDefinition  (builder);
				this.CreateUIParameter          (builder);
				this.CreateUIArticleDescription (builder);
				this.CreateUIPrice              (builder);

				using (var data = TileContainerController.Setup (this))
				{
					this.CreateUIQuantities (data);
				}
				
				builder.CreateFooterEditorTile ();
			}

//-			this.DataContext.EntityChanged += new Epsitec.Common.Support.EventHandler<EntityChangedEventArgs> (this.HandleDataContextEntityChanged);
		}


		private void CreateTabBook(UIBuilder builder)
		{
			Common.CreateDocumentItemTabBook (builder, this, DocumentItemTabId.Article);
		}


		private void CreateUIArticleDefinition(UIBuilder builder)
		{
			var controller = new SelectionController<ArticleDefinitionEntity> (this.BusinessContext)
			{
				ValueGetter         = () => this.Entity.ArticleDefinition,
				ValueSetter         = x => this.SetArticleDefinition (x),
				ReferenceController = new ReferenceController (() => this.Entity.ArticleDefinition, creator: this.CreateNewArticleDefinition),
			};

			builder.CreateAutoCompleteTextField ("Article", controller);
		}

		private void CreateUIParameter(UIBuilder builder)
		{
			var tile = builder.CreateEditionTile ();
			var group = builder.CreateGroup (tile);

			this.parameterController = new ArticleParameterControllers.ValuesArticleParameterController (this.TileContainer, tile);
			this.parameterController.CallbackParameterChanged = this.ParameterChanged;
			this.parameterController.CreateUI (group);
			this.parameterController.UpdateUI (this.Entity);
		}

		private void CreateUIArticleDescription(UIBuilder builder)
		{
			var tile = builder.CreateEditionTile ();

			this.toolbarController = new ArticleParameterControllers.ArticleParameterToolbarController (this.TileContainer);
			this.toolbarController.CreateUI (tile.Container, "Désignation");

			this.articleDescriptionTextField = builder.CreateTextFieldMulti (tile, 80, null, Marshaler.Create (() => this.GetArticleDescription (), this.SetArticleDescription));

			this.toolbarController.UpdateUI (this.Entity, this.articleDescriptionTextField);
		}

		private void CreateUIPrice(Epsitec.Cresus.Core.UIBuilder builder)
		{
			var tile = builder.CreateEditionTile ();

			builder.CreateMargin (tile, horizontalSeparator: true);

			builder.CreateTextField (tile, 80, "Rabais (pourcent ou montant)", Marshaler.Create (() => this.GetDiscount (), this.SetDiscount));

			FrameBox group1 = builder.CreateGroup (tile, "Prix unitaire HT et total HT avant rabais");
			builder.CreateTextField (group1, DockStyle.Left, 80, Marshaler.Create (() => this.Entity.PrimaryUnitPriceBeforeTax));
			builder.CreateTextField (group1, DockStyle.Left, 80, Marshaler.Create (() => this.Entity.PrimaryLinePriceBeforeTax));

			FrameBox group2 = builder.CreateGroup (tile, "Prix unitaire TTC et total TTC avant rabais");
			builder.CreateTextField (group2, DockStyle.Left, 80, Marshaler.Create (() => this.Entity.PrimaryUnitPriceAfterTax));
			builder.CreateTextField (group2, DockStyle.Left, 80, Marshaler.Create (() => this.Entity.PrimaryLinePriceAfterTax));

			FrameBox group3 = builder.CreateGroup (tile, "Prix unitaire TTC et total TTC après rabais");
			builder.CreateTextField (group3, DockStyle.Left, 80, Marshaler.Create (() => this.Entity.ResultingUnitPriceAfterTax));
			builder.CreateTextField (group3, DockStyle.Left, 80, Marshaler.Create (() => this.Entity.ResultingLinePriceAfterTax));
		}

		private void CreateUIQuantities(SummaryDataItems data)
		{
			data.Add (
				new SummaryData
				{
					AutoGroup    = true,
					Name		 = "ArticleQuantities",
					IconUri		 = "Data.ArticleQuantity",
					Title		 = TextFormatter.FormatText ("Quantités"),
					CompactTitle = TextFormatter.FormatText ("Quantités"),
					Text		 = CollectionTemplate.DefaultEmptyText,
				});

			var template = new CollectionTemplate<ArticleQuantityEntity> ("ArticleQuantities", data.Controller, this.DataContext);

			template.DefineText        (x => x.GetCompactSummary ());
			template.DefineCompactText (x => x.GetCompactSummary ());
			template.DefineSetupItem   (EditionArticleDocumentItemViewController.SetupArticleQuantity);

			data.Add (this.CreateCollectionAccessor (template, x => x.ArticleQuantities));
		}

		private static void SetupArticleQuantity(ArticleQuantityEntity quantity)
		{
			quantity.QuantityType = Business.ArticleQuantityType.Billed;
			quantity.Quantity = 1;
		}


		private void SetArticleDefinition(ArticleDefinitionEntity value)
		{
			if (this.Entity.ArticleDefinition.RefEquals (value))
			{
				return;
			}

			var item = this.Entity;

			item.ArticleDefinition = value;
			item.ArticleParameters = null;
			item.ArticleTraceabilityDetails.Clear ();
			item.ArticleQuantities.Clear ();
			item.VatCode = value.GetOutputVatCode ();
			item.NeverApplyDiscount = false;
			item.TaxRate1 = null;
			item.TaxRate2 = null;
			item.FixedLinePrice = null;
			item.FixedLinePriceIncludesTaxes = false;
			item.ResultingLinePriceBeforeTax = null;
			item.ResultingLineTax1 = null;
			item.ResultingLineTax2 = null;
			item.FinalLinePriceBeforeTax = null;
			item.ArticleShortDescriptionCache = null;
			item.ArticleLongDescriptionCache = null;
			item.ReplacementText = null;

			this.Entity.ArticleParameters = null;

			this.UnitOfMeasure = value.BillingUnit;
//-			this.SetPrice (this.GetArticlePrice ());

			this.Entity.ReplacementText = null;
			this.SetArticleDescription (this.GetArticleDescription ());

			this.parameterController.UpdateUI (this.Entity);
			this.toolbarController.UpdateUI (this.Entity, this.articleDescriptionTextField);

			this.TileContainer.UpdateAllWidgets ();
		}

		private UnitOfMeasureEntity UnitOfMeasure
		{
			get
			{
				return this.GetUnitOfMeasure (Business.ArticleQuantityType.Billed, 0);
			}
			set
			{
				this.SetUnitOfMeasure (Business.ArticleQuantityType.Billed, 0, value);
			}
		}


		private FormattedText GetArticleDescription()
		{
			return ArticleDocumentItemHelper.GetArticleDescription (this.Entity);
		}

		private void SetArticleDescription(FormattedText value)
		{
			//	The replacement text of the article item might be defined in several different
			//	languages; compare and replace only the text for the active language :

			string articleDescription = value.IsNull ? null : TextFormatter.ConvertToText (value);
			string defaultDescription = TextFormatter.ConvertToText (this.Entity.ArticleDefinition.LongDescription);
			string currentReplacement = this.Entity.ReplacementText.IsNull ? null : TextFormatter.ConvertToText (this.Entity.ReplacementText);
			
			if (articleDescription == defaultDescription)  // description standard ?
			{
				articleDescription = null;
			}

			if (currentReplacement != articleDescription)
			{
				MultilingualText text = new MultilingualText (this.Entity.ReplacementText);
				text.SetText (TextFormatter.CurrentLanguageId, articleDescription);
				this.Entity.ReplacementText = text.GetGlobalText ();
			}
		}


		private UnitOfMeasureEntity GetUnitOfMeasure(Business.ArticleQuantityType quantityType, int rank)
		{
			foreach (var quantity in this.Entity.ArticleQuantities)
			{
				if (quantity.QuantityType == quantityType && rank-- == 0)
				{
					return quantity.Unit;
				}
			}

			return null;
		}

		private void SetUnitOfMeasure(Business.ArticleQuantityType quantityType, int rank, UnitOfMeasureEntity value)
		{
			for (int i = 0; i < this.Entity.ArticleQuantities.Count; i++)
			{
				var quantity = this.Entity.ArticleQuantities[i];

				if (quantity.QuantityType == quantityType && rank-- == 0)
				{
					quantity.Unit = value;

					if (IsEmpty (quantity))
					{
						this.Entity.ArticleQuantities.RemoveAt (i);

						this.TileContainer.UpdateAllWidgets ();
					}

					return;
				}
			}

			if (!string.IsNullOrEmpty (value.Code))
			{
				var newQuantity = this.DataContext.CreateEntity<ArticleQuantityEntity> ();

				newQuantity.QuantityType = quantityType;
				newQuantity.Quantity     = 1;
				newQuantity.Unit         = value;

				this.Entity.ArticleQuantities.Add (newQuantity);

				this.TileContainer.UpdateAllWidgets ();
			}
		}


		private string GetDiscount()
		{
			if (this.Entity.Discounts.Count != 0)
			{
				if (this.Entity.Discounts[0].DiscountRate.HasValue)
				{
					return Misc.PercentToString (this.Entity.Discounts[0].DiscountRate.Value);
				}

				if (this.Entity.Discounts[0].Value.HasValue)
				{
					return Misc.PriceToString (this.Entity.Discounts[0].Value.Value);
				}
			}

			return null;
		}

		private void SetDiscount(string value)
		{
			value = value.Trim ();

			decimal? v = null;
			bool percent = false;

			if (value.EndsWith ("%"))
			{
				v = Misc.StringToDecimal (value.Substring (0, value.Length-1)) / 100.0M;
				percent = true;
			}
			else
			{
				v = Misc.StringToDecimal (value);
			}

			if (v == null)
			{
				this.Entity.Discounts.Clear ();
			}
			else
			{
				if (this.Entity.Discounts.Count == 0)
				{
					var discount = this.DataContext.CreateEntity<DiscountEntity> ();
					this.Entity.Discounts.Add (discount);
				}

				if (percent)
				{
					this.Entity.Discounts[0].DiscountRate = v;
					this.Entity.Discounts[0].Value = null;
				}
				else
				{
					this.Entity.Discounts[0].DiscountRate = null;
					this.Entity.Discounts[0].Value = v;
				}
			}
		}

		private static bool IsEmpty(ArticleQuantityEntity quantity)
		{
			return (quantity.Quantity == 0 &&
					string.IsNullOrEmpty (quantity.Unit.Code));
		}



		private NewEntityReference CreateNewArticleDefinition(DataContext context)
		{
			var article = context.CreateEntityAndRegisterAsEmpty<ArticleDefinitionEntity> ();
			return article;
		}

		private NewEntityReference CreateNewUnitOfMeasure(DataContext context)
		{
			var unit = context.CreateEntityAndRegisterAsEmpty<UnitOfMeasureEntity> ();
			return unit;
		}


		private void ParameterChanged(AbstractArticleParameterDefinitionEntity parameterDefinitionEntity)
		{
			//	Cette méthode est appelée lorsqu'un paramètre a été changé.
			ArticleParameterControllers.ArticleParameterToolbarController.UpdateTextFieldParameter (this.Entity, this.articleDescriptionTextField);
		}

		private void HandleDataContextEntityChanged(object sender, EntityChangedEventArgs e)
		{
			//?System.Diagnostics.Debug.WriteLine (string.Format ("HandleDataContextEntityChanged {0}", e.Entity.GetType()));

			if (e.Entity is ArticleDefinitionEntity ||
				e.Entity is NumericValueArticleParameterDefinitionEntity ||
				e.Entity is EnumValueArticleParameterDefinitionEntity ||
				e.Entity is FreeTextValueArticleParameterDefinitionEntity)
			{
				this.parameterController.UpdateUI (this.Entity);
				this.toolbarController.UpdateUI (this.Entity, this.articleDescriptionTextField);
			}
		}


	
		private ArticleParameterControllers.ValuesArticleParameterController	parameterController;
		private ArticleParameterControllers.ArticleParameterToolbarController	toolbarController;
		private TextFieldMultiEx												articleDescriptionTextField;
	}
}
