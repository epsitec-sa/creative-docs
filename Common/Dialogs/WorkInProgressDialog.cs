//	Copyright © 2007-2009, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System;
using System.Threading;
using System.Threading.Tasks;
using Epsitec.Common.Support;
using Epsitec.Common.Widgets;
using Epsitec.Common.Widgets.Platform;

namespace Epsitec.Common.Dialogs
{
    /// <summary>
    /// The <c>WorkInProgressDialog</c> class implements a dialog which displays
    /// a progress message to the user, while some background thread is working.
    /// The background thread can be canceled.
    /// </summary>
    public class WorkInProgressDialog : AbstractMessageDialog, IWorkInProgressReport
    {
        public delegate Task WIPTaskDelegate(IWorkInProgressReport report, CancellationToken ct);

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkInProgressDialog"/> class.
        /// </summary>
        /// <param name="title">The title displayed in the dialog.</param>
        /// <param name="cancellable">if set to <c>true</c> the action is cancellable.</param>
        public WorkInProgressDialog(string title, bool cancellable, WIPTaskDelegate operation)
        {
            this.dialogTitle = title;
            this.cancellable = cancellable;
            this.operation = operation;

            this.privateDispatcher = new CommandDispatcher(
                "Dialog",
                CommandDispatcherLevel.Secondary
            );
            this.privateContext = new CommandContext();

            this.privateDispatcher.RegisterController(this);
        }

        /// <summary>
        /// Gets or sets the progress indicator style.
        /// </summary>
        /// <value>The progress indicator style.</value>
        public ProgressIndicatorStyle ProgressIndicatorStyle
        {
            get { return this.progressIndicatorStyle; }
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
        /// Gets the operation result.
        /// </summary>
        /// <value>The operation result.</value>
        public OperationResult OperationResult
        {
            get { return this.operationResult; }
        }

        /// <summary>
        /// Gets the exception thrown by the operation if the operation result
        /// is set to <code>OperationResult.Error</code>.
        /// </summary>
        /// <value>The operation exception.</value>
        public Exception OperationException
        {
            get { return this.operationException; }
        }

        public static OperationResult Execute(
            string title,
            ProgressIndicatorStyle style,
            WIPTaskDelegate action
        )
        {
            return WorkInProgressDialog.Execute(title, style, action, null);
        }

        public static OperationResult Execute(
            string title,
            ProgressIndicatorStyle style,
            WIPTaskDelegate action,
            Window owner
        )
        {
            return WorkInProgressDialog.Execute(title, style, action, owner, false);
        }

        public static OperationResult ExecuteCancellable(
            string title,
            ProgressIndicatorStyle style,
            WIPTaskDelegate action
        )
        {
            return WorkInProgressDialog.ExecuteCancellable(title, style, action, null);
        }

        public static OperationResult ExecuteCancellable(
            string title,
            ProgressIndicatorStyle style,
            WIPTaskDelegate action,
            Window owner
        )
        {
            return WorkInProgressDialog.Execute(title, style, action, owner, true);
        }

        private static OperationResult Execute(
            string title,
            ProgressIndicatorStyle style,
            WIPTaskDelegate action,
            Window owner,
            bool cancellable
        )
        {
            WorkInProgressDialog dialog = new WorkInProgressDialog(title, cancellable, action);
            dialog.OwnerWindow = owner;
            dialog.ProgressIndicatorStyle = style;
            dialog.OpenDialog();
            return dialog.OperationResult;
        }

        #region IWorkInProgressReport Members

        void IWorkInProgressReport.DefineOperation(string formattedText)
        {
            this.operationMessage = formattedText;
        }

        void IWorkInProgressReport.DefineProgress(double value, string formattedText)
        {
            this.progressValue = value;
            this.progressMessage = formattedText;
        }

        bool IWorkInProgressReport.Canceled
        {
            get { return this.operationResult == OperationResult.Canceled; }
        }

        #endregion

        protected override Window CreateWindow()
        {
            Window dialogWindow = new Window(WindowFlags.HideFromTaskbar | WindowFlags.NoBorder);

            dialogWindow.Text = this.dialogTitle;
            dialogWindow.Name = "Dialog";
            dialogWindow.ClientSize = new Drawing.Size(400, 150);
            dialogWindow.PreventAutoClose = true;

            CommandDispatcher.SetDispatcher(dialogWindow, this.privateDispatcher);
            CommandContext.SetContext(dialogWindow, this.privateContext);

            FrameBox frame = new FrameBox(dialogWindow.Root);
            frame.DrawFullFrame = false;
            frame.Dock = DockStyle.Fill;
            frame.ContainerLayoutMode = ContainerLayoutMode.VerticalFlow;

            this.operationMessageWidget = new StaticText(frame);
            this.operationMessageWidget.PreferredHeight = 30;
            this.operationMessageWidget.Dock = DockStyle.Stacked;
            this.operationMessageWidget.Margins = new Drawing.Margins(10, 10, 15, 0);

            this.progressMessageWidget = new StaticText(frame);
            this.progressMessageWidget.Dock = DockStyle.Stacked;
            this.progressMessageWidget.Margins = new Drawing.Margins(10, 10, 0, 0);

            this.progressIndicator = new ProgressIndicator(frame);
            this.progressIndicator.ProgressStyle = this.progressIndicatorStyle;
            this.progressIndicator.Dock = DockStyle.Stacked;
            this.progressIndicator.Margins = new Drawing.Margins(10, 10, 15, 0);

            if (this.cancellable)
            {
                this.cancelButton = new Button(frame);
                this.cancelButton.Dock = DockStyle.StackEnd;
                this.cancelButton.HorizontalAlignment = HorizontalAlignment.Center;
                this.cancelButton.Margins = new Drawing.Margins(0, 0, 16, 8);
                this.cancelButton.Clicked += this.HandleCancelButtonClicked;
                this.cancelButton.Text = Res.Strings.Dialog.Generic.Button.Cancel.ToString();
            }

            this.timer = new Widgets.Platform.Timer();
            this.timer.TimeElapsed += delegate(object sender)
            {
                this.operationMessageWidget.Text = string.Concat(
                    @"<font size=""150%"">",
                    this.operationMessage,
                    "</font>"
                );
                this.progressMessageWidget.Text = this.progressMessage;

                switch (this.progressIndicatorStyle)
                {
                    case ProgressIndicatorStyle.Default:
                        this.progressIndicator.ProgressValue = this.progressValue;
                        break;

                    case ProgressIndicatorStyle.UnknownDuration:
                        this.progressIndicator.UpdateProgress();
                        break;
                }
            };

            this.timer.AutoRepeat = true;
            this.timer.Period = 0.050;

            return dialogWindow;
        }

        protected override void OnDialogOpened()
        {
            base.OnDialogOpened();

            if (this.operation != null)
            {
                this.timer.Start();
                Window.SuspendAsyncNotify();
                Application.SetWaitCursor();

                this.DialogWindow.MouseCursor = MouseCursor.AsWait;

                this.ProcessAction();
            }
        }

        protected override void OnDialogClosed()
        {
            base.OnDialogClosed();

            if (this.timer.State == TimerState.Running)
            {
                this.timer.Stop();
                Window.ResumeAsyncNotify();
                Application.ClearWaitCursor();
            }
        }

        protected void CancelOperation()
        {
            this.cancelTokenSource?.Cancel();

            if (this.cancelButton != null)
            {
                this.cancelButton.Enable = false;
            }
        }

        private void HandleCancelButtonClicked(object sender, MessageEventArgs e)
        {
            this.CancelOperation();
        }

        private void ProcessAction()
        {
            this.operationResult = OperationResult.Pending;
            this.cancelTokenSource = new CancellationTokenSource();
            this.operation(this, this.cancelTokenSource.Token).ContinueWith(this.OnOperationDone);
        }

        private void OnOperationDone(Task operationTask)
        {
            // inspired from https://stackoverflow.com/questions/21520869/proper-way-of-handling-exception-in-task-continuewith
            if (operationTask.IsFaulted)
            {
                Exception ex = operationTask.Exception;
                while (ex is AggregateException && ex.InnerException != null)
                {
                    ex = ex.InnerException;
                }
                this.operationException = ex;
                this.operationResult = OperationResult.Error;
            }
            else if (operationTask.IsCanceled)
            {
                this.operationResult = OperationResult.Canceled;
                this.Result = DialogResult.Cancel;
            }
            else
            {
                this.operationResult = OperationResult.Done;
            }
            this.DialogWindow.GenerateCloseEvent();
        }

        [Command(Res.CommandIds.Dialog.Generic.Cancel)]
        protected void CommandQuitDialog()
        {
            this.CancelOperation();
        }

        private WIPTaskDelegate operation;
        private CancellationTokenSource cancelTokenSource;
        private string dialogTitle;

        private CommandDispatcher privateDispatcher;
        private CommandContext privateContext;

        private StaticText operationMessageWidget;
        private ProgressIndicator progressIndicator;
        private StaticText progressMessageWidget;
        private Button cancelButton;
        private Widgets.Platform.Timer timer;

        private string operationMessage;
        private string progressMessage;
        private double progressValue;
        private bool cancellable;
        private ProgressIndicatorStyle progressIndicatorStyle;
        private OperationResult operationResult;

        private Exception operationException;
    }
}
