//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.Extensions;
using Epsitec.Common.Types;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Library
{
	public class NotificationManager : CoreAppComponent
	{
		public NotificationManager(CoreApp app)
			: base (app)
		{
            
		}


		public void Notify(string userName, NotificationMessage message, bool onConnect)
		{
			if (NotificationManager.hub != null)
			{
				NotificationManager.hub.Notify (userName, message,onConnect);
			}
		}

		public void WarnUser(string userName, NotificationMessage message,bool onConnect)
		{
			if (NotificationManager.hub != null)
			{
				NotificationManager.hub.WarnUser (userName, message,onConnect);
			}
		}

		public void NotifyAll(NotificationMessage message,bool onConnect)
		{
			if (NotificationManager.hub != null)
			{
				NotificationManager.hub.NotifyAll (message, onConnect);
			}
		}

		public static NotificationManager GetCurrentNotificationManager()
		{
			return CoreApp.FindCurrentAppSessionComponent<NotificationManager> ();
		}

		public static void RegisterHub(INotificationHub hub)
		{
			NotificationManager.hub = hub;
		}

		#region Factory Class

		private sealed class Factory : Epsitec.Cresus.Core.Factories.DefaultCoreAppComponentFactory<NotificationManager>
		{
			public override bool ShouldCreate(CoreApp host)
			{
				return CoreContext.HasExperimentalFeature ("Notifications");
			}
		}

		#endregion

		private static INotificationHub			hub;
	}
}

