//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Common.Types;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Widgets;
using Epsitec.Cresus.Core.Business;

using Epsitec.Cresus.Compta.Accessors;
using Epsitec.Cresus.Compta.Entities;

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

			this.CreateButton (toolbar, Res.Commands.Panel.Search);
			this.CreateButton (toolbar, Res.Commands.Panel.Filter);
			this.CreateButton (toolbar, Res.Commands.Panel.Options);
			this.CreateButton (toolbar, Res.Commands.Panel.Info);
		}

		private IconButton CreateButton(Widget parent, Command command)
		{
			return new RibbonIconButton
			{
				Parent            = parent,
				CommandObject     = command,
				PreferredIconSize = new Size (20, 20),
				PreferredSize     = new Size (20, 20),
				Dock              = DockStyle.Left,
				Name              = command.Name,
				AutoFocus         = false,
				Margins           = new Margins (0, 1, 0, 0),
			};
		}


		private static readonly double			toolbarHeight = 20;

		private readonly AbstractController		controller;
	}
}
