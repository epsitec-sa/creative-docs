//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using Epsitec.Common.Widgets;
using Epsitec.Common.Support;

namespace Epsitec.Common.Designer
{
	/// <summary>
	/// Summary description for DialogDesigner.
	/// </summary>
	public class DialogDesigner : Epsitec.Common.Dialogs.IDialogDesigner
	{
		public DialogDesigner()
		{
		}
		
		
		#region IDialogDesigner Members
		public Epsitec.Common.Types.IDataGraph	DialogData
		{
			get
			{
				return this.dialog_data;
			}
			set
			{
				if (this.dialog_data != value)
				{
					this.dialog_data = value;
				}
			}
		}

		public Window							DialogWindow
		{
			get
			{
				return this.dialog_window;
			}
			set
			{
				if (this.dialog_window != value)
				{
					this.dialog_window = value;
				}
			}
		}
		
		public void StartDesign()
		{
		}
		#endregion
		
		#region IDisposable Members
		public void Dispose()
		{
			this.Dispose (true);
			System.GC.SuppressFinalize (this);
		}
		#endregion
		
		
		protected virtual void Dispose(bool disposing)
		{
		}
		
		
		private Types.IDataGraph				dialog_data;
		private Window							dialog_window;
	}
}
