//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

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
	[ControllerSubType (1)]
	public class EditionPriceCalculatorViewControllerTableDesigner : EditionViewController<PriceCalculatorEntity>
	{
		public EditionPriceCalculatorViewControllerTableDesigner(string name, PriceCalculatorEntity entity)
			: base (name, entity)
		{
		}

		public override double GetPreferredWidth(int columnIndex, int columnCount)
		{
			return base.GetPreferredWidth (columnIndex, columnCount) * 3;
		}

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
