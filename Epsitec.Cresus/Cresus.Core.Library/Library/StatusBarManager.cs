//	Copyright � 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Samuel LOUP, Maintainer: Samuel LOUP

using Epsitec.Common.Support;
using Epsitec.Common.Support.Extensions;
using Epsitec.Common.Types;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Library
{
	public class StatusBarManager : CoreAppComponent
	{
		public StatusBarManager(CoreApp app)
			: base (app)
		{
		}


		public void AddToBar(string type, string text, string iconClass, string statusId, When when)
		{
			if (StatusBarManager.hub != null)
			{
				StatusBarManager.hub.AddToBar (type, text, iconClass, statusId, when);
			}
		}

		public void RemoveFromBar(string statusId, When when)
		{
			if (StatusBarManager.hub != null)
			{
				StatusBarManager.hub.RemoveFromBar (statusId, when);
			}
		}

		public IEnumerable<string> GetStatusEntitiesId()
		{
			if (StatusBarManager.hub != null)
			{
				return StatusBarManager.hub.GetStatusEntitiesId ();
			}

			return Enumerable.Empty<string> ();
		}

		public static StatusBarManager GetCurrentStatusBarManager()
		{
			return CoreApp.FindCurrentAppSessionComponent<StatusBarManager> ();
		}

		public static void RegisterHub(IStatusBarHub hub)
		{
			StatusBarManager.hub = hub;
		}

		#region Factory Class

		private sealed class Factory : Epsitec.Cresus.Core.Factories.DefaultCoreAppComponentFactory<StatusBarManager>
		{
			public override bool ShouldCreate(CoreApp host)
			{
				return true;
			}
		}

		#endregion

	
		private static IStatusBarHub			hub;
	}
}