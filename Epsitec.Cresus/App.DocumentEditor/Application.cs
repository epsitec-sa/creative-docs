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
		#region Application Startup
		[System.STAThread]
		static void Main() 
		{
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
		#endregion
		
		public Application(DocumentType type)
		{
			//?System.Threading.Thread.Sleep(60000);
			this.editor = new DocumentEditor(type);

			this.mainWindow = new Window();
			this.mainWindow.Root.WindowStyles = WindowStyles.CanResize |
												WindowStyles.CanMinimize |
												WindowStyles.CanMaximize |
												WindowStyles.HasCloseButton;

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
					this.mainWindow.Text = "Cr�sus Documents";
					break;

				case DocumentType.Pictogram:
					this.mainWindow.Text = "Cr�sus Pictogrammes";
					break;

				case DocumentType.Text:
					this.mainWindow.Text = "Cr�sus Texte";
					break;

				default:
					this.mainWindow.Text = "Cr�sus Documents";
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

			this.editor.Finalize();
		}
		
		private void HandleWindowAsyncNotification(object sender)
		{
			this.editor.AsyncNotify();
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
						 !file.ToLower().EndsWith(this.ColorsExtension) )
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
						 !file.ToLower().EndsWith(this.ColorsExtension) )
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

		protected string ColorsExtension
		{
			get
			{
				return ".crcolors";
			}
		}

		
		private static Application		application;
		
		private Window					mainWindow;
		private HMenu					menu;
		private DocumentEditor			editor;
	}
}
