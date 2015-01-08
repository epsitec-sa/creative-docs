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
	/// Ce popup permet de choisir les deux langues de l'application (UI et Data).
	/// </summary>
	public class LanguagesPopup : AbstractStackedPopup
	{
		private LanguagesPopup(DataAccessor accessor)
			: base (accessor)
		{
			this.title = Res.Strings.Popup.Language.Title.ToString ();

			var list = new List<StackedControllerDescription> ();

			list.Add (new StackedControllerDescription  // 0
			{
				StackedControllerType = StackedControllerType.Label,
				Width                 = LanguagesPopup.Width,
				Label                 = Res.Strings.Popup.Language.UI.ToString (),
			});

			list.Add (new StackedControllerDescription  // 1
			{
				StackedControllerType = StackedControllerType.Radio,
				MultiLabels           = LanguagesPopup.RadioLanguages,
				BottomMargin          = 10,
			});

			list.Add (new StackedControllerDescription  // 2
			{
				StackedControllerType = StackedControllerType.Label,
				Width                 = LanguagesPopup.Width,
				Label                 = Res.Strings.Popup.Language.Data.ToString (),
			});

			list.Add (new StackedControllerDescription  // 3
			{
				StackedControllerType = StackedControllerType.Radio,
				MultiLabels           = LanguagesPopup.RadioLanguages,
			});

			this.SetDescriptions (list);
		}


		private int								UILanguage
		{
			get
			{
				var controller = this.GetController (1) as RadioStackedController;
				System.Diagnostics.Debug.Assert (controller != null);
				return controller.Value.GetValueOrDefault ();
			}
			set
			{
				var controller = this.GetController (1) as RadioStackedController;
				System.Diagnostics.Debug.Assert (controller != null);
				controller.Value = value;
			}
		}

		private int								DataLanguage
		{
			get
			{
				var controller = this.GetController (3) as RadioStackedController;
				System.Diagnostics.Debug.Assert (controller != null);
				return controller.Value.GetValueOrDefault ();
			}
			set
			{
				var controller = this.GetController (3) as RadioStackedController;
				System.Diagnostics.Debug.Assert (controller != null);
				controller.Value = value;
			}
		}


		#region Language list
		private static string RadioLanguages
		{
			//	Retourne le texte multilignes permettant de créer les boutons radio.
			get
			{
				return string.Join ("<br/>", LanguagesPopup.Languages.Select (x => LanguagesPopup.GetLanguageName (x)));
			}
		}

		private static int GetLanguageRank(string twoLetterCode)
		{
			return LanguagesPopup.Languages.ToList ().IndexOf (twoLetterCode);
		}

		private static string GetLanguageTwoLetterCode(int rank)
		{
			if (rank == -1)
			{
				return null;
			}
			else
			{
				return LanguagesPopup.Languages.ToArray ()[rank];
			}
		}

		private static string GetLanguageName(string twoLetterCode)
		{
			//	Retourne le nom d'une langue en clair, dans la langue en question.
			//	Il n'est donc pas nécessaire de mettre ces textes dans les ressources.
			//	Par exemple "FR — Français".
			string name = null;

			switch (twoLetterCode)
			{
				case "fr":
					name = "Français";
					break;

				case "de":
					name = "Deutsch";
					break;

				case "en":
					name = "English";
					break;

				case "it":
					name = "Italiano";
					break;
			}

			if (name == null)
			{
				return twoLetterCode.ToUpper ();  // ne devrait pas arriver
			}
			else
			{
				return string.Format ("{0} — {1}", twoLetterCode.ToUpper (), name);
			}
		}

		private static IEnumerable<string> Languages
		{
			//	Retourne les langues disponibles, dans l'ordre dans lequel elles apparaissent
			//	dans la UI.
			get
			{
				yield return "fr";
				yield return "de";
				yield return "en";
				//yield return "it";
			}
		}
		#endregion


		#region Helpers
		public static void Show(Widget target, DataAccessor accessor, string uiLanguage, string dataLanguage, System.Action<string, string> action)
		{
			if (target != null)
			{
				var popup = new LanguagesPopup (accessor)
				{
					UILanguage   = LanguagesPopup.GetLanguageRank (uiLanguage),
					DataLanguage = LanguagesPopup.GetLanguageRank (dataLanguage),
				};

				popup.Create (target, leftOrRight: false);

				popup.ButtonClicked += delegate (object sender, string name)
				{
					if (name == "ok")
					{
						action (
							LanguagesPopup.GetLanguageTwoLetterCode (popup.UILanguage),
							LanguagesPopup.GetLanguageTwoLetterCode (popup.DataLanguage));
					}
				};
			}
		}
		#endregion


		private const int Width = 180;
	}
}