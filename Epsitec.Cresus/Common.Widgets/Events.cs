//	Copyright © 2003-2009, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Drawing;

namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// La classe PaintEventArgs décrit les arguments pour une opération de dessin, ce qui
	/// comprend un contexte graphique et un rectangle de clipping.
	/// </summary>
	public class PaintEventArgs : Support.EventArgs
	{
		public PaintEventArgs(Graphics graphics, Rectangle clipRect)
		{
			this.graphics = graphics;
			this.clipRect = clipRect;
		}


		public Graphics Graphics
		{
			get
			{
				return this.graphics;
			}
		}

		public Rectangle ClipRectangle
		{
			get
			{
				return this.clipRect;
			}
		}

		public bool Suppress
		{
			get
			{
				return this.suppress;
			}
			set
			{
				this.suppress = value;
			}
		}


		private readonly Graphics graphics;
		private readonly Rectangle clipRect;
		private bool suppress;
	}


	/// <summary>
	/// La classe MessageEventArgs décrit un événement produit par l'utilisateur.
	/// </summary>
	public class MessageEventArgs : Support.EventArgs
	{
		public MessageEventArgs(Message message, Point point)
		{
			this.message = message;
			this.point   = point;
		}


		public Message Message
		{
			get
			{
				return this.message;
			}
		}

		public Point Point
		{
			get
			{
				return this.point;
			}
		}

		public bool Suppress
		{
			get
			{
				return this.suppress;
			}
			set
			{
				this.suppress = value;
			}
		}


		private readonly Message message;
		private readonly Point point;
		private bool suppress;
	}

	/// <summary>
	/// La classe DragEventArgs décrit un déplacement (drag) produit par l'utilisateur.
	/// </summary>
	public class DragEventArgs : Support.EventArgs
	{
		public DragEventArgs(Point p1, Point p2)
		{
			this.p1 = p1;
			this.p2 = p2;
		}


		public Point FromPoint
		{
			get
			{
				return this.p1;
			}
		}

		public Point ToPoint
		{
			get
			{
				return this.p2;
			}
		}

		public Point Offset
		{
			get
			{
				return this.p2 - this.p1;
			}
		}


		private readonly Point p1;
		private readonly Point p2;
	}


	/// <summary>
	/// Summary description for WindowDragEventArgs.
	/// </summary>
	public sealed class WindowDragEventArgs : Support.EventArgs
	{
		internal WindowDragEventArgs(System.EventArgs args)
		{
			this.original = args as System.Windows.Forms.DragEventArgs;
		}


		public Support.ClipboardReadData Data
		{
			get
			{
				return Support.Clipboard.CreateReadDataFromIDataObject (this.original.Data);
			}
		}

		public bool AcceptDrop
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
