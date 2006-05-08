//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Dialogs
{
	/// <summary>
	/// La classe AbstractMessageDialog sert de base aux dialogues présentant
	/// un message avec des boutons de style oui/non.
	/// </summary>
	public abstract class AbstractMessageDialog : AbstractDialog
	{
		public AbstractMessageDialog()
		{
		}
		
		
		
		public override Widgets.Window			Window
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
		
		public override Widgets.Window			Owner
		{
			get
			{
				return base.Owner;
			}
			set
			{
				if (this.window == null)
				{
					this.CreateWindow ();
				}
				
				base.Owner = value;
			}
		}
		
		
		internal void HideCancelButton()
		{
			this.hide_cancel = true;
		}
		
		
		public static void LayoutButtons(double width, params Widgets.Button[] buttons)
		{
			if (buttons.Length > 0)
			{
				double total_width = 0;
				
				for (int i = 0; i < buttons.Length; i++)
				{
					if (buttons[i] != null)
					{
						total_width += buttons[i].ActualWidth;
					}
				}
				
				total_width += (buttons.Length-1) * 8;
				
				if (total_width < width)
				{
					double x = System.Math.Floor ((width - total_width) / 2);
					
					for (int i = 0; i < buttons.Length; i++)
					{
						if (buttons[i] != null)
						{
							buttons[i].SetManualBounds(new Drawing.Rectangle(x, buttons[i].ActualLocation.Y, buttons[i].ActualWidth, buttons[i].ActualHeight));
							
							x += buttons[i].ActualWidth;
							x += 8;
						}
					}
				}
			}
		}
		
		
		protected abstract void CreateWindow();
		
		protected bool							hide_cancel;
	}
}
