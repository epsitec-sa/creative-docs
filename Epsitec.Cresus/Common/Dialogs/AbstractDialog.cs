//	Copyright © 2004-2009, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
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
			this.AutoCenterDialog = true;
			
			this.commandDispatcher = new CommandDispatcher ("Dialog", CommandDispatcherLevel.Secondary);
			this.commandContext    = new CommandContext ();
			
			this.useAsyncDisposePattern = true;

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
					this.dialogWindow.Owner = this.OwnerWindow;
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
				return this.ownerWindow;
			}
			set
			{
				if (this.ownerWindow != value)
				{
					this.ownerWindow = value;

					if (this.dialogWindow != null)
					{
						if (this.dialogWindow.Owner != value)
						{
							if (value != null)
							{
								this.dialogWindow.Icon = value.Icon;
							}

							this.dialogWindow.Owner = value;
						}
					}
					
					this.OnWindowOwnerChanged ();
				}
			}
		}

		/// <summary>
		/// Gets the dialog result.
		/// </summary>
		/// <value>The dialog result.</value>
		public DialogResult						Result
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
		/// Gets or sets a value indicating whether this dialog is in fact a
		/// real application window.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if the dialog is a real application window; otherwise, <c>false</c>.
		/// </value>
		public bool								IsApplicationWindow
		{
			get
			{
				return this.isApplicationWindow;
			}
			set
			{
				this.isApplicationWindow = value;
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

		public bool								AutoCenterDialog
		{
			get;
			set;
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
		public void OpenDialog(Window owner = null)
		{
			if (owner != null)
			{
				this.OwnerWindow = owner;
			}
			
			Window window = this.DialogWindow;

			if (window == null)
			{
				throw new System.InvalidOperationException ("Cannot show window");
			}

			System.Diagnostics.Debug.Assert ((Application.IsExecutingAsyncCallbacks == false) || (Application.HasPendingAsyncCallbacks == false));

			owner = this.OwnerWindow;
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

			if (this.AutoCenterDialog)
			{
				Drawing.Rectangle dialogBounds = window.WindowBounds;

				double ox = System.Math.Floor (ownerBounds.Left + (ownerBounds.Width - dialogBounds.Width) / 2);
				double oy = System.Math.Floor (ownerBounds.Top  - (ownerBounds.Height - dialogBounds.Height) / 3 - dialogBounds.Height);

				dialogBounds.Location = new Drawing.Point (ox, oy);

				window.WindowBounds = dialogBounds;
			}

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

				this.dialogWindow = null;

				if (this.useAsyncDisposePattern)
				{
					window.WindowDisposing += sender => this.Dispose ();
					window.AsyncDispose ();
				}
				else
				{
					window.Dispose ();
				}
			}
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose (disposing);
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

		protected void ReuseDialogWindow(Window window)
		{
			this.dialogWindow = window;
		}

		private void CreateDialogWindow()
		{
			this.dialogWindow = this.CreateWindow ();

			if (this.dialogWindow != null)
			{
				this.OnDialogWindowCreated ();

				this.dialogWindow.PreventAutoClose = true;

				if (this.IsApplicationWindow)
				{
					this.dialogWindow.WindowCloseClicked += this.HandleWindowCloseClicked;
					this.dialogWindow.Root.WindowType = WindowType.Document;
				}
				else
				{
					this.dialogWindow.MakeSecondaryWindow ();
					this.dialogWindow.Root.WindowType = WindowType.Dialog;

					if ((this.ContainsCommand (Res.Commands.Dialog.Generic.Cancel)) ||
						(this.ContainsCommand (Res.Commands.Dialog.Generic.Close)))
					{
						this.dialogWindow.WindowCloseClicked += this.HandleWindowCloseClicked;
					}
					else
					{
						WindowStyles styles = this.dialogWindow.Root.WindowStyles;

						styles &= ~WindowStyles.HasCloseButton;

						this.dialogWindow.Root.WindowStyles = styles;
					}
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
			if (this.ContainsCommand (Res.Commands.Dialog.Generic.Cancel))
			{
				this.DialogWindow.Root.ExecuteCommand (Res.Commands.Dialog.Generic.Cancel);
			}
			else if (this.ContainsCommand (Res.Commands.Dialog.Generic.Close))
			{
				this.DialogWindow.Root.ExecuteCommand (Res.Commands.Dialog.Generic.Close);
			}
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

		private Window							ownerWindow;
		private Window							dialogWindow;
		private DialogResult					dialogResult;

		protected bool							useAsyncDisposePattern;

		private readonly CommandDispatcher		commandDispatcher;
		private readonly CommandContext			commandContext;

		private bool							isModalDialog;
		private bool							isApplicationWindow;
	}
}
