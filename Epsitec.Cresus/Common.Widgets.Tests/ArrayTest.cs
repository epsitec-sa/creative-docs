using NUnit.Framework;
using Epsitec.Common.Support;

namespace Epsitec.Common.Widgets
{
	[TestFixture] public class ArrayTest
	{
		[Test] public void CheckVirtualRows()
		{
			ScrollArray sa = new ScrollArray ();
			
			sa.RowCount    = 9;
			sa.ColumnCount = 2;
			
			//	Exactement 4 lignes visibles :
			
			sa.AdjustToRows (ScrollAdjustMode.MoveUp, 4);
			
			Assertion.AssertEquals (9, sa.RowCount);
			Assertion.AssertEquals (4, sa.VisibleRowCount);
			Assertion.AssertEquals (4, sa.FullyVisibleRowCount);
			
			//	Ajuste un tout petit poil pour avoir une 5è partiellement visible :
			
			sa.Top = sa.Top + 1;
			
			Assertion.AssertEquals (5, sa.VisibleRowCount);
			Assertion.AssertEquals (4, sa.FullyVisibleRowCount);
			
			//	Teste avec la configuration :
			//
			//	0		virtual 0
			//	1		virtual 1
			//	2		virtual 2 < Zone en cours.. >
			//			virtual 3 < ..d'édition     >
			//	3		virtual 4
			//	4		virtual 5
			//	5		virtual 6
			//	6		virtual 7
			//	7		virtual 8
			//	8		virtual 9
			
			sa.Top = sa.Top - 1;
			
			sa.EditionIndex      = 2;
			sa.EditionZoneHeight = 2;
			sa.SelectedIndex     = 5;
			
			Assertion.AssertEquals (10, sa.VirtualRowCount);
			
			Assertion.AssertEquals (1, sa.ToVirtualRow (1));
			Assertion.AssertEquals (2, sa.ToVirtualRow (2));
			Assertion.AssertEquals (4, sa.ToVirtualRow (3));
			
			Assertion.AssertEquals (1, sa.FromVirtualRow (1));
			Assertion.AssertEquals (2, sa.FromVirtualRow (2));
			Assertion.AssertEquals (2, sa.FromVirtualRow (3));
			Assertion.AssertEquals (3, sa.FromVirtualRow (4));
			
			sa.FirstVisibleIndex = 2;
			Assertion.AssertEquals (2, sa.FirstVisibleIndex);
			Assertion.Assert (! sa.IsSelectedVisible);
			
			sa.FirstVisibleIndex = 3;
			Assertion.AssertEquals (3, sa.FirstVisibleIndex);
			Assertion.Assert (sa.IsSelectedVisible);
			
			sa.FirstVisibleIndex = 4;
			Assertion.AssertEquals (4, sa.FirstVisibleIndex);
			Assertion.Assert (sa.IsSelectedVisible);
			
			sa.FirstVisibleIndex = 5;
			Assertion.AssertEquals (5, sa.FirstVisibleIndex);
			Assertion.Assert (sa.IsSelectedVisible);
			
			sa.FirstVisibleIndex = 6;
			Assertion.AssertEquals (5, sa.FirstVisibleIndex);
			Assertion.Assert (sa.IsSelectedVisible);
			
			sa.EditionIndex      = 8;
			sa.FirstVisibleIndex = 7;
			sa.SelectedIndex     = 6;
			
			Assertion.AssertEquals (6, sa.FirstVisibleIndex);
			Assertion.Assert (sa.IsSelectedVisible);
			
			//	Sélectionne la première ligne et rend celle-ci visible; force un
			//	alignement en haut :
			
			sa.SelectedIndex     = 0;
			sa.EditionIndex      = 2;
			sa.EditionZoneHeight = 3;
			sa.ShowSelected (ScrollShowMode.Extremity);
			Assertion.AssertEquals (0, sa.FirstVisibleIndex);
			
			//	Sélectionner la ligne suivante ne devrait rien bouger :
			
			sa.SelectedIndex     = 1;
			sa.ShowSelected (ScrollShowMode.Extremity);
			Assertion.AssertEquals (0, sa.FirstVisibleIndex);
			
			//	Mais sélectionner la ligne 2 nécessitera un scroll, car elle occupe les
			//	lignes virtuelles 2,3,4 et 4 dépasse avec le positionnement précédent :
			
			sa.SelectedIndex = 2;
			sa.ShowSelected (ScrollShowMode.Extremity);
			Assertion.AssertEquals (1, sa.FirstVisibleIndex);
			
			sa.SelectedIndex = 8;
			sa.ShowSelected (ScrollShowMode.Extremity);
			Assertion.AssertEquals (5, sa.FirstVisibleIndex);
		}
		
		[Test] public void CheckInteractive()
		{
			Window window = new Window();
			
			window.ClientSize = new Drawing.Size(400, 300);
			window.Text = "CheckInteractive / ScrollArray";
			window.Root.DockMargins = new Drawing.Margins (5, 5, 5, 5);
			
			ScrollArray table = new ScrollArray();
			
			table.Parent            = window.Root;
			table.Dock              = DockStyle.Fill;
			table.ColumnCount       = 5;
			table.RowCount          = 100;
			table.SelectedIndex     = 0;
			table.EditionIndex      = 1;
			table.EditionZoneHeight = 2;
			table.TitleHeight       = 32;
			table.SelectedIndexChanged += new EventHandler(this.HandleSelectedIndexChanged);
			table.Clicked              += new MessageEventHandler(this.HandleClicked);
			table.DoubleClicked        += new MessageEventHandler(this.HandleDoubleClicked);
			table.PaintForeground      += new PaintEventHandler(this.HandlePaintForeground);
			
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
			
			StaticText title = new StaticText (@"<font size=""160%"">ScrollArray test.</font> Double-click to start edition.");
			
			title.Parent = table;
			title.Bounds = table.TitleBounds;
			title.Anchor = AnchorStyles.LeftAndRight | AnchorStyles.Top;
			
			window.Show();
		}
		
		[Test] public void CheckEditArray()
		{
			Window window = new Window();
			
			window.ClientSize = new Drawing.Size(400, 300);
			window.Text = "CheckEditArray";
			window.Root.DockMargins = new Drawing.Margins (5, 5, 5, 5);
			
			EditArray table = new EditArray();
			
			table.Parent            = window.Root;
			table.Dock              = DockStyle.Fill;
			table.ColumnCount       = 5;
			table.RowCount          = 100;
			table.SelectedIndex     = 0;
			table.EditionIndex      = -1;
			table.EditionZoneHeight = 1;
			table.TitleHeight       = 32;
			table.DoubleClicked    += new MessageEventHandler(this.HandleEditDoubleClicked);
			
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
			
			StaticText title = new StaticText (@"<font size=""160%"">EditArray test.</font> Double-click to start edition.");
			
			title.Parent = table;
			title.Bounds = table.TitleBounds;
			title.Anchor = AnchorStyles.LeftAndRight | AnchorStyles.Top;
			
			window.Show();
		}
		
		[Test] public void CheckEditArrayWithTextStore()
		{
			Window window = new Window();
			
			window.ClientSize = new Drawing.Size(400, 300);
			window.Text = "CheckEditArrayWithTextStore";
			window.Root.DockMargins = new Drawing.Margins (5, 5, 5, 5);
			
			EditArray table = new EditArray();
			TextStore store = new TextStore ();
			
			table.Parent            = window.Root;
			table.Dock              = DockStyle.Fill;
			table.SelectedIndex     = 0;
			table.EditionIndex      = -1;
			table.EditionZoneHeight = 4;
			table.TitleHeight       = 32;
			table.DoubleClicked    += new MessageEventHandler(this.HandleEditDoubleClicked);
			table.TextArrayStore    = store;
			
			StaticText title = new StaticText (@"<font size=""160%"">EditArray test.</font> Double-click to start edition.");
			
			title.Parent = table;
			title.Bounds = table.TitleBounds;
			title.Anchor = AnchorStyles.LeftAndRight | AnchorStyles.Top;
			
			window.Show();
		}
		
		[Test] public void CheckEditArrayWithBundleStore()
		{
			Window window = new Window();
			
			window.ClientSize = new Drawing.Size(400, 300);
			window.Text = "CheckEditArrayWithBundleStore";
			window.Root.DockMargins = new Drawing.Margins (5, 5, 5, 5);
			
			EditArray table = new EditArray();
			Design.Tools.TextBundleArrayStore store = new Design.Tools.TextBundleArrayStore ();
			store.Bundle = Support.Resources.GetBundle ("file:strings");
			
			table.Parent            = window.Root;
			table.Dock              = DockStyle.Fill;
			table.SelectedIndex     = 0;
			table.EditionIndex      = -1;
			table.EditionZoneHeight = 1;
			table.TitleHeight       = 32;
			table.DoubleClicked    += new MessageEventHandler(this.HandleEditDoubleClicked);
			table.TextArrayStore    = store;
			
			StaticText title = new StaticText (@"<font size=""160%"">EditArray test.</font> Double-click to start edition.");
			
			title.Parent = table;
			title.Bounds = table.TitleBounds;
			title.Anchor = AnchorStyles.LeftAndRight | AnchorStyles.Top;
			
			window.Show();
		}
		
		
		[Test] public void CheckScrollArraySearch()
		{
			Window window = new Window();
			
			window.ClientSize = new Drawing.Size(400, 300);
			window.Text = "CheckScrollArraySearch";
			window.Root.DockMargins = new Drawing.Margins (5, 5, 5, 5);
			
			ScrollArray table = new ScrollArray();
			
			table.Parent            = window.Root;
			table.Dock              = DockStyle.Fill;
			table.ColumnCount       = 5;
			table.RowCount          = 100;
			table.SelectedIndex     = 0;
			table.InnerTopMargin    = 20;
			table.InnerBottomMargin = 10;
			
			for (int x = 0 ; x < table.ColumnCount; x++)
			{
				table.SetHeaderText (x, string.Format ("C{0}", x));
				table.SetColumnWidth (x, 80);
			}
			for (int y = 0; y < 100; y++)
			{
				table[y,0] = string.Format ("Val {0}.{1}", y/5, 0);
				table[y,1] = string.Format ("Val {0}.{1}", y/5, y%5);
				table[y,2] = string.Format ("Val {0}.{1}", y/5, "A");
				table[y,3] = string.Format ("Val {0}.{1}", y/5, "B");
				table[y,4] = string.Format ("Val {0}.{1}", y/5, "C");
			}
			
			TextField field = new TextField ();
			
			field.Parent       = window.Root;
			field.Dock         = DockStyle.Bottom;
			field.TextChanged += new EventHandler (this.HandleCheckScrollArraySearchTextChanged);
			field.SetProperty ("table", table);
			
			window.Show();
		}
		
		[Test] public void CheckEditArraySearch()
		{
			Window window = new Window();
			
			window.ClientSize = new Drawing.Size(400, 300);
			window.Text = "CheckEditArraySearch";
			window.Root.DockMargins = new Drawing.Margins (5, 5, 5, 5);
			
			EditArray table = new EditArray();
			
			table.Parent            = window.Root;
			table.Dock              = DockStyle.Fill;
			table.ColumnCount       = 5;
			table.RowCount          = 100;
			table.SelectedIndex     = 0;
			table.EditionZoneHeight = 1;
			table.EditArrayMode     = EditArrayMode.Search;
			
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
			
			table.EditTextChanged += new EventHandler(HandleTableEditTextChanged);
			
			window.Show();
		}
		
		[Test] public void CheckEditArraySearchWithCaption()
		{
			Window window = new Window();
			
			window.ClientSize = new Drawing.Size(400, 300);
			window.Text = "CheckEditArraySearchWithCaption";
			window.Root.DockMargins = new Drawing.Margins (5, 5, 5, 5);
			
			EditArray table = new EditArray();
			
			table.Parent            = window.Root;
			table.Dock              = DockStyle.Fill;
			table.ColumnCount       = 5;
			table.RowCount          = 100;
			table.SelectedIndex     = 0;
			table.EditionZoneHeight = 1;
			table.EditArrayMode     = EditArrayMode.Search;
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
			
			table.EditTextChanged += new EventHandler(HandleTableEditTextChanged);
			
			window.Show();
		}
		
		
		protected class TextStore : Support.Data.ITextArrayStore
		{
			public void InsertRows(int row, int num)
			{
				if (this.StoreChanged != null)
				{
					this.StoreChanged (this);
				}
			}

			public void RemoveRows(int row, int num)
			{
				if (this.StoreChanged != null)
				{
					this.StoreChanged (this);
				}
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
			
			public event Support.EventHandler	StoreChanged;
		}
		
		
		private void HandleCheckScrollArraySearchTextChanged(object sender)
		{
			TextField   field = sender as TextField;
			ScrollArray table = field.GetProperty ("table") as ScrollArray;
			table.SelectedItem = field.Text;
		}
		
		private void HandleSelectedIndexChanged(object sender)
		{
			ScrollArray table = sender as ScrollArray;
			System.Diagnostics.Debug.WriteLine ("Selected : " + table.SelectedIndex);
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
			table.EditionIndex = this.hilite_row;
			table.ShowEdition (ScrollShowMode.Extremity);
			table.Invalidate ();
		}

		private void HandleEditDoubleClicked(object sender, MessageEventArgs e)
		{
			EditArray table = sender as EditArray;
			
			int row, column;
			
			table.HitTestTable (e.Point, out row, out column);
			System.Diagnostics.Debug.WriteLine ("Double-clicked : " + row + "," + column);
			table.StartEdition (row, column);
		}

		private void HandlePaintForeground(object sender, PaintEventArgs e)
		{
			ScrollArray table = sender as ScrollArray;
			Drawing.Rectangle hilite = table.GetCellBounds (this.hilite_row, this.hilite_column);
			
			if (hilite.IsValid)
			{
				e.Graphics.AddFilledRectangle (hilite);
				e.Graphics.RenderSolid (Drawing.Color.FromARGB (0.25, 0, 0, 1));
			}
		}
		
		private void HandleTableEditTextChanged(object sender)
		{
			EditArray table = sender as EditArray;
			string[] search = table.EditValues;
			
			table.SelectedItem = string.Join (table.Separator.ToString (), search);
			table.ShowSelected (ScrollShowMode.Center);
		}
		
		
		private int			hilite_row		= -1;
		private int			hilite_column	= -1;
	}
}
