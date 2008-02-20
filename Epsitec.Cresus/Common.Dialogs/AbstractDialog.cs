//	Copyright � 2004-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
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


		/// <summary>
		/// Gets the command dispatcher for this dialog.
		/// </summary>
		/// <value>The command dispatcher.</value>
		public CommandDispatcher				CommandDispatcher
		{
			get
			{
				return this.commandDispatcher;
			}
		}

		/// <summary>
		/// Gets the command context for this dialog.
		/// </summary>
		/// <value>The command context.</value>
		public CommandContext					CommandContext
		{
			get
			{
				return this.commandContext;
			}
		}

		/// <summary>
		/// Gets the window for this dialog. If the window did not exist yet,
		/// it will be created by calling <see cref="CreateWindow"/>.
		/// </summary>
		/// <value>The dialog window.</value>
		public Window							DialogWindow
		{
			get
			{
				if (this.dialogWindow == null)
				{
					this.CreateDialogWindow ();
				}

				return this.dialogWindow;
			}
		}

		/// <summary>
		/// Gets or sets the owner window for this dialog.
		/// </summary>
		/// <value>The owner window.</value>
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
					this.OnWindowOwnerChanged ();
				}
			}
		}

		/// <summary>
		/// Gets the dialog result.
		/// </summary>
		/// <value>The dialog result.</value>
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

		/// <summary>
		/// Gets the dispatch window for this dialog. If the dialog has an
		/// owner, then the dispatch window will be the owner window.
		/// </summary>
		/// <value>The dispatch window.</value>
		public Window							DispatchWindow
		{
			get
			{
				Window window = this.DialogWindow;
				Window owner  = window == null ? null : window.Owner;
				
				return owner ?? window;
			}
		}

		/// <summary>
		/// Gets a value indicating whether this dialog is visible.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this dialog is visible; otherwise, <c>false</c>.
		/// </value>
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

		/// <summary>
		/// Gets or sets a value indicating whether this dialog is modal.
		/// </summary>
		/// <value><c>true</c> if this dialog is modal; otherwise, <c>false</c>.</value>
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

		/// <summary>
		/// Gets a value indicating whether this dialog has an associated
		/// window.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this dialog has an associated window; otherwise,
		/// 	<c>false</c>.
		/// </value>
		public bool								HasWindow
		{
			get
			{
				return this.dialogWindow == null ? false : true;
			}
		}


		/// <summary>
		/// Registers a controller with the dialog's local command dispatcher.
		/// </summary>
		/// <param name="controller">The controller.</param>
		public void RegisterController(object controller)
		{
			this.CommandDispatcher.RegisterController (controller);
		}


		/// <summary>
		/// Opens the dialog (creates the window and makes it visible). If the
		/// dialog is modal, this method won't return until the user closes
		/// the dialog.
		/// </summary>
		public void OpenDialog()
		{
			Window window = this.DialogWindow;

			if (window == null)
			{
				throw new System.InvalidOperationException ("Cannot show window");
			}

			Window owner = this.OwnerWindow;
			Drawing.Rectangle ownerBounds;

			if ((owner != null) &&
				(owner.IsMinimized == false))
			{
				ownerBounds = owner.WindowBounds;
			}
			else
			{
				ownerBounds = ScreenInfo.AllScreens[0].Bounds;
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

			Drawing.Rectangle dialogBounds = window.WindowBounds;

			double ox = System.Math.Floor (ownerBounds.Left + (ownerBounds.Width - dialogBounds.Width) / 2);
			double oy = System.Math.Floor (ownerBounds.Top  - (ownerBounds.Height - dialogBounds.Height) / 3 - dialogBounds.Height);

			dialogBounds.Location = new Drawing.Point (ox, oy);

			window.WindowBounds = dialogBounds;

			this.OnDialogOpening ();

			window.WindowFocused += this.HandleWindowFocused;

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

		/// <summary>
		/// Closes the dialog. The window will be disposed asynchronously.
		/// </summary>
		public void CloseDialog()
		{
			Window window = this.dialogWindow;

			if (window != null)
			{
				if (window.IsActive)
				{
					//	Si la fen�tre est active, il faut faire attention � rendre d'abord
					//	le parent actif, avant de cacher la fen�tre, pour �viter que le focus
					//	ne parte dans le d�cor.

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


		/// <summary>
		/// Creates a window for the current dialog.
		/// </summary>
		/// <returns>The window.</returns>
		protected abstract Window CreateWindow();

		/// <summary>
		/// Called when the dialog is opening.
		/// </summary>
		protected virtual void OnDialogOpening()
		{
			if (this.DialogOpening != null)
			{
				this.DialogOpening (this);
			}
		}

		/// <summary>
		/// Called when the dialog was opened.
		/// </summary>
		protected virtual void OnDialogOpened()
		{
			if (this.DialogOpened != null)
			{
				this.DialogOpened (this);
			}
		}

		/// <summary>
		/// Called when the dialog was closed.
		/// </summary>
		protected virtual void OnDialogClosed()
		{
			if (this.DialogClosed != null)
			{
				this.DialogClosed (this);
			}
		}

		protected virtual void OnDialogWindowCreated()
		{
			if (this.DialogWindowCreated != null)
			{
				this.DialogWindowCreated (this);
			}
		}



		/// <summary>
		/// Called when the window owner changed.
		/// </summary>
		protected virtual void OnWindowOwnerChanged()
		{
		}

		/// <summary>
		/// Sets the focus on the default widget.
		/// </summary>
		protected virtual void SetDefaultFocus()
		{
		}


		private void CreateDialogWindow()
		{
			this.dialogWindow = this.CreateWindow ();

			if (this.dialogWindow != null)
			{
				this.OnDialogWindowCreated ();

				this.dialogWindow.MakeSecondaryWindow ();
				this.dialogWindow.PreventAutoClose = true;

				if (this.ContainsCommand (Res.Commands.Dialog.Generic.Cancel))
				{
					this.dialogWindow.WindowCloseClicked += this.HandleWindowCloseClicked;
				}
				else
				{
					this.dialogWindow.MakeButtonlessWindow ();
				}

				this.dialogWindow.Root.ValidationGroups = "Accept";
				
				CommandDispatcher.SetDispatcher (this.dialogWindow, this.CommandDispatcher);
				CommandContext.SetContext (this.dialogWindow, this.CommandContext);
			}
		}

		private bool ContainsCommand(Command command)
		{
			if (this.dialogWindow != null)
			{
				if (this.dialogWindow.Root.FindCommandWidget (command) != null)
				{
					return true;
				}
			}

			return false;
		}

		private void HandleWindowCloseClicked(object sender)
		{
			this.DialogWindow.Root.ExecuteCommand (Res.Commands.Dialog.Generic.Cancel);
		}

		private void HandleWindowShown(object sender)
		{
			this.OnDialogOpened ();
		}

		private void HandleWindowFocused(object sender)
		{
			this.DialogWindow.WindowFocused -= this.HandleWindowFocused;
			this.SetDefaultFocus ();
		}

		public event EventHandler DialogWindowCreated;
		
		
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
