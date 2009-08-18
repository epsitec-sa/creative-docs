//	Copyright © 2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Drawing;
using Epsitec.Common.Graph.Widgets;

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;
using Epsitec.Common.Widgets;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Graph.Controllers
{
	internal sealed class MainScrollListController
	{
		public MainScrollListController(ScrollListMultiSelect list)
		{
			this.ScrollList = list;

			this.ScrollList.DragMultiSelectionStarted += this.HandleDragMultiSelectionStarted;
			this.ScrollList.DragMultiSelectionEnded   += this.HandleDragMultiSelectionEnded;
			this.ScrollList.MultiSelectionChanged     += this.HandleMultiSelectionChanged;
			this.ScrollList.MouseMove                 += this.HandleMouseMove;
		}

		public ScrollListMultiSelect ScrollList
		{
			get;
			private set;
		}


		private void HandleDragMultiSelectionStarted(object sender, MultiSelectEventArgs e)
		{
			if (Message.CurrentState.IsControlPressed == false)
			{
				this.ScrollList.ClearSelection ();
			}
		}

		private void HandleDragMultiSelectionEnded(object sender, MultiSelectEventArgs e)
		{
			if ((e.Count == 1) &&
				(this.ScrollList.IsItemSelected (e.BeginIndex)))
			{
				this.ScrollList.RemoveSelection (Enumerable.Range (e.BeginIndex, e.Count));
			}
			else
			{
				this.ScrollList.AddSelection (Enumerable.Range (e.BeginIndex, e.Count));
			}
		}

		private void HandleMultiSelectionChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			this.OnChanged ();
		}

		private void HandleMouseMove(object sender, MessageEventArgs e)
		{
			int count = this.ScrollList.VisibleRowCount;
			int first = this.ScrollList.FirstVisibleRow;

			for (int i = 0; i < count; i++)
			{
				int index = first + i;
				var frame = this.ScrollList.GetRowBounds (index);

				if (frame.Contains (e.Point))
				{
					System.Diagnostics.Debug.WriteLine (string.Format ("Line {0} contains point {1}", index, e.Point));
				}
			}
		}

		private void OnChanged()
		{
			var handler = this.Changed;

			if (handler != null)
			{
				handler (this);
			}
		}

		
		public event EventHandler Changed;
	}
}
