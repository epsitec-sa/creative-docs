//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Common.Types;

using Epsitec.Cresus.Compta.Accessors;
using Epsitec.Cresus.Compta.Widgets;
using Epsitec.Cresus.Compta.Helpers;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.Controllers
{
	/// <summary>
	/// Ce contrôleur gère la barre d'outil pour choisir les panneaux visibles.
	/// </summary>
	public class PanelsToolbarController
	{
		public PanelsToolbarController(AbstractController controller)
		{
			this.controller = controller;
		}


		public void CreateUI(FrameBox parent)
		{
			var toolbar = new FrameBox
			{
				Parent          = parent,
				PreferredHeight = PanelsToolbarController.toolbarHeight,
				Dock            = DockStyle.Left,
			};

			this.CreateButton (toolbar, Res.Commands.Panel.ViewSettings, 0, UIBuilder.ViewSettingsBackColor);

			new StaticText
			{
				Parent         = toolbar,
				Text           = UIBuilder.GetTextIconUri ("Panel.Separator"),  // petite flèche ->
				PreferredWidth = 12,
				Dock           = DockStyle.Left,
			};

			this.CreateButton (toolbar, Res.Commands.Panel.Search,  -1, UIBuilder.SearchBackColor);
			this.CreateButton (toolbar, Res.Commands.Panel.Filter,  -1, UIBuilder.FilterBackColor);
			this.CreateButton (toolbar, Res.Commands.Panel.Options,  0, UIBuilder.OptionsBackColor);
		}

		private IconButton CreateButton(Widget parent, Command command, double margin, Color backColor)
		{
			return new BackIconButton
			{
				Parent            = parent,
				CommandObject     = command,
				PreferredIconSize = new Size (20, 20),
				PreferredSize     = new Size (PanelsToolbarController.toolbarHeight, PanelsToolbarController.toolbarHeight),
				BackColor         = backColor,
				Dock              = DockStyle.Left,
				Name              = command.Name,
				AutoFocus         = false,
				Margins           = new Margins (0, margin, 0, 0),
			};
		}


		private static readonly double			toolbarHeight = 24;

		private readonly AbstractController		controller;
	}
}
