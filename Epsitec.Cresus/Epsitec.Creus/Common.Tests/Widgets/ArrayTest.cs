using NUnit.Framework;
using Epsitec.Common.Support;
using Epsitec.Common.Widgets;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Tests.Widgets
{
	[TestFixture] public class ArrayTest
	{
		[SetUp] public void SetUp()
		{
			Widget.Initialize ();
			Document.Engine.Initialize ();
			Common.Widgets.Adorners.Factory.SetActive ("LookMetal");
		}
		
		[Test] public void AutomatedTestEnvironment()
		{
			Epsitec.Common.Widgets.Window.RunningInAutomatedTestEnvironment = true;
		}
		
		[Test] public void CheckInteractive()
		{
			Window window = new Window();
			
			window.ClientSize = new Size(400, 300);
			window.Text = "CheckInteractive / ScrollArray";
			window.Root.Padding = new Margins (5, 5, 5, 5);
			
			ScrollArray table = new ScrollArray();
			
			table.SetParent (window.Root);
			table.Dock              = DockStyle.Fill;
			table.ColumnCount       = 5;
			table.RowCount          = 100;
			table.SelectedItemIndex = 0;
			table.TitleHeight       = 32;
			table.SelectedItemChanged  += this.HandleSelectedItemChanged;
			table.Clicked              += this.HandleClicked;
			table.DoubleClicked        += this.HandleDoubleClicked;
			table.PaintForeground      += this.HandlePaintForeground;
			table.TitleWidget       = new StaticText (@"<font size=""160%"">ScrollArray test.</font> Double-click to start edition.");
			table.TagWidget			= new Tag ();
			
			for (int x = 0 ; x < table.ColumnCount; x++)
			{
				table.SetHeaderText (x, string.Format ("C{0}", x));
				table.SetColumnWidth (x, 80);
			}
			for (int y = 0; y < 100; y++)
			{
				for (int x = 0 ; x < table.ColumnCount; x++)
				{
					table[y,x] = string.Format ("Val {0}.{1}", y, x);
				}
			}
			
			window.Show();
			Window.RunInTestEnvironment (window);
		}
		
#if false
		[Test] public void CheckEditArray()
		{
			Window window = new Window();
			
			window.ClientSize = new Drawing.Size(400, 300);
			window.Text = "CheckEditArray";
			window.Root.Padding = new Drawing.Margins (5, 5, 5, 5);
			
			EditArray table = new EditArray();
			
			table.SetParent (window.Root);
			table.Dock              = DockStyle.Fill;
			table.ColumnCount       = 5;
			table.RowCount          = 100;
			table.SelectedItemIndex     = 0;
			table.EditionZoneHeight = 1;
			table.TitleHeight       = 32;
			table.DoubleClicked    += this.HandleEditDoubleClicked;
			table.TitleWidget       = new StaticText (@"<font size=""160%"">EditArray test.</font> Double-click to start edition (h x 1).");
			
			for (int x = 0 ; x < table.ColumnCount; x++)
			{
				table.SetHeaderText (x, string.Format ("C{0}", x));
				table.SetColumnWidth (x, 80);
			}
			for (int y = 0; y < 100; y++)
			{
				for (int x = 0 ; x < table.ColumnCount; x++)
				{
					table[y,x] = string.Format ("Val {0}.{1}", y, x);
				}
			}
			
			window.Show();
			Window.RunInTestEnvironment (window);
		}
		
		[Test] public void CheckEditArrayWithTextStore()
		{
			Window window = new Window();
			
			window.ClientSize = new Drawing.Size(400, 300);
			window.Text = "CheckEditArrayWithTextStore";
			window.Root.Padding = new Drawing.Margins (5, 5, 5, 5);
			
			EditArray table = new EditArray();
			TextStore store = new TextStore ();
			
			table.SetParent (window.Root);
			table.Dock              = DockStyle.Fill;
			table.SelectedItemIndex     = 0;
			table.EditionZoneHeight = 4;
			table.TitleHeight       = 32;
			table.DoubleClicked    += this.HandleEditDoubleClicked;
			table.TextArrayStore    = store;
			table.TitleWidget       = new StaticText (@"<font size=""160%"">EditArray test.</font> Double-click to start edition (h x 4).");
			
			window.Show();
			Window.RunInTestEnvironment (window);
		}
		
		
		[Test] public void CheckEditArraySearch()
		{
			Window window = new Window();
			
			window.ClientSize = new Drawing.Size(400, 300);
			window.Text = "CheckEditArraySearch";
			window.Root.Padding = new Drawing.Margins (5, 5, 5, 5);
			
			EditArray table = new EditArray();
			
			table.SetParent (window.Root);
			table.Dock              = DockStyle.Fill;
			table.ColumnCount       = 5;
			table.RowCount          = 100;
			table.SelectedItemIndex     = 0;
			table.EditionZoneHeight = 1;
			
			for (int x = 0 ; x < table.ColumnCount; x++)
			{
				table.SetHeaderText (x, string.Format ("C{0}", x));
				table.SetColumnWidth (x, 80);
			}
			for (int y = 0; y < 100; y++)
			{
				table[y,0] = string.Format ("Val {0}.{1}", y/5, 0);
				table[y,1] = string.Format ("Val {0}.{1}", "X", y%5);
				table[y,2] = string.Format ("Val {0}.{1}", y/10, "A");
				table[y,3] = string.Format ("Val {0}.{1}", "Y", y%10);
				table[y,4] = string.Format ("Val {0}.{1}", y/5, "C");
			}
			
			table.EditTextChanged += HandleTableEditTextChanged;
			table.StartSearch ();
			
			window.Show();
			Window.RunInTestEnvironment (window);
		}
		
		[Test] public void CheckEditArraySearchWithCaption()
		{
			Window window = new Window();
			
			window.ClientSize = new Drawing.Size(400, 300);
			window.Text = "CheckEditArraySearchWithCaption";
			window.Root.Padding = new Drawing.Margins (5, 5, 5, 5);
			
			EditArray table = new EditArray();
			
			table.SetParent (window.Root);
			table.Dock              = DockStyle.Fill;
			table.ColumnCount       = 5;
			table.RowCount          = 100;
			table.SelectedItemIndex     = 0;
			table.EditionZoneHeight = 1;
			table.TitleHeight       = 32;
			table.TitleWidget       = new StaticText (@"<font size=""120%"">Search Test.</font>");
			table.SearchCaption     = @"<b>Search. </b><font size=""90%"">Type in some text below to search for it in the table.</font>";
			
			for (int x = 0 ; x < table.ColumnCount; x++)
			{
				table.SetHeaderText (x, string.Format ("C{0}", x));
				table.SetColumnWidth (x, 80);
			}
			for (int y = 0; y < 100; y++)
			{
				table[y,0] = string.Format ("Val {0}.{1}", y/5, 0);
				table[y,1] = string.Format ("Val {0}.{1}", "X", y%5);
				table[y,2] = string.Format ("Val {0}.{1}", y/10, "A");
				table[y,3] = string.Format ("Val {0}.{1}", "Y", y%10);
				table[y,4] = string.Format ("Val {0}.{1}", y/5, "C");
			}
			
			table.EditTextChanged += HandleTableEditTextChanged;
			table.StartSearch ();
			
			window.Show();
			Window.RunInTestEnvironment (window);
		}
		
		[Test] public void CheckEditArraySearchWithToolBar()
		{
			Window window = new Window();
			
			window.ClientSize = new Drawing.Size(400, 300);
			window.Text = "CheckEditArraySearchWithToolBar";
			window.Root.Padding = new Drawing.Margins (5, 5, 5, 5);
			
			EditArray            table  = new EditArray();
			EditArray.Header     header = new EditArray.Header (table);
			EditArray.Controller ctrl   = new EditArray.Controller (table, "Table");
			
			table.AttachCommandDispatcher (new CommandDispatcher ("SearchTable", CommandDispatcherLevel.Secondary));
			table.SetParent (window.Root);
			table.Dock              = DockStyle.Fill;
			table.ColumnCount       = 5;
			table.RowCount          = 100;
			table.SelectedItemIndex     = 0;
			table.EditionZoneHeight = 1;
			table.TitleWidget       = header;
			table.SearchCaption     = @"<b>Search. </b><font size=""90%"">Type in some text below to search for it in the table.</font>";
			
			TextFieldEx model_field_1 = new TextFieldEx ();
			
			model_field_1.ButtonShowCondition = ShowCondition.WhenModified;
			model_field_1.DefocusAction       = DefocusAction.Modal;
			
			table.Columns[0].EditionWidgetModel = model_field_1;
			table.EditWidgetsCreated += this.HandleTableEditWidgetsCreated;
			
			header.Caption = @"<font size=""120%"">Search Test.</font>";
			
			ctrl.CreateCommands ();
			ctrl.CreateToolBarButtons ();
			ctrl.StartReadOnly ();
			
			for (int x = 0 ; x < table.ColumnCount; x++)
			{
				table.SetHeaderText (x, string.Format ("C{0}", x));
				table.SetColumnWidth (x, 80);
			}
			for (int y = 0; y < 100; y++)
			{
				table[y,0] = string.Format ("Val {0}.{1}", y/5, 0);
				table[y,1] = string.Format ("Val {0}.{1}", "X", y%5);
				table[y,2] = string.Format ("Val {0}.{1}", y/10, "A");
				table[y,3] = string.Format ("Val {0}.{1}", "Y", y%10);
				table[y,4] = string.Format ("Val {0}.{1}", y/5, "C");
			}
			
			table.EditTextChanged += HandleTableEditTextChanged;
			table.StartSearch ();
			
			window.Show();
			Window.RunInTestEnvironment (window);
		}
		
		
		protected class TextStore : Support.Data.ITextArrayStore
		{
			public void InsertRows(int row, int num)
			{
				if (this.StoreContentsChanged != null)
				{
					this.StoreContentsChanged (this);
				}
			}

			public void RemoveRows(int row, int num)
			{
				if (this.StoreContentsChanged != null)
				{
					this.StoreContentsChanged (this);
				}
			}
			
			public void MoveRow(int row, int distance)
			{
			}

			public string GetCellText(int row, int column)
			{
				return string.Format ("Auto {0}.{1}", row, column);
			}

			public int GetRowCount()
			{
				return 100;
			}

			public int GetColumnCount()
			{
				return 6;
			}

			public void SetCellText(int row, int column, string value)
			{
			}
			
			public bool CheckSetRow(int row)
			{
				return false;
			}
			
			public bool CheckInsertRows(int row, int num)
			{
				return false;
			}
			
			public bool CheckRemoveRows(int row, int num)
			{
				return false;
			}
			
			public bool CheckMoveRow(int row, int distance)
			{
				return false;
			}
			
			public bool CheckEnabledCell(int row, int column)
			{
				return (row % 2) == 0 ? true : false;
			}
			
			public event Support.EventHandler	StoreContentsChanged;
		}
#endif

		private void HandleSelectedItemChanged(object sender)
		{
			ScrollArray table = sender as ScrollArray;
			System.Diagnostics.Debug.WriteLine ("Selected : " + table.SelectedItemIndex);
		}

		private void HandleClicked(object sender, MessageEventArgs e)
		{
			ScrollArray table = sender as ScrollArray;
			int row, column;
			table.HitTestTable (e.Point, out row, out column);
			System.Diagnostics.Debug.WriteLine ("Clicked : " + row + "," + column);
		}
		
		private void HandleDoubleClicked(object sender, MessageEventArgs e)
		{
			ScrollArray table = sender as ScrollArray;
			table.HitTestTable (e.Point, out this.hilite_row, out this.hilite_column);
			System.Diagnostics.Debug.WriteLine ("Double-clicked : " + this.hilite_row + "," + this.hilite_column);
			table.SelectedItemIndex = this.hilite_row;
			table.ShowEdition (ScrollShowMode.Extremity);
			table.Invalidate ();
		}

#if false
		private void HandleEditDoubleClicked(object sender, MessageEventArgs e)
		{
			EditArray table = sender as EditArray;
			
			int row, column;
			
			table.HitTestTable (e.Point, out row, out column);
			System.Diagnostics.Debug.WriteLine ("Double-clicked : " + row + "," + column);
			table.StartEdition (row, column);
		}
#endif

		private void HandlePaintForeground(object sender, PaintEventArgs e)
		{
			ScrollArray table = sender as ScrollArray;
			Rectangle hilite = table.GetCellBounds (this.hilite_row, this.hilite_column);
			
			if (hilite.IsValid)
			{
				e.Graphics.AddFilledRectangle (hilite);
				e.Graphics.RenderSolid (Color.FromAlphaRgb (0.25, 0, 0, 1));
			}
		}
		
#if false
		private void HandleTableEditTextChanged(object sender)
		{
			EditArray table = sender as EditArray;
			
			if (table.InteractionMode == ScrollInteractionMode.Search)
			{
				string[] search = table.EditValues;
				
				table.SelectedItem = string.Join (table.Separator.ToString (), search);
				table.ShowSelected (ScrollShowMode.Center);
			}
		}
		
		private void HandleTableEditWidgetsCreated(object sender)
		{
			EditArray edit = sender as EditArray;
			
			if (edit.InteractionMode == ScrollInteractionMode.Edition)
			{
				TextFieldEx col_0_text = edit.Columns[0].EditionWidget as TextFieldEx;
				IValidator validator = new Validators.RegexValidator (col_0_text, Support.RegexFactory.ResourceBundleName);
				col_0_text.EditionAccepted += this.HandleColumnZeroEditionAccepted;
				col_0_text.EditionRejected += this.HandleColumnZeroEditionRejected;
			}
		}
#endif
		
		private void HandleColumnZeroEditionAccepted(object sender)
		{
			TextFieldEx text = sender as TextFieldEx;
			System.Diagnostics.Debug.WriteLine (string.Format ("Edition accepted: {0}", text.Text));
		}
		
		private void HandleColumnZeroEditionRejected(object sender)
		{
			TextFieldEx text = sender as TextFieldEx;
			System.Diagnostics.Debug.WriteLine (string.Format ("Edition rejected: {0}", text.Text));
		}
		
		private int			hilite_row		= -1;
		private int			hilite_column	= -1;
	}
}
