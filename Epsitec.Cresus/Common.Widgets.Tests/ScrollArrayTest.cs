using NUnit.Framework;

namespace Epsitec.Common.Widgets
{
	[TestFixture] public class ScrollArrayTest
	{
		[Test] public void CheckVirtualRows()
		{
			ScrollArray sa = new ScrollArray ();
			
			sa.RowCount    = 9;
			sa.ColumnCount = 2;
			
			//	Exactement 4 lignes visibles :
			
			sa.AdjustToRows (ScrollArrayAdjustMode.MoveUp, 4);
			
			Assertion.AssertEquals (9, sa.RowCount);
			Assertion.AssertEquals (4, sa.VisibleRowCount);
			Assertion.AssertEquals (4, sa.FullyVisibleRowCount);
			
			//	Ajuste un tout petit poil pour avoir une 5� partiellement visible :
			
			sa.Top = sa.Top + 1;
			
			Assertion.AssertEquals (5, sa.VisibleRowCount);
			Assertion.AssertEquals (4, sa.FullyVisibleRowCount);
			
			//	Teste avec la configuration :
			//
			//	0		virtual 0
			//	1		virtual 1
			//	2		virtual 2 < Zone en cours.. >
			//			virtual 3 < ..d'�dition     >
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
			
			//	S�lectionne la premi�re ligne et rend celle-ci visible; force un
			//	alignement en haut :
			
			sa.SelectedIndex     = 0;
			sa.EditionIndex      = 2;
			sa.EditionZoneHeight = 3;
			sa.ShowSelected (ScrollArrayShowMode.Extremity);
			Assertion.AssertEquals (0, sa.FirstVisibleIndex);
			
			//	S�lectionner la ligne suivante ne devrait rien bouger :
			
			sa.SelectedIndex     = 1;
			sa.ShowSelected (ScrollArrayShowMode.Extremity);
			Assertion.AssertEquals (0, sa.FirstVisibleIndex);
			
			//	Mais s�lectionner la ligne 2 n�cessitera un scroll, car elle occupe les
			//	lignes virtuelles 2,3,4 et 4 d�passe avec le positionnement pr�c�dent :
			
			sa.SelectedIndex = 2;
			sa.ShowSelected (ScrollArrayShowMode.Extremity);
			Assertion.AssertEquals (1, sa.FirstVisibleIndex);
			
			sa.SelectedIndex = 8;
			sa.ShowSelected (ScrollArrayShowMode.Extremity);
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
		
		[Test] public void CheckSmartTagColor()
		{
			Window window = new Window ();
			window.Text = "CheckSmartTagColor";
			window.ClientSize = new Drawing.Size (510, 220);
			window.MakeFixedSizeWindow ();
			
			ColorSelector selector1, selector2;
			
			selector1 = new ColorSelector ();
			selector1.Bounds = new Drawing.Rectangle (100, 10, 200, 200);
			selector1.Parent = window.Root;
			selector1.Changed += new EventHandler (this.HandleSelectorChangedForeground);
			
			selector2 = new ColorSelector ();
			selector2.Bounds = new Drawing.Rectangle (300, 10, 200, 200);
			selector2.Parent = window.Root;
			selector2.Changed += new EventHandler (this.HandleSelectorChangedBackground);
			
			Tag tag;
			
			tag = new Tag ("", "tag1");
			tag.Bounds = new Drawing.Rectangle (10, 10, 10, 10);
			tag.Parent = window.Root;
			
			tag = new Tag ("", "tag2");
			tag.Bounds = new Drawing.Rectangle (10, 25, 15, 15);
			tag.Parent = window.Root;
			
			tag = new Tag ("", "tag3");
			tag.Bounds = new Drawing.Rectangle (10, 45, 20, 20);
			tag.Parent = window.Root;
			
			tag = new Tag ("", "tag4");
			tag.Bounds = new Drawing.Rectangle (10, 70, 25, 25);
			tag.Parent = window.Root;
			
			selector1.Color = tag.Color;
			selector2.Color = tag.BackColor;
			
			window.Show ();
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
			table.ShowEdition (ScrollArrayShowMode.Extremity);
			table.Invalidate ();
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
		
		private void HandleSelectorChangedForeground(object sender)
		{
			ColorSelector selector = sender as ColorSelector;
			Drawing.Color color    = selector.Color;
			Widget        parent   = selector.Parent;
			
			Tag tag;
			
			tag = parent.FindChild ("tag1", Widget.ChildFindMode.Deep) as Tag;
			tag.Color = color;
			
			tag = parent.FindChild ("tag2", Widget.ChildFindMode.Deep) as Tag;
			tag.Color = color;
			
			tag = parent.FindChild ("tag3", Widget.ChildFindMode.Deep) as Tag;
			tag.Color = color;
			
			tag = parent.FindChild ("tag4", Widget.ChildFindMode.Deep) as Tag;
			tag.Color = color;
		}
		
		private void HandleSelectorChangedBackground(object sender)
		{
			ColorSelector selector = sender as ColorSelector;
			Drawing.Color color    = selector.Color;
			Widget        parent   = selector.Parent;
			
			Tag tag;
			
			tag = parent.FindChild ("tag1", Widget.ChildFindMode.Deep) as Tag;
			tag.BackColor = color;
			
			tag = parent.FindChild ("tag2", Widget.ChildFindMode.Deep) as Tag;
			tag.BackColor = color;
			
			tag = parent.FindChild ("tag3", Widget.ChildFindMode.Deep) as Tag;
			tag.BackColor = color;
			
			tag = parent.FindChild ("tag4", Widget.ChildFindMode.Deep) as Tag;
			tag.BackColor = color;
		}
		
		private int						hilite_row		= -1;
		private int						hilite_column	= -1;
	}
}
