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
		public Application()
		{
			Epsitec.Common.Widgets.Adorner.Factory.SetActive ("LookMetal");
			
			Button button;
			
			button = new Button ("New bundle");
			button.Bounds  = new Rectangle (10, 10, 80, 24);
			button.Command = "CreateStringBundle";
			button.Parent  = this.MainWindow.Root;
			button.Anchor  = AnchorStyles.BottomLeft;
			button.CreateCommandState ();
			
			button = new Button ("Open bundle");
			button.Bounds  = new Rectangle (100, 10, 80, 24);
			button.Command = "OpenStringBundle";
			button.Parent  = this.MainWindow.Root;
			button.Anchor  = AnchorStyles.BottomLeft;
			button.CreateCommandState ();
			
			button = new Button ("Save bundle");
			button.Bounds  = new Rectangle (190, 10, 80, 24);
			button.Command = "SaveStringBundle";
			button.Parent  = this.MainWindow.Root;
			button.Anchor  = AnchorStyles.BottomLeft;
			button.CreateCommandState ();
			
			this.StringEditController.Window.Show ();
		}
		
		#region Application Commands
		[Command ("QuitDesigner")] void CommandQuitDesigner()
		{
			this.MainWindow.Quit ();
		}
		#endregion
		
		#region Application Startup
		[System.STAThread] static void Main() 
		{
			Application.application = new Application ();
			Application.application.MainWindow.Show ();
			Application.application.MainWindow.Run ();
		}
		#endregion
		
		static Designer.Application		application;
	}
}
