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
			Epsitec.Common.Pictogram.Engine.Initialise();
			Epsitec.Common.Document.Engine.Initialise();
			Epsitec.Common.UI.Engine.Initialise();
			Application.application = new Application(DocumentType.Pictogram);
			//Application.application = new Application(DocumentType.Graphic);
			Application.application.MainWindow.Run();
		}
		#endregion
		
		public Application(DocumentType type)
		{
			//?System.Threading.Thread.Sleep(60000);
			Epsitec.Common.Widgets.Adorner.Factory.SetActive("LookMetal");

			this.mainWindow = new Window();
			this.mainWindow.Root.WindowStyles = WindowStyles.CanResize |
												WindowStyles.CanMinimize |
												WindowStyles.CanMaximize |
												WindowStyles.HasCloseButton;

			this.mainWindow.Name = "Application";  // utilisé pour générer "QuitApplication" !
			this.mainWindow.PreventAutoClose = true;
			this.mainWindow.AsyncNotification += new EventHandler(this.HandleWindowAsyncNotification);
			
			if ( type == DocumentType.Graphic )
			{
				this.mainWindow.ClientSize = new Size(830, 580);
			}
			else
			{
				this.mainWindow.ClientSize = new Size(830, 580);
			}

			switch ( type )
			{
				case DocumentType.Graphic:
					this.mainWindow.Text = "Crésus dessin";
					break;

				case DocumentType.Pictogram:
					this.mainWindow.Text = "Crésus pictogramme";
					break;

				case DocumentType.Text:
					this.mainWindow.Text = "Crésus texte";
					break;

				default:
					this.mainWindow.Text = "Crésus document";
					break;
			}
			
			this.editor = new DocumentEditor(type);
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

		
		private static Application		application;
		
		private Window					mainWindow;
		private HMenu					menu;
		private DocumentEditor			editor;
	}
}
