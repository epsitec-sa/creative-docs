//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;

using Epsitec.Cresus.Compta.Widgets;
using Epsitec.Cresus.Compta.Helpers;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.Controllers
{
	/// <summary>
	/// Ce contrôleur permet de choisir le niveau débutant/spécialiste.
	/// </summary>
	public class TopPanelRightController
	{
		public TopPanelRightController(AbstractController controller)
		{
			this.controller = controller;
		}


		public FrameBox CreateUI(FrameBox parent, string clearText, System.Action miscAction, System.Action clearAction, System.Action closeAction, System.Action levelChangedAction)
		{
			var frame = new FrameBox
			{
				Parent          = parent,
				PreferredWidth  = 20,
				PreferredHeight = 20,
				Dock            = DockStyle.Top,
			};

			if (miscAction == null)
			{
				new FrameBox
				{
					Parent        = frame,
					PreferredSize = new Size (20, 20),
					Dock          = DockStyle.Left,
				};
			}
			else
			{
				this.buttonMisc = new IconButton
				{
					Parent        = frame,
					IconUri       = UIBuilder.GetResourceIconUri ("Level.Misc"),
					PreferredSize = new Size (20, 20),
					AutoFocus     = false,
					Dock          = DockStyle.Left,
				};

				ToolTip.Default.SetToolTip (this.buttonMisc, "Menu des actions supplémentaires");

				this.buttonMisc.Clicked += delegate
				{
					miscAction ();
				};
			}

			this.buttonClear = new IconButton
			{
				Parent        = frame,
				IconUri       = UIBuilder.GetResourceIconUri ("Level.Clear"),
				PreferredSize = new Size (20, 20),
				AutoFocus     = false,
				Dock          = DockStyle.Left,
			};

			this.buttonClose = new IconButton
			{
				Parent        = frame,
				IconUri       = UIBuilder.GetResourceIconUri ("Level.Close"),
				PreferredSize = new Size (20, 20),
				AutoFocus     = false,
				Dock          = DockStyle.Left,
			};

			ToolTip.Default.SetToolTip (this.buttonClear, clearText);
			ToolTip.Default.SetToolTip (this.buttonClose, "Ferme le panneau");

			this.UpdateButtons ();

			this.buttonClear.Clicked += delegate
			{
				clearAction ();
			};

			this.buttonClose.Clicked += delegate
			{
				closeAction ();
			};

			return frame;
		}


		public IconButton ButtonMisc
		{
			get
			{
				return this.buttonMisc;
			}
		}

		public bool ClearEnable
		{
			get
			{
				return this.buttonClear.Enable;
			}
			set
			{
				this.buttonClear.Enable = value;
			}
		}


		private void UpdateButtons()
		{
		}


		private readonly AbstractController		controller;

		private bool							specialist;
		private IconButton						buttonMisc;
		private IconButton						buttonClear;
		private IconButton						buttonClose;
	}
}
