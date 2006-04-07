using NUnit.Framework;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;

namespace Epsitec.Common.Dialogs
{
	[TestFixture] public class FileDialogTest
	{
		[Test]
		public void AutomatedTestEnvironment()
		{
			Epsitec.Common.Widgets.Window.RunningInAutomatedTestEnvironment = true;
		}

		[Test]
		public void CheckApplication()
		{
			Epsitec.Common.Document.Engine.Initialise();

			Window window = new Window();
			
			window.ClientSize = new Size(400, 300);
			window.Text = "CheckApplication";

			Button button1 = new Button();
			button1.Text = "Ouvrir";
			button1.Margins = new Margins (50, 0, 0, 50);
			button1.Anchor = AnchorStyles.BottomLeft;
			button1.Width = 100;
			button1.Clicked += new MessageEventHandler(this.button1_Clicked);
			window.Root.Children.Add(button1);

			Button button2 = new Button();
			button2.Text = "Enregistrer";
			button2.Margins = new Margins (160, 0, 0, 50);
			button2.Anchor = AnchorStyles.BottomLeft;
			button2.Width = 100;
			button2.Clicked += new MessageEventHandler(this.button2_Clicked);
			window.Root.Children.Add(button2);

			IconButton button3 = new IconButton();
			button3.IconName = @"file:images/open.icon";
			button3.Margins = new Margins (50, 0, 0, 100);
			button3.Anchor = AnchorStyles.BottomLeft;
			button3.Clicked += new MessageEventHandler(this.button1_Clicked);
			window.Root.Children.Add(button3);

			IconButton button4 = new IconButton();
			button4.IconName = @"file:images/save.icon";
			button4.Margins = new Margins (80, 0, 0, 100);
			button4.Anchor = AnchorStyles.BottomLeft;
			button4.Clicked += new MessageEventHandler(this.button2_Clicked);
			window.Root.Children.Add(button4);

			window.Show();
			
			this.app_window = window;
			Window.RunInTestEnvironment (window);
		}

		private void button1_Clicked(object sender, MessageEventArgs e)
		{
			FileOpen dialog = new FileOpen ();
			
			dialog.Title = "CheckFileOpen";
			dialog.Filters.Add ("text", "Textes", "*.txt");
			dialog.Filters.Add ("image", "Images", "*.jpg;*.png;*.bmp");
			dialog.Filters.Add ("any", "Tous les fichiers", "*.*");
			dialog.Owner = this.app_window;
			dialog.OpenDialog ();
		}

		private void button2_Clicked(object sender, MessageEventArgs e)
		{
			FileSave dialog = new FileSave ();
			
			dialog.Title = "CheckFileSave";
			dialog.Filters.Add ("text", "Textes", "*.txt");
			dialog.Filters.Add ("image", "Images", "*.jpg;*.png;*.bmp");
			dialog.Filters.Add ("any", "Tous les fichiers", "*.*");
			dialog.Owner = this.app_window;
			dialog.OpenDialog ();
		}

		[Test] public void CheckFileOpen()
		{
			FileOpen dialog = new FileOpen ();
			
			dialog.Title = "CheckFileOpen";
			dialog.Filters.Add ("text", "Textes", "*.txt");
			dialog.Filters.Add ("image", "Images", "*.jpg;*.png;*.bmp");
			dialog.Filters.Add ("any", "Tous les fichiers", "*.*");
			dialog.Owner = this.app_window;
			dialog.OpenDialog ();
			
			System.Console.Out.WriteLine ("Name: {0}", dialog.FileName);
			System.Console.Out.WriteLine ("Filter: {0} -> {1}", dialog.FilterIndex, dialog.Filters[dialog.FilterIndex].Name);
			System.Console.Out.WriteLine ("Initial directory: {0}", dialog.InitialDirectory);
		}
		
		[Test] public void CheckFileSave1()
		{
			FileSave dialog = new FileSave ();
			
			dialog.Title = "CheckFileSave1";
			dialog.Filters.Add ("text", "Textes", "*.txt");
			dialog.Filters.Add ("image", "Images", "*.jpg;*.png;*.bmp");
			dialog.Filters.Add ("any", "Tous les fichiers", "*.*");
			dialog.Owner = this.app_window;
			dialog.OpenDialog ();
			
			System.Console.Out.WriteLine ("Name: {0}", dialog.FileName);
			System.Console.Out.WriteLine ("Filter: {0} -> {1}", dialog.FilterIndex, dialog.Filters[dialog.FilterIndex].Name);
			System.Console.Out.WriteLine ("Initial directory: {0}", dialog.InitialDirectory);
		}
		
		[Test] public void CheckFileSave2()
		{
			FileSave dialog = new FileSave ();
			
			dialog.Title = "CheckFileSave2";
			dialog.Filters.Add ("text", "Textes", "*.txt");
			dialog.Filters.Add ("image", "Images", "*.jpg;*.png;*.bmp");
			dialog.Filters.Add ("any", "Tous les fichiers", "*.*");
			dialog.PromptForCreation = true;
			dialog.PromptForOverwriting = true;
			dialog.Owner = this.app_window;
			dialog.OpenDialog ();
			
			System.Console.Out.WriteLine ("Name: {0}", dialog.FileName);
			System.Console.Out.WriteLine ("Filter: {0} -> {1}", dialog.FilterIndex, dialog.Filters[dialog.FilterIndex].Name);
			System.Console.Out.WriteLine ("Initial directory: {0}", dialog.InitialDirectory);
		}
		
		private Window					app_window;
	}
}
