using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;

using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Controllers;

using Epsitec.Cresus.DataLayer.Context;

using System;

using System.Collections.Generic;


namespace Epsitec.Cresus.WebCore.Server
{


	internal static class Tools
	{


		public static ViewControllerMode ParseViewMode(string value)
		{
			object viewMode = Enum.Parse (typeof (ViewControllerMode), value, true);

			return (ViewControllerMode) viewMode;
		}


		public static string ViewModeToString(ViewControllerMode mode)
		{
			return InvariantConverter.ToString (mode);
		}


		public static int? ParseViewId(string value)
		{
			int viewId;

			if (value != "null" && int.TryParse (value, out viewId))
			{
				return viewId;
			}
			else
			{
				return null;
			}
		}


		public static string ViewIdToString(int? id)
		{
			if (id.HasValue)
			{
				return InvariantConverter.ToString (id.Value);
			}
			else
			{
				return "null";
			}
		}


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


		public static string GetEntityId(BusinessContext businessContext, AbstractEntity entity)
		{
			string entityId = null;

			if (entity != null)
			{
				var entityKey = businessContext.DataContext.GetNormalizedEntityKey (entity);

				if (entityKey.HasValue)
				{
					entityId = entityKey.Value.ToString ();
				}
			}

			return entityId;
		}


		public static AbstractEntity ResolveEntity(BusinessContext businessContext, string entityId)
		{
			var entityKey = EntityKey.Parse (entityId);

			return businessContext.DataContext.ResolveEntity (entityKey);
		}
	}


}
