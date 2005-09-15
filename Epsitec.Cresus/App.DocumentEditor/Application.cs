using Epsitec.Common.Document;
using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.App.DocumentEditor
{
	/// <summary>
	/// La classe Application d�marre l'�diteur de documents.
	/// </summary>
	public class Application
	{
		public static void Start(string mode) 
		{
			Application.mode = mode;
			
			Res.Initialise(typeof(Application), "App");
			
			// Il faut indiquer ci-apr�s la date de diffusion du logiciel, qui doit
			// �tre mise � jour chaque fois que l'on g�n�re un nouveau CD :
			Common.Support.SerialAlgorithm.SetProductBuildDate(new System.DateTime(2005, 1, 20));
			Common.Support.SerialAlgorithm.SetProductGenerationNumber(1, 0);
			
			Widget.Initialise();
			
			Common.Support.ImageProvider.Default.EnableLongLifeCache = true;
			Common.Support.ImageProvider.Default.PrefillManifestIconCache();

			Epsitec.Common.Document.Engine.Initialise();
			//Application.application = new Application(DocumentType.Pictogram);
			Application.application = new Application(DocumentType.Graphic);
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
			this.mainWindow.Name = "Application";  // utilis� pour g�n�rer "QuitApplication" !
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
			
			this.menu = this.editor.GetMenu();
			this.menu.Dock   = DockStyle.Top;
			this.menu.Parent = this.mainWindow.Root;
			
			this.editor.Size   = this.mainWindow.ClientSize;
			this.editor.Dock   = DockStyle.Fill;
			this.editor.Parent = this.mainWindow.Root;
			
			this.mainWindow.CommandDispatcher = this.editor.CommandDispatcher;
			this.mainWindow.Show();
			this.mainWindow.MakeActive();

			this.editor.Finalize();
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
			get { return this.editor.Type; }
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
					if ( !file.ToLower().EndsWith(this.DefaultExtension) &&
						 !file.ToLower().EndsWith(this.ModelExtension)   &&
						 !file.ToLower().EndsWith(this.ColorsExtension)  )
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
					if ( !file.ToLower().EndsWith(this.DefaultExtension) &&
						 !file.ToLower().EndsWith(this.ModelExtension)   &&
						 !file.ToLower().EndsWith(this.ColorsExtension)  )
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
				if ( this.editor.Type == DocumentType.Pictogram )
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
		private HMenu					menu;
		private DocumentEditor			editor;
	}
}
