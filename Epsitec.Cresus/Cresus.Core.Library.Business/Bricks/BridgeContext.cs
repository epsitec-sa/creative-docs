//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.DataAccessors;
using Epsitec.Cresus.Core.Features;
using Epsitec.Cresus.Core.Widgets;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Bricks
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
			this.features   = this.controller.Data.Host.FindComponent<FeatureManager> ();

			BridgeContext.instance = this;
		}

		public static BridgeContext				Instance
		{
			get
			{
				return BridgeContext.instance;
			}
		}

		public FeatureManager					FeatureManager
		{
			get
			{
				return this.features;
			}
		}

		public bool								IsCreatingTileDataItems
		{
			get
			{
				return this.pendingBridges != null;
			}
		}

		public bool								HasPendingBridges
		{
			get
			{
				return (this.pendingBridges != null)
					&& (this.pendingBridges.Count > 0);
			}
		}

		public Bridge<T> CreateBridge<T>(EntityViewController<T> controller)
			where T : AbstractEntity, new ()
		{
			var bridge = new Bridge<T> (this, controller);

			if (this.IsCreatingTileDataItems)
			{
				this.pendingBridges.Push (bridge);
			}
			else
			{
				this.bridges.Add (bridge);
			}

			return bridge;
		}

		public Bridge GetNextPendingBridge()
		{
			if ((this.pendingBridges == null) ||
				(this.pendingBridges.Count == 0))
			{
				return null;
			}
			else
			{
				return this.pendingBridges.Pop ();
			}
		}

		private void CreateTileDataItems()
		{
			System.Diagnostics.Debug.Assert (this.bridges.Count > 0);
			System.Diagnostics.Debug.Assert (this.bridges[0].Controller == this.controller);
			System.Diagnostics.Debug.Assert (this.IsCreatingTileDataItems == false);

			if (this.bridges.Any (x => x.ContainsBricks))
			{
				using (var data = TileContainerController.Setup (this.controller))
				{
					try
					{
						this.pendingBridges = new Stack<Bridge> ();

						//	The collection of bridges might change while we are processing the
						//	items :

						for (int i = 0; i < this.bridges.Count; i++)
						{
							var bridge = this.bridges[i];

							bridge.CreateTileDataItems (data);
							
							System.Diagnostics.Debug.Assert (this.HasPendingBridges == false);
						}
					}
					finally
					{
						this.pendingBridges = null;
					}
				}
			}

			System.Diagnostics.Debug.Assert (this.IsCreatingTileDataItems == false);
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
		private readonly FeatureManager			features;

		private Stack<Bridge>					pendingBridges;
	}
}
