using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;

using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Controllers;

using System;

using System.Collections.Generic;


namespace Epsitec.Cresus.WebCore.Server
{


	internal static class Tools
	{


		public static ViewControllerMode ParseViewControllerMode(string value)
		{
			object mode = Enum.Parse (typeof (ViewControllerMode), value, true);

			return (ViewControllerMode) mode;
		}


		public static string ViewControllerModeToString(ViewControllerMode mode)
		{
			return InvariantConverter.ToString (mode);
		}


		public static int? ParseControllerSubTypeId(string value)
		{
			int controllerSubTypeId;

			if (value != "null" && int.TryParse (value, out controllerSubTypeId))
			{
				return controllerSubTypeId;
			}
			else
			{
				return null;
			}
		}


		public static string ControllerSubTypeIdToString(int? id)
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


		public static string GetLambdaFieldName(string entityKey)
		{
			return string.Concat ("lambda_", entityKey);
		}


		public static bool IsLambdaFieldName(string text)
		{
			return text.StartsWith ("lambda_");
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
				try
				{
					businessContext.ApplyRulesToRegisteredEntities (RuleType.Update);
				}
				finally
				{
					foreach (var entity in entities)
					{
						businessContext.Unregister (entity);
					}
				}
			};

			return DisposableWrapper.CreateDisposable (action);
		}


	}


}
