using System.Collections.Generic;
using System.ServiceModel;

namespace Epsitec.Designer.Protocol
{
	[ServiceBehavior]
	public class NavigatorService : INavigator
	{
		public static void DefineNavigateToStringAction(System.Action<string> action)
		{
			NavigatorService.navigateToStringAction = action;
		}

		public static void DefineNavigateToCaptionAction(System.Action<string> action)
		{
			NavigatorService.navigateToCaptionAction = action;
		}

		public static void DefineNavigateToEntityFieldAction(System.Action<string> action)
		{
			NavigatorService.navigateToEntityFieldAction = action;
		}

		#region INavigator Members

		[OperationBehavior]
		public void NavigateToString(string id)
		{
			if (NavigatorService.navigateToStringAction != null)
			{
				NavigatorService.navigateToStringAction (id);
			}
		}

		[OperationBehavior]
		public void NavigateToCaption(string id)
		{
			if (NavigatorService.navigateToCaptionAction != null)
			{
				NavigatorService.navigateToCaptionAction (id);
			}
		}

		[OperationBehavior]
		public void NavigateToEntityField(string id)
		{
			if (NavigatorService.navigateToEntityFieldAction != null)
			{
				NavigatorService.navigateToEntityFieldAction (id);
			}
		}

		#endregion

		private static System.Action<string> navigateToStringAction;
		private static System.Action<string> navigateToCaptionAction;
		private static System.Action<string> navigateToEntityFieldAction;
	}
}
