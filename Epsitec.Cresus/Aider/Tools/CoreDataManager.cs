using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Data;
using Epsitec.Cresus.Core.Library;

using System;

using System.Linq;


namespace Epsitec.Aider.Tools
{
	
	
	internal sealed class CoreDataManager
	{


		public CoreDataManager(CoreData coreData)
		{
			this.CoreData = coreData;
		}


		public CoreData CoreData
		{
			get;
			private set;
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
			using (var businessContext = this.Create ())
			{
				try
				{
					return function (businessContext);
				}
				finally
				{
					this.Cleanup (businessContext);
				}
			}
		}


		public BusinessContext Create()
		{
			return new BusinessContext (this.CoreData);
		}


		public void Cleanup(BusinessContext businessContext)
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
