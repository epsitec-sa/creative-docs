using Epsitec.Common.Designer;
using Epsitec.Common.Drawing;
using Epsitec.Common.Support;
using Epsitec.Common.Widgets;

namespace Epsitec.Designer
{
	/// <summary>
	/// La classe Application démarre le designer.
	/// </summary>
	public class Application : Epsitec.Common.Designer.Application
	{
		public Application(bool runs_as_plug_in)
		{
			this.runs_as_plug_in = runs_as_plug_in;
			this.name = "Designer";
			
//			Button button;
//			
//			button = new Button ("New bundle");
//			button.Bounds  = new Rectangle (10, 10, 80, 24);
//			button.Command = "CreateStringBundle";
//			button.Parent  = this.MainWindow.Root;
//			button.CreateCommandState ();
//			
//			button = new Button ("Open bundle");
//			button.Bounds  = new Rectangle (100, 10, 80, 24);
//			button.Command = "OpenBundle";
//			button.Parent  = this.MainWindow.Root;
//			button.CreateCommandState ();
//			
//			button = new Button ("Save bundle");
//			button.Bounds  = new Rectangle (190, 10, 80, 24);
//			button.Command = "SaveStringBundle";
//			button.Parent  = this.MainWindow.Root;
//			button.CreateCommandState ();
			
			this.MainWindow.Show ();
		}
		
		#region Application Commands
		[Command ("QuitDesigner")] void CommandQuitDesigner()
		{
			if (this.runs_as_plug_in)
			{
				this.MainWindow.Close ();
			}
			else
			{
				this.MainWindow.Quit ();
			}
		}
		#endregion
		
		#region Application Startup
		[System.STAThread] static void Main() 
		{
			Epsitec.Common.Widgets.Adorner.Factory.SetActive ("LookMetal");
			
			Application.application = new Application (false);
			Application.application.MainWindow.Run ();
		}
		#endregion
		
		public static void StartAsPlugIn()
		{
			Application.application = new Application (true);
		}
		
		
		private bool							runs_as_plug_in;
	}
}
