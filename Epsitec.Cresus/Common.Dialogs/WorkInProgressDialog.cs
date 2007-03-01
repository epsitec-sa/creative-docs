//	Copyright © 2007, EPSITEC SA, CH-1092 BELMONT, Switzerland
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
		public WorkInProgressDialog(string title, bool cancellable)
		{
			this.dialogTitle = title;
			this.cancellable = cancellable;

			this.privateDispatcher = new CommandDispatcher ("Dialog", CommandDispatcherLevel.Secondary);
			this.privateContext    = new CommandContext ();

			this.privateDispatcher.RegisterController (this);
		}

		public System.Action<IWorkInProgressReport> Action
		{
			get
			{
				return this.action;
			}
			set
			{
				this.action = value;
			}
		}

		public bool UnknownProgressValue
		{
			get
			{
				return this.unknownProgressValue;
			}
			set
			{
				if (this.unknownProgressValue != value)
				{
					this.unknownProgressValue = value;

					//	TODO: modifier l'interface graphique en conséquence, si elle
					//	a déjà été créée; cela permet de remplacer un widget avec barre
					//	d'avancement par un widget animé simple, sans information de
					//	durée ou de progrès.
				}
			}
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

			//	TODO: en fonction de this.unknownProgressValue, crée soit un widget qui
			//	permet d'afficher le progrès de l'opération (sorte de slider), soit un
			//	widget qui affiche juste une animation "générale", sans information de
			//	progrès... On peut utiliser le timer interne ci-après pour faire un
			//	refresh du widget qui affiche l'animation générale, ou alors décider
			//	d'implémenter ce widget de manière 100% autonome, avec refresh automatique
			//	de son animation, sans intervention externe.
			
			this.progressValueSlider = new HSlider (frame);
			this.progressValueSlider.Dock = DockStyle.Stacked;
			this.progressValueSlider.PreferredHeight = 20;
			this.progressValueSlider.Margins = new Epsitec.Common.Drawing.Margins (4, 4, 4, 4);
			this.progressValueSlider.MinValue = 0.0M;
			this.progressValueSlider.MaxValue = 1.0M;

			this.progressMessageWidget = new StaticText (frame);
			this.progressMessageWidget.Dock = DockStyle.Stacked;
			this.progressMessageWidget.PreferredHeight = 32;
			this.progressMessageWidget.Margins = new Epsitec.Common.Drawing.Margins (4, 4, 4, 4);

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
					this.progressValueSlider.Value = (decimal) progressValue;
				};

			this.timer.AutoRepeat = 0.050;
			this.timer.Delay      = 0.050;
		}

		protected override void OnDialogOpened()
		{
			base.OnDialogOpened ();

			if (this.action != null)
			{
				this.timer.Start ();

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

			this.timer.Stop ();
		}

		protected void CancelOperation()
		{
			this.cancelled = true;
		}

		private void HandleCancelButtonClicked(object sender, MessageEventArgs e)
		{
			this.cancelled = true;
			this.cancelButton.Enable = false;
		}

		private void ProcessAction()
		{
			this.action (this);

			SimpleCallback callback = this.CloseDialog;

			Drawing.Platform.Dispatcher.Invoke (callback);
		}

		[Command ("QuitDialog")]
		protected void CommandQuitDialog()
		{
			this.result = DialogResult.Cancel;

			this.CloseDialog ();
		}

		private readonly object exclusion = new object ();

		private System.Action<IWorkInProgressReport> action;
		private string dialogTitle;
		
		private CommandDispatcher privateDispatcher;
		private CommandContext privateContext;
		private StaticText operationMessageWidget;
		private HSlider progressValueSlider;
		private StaticText progressMessageWidget;
		private Button cancelButton;
		private Timer timer;

		private string							operationMessage;
		private string							progressMessage;
		private double							progressValue;
		private bool							cancelled;
		private bool							cancellable;
		private bool							unknownProgressValue;
	}
}
