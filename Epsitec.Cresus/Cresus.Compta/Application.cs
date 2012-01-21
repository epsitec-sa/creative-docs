//	Copyright © 2011-2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

using Epsitec.Cresus.Compta.Controllers;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta
{
	/// <summary>
	/// La classe Application démarre l'éditeur de documents.
	/// </summary>
	public class Application : Epsitec.Common.Widgets.Application
	{
		public static void Start(string mode)
		{
			System.Diagnostics.Debug.Assert (mode != null);
			System.Diagnostics.Debug.Assert (mode.Length > 0);

			Epsitec.Cresus.Core.Library.UI.Services.Initialize ();

			//	Evite de créer la liste des fontes complète maintenant si elle n'existe
			//	pas, puisque l'on veut tout d'abord être capable d'afficher le splash:
			Epsitec.Common.Text.TextContext.PostponeFullFontCollectionInitialization ();

			Widget.Initialize ();
			Application.mode = mode.Substring (0, 1);
			
			Res.Initialize();
			
			//	Il faut indiquer ci-après la date de diffusion du logiciel, qui doit
			//	être mise à jour chaque fois que l'on génère un nouveau CD :
			Common.Support.SerialAlgorithm.SetProductBuildDate(new System.DateTime(2010, 01, 04));
			Common.Support.SerialAlgorithm.SetProductGenerationNumber(74, 6);	// Accepte CDID = 74
			
			Common.Support.ImageProvider.Default.EnableLongLifeCache = true;
			Common.Support.ImageProvider.Default.PrefillManifestIconCache();

			Application.application = new Application();
			
			Application.application.Window.Run();
		}
		
		
		public Application()
		{
			Epsitec.Cresus.Core.Library.UI.Services.SetApplication (this);

			Window window = new Window ();

			//?this.editor = new DocumentEditor (type, this.CommandDispatcher, this.CommandContext, window);

			this.Window = window;

			window.Icon = Bitmap.FromManifestResource ("Epsitec.Common.DocumentEditor.Images.Application.icon", typeof (Application).Assembly);
			window.Root.MinSize = new Size(640, 480);
			window.WindowBounds = new Rectangle (100, 100, 800, 600);
			//?window.WindowBounds = this.editor.GlobalSettings.MainWindowBounds;
			//?window.IsFullScreen = this.editor.GlobalSettings.IsFullScreen;
			window.Text = "Crésus MCH-2";
			window.WindowCloseClicked += new EventHandler (this.HandleWindowCloseClicked);
			
			//?this.editor.PreferredSize = window.ClientSize;
			//?this.editor.Dock = DockStyle.Fill;
			//?this.editor.SetParent(window.Root);

			this.windowController = new WindowController (this);
			this.windowController.CreateUI (window);

			window.Show ();
			window.MakeActive();

			//?this.editor.MakeReadyToRun();
		}

		private void HandleWindowCloseClicked(object sender)
		{
			this.Window.Close ();
			// TODO: Comment quitter ?
		}
		
		public override string ShortWindowTitle
		{
			get
			{
				return "Crésus MCH-2";
			}
		}

		public override string ApplicationIdentifier
		{
			get
			{
				return "Cr.MCH-2";
			}
		}

		public static string					Mode
		{
			get
			{
				return Application.mode;
			}
		}

		public static Application				Current
		{
			get
			{
				return Application.application;
			}
		}


		protected override void ExecuteQuit(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			System.Diagnostics.Debug.WriteLine ("Application: Quit executed");
			e.Executed = true;
			Window.Quit ();
		}


		
		private static Application		application;
		private static string			mode;
		
		//?private DocumentEditor			editor;
		private WindowController		windowController;
	}
}
