//	Copyright � 2007, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Widgets;
using Epsitec.Common.Support;

namespace Epsitec.Common.Dialogs
{
	/// <summary>
	/// The <c>WorkInProgressDialog</c> class implements a dialog which displays
	/// a progress message to the user, while some background thread is working.
	/// The background thread can be cancelled.
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

					//	TODO: modifier l'interface graphique en cons�quence, si elle
					//	a d�j� �t� cr��e; cela permet de remplacer un widget avec barre
					//	d'avancement par un widget anim� simple, sans information de
					//	dur�e ou de progr�s.
				}
			}
		}

		/// <summary>
		/// Gets a value indicating whether the operation was cancelled.
		/// </summary>
		/// <value><c>true</c> if the operation was cancelled; otherwise, <c>false</c>.</value>
		public bool Cancelled
		{
			get
			{
				return this.cancelled;
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
			dialog.Owner = owner;
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

		bool IWorkInProgressReport.Cancelled
		{
			get
			{
				lock (this.exclusion)
				{
					return this.cancelled;
				}
			}
		}

		#endregion
		
		protected override void CreateWindow()
		{
			this.window = new Window ();

			this.window.Text              = this.dialogTitle;
			this.window.Name              = "Dialog";
			this.window.ClientSize        = new Drawing.Size (320, 200);
			this.window.PreventAutoClose  = true;

			CommandDispatcher.SetDispatcher (this.window, this.privateDispatcher);
			CommandContext.SetContext (this.window, this.privateContext);

			this.window.MakeFramelessWindow ();
			this.window.MakeFloatingWindow ();

			FrameBox frame = new FrameBox (this.window.Root);
			
			frame.DrawFullFrame = true;
			frame.Dock = DockStyle.Fill;
			frame.ContainerLayoutMode = ContainerLayoutMode.VerticalFlow;

			StaticText textTitle = new StaticText (frame);

			textTitle.Dock = DockStyle.Stacked;
			textTitle.PreferredHeight = 32;
			textTitle.ContentAlignment = Epsitec.Common.Drawing.ContentAlignment.MiddleCenter;
			textTitle.Text = string.Concat (@"<font size=""120%"">", this.dialogTitle, @"</font>");

			this.operationMessageWidget = new StaticText (frame);
			this.operationMessageWidget.Dock = DockStyle.Stacked;
			this.operationMessageWidget.PreferredHeight = 32;
			this.operationMessageWidget.Margins = new Epsitec.Common.Drawing.Margins (4, 4, 4, 4);

			//	TODO: en fonction de this.unknownProgressValue, cr�e soit un widget qui
			//	permet d'afficher le progr�s de l'op�ration (sorte de slider), soit un
			//	widget qui affiche juste une animation "g�n�rale", sans information de
			//	progr�s... On peut utiliser le timer interne ci-apr�s pour faire un
			//	refresh du widget qui affiche l'animation g�n�rale, ou alors d�cider
			//	d'impl�menter ce widget de mani�re 100% autonome, avec refresh automatique
			//	de son animation, sans intervention externe.
			
			this.progressIndicator = new ProgressIndicator (frame);
			this.progressIndicator.Dock = DockStyle.Stacked;
			this.progressIndicator.PreferredHeight = 20;
			this.progressIndicator.Margins = new Epsitec.Common.Drawing.Margins (4, 4, 4, 4);

			this.progressMessageWidget = new StaticText (frame);
			this.progressMessageWidget.Dock = DockStyle.Stacked;
			this.progressMessageWidget.PreferredHeight = 32;
			this.progressMessageWidget.Margins = new Epsitec.Common.Drawing.Margins (4, 4, 4, 4);

			this.startTicks = System.Environment.TickCount;

			if (this.cancellable)
			{
				this.cancelButton = new Button (frame);
				this.cancelButton.Dock = DockStyle.StackEnd;
				this.cancelButton.HorizontalAlignment = HorizontalAlignment.Center;
				this.cancelButton.Margins = new Epsitec.Common.Drawing.Margins (0, 0, 16, 8);
				this.cancelButton.Clicked += this.HandleCancelButtonClicked;
				this.cancelButton.Text = Res.Strings.Dialog.Generic.Button.Cancel;
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

					this.operationMessageWidget.Text = operationMessage;
					this.progressMessageWidget.Text = progressMessage;

					switch (this.progressIndicatorStyle)
					{
						case ProgressIndicatorStyle.Default:
							this.progressIndicator.Value = progressValue;
							break;

						case ProgressIndicatorStyle.UnknownDuration:
							this.progressIndicator.Value = ((double)(System.Environment.TickCount-this.startTicks) / 2000.0 + 0.5) % 1.0;
							break;
					}
				};

			this.timer.AutoRepeat = 0.050;
			this.timer.Delay      = 0.050;
		}

		protected override void OnDialogOpened()
		{
			base.OnDialogOpened ();

			if (this.operation != null)
			{
				this.timer.Start ();
				Window.SuspendAsyncNotify ();

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
			}
		}

		protected void CancelOperation()
		{
			this.cancelled = true;

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
				this.operationResult = this.cancelled ? OperationResult.Cancelled : OperationResult.Done;
			}
			catch (System.Exception ex)
			{
				this.operationException = ex;
				this.operationResult = OperationResult.Error;
			}

			SimpleCallback callback = this.CloseDialog;

			Drawing.Platform.Dispatcher.Invoke (callback);
		}

		[Command ("QuitDialog")]
		protected void CommandQuitDialog()
		{
			this.result = DialogResult.Cancel;

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
		private int								startTicks;
		private bool							cancelled;
		private bool							cancellable;
		private ProgressIndicatorStyle			progressIndicatorStyle;
		private OperationResult					operationResult;

		private System.Exception				operationException;
	}
}
