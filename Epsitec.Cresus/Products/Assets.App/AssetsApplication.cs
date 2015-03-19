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
			Epsitec.Cresus.Assets.Server.LogFile.CreateEmptyLogFile ();
			Epsitec.Cresus.Assets.Server.LogFile.AppendToLogFile ("Start");
			this.commandDispatcher = new CommandDispatcher ("Assets", CommandDispatcherLevel.Primary, CommandDispatcherOptions.AutoForwardCommands);
			this.commandContext = new CommandContext ();

			this.commandDispatcher.RegisterController (this);
		}

		public override string					ShortWindowTitle
		{
			get
			{
				return Res.Strings.AssetsApplication.WindowTitle.ToString ();
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

		protected override void ExecuteQuit(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			if (!PopupStack.HasPopup)  // garde-fou, car on appelle cette méthode plusieurs fois !
			{
				var target = this.pseudoCloseButton;
				QuitPopup.Show (target, this.accessor, delegate (bool save)
				{
					if (save)
					{
						var path = System.IO.Path.Combine (this.accessor.ComputerSettings.MandatDirectory, this.accessor.ComputerSettings.MandatFilename);
						var err = AssetsApplication.SaveMandat (this.accessor, path, this.accessor.GlobalSettings.SaveMandatMode);

						if (!string.IsNullOrEmpty (err))
						{
							err = TextLayout.ConvertToTaggedText (err);
							MessagePopup.ShowError (target, err);
							return;
						}
					}

					this.accessor.ComputerSettings.WindowPlacement = this.Window.WindowPlacement;
					this.accessor.ComputerSettings.Serialize ();

					base.ExecuteQuit (dispatcher, e);
				});
			}
		}

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
			var computerSettings = new ComputerSettings ();

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
