//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

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
