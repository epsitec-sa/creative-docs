//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;
using Epsitec.Common.Types.Converters;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.DataAccessors;
using Epsitec.Cresus.Core.Controllers.SummaryControllers;
using Epsitec.Cresus.Core.Widgets;
using Epsitec.Cresus.Core.Widgets.Tiles;

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
			using (var builder = new UIBuilder (container, this))
			{
				builder.CreateHeaderEditorTile ();
				builder.CreateEditionTitleTile ("Data.ArticleDocumentItem", "Ligne d'article");

				this.CreateUIMain (builder);
				this.CreateUIArticleDefinition (builder);
				this.CreateUIPrice (builder);

				builder.CreateFooterEditorTile ();
			}
		}


		private void CreateUIMain(Epsitec.Cresus.Core.UIBuilder builder)
		{
			var tile = builder.CreateEditionTile ();

			builder.CreateTextField (tile, 50, "Quantité", Marshaler.Create (this.GetQuantity, this.SetQuantity));
			builder.CreateTextField (tile, 50, "Unité",    Marshaler.Create (this.GetUnit, this.SetUnit));
		}


		private void CreateUIPrice(UIBuilder builder)
		{
			var tile = builder.CreateEditionTile ();

			builder.CreateTextField (tile, 100, "Prix unitaire", Marshaler.Create (this.GetPrice, this.SetPrice));
		}


		private void CreateUIArticleDefinition(UIBuilder builder)
		{
			var textField = builder.CreateAutoCompleteTextField ("Article à facturer",
				new SelectionController<ArticleDefinitionEntity>
				{
					ValueGetter = () => this.Entity.ArticleDefinition,
					ValueSetter = x => this.Entity.ArticleDefinition = x.WrapNullEntity (),
					ReferenceController = new ReferenceController (() => this.Entity.ArticleDefinition, creator: this.CreateNewArticleDefinition),
					PossibleItemsGetter = () => CoreProgram.Application.Data.GetArticleDefinitions (),

					ToTextArrayConverter     = x => new string[] { x.IdA, x.ShortDescription },
					ToFormattedTextConverter = x => UIBuilder.FormatText (x.IdA, x.ShortDescription),
				});
		}


		private decimal GetQuantity()
		{
			foreach (var quantity in this.Entity.ArticleQuantities)
			{
				if (quantity.Code != "suivra")
				{
					return quantity.Quantity;
				}
			}

			return 0;
		}


		private void SetQuantity(decimal value)
		{
		}

		private string GetUnit()
		{
			foreach (var quantity in this.Entity.ArticleQuantities)
			{
				if (quantity.Code != "suivra")
				{
					return quantity.Unit.Code;
				}
			}

			return null;
		}

		private void SetUnit(string value)
		{
		}


		private decimal GetPrice()
		{
			return SummaryInvoiceDocumentViewController.GetArticlePrice (this.Entity);
		}

		private void SetPrice(decimal value)
		{
		}


	
		
		private NewEntityReference CreateNewArticleDefinition(DataContext context)
		{
			var title = context.CreateRegisteredEmptyEntity<ArticleDefinitionEntity> ();
			return title;
		}


		protected override EditionStatus GetEditionStatus()
		{
			return EditionStatus.Valid;
		}
	}
}
