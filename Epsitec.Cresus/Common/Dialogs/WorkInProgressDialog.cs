//	Copyright © 2007-2009, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Widgets;
using Epsitec.Common.Support;

namespace Epsitec.Common.Dialogs
{
	/// <summary>
	/// The <c>WorkInProgressDialog</c> class implements a dialog which displays
	/// a progress message to the user, while some background thread is working.
	/// The background thread can be canceled.
	/// </summary>
	public class WorkInProgressDialog : AbstractMessageDialog, IWorkInProgressReport
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="WorkInProgressDialog"/> class.
		/// </summary>
		/// <param name="title">The title displayed in the dialog.</param>
		/// <param name="cancellable">if set to <c>true</c> the action is cancellable.</param>
		public WorkInProgressDialog(string title, bool cancellable)
		{
			this.dialogTitle = title;
			this.cancellable = cancellable;

			this.privateDispatcher = new CommandDispatcher ("Dialog", CommandDispatcherLevel.Secondary);
			this.privateContext    = new CommandContext ();

			this.privateDispatcher.RegisterController (this);
		}

		/// <summary>
		/// Gets or sets the action which will be executed when the dialog
		/// is shown.
		/// </summary>
		/// <value>The action.</value>
		public System.Action<IWorkInProgressReport> Operation
		{
			get
			{
				return this.operation;
			}
			set
			{
				this.operation = value;
			}
		}

		/// <summary>
		/// Gets or sets the progress indicator style.
		/// </summary>
		/// <value>The progress indicator style.</value>
		public ProgressIndicatorStyle ProgressIndicatorStyle
		{
			get
			{
				return this.progressIndicatorStyle;
			}
			set
			{
				if (this.progressIndicatorStyle != value)
				{
					this.progressIndicatorStyle = value;

					if (this.progressIndicator != null)
					{
						this.progressIndicator.ProgressStyle = this.progressIndicatorStyle;
					}
				}
			}
		}

		/// <summary>
		/// Gets a value indicating whether the operation was canceled.
		/// </summary>
		/// <value><c>true</c> if the operation was canceled; otherwise, <c>false</c>.</value>
		public bool Canceled
		{
			get
			{
				return this.canceled;
			}
		}

		/// <summary>
		/// Gets the operation result.
		/// </summary>
		/// <value>The operation result.</value>
		public OperationResult OperationResult
		{
			get
			{
				return this.operationResult;
			}
		}

		/// <summary>
		/// Gets the exception thrown by the operation if the operation result
		/// is set to <code>OperationResult.Error</code>.
		/// </summary>
		/// <value>The operation exception.</value>
		public System.Exception OperationException
		{
			get
			{
				return this.operationException;
			}
		}


		public static OperationResult Execute(string title, ProgressIndicatorStyle style, System.Action<IWorkInProgressReport> action)
		{
			return WorkInProgressDialog.Execute (title, style, action, null);
		}

		public static OperationResult Execute(string title, ProgressIndicatorStyle style, System.Action<IWorkInProgressReport> action, Window owner)
		{
			return WorkInProgressDialog.Execute (title, style, action, owner, false);
		}

		public static OperationResult ExecuteCancellable(string title, ProgressIndicatorStyle style, System.Action<IWorkInProgressReport> action)
		{
			return WorkInProgressDialog.ExecuteCancellable (title, style, action, null);
		}

		public static OperationResult ExecuteCancellable(string title, ProgressIndicatorStyle style, System.Action<IWorkInProgressReport> action, Window owner)
		{
			return WorkInProgressDialog.Execute (title, style, action, owner, true);
		}

		private static OperationResult Execute(string title, ProgressIndicatorStyle style, System.Action<IWorkInProgressReport> action, Window owner, bool cancellable)
		{
			WorkInProgressDialog dialog = new WorkInProgressDialog (title, cancellable);
			dialog.Operation = action;
			dialog.OwnerWindow = owner;
			dialog.ProgressIndicatorStyle = style;
			dialog.OpenDialog ();
			return dialog.OperationResult;
		}



		#region IWorkInProgressReport Members

		void IWorkInProgressReport.DefineOperation(string formattedText)
		{
			lock (this.exclusion)
			{
				this.operationMessage = formattedText;
			}
		}

		void IWorkInProgressReport.DefineProgress(double value, string formattedText)
		{
			lock (this.exclusion)
			{
				this.progressValue = value;
				this.progressMessage = formattedText;
			}
		}

		bool IWorkInProgressReport.Canceled
		{
			get
			{
				lock (this.exclusion)
				{
					return this.canceled;
				}
			}
		}

		#endregion
		
		protected override Window CreateWindow()
		{
			Window dialogWindow = new Window ();

			dialogWindow.Text             = this.dialogTitle;
			dialogWindow.Name             = "Dialog";
			dialogWindow.ClientSize       = new Drawing.Size (400, 150);
			dialogWindow.PreventAutoClose = true;

			CommandDispatcher.SetDispatcher (dialogWindow, this.privateDispatcher);
			CommandContext.SetContext (dialogWindow, this.privateContext);

			dialogWindow.MakeFixedSizeWindow ();
			dialogWindow.MakeSecondaryWindow ();
			dialogWindow.MakeButtonlessWindow ();

			FrameBox frame = new FrameBox (dialogWindow.Root);
			frame.DrawFullFrame = false;
			frame.Dock = DockStyle.Fill;
			frame.ContainerLayoutMode = ContainerLayoutMode.VerticalFlow;

#if false
			StaticText textTitle = new StaticText (frame);
			textTitle.Dock = DockStyle.Stacked;
			textTitle.PreferredHeight = 32;
			textTitle.ContentAlignment = Epsitec.Common.Drawing.ContentAlignment.MiddleCenter;
			textTitle.Text = string.Concat (@"<font size=""120%"">", this.dialogTitle, @"</font>");
#endif

			this.operationMessageWidget = new StaticText (frame);
			this.operationMessageWidget.PreferredHeight = 30;
			this.operationMessageWidget.Dock = DockStyle.Stacked;
			this.operationMessageWidget.Margins = new Epsitec.Common.Drawing.Margins (10, 10, 15, 0);

			this.progressMessageWidget = new StaticText (frame);
			this.progressMessageWidget.Dock = DockStyle.Stacked;
			this.progressMessageWidget.Margins = new Epsitec.Common.Drawing.Margins (10, 10, 0, 0);

			this.progressIndicator = new ProgressIndicator (frame);
			this.progressIndicator.ProgressStyle = this.progressIndicatorStyle;
			this.progressIndicator.Dock = DockStyle.Stacked;
			this.progressIndicator.Margins = new Epsitec.Common.Drawing.Margins (10, 10, 15, 0);

			if (this.cancellable)
			{
				this.cancelButton = new Button (frame);
				this.cancelButton.Dock = DockStyle.StackEnd;
				this.cancelButton.HorizontalAlignment = HorizontalAlignment.Center;
				this.cancelButton.Margins = new Epsitec.Common.Drawing.Margins (0, 0, 16, 8);
				this.cancelButton.Clicked += this.HandleCancelButtonClicked;
				this.cancelButton.Text = Res.Strings.Dialog.Generic.Button.Cancel.ToString ();
			}

			this.timer = new Timer ();
			this.timer.TimeElapsed +=
				delegate (object sender)
				{
					string operationMessage;
					string progressMessage;
					double progressValue;
					
					lock (this.exclusion)
					{
						operationMessage = this.operationMessage;
						progressMessage  = this.progressMessage;
						progressValue    = this.progressValue;
					}

					this.operationMessageWidget.Text = string.Concat(@"<font size=""150%"">", operationMessage, "</font>");
					this.progressMessageWidget.Text = progressMessage;

					switch (this.progressIndicatorStyle)
					{
						case ProgressIndicatorStyle.Default:
							this.progressIndicator.ProgressValue = progressValue;
							break;

						case ProgressIndicatorStyle.UnknownDuration:
							this.progressIndicator.UpdateProgress ();
							break;
					}
				};

			this.timer.AutoRepeat = 0.050;
			this.timer.Delay      = 0.050;

			return dialogWindow;
		}

		protected override void OnDialogOpened()
		{
			base.OnDialogOpened ();

			if (this.operation != null)
			{
				this.timer.Start ();
				Window.SuspendAsyncNotify ();
				Application.SetWaitCursor ();

				this.DialogWindow.MouseCursor = MouseCursor.AsWait;

				System.Threading.Thread thread = new System.Threading.Thread (this.ProcessAction);
				
				thread.Name = "Process Action";
				thread.Start ();
			}
			else
			{
				this.CloseDialog ();
			}
		}

		protected override void OnDialogClosed()
		{
			base.OnDialogClosed ();

			if (this.timer.State == TimerState.Running)
			{
				this.timer.Stop ();
				Window.ResumeAsyncNotify ();
				Application.ClearWaitCursor ();
			}
		}

		protected void CancelOperation()
		{
			this.canceled = true;

			if (this.cancelButton != null)
			{
				this.cancelButton.Enable = false;
			}
		}

		private void HandleCancelButtonClicked(object sender, MessageEventArgs e)
		{
			this.CancelOperation ();
		}

		private void ProcessAction()
		{
			this.operationResult = OperationResult.Pending;

			try
			{
				this.operation (this);
				this.operationResult = this.canceled ? OperationResult.Canceled : OperationResult.Done;
			}
			catch (System.Exception ex)
			{
				this.operationException = ex;
				this.operationResult = OperationResult.Error;
			}

			SimpleCallback callback = this.CloseDialog;

			Drawing.Platform.Dispatcher.Invoke (callback);
		}

		[Command (Res.CommandIds.Dialog.Generic.Cancel)]
		protected void CommandQuitDialog()
		{
			this.Result = DialogResult.Cancel;

			this.CloseDialog ();
		}

		private readonly object					exclusion = new object ();

		private System.Action<IWorkInProgressReport> operation;
		private string							dialogTitle;
		
		private CommandDispatcher				privateDispatcher;
		private CommandContext					privateContext;
		
		private StaticText						operationMessageWidget;
		private ProgressIndicator				progressIndicator;
		private StaticText						progressMessageWidget;
		private Button							cancelButton;
		private Timer							timer;

		private string							operationMessage;
		private string							progressMessage;
		private double							progressValue;
		private bool							canceled;
		private bool							cancellable;
		private ProgressIndicatorStyle			progressIndicatorStyle;
		private OperationResult					operationResult;

		private System.Exception				operationException;
	}
}
