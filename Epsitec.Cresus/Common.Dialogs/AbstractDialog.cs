//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using Epsitec.Common.Widgets;
using Epsitec.Common.Support;

namespace Epsitec.Common.Dialogs
{
	/// <summary>
	/// Summary description for AbstractClass.
	/// </summary>
	public abstract class AbstractDialog : IDialog
	{
		public AbstractDialog()
		{
		}
		
		
		public void Close()
		{
			if (this.window != null)
			{
				this.window.Hide ();
				this.window.CommandDispatcher.Dispose ();
				this.window.CommandDispatcher = null;
				this.window.AsyncDispose ();
			}
		}
		
		
		public Window							Window
		{
			get
			{
				if (this.window == null)
				{
					this.CreateWindow ();
				}
				
				return this.window;
			}
		}
		
		
		#region IDialog Members
		public void Show()
		{
			this.Window.Show ();
		}
		
		public Window							Owner
		{
			get
			{
				if (this.window != null)
				{
					return this.window.Owner;
				}
				
				return null;
			}
			
			set
			{
				this.Window.Owner = value;
			}
		}
		#endregion
		
		public static void LayoutButtons(double width, params Widgets.Button[] buttons)
		{
			if (buttons.Length > 0)
			{
				double total_width = 0;
				
				for (int i = 0; i < buttons.Length; i++)
				{
					total_width += buttons[i].Width;
				}
				
				total_width += (buttons.Length-1) * 8;
				
				if (total_width < width)
				{
					double x = System.Math.Floor ((width - total_width) / 2);
					
					for (int i = 0; i < buttons.Length; i++)
					{
						buttons[i].Location = new Drawing.Point (x, buttons[i].Bottom);
						
						x += buttons[i].Width;
						x += 8;
					}
				}
			}
		}
		
		
		protected abstract void CreateWindow();
		
		protected Window						window;
	}
}
