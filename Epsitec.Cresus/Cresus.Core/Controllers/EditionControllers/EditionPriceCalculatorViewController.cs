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

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers.EditionControllers
{
	public class EditionPriceCalculatorViewController : EditionViewController<Entities.PriceCalculatorEntity>
	{
		public EditionPriceCalculatorViewController(string name, Entities.PriceCalculatorEntity entity)
			: base (name, entity)
		{
		}

		protected override void CreateUI()
		{
			using (var builder = new UIBuilder (this))
			{
				builder.CreateHeaderEditorTile ();
				builder.CreateEditionTitleTile ("Data.PriceCalculator", "Calculateur de prix");

				this.CreateUIMain (builder);

				builder.CreateFooterEditorTile ();
			}
		}

		private void CreateUIMain(UIBuilder builder)
		{
			var tile = builder.CreateEditionTile ();

			builder.CreateTextField      (tile,   0, "Code",        Marshaler.Create (() => this.Entity.Code,        x => this.Entity.Code = x));
			builder.CreateTextField      (tile,   0, "Nom",         Marshaler.Create (() => this.Entity.Name,        x => this.Entity.Name = x));
			builder.CreateTextFieldMulti (tile, 100, "Description", Marshaler.Create (() => this.Entity.Description, x => this.Entity.Description = x));

			builder.CreateMargin (tile, horizontalSeparator: true);
			builder.CreateMargin (tile, horizontalSeparator: false);

			// TODO: Provisoirement, on crée un bouton qui ouvre une fenêtre. Cette fenêtre devra être intégrée dans les tuiles...
			var button = builder.CreateButton (tile, 0, null, "Editer la tabelle de prix...");

			button.Clicked += delegate
			{
				this.Edition ();
			};
		}


		private void Edition()
		{
			var window = this.CreateWindow ();

			var articleDefinition = this.BusinessContext.GetMasterEntity<ArticleDefinitionEntity> ();
			System.Diagnostics.Debug.Assert (articleDefinition != null);
			var tableDesigner = new TableDesignerController (window, this.Orchestrator, this.BusinessContext, this.Entity, articleDefinition);

			var box = tableDesigner.CreateUI ();
			box.Parent = window.Root;

			window.ShowDialog ();
		}

		private Window CreateWindow()
		{
			var window = new Window ();

			window.Owner = CoreProgram.Application.Window;
			window.Icon = CoreProgram.Application.Window.Icon;
			window.Text = "Calculateur de prix";
			window.ClientSize = new Size (800, 600);
			window.Root.WindowStyles = WindowStyles.DefaultDocumentWindow;  // pour avoir les boutons Minimize/Maximize/Close !
			window.AdjustWindowSize ();

			window.WindowCloseClicked += delegate
			{
				window.Hide ();
				window.Close ();
			};

			return window;
		}
	}
}
