//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using Epsitec.Common.Dialogs;
using Epsitec.Common.Support;
using Epsitec.Common.Widgets;
using Epsitec.Common.UI;
using Epsitec.Common.UI.Data;

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
			
			Dialog dialog = new Dialog (Epsitec.Common.Support.Resources.DefaultManager);
			ObsoleteRecord record = new ObsoleteRecord ("Rec");
			
			record.AddField ("IsRunning", false);
			
			dialog.Load ("ServerManager.MainWindow");
			dialog.AddController (this);
			dialog.IsModal = false;
			dialog.Data = record;
			
			this.main_window = dialog.Window;
			this.dialog = dialog;
			this.record = record;
			this.timer = new Timer ();
			
			this.timer.AutoRepeat = 0.5;
			this.timer.Delay = 0.5;
			this.timer.HighAccuracy = false;
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
			ObsoleteField field = this.record["IsRunning"];
			field.Value = this.service_controller.IsRunning;
			System.Diagnostics.Debug.WriteLine ("field set to " + field.Value);
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
		void CommandQuitApplication()
		{
			this.main_window.Quit ();
		}
		
		[Command ("StartService")]
		void CommandStartService()
		{
			this.service_controller.Start ();
		}
		
		[Command ("StopService")]
		void CommandStopService()
		{
			this.service_controller.Stop ();
		}
		
		
		private static Application				application;
		private Window							main_window;
		private WindowsServiceController		service_controller;
		private ObsoleteRecord							record;
		private Dialog							dialog;
		private Timer							timer;
	}
}
