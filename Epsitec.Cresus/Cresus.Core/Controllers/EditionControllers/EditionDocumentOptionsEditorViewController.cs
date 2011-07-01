//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Widgets;
using Epsitec.Cresus.Core.Widgets.Tiles;
using Epsitec.Cresus.Core.DocumentOptionsEditor;
using Epsitec.Cresus.Bricks;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers.EditionControllers
{
	[ControllerSubType (2)]
	public class EditionDocumentOptionsEditorViewController : EditionViewController<DocumentOptionsEntity>
	{
		public override double GetPreferredWidth(int columnIndex, int columnCount)
		{
			return 640;
		}

#if true
		protected override void CreateBricks(BrickWall<DocumentOptionsEntity> wall)
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
				builder.CreateEditionTitleTile ("Data.DocumentOptions", "Options d'impression pour un document");

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

			var editor = new DocumentOptionsEditorController (this.BusinessContext, this.Entity);

			editor.CreateUI (box);
		}
#endif
	}
}
