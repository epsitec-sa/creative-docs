//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Core.Library;
using Epsitec.Cresus.Assets.App.Widgets;
using Epsitec.Cresus.Assets.Server.SimpleEngine;
using Epsitec.Cresus.Assets.Server.Engine;
using Epsitec.Cresus.Assets.Data;
using Epsitec.Cresus.Assets.App.Settings;
using Epsitec.Cresus.Assets.App.Popups;

namespace Epsitec.Cresus.Assets.App
{
	public class AssetsApplication : CoreInteractiveApp
	{
		public AssetsApplication()
		{
			this.commandDispatcher = new CommandDispatcher ("Assets", CommandDispatcherLevel.Primary, CommandDispatcherOptions.AutoForwardCommands);
			this.commandContext = new CommandContext ();

			this.commandDispatcher.RegisterController (this);
		}

		public override string					ShortWindowTitle
		{
			get
			{
				var filename = (this.accessor == null) ? null : this.accessor.ComputerSettings.MandatFilename;

				if (string.IsNullOrEmpty (filename))
				{
					return Res.Strings.AssetsApplication.WindowTitle.ToString ();
				}
				else
				{
					return string.Join (" - ", filename, Res.Strings.AssetsApplication.WindowTitle.ToString ());
				}
			}
		}
		
		public override string					ApplicationIdentifier
		{
			get
			{
				return "Cr.Assets";
			}
		}

		
		public override bool					StartupLogin()
		{
			return true;
		}


		protected override Window CreateWindow(Size size)
		{
			var window = base.CreateWindow (new Size (1100, 700));

//-			window.MakeTitlelessResizableWindow ();

			return window;
		}


		#region Quit logic
		protected override void ExecuteQuit(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			//	Commande exécutée lorsque l'utilisateur a cliqué la croix de fermeture "x" en haut
			//	à droite de la fenêtre.
			if (!PopupStack.HasPopup)  // garde-fou, car on appelle cette méthode plusieurs fois !
			{
				var target = this.pseudoCloseButton;

				if (string.IsNullOrEmpty (this.accessor.ComputerSettings.MandatFilename))  // fichier inconnu ?
				{
					//	Si le nom du fichier n'est pas connu, on pose simplement la question
					//	"Voulez-vous quitter sans enregistrer ? Oui/Non".
					//	C'est le cas après avoir fait Nouveau et n'avoir jamais enregistré.
					this.WithoutSaveQuit (dispatcher, e, target);
				}
				else  // fichier connu ?
				{
					//	Si le nom du fichier est connu, on pose la question standard
					//	"Enregistrer avant de quitter ? Oui/Non/Annuler".
					this.YesNoCancelQuit (dispatcher, e, target);
				}
			}
		}

		private void WithoutSaveQuit(CommandDispatcher dispatcher, CommandEventArgs e, Widget target)
		{
			//	Pose la question "Voulez-vous quitter sans enregistrer ? Oui/Non".
			string question = Res.Strings.Popup.Quit.WithoutSaveMessage.ToString ();

			YesNoPopup.Show (target, question, delegate
			{
				this.SaveSettingsAndQuit (dispatcher, e);
			},
			300);  // largeur plus grande que la standard
		}

		private void YesNoCancelQuit(CommandDispatcher dispatcher, CommandEventArgs e, Widget target)
		{
			//	Pose la question standard "Enregistrer avant de quitter ? Oui/Non/Annuler".
			QuitPopup.Show (target, this.accessor,
				this.accessor.ComputerSettings.MandatDirectory,
				this.accessor.ComputerSettings.MandatFilename,
				delegate (bool save)
			{
				if (save)  // réponse "oui" ?
				{
					var path = System.IO.Path.Combine (this.accessor.ComputerSettings.MandatDirectory, this.accessor.ComputerSettings.MandatFilename);
					var err = AssetsApplication.SaveMandat (this.accessor, path, this.accessor.GlobalSettings.SaveMandatMode);

					if (!string.IsNullOrEmpty (err))  // erreur ?
					{
						//	En cas d'erreur lors de l'enregistrement, on l'affiche et on ne
						//	quitte pas le logiciel.
						err = TextLayout.ConvertToTaggedText (err);
						MessagePopup.ShowError (target, err);
						return;
					}
				}

				this.SaveSettingsAndQuit (dispatcher, e);
			});
		}

		private void SaveSettingsAndQuit(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			//	Enregistre les réglages locaux puis quitte le logiciel.
			this.accessor.ComputerSettings.WindowPlacement = this.Window.WindowPlacement;
			this.accessor.ComputerSettings.Serialize ();

			base.ExecuteQuit (dispatcher, e);
		}
		#endregion


		protected override CoreAppPolicy CreateDefaultPolicy()
		{
			var policy = base.CreateDefaultPolicy ();
			
			policy.RequiresCoreCommandHandlers = false;
			policy.UseEmbeddedServer = true;

			return policy;
		}

		protected override void CreateManualComponents(IList<System.Action> initializers)
		{
			initializers.Add (this.InitializeApplication);
		}

		protected override System.Xml.Linq.XDocument LoadApplicationState()
		{
			return null;
		}

		protected override void SaveApplicationState(System.Xml.Linq.XDocument doc)
		{
		}

		private void InitializeApplication()
		{
			var window = this.Window;
			
			window.Root.BackColor = Color.FromName ("White");
			this.CreateUI (window);	

			window.Show ();
			window.MakeActive ();

			window.PreventAutoClose = true;
		}


		private void CreateUI(Window window)
		{
			var computerSettings = new ComputerSettings (this.UpdateWindowText);

			if (!computerSettings.WindowPlacement.Bounds.IsSurfaceZero)
			{
				this.Window.WindowPlacement = computerSettings.WindowPlacement;
			}

			//?Epsitec.Common.Widgets.Adorners.Factory.SetActive ("LookMetal");
			//?Epsitec.Common.Widgets.Adorners.Factory.SetActive ("LookSimply");
			Epsitec.Common.Widgets.Adorners.Factory.SetActive ("LookFlat");

			CommandDispatcher.SetDispatcher (this.Window, this.commandDispatcher);
			CommandContext.SetContext (this.Window, this.commandContext);

			var frame = new FrameBox
			{
				Parent    = window.Root,
				Dock      = DockStyle.Fill,
				BackColor = ColorManager.WindowBackgroundColor,
				Name      = "PopupParentFrame",
			};

			//	Crée le bouton invisible qui vise à peu près le bouton de fermeture tout
			//	à droite dans la barre de titre Windows.
			this.pseudoCloseButton = new Button
			{
				Parent        = window.Root,
				Anchor        = AnchorStyles.TopRight,
				PreferredSize = Size.Zero,
				Margins       = new Margins (0, 20, 0, 0),
			};

			//	Crée le clipboard unique à l'application.
			var cb = new DataClipboard ();

			//	Crée et ouvre le mandat par défaut.
			this.accessor = new DataAccessor(computerSettings, cb);

			//	Nécessaire, car lors de la première initialisation de computerSettings.MandatFilename,
			//	this.accessor était null !
			computerSettings.UpdateWindowTitle ();

			if (string.IsNullOrEmpty (computerSettings.MandatFilename))
			{
				this.OpenDefaultMandat ();
			}
			else
			{
				try
				{
					var filename = System.IO.Path.Combine (computerSettings.MandatDirectory, computerSettings.MandatFilename);
					var err = AssetsApplication.OpenMandat (this.accessor, filename);

					if (!string.IsNullOrEmpty (err))  // erreur ?
					{
						this.OpenDefaultMandat ();
					}
				}
				catch
				{
					this.OpenDefaultMandat ();
				}
			}

			var ui = new AssetsUI (this.accessor, this.commandDispatcher, this.commandContext);
			ui.CreateUI (frame);

			frame.Focus ();
		}

		private void OpenDefaultMandat()
		{
			var factory = MandatFactory.Factories.Where (x => x.IsDefault).FirstOrDefault ();
			System.Diagnostics.Debug.Assert (factory != null);
			factory.Create (this.accessor, Res.Strings.AssetsApplication.DefaultMandat.ToString (),
				new System.DateTime (2014, 1, 1), true);
		}

		public static bool IsExistingMandat(string filename)
		{
			//	Indique si le fichier d'un mandat existe.
			try
			{
				return DataIO.IsExistingMandat (filename);
			}
			catch
			{
				return false;
			}
		}

		public static string OpenMandat(DataAccessor accessor, string filename)
		{
			//	Nécessaire si DataIO.OpenMandat ne lit pas les LocalSettings !
			LocalSettings.Initialize (Timestamp.Now.Date);

			try
			{
				DataIO.OpenMandat (accessor, filename, delegate (System.Xml.XmlReader reader)
				{
					//	Effectue la désérialisation des LocalSettings.
					LocalSettings.Deserialize (reader);
				});
			}
			catch (System.Exception ex)
			{
				return ex.Message;
			}

			return null;  // ok
		}

		public static string SaveMandat(DataAccessor accessor, string filename, SaveMandatMode mode)
		{
			try
			{
				DataIO.SaveMandat (accessor, filename, mode, delegate (System.Xml.XmlWriter writer)
				{
					//	Effectue la sérialisation des LocalSettings.
					LocalSettings.Serialize (writer);
				});
			}
			catch (System.Exception ex)
			{
				return ex.Message;
			}

			return null;  // ok
		}


		private readonly CommandDispatcher		commandDispatcher;
		private readonly CommandContext			commandContext;

		private DataAccessor					accessor;
		private Button							pseudoCloseButton;
	}
}
