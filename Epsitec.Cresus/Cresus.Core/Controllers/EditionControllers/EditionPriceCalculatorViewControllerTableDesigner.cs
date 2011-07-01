//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Widgets;
using Epsitec.Cresus.Core.Widgets.Tiles;
using Epsitec.Cresus.Core.TableDesigner;
using Epsitec.Cresus.Bricks;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers.EditionControllers
{
	[ControllerSubType (1)]
	public class EditionPriceCalculatorViewControllerTableDesigner : EditionViewController<PriceCalculatorEntity>
	{
		public override double GetPreferredWidth(int columnIndex, int columnCount)
		{
			return base.GetPreferredWidth (columnIndex, columnCount) * 3;
		}

#if true
		protected override void CreateBricks(BrickWall<PriceCalculatorEntity> wall)
		{
			wall.AddBrick ()
				.Input ()
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
				builder.CreateEditionTitleTile ("Data.PriceCalculator", "Tabelle de prix");

				this.CreateUIMain (builder);

				builder.CreateFooterEditorTile ();
			}
		}

		private void CreateUIMain(UIBuilder builder)
		{
			var tile = builder.CreateEditionTile ();

			var box = new FrameBox
			{
				Parent = tile.Container,
				PreferredHeight = 500,  // TODO: Comment mettre un mode "full height" ?
				Dock = DockStyle.Top,
			};

			var articleDefinition = this.BusinessContext.GetMasterEntity<ArticleDefinitionEntity> ();
			System.Diagnostics.Debug.Assert (articleDefinition != null);
			var tableDesigner = new TableDesignerController (this.Orchestrator, this.BusinessContext, this.Entity, articleDefinition);

			tableDesigner.CreateUI (box);
		}
#endif
	}
}
