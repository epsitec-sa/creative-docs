//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Popups.StackedControllers;
using Epsitec.Cresus.Assets.Data;
using Epsitec.Cresus.Assets.Data.Helpers;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Popups
{
	/// <summary>
	/// Popup permettant la saisie le nom d'un mandat à ouvrir.
	/// </summary>
	public class OpenMandatPopup : AbstractStackedPopup
	{
		private OpenMandatPopup(DataAccessor accessor)
			: base (accessor)
		{
			this.title = Res.Strings.Popup.OpenMandat.Title.ToString ();

			var list = new List<StackedControllerDescription> ();

			list.Add (new StackedControllerDescription  // 0
			{
				StackedControllerType = StackedControllerType.Filename,
				Label                 = Res.Strings.Popup.AccountsImport.File.ToString (),
				Width                 = 300,
				BottomMargin          = 10,
			});

			list.Add (new StackedControllerDescription  // 1
			{
				StackedControllerType = StackedControllerType.Label,
				Width                 = 300,
				Height                = 15*MandatStatistics.LinesCount,  // place pour les lignes des statistiques
			});

			this.SetDescriptions (list);

			this.defaultAcceptButtonName = Res.Strings.Popup.Button.Open.ToString ();
			this.defaultControllerRankFocus = 0;
		}


		private string							Path
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
				controller.Value = this.Path;

				controller.DialogTitle      = Res.Strings.Popup.OpenMandat.DialogTitle.ToString ();
				controller.DialogExtensions = IOHelpers.Extension;
				controller.DialogFormatName = Res.Strings.Popup.OpenMandat.DialogFormatName.ToString ();
				controller.Save             = false;

				controller.Update ();
			}

			//	Met à jour le rapport.
			{
				var controller = this.GetController (1) as LabelStackedController;
				System.Diagnostics.Debug.Assert (controller != null);
				controller.SetLabel (this.Statistics);
			}

			this.okButton.Enable = !string.IsNullOrEmpty (this.Path);
		}


		private string Statistics
		{
			//	Retourne les statistiques sur le fichier à ouvrir, obtenues en lisant
			//	uniquement le petit fichier xml d'informations.
			get
			{
				if (!string.IsNullOrEmpty (this.Path))  // a-t-on donné un nom de fichier ?
				{
					try
					{
						var info = DataIO.OpenInfo (this.Path);

						if (!info.IsEmpty && !info.Statistics.IsEmpty)
						{
							return info.Statistics.Summary;
						}
					}
					catch (System.Exception ex)
					{
						return ex.Message;  // retourne l'erreur comme statistique
					}
				}

				return null;  // pas de statistiques à afficher
			}
		}


		#region Static Helpers
		public static void Show(DataAccessor accessor, Widget target, string directory, string filename, System.Action<string> action)
		{
			if (string.IsNullOrEmpty (filename))
			{
				filename = "default" + IOHelpers.Extension;
			}

			var popup = new OpenMandatPopup (accessor)
			{
				Path = System.IO.Path.Combine (directory, filename),
			};

			popup.Create (target, leftOrRight: false);

			popup.ButtonClicked += delegate (object sender, string name)
			{
				if (name == "ok")
				{
					action (popup.Path);
				}
			};
		}
		#endregion
	}
}