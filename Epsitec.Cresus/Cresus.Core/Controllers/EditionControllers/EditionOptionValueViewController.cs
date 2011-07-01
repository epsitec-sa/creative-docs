//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Types;
using Epsitec.Common.Types.Converters;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Widgets;
using Epsitec.Cresus.Core.Widgets.Tiles;
using Epsitec.Cresus.Core.TableDesigner;
using Epsitec.Cresus.Bricks;

using Epsitec.Cresus.DataLayer.Context;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers.EditionControllers
{
	public class EditionOptionValueViewController : EditionViewController<OptionValueEntity>
	{
#if true
		protected override void CreateBricks(BrickWall<OptionValueEntity> wall)
		{
			wall.AddBrick ()
				.GlobalWarning ()
				.Input ()
				  .Field (x => x.Name)
				  .Field (x => x.Description)
				.End ()
				.Separator ()
				.Input ()
				  .Field (x => x.Quantity)
				  .Field (x => x.ArticleDefinition)
				  .Field (x => x).WithSpecialController ()
				.End ()
				;
		}
#else
		protected override void CreateUI()
		{
			using (var builder = new UIBuilder (this))
			{
				builder.CreateHeaderEditorTile ();
				builder.CreateEditionTitleTile ("Data.OptionValue", "Option");

				this.CreateUIMain (builder);
				this.CreateUIArticleDefinition (builder);
				this.CreateUIParameter (builder);

				builder.CreateFooterEditorTile ();
			}
		}

		private void CreateUIMain(UIBuilder builder)
		{
			var tile = builder.CreateEditionTile ();

			builder.CreateTextField      (tile,   0, "Nom",         Marshaler.Create (() => this.Entity.Name,        x => this.Entity.Name = x));
			builder.CreateTextFieldMulti (tile, 100, "Description", Marshaler.Create (() => this.Entity.Description, x => this.Entity.Description = x));

			builder.CreateMargin (tile, horizontalSeparator: true);

			builder.CreateTextField (tile, 0, "Quantité", Marshaler.Create (() => this.Entity.Quantity, x => this.Entity.Quantity = x));
		}

		private void CreateUIArticleDefinition(UIBuilder builder)
		{
			var controller = new SelectionController<ArticleDefinitionEntity> (this.BusinessContext)
			{
				ValueGetter         = () => this.Entity.ArticleDefinition,
				ValueSetter         = x => this.Entity.ArticleDefinition = x,
				ReferenceController = new ReferenceController (() => this.Entity.ArticleDefinition, creator: this.CreateNewArticle),
			};

			builder.CreateAutoCompleteTextField ("Article", controller);
		}

		private void CreateUIParameter(UIBuilder builder)
		{
			var tile = builder.CreateEditionTile ();
			var group = builder.CreateGroup (tile);

			this.parameterController = new ArticleParameterControllers.ValuesArticleParameterController (this.TileContainer, tile);
			this.parameterController.CreateUI (group);
			this.parameterController.UpdateUI (this.Entity);
		}


		private NewEntityReference CreateNewArticle(DataContext context)
		{
			return context.CreateEntityAndRegisterAsEmpty<ArticleDefinitionEntity> ();
		}


		private ArticleParameterControllers.ValuesArticleParameterController	parameterController;
#endif
	}
}
