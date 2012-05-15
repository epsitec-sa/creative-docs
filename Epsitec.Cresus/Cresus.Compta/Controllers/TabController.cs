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
	public class TabController
	{
		public TabController(MainWindowController mainWindowController)
		{
			this.mainWindowController = mainWindowController;
		}


		public void CreateUI(FrameBox parent)
		{
			this.tabFrame = new FrameBox
			{
				Parent          = parent,
				BackColor       = RibbonController.GetBackgroundColor1 (),
				Dock            = DockStyle.Fill,
				Padding         = new Margins (2, 2, 4, 0),
			};
		}

		public void CreateTabs(MetaControllerType meta)
		{
#if false
			this.tabFrame.Children.Clear ();

			if (this.mainWindowController.Metas.ContainsKey (meta))
			{
				var types = this.mainWindowController.Metas[meta];

				foreach (var type in types)
				{
					this.CreateButton (this.tabFrame, type);
				}
			}
#endif
		}


		private void CreateButton(Widget parent, ControllerType type)
		{
			bool select = type == this.mainWindowController.SelectedDocument;

			var button = new TabButton
			{
				Parent            = parent,
				Text              = type.ToString (),
				ActiveState       = select ? ActiveState.Yes : ActiveState.No,
				PreferredHeight   = 26,
				Dock              = DockStyle.Left,
				Name              = type.ToString (),
				AutoFocus         = false,
			};

			button.PreferredWidth = button.GetBestFitSize ().Width + 10;

			button.Clicked += delegate
			{
				var t = (ControllerType) System.Enum.Parse (typeof (ControllerType), button.Name);
				this.mainWindowController.ShowPrésentation (t);
			};
		}


		private readonly MainWindowController		mainWindowController;

		private FrameBox tabFrame;
	}
}
