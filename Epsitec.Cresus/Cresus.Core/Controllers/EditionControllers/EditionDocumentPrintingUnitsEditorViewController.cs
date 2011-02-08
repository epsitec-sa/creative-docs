//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Widgets;
using Epsitec.Cresus.Core.Widgets.Tiles;
using Epsitec.Cresus.Core.DocumentPrintingUnitsEditor;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers.EditionControllers
{
	[ControllerSubType (2)]
	public class EditionDocumentPrintingUnitsEditorViewController : EditionViewController<DocumentPrintingUnitsEntity>
	{
		public EditionDocumentPrintingUnitsEditorViewController(string name, DocumentPrintingUnitsEntity entity)
			: base (name, entity)
		{
		}

		public override double GetPreferredWidth(int columnIndex, int columnCount)
		{
			return base.GetPreferredWidth (columnIndex, columnCount) * 2;
		}

		protected override void CreateUI()
		{
			using (var builder = new UIBuilder (this))
			{
				builder.CreateHeaderEditorTile ();
				builder.CreateEditionTitleTile ("Data.DocumentPrintingUnits", "Unités d'impression");

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

			var editor = new DocumentPrintingUnitsEditorController (this.Orchestrator, this.BusinessContext, this.Entity);

			editor.CreateUI (box);
		}
	}
}
