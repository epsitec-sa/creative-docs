//	Copyright © 2004-2007, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Types;
using Epsitec.Common.Widgets;

namespace Epsitec.Common.Dialogs
{
	/// <summary>
	/// The <c>AbstractDialog</c> class provides the basic plumbing required
	/// to manage a dialog.
	/// </summary>
	public abstract class AbstractDialog : DependencyObject, IDialog
	{
		protected AbstractDialog()
		{
			this.isModalDialog = true;
			
			this.commandDispatcher = new CommandDispatcher ("Dialog", CommandDispatcherLevel.Secondary);
			this.commandContext    = new CommandContext ();

			CommandDispatcher.SetDispatcher (this, this.commandDispatcher);
			CommandContext.SetContext (this, this.commandContext);
		}


		public CommandDispatcher CommandDispatcher
		{
			get
			{
				return this.commandDispatcher;
			}
		}

		public CommandContext CommandContext
		{
			get
			{
				return this.commandContext;
			}
		}

		
		public Window							DialogWindow
		{
			get
			{
				if (this.dialogWindow == null)
				{
					this.dialogWindow = this.CreateWindow ();

					if (this.dialogWindow != null)
					{
						CommandDispatcher.SetDispatcher (this.dialogWindow, this.CommandDispatcher);
						CommandContext.SetContext (this.dialogWindow, this.CommandContext);
					}
				}

				return this.dialogWindow;
			}
		}
		
		public Window							OwnerWindow
		{
			get
			{
				if (this.dialogWindow != null)
				{
					return this.dialogWindow.Owner;
				}
				
				return null;
			}
			set
			{
				Window window = this.DialogWindow;

				if ((window != null) &&
					(window.Owner != value))
				{
					window.Owner = value;
					this.OnOwnerChanged ();
				}
			}
		}
		
		public DialogResult						DialogResult
		{
			get
			{
				return this.dialogResult;
			}
			protected set
			{
				this.dialogResult = value;
			}
		}
		
		public Window							DispatchWindow
		{
			get
			{
				Window window = this.DialogWindow;
				Window owner  = window == null ? null : window.Owner;
				
				return owner ?? window;
			}
		}
		
		public bool								IsVisible
		{
			get
			{
				if (this.dialogWindow != null)
				{
					return this.dialogWindow.IsVisible;
				}
				else
				{
					return false;
				}
			}
		}
		
		public bool								IsModal
		{
			get
			{
				return this.isModalDialog;
			}
			set
			{
				this.isModalDialog = value;
			}
		}


		public void AddController(object controller)
		{
			this.CommandDispatcher.RegisterController (controller);
		}


		public void OpenDialog()
		{
			Window window = this.DialogWindow;

			if (window == null)
			{
				throw new System.InvalidOperationException ("Cannot show window");
			}

			Window owner = this.OwnerWindow;
			Drawing.Rectangle owner_bounds;

			if ((owner != null) &&
				(owner.IsMinimized == false))
			{
				owner_bounds = owner.WindowBounds;
			}
			else
			{
				owner_bounds = ScreenInfo.AllScreens[0].Bounds;
			}

			//	Make sure we release the mouse capture and we don't process
			//	the currently dispatched message (if any) so we don't activate
			//	a capture after the dialog closes :

			Window  capturingWindow = Window.FindCapturing ();
			Message dispatchMessage = Message.GetLastMessage ();

			if (capturingWindow != null)
			{
				capturingWindow.ReleaseCapture ();
			}
			if (dispatchMessage != null)
			{
				dispatchMessage.Retired = true;
			}

			Drawing.Rectangle dialog_bounds = window.WindowBounds;

			double ox = System.Math.Floor (owner_bounds.Left + (owner_bounds.Width - dialog_bounds.Width) / 2);
			double oy = System.Math.Floor (owner_bounds.Top  - (owner_bounds.Height - dialog_bounds.Height) / 3 - dialog_bounds.Height);

			dialog_bounds.Location = new Drawing.Point (ox, oy);

			window.WindowBounds = dialog_bounds;

			this.OnDialogOpening ();

			if (this.isModalDialog)
			{
				window.WindowShown += this.HandleWindowShown;
				window.ShowDialog ();
				window.WindowShown -= this.HandleWindowShown;
			}
			else
			{
				window.Show ();
				this.HandleWindowShown (window);
			}
		}

		public void CloseDialog()
		{
			Window window = this.dialogWindow;

			if (window != null)
			{
				if (window.IsActive)
				{
					//	Si la fenêtre est active, il faut faire attention à rendre d'abord
					//	le parent actif, avant de cacher la fenêtre, pour éviter que le focus
					//	ne parte dans le décor.

					Window owner = this.OwnerWindow;

					if (owner != null)
					{
						owner.MakeActive ();
					}
				}

				window.Hide ();
				this.OnDialogClosed ();

				if (this.isModalDialog)
				{
					window.Close ();
				}

				window.AsyncDispose ();

				this.dialogWindow = null;
			}
		}


		protected abstract Window CreateWindow();

		protected virtual void OnDialogOpening()
		{
			if (this.DialogOpening != null)
			{
				this.DialogOpening (this);
			}
		}
		
		protected virtual void OnDialogOpened()
		{
			if (this.DialogOpened != null)
			{
				this.DialogOpened (this);
			}
		}

		protected virtual void OnDialogClosed()
		{
			if (this.DialogClosed != null)
			{
				this.DialogClosed (this);
			}
		}

		protected virtual void OnOwnerChanged()
		{
		}
		
		
		private void HandleWindowShown(object sender)
		{
			this.OnDialogOpened ();
		}
		
		
		
		public event EventHandler				DialogOpening;
		public event EventHandler				DialogOpened;
		public event EventHandler				DialogClosed;
		
		private Window							dialogWindow;
		private DialogResult					dialogResult;

		private readonly CommandDispatcher		commandDispatcher;
		private readonly CommandContext			commandContext;

		private bool							isModalDialog;
	}
}
