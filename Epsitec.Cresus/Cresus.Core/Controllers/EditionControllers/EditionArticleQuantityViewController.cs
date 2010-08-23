//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;
using Epsitec.Common.Types.Converters;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Widgets;
using Epsitec.Cresus.Core.Widgets.Tiles;

using Epsitec.Cresus.DataLayer.Context;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers.EditionControllers
{
	public class EditionArticleQuantityViewController : EditionViewController<Entities.ArticleQuantityEntity>
	{
		public EditionArticleQuantityViewController(string name, Entities.ArticleQuantityEntity entity)
			: base (name, entity)
		{
		}

		protected override void CreateUI()
		{
			using (var builder = new UIBuilder (this))
			{
				builder.CreateHeaderEditorTile ();
				builder.CreateEditionTitleTile ("Data.ArticleQuantity", "Quantité");

				this.CreateUIType          (builder);
				this.CreateUIMain          (builder);
				this.CreateUIUnitOfMeasure (builder);

				builder.CreateFooterEditorTile ();
			}
		}

		protected override EditionStatus GetEditionStatus()
		{
			return EditionStatus.Valid;
		}


		private void CreateUIType(Epsitec.Cresus.Core.UIBuilder builder)
		{
			var controller = new EnumController<BusinessLogic.ArticleQuantityType> (BusinessLogic.Enumerations.GetAllPossibleValueArticleQuantityType ())
			{
				ValueGetter = () => this.Entity.QuantityType,
				ValueSetter = x => this.Entity.QuantityType = x,
				ValueToDescriptionConverter = x => TextFormatter.FormatText (x as string[]),
			};

			builder.CreateEditionDetailedItemPicker ("Type", controller);
		}

		private void CreateUIMain(UIBuilder builder)
		{
			var tile = builder.CreateEditionTile ();

			//?builder.CreateAutoCompleteTextField (tile,  87, "Type",        Marshaler.Create (this.Entity, x => x.QuantityType, (x, v) => x.QuantityType = v), BusinessLogic.Enumerations.GetAllPossibleValueArticleQuantityType (), x => TextFormatter.FormatText (x.Values[0]));
			builder.CreateTextField             (tile, 100, "Date prévue", Marshaler.Create (() => this.Entity.ExpectedDate, x => this.Entity.ExpectedDate = x));
			builder.CreateMargin                (tile, horizontalSeparator: true);
			builder.CreateTextField             (tile,  60, "Quantité",    Marshaler.Create (() => this.Entity.Quantity, x => this.Entity.Quantity = x));
		}

		private void CreateUIUnitOfMeasure(UIBuilder builder)
		{
#if false
			builder.CreateAutoCompleteTextField ("Unité",
				new SelectionController<UnitOfMeasureEntity>
				{
					ValueGetter = () => this.Entity.Unit,
					ValueSetter = x => this.Entity.Unit = x.WrapNullEntity (),
					ReferenceController = new ReferenceController (() => this.Entity.Unit, creator: this.CreateNewUnitOfMeasure),
					//?PossibleItemsGetter = () => this.GetUnitOfMeasure (),
					PossibleItemsGetter = () => CoreProgram.Application.Data.GetUnitOfMeasure (),

					ToTextArrayConverter     = x => new string[] { x.Name, x.Code },
					ToFormattedTextConverter = x => TextFormatter.FormatText (x.Name, "(", x.Code, ")")
				});
#else
			builder.CreateEditionDetailedItemPicker ("Unité",
				new SelectionController<UnitOfMeasureEntity>
				{
					ValueGetter = () => this.Entity.Unit,
					ValueSetter = x => this.Entity.Unit = x.WrapNullEntity (),
					ReferenceController = new ReferenceController (() => this.Entity.Unit, creator: this.CreateNewUnitOfMeasure),
					//?PossibleItemsGetter = () => this.GetUnitOfMeasure (),
					PossibleItemsGetter = () => CoreProgram.Application.Data.GetUnitOfMeasure (),

					ToTextArrayConverter     = x => new string[] { x.Name.ToSimpleText (), x.Code },
					ToFormattedTextConverter = x => TextFormatter.FormatText (x.Name, "(", x.Code, ")")
				}, BusinessLogic.EnumValueCardinality.ExactlyOne);
#endif
		}

		// TODO: Dès qu'il sera possible de connaître l'entité parente, il faudra réactiver le code ci-dessous:
#if false
		private IEnumerable<UnitOfMeasureEntity> GetUnitOfMeasure()
		{
			//	Retourne les unités appartenant au même groupe que l'article.
			return CoreProgram.Application.Data.GetUnitOfMeasure ().Where (x => x.Category == this.Entity.ArticleDefinition.Units.Category);
		}
#endif


		private NewEntityReference CreateNewUnitOfMeasure(DataContext context)
		{
			var title = context.CreateEmptyEntity<UnitOfMeasureEntity> ();
			return title;
		}



	}
}
