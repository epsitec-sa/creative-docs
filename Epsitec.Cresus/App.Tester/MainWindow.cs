using System.Collections.Generic;
using System.Data;
using System.Text;
using Epsitec.Common.Drawing;
using Epsitec.Common.Support;
using Epsitec.Common.Types;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Database;
using Epsitec.Cresus.DataLayer;
using Epsitec.Cresus.UI;

namespace App.Tester
{
	/// <summary>
	/// Fenêtre principale de l'éditeur de l'application de test.
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
				Rectangle windowBounds;
				Rectangle area = ScreenInfo.GlobalArea;
				windowBounds = new Rectangle (0, area.Height - 700, 1400, 700);
				windowBounds = ScreenInfo.FitIntoWorkingArea (windowBounds);
				this.window.WindowBounds = windowBounds;
//				this.window.ClientSize = new Size (1380, 700);
				this.window.Root.MinSize = new Size (500, 400);
				this.window.Text = "Tester";
				this.window.Name = "Application";  // utilisé pour générer "QuitApplication" !
				this.window.PreventAutoClose = true;
				this.window.WindowClosed += new EventHandler(this.HandleWindowClosed);

				MainWindow.SetInstance (this.window, this);  // attache l'instance de MainWindow à la fenêtre
				this.CreateLayout ();
			}
			this.leftList = new LeftList (this.broker, this.tableNameContactMain, this.paneLeftList);
			this.leftList.SelectedChanged += new EventHandler (this.HandleLeftListSelectedChanged);
			this.leftListRecordSelected = System.Math.Max(this.leftList.RecordRank, 0);

			this.window.Show ();
		}

		private void HandleLeftListSelectedChanged(object sender)
		{
			this.leftListRecordSelected = System.Math.Max (this.leftList.RecordRank, 0)
			;
			bool test = true;
		}

		private void CreateLayout()
		{
			Widget root = new Panel ();
			//			root.SetClientAngle(0);
			//			root.SetClientZoom(1.0);
			//-			root.Location = new Point(0, 0);
			//-			root.Size = new Size(rect.Width, rect.Height-this.menu.DefaultHeight-this.toolBar.DefaultHeight);
			root.Dock = DockStyle.Fill;
			root.SetParent (this.window.Root);

			this.mainPaneBook = new PaneBook ();
			//-			this.mainPaneBook.Location = new Point(0, 0);
			//-			this.mainPaneBook.Size = root.Size;
			this.mainPaneBook.PaneBookStyle = PaneBookStyle.LeftRight;
			this.mainPaneBook.PaneBehaviour = PaneBookBehaviour.Draft;
			//this.mainPaneBook.PaneBehaviour = PaneBookBehaviour.FollowMe;
			this.mainPaneBook.Dock = DockStyle.Fill;
			this.mainPaneBook.PaneSizeChanged += new EventHandler (this.HandlePaneSizeChanged);
			this.mainPaneBook.SetParent (root);

			this.paneLeftList = new PanePage ();
			this.paneLeftList.PaneRelativeSize = 5;
			this.paneLeftList.PaneElasticity = 1;
			this.paneLeftList.PaneHideSize = 50;
			this.mainPaneBook.Items.Add (this.paneLeftList);

			this.paneEdit = new PanePage ();
			this.paneEdit.PaneRelativeSize = 10;
			this.paneEdit.PaneElasticity = 1;
			this.paneEdit.PaneHideSize = 100;
			this.mainPaneBook.Items.Add (this.paneEdit);

			this.paneSettingsDb = new PanePage ();
			this.paneSettingsDb.PaneRelativeSize = 10;
			this.paneSettingsDb.PaneElasticity = 1;
			this.paneSettingsDb.PaneHideSize = 100;
			this.mainPaneBook.Items.Add (this.paneSettingsDb);

			this.paneSettingsType = new PanePage ();
			this.paneSettingsType.PaneRelativeSize = 10;
			this.paneSettingsType.PaneElasticity = 1;
			this.paneSettingsType.PaneHideSize = 100;
			this.mainPaneBook.Items.Add (this.paneSettingsType);

			this.paneSettingsTab = new PanePage ();
			this.paneSettingsTab.PaneRelativeSize = 10;
			this.paneSettingsTab.PaneElasticity = 1;
			this.paneSettingsTab.PaneHideSize = 100;
			this.mainPaneBook.Items.Add (this.paneSettingsTab);
		}

		private void HandlePaneSizeChanged(object sender)
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
			this.tableNameContactMain = "CONTACTMAIN";

			//			caption.Name = "CONTACTMAIN";
			StructuredType strucType = new StructuredType (caption); // (STructuredTypeClass.entity), .defineCaption
			strucType.Fields.Add (new StructuredTypeField (null, StringType.Default, Druid.Parse ("[9001]"), 0));
			strucType.Fields.Add (new StructuredTypeField (null, StringType.Default, Druid.Parse ("[9002]"), 1));
			strucType.Fields.Add (new StructuredTypeField (null, StringType.Default, Druid.Parse ("[9003]"), 2));
			strucType.Fields.Add (new StructuredTypeField (null, StringType.Default, Druid.Parse ("[9004]"), 3));
			strucType.Fields.Add (new StructuredTypeField (null, StringType.Default, Druid.Parse ("[9005]"), 4));
			strucType.Fields.Add (new StructuredTypeField (null, StringType.Default, Druid.Parse ("[9006]"), 5));
			strucType.Fields.Add (new StructuredTypeField (null, StringType.Default, Druid.Parse ("[9007]"), 6));
			// 90011 mmdll
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

				using (DbTransaction transaction = infrastructure.BeginTransaction ())
				{
					DbTable dbTable1 = Adapter.CreateTableDefinition (transaction, strucType);
					transaction.Commit ();
				}

				this.broker = new DataBroker (infrastructure);
				broker.LoadTable ( strucType );

				DataTableBroker tableBroker = this.broker.GetTableBrokerFromType (strucType);

//				DataTable dataTable = tableBroker.DataTable;
//				int n = this.DumpDataTable (dataTable);

				DataBrokerRecord record = tableBroker.AddRow ();
				record.SetValue ("[9001]", "Otto");
				record.SetValue ("[9002]", "Kölbl");
				record.SetValue ("[9003]", "Ch. de la Tour-Grise");
				record.SetValue ("[9004]", "28");
				record.SetValue ("[9005]", "1007");
				record.SetValue ("[9006]", "Lausanne");
				record.SetValue ("[9007]", "Suisse");

				record = tableBroker.AddRow ();
				record.SetValue ("[9001]", "Timothy");
				record.SetValue ("[9002]", "Loayza");
				record.SetValue ("[9003]", "Ch. de la Tour-Grise");
				record.SetValue ("[9004]", "28");
				record.SetValue ("[9005]", "1007");
				record.SetValue ("[9006]", "Lausanne");
				record.SetValue ("[9007]", "Suisse");

				record = tableBroker.AddRow ();
				record.SetValue ("[9001]", "Vladimir");
				record.SetValue ("[9002]", "Jaboyedoff");
				record.SetValue ("[9003]", "Ch. de la Tour-Grise");
				record.SetValue ("[9004]", "28");
				record.SetValue ("[9005]", "1007");
				record.SetValue ("[9006]", "Lausanne");
				record.SetValue ("[9007]", "Suisse");

				record = tableBroker.AddRow ();
				record.SetValue ("[9001]", "Frédéric");
				record.SetValue ("[9002]", "Von Arx");
				record.SetValue ("[9003]", "Av. de la Gare");
				record.SetValue ("[9004]", "34");
				record.SetValue ("[9005]", "2710");
				record.SetValue ("[9006]", "Tavannes");
				record.SetValue ("[9007]", "Suisse");

				record = tableBroker.AddRow ();
				record.SetValue ("[9001]", "Pierre");
				record.SetValue ("[9002]", "Arnaud");
				record.SetValue ("[9003]", "Ch. du Brit");
				record.SetValue ("[9004]", "14");
				record.SetValue ("[9005]", "1400");
				record.SetValue ("[9006]", "Yverdon");
				record.SetValue ("[9007]", "Suisse");

				tableBroker.CopyChangesToDataTable ();
				using (DbTransaction transaction = infrastructure.BeginTransaction ())
				{
					this.broker.Save (transaction);
					transaction.Commit ();
				}
			}
			return true;
		}


		int DumpDataTable(DataTable myTable)
		{
			int nb = 0;

			System.Diagnostics.Debug.WriteLine ("TableName = " + myTable.TableName.ToString ());

			//	Affiche le nom des colonnes
			foreach (DataColumn myColumn in myTable.Columns)
			{
				System.Diagnostics.Debug.Write (myColumn.ColumnName);
				System.Diagnostics.Debug.Write (", ");
			}
			System.Diagnostics.Debug.WriteLine (" ");

			foreach (DataRow myRow in myTable.Rows)
			{
				nb++;
				foreach (DataColumn myColumn in myTable.Columns)
				{
					System.Diagnostics.Debug.Write (myRow[myColumn]);
					System.Diagnostics.Debug.Write (", ");
				}
				System.Diagnostics.Debug.WriteLine (" ");
			}
			System.Diagnostics.Debug.WriteLine (nb + " fiches listées");
			return nb;
		}

		
		int DumpDataSet(DataSet data_set)
		{
			int nb = 0;

			//	Pour chaque table dans DataSet, imprime les valeurs.
			foreach (DataTable myTable in data_set.Tables)
			{
				System.Diagnostics.Debug.WriteLine ("TableName = " + myTable.TableName.ToString ());

				//	Affiche le nom des colonnes
				foreach (DataColumn myColumn in data_set.Tables[0].Columns)
				{
					System.Diagnostics.Debug.Write (myColumn.ColumnName);
					System.Diagnostics.Debug.Write (", ");
				}
				System.Diagnostics.Debug.WriteLine (" ");

				foreach (DataRow myRow in myTable.Rows)
				{
					nb++;
					foreach (DataColumn myColumn in myTable.Columns)
					{
						System.Diagnostics.Debug.Write (myRow[myColumn]);
						System.Diagnostics.Debug.Write (", ");
					}
					System.Diagnostics.Debug.WriteLine (" ");
				}
			}
			System.Diagnostics.Debug.WriteLine (nb + " fiches listées");
			return nb;
		}

		protected LeftList leftList;
		protected int leftListRecordSelected;

		protected Window window;
		protected PaneBook mainPaneBook;
		protected PanePage paneLeftList;
		protected PanePage paneEdit;
		protected PanePage paneSettingsDb;
		protected PanePage paneSettingsType;
		protected PanePage paneSettingsTab;

		protected string tableNameContactMain;
		protected DataBroker broker;

		public static readonly DependencyProperty InstanceProperty = DependencyProperty.RegisterAttached ("Instance", typeof (MainWindow), typeof (MainWindow));
	}
}
