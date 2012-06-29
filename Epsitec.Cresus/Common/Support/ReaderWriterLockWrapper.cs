using System;
using System.Threading;


namespace Epsitec.Common.Support
{


	public sealed class ReaderWriterLockWrapper : IDisposable
	{


		public ReaderWriterLockWrapper()
		{
			this.rwLock = new ReaderWriterLockSlim ();
		}


		public IDisposable LockRead()
		{
			return TimedReaderWriterLock.LockRead (this.rwLock);
		}


		public IDisposable LockWrite()
		{
			return TimedReaderWriterLock.LockWrite (this.rwLock);
		}


		#region IDisposable Members


		public void Dispose()
		{
			this.rwLock.Dispose ();
		}


		#endregion
	

		private readonly ReaderWriterLockSlim rwLock;


	}


}
