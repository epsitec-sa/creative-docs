using System;
using System.Text;
using System.Collections.Generic;
using Epsitec.Cresus.Database;
using Epsitec.Cresus.DataLayer;
using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Types;
using Epsitec.Common.Drawing;

namespace App.Tester
{
	/// <summary>
	/// Fenêtre principale de l'éditeur de ressources.
	/// </summary>
	public class MainWindow : DependencyObject
	{
		public MainWindow()
		{
		}

		public void Show(Window parentWindow)
		{
			//	Crée et montre la fenêtre de l'éditeur.
			if (this.window == null)
			{
				this.window = new Window ();

				this.window.Root.WindowStyles = WindowStyles.DefaultDocumentWindow;

/*				Point parentCenter;
				Rectangle windowBounds;

				if (parentWindow == null)
				{
					Rectangle area = ScreenInfo.GlobalArea;
					parentCenter = area.Center;
				}
				else
				{
					parentCenter = parentWindow.WindowBounds.Center;
				}

				windowBounds = new Rectangle (parentCenter.X-1000/2, parentCenter.Y-700/2, 1000, 700);
				windowBounds = ScreenInfo.FitIntoWorkingArea (windowBounds);

				this.window.WindowBounds = windowBounds;*/
				this.window.ClientSize = new Size(1024, 768);
				this.window.Root.MinSize = new Size (500, 400);
				this.window.Text = "Tester";
				this.window.Name = "Application";  // utilisé pour générer "QuitApplication" !
				this.window.PreventAutoClose = true;
				this.window.WindowClosed += new Epsitec.Common.Support.EventHandler(this.HandleWindowClosed);

				MainWindow.SetInstance (this.window, this);  // attache l'instance de MainWindow à la fenêtre
				this.CreateLayout ();
			}
			this.window.Show ();
		}

		private void CreateLayout()
		{

		}

		private void HandleWindowClosed(object sender)
		{
		}

		public static void SetInstance(DependencyObject obj, MainWindow value)
		{
			obj.SetValue (MainWindow.InstanceProperty, value);
		}

		public Window Window
		{
			//	Retourne la fenêtre principale de l'application.
			get
			{
				return this.window;
			}
		}

		/// <summary>
		/// Creates and initializes the database for the AppTester program: a simple database with one table
		/// containing a few string fields
		/// </summary>
		public bool InitDb()
		{
			// Creates the StructuredType serving as model for the only table
			Caption caption = new Caption (Druid.Parse ("[9000]"));

			//			caption.Name = "CONTACTMAIN";
			StructuredType strucType = new StructuredType (caption);
			strucType.Fields.Add (new StructuredTypeField (null, StringType.Default, Druid.Parse ("[9001]"), 0));
			strucType.Fields.Add (new StructuredTypeField (null, StringType.Default, Druid.Parse ("[9002]"), 1));
			strucType.Fields.Add (new StructuredTypeField (null, StringType.Default, Druid.Parse ("[9003]"), 2));
			strucType.Fields.Add (new StructuredTypeField (null, StringType.Default, Druid.Parse ("[9004]"), 3));
			strucType.Fields.Add (new StructuredTypeField (null, StringType.Default, Druid.Parse ("[9005]"), 4));
			strucType.Fields.Add (new StructuredTypeField (null, StringType.Default, Druid.Parse ("[9006]"), 5));
			strucType.Fields.Add (new StructuredTypeField (null, StringType.Default, Druid.Parse ("[9007]"), 6));

			StructuredTypeField strucTypeField;
			if (strucType.FindFieldByRank (3, out strucTypeField))
			{
				System.Diagnostics.Debug.WriteLine ("TTT Name of field 3: " + strucTypeField.Id);
			}
			else
			{
				System.Diagnostics.Debug.WriteLine ("Field not found");
			}

			// Creates the database
			try
			{
				System.IO.File.Delete (@"C:\Program Files\firebird\Data\Epsitec\APPTESTER.FIREBIRD");
			}
			catch (System.IO.IOException ex)
			{
				System.Console.Out.WriteLine ("Cannot delete database file. Error message :\n{0}\nWaiting for 5 seconds...", ex.ToString ());
				System.Threading.Thread.Sleep (5000);

				try
				{
					System.IO.File.Delete (@"C:\Program Files\firebird\Data\Epsitec\APPTESTER.FIREBIRD");
				}
				catch
				{
					return false;
				}
			}

			using (DbInfrastructure infrastructure = new DbInfrastructure ())
			{
				DbAccess db_access = DbInfrastructure.CreateDatabaseAccess ("APPTESTER");
				infrastructure.CreateDatabase (db_access);

				ResourceManager resMan = new ResourceManager (@"S:\Epsitec.Cresus\App.Tester\Resources\App.Tester");
				infrastructure.DefaultContext.ResourceManager = resMan;

				DbRichCommand command;
				using (DbTransaction transaction = infrastructure.BeginTransaction ())
				{
					DbTable dbTable1 = Adapter.CreateTableDefinition (transaction, strucType);
					transaction.Commit ();
				}

			}
			return true;
		}

		protected Window window;
		public static readonly DependencyProperty InstanceProperty = DependencyProperty.RegisterAttached ("Instance", typeof (MainWindow), typeof (MainWindow));
	}
}
