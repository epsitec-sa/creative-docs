using System;
using System.Collections.Generic;
using System.Text;

namespace Epsitec.Common.Support.Platform
{
	internal abstract class FolderItemHandle : System.IDisposable
	{
		public FolderItemHandle()
		{
		}

		~FolderItemHandle()
		{
			this.Dispose (false);
		}

		#region IDisposable Members

		public void Dispose()
		{
			this.Dispose (true);
			System.GC.SuppressFinalize (this);
		}

		#endregion

		protected abstract void Dispose(bool disposing);
	}
}
