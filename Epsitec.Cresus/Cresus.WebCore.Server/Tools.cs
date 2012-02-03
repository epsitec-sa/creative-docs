using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Controllers;

using System;


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
		

	}


}
