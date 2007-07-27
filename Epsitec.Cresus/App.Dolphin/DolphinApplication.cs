using System.Collections.Generic;
using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Types;
using Epsitec.Common.Drawing;

namespace Epsitec.App.Dolphin
{
	/// <summary>
	/// Fenêtre principale de l'application.
	/// </summary>
	public class DolphinApplication : Application
	{
		static DolphinApplication()
		{
			ImageProvider.Default.EnableLongLifeCache = true;
			ImageProvider.Default.PrefillManifestIconCache();
		}

		public DolphinApplication() : this(new ResourceManagerPool("App.Dolphin"))
		{
			this.resourceManagerPool.DefaultPrefix = "file";
			this.resourceManagerPool.SetupDefaultRootPaths();
		}

		public DolphinApplication(ResourceManagerPool pool)
		{
			this.resourceManagerPool = pool;
		}

		public void Show(Window parentWindow)
		{
			//	Crée et montre la fenêtre de l'éditeur.
			if ( this.Window == null )
			{
				Window window = new Window();

				this.Window = window;

				window.Root.WindowStyles = WindowStyles.DefaultDocumentWindow;

				Point parentCenter;
				Rectangle windowBounds;

				if (parentWindow == null)
				{
					Rectangle area = ScreenInfo.GlobalArea;
					parentCenter = area.Center;
				}
				else
				{
					parentCenter = parentWindow.WindowBounds.Center;
				}

				windowBounds = new Rectangle (parentCenter.X-1000/2, parentCenter.Y-700/2, 1000, 700);
				windowBounds = ScreenInfo.FitIntoWorkingArea (windowBounds);

				window.WindowBounds = windowBounds;
				window.Root.MinSize = new Size(500, 400);
				window.Text = "Dolphin";
				window.Name = "Application";  // utilisé pour générer "QuitApplication" !
				window.PreventAutoClose = true;
				
				DolphinApplication.SetInstance(window, this);  // attache l'instance de DolphinApplication à la fenêtre

				this.CreateLayout();
			}

			this.Window.Show();
		}

		internal void Hide()
		{
			this.Window.Hide();
		}

		public override string ShortWindowTitle
		{
			get
			{
				return "Dolphin";
			}
		}

		public new ResourceManagerPool ResourceManagerPool
		{
			get
			{
				return this.resourceManagerPool;
			}
		}
		
		public CommandState GetCommandState(string command)
		{
			CommandContext context = this.CommandContext;
			CommandState state = context.GetCommandState (command);

			return state;
		}

		protected void CreateLayout()
		{
		}


		#region Instance
		public static DolphinApplication GetInstance(DependencyObject obj)
		{
			return (DolphinApplication) obj.GetValue(DolphinApplication.InstanceProperty);
		}

		public static void SetInstance(DependencyObject obj, DolphinApplication value)
		{
			obj.SetValue(DolphinApplication.InstanceProperty, value);
		}
		#endregion



		protected Common.Support.ResourceManagerPool	resourceManagerPool;

		public static readonly DependencyProperty InstanceProperty = DependencyProperty.RegisterAttached("Instance", typeof(DolphinApplication), typeof(DolphinApplication));
	}
}
