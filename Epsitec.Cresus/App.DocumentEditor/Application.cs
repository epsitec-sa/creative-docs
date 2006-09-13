using Epsitec.Common.Document;
using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.App.DocumentEditor
{
	/// <summary>
	/// La classe Application démarre l'éditeur de documents.
	/// </summary>
	public class Application
	{
		public static void Start(string mode)
		{
			Application.mode = mode.Substring (0, 1);
			
			Res.Initialise(typeof(Application), "App");
			
			//	Il faut indiquer ci-après la date de diffusion du logiciel, qui doit
			//	être mise à jour chaque fois que l'on génère un nouveau CD :
			Common.Support.SerialAlgorithm.SetProductBuildDate(new System.DateTime(2005, 12, 31));
			Common.Support.SerialAlgorithm.SetProductGenerationNumber(1, 6);
			
			Widget.Initialise();
			Epsitec.Common.Document.Engine.Initialise();
			
			Common.Support.ImageProvider.Default.EnableLongLifeCache = true;
			Common.Support.ImageProvider.Default.PrefillManifestIconCache();

			if (mode.Length > 1)
			{
				switch (mode[1])
				{
					case 'p':
						Application.application = new Application(DocumentType.Pictogram);
						break;
				}
			}
			
			if (Application.application == null)
			{
				Application.application = new Application(DocumentType.Graphic);
				//Application.application = new Application(DocumentType.Pictogram);
			}
			
			Application.application.MainWindow.Run();
		}
		
		
		public Application(DocumentType type)
		{
			//?System.Threading.Thread.Sleep(60000);
			this.editor = new DocumentEditor(type);

			this.mainWindow = new Window();
			this.mainWindow.Root.WindowStyles = WindowStyles.CanResize |
												WindowStyles.CanMinimize |
												WindowStyles.CanMaximize |
												WindowStyles.HasCloseButton;

			//this.mainWindow.Root.SetClientZoom(2.0);
			this.mainWindow.Name = "Application";  // utilisé pour générer "QuitApplication" !
			this.mainWindow.PreventAutoClose = true;
			this.mainWindow.IsValidDropTarget = true;
			this.mainWindow.Icon = Bitmap.FromManifestResource("Epsitec.App.DocumentEditor.Images.Application.icon", this.GetType().Assembly);
			this.mainWindow.AsyncNotification += new EventHandler(this.HandleWindowAsyncNotification);
			this.mainWindow.WindowDragEntered += new WindowDragEventHandler(this.HandleMainWindowWindowDragEntered);
			this.mainWindow.WindowDragDropped += new WindowDragEventHandler(this.HandleMainWindowWindowDragDropped);
			
			if ( type == DocumentType.Graphic )
			{
				this.mainWindow.Root.MinSize = new Size(410, 250);
			}
			else
			{
				this.mainWindow.Root.MinSize = new Size(430, 250);
			}

			this.mainWindow.WindowBounds = this.editor.GlobalSettings.MainWindow;
			this.mainWindow.IsFullScreen = this.editor.GlobalSettings.IsFullScreen;

			switch ( type )
			{
				case DocumentType.Graphic:
					this.mainWindow.Text = Res.Strings.Application.TitleDoc;
					break;

				case DocumentType.Pictogram:
					this.mainWindow.Text = Res.Strings.Application.TitlePic;
					break;

				case DocumentType.Text:
					this.mainWindow.Text = Res.Strings.Application.TitleTxt;
					break;

				default:
					this.mainWindow.Text = Res.Strings.Application.TitleDoc;
					break;
			}
			
#if false
			this.menu = this.editor.GetMenu();
			this.menu.Dock   = DockStyle.Top;
			this.menu.Parent = this.mainWindow.Root;
#endif

			this.editor.PreferredSize = this.mainWindow.ClientSize;
			this.editor.Dock = DockStyle.Fill;
			this.editor.SetParent(this.mainWindow.Root);
			
			CommandDispatcher.SetDispatcher (this.mainWindow, this.editor.CommandDispatcher);
			CommandContext.SetContext (this.mainWindow, this.editor.CommandContext);

			this.mainWindow.Show();
			this.mainWindow.MakeActive();

			this.editor.MakeReadyToRun();
		}
		
		private void HandleWindowAsyncNotification(object sender)
		{
			this.editor.AsyncNotify();
		}


		public static string			Mode
		{
			get
			{
				return Application.mode;
			}
		}
		
		public DocumentType				Type
		{
			get { return this.editor.DocumentType; }
		}
		
		public Window					MainWindow
		{
			get { return this.mainWindow; }
		}
		
		public static Application		Current
		{
			get { return Application.application; }
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
				return ".crmod";
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
		
		private Window					mainWindow;
#if false
		private HMenu					menu;
#endif
		private DocumentEditor			editor;
	}
}
