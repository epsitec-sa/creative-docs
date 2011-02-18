//	Copyright © 2003-2008, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using NUnit.Framework;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;

namespace Epsitec.Common.Dialogs
{
	[TestFixture] public class FileDialogTest
	{
		[SetUp]
		public void Initialize()
		{
			Assert.AreEqual (System.Threading.ApartmentState.STA, System.Threading.Thread.CurrentThread.GetApartmentState ());
		}

		[Test]
		public void AutomatedTestEnvironment()
		{
			Epsitec.Common.Widgets.Window.RunningInAutomatedTestEnvironment = true;
		}

		[Test]
		public void CheckApplication()
		{
			Epsitec.Common.Document.Engine.Initialize();

			Window window = new Window();
			
			window.ClientSize = new Size(400, 300);
			window.Text = "CheckApplication";

			Button button1 = new Button();
			button1.Text = "Ouvrir";
			button1.Margins = new Margins (50, 0, 0, 50);
			button1.Anchor = AnchorStyles.BottomLeft;
			button1.PreferredWidth = 100;
			button1.Clicked += this.button1_Clicked;
			window.Root.Children.Add(button1);

			Button button2 = new Button();
			button2.Text = "Enregistrer";
			button2.Margins = new Margins (160, 0, 0, 50);
			button2.Anchor = AnchorStyles.BottomLeft;
			button2.PreferredWidth = 100;
			button2.Clicked += this.button2_Clicked;
			window.Root.Children.Add(button2);

			IconButton button3 = new IconButton();
			button3.IconUri = @"file:images/open.icon";
			button3.Margins = new Margins (50, 0, 0, 100);
			button3.Anchor = AnchorStyles.BottomLeft;
			button3.Clicked += this.button1_Clicked;
			window.Root.Children.Add(button3);

			IconButton button4 = new IconButton();
			button4.IconUri = @"file:images/save.icon";
			button4.Margins = new Margins (80, 0, 0, 100);
			button4.Anchor = AnchorStyles.BottomLeft;
			button4.Clicked += this.button2_Clicked;
			window.Root.Children.Add(button4);

			window.Show();
			
			this.app_window = window;
			Window.RunInTestEnvironment (window);
		}

		private void button1_Clicked(object sender, MessageEventArgs e)
		{
			FileOpenDialog dialog = new FileOpenDialog ();
			
			dialog.Title = "CheckFileOpen";
			dialog.Filters.Add ("text", "Textes", "*.txt");
			dialog.Filters.Add ("image", "Images", "*.jpg;*.png;*.bmp");
			dialog.Filters.Add ("any", "Tous les fichiers", "*.*");
			dialog.OwnerWindow = this.app_window;
			dialog.OpenDialog ();
		}

		private void button2_Clicked(object sender, MessageEventArgs e)
		{
			FileSaveDialog dialog = new FileSaveDialog ();
			
			dialog.Title = "CheckFileSave";
			dialog.Filters.Add ("text", "Textes", "*.txt");
			dialog.Filters.Add ("image", "Images", "*.jpg;*.png;*.bmp");
			dialog.Filters.Add ("any", "Tous les fichiers", "*.*");
			dialog.OwnerWindow = this.app_window;
			dialog.OpenDialog ();
		}

		[Test] public void CheckFileOpen()
		{
			FileOpenDialog dialog = new FileOpenDialog ();

			using (Tool.InjectKey (System.Windows.Forms.Keys.Escape))
			{
				dialog.Title = "CheckFileOpen";
				dialog.Filters.Add ("text", "Textes", "*.txt");
				dialog.Filters.Add ("image", "Images", "*.jpg;*.png;*.bmp");
				dialog.Filters.Add ("any", "Tous les fichiers", "*.*");
				dialog.OwnerWindow = this.app_window;
				dialog.OpenDialog ();

				System.Console.Out.WriteLine ("Name: {0}", dialog.FileName);
				System.Console.Out.WriteLine ("Filter: {0} -> {1}", dialog.FilterIndex, dialog.Filters[dialog.FilterIndex].Name);
				System.Console.Out.WriteLine ("Initial directory: {0}", dialog.InitialDirectory);
			}
		}

		[Test] public void CheckFileSave1()
		{
			FileSaveDialog dialog = new FileSaveDialog ();

			using (Tool.InjectKey (System.Windows.Forms.Keys.Escape))
			{
				dialog.Title = "CheckFileSave1";
				dialog.Filters.Add ("text", "Textes", "*.txt");
				dialog.Filters.Add ("image", "Images", "*.jpg;*.png;*.bmp");
				dialog.Filters.Add ("any", "Tous les fichiers", "*.*");
				dialog.OwnerWindow = this.app_window;
				dialog.OpenDialog ();

				System.Console.Out.WriteLine ("Name: {0}", dialog.FileName);
				System.Console.Out.WriteLine ("Filter: {0} -> {1}", dialog.FilterIndex, dialog.Filters[dialog.FilterIndex].Name);
				System.Console.Out.WriteLine ("Initial directory: {0}", dialog.InitialDirectory);
			}
		}
		
		[Test] public void CheckFileSave2()
		{
			FileSaveDialog dialog = new FileSaveDialog ();

			using (Tool.InjectKey (System.Windows.Forms.Keys.Escape))
			{
				dialog.Title = "CheckFileSave2";
				dialog.Filters.Add ("text", "Textes", "*.txt");
				dialog.Filters.Add ("image", "Images", "*.jpg;*.png;*.bmp");
				dialog.Filters.Add ("any", "Tous les fichiers", "*.*");
				dialog.PromptForCreation = true;
				dialog.PromptForOverwriting = true;
				dialog.OwnerWindow = this.app_window;
				dialog.OpenDialog ();

				System.Console.Out.WriteLine ("Name: {0}", dialog.FileName);
				System.Console.Out.WriteLine ("Filter: {0} -> {1}", dialog.FilterIndex, dialog.Filters[dialog.FilterIndex].Name);
				System.Console.Out.WriteLine ("Initial directory: {0}", dialog.InitialDirectory);
			}
		}

		private Window					app_window;
	}
}
