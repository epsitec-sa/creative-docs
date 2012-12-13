using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Core.Business;

using System;

using System.Collections.Generic;


namespace Epsitec.Cresus.WebCore.Server
{


	internal static class Tools
	{


		public static IDisposable Bind(this BusinessContext businessContext, params AbstractEntity[] entities)
		{
			var entitiesToDispose = new List<AbstractEntity> ();

			try
			{
				foreach (var entity in entities)
				{
					businessContext.Register (entity);

					entitiesToDispose.Add (entity);
				}
			}
			catch (Exception)
			{
				foreach (var entity in entitiesToDispose)
				{
					businessContext.Unregister (entity);
				}

				throw;
			}

			Action action = () =>
			{
				foreach (var entity in entities)
				{
					businessContext.Unregister (entity);
				}
			};

			return DisposableWrapper.CreateDisposable (action);
		}


	}


}
