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
			sa.ShowSelected (ScrollArrayShowMode.Extremity);
			Assertion.AssertEquals (0, sa.FirstVisibleIndex);
			
			//	Sélectionner la ligne suivante ne devrait rien bouger :
			
			sa.SelectedIndex     = 1;
			sa.ShowSelected (ScrollArrayShowMode.Extremity);
			Assertion.AssertEquals (0, sa.FirstVisibleIndex);
			
			//	Mais sélectionner la ligne 2 nécessitera un scroll, car elle occupe les
			//	lignes virtuelles 2,3,4 et 4 dépasse avec le positionnement précédent :
			
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
			table.SelectedIndexChanged += new EventHandler(this.HandleSelectedIndexChanged);
			table.Clicked              += new MessageEventHandler(this.HandleClicked);
			table.DoubleClicked        += new MessageEventHandler(this.HandleDoubleClicked);
			
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
			int row, column;
			table.HitTestTable (e.Point, out row, out column);
			System.Diagnostics.Debug.WriteLine ("Double-clicked : " + row + "," + column);
			table.EditionIndex = row;
		}
	}
}
