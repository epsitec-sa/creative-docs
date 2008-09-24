//	Copyright � 2004-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

using Epsitec.Common.Dialogs;
using Epsitec.Common.Support;
using Epsitec.Common.Widgets;
using Epsitec.Common.UI;

namespace Epsitec.Cresus.ServerManager
{
	/// <summary>
	/// La classe Application d�marre le gestionnaire du serveur.
	/// </summary>
	public class Application : Epsitec.Common.Widgets.Application
	{
		private Application()
		{
			this.service_controller = new WindowsServiceController ();
			this.serviceSettings = new Entities.ServiceSettingsEntity ();

			Dialog dialog = Dialog.Load (Res.Manager, FormIds.ServiceSettings);

			dialog.CommandDispatcher.RegisterController (this);
			dialog.IsModal = false;
			dialog.Data = new DialogData (this.serviceSettings, DialogDataMode.Transparent);
			dialog.IsApplicationWindow = true;

			this.dialog = dialog;
			this.Window = dialog.DialogWindow;
			this.Window.Text = this.ShortWindowTitle;
//-			this.Window.Icon = Epsitec.Common.Drawing.Bitmap.FromNativeIcon (Epsitec.Common.Support.Platform.StockIcons.ShieldIcon);

			this.DispatchCommandLineCommands ();
			
			this.timer = new Timer ();
			
			this.timer.AutoRepeat = 0.5;
			this.timer.Delay = 0.5;
			this.timer.HigherAccuracy = false;
			this.timer.TimeElapsed += new EventHandler (this.HandleTimerTimeElapsed);
			
			this.timer.Start ();

			this.dialog.OpenDialog ();
		}


		public override string					ShortWindowTitle
		{
			get
			{
				return Res.Strings.ApplicationTitle.ToSimpleText ();
			}
		}
		
		private void HandleTimerTimeElapsed(object sender)
		{
			this.serviceSettings.IsServiceRunning = this.service_controller.IsRunning;
		}
		
		
		#region Application Startup
		[System.STAThread] static void Main() 
		{
			Epsitec.Common.Document.Engine.Initialize ();
			Epsitec.Common.Support.Resources.DefaultManager.DefineDefaultModuleName ("ServerManager");
			Epsitec.Common.Widgets.Adorners.Factory.SetActive ("LookRoyale");
			
			Application.application = new Application ();
			Application.application.RunMessageLoop ();
		}
		#endregion


		[Command (Res.CommandIds.StartService)]
		void CommandStartService()
		{
			try
			{
				this.service_controller.Start ();
			}
			catch
			{
			}
		}

		[Command (Res.CommandIds.StopService)]
		void CommandStopService()
		{
			try
			{
				this.service_controller.Stop ();
			}
			catch
			{
			}
		}
		
		
		private static Application				application;
		private WindowsServiceController		service_controller;
		private Entities.ServiceSettingsEntity	serviceSettings;
		private Dialog							dialog;
		private Timer							timer;
	}
}
