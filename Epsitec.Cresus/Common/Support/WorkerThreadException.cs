//	Copyright © 2012-2014, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Common.Support
{
	public sealed class WorkerThreadException : System.Exception
	{
		public WorkerThreadException(string message, System.Exception innerException)
			: base (message, innerException)
		{
		}
	}
}
