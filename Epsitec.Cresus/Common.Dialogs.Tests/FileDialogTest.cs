using NUnit.Framework;

namespace Epsitec.Common.Dialogs
{
	[TestFixture] public class FileDialogTest
	{
		[Test] public void CheckFileOpen()
		{
			FileOpen dialog = new FileOpen ();
			
			dialog.Title = "CheckFileOpen";
			dialog.Filters.Add ("text", "Textes", "*.txt");
			dialog.Filters.Add ("image", "Images", "*.jpg;*.png;*.bmp");
			dialog.Filters.Add ("any", "Tous les fichiers", "*.*");
			
			dialog.Show ();
			
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
			
			dialog.Show ();
			
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
			
			dialog.Show ();
			
			System.Console.Out.WriteLine ("Name: {0}", dialog.FileName);
			System.Console.Out.WriteLine ("Filter: {0} -> {1}", dialog.FilterIndex, dialog.Filters[dialog.FilterIndex].Name);
			System.Console.Out.WriteLine ("Initial directory: {0}", dialog.InitialDirectory);
		}
	}
}
