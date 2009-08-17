//	Copyright © 2003-2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Drawing;
using Epsitec.Common.Support;

namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// The <c>ScrollListMultiselect</c> class implements a <see cref="ScrollList"/>
	/// with support for multiple line selection.
	/// </summary>
	public class ScrollListMultiSelect : ScrollList
	{
		public ScrollListMultiSelect()
		{
			this.dragSelectionStartIndex = NoSelection;
		}

		public ScrollListMultiSelect(Widget embedder)
			: this ()
		{
			this.SetEmbedder (embedder);
		}

		
		protected override bool IsLineSelected(int index)
		{
			if (this.dragSelectionStartIndex >= 0)
			{
				int begin = System.Math.Min (this.dragSelectionStartIndex, this.SelectedIndex);
				int end   = System.Math.Max (this.dragSelectionStartIndex, this.SelectedIndex);

				if ((index >= begin) &&
					(index <= end))
				{
					return true;
				}
			}

			return base.IsLineSelected (index);
		}

		protected override void MouseSelectBegin()
		{
			base.MouseSelectBegin ();
			this.dragSelectionStartIndex = EmptySelection;
		}

		protected override void MouseSelectEnd()
		{
			base.MouseSelectEnd ();
			
			if (this.dragSelectionStartIndex != this.SelectedIndex)
			{
				//	The user dragged a multiline selection, which will trigger an appropriate
				//	notification event.

				//	TODO: ...
			}

			this.dragSelectionStartIndex = NoSelection;
		}

		protected override void MouseSelectLine(int index)
		{
			if (this.dragSelectionStartIndex == EmptySelection)
			{
				this.dragSelectionStartIndex = index;
			}

			base.MouseSelectLine (index);
		}


		private const int NoSelection = -2;
		private const int EmptySelection = -1;

		private int dragSelectionStartIndex;
	}
}
