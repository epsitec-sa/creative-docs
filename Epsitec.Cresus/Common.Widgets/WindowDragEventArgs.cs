//	Copyright © 2003-2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Drawing;

namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// The <c>WindowDragEventArgs</c> class describes an event which is produced
	/// while <c>WinForms</c> drag and drop is happening.
	/// </summary>
	public sealed class WindowDragEventArgs : Support.EventArgs
	{
		internal WindowDragEventArgs(System.Windows.Forms.DragEventArgs args)
		{
			this.original = args;
		}


		/// <summary>
		/// Gets the associated clipboard data.
		/// </summary>
		/// <value>The associated clipboard data.</value>
		public Support.ClipboardReadData		Data
		{
			get
			{
				return Support.Clipboard.CreateReadDataFromIDataObject (this.original.Data);
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether the target is accepting the drop.
		/// </summary>
		/// <value><c>true</c> if the target is accepting the drop; otherwise, <c>false</c>.</value>
		public bool								AcceptDrop
		{
			get
			{
				return (this.original.Effect & System.Windows.Forms.DragDropEffects.Copy) != 0;
			}
			set
			{
				if (this.AcceptDrop != value)
				{
					if (value)
					{
						this.original.Effect = System.Windows.Forms.DragDropEffects.Copy;
					}
					else
					{
						this.original.Effect = System.Windows.Forms.DragDropEffects.None;
					}
				}
			}
		}


		private readonly System.Windows.Forms.DragEventArgs original;
	}
}
