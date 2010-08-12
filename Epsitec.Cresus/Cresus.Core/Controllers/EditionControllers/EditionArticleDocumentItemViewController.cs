//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
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

				this.CreateUIArticleDefinition (builder);
				this.CreateUIParameter         (builder);
				this.CreateUIDesignation       (builder);
				this.CreateUIPrice             (builder);

				builder.CreateFooterEditorTile ();
			}

			//	Summary:
			using (var data = TileContainerController.Setup (this))
			{
				this.CreateUIQuantities (data);
			}
		}


		private void CreateTabBook(UIBuilder builder)
		{
			Common.CreateDocumentItemTabBook (builder, this, DocumentItemTabId.Article);
		}


		private void CreateUIArticleDefinition(UIBuilder builder)
		{
			var textField = builder.CreateAutoCompleteTextField ("Article à facturer",
				new SelectionController<ArticleDefinitionEntity>
				{
					ValueGetter = () => this.ArticleDefinition,
					ValueSetter = x => this.ArticleDefinition = x.WrapNullEntity (),
					ReferenceController = new ReferenceController (() => this.ArticleDefinition, creator: this.CreateNewArticleDefinition),
					PossibleItemsGetter = () => CoreProgram.Application.Data.GetArticleDefinitions (this.DataContext),

					ToTextArrayConverter     = x => new string[] { x.IdA, x.ShortDescription },
					ToFormattedTextConverter = x => TextFormatter.FormatText (x.IdA, x.ShortDescription),
				});
		}

		private void CreateUIParameter(UIBuilder builder)
		{
			var tile = builder.CreateEditionTile ();

			var group = builder.CreateGroup (tile, null);  // groupe sans titre
			this.parameterController = new ArticleParameterControllers.ValuesArticleParameterController (this.TileContainer, tile);
			this.parameterController.CreateUI (group);
			this.parameterController.UpdateUI (this.Entity);
		}

		private void CreateUIDesignation(UIBuilder builder)
		{
			var tile = builder.CreateEditionTile ();

			this.toolbarController = new ArticleParameterControllers.ArticleParameterToolbarController (this.TileContainer);
			this.toolbarController.CreateUI (tile.Container, "Désignation");

			this.designationTextField = builder.CreateTextFieldMulti (tile, 80, null, Marshaler.Create (this.GetArticleDescription, this.SetArticleDescription));

			this.toolbarController.UpdateUI (this.Entity.ArticleDefinition, this.designationTextField);
		}

		private void CreateUIPrice(Epsitec.Cresus.Core.UIBuilder builder)
		{
			var tile = builder.CreateEditionTile ();

			builder.CreateMargin (tile, horizontalSeparator: true);

			builder.CreateTextField (tile, 80, "Rabais (pourcent ou montant)", Marshaler.Create (this.GetDiscount, this.SetDiscount));

			FrameBox group = builder.CreateGroup (tile, "Prix unitaire et total HT");
			        builder.CreateTextField (group, DockStyle.Left, 80, Marshaler.Create (this.GetPrice,      this.SetPrice));
			var t = builder.CreateTextField (group, DockStyle.Left, 80, Marshaler.Create (this.GetTotalPrice, this.SetTotalPrice));
			t.IsReadOnly = true;
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

			template.DefineText        (x => TextFormatter.FormatText (GetArticleQuantitySummary (x)));
			template.DefineCompactText (x => TextFormatter.FormatText (GetArticleQuantitySummary (x)));
			template.DefineSetupItem   (SetupArticleQuantity);

			data.Add (CollectionAccessor.Create (this.EntityGetter, x => x.ArticleQuantities, template));
		}

		private static string GetArticleQuantitySummary(ArticleQuantityEntity quantity)
		{
			string type = null;
			foreach (var q in BusinessLogic.Enumerations.GetAllPossibleValueArticleQuantityType ())
			{
				if (q.Key == quantity.QuantityType)
				{
					type = q.Values[0];
					break;
				}
			}

			string unit = Misc.FormatUnit (quantity.Quantity, quantity.Unit.Code);

			return string.Concat (type, " ", unit);
		}

		private static void SetupArticleQuantity(ArticleQuantityEntity quantity)
		{
			quantity.QuantityType = BusinessLogic.ArticleQuantityType.Billed;
			quantity.Quantity = 1;
		}


		private ArticleDefinitionEntity ArticleDefinition
		{
			get
			{
				return this.Entity.ArticleDefinition;
			}
			set
			{
				if (this.Entity.ArticleDefinition.CompareWith (value) == false)
				{
					this.Entity.ArticleDefinition = value;

					this.UnitOfMeasure = value.BillingUnit;
					this.SetPrice (this.GetArticlePrice ());

					this.Entity.ReplacementText = null;
					this.SetArticleDescription (this.GetArticleDescription ());

					this.parameterController.UpdateUI (this.Entity);
					this.toolbarController.UpdateUI (this.Entity.ArticleDefinition, this.designationTextField);
				}
			}
		}

		private UnitOfMeasureEntity UnitOfMeasure
		{
			get
			{
				return this.GetUnitOfMeasure (BusinessLogic.ArticleQuantityType.Billed, 0);
			}
			set
			{
				this.SetUnitOfMeasure (BusinessLogic.ArticleQuantityType.Billed, 0, value);
			}
		}


		private string GetArticleDescription()
		{
			return ArticleDocumentItemHelper.GetArticleDescription (this.Entity);
		}

		private void SetArticleDescription(string value)
		{
			if (this.Entity.ReplacementText != value)
			{
				this.Entity.ReplacementText = value;
			}
		}


		private UnitOfMeasureEntity GetUnitOfMeasure(BusinessLogic.ArticleQuantityType quantityType, int rank)
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

		private void SetUnitOfMeasure(BusinessLogic.ArticleQuantityType quantityType, int rank, UnitOfMeasureEntity value)
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

				if (this.Entity.Discounts[0].DiscountAmount.HasValue)
				{
					return Misc.PriceToString (this.Entity.Discounts[0].DiscountAmount.Value);
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
					this.Entity.Discounts[0].DiscountAmount = null;
				}
				else
				{
					this.Entity.Discounts[0].DiscountRate = null;
					this.Entity.Discounts[0].DiscountAmount = v;
				}
			}

			this.UpdatePrices ();
		}


		private decimal? GetArticlePrice()
		{
			return ArticleDocumentItemHelper.GetArticlePrice (this.Entity, System.DateTime.Now, BusinessLogic.Finance.CurrencyCode.Chf);
		}

		private decimal? GetPrice()
		{
			return this.Entity.PrimaryUnitPriceBeforeTax;
		}

		private void SetPrice(decimal? value)
		{
			if (value.HasValue)
			{
				this.Entity.PrimaryUnitPriceBeforeTax = value.Value;
				this.UpdatePrices ();
			}
		}

		private decimal? GetTotalPrice()
		{
			return this.Entity.ResultingLinePriceBeforeTax;
		}

		private void SetTotalPrice(decimal? value)
		{
			if (value.HasValue)
			{
				this.Entity.ResultingLinePriceBeforeTax = value.Value;
			}
		}

		private void UpdatePrices()
		{
			var invoiceDocument = Common.GetParentEntity (this.TileContainer) as InvoiceDocumentEntity;

			if (invoiceDocument != null)
			{
				InvoiceDocumentHelper.UpdatePrices (invoiceDocument, this.DataContext);
			}

			this.TileContainer.UpdateAllWidgets ();
		}


		private static bool IsEmpty(ArticleQuantityEntity quantity)
		{
			return (quantity.Quantity == 0 &&
					string.IsNullOrEmpty (quantity.Unit.Code));
		}



		private NewEntityReference CreateNewArticleDefinition(DataContext context)
		{
			var title = context.CreateEmptyEntity<ArticleDefinitionEntity> ();
			return title;
		}

		private NewEntityReference CreateNewUnitOfMeasure(DataContext context)
		{
			var title = context.CreateEmptyEntity<UnitOfMeasureEntity> ();
			return title;
		}


		protected override EditionStatus GetEditionStatus()
		{
			return EditionStatus.Valid;
		}

		private ArticleParameterControllers.ValuesArticleParameterController	parameterController;
		private ArticleParameterControllers.ArticleParameterToolbarController	toolbarController;
		private TextFieldMultiEx												designationTextField;
	}
}
