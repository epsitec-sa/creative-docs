using Epsitec.Common.Support.Extensions;

using System;

using System.Collections.Generic;
using System.Collections.ObjectModel;


namespace Epsitec.Common.Support
{


	public sealed class GroupedException : Exception
	{


		internal GroupedException(IEnumerable<Exception> exceptions)
			: base ()
		{
			this.exceptions = exceptions.AsReadOnlyCollection ();
		}


		public ReadOnlyCollection<Exception> Exceptions
		{
			get
			{
				return this.exceptions;
			}
		}


		private readonly ReadOnlyCollection<Exception> exceptions;


	}


}
