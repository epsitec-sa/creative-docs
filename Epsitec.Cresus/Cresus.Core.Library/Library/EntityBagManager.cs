//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Samuel LOUP, Maintainer: Samuel LOUP

using Epsitec.Common.Support;
using Epsitec.Common.Support.Extensions;
using Epsitec.Common.Types;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Library
{
	public class EntityBagManager : CoreAppComponent
	{
		public EntityBagManager(CoreApp app)
			: base (app)
		{
		}


		public void AddToBag(string userName, string title, string summary, string entityId,NotificationTime when)
		{
			if (EntityBagManager.hub != null)
			{
				EntityBagManager.hub.AddToBag (userName, title, summary, entityId, when);
			}
		}

		public void RemoveFromBag(string userName, string title, string summary, string entityId,NotificationTime when)
		{
			if (EntityBagManager.hub != null)
			{
				EntityBagManager.hub.RemoveFromBag (userName, title, summary, entityId, when);
			}
		}

		public static EntityBagManager GetCurrentEntityBagManager()
		{
			return CoreApp.FindCurrentAppSessionComponent<EntityBagManager> ();
		}

		public static void RegisterHub(IEntityBagHub hub)
		{
			EntityBagManager.hub = hub;
		}

		#region Factory Class

		private sealed class Factory : Epsitec.Cresus.Core.Factories.DefaultCoreAppComponentFactory<EntityBagManager>
		{
			public override bool ShouldCreate(CoreApp host)
			{
				return true;
			}
		}

		#endregion

		private static IEntityBagHub			hub;
	}
}

