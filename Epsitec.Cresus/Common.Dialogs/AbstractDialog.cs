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
		
		protected abstract void CreateWindow();
		
		protected Window						window;}
}
