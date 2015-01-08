//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Helpers;
using Epsitec.Cresus.Assets.App.Popups;
using Epsitec.Cresus.Assets.App.Widgets;
using Epsitec.Cresus.Assets.Server.SimpleEngine;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;
using Epsitec.Cresus.Assets.App.Settings;

namespace Epsitec.Cresus.Assets.App.Views.CommandToolbars
{
	/// <summary>
	/// Toolbar principale toujours présente en haut de la fenêtre, avec de gros boutons.
	/// </summary>
	public class MainToolbar : AbstractCommandToolbar
	{
		public MainToolbar(DataAccessor accessor, CommandContext commandContext)
			: base (accessor, commandContext)
		{
			this.adjustRequired = false;
		}


		public ViewType							ViewType
		{
			get
			{
				return this.viewType;
			}
			set
			{
				if (this.viewType != value)
				{
					this.viewType = value;
					this.UpdateViewTypeCommands ();
					this.UpdateViewModeCommands ();
				}
			}
		}

		public ViewMode							ViewMode
		{
			get
			{
				return this.viewMode;
			}
			set
			{
				if (this.viewMode != value)
				{
					this.viewMode = value;
					this.UpdateViewModeCommands ();
				}
			}
		}

		public int								Simulation
		{
			get
			{
				return this.simulation;
			}
			set
			{
				if (this.simulation != value)
				{
					this.simulation = value;
					this.UpdateSimulation ();
				}
			}
		}


		public override void CreateUI(Widget parent)
		{
			base.CreateUI (parent);

			this.toolbar.PreferredHeight = AbstractCommandToolbar.primaryToolbarHeight;

			this.viewType = ViewType.Assets;
			this.viewMode = ViewMode.Single;
			this.simulation = 0;

			this.CreateButton (DockStyle.Left, Res.Commands.Main.New);
			this.CreateButton (DockStyle.Left, Res.Commands.Main.Open);
			this.CreateButton (DockStyle.Left, Res.Commands.Main.Save);
			this.CreateButton (DockStyle.Left, Res.Commands.Main.Navigate.Back);
			this.CreateButton (DockStyle.Left, Res.Commands.Main.Navigate.Forward);
			this.CreateButton (DockStyle.Left, Res.Commands.Main.Navigate.Menu);

			this.CreateButton (DockStyle.Left, Res.Commands.View.Assets);
			this.CreateButton (DockStyle.Left, Res.Commands.View.Amortizations);
			this.CreateButton (DockStyle.Left, Res.Commands.View.Entries);
			this.CreateButton (DockStyle.Left, Res.Commands.View.Categories);
			this.CreateButton (DockStyle.Left, Res.Commands.View.Groups);
			this.CreateButton (DockStyle.Left, Res.Commands.View.Persons);
			this.CreateButton (DockStyle.Left, Res.Commands.View.Reports);
			this.buttonWarnings =
			this.CreateButton (DockStyle.Left, Res.Commands.View.Warnings);
			this.CreateButton (DockStyle.Left, Res.Commands.View.Settings);

			this.CreateSajex (10);

			{
				//	Le bouton est mis dans un frame, afin qu'il conserve sa place
				//	même s'il est caché.
				var frame = new FrameBox
				{
					Parent         = this.toolbar,
					PreferredWidth = this.toolbar.PreferredHeight * 1,
					Dock           = DockStyle.Left,
				};

				this.buttonViewMode = this.CreateButton (frame, Res.Commands.ViewMode.Choice);
			}

			this.CreateSajex (10);

			var b1 = this.CreateButton (DockStyle.Left, Res.Commands.Main.Undo);
			var b2 = this.CreateButton (DockStyle.Left, Res.Commands.Main.UndoHistory, widthScale: 0.5);
			var b3 = this.CreateButton (DockStyle.Left, Res.Commands.Main.Redo);
			var b4 = this.CreateButton (DockStyle.Left, Res.Commands.Main.RedoHistory, widthScale: 0.5);

			MainToolbar.AttachHover (b1, b2);
			MainToolbar.AttachHover (b3, b4);

			this.CreateSajex (10);

			{
				//	Le bouton est mis dans un frame, afin qu'il conserve sa place
				//	même s'il est caché.
				var frame = new FrameBox
				{
					Parent         = this.toolbar,
					PreferredWidth = this.toolbar.PreferredHeight * 1,
					Dock           = DockStyle.Left,
				};

				this.CreateButton (frame, Res.Commands.Main.Edit);
			}

			this.CreateButton (DockStyle.Left, Res.Commands.Main.Locked);
			//-this.buttonSimulation =
			//-this.CreateButton (DockStyle.Left, Res.Commands.Main.Simulation);

			this.CreateLanguagesButton ();

			this.CreateButton (DockStyle.Right, Res.Commands.Edit.Cancel);
			this.CreateButton (DockStyle.Right, Res.Commands.Edit.Accept);

			this.UpdateViewTypeCommands ();
			this.UpdateViewModeCommands ();
			this.UpdateSimulation ();
			this.UpdateLanguagesButton ();
		}

		private void CreateLanguagesButton()
		{
			//	Crée le bouton permettant de choisir les langues.
			var size = this.toolbar.PreferredHeight;

			this.buttonLanguages = new Button
			{
				Parent        = this.toolbar,
				ButtonStyle   = ButtonStyle.ToolItem,
				AutoFocus     = false,
				Dock          = DockStyle.Left,
				PreferredSize = new Size (size, size),
			};

			ToolTip.Default.SetToolTip (this.buttonLanguages, Res.Strings.Popup.Language.Tooltip.ToString ());

			this.buttonLanguages.Clicked += delegate (object sender, MessageEventArgs e)
			{
				if (e.Message.IsControlPressed)
				{
					this.SwapLanguages ();
				}
				else
				{
					this.ShowLanguagesPopup (this.buttonLanguages);
				}
			};
		}


		public void UpdateWarningsRedDot()
		{
			//	Met à jour le nombre d'avertissements dans la pastille rouge sur le
			//	bouton de la vue des avertissements.
			//	ATTENTION: Il faut construire la liste complète des avertissements,
			//	ce qui peut prendre du temps !
			//	TODO: Rendre cela asynchrone !?
			if (this.accessor.WarningsDirty && this.buttonWarnings != null)
			{
				var list = LocalSettings.GetVisibleWarnings (this.accessor);
				this.buttonWarnings.RedDotCount = list.Count ();

				this.accessor.WarningsDirty = false;
			}
		}


		private static void AttachHover(ButtonWithRedDot a, ButtonWithRedDot b)
		{
			//	Lie le hover de deux boutons, afin que le survol de l'un produise le
			//	même effet sur l'autre.
			a.Entered += delegate
			{
				b.HoverColor = Color.FromAlphaColor (0.7, ColorManager.HoverColor);
			};

			a.Exited += delegate
			{
				b.HoverColor = Color.Empty;
			};

			b.Entered += delegate
			{
				a.HoverColor = Color.FromAlphaColor (0.7, ColorManager.HoverColor);
			};

			b.Exited += delegate
			{
				a.HoverColor = Color.Empty;
			};
		}


		[Command (Res.CommandIds.View.Assets)]
		[Command (Res.CommandIds.View.Amortizations)]
		[Command (Res.CommandIds.View.Entries)]
		[Command (Res.CommandIds.View.Categories)]
		[Command (Res.CommandIds.View.Groups)]
		[Command (Res.CommandIds.View.Persons)]
		[Command (Res.CommandIds.View.Reports)]
		[Command (Res.CommandIds.View.Warnings)]
		[Command (Res.CommandIds.View.AssetsSettings)]
		[Command (Res.CommandIds.View.PersonsSettings)]
		[Command (Res.CommandIds.View.Accounts)]
		[Command (Res.CommandIds.View.Methods)]
		[Command (Res.CommandIds.View.Arguments)]
		private void OnView(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.ViewType = ViewType.FromDefaultKind (this.accessor, MainToolbar.GetViewKind (e.Command));
			this.OnChangeView ();
		}

		[Command (Res.CommandIds.View.Settings)]
		private void OnViewSettings(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			var target = this.GetTarget (e);
			this.ShowViewPopup (target);
		}

		[Command (Res.CommandIds.ViewMode.Choice)]
		private void OnViewModeChoice(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			var target = this.GetTarget (e);
			this.ShowViewModePopup (target);
		}

		[Command (Res.CommandIds.ViewMode.Single)]
		[Command (Res.CommandIds.ViewMode.Event)]
		[Command (Res.CommandIds.ViewMode.Multiple)]
		private void OnViewMode(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.ViewMode = MainToolbar.GetViewMode (e.Command);
			this.OnChangeView ();
		}


		private void UpdateViewTypeCommands()
		{
			//	"Allume" le bouton correspondant à la vue sélectionnée.
			foreach (var kind in MainToolbar.ViewTypeKinds)
			{
				var command = MainToolbar.GetViewCommand (kind);
				this.SetActiveState (command, this.viewType.Kind == kind);
			}

			//	"Allume" l'engrenage si la vue sélectionnée a été choisie dans le Popup.
			{
				bool active = MainToolbar.PopupViewTypeKinds.Contains (this.viewType.Kind);
				this.SetActiveState (Res.Commands.View.Settings, active);
			}
		}

		private void UpdateViewModeCommands()
		{
			bool visibility = (this.viewType.Kind == ViewTypeKind.Assets);

			this.SetVisibility (Res.Commands.ViewMode.Choice, visibility);
			this.SetEnable     (Res.Commands.ViewMode.Choice, visibility);

			var icon = MainToolbar.GetViewModeIcon (this.viewMode);
			this.buttonViewMode.IconUri = Misc.GetResourceIconUri (icon);

			foreach (var mode in MainToolbar.ViewTypeModes)
			{
				var command = MainToolbar.GetViewModeCommand (mode);

				if (visibility)
				{
					this.SetVisibility  (command, true);
					this.SetEnable      (command, true);
					this.SetActiveState (command, this.viewMode == mode);
				}
				else
				{
					this.SetVisibility  (command, false);
					this.SetActiveState (command, false);
				}
			}
		}

		private void UpdateSimulation()
		{
			string icon = "Main.Simulation";
			
			if (this.simulation >= 0)
			{
				icon += this.simulation.ToString (System.Globalization.CultureInfo.InvariantCulture);
			}

			if (this.buttonSimulation != null)
			{
				this.buttonSimulation.IconUri = Misc.GetResourceIconUri (icon);
			}
		}

		private void UpdateLanguagesButton()
		{
			//	Met à jour le bouton qui affiche les deux langues (UI et Data).
			string desc;

			if (LocalSettings.UILanguage == LocalSettings.DataLanguage)
			{
				//	Si les deux langues sont les mêmes, affiche les 2 lettres en grand.
				desc = string.Format ("<font size=\"18\">{0}</font>", LocalSettings.DataLanguage.ToUpper ());
			}
			else
			{
				//	Si les deux langues sont différentes, elles sont affichées l'une sous l'autre.
				desc = string.Format ("{0}<br/>{1}", LocalSettings.UILanguage.ToUpper (), LocalSettings.DataLanguage.ToUpper ());
			}

			this.buttonLanguages.Text = desc;
		}


		private void ShowViewPopup(Widget target)
		{
			//	Affiche le popup permettant de choisir une vue parmi celles regroupées dans l'engrenage.
			var commands = MainToolbar.PopupViewTypeKinds
				.Select (x => MainToolbar.GetViewCommand (x))
				.ToArray ();

			var popup = new ViewPopup ()
			{
				ViewCommands = commands,
			};

			popup.Create (target, leftOrRight: false);

			popup.ChangeView += delegate (object sender, Command command)
			{
				//	On exécute la commande lorsque le popup est fermé et son CommandDispatcher détruit.
				this.toolbar.ExecuteCommand (command);
			};
		}

		private void ShowViewModePopup(Widget target)
		{
			//	Affiche le popup permettant de choisir le ViewMode.
			var commands = MainToolbar.ViewTypeModes
				.Select (x => MainToolbar.GetViewModeCommand (x))
				.ToArray ();

			var popup = new ViewPopup ()
			{
				ViewCommands = commands,
			};

			popup.Create (target, leftOrRight: false);

			popup.ChangeView += delegate (object sender, Command command)
			{
				//	On exécute la commande lorsque le popup est fermé et son CommandDispatcher détruit.
				this.toolbar.ExecuteCommand (command);
			};
		}

		private void ShowLanguagesPopup(Widget target)
		{
			//	Affiche le popup permettant de choisir les langues.
			LanguagesPopup.Show (target, this.accessor, LocalSettings.UILanguage, LocalSettings.DataLanguage,
				delegate (string uiLanguage, string dataLanguage)
			{
				if (LocalSettings.UILanguage   != uiLanguage  ||
					LocalSettings.DataLanguage != dataLanguage)  // y a-t-il un changement ?
				{
					this.ChangeLanguages (uiLanguage, dataLanguage);
				}
			});
		}

		private void SwapLanguages()
		{
			//	Permute les langues actuelles avec les précédentes.
			if (!string.IsNullOrEmpty (LocalSettings.LastUILanguage) &&
				!string.IsNullOrEmpty (LocalSettings.LastDataLanguage))  // y a-t-il des langues précédentes ?
			{
				this.ChangeLanguages (LocalSettings.LastUILanguage, LocalSettings.LastDataLanguage);
			}
		}

		private void ChangeLanguages(string uiLanguage, string dataLanguage)
		{
			//	Effectue le changement des langues et met à jour tout ce qu'il faut.
			LocalSettings.UILanguage   = uiLanguage;
			LocalSettings.DataLanguage = dataLanguage;

			this.UpdateLanguagesButton ();

			//?var info = Resources.FindSpecificCultureInfo (LocalSettings.UILanguage);
			var info = Resources.FindCultureInfo (LocalSettings.UILanguage);

			//?System.Threading.Thread.CurrentThread.CurrentCulture = info;
			//?System.Threading.Thread.CurrentThread.CurrentUICulture = info;

			var t1 = Res.Strings.Popup.Language.Title;
			ResourceManager.ActivateCulture (info);

			//?if (newTwo != Resources.GetDefaultTwoLetterISOLanguageName ())
			//?{
			//?	Resources.OverrideDefaultTwoLetterISOLanguageName (LocalSettings.UILanguage);
			//?}

			// TODO...
		}


		#region View
		public static ViewTypeKind GetViewKind(Command command)
		{
			foreach (var kind in MainToolbar.ViewTypeKinds)
			{
				if (command == MainToolbar.GetViewCommand (kind))
				{
					return kind;
				}
			}

			return ViewTypeKind.Unknown;
		}

		private static Command GetViewCommand(ViewTypeKind kind)
		{
			switch (kind)
			{
				case ViewTypeKind.Assets:
					return Res.Commands.View.Assets;

				case ViewTypeKind.Amortizations:
					return Res.Commands.View.Amortizations;

				case ViewTypeKind.Entries:
					return Res.Commands.View.Entries;

				case ViewTypeKind.Categories:
					return Res.Commands.View.Categories;

				case ViewTypeKind.Groups:
					return Res.Commands.View.Groups;

				case ViewTypeKind.Persons:
					return Res.Commands.View.Persons;

				case ViewTypeKind.Reports:
					return Res.Commands.View.Reports;

				case ViewTypeKind.Warnings:
					return Res.Commands.View.Warnings;

				case ViewTypeKind.AssetsSettings:
					return Res.Commands.View.AssetsSettings;

				case ViewTypeKind.PersonsSettings:
					return Res.Commands.View.PersonsSettings;

				case ViewTypeKind.Accounts:
					return Res.Commands.View.Accounts;

				case ViewTypeKind.Methods:
					return Res.Commands.View.Methods;

				case ViewTypeKind.Arguments:
					return Res.Commands.View.Arguments;

				default:
					throw new System.InvalidOperationException (string.Format ("Unsupported ViewType {0}", kind.ToString ()));
			}
		}

		private static IEnumerable<ViewTypeKind> PopupViewTypeKinds
		{
			//	Enumère uniquement les vues choisies via le ViewPopup.
			get
			{
				yield return ViewTypeKind.AssetsSettings;
				yield return ViewTypeKind.PersonsSettings;
				yield return ViewTypeKind.Accounts;
				yield return ViewTypeKind.Methods;
				yield return ViewTypeKind.Arguments;
			}
		}

		private static IEnumerable<ViewTypeKind> ViewTypeKinds
		{
			//	Enumère toutes les vues possibles.
			get
			{
				yield return ViewTypeKind.Assets;
				yield return ViewTypeKind.Amortizations;
				yield return ViewTypeKind.Entries;
				yield return ViewTypeKind.Categories;
				yield return ViewTypeKind.Groups;
				yield return ViewTypeKind.Persons;
				yield return ViewTypeKind.Reports;
				yield return ViewTypeKind.Warnings;
				yield return ViewTypeKind.AssetsSettings;
				yield return ViewTypeKind.PersonsSettings;
				yield return ViewTypeKind.Accounts;
				yield return ViewTypeKind.Methods;
				yield return ViewTypeKind.Arguments;
			}
		}
		#endregion


		#region ViewMode
		public static ViewMode GetViewMode(Command command)
		{
			foreach (var mode in MainToolbar.ViewTypeModes)
			{
				if (command == MainToolbar.GetViewModeCommand (mode))
				{
					return mode;
				}
			}

			return ViewMode.Unknown;
		}

		private static Command GetViewModeCommand(ViewMode mode)
		{
			switch (mode)
			{
				case ViewMode.Single:
					return Res.Commands.ViewMode.Single;

				case ViewMode.Event:
					return Res.Commands.ViewMode.Event;

				case ViewMode.Multiple:
					return Res.Commands.ViewMode.Multiple;

				default:
					throw new System.InvalidOperationException (string.Format ("Unsupported ViewMode {0}", mode.ToString ()));
			}
		}

		private static string GetViewModeIcon(ViewMode mode)
		{
			switch (mode)
			{
				case ViewMode.Single:
					return "ViewMode.Single";

				case ViewMode.Event:
					return "ViewMode.Event";

				case ViewMode.Multiple:
					return "ViewMode.Multiple";

				default:
					throw new System.InvalidOperationException (string.Format ("Unsupported ViewMode {0}", mode.ToString ()));
			}
		}

		private static IEnumerable<ViewMode> ViewTypeModes
		{
			get
			{
				yield return ViewMode.Single;
				yield return ViewMode.Event;
				yield return ViewMode.Multiple;
			}
		}
		#endregion


		#region Events handler
		private void OnChangeView()
		{
			this.ChangeView.Raise (this);
		}

		public event EventHandler ChangeView;
		#endregion


		private ButtonWithRedDot				buttonWarnings;
		private ButtonWithRedDot				buttonViewMode;
		private ButtonWithRedDot				buttonSimulation;
		private Button							buttonLanguages;

		private ViewType						viewType;
		private ViewMode						viewMode;
		private int								simulation;
	}
}
