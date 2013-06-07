using System;

namespace Epsitec.Common.Support
{
	public sealed class WorkerThreadException : Exception
	{
		public WorkerThreadException(string message, Exception innerException)
			: base (message, innerException)
		{
		}
	}
}
