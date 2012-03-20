using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Library;

using System;


namespace Epsitec.Aider.Tools
{
	
	
	internal sealed class BusinessContextManager
	{


		public BusinessContextManager(CoreData coreData)
		{
			this.coreData = coreData;
		}


		public void Execute(Action<BusinessContext> action)
		{
			Func<BusinessContext, int> function = (b) =>
			{
				action (b);

				return 0;
			};

			this.Execute (function);
		}


		public T Execute<T>(Func<BusinessContext, T> function)
		{
			using (BusinessContext businessContext = new BusinessContext(this.coreData))
			{
				try
				{
					return function (businessContext);
				}
				finally
				{
					// NOTE Here we need to call this, because there is somewhere something that
					// registers a callback with a reference to the business context that we
					// have disposed. If we don't execute that callback, the pointer stays there
					// and the garbage collector can't reclaim the memory and we have a memory
					// leak. I think that this is a hack because that callback is related to user
					// interface stuff and we should be able to get a business context without being
					// related to a user interface.
				
					Dispatcher.ExecutePending ();
				}
			}
		}


		private readonly CoreData coreData;


	}


}
