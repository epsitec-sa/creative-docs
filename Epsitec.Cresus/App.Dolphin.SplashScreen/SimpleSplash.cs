//	Copyright © 2006-2008, OPaC bright ideas, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.App.Dolphin
{
	public class SimpleSplash : Opac.Shell.Interfaces.ISplashScreen, System.IDisposable
	{
		#region ISplashScreen Members

		public System.Windows.Forms.Form CreateForm()
		{
			return new SplashForm ();
		}

		public void DisplayMessage(System.Windows.Forms.Form form, string message)
		{
			SplashForm splash = form as SplashForm;

			if (splash != null)
			{
				splash.labelMessage.Text = message;
			}
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
	}
}
