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
		#region Application Startup
		[System.STAThread]
		static void Main() 
		{
			Widget.Initialise();
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

			Epsitec.Common.Widgets.Adorner.Factory.SetActive(this.editor.GlobalSettings.Adorner);

			this.mainWindow = new Window();
			this.mainWindow.Root.WindowStyles = WindowStyles.CanResize |
												WindowStyles.CanMinimize |
												WindowStyles.CanMaximize |
												WindowStyles.HasCloseButton;

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

			if ( this.editor.GlobalSettings.WindowLocation.IsEmpty )
			{
				ScreenInfo si = ScreenInfo.Find(new Point(0,0));
				Rectangle wa = si.WorkingArea;
				if ( this.editor.GlobalSettings.WindowSize.Width  > wa.Width  ||
					 this.editor.GlobalSettings.WindowSize.Height > wa.Height )
				{
					this.editor.GlobalSettings.WindowSize = wa.Size;
					this.editor.GlobalSettings.IsFullScreen = true;
				}
				this.editor.GlobalSettings.WindowLocation = wa.Center-this.editor.GlobalSettings.WindowSize/2;
			}
			this.mainWindow.WindowLocation = this.editor.GlobalSettings.WindowLocation;
			this.mainWindow.WindowSize     = this.editor.GlobalSettings.WindowSize;
			this.mainWindow.IsFullScreen   = this.editor.GlobalSettings.IsFullScreen;
			this.mainWindow.Show();

			switch ( type )
			{
				case DocumentType.Graphic:
					this.mainWindow.Text = "Crésus Documents";
					break;

				case DocumentType.Pictogram:
					this.mainWindow.Text = "Crésus Pictogrammes";
					break;

				case DocumentType.Text:
					this.mainWindow.Text = "Crésus Texte";
					break;

				default:
					this.mainWindow.Text = "Crésus Documents";
					break;
			}
			
			this.menu = this.editor.GetMenu();

			this.menu.Dock     = DockStyle.Top;
			this.menu.Parent   = this.mainWindow.Root;
			
			this.editor.Size   = this.mainWindow.ClientSize;
			this.editor.Dock   = DockStyle.Fill;
			this.editor.Parent = this.mainWindow.Root;
			
			this.mainWindow.CommandDispatcher = this.editor.CommandDispatcher;
			this.mainWindow.Show();
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
			e.AcceptDrop = false;
			
			if ( files.Length > 0 )
			{
				foreach ( string file in files )
				{
					if ( !file.ToLower().EndsWith(this.DefaultExtension) )
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
			e.AcceptDrop = false;
			
			if ( files.Length > 0 )
			{
				foreach ( string file in files )
				{
					if ( !file.ToLower().EndsWith(this.DefaultExtension) )
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

		
		private static Application		application;
		
		private Window					mainWindow;
		private HMenu					menu;
		private DocumentEditor			editor;
	}
}
