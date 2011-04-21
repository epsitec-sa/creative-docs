using System;


namespace Epsitec.Common.Support
{


	public sealed class EmptyDisposable : IDisposable
	{


		private EmptyDisposable()
		{
		}


		#region IDisposable Members


		public void Dispose()
		{
		}


		#endregion


		public static IDisposable Instance
		{
			get
			{
				return EmptyDisposable.instance;
			}
		}


		private static readonly IDisposable instance = new EmptyDisposable ();
		

	}


}
