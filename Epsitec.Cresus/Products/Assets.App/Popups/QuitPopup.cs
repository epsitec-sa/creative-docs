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
	/// Popup posant la question standard "Enregistrer avant de quitter ? Oui/Non/Annuler"
	/// avant de quitter le logiciel.
	/// Ce PopUp n'est plus basé sur AbstractStackedPopup, qui n'a pas la souplesse nécessaire
	/// pour afficher 3 boutons dans la partie inférieure.
	/// </summary>
	public class QuitPopup : AbstractPopup
	{
		private QuitPopup(DataAccessor accessor, string directory, string filename)
		{
			this.accessor  = accessor;
			this.directory = directory;
			this.filename  = filename;
		}


		protected override Size					DialogSize
		{
			get
			{
				return new Size (QuitPopup.popupWidth, QuitPopup.popupHeight);
			}
		}

		protected override void CreateUI()
		{
			this.CreateTitle (Res.Strings.Popup.Quit.Title.ToString ());

			//	Crée le frame supérieur qui contiendra le message.
			var messageFrame = this.CreateFrame
			(
				0,
				QuitPopup.buttonHeight,
				QuitPopup.popupWidth,
				QuitPopup.popupHeight - QuitPopup.buttonHeight
			);

			//	Crée le frame inférieur qui contiendra les boutons.
			var buttonsFrame = this.CreateFrame
			(
				0,
				0,
				QuitPopup.popupWidth,
				QuitPopup.buttonHeight
			);

			//	Crée le message.
			var path = System.IO.Path.Combine (this.directory, this.filename);
			var message = string.Format (Res.Strings.Popup.Quit.Message.ToString (), path);

			new StaticText
			{
				Parent           = messageFrame,
				Dock             = DockStyle.Fill,
				Margins          = new Margins (10),
				Text             = message,
				ContentAlignment = ContentAlignment.MiddleCenter,
			};

			//	Crée les boutons.
			int w12 = QuitPopup.buttonWidth;
			int w3  = QuitPopup.popupWidth - QuitPopup.buttonWidth*2 - QuitPopup.buttonMargin12 - QuitPopup.buttonMargin23;

			//	Les textes pour les boutons yes/no/cancel sont dans des ressources spécifiques, car on peut
			//	imaginer de les remplacer par d'autres textes. Par exemple:
			//	Oui     -> Enregistrer
			//	Non     -> Ne pas enregistrer
			//	Annuler -> Annuler
			//	C'est ce que fait Word 2013.

			this.CreateButton (buttonsFrame, w12, QuitPopup.buttonMargin12, "yes",    Res.Strings.Popup.Quit.Yes.Button.ToString (),    Res.Strings.Popup.Quit.Yes.Tooltip.ToString ());
			this.CreateButton (buttonsFrame, w12, QuitPopup.buttonMargin23, "no",     Res.Strings.Popup.Quit.No.Button.ToString (),     Res.Strings.Popup.Quit.No.Tooltip.ToString ());
			this.CreateButton (buttonsFrame, w3,  0,                        "cancel", Res.Strings.Popup.Quit.Cancel.Button.ToString (), Res.Strings.Popup.Quit.Cancel.Tooltip.ToString ());

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
		public static void Show(Widget target, DataAccessor accessor, string directory, string filename, System.Action<bool> action)
		{
			if (target != null)
			{
				var popup = new QuitPopup (accessor, directory, filename);

				popup.Create (target, leftOrRight: false);

				popup.ButtonClicked += delegate (object sender, string name)
				{
					if (name == "yes")
					{
						action (true);  // on enregistre puis on quitte
					}
					else if (name == "no")
					{
						action (false);  // on quitte sans enregistrer
					}

					//	Si on a cliqué "cancel", le Popup sera simplement fermé.
				};
			}
		}
		#endregion


		private const int popupWidth     = 400;
		private const int popupHeight    = 180;
		private const int buttonWidth    = 150;
		private const int buttonMargin12 = 1;	// marge entre Oui/Non
		private const int buttonMargin23 = 10;	// marge entre Non/Annuler
		private const int buttonHeight   = 30;

		private readonly DataAccessor					accessor;
		private readonly string							directory;
		private readonly string							filename;
	}}