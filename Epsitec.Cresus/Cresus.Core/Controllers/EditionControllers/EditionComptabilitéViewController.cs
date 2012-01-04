//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;
using Epsitec.Common.Types.Converters;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;

using Epsitec.Cresus.Bricks;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.ComptabilitéControllers;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers.EditionControllers
{
	public class EditionComptabilitéViewController : EditionViewController<ComptabilitéEntity>
	{
#if false
		protected override void CreateBricks(BrickWall<ComptabilitéEntity> wall)
		{
			wall.AddBrick ()
				.Icon ("Comptabilité")
				.Input ()
				  .Title ("Nom de la comptabilité").Field (x => x.Name)
				  .Title ("Description de la comptabilité").Field (x => x.Description)
				.End ()
				.Separator ()
				.Input ()
				  .Title ("Début de la période comptable").Field (x => x.BeginDate)
				  .Title ("Fin de la période comptable").Field (x => x.EndDate)
				.End ()
				;
		}
#endif

		protected override void CreateUI()
		{
			using (var builder = new UIBuilder (this))
			{
				builder.CreateHeaderEditorTile ();
				builder.CreateEditionTitleTile ("Comptabilité", "Comptabilité");

				this.CreateUIMain (builder);

				builder.CreateFooterEditorTile ();
			}
		}


		private void CreateUIMain(Epsitec.Cresus.Core.UIBuilder builder)
		{
			var tile = builder.CreateEditionTile ();

			builder.CreateTextField (tile, 0, "Nom de la comptabilité", Marshaler.Create (() => this.Entity.Name, x => this.Entity.Name = x));
			builder.CreateTextFieldMulti (tile, 52, "Description de la comptabilité", Marshaler.Create (() => this.Entity.Description, x => this.Entity.Description = x));

			builder.CreateMargin (tile, horizontalSeparator: true);

			builder.CreateTextField (tile, 120, "Début de la période comptable", Marshaler.Create (() => this.Entity.BeginDate, x => this.Entity.BeginDate = x));
			builder.CreateTextField (tile, 120, "Fin de la période comptable", Marshaler.Create (() => this.Entity.EndDate, x => this.Entity.EndDate   = x));

			builder.CreateMargin (tile, horizontalSeparator: true);

			this.CreateAccessor (tile.Container);
		}

		private void CreateAccessor(Widget parent)
		{
			var button = new Button
			{
				Parent          = parent,
				FormattedText   = TextFormatter.FormatText("Accès aux données comptables").ApplyFontSize (13.0),
				PreferredHeight = 50,
				Dock            = DockStyle.Stacked,
				Margins         = new Margins (0, 10, 10, 10),
			};

			button.Clicked += delegate
			{
				this.CreateWindow (parent);
			};

			ToolTip.Default.SetToolTip (button, "Ouvre une nouvelle fenêtre pour les données comptables");
		}

		private void CreateWindow(Widget parent)
		{
			var window = new MainWindow (this.Data.Host, this.TileContainer.Controller.BusinessContext, this.Entity, MainWindow.TypeDeDocumentComptable.Journal);

			window.IsModal = false;
			window.OpenDialog ();
		}
	}
}
