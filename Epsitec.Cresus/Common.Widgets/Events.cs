//	Copyright © 2003-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Widgets
{
	using Epsitec.Common.Drawing;

	public delegate void PaintEventHandler(object sender, PaintEventArgs e);
	public delegate void MessageEventHandler(object sender, MessageEventArgs e);
	public delegate void DragEventHandler(object sender, DragEventArgs e);
	public delegate void SelectionEventHandler(object sender, object o);
	public delegate void WindowDragEventHandler(object sender, WindowDragEventArgs e);


	/// <summary>
	/// La classe PaintEventArgs décrit les arguments pour une opération de dessin, ce qui
	/// comprend un contexte graphique et un rectangle de clipping.
	/// </summary>
	public class PaintEventArgs : Support.EventArgs, System.IDisposable
	{
		public PaintEventArgs(Graphics graphics, Drawing.Rectangle clip_rect)
		{
			this.graphics  = graphics;
			this.clip_rect = clip_rect;
		}


		public Graphics Graphics
		{
			get
			{
				return this.graphics;
			}
		}

		public Drawing.Rectangle ClipRectangle
		{
			get
			{
				return this.clip_rect;
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


		#region IDisposable Members

		public void Dispose()
		{
			this.Dispose (true);
			System.GC.SuppressFinalize (this);
		}

		#endregion

		~PaintEventArgs()
		{
			this.Dispose (false);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing && (this.graphics != null))
			{
				this.graphics.Dispose ();
				this.graphics = null;
			}
		}


		private Graphics graphics;
		private Drawing.Rectangle clip_rect;
		private bool suppress;
	}


	/// <summary>
	/// La classe MessageEventArgs décrit un événement produit par l'utilisateur.
	/// </summary>
	public class MessageEventArgs : Support.EventArgs
	{
		public MessageEventArgs(Message message, Drawing.Point point)
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

		public Drawing.Point Point
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


		private Message message;
		private Drawing.Point point;
		private bool suppress;
	}

	/// <summary>
	/// La classe DragEventArgs décrit un déplacement (drag) produit par l'utilisateur.
	/// </summary>
	public class DragEventArgs : Support.EventArgs
	{
		public DragEventArgs(Drawing.Point p1, Drawing.Point p2)
		{
			this.p1 = p1;
			this.p2 = p2;

			this.offset = p2 - p1;
		}


		public Drawing.Point FromPoint
		{
			get
			{
				return this.p1;
			}
		}

		public Drawing.Point ToPoint
		{
			get
			{
				return this.p2;
			}
		}

		public Drawing.Point Offset
		{
			get
			{
				return this.offset;
			}
			set
			{
				this.offset = value;
			}
		}

		public Drawing.Point InitialOffset
		{
			get
			{
				return this.p2 - this.p1;
			}
		}


		private Drawing.Point p1;
		private Drawing.Point p2;
		private Drawing.Point offset;
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


		public Support.Clipboard.ReadData Data
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


		System.Windows.Forms.DragEventArgs original;
	}
}
