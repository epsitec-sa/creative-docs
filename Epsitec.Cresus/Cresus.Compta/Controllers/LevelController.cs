//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;

using Epsitec.Cresus.Core.Widgets;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.Controllers
{
	/// <summary>
	/// Ce contrôleur permet de choisir le niveau débutant/spécialiste.
	/// </summary>
	public class LevelController
	{
		public LevelController(AbstractController controller)
		{
			this.controller = controller;
		}


		public FrameBox CreateUI(FrameBox parent, string clearText, System.Action clearAction, System.Action levelChangedAction)
		{
			var frame = new FrameBox
			{
				Parent          = parent,
				PreferredWidth  = 20,
				PreferredHeight = 20,
				Dock            = DockStyle.Top,
			};

			this.beginnerButton   = this.CreateButton (frame, "Level.Beginner", "Simple");
			this.specialistButton = this.CreateButton (frame, "Level.Specialist", "Avancé");

			this.buttonClear = new IconButton
			{
				Parent        = frame,
				IconUri       = UIBuilder.GetResourceIconUri ("Edit.Cancel"),
				PreferredSize = new Size (20, 20),
				Dock          = DockStyle.Left,
				Margins       = new Margins (8, 0, 0, 0),
			};

			ToolTip.Default.SetToolTip (this.buttonClear, clearText);

			this.UpdateButtons ();

			this.buttonClear.Clicked += delegate
			{
				clearAction ();
			};

			this.beginnerButton.Clicked += delegate
			{
				this.Specialist = false;
				levelChangedAction ();
			};

			this.specialistButton.Clicked += delegate
			{
				this.Specialist = true;
				levelChangedAction ();
			};

			return frame;
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

		public bool Beginner
		{
			get
			{
				return !this.Specialist;
			}
			set
			{
				this.Specialist = !value;
			}
		}

		public bool Specialist
		{
			get
			{
				return this.specialist;
			}
			set
			{
				if (this.specialist != value)
				{
					this.specialist = value;
					this.UpdateButtons ();
				}
			}
		}


		private void UpdateButtons()
		{
			this.beginnerButton.ActiveState   = this.specialist ? ActiveState.No  : ActiveState.Yes;
			this.specialistButton.ActiveState = this.specialist ? ActiveState.Yes : ActiveState.No;
		}

		private RibbonIconButton CreateButton(FrameBox parent, string icon, string description)
		{
			var button = new RibbonIconButton
			{
				Parent            = parent,
				IconUri           = UIBuilder.GetResourceIconUri (icon),
				PreferredIconSize = new Size (20, 20),
				PreferredSize     = new Size (20, 20),
				Dock              = DockStyle.Left,
				AutoToggle        = false,
				AutoFocus         = false,
			};

			ToolTip.Default.SetToolTip (button, description);

			return button;
		}


		private readonly AbstractController		controller;

		private bool							specialist;
		private IconButton						buttonClear;
		private RibbonIconButton				beginnerButton;
		private RibbonIconButton				specialistButton;
	}
}
