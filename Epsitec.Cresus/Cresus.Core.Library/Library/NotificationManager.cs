//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.Extensions;
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


		public static NotificationManager GetCurrentNotificationManager()
		{
			return CoreApp.FindCurrentAppSessionComponent<NotificationManager> ();
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
	}
}

