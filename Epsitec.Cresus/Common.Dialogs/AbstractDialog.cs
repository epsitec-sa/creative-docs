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
				if (this.Window.IsActive)
				{
					Window owner = this.Owner;
					
					if (owner != null)
					{
						owner.MakeActive ();
					}
				}
				
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
			Window owner = this.Owner;
			
			if (owner != null)
			{
				Drawing.Rectangle owner_bounds  = owner.WindowBounds;
				Drawing.Rectangle dialog_bounds = this.Window.WindowBounds;
				
				double ox = System.Math.Floor (owner_bounds.Left + (owner_bounds.Width - dialog_bounds.Width) / 2);
				double oy = System.Math.Floor (owner_bounds.Top  - (owner_bounds.Height - dialog_bounds.Height) / 3 - dialog_bounds.Height);
				
				dialog_bounds.Location = new Drawing.Point (ox, oy);
				
				this.Window.WindowBounds = dialog_bounds;
			}
			
			this.OnShowing ();
			this.Window.ShowDialog ();
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
		
		protected virtual void OnShowing()
		{
			if (this.Showing != null)
			{
				this.Showing (this);
			}
		}
		
		
		public event Support.EventHandler		Showing;
		
		protected Window						window;
	}
}
