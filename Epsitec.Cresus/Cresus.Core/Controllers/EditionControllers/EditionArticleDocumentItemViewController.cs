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

using Epsitec.Cresus.DataLayer;

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

				this.CreateUIArticleDefinition (builder);

				this.CreateUIQuantity (builder);
				this.CreateUIUnitOfMeasure (builder);

				this.CreateUIDelayedQuantity1 (builder);
				this.CreateUIDelayedUnitOfMeasure1 (builder);

#if false
				this.CreateUIDelayedQuantity2 (builder);
				this.CreateUIDelayedUnitOfMeasure2 (builder);
#endif

				this.CreateUIPrice (builder);

				builder.CreateFooterEditorTile ();
			}
		}


		private void CreateUIArticleDefinition(UIBuilder builder)
		{
			var textField = builder.CreateAutoCompleteTextField ("Article à facturer",
				new SelectionController<ArticleDefinitionEntity>
				{
					ValueGetter = () => this.ArticleDefinition,
					ValueSetter = x => this.ArticleDefinition = x.WrapNullEntity (),
					ReferenceController = new ReferenceController (() => this.ArticleDefinition, creator: this.CreateNewArticleDefinition),
					PossibleItemsGetter = () => CoreProgram.Application.Data.GetArticleDefinitions (),

					ToTextArrayConverter     = x => new string[] { x.IdA, x.ShortDescription },
					ToFormattedTextConverter = x => UIBuilder.FormatText (x.IdA, x.ShortDescription),
				});
		}

		private void CreateUIUnitOfMeasure(UIBuilder builder)
		{
			builder.CreateAutoCompleteTextField ("Unité",
				new SelectionController<UnitOfMeasureEntity>
				{
					ValueGetter = () => this.UnitOfMeasure,
					ValueSetter = x => this.UnitOfMeasure = x.WrapNullEntity (),
					ReferenceController = new ReferenceController (() => this.UnitOfMeasure, creator: this.CreateNewUnitOfMeasure),
					PossibleItemsGetter = () => CoreProgram.Application.Data.GetUnitOfMeasure (),

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
					PossibleItemsGetter = () => CoreProgram.Application.Data.GetUnitOfMeasure (),

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
					PossibleItemsGetter = () => CoreProgram.Application.Data.GetUnitOfMeasure (),

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
				}
			}
		}

		private UnitOfMeasureEntity UnitOfMeasure
		{
			get
			{
				return this.GetUnitOfMeasure ("livré", 0);
			}
			set
			{
				this.SetUnitOfMeasure ("livré", 0, value);
			}
		}

		private UnitOfMeasureEntity DelayedUnitOfMeasure1
		{
			get
			{
				return this.GetUnitOfMeasure ("suivra", 0);
			}
			set
			{
				this.SetUnitOfMeasure ("suivra", 0, value);
			}
		}

		private UnitOfMeasureEntity DelayedUnitOfMeasure2
		{
			get
			{
				return this.GetUnitOfMeasure ("suivra", 1);
			}
			set
			{
				this.SetUnitOfMeasure ("suivra", 1, value);
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
				this.UpdateDialogs ();
			}
		}


		private decimal? GetQuantity()
		{
			return this.GetQuantity ("livré", 0);
		}


		private void SetQuantity(decimal? value)
		{
			this.SetQuantity ("livré", 0, value);
		}


		private decimal? GetDelayedQuantity1()
		{
			return this.GetQuantity ("suivra", 0);
		}


		private void SetDelayedQuantity1(decimal? value)
		{
			this.SetQuantity ("suivra", 0, value);
		}


		private decimal? GetDelayedQuantity2()
		{
			return this.GetQuantity ("suivra", 1);
		}


		private void SetDelayedQuantity2(decimal? value)
		{
			this.SetQuantity ("suivra", 1, value);
		}


		private Date? GetDelayedDate1()
		{
			return this.GetDate ("suivra", 0);
		}

		private void SetDelayedDate1(Date? value)
		{
			this.SetDate ("suivra", 0, value);
		}


		private Date? GetDelayedDate2()
		{
			return this.GetDate ("suivra", 1);
		}

		private void SetDelayedDate2(Date? value)
		{
			this.SetDate ("suivra", 1, value);
		}


		private decimal? GetQuantity(string code, int rank)
		{
			foreach (var quantity in this.Entity.ArticleQuantities)
			{
				if (quantity.Code == code && rank-- == 0)
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

		private void SetQuantity(string code, int rank, decimal? value)
		{
			for (int i = 0; i < this.Entity.ArticleQuantities.Count; i++)
			{
				var quantity = this.Entity.ArticleQuantities[i];

				if (quantity.Code == code && rank-- == 0)
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
				var newQuantity = this.DataContext.CreateEmptyEntity<ArticleQuantityEntity> ();

				newQuantity.Code     = code;
				newQuantity.Quantity = value.Value;
				newQuantity.Unit     = this.Entity.ArticleDefinition.BillingUnit;

				this.Entity.ArticleQuantities.Add (newQuantity);
			}

			this.UpdatePrices ();
		}


		private UnitOfMeasureEntity GetUnitOfMeasure(string code, int rank)
		{
			foreach (var quantity in this.Entity.ArticleQuantities)
			{
				if (quantity.Code == code && rank-- == 0)
				{
					return quantity.Unit;
				}
			}

			return null;
		}

		private void SetUnitOfMeasure(string code, int rank, UnitOfMeasureEntity value)
		{
			for (int i = 0; i < this.Entity.ArticleQuantities.Count; i++)
			{
				var quantity = this.Entity.ArticleQuantities[i];

				if (quantity.Code == code && rank-- == 0)
				{
					quantity.Unit = value;
					this.UpdateDialogs ();

					if (IsEmpty (quantity))
					{
						this.Entity.ArticleQuantities.RemoveAt (i);

						this.tileContainer.UpdateAllWidgets ();
						this.UpdateDialogs ();
					}

					return;
				}
			}

			if (!string.IsNullOrEmpty (value.Code))
			{
				var newQuantity = this.DataContext.CreateEmptyEntity<ArticleQuantityEntity> ();

				newQuantity.Code     = code;
				newQuantity.Quantity = 1;
				newQuantity.Unit     = value;

				this.Entity.ArticleQuantities.Add (newQuantity);

				this.tileContainer.UpdateAllWidgets ();
				this.UpdateDialogs ();
			}
		}


		private Date? GetDate(string code, int rank)
		{
			foreach (var quantity in this.Entity.ArticleQuantities)
			{
				if (quantity.Code == code && rank-- == 0)
				{
					return quantity.ExpectedDate;
				}
			}

			return null;
		}

		private void SetDate(string code, int rank, Date? value)
		{
			for (int i = 0; i < this.Entity.ArticleQuantities.Count; i++)
			{
				var quantity = this.Entity.ArticleQuantities[i];

				if (quantity.Code == code && rank-- == 0)
				{
					quantity.ExpectedDate = value;
					this.UpdateDialogs ();

					if (IsEmpty (quantity))
					{
						this.Entity.ArticleQuantities.RemoveAt (i);

						this.tileContainer.UpdateAllWidgets ();
						this.UpdateDialogs ();
					}

					return;
				}
			}

			var newQuantity = this.DataContext.CreateEmptyEntity<ArticleQuantityEntity> ();

			newQuantity.Code         = code;
			newQuantity.Quantity     = 1;
			newQuantity.Unit         = this.Entity.ArticleDefinition.BillingUnit;
			newQuantity.ExpectedDate = value;

			this.Entity.ArticleQuantities.Add (newQuantity);

			this.tileContainer.UpdateAllWidgets ();
			this.UpdateDialogs ();
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
					var discount = this.DataContext.CreateEmptyEntity<DiscountEntity> ();
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
				this.UpdateDialogs ();
			}
		}

		private void UpdatePrices()
		{
			//?InvoiceDocumentHelper.UpdatePrices ();  // TODO: je ne sais pas comment retrouver InvoiceDocumentEntity
			ArticleDocumentItemHelper.UpdatePrices (this.Entity);

			this.tileContainer.UpdateAllWidgets ();
			this.UpdateDialogs ();
		}


		private void UpdateDialogs()
		{
			AbstractDocumentItemHelper.UpdateDialogs (this.Entity);
		}


		private static bool IsEmpty(ArticleQuantityEntity quantity)
		{
			return (quantity.Quantity == 0 &&
					string.IsNullOrEmpty (quantity.Unit.Code));
		}



		private NewEntityReference CreateNewArticleDefinition(DataContext context)
		{
			var title = context.CreateRegisteredEmptyEntity<ArticleDefinitionEntity> ();
			return title;
		}

		private NewEntityReference CreateNewUnitOfMeasure(DataContext context)
		{
			var title = context.CreateRegisteredEmptyEntity<UnitOfMeasureEntity> ();
			return title;
		}


		protected override EditionStatus GetEditionStatus()
		{
			return EditionStatus.Valid;
		}


		private TileContainer					tileContainer;
	}
}
