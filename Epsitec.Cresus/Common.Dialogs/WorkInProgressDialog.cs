using Epsitec.Common.Widgets;
using Epsitec.Common.Support;

namespace Epsitec.Common.Dialogs
{
	public class WorkInProgressDialog : AbstractMessageDialog
	{
		public WorkInProgressDialog(string title)
		{
			this.dialogTitle = title;

			this.privateDispatcher = new CommandDispatcher ("Dialog", CommandDispatcherLevel.Secondary);
			this.privateContext    = new CommandContext ();

			this.privateDispatcher.RegisterController (this);
		}

		public System.Action<WorkInProgressDialog> Action
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

		public void DefineMessage(string message)
		{
			lock (this.exclusion)
			{
				this.message = message;
			}
		}

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

			StaticText textTitle = new StaticText (frame);
			
			textTitle.Dock = DockStyle.Top;
			textTitle.PreferredHeight = 32;
			textTitle.ContentAlignment = Epsitec.Common.Drawing.ContentAlignment.MiddleCenter;
			textTitle.Text = string.Concat (@"<font size=""120%"">", this.dialogTitle, @"</font>");

			this.textMessage = new StaticText (frame);
			this.textMessage.Dock = DockStyle.Top;
			this.textMessage.PreferredHeight = 32;
			this.textMessage.Margins = new Epsitec.Common.Drawing.Margins (4, 4, 4, 4);

			this.timer = new Timer ();
			this.timer.TimeElapsed +=
				delegate (object sender)
				{
					string message;
					
					lock (this.exclusion)
					{
						message = this.message;
					}

					this.textMessage.Text = message;
				};

			this.timer.AutoRepeat = 0.1;
			this.timer.Delay = 0.1;
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

		private System.Action<WorkInProgressDialog> action;
		private string dialogTitle;
		
		private CommandDispatcher privateDispatcher;
		private CommandContext privateContext;
		private volatile string message;
		private StaticText textMessage;
		private Timer timer;
	}
}
