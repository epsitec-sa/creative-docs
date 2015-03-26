//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Widgets;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Popups
{
	/// <summary>
	/// Popup posant la question avant de quitter le logiciel.
	/// </summary>
# if false
	public class QuitPopup : AbstractStackedPopup
	{
		private QuitPopup(DataAccessor accessor)
			: base (accessor)
		{
			this.title = Res.Strings.Popup.Quit.Title.ToString ();

			var list = new List<StackedControllerDescription> ();

			list.Add (new StackedControllerDescription  // 0
			{
				StackedControllerType = StackedControllerType.Label,
				Width                 = 200,
				Label                 = Res.Strings.Popup.Quit.Question.ToString (),
			});

			list.Add (new StackedControllerDescription  // 1
			{
				StackedControllerType = StackedControllerType.Radio,
				MultiLabels           = Res.Strings.Popup.Quit.Radios.ToString (),
			});

			this.SetDescriptions (list);

			this.defaultAcceptButtonName = Res.Strings.Popup.Quit.MainButton.ToString ();
		}


		private bool							Save
		{
			get
			{
				var controller = this.GetController (1) as RadioStackedController;
				System.Diagnostics.Debug.Assert (controller != null);
				return controller.Value.GetValueOrDefault () == 0;
			}
			set
			{
				var controller = this.GetController (1) as RadioStackedController;
				System.Diagnostics.Debug.Assert (controller != null);
				controller.Value = value ? 0 : 1;
			}
		}


		protected override void UpdateWidgets(StackedControllerDescription description)
		{
			if (string.IsNullOrEmpty (this.accessor.ComputerSettings.MandatFilename))
			{
				//	S'il n'y a pas de nom de fichier connu pour le mandat, on ne pourra
				//	pas enregistrer au moment de quitter le logiciel.
				this.Save = false;
				this.SetEnable (1, false);
			}
		}


		#region Helpers
		public static void Show(Widget target, DataAccessor accessor, System.Action<bool> action)
		{
			if (target != null)
			{
				var popup = new QuitPopup (accessor)
				{
					Save = true,
				};

				popup.Create (target, leftOrRight: false);

				popup.ButtonClicked += delegate (object sender, string name)
				{
					if (name == "ok")
					{
						action (popup.Save);
					}
				};
			}
		}
		#endregion

	}
#else
	public class QuitPopup : AbstractPopup
	{
		private QuitPopup(DataAccessor accessor)
		{
			this.accessor = accessor;
		}


		protected override Size DialogSize
		{
			get
			{
				return new Size (QuitPopup.popupWidth, QuitPopup.popupHeight);
			}
		}

		protected override void CreateUI()
		{
			this.CreateTitle (Res.Strings.Popup.Quit.Title.ToString ());

			var messageFrame = this.CreateFrame
			(
				0,
				QuitPopup.buttonHeight,
				QuitPopup.popupWidth,
				QuitPopup.popupHeight - QuitPopup.buttonHeight
			);

			var buttonsFrame = this.CreateFrame
			(
				0,
				0,
				QuitPopup.popupWidth,
				QuitPopup.buttonHeight
			);

			new StaticText
			{
				Parent           = messageFrame,
				Dock             = DockStyle.Fill,
				Margins          = new Margins (10),
				Text             = "Voulez-vous enregistrer les modifications avant de quitter ?",
				ContentAlignment = ContentAlignment.MiddleCenter,
			};

			int w1 = QuitPopup.buttonWidth;
			int w2 = QuitPopup.popupWidth - QuitPopup.buttonWidth*2 - 1 - QuitPopup.buttonMargin;

			this.CreateButton (buttonsFrame, w1, 1, "yes", "Oui", "Enregistrer puis quitter");
			this.CreateButton (buttonsFrame, w1, QuitPopup.buttonMargin, "no", "Non", "Quitter sans enregistrer");
			this.CreateButton (buttonsFrame, w2, 0, "cancel", "Annuler", "Ne rien faire");

			this.CreateCloseButton ();
		}

		private ColoredButton CreateButton(FrameBox parent, int dx, int rightMargin, string name, string text, string tooltip)
		{
			var button = new ColoredButton
			{
				Parent         = parent,
				Name           = name,
				Text           = text,
				Dock           = DockStyle.Left,
				PreferredWidth = dx,
				Margins        = new Margins (0, rightMargin, 0, 0),
			};

			if (!string.IsNullOrEmpty (tooltip))
			{
				ToolTip.Default.SetToolTip (button, tooltip);
			}

			button.Clicked += delegate
			{
				this.ClosePopup ();
				this.OnButtonClicked (button.Name);
			};

			return button;
		}


		#region Helpers
		public static void Show(Widget target, DataAccessor accessor, System.Action<bool> action)
		{
			if (target != null)
			{
				var popup = new QuitPopup (accessor);

				popup.Create (target, leftOrRight: false);

				popup.ButtonClicked += delegate (object sender, string name)
				{
					if (name == "yes")
					{
						action (true);
					}
					else if (name == "no")
					{
						action (false);
					}
				};
			}
		}
		#endregion


		private const int popupWidth   = 400;
		private const int popupHeight  = 180;
		private const int buttonWidth  = 150;
		private const int buttonMargin = 10;
		private const int buttonHeight = 30;

		private readonly DataAccessor					accessor;
	}
#endif
}