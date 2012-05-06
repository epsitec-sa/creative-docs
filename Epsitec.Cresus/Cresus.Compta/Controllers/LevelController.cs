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
	public class LevelController
	{
		public LevelController(AbstractController controller)
		{
			this.controller = controller;
		}


		public FrameBox CreateUI(FrameBox parent, bool hasBeginnerSpecialist, string clearText, System.Action clearAction, System.Action closeAction, System.Action levelChangedAction)
		{
			var frame = new FrameBox
			{
				Parent          = parent,
				PreferredWidth  = 20,
				PreferredHeight = 20,
				Dock            = DockStyle.Top,
			};

			if (hasBeginnerSpecialist)
			{
				this.beginnerButton = this.CreateButton (frame, "Level.Beginner", "Simple");

				this.arrowButton = new IconButton
				{
					Parent            = frame,
					PreferredIconSize = new Size (10, 20),
					PreferredSize     = new Size (10, 20),
					Dock              = DockStyle.Left,
					AutoToggle        = false,
					AutoFocus         = false,
				};

				ToolTip.Default.SetToolTip (this.arrowButton, "Permute les modes simple et avancé");

				this.specialistButton = this.CreateButton (frame, "Level.Specialist", "Avancé");

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

				this.arrowButton.Clicked += delegate
				{
					this.Specialist = !this.Specialist;
					levelChangedAction ();
				};
			}
			else
			{
				new FrameBox
				{
					Parent        = frame,
					PreferredSize = new Size (20+10+20, 20),
					Dock          = DockStyle.Left,
				};
			}

			this.buttonClear = new IconButton
			{
				Parent        = frame,
				IconUri       = UIBuilder.GetResourceIconUri ("Level.Clear"),
				PreferredSize = new Size (20, 20),
				Dock          = DockStyle.Left,
				Margins       = new Margins (8, 0, 0, 0),
			};

			this.buttonClose = new IconButton
			{
				Parent        = frame,
				IconUri       = UIBuilder.GetResourceIconUri ("Level.Close"),
				PreferredSize = new Size (20, 20),
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
			if (this.beginnerButton != null)
			{
				this.beginnerButton.ActiveState = this.specialist ? ActiveState.No : ActiveState.Yes;
				this.arrowButton.IconUri = UIBuilder.GetResourceIconUri (this.specialist ? "Level.Specialist.Arrow" : "Level.Beginner.Arrow");
				this.specialistButton.ActiveState = this.specialist ? ActiveState.Yes : ActiveState.No;
			}
		}

		private BackIconButton CreateButton(FrameBox parent, string icon, string description)
		{
			var button = new BackIconButton
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
		private IconButton						buttonClose;
		private BackIconButton					beginnerButton;
		private IconButton                      arrowButton;
		private BackIconButton					specialistButton;
	}
}
