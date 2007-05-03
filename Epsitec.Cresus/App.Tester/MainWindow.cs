using System;
using System.Text;
using System.Collections.Generic;
using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Types;
using Epsitec.Common.Drawing;

namespace App.Tester
{
	/// <summary>
	/// Fenêtre principale de l'éditeur de ressources.
	/// </summary>
	public class MainWindow : DependencyObject
	{
		public MainWindow()
		{
		}
		public void Show(Window parentWindow)
		{
			//	Crée et montre la fenêtre de l'éditeur.
			if (this.window == null)
			{
				this.window = new Window ();

				this.window.Root.WindowStyles = WindowStyles.DefaultDocumentWindow;

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

				this.window.WindowBounds = windowBounds;
				this.window.Root.MinSize = new Size (500, 400);
				this.window.Text = "Tester";
				this.window.Name = "Application";  // utilisé pour générer "QuitApplication" !
				this.window.PreventAutoClose = true;

				MainWindow.SetInstance (this.window, this);  // attache l'instance de MainWindow à la fenêtre
//				this.CreateLayout ();
			}
			this.window.Show ();
		}

		public static void SetInstance(DependencyObject obj, MainWindow value)
		{
			obj.SetValue (MainWindow.InstanceProperty, value);
		}

		public Window Window
		{
			//	Retourne la fenêtre principale de l'application.
			get
			{
				return this.window;
			}
		}

		protected Window window;
		public static readonly DependencyProperty InstanceProperty = DependencyProperty.RegisterAttached ("Instance", typeof (MainWindow), typeof (MainWindow));
	}
}
