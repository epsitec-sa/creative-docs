//	Copyright © 2004-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

using Epsitec.Common.Dialogs;
using Epsitec.Common.Support;
using Epsitec.Common.Widgets;
using Epsitec.Common.UI;

namespace Epsitec.Cresus.ServerManager
{
	/// <summary>
	/// La classe Application démarre le gestionnaire du serveur.
	/// </summary>
	public class Application
	{
		private Application()
		{
			this.service_controller = new WindowsServiceController ();
			this.serviceSettings = new Entities.ServiceSettingsEntity ();
			
			Dialog dialog = Dialog.Load (Res.Manager, FormIds.ServiceSettings);

			dialog.CommandDispatcher.RegisterController (this);
			dialog.IsModal = false;
			dialog.Data = new DialogData (this.serviceSettings, DialogDataMode.Transparent);

//-			ObsoleteRecord record = new ObsoleteRecord ("Rec");
			
//-			record.AddField ("IsRunning", false);
			
#if false
			dialog.Load ("ServerManager.MainWindow");
			dialog.AddController (this);
			dialog.IsModal = false;
			dialog.Data = record;
#endif
			
			this.main_window = dialog.DialogWindow;
			this.dialog = dialog;
//-			this.record = record;
			this.timer = new Timer ();
			
			this.timer.AutoRepeat = 0.5;
			this.timer.Delay = 0.5;
			this.timer.HigherAccuracy = false;
			this.timer.TimeElapsed += new EventHandler (this.HandleTimerTimeElapsed);
			
			this.timer.Start ();
		}
		
		
		public Window							MainWindow
		{
			get
			{
				return this.main_window;
			}
		}
		
		
		private void HandleTimerTimeElapsed(object sender)
		{
			this.serviceSettings.IsServiceRunning = this.service_controller.IsRunning;
#if false
			ObsoleteField field = this.record["IsRunning"];
			field.Value = this.service_controller.IsRunning;
			System.Diagnostics.Debug.WriteLine ("field set to " + field.Value);
#endif
		}
		
		
		#region Application Startup
		[System.STAThread] static void Main() 
		{
			Epsitec.Common.Document.Engine.Initialize ();
			Epsitec.Common.Support.Resources.DefaultManager.DefineDefaultModuleName ("ServerManager");
			Epsitec.Common.Widgets.Adorners.Factory.SetActive ("LookRoyale");
			
			Application.application = new Application ();
			Application.application.MainWindow.Run ();
		}
		#endregion


		[Command ("QuitApplication")]
		[Command (ApplicationCommands.Id.Quit)]
		void CommandQuitApplication()
		{
			this.main_window.Quit ();
		}
		
		[Command (Res.CommandIds.StartService)]
		void CommandStartService()
		{
			this.service_controller.Start ();
		}

		[Command (Res.CommandIds.StopService)]
		void CommandStopService()
		{
			this.service_controller.Stop ();
		}
		
		
		private static Application				application;
		private Window							main_window;
		private WindowsServiceController		service_controller;
		private Entities.ServiceSettingsEntity	serviceSettings;
		private Dialog							dialog;
		private Timer							timer;
	}
}
