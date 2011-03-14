using Epsitec.Common.Document;
using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.DocumentEditor
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
			
			Epsitec.Common.Document.Engine.Initialize();
			
			Common.Support.ImageProvider.Default.EnableLongLifeCache = true;
			Common.Support.ImageProvider.Default.PrefillManifestIconCache();

			switch (mode.Substring (1))
			{
				case "p":
					Application.application = new Application(DocumentType.Pictogram);
					break;
				default:
					Application.application = new Application(DocumentType.Graphic);
					break;
			}
			
			Application.application.Window.Run();
		}
		
		
		public Application(DocumentType type)
		{
			Window window = new Window ();

			this.editor = new DocumentEditor (type, this.CommandDispatcher, this.CommandContext, window);

			this.Window = window;

			window.IsValidDropTarget = true;
			window.Icon = Bitmap.FromManifestResource ("Epsitec.Common.DocumentEditor.Images.Application.icon", typeof (Application).Assembly);
			window.AsyncNotification += this.HandleWindowAsyncNotification;
			window.WindowDragEntered += this.HandleMainWindowWindowDragEntered;
			window.WindowDragDropped += this.HandleMainWindowWindowDragDropped;
			
			if ( type == DocumentType.Graphic )
			{
				window.Root.MinSize = new Size(410, 250);
			}
			else
			{
				window.Root.MinSize = new Size(430, 250);
			}

			window.WindowBounds = this.editor.GlobalSettings.MainWindowBounds;
			window.IsFullScreen = this.editor.GlobalSettings.IsFullScreen;

			switch ( type )
			{
				case DocumentType.Graphic:
					window.Text = Res.Strings.Application.TitleDoc;
					break;

				case DocumentType.Pictogram:
					window.Text = Res.Strings.Application.TitlePic;
					break;

				case DocumentType.Text:
					window.Text = Res.Strings.Application.TitleTxt;
					break;

				default:
					window.Text = Res.Strings.Application.TitleDoc;
					break;
			}
			
			this.editor.PreferredSize = window.ClientSize;
			this.editor.Dock = DockStyle.Fill;
			this.editor.SetParent(window.Root);
			
			window.Show();
			window.MakeActive();

			this.editor.MakeReadyToRun();
		}
		
		private void HandleWindowAsyncNotification(object sender)
		{
			this.editor.AsyncNotify();
		}

		public override string ShortWindowTitle
		{
			get
			{
				return this.editor.ShortWindowTitle;
			}
		}

		public override string ApplicationIdentifier
		{
			get
			{
				return "EpDocumentEditor";
			}
		}

		public static string					Mode
		{
			get
			{
				return Application.mode;
			}
		}

		public DocumentType						Type
		{
			get
			{
				return this.editor.DocumentType;
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
			//	Evite que cette commande ne soit exécutée par Widgets.Application,
			//	car cela provoquerait la fin du programme, quelle que soit la
			//	réponse donnée par l'utilisateur au dialogue affiché par DocumentEditor.
		}

		private void HandleMainWindowWindowDragEntered(object sender, WindowDragEventArgs e)
		{
			string[] files = e.Data.ReadAsStringArray("FileDrop");
			if ( files == null )  return;
			e.AcceptDrop = false;
			
			if ( files.Length > 0 )
			{
				foreach ( string file in files )
				{
					if ( !Misc.IsExtension(file, this.DefaultExtension) &&
						 !Misc.IsExtension(file, this.ModelExtension)   &&
						 !Misc.IsExtension(file, this.ColorsExtension)  )
					{
						return;
					}
				}
				
				e.AcceptDrop = true;
			}
		}

		private void HandleMainWindowWindowDragDropped(object sender, WindowDragEventArgs e)
		{
			string[] files = e.Data.ReadAsStringArray("FileDrop");
			if ( files == null )  return;
			e.AcceptDrop = false;
			
			if ( files.Length > 0 )
			{
				foreach ( string file in files )
				{
					if ( !Misc.IsExtension(file, this.DefaultExtension) &&
						 !Misc.IsExtension(file, this.ModelExtension)   &&
						 !Misc.IsExtension(file, this.ColorsExtension)  )
					{
						return;
					}
				}
				
				e.AcceptDrop = true;
				
				foreach ( string file in files )
				{
					this.editor.Open(file);
				}
			}
		}

		protected string DefaultExtension
		{
			get
			{
				if ( this.editor.DocumentType == DocumentType.Pictogram )
				{
					return ".icon";
				}
				else
				{
					return ".crdoc";
				}
			}
		}

		protected string ModelExtension
		{
			get
			{
				if (this.editor.DocumentType == DocumentType.Pictogram)
				{
					return ".iconmod";
				}
				else
				{
					return ".crmod";
				}
			}
		}

		protected string ColorsExtension
		{
			get
			{
				return ".crcolors";
			}
		}

		
		private static Application		application;
		private static string			mode;
		
		private DocumentEditor			editor;
	}
}
