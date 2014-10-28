//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Popups.StackedControllers;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Popups
{
	/// <summary>
	/// Popup permettant la saisie le nom d'un mandat à ouvrir.
	/// </summary>
	public class SaveMandatPopup : AbstractStackedPopup
	{
		private SaveMandatPopup(DataAccessor accessor)
			: base (accessor)
		{
			this.title = Res.Strings.Popup.SaveMandat.Title.ToString ();

			var list = new List<StackedControllerDescription> ();

			list.Add (new StackedControllerDescription  // 0
			{
				StackedControllerType = StackedControllerType.Filename,
				Label                 = Res.Strings.Popup.AccountsImport.File.ToString (),
				Width                 = 300,
				BottomMargin          = 10,
			});

			this.SetDescriptions (list);

			this.defaultAcceptButtonName = Res.Strings.Popup.Button.Save.ToString ();
			this.defaultControllerRankFocus = 0;
		}


		private string							Filename
		{
			get
			{
				string				filename;

				{
					var controller = this.GetController (0) as FilenameStackedController;
					System.Diagnostics.Debug.Assert (controller != null);
					filename = controller.Value;
				}

				return filename;
			}
			set
			{
				{
					var controller = this.GetController (0) as FilenameStackedController;
					System.Diagnostics.Debug.Assert (controller != null);
					controller.Value = value;
				}
			}
		}


		protected override void UpdateWidgets(StackedControllerDescription description)
		{
			//	Met à jour le nom du fichier.
			{
				var controller = this.GetController (0) as FilenameStackedController;
				System.Diagnostics.Debug.Assert (controller != null);
				controller.Value = this.Filename;

				controller.DialogTitle      = Res.Strings.Popup.SaveMandat.DialogTitle.ToString ();
				controller.DialogExtensions = ".assets";
				controller.DialogFormatName = Res.Strings.Popup.SaveMandat.DialogFormatName.ToString ();
				controller.Save             = true;

				controller.Update ();
			}

			this.okButton.Enable = !string.IsNullOrEmpty (this.Filename);
		}


		#region Static Helpers
		public static void Show(DataAccessor accessor, Widget target, string filename, System.Action<string> action)
		{
			var popup = new SaveMandatPopup (accessor)
			{
				Filename = filename,
			};

			popup.Create (target, leftOrRight: false);

			popup.ButtonClicked += delegate (object sender, string name)
			{
				if (name == "ok")
				{
					action (popup.Filename);
				}
			};
		}
		#endregion
	}
}