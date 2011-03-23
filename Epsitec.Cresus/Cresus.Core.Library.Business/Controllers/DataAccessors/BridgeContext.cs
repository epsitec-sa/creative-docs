//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Core.Widgets;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers.DataAccessors
{
	public class BridgeContext : System.IDisposable
	{
		public BridgeContext(EntityViewController controller)
		{
			if (BridgeContext.instance != null)
			{
				throw new System.InvalidOperationException ("Cannot create recursive BridgeContext instances");
			}

			this.bridges    = new List<Bridge> ();
			this.controller = controller;
			
			BridgeContext.instance = this;
		}

		public static BridgeContext			Instance
		{
			get
			{
				return BridgeContext.instance;
			}
		}

		public Bridge<T> CreateBridge<T>(EntityViewController<T> controller)
			where T : AbstractEntity, new ()
		{
			var bridge = new Bridge<T> (controller);

			this.bridges.Add (bridge);

			return bridge;
		}

		private void CreateTileDataItems()
		{
			System.Diagnostics.Debug.Assert (this.bridges.Count > 0);
			System.Diagnostics.Debug.Assert (this.bridges[0].Controller == this.controller);

			if (this.bridges.Any (x => x.ContainsBricks))
			{
				using (var data = TileContainerController.Setup (this.controller))
				{
					foreach (var bridge in this.bridges)
					{
						bridge.Controller.NotifyAboutToCreateUI ();
						bridge.CreateTileDataItems (data);
					}
				}
			}
		}

		#region IDisposable Members

		void System.IDisposable.Dispose()
		{
			this.controller.CreateBridgeAndBuildBricks ();

			if (this.bridges.Count > 0)
			{
				this.CreateTileDataItems ();
			}

			BridgeContext.instance = null;
		}

		#endregion


		[System.ThreadStatic]
		public static BridgeContext				instance;

		private readonly List<Bridge>			bridges;
		private readonly EntityViewController	controller;
	}
}
