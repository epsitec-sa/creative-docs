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

		protected override void CreateUI(TileContainer container)
		{
			this.tileContainer = container;

			using (var builder = new UIBuilder (container, this))
			{
				builder.CreateHeaderEditorTile ();
				builder.CreateEditionTitleTile ("Data.ArticleDocumentItem", "Ligne d'article");

				this.CreateTabBook (builder);

				this.CreateUIArticleDefinition (builder);
				this.CreateUIParameter (builder);

#if false
				this.CreateUIQuantity (builder);
				this.CreateUIUnitOfMeasure (builder);

				this.CreateUIDelayedQuantity1 (builder);
				this.CreateUIDelayedUnitOfMeasure1 (builder);
#endif

#if false
				this.CreateUIDelayedQuantity2 (builder);
				this.CreateUIDelayedUnitOfMeasure2 (builder);
#endif

				this.CreateUIPrice (builder);

				builder.CreateFooterEditorTile ();
			}

			//	Summary:
			this.TileContainerController = new TileContainerController (this, container);
			var data = this.TileContainerController.DataItems;

			this.CreateUIQuantities (data);

			this.TileContainerController.GenerateTiles ();
		}


		private void CreateTabBook(UIBuilder builder)
		{
			Common.CreateAbstractDocumentItemTabBook (builder, this.tileContainer, this.DataContext, this.Entity, DocumentItemTabId.Article);
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
					ToFormattedTextConverter = x => UIBuilder.FormatText (x.IdA, x.ShortDescription),
				});
		}

		private void CreateUIParameter(UIBuilder builder)
		{
			var tile = builder.CreateEditionTile ();

			var group = builder.CreateGroup (tile, null);  // groupe sans titre
			this.parameterController = new ArticleParameterControllers.ValuesArticleParameterController (this.tileContainer, tile);
			this.parameterController.CreateUI (group);
			this.parameterController.UpdateUI (this.Entity);
		}

		private void CreateUIUnitOfMeasure(UIBuilder builder)
		{
			builder.CreateAutoCompleteTextField ("Unité",
				new SelectionController<UnitOfMeasureEntity>
				{
					ValueGetter = () => this.UnitOfMeasure,
					ValueSetter = x => this.UnitOfMeasure = x.WrapNullEntity (),
					ReferenceController = new ReferenceController (() => this.UnitOfMeasure, creator: this.CreateNewUnitOfMeasure),
					PossibleItemsGetter = () => this.GetUnitOfMeasure (),

					ToTextArrayConverter     = x => new string[] { x.Name, x.Code },
					ToFormattedTextConverter = x => UIBuilder.FormatText (x.Name, "(", x.Code, ")")
				});
		}

		private void CreateUIDelayedUnitOfMeasure1(UIBuilder builder)
		{
			builder.CreateAutoCompleteTextField ("Unité",
				new SelectionController<UnitOfMeasureEntity>
				{
					ValueGetter = () => this.DelayedUnitOfMeasure1,
					ValueSetter = x => this.DelayedUnitOfMeasure1 = x.WrapNullEntity (),
					ReferenceController = new ReferenceController (() => this.DelayedUnitOfMeasure1, creator: this.CreateNewUnitOfMeasure),
					PossibleItemsGetter = () => this.GetUnitOfMeasure (),

					ToTextArrayConverter     = x => new string[] { x.Name, x.Code },
					ToFormattedTextConverter = x => UIBuilder.FormatText (x.Name, "(", x.Code, ")")
				});
		}

		private void CreateUIDelayedUnitOfMeasure2(UIBuilder builder)
		{
			builder.CreateAutoCompleteTextField ("Unité",
				new SelectionController<UnitOfMeasureEntity>
				{
					ValueGetter = () => this.DelayedUnitOfMeasure2,
					ValueSetter = x => this.DelayedUnitOfMeasure2 = x.WrapNullEntity (),
					ReferenceController = new ReferenceController (() => this.DelayedUnitOfMeasure2, creator: this.CreateNewUnitOfMeasure),
					PossibleItemsGetter = () => this.GetUnitOfMeasure (),

					ToTextArrayConverter     = x => new string[] { x.Name, x.Code },
					ToFormattedTextConverter = x => UIBuilder.FormatText (x.Name, "(", x.Code, ")")
				});
		}

		private void CreateUIQuantity(Epsitec.Cresus.Core.UIBuilder builder)
		{
			var tile = builder.CreateEditionTile ();

			builder.CreateTextFieldMulti (tile, 80, "Désignation", Marshaler.Create (this.GetArticleDescription, this.SetArticleDescription));

			builder.CreateMargin (tile, horizontalSeparator: true);

			FrameBox group = builder.CreateGroup (tile, "Quantité livrée");
			builder.CreateTextField (group, DockStyle.Left, 60, Marshaler.Create (this.GetQuantity, this.SetQuantity));
		}

		private void CreateUIDelayedQuantity1(Epsitec.Cresus.Core.UIBuilder builder)
		{
			var tile = builder.CreateEditionTile ();

			builder.CreateMargin (tile, horizontalSeparator: true);

			FrameBox group = builder.CreateGroup (tile, "Quantité livrée ultérieurement et date");
			builder.CreateTextField (group, DockStyle.Left, 60, Marshaler.Create (this.GetDelayedQuantity1, this.SetDelayedQuantity1));
			builder.CreateTextField (group, DockStyle.Fill,  0, Marshaler.Create (this.GetDelayedDate1, this.SetDelayedDate1));
		}

		private void CreateUIDelayedQuantity2(Epsitec.Cresus.Core.UIBuilder builder)
		{
			var tile = builder.CreateEditionTile ();

			builder.CreateMargin (tile, horizontalSeparator: true);

			FrameBox group = builder.CreateGroup (tile, "Deuxième quantité livrée ultérieurement et date");
			builder.CreateTextField (group, DockStyle.Left, 60, Marshaler.Create (this.GetDelayedQuantity2, this.SetDelayedQuantity2));
			builder.CreateTextField (group, DockStyle.Fill,  0, Marshaler.Create (this.GetDelayedDate2, this.SetDelayedDate2));
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
					Title		 = UIBuilder.FormatText ("Quantités"),
					CompactTitle = UIBuilder.FormatText ("Quantités"),
					Text		 = CollectionTemplate.DefaultEmptyText,
				});

			var template = new CollectionTemplate<ArticleQuantityEntity> ("ArticleQuantities", data.Controller, this.DataContext);

			template.DefineText        (x => UIBuilder.FormatText (GetArticleQuantitySummary (x)));
			template.DefineCompactText (x => UIBuilder.FormatText (GetArticleQuantitySummary (x)));
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


		private IEnumerable<UnitOfMeasureEntity> GetUnitOfMeasure()
		{
			//	Retourne les unités appartenant au même groupe que l'article.
#if false
			//	Version normale :
			var fullList = CoreProgram.Application.Data.GetUnitOfMeasure ();
			var groupList = new List<UnitOfMeasureEntity> ();

			foreach (var uom in fullList)
			{
				if (uom.Category == this.Entity.ArticleDefinition.Units.Category)
				{
					groupList.Add (uom);
				}
			}

			return groupList;
#else
			//	With lambda expression and query operator :
			return CoreProgram.Application.Data.GetUnitOfMeasure ().Where (x => x.Category == this.Entity.ArticleDefinition.Units.Category);
#endif
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

					//?this.SetQuantity (1);
					this.UnitOfMeasure = value.BillingUnit;
					this.SetPrice (this.GetArticlePrice ());

					this.Entity.ReplacementText = null;
					this.SetArticleDescription (this.GetArticleDescription ());

					this.parameterController.UpdateUI (this.Entity);
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

		private UnitOfMeasureEntity DelayedUnitOfMeasure1
		{
			get
			{
				return this.GetUnitOfMeasure (BusinessLogic.ArticleQuantityType.Delayed, 0);
			}
			set
			{
				this.SetUnitOfMeasure (BusinessLogic.ArticleQuantityType.Delayed, 0, value);
			}
		}

		private UnitOfMeasureEntity DelayedUnitOfMeasure2
		{
			get
			{
				return this.GetUnitOfMeasure (BusinessLogic.ArticleQuantityType.Delayed, 1);
			}
			set
			{
				this.SetUnitOfMeasure (BusinessLogic.ArticleQuantityType.Delayed, 1, value);
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


		private decimal? GetQuantity()
		{
			return this.GetQuantity (BusinessLogic.ArticleQuantityType.Billed, 0);
		}


		private void SetQuantity(decimal? value)
		{
			this.SetQuantity (BusinessLogic.ArticleQuantityType.Billed, 0, value);
		}


		private decimal? GetDelayedQuantity1()
		{
			return this.GetQuantity (BusinessLogic.ArticleQuantityType.Delayed, 0);
		}


		private void SetDelayedQuantity1(decimal? value)
		{
			this.SetQuantity (BusinessLogic.ArticleQuantityType.Delayed, 0, value);
		}


		private decimal? GetDelayedQuantity2()
		{
			return this.GetQuantity (BusinessLogic.ArticleQuantityType.Delayed, 1);
		}


		private void SetDelayedQuantity2(decimal? value)
		{
			this.SetQuantity (BusinessLogic.ArticleQuantityType.Delayed, 1, value);
		}


		private Date? GetDelayedDate1()
		{
			return this.GetDate (BusinessLogic.ArticleQuantityType.Delayed, 0);
		}

		private void SetDelayedDate1(Date? value)
		{
			this.SetDate (BusinessLogic.ArticleQuantityType.Delayed, 0, value);
		}


		private Date? GetDelayedDate2()
		{
			return this.GetDate (BusinessLogic.ArticleQuantityType.Delayed, 1);
		}

		private void SetDelayedDate2(Date? value)
		{
			this.SetDate (BusinessLogic.ArticleQuantityType.Delayed, 1, value);
		}


		private decimal? GetQuantity(BusinessLogic.ArticleQuantityType quantityType, int rank)
		{
			foreach (var quantity in this.Entity.ArticleQuantities)
			{
				if (quantity.QuantityType == quantityType && rank-- == 0)
				{
					if (quantity.Quantity == 0)
					{
						return null;
					}
					else
					{
						return quantity.Quantity;
					}
				}
			}

			return null;
		}

		private void SetQuantity(BusinessLogic.ArticleQuantityType quantityType, int rank, decimal? value)
		{
			for (int i = 0; i < this.Entity.ArticleQuantities.Count; i++)
			{
				var quantity = this.Entity.ArticleQuantities[i];

				if (quantity.QuantityType == quantityType && rank-- == 0)
				{
					if (value.HasValue)
					{
						quantity.Quantity = value.Value;
					}
					else
					{
						quantity.Quantity = 0;
					}

					if (IsEmpty (quantity))
					{
						this.Entity.ArticleQuantities.RemoveAt (i);
					}

					this.UpdatePrices ();
					return;
				}
			}

			if (value.HasValue)
			{
				var newQuantity = this.DataContext.CreateEntity<ArticleQuantityEntity> ();

				newQuantity.QuantityType = quantityType;
				newQuantity.Quantity     = value.Value;
				newQuantity.Unit         = this.Entity.ArticleDefinition.BillingUnit;

				this.Entity.ArticleQuantities.Add (newQuantity);
			}

			this.UpdatePrices ();
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

						this.tileContainer.UpdateAllWidgets ();
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

				this.tileContainer.UpdateAllWidgets ();
			}
		}


		private Date? GetDate(BusinessLogic.ArticleQuantityType quantityType, int rank)
		{
			foreach (var quantity in this.Entity.ArticleQuantities)
			{
				if (quantity.QuantityType == quantityType && rank-- == 0)
				{
					return quantity.ExpectedDate;
				}
			}

			return null;
		}

		private void SetDate(BusinessLogic.ArticleQuantityType quantityType, int rank, Date? value)
		{
			for (int i = 0; i < this.Entity.ArticleQuantities.Count; i++)
			{
				var quantity = this.Entity.ArticleQuantities[i];

				if (quantity.QuantityType == quantityType && rank-- == 0)
				{
					quantity.ExpectedDate = value;

					if (IsEmpty (quantity))
					{
						this.Entity.ArticleQuantities.RemoveAt (i);

						this.tileContainer.UpdateAllWidgets ();
					}

					return;
				}
			}

			var newQuantity = this.DataContext.CreateEntity<ArticleQuantityEntity> ();

			newQuantity.QuantityType = quantityType;
			newQuantity.Quantity     = 1;
			newQuantity.Unit         = this.Entity.ArticleDefinition.BillingUnit;
			newQuantity.ExpectedDate = value;

			this.Entity.ArticleQuantities.Add (newQuantity);

			this.tileContainer.UpdateAllWidgets ();
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
			var invoiceDocument = Common.GetParentEntity (this.tileContainer) as InvoiceDocumentEntity;

			if (invoiceDocument != null)
			{
				InvoiceDocumentHelper.UpdatePrices (invoiceDocument, this.DataContext);
			}

			this.tileContainer.UpdateAllWidgets ();
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


		private TileContainer													tileContainer;
		private ArticleParameterControllers.ValuesArticleParameterController	parameterController;
	}
}
