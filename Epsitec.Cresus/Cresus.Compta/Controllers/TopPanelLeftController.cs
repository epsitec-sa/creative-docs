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
	public class TopPanelLeftController
	{
		public TopPanelLeftController(AbstractController controller)
		{
			this.controller = controller;
		}


		public FrameBox CreateUI(FrameBox parent, bool hasBeginnerSpecialist, string icon, System.Action levelChangedAction)
		{
			var frame = new FrameBox
			{
				Parent          = parent,
				PreferredWidth  = 20,
				PreferredHeight = 20,
				Dock            = DockStyle.Top,
			};

#if true
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
#else
			this.levelButton = new IconButton
			{
				Parent        = frame,
				IconUri       = UIBuilder.GetResourceIconUri (icon),
				PreferredSize = new Size (20, 20),
				Dock          = DockStyle.Left,
				Enable        = hasBeginnerSpecialist,
				AutoFocus     = false,
			};

			if (hasBeginnerSpecialist)
			{
				ToolTip.Default.SetToolTip (this.levelButton, "Mode simple ou avancé");

				this.levelMarker = UIBuilder.CreateMarker (this.levelButton, "Panel.Specialist");

				this.levelButton.Clicked += delegate
				{
					this.Specialist = !this.Specialist;
					levelChangedAction ();
				};
			}
#endif

			this.UpdateButtons ();

			return frame;
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

			if (this.levelMarker != null)
			{
				this.levelMarker.Visibility = this.specialist;
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

		private StaticText CreateMarker(Widget parent)
		{
			//	Crée le petit 'vu' vert en surimpression d'un bouton. Par chance, le widget StaticText ne capture
			//	pas les événements souris !
			return new StaticText
			{
				Parent           = parent,
				Text             = UIBuilder.GetIconTag ("Panel.Specialist"),
				ContentAlignment = ContentAlignment.BottomRight,
				Anchor           = AnchorStyles.All,
			};
		}


		private readonly AbstractController		controller;

		private bool							specialist;
		private BackIconButton					beginnerButton;
		private IconButton                      arrowButton;
		private BackIconButton					specialistButton;
		private IconButton						levelButton;
		private StaticText						levelMarker;
	}
}
