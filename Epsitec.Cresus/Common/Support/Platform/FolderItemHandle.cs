//	Copyright © 2006-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Support.Platform
{
	internal abstract class FolderItemHandle : System.IDisposable, System.IEquatable<FolderItemHandle>
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

		#region IEquatable<FolderItemHandle> Members

		public bool Equals(FolderItemHandle other)
		{
			if (System.Object.ReferenceEquals (other, null))
			{
				return false;
			}
			if (System.Object.ReferenceEquals (other, this))
			{
				return true;
			}

			return this.InternalEquals (other);
		}

		#endregion

		public override bool Equals(object obj)
		{
			FolderItemHandle other = obj as FolderItemHandle;
			return this.Equals (other);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode ();
		}

		protected abstract void Dispose(bool disposing);
		protected abstract bool InternalEquals(FolderItemHandle other);
	}
}
