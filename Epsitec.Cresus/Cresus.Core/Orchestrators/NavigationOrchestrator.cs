//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;
using Epsitec.Common.Types.Converters;
using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Support.Extensions;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.BrowserControllers;
using Epsitec.Cresus.Core.Orchestrators;

using Epsitec.Cresus.DataLayer;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Orchestrators
{
	/// <summary>
	/// The <c>NavigationOrchestrator</c> class manages basic navigation tasks between
	/// view controllers. It is tightly related to <see cref="DataViewController"/>.
	/// </summary>
	public class NavigationOrchestrator
	{
		public NavigationOrchestrator(MainViewController mainViewController)
		{
			this.liveNodes = new List<Node> ();
			this.mainViewController = mainViewController;
			this.history = new Navigation.NavigationHistory (this);
			this.clickSimulators = new Dictionary<Key, ClickSimulator> ();
			
			this.MakeDirty ();
		}


		public MainViewController MainViewController
		{
			get
			{
				return this.mainViewController;
			}
		}

		public BrowserViewController BrowserViewController
		{
			get
			{
				return this.mainViewController.BrowserViewController;
			}
		}

		public DataViewController DataViewController
		{
			get
			{
				return this.mainViewController.DataViewController;
			}
		}

		public DataViewOrchestrator Orchestrator
		{
			get
			{
				return this.mainViewController.Orchestrator;
			}
		}

		public Navigation.NavigationHistory History
		{
			get
			{
				return this.history;
			}
		}


		/// <summary>
		/// Adds a node in the navigation history for the controller which was just
		/// opened.
		/// </summary>
		/// <param name="parentController">The parent controller.</param>
		/// <param name="controller">The controller which was just opened.</param>
		public void Add(CoreViewController parentController, CoreViewController controller)
		{
			System.Diagnostics.Debug.Assert (parentController != controller);
			System.Diagnostics.Debug.Assert (controller != null);

			this.RecordStateBeforeChange ();

			this.liveNodes.Add (new Node (parentController, controller, this.currentHistoryId));
			this.MakeDirty ();
		}

		/// <summary>
		/// Removes the node in the navigation history for the controller which was
		/// just closed.
		/// </summary>
		/// <param name="parentController">The parent controller.</param>
		/// <param name="controller">The controller which was just closed.</param>
		public void Remove(CoreViewController parentController, CoreViewController controller)
		{
			System.Diagnostics.Debug.Assert (parentController != controller);
			System.Diagnostics.Debug.Assert (controller != null);
			
			this.RecordStateBeforeChange ();
			
			this.liveNodes.RemoveAll (node => node.Item == controller);
			this.MakeDirty ();
		}

		/// <summary>
		/// Gets the level of the controller.
		/// </summary>
		/// <param name="controller">The controller.</param>
		/// <returns>The level or <c>-1</c> if the controller cannot be found.</returns>
		public int GetLevel(CoreViewController controller)
		{
			return this.WalkToRoot (controller).Count () - 1;
		}

		public IClickSimulator GetLeafClickSimulator()
		{
			var key = this.GetLeafViewControllerKey ();
			
			ClickSimulator clickSimulator;

			this.clickSimulators.TryGetValue (key, out clickSimulator);

			return clickSimulator;
		}

		private Key GetLeafViewControllerKey()
		{
			var dataViewController = this.DataViewController;
			var leafViewController = dataViewController.GetLeafViewController ();
			
			int level = leafViewController.GetNavigationLevel ();
			
			return new Key (level);
		}

		/// <summary>
		/// Notifies the navigation orchestrator that we are about to navigate in the
		/// history. This should record the current state immediately, if needed.
		/// </summary>
		internal void NotifyAboutToNavigateHistory()
		{
			this.RecordStateBeforeChange ();
		}


		internal void Register(IClickSimulator clickSimulator)
		{
			var key = this.GetLeafViewControllerKey ();

			ClickSimulator simulator;
			
			if (this.clickSimulators.TryGetValue (key, out simulator) == false)
			{
				simulator = new ClickSimulator ();
				this.clickSimulators[key] = simulator;
			}

			simulator.Add (clickSimulator);
		}

		internal void Unregister(IClickSimulator clickSimulator)
		{
			this.clickSimulators.Values.ForEach (x => x.Remove (clickSimulator));
		}


		class ClickSimulator : IClickSimulator
		{
			public ClickSimulator()
			{
				this.simulators = new HashSet<IClickSimulator> ();
			}

			public void Add(IClickSimulator simulator)
			{
				this.simulators.Add (simulator);
			}

			public void Remove(IClickSimulator simulator)
			{
				if (this.simulators.Remove (simulator))
				{
					//	OK, removed successfully the specified simulator from this instance.
				}
			}

			#region IClickSimulator Members

			public bool SimulateClick(string name)
			{
				return this.simulators.Any (x => x.SimulateClick (name));
			}

			#endregion

			private readonly HashSet<IClickSimulator> simulators;
		}
		
		struct Key : System.IEquatable<Key>
		{
			public Key(int level)
			{
				this.level = level;
			}


			public int Level
			{
				get
				{
					return this.level;
				}
			}


			public override int GetHashCode()
			{
				return this.level;
			}

			public override bool Equals(object obj)
			{
				if (obj is Key)
				{
					return this.Equals ((Key) obj);
				}
				else
				{
					return false;
				}
			}

			#region IEquatable<Key> Members

			public bool Equals(Key other)
			{
				return this.level == other.level;
			}

			#endregion

			private readonly int level;
		}


		private void RecordStateBeforeChange()
		{
			var actionId = NavigationOrchestrator.GetCurrentActionId ();

			if (this.currentActionId != actionId)
			{
				this.currentActionId  = actionId;
				this.RecordCurrentStateAfterNewUserAction ();
			}
		}

		private void RecordCurrentStateAfterNewUserAction()
		{
			if (this.recordedHistoryId != this.currentHistoryId)
			{
				this.recordedHistoryId = this.currentHistoryId;

				var sortedNodes = this.liveNodes.OrderByDescending (x => x.Id);
				var topNode     = sortedNodes.FirstOrDefault ();

				this.RecordTopNode (topNode);
			}
		}

		private void RecordTopNode(Node topNode)
		{
			if (topNode != null)
			{
				var topController = topNode.Item;
				var fullPath = new Navigation.NavigationPath ();

				fullPath.AddRange (this.WalkToRoot (topController).Select (node => node.Item.NavigationPathElement).Reverse ());

				this.history.Record (fullPath);
			}
		}

		private IEnumerable<Node> WalkToRoot(CoreViewController controller)
		{
			this.Refresh ();

			Node node = this.liveNodes.Find (x => x.Item == controller);

			while (node != null)
			{
				yield return node;
				node = node.Link;
			}
		}

		private void MakeDirty()
		{
			this.currentHistoryId++;
			this.isDirty = true;
		}

		private void Refresh()
		{
			if (this.isDirty)
			{
				foreach (var node in this.liveNodes)
				{
					node.Link = this.liveNodes.Find (x => x.Item == node.Parent);
				}

				this.isDirty = false;
			}
		}

		#region Node Class

		private class Node
		{
			public Node(CoreViewController parent, CoreViewController item, long id)
			{
				this.parent = parent;
				this.item   = item;
				this.id     = id;
			}

			
			public Node Link
			{
				get;
				set;
			}
			
			public CoreViewController Parent
			{
				get
				{
					return this.parent;
				}
			}

			public CoreViewController Item
			{
				get
				{
					return this.item;
				}
			}

			public long Id
			{
				get
				{
					return this.id;
				}
			}


			private readonly CoreViewController		parent;
			private readonly CoreViewController		item;
			private readonly long					id;
		}

		#endregion

		private static long GetCurrentActionId()
		{
			var message = Epsitec.Common.Widgets.Message.GetLastMessage ();
			return message == null ? 0 : message.MessageId;
		}


		private readonly List<Node> liveNodes;
		private readonly Dictionary<Key, ClickSimulator> clickSimulators;
		private readonly MainViewController mainViewController;
		private readonly Navigation.NavigationHistory history;
		private bool isDirty;
		private long currentActionId;
		private long currentHistoryId;
		private long recordedHistoryId;
	}

	public class NavigationFieldNode
	{

		public EntityFieldPath Path
		{
			get;
			set;
		}

		public Marshaler Marshaler
		{
			get;
			set;
		}
	}

	public class NavigationViewNode : NavigationFieldNode
	{
		public Druid EntityId
		{
			get;
			set;
		}

		public ViewControllerMode Mode
		{
			get;
			set;
		}
	}
}
