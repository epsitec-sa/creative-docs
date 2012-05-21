//	Copyright © 2010-2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;
using Epsitec.Common.Types.Converters;
using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Support.Extensions;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.BrowserControllers;
using Epsitec.Cresus.Core.Library;
using Epsitec.Cresus.Core.Orchestrators;
using Epsitec.Cresus.Core.Orchestrators.Navigation;

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
		public NavigationOrchestrator(DataViewOrchestrator orchestrator)
		{
			this.liveNodes = new List<Node> ();
			this.orchestrator = orchestrator;
			this.history = new NavigationHistory (this, this.orchestrator.CommandContext);
			this.clickSimulators = new KeyedClickSimulators ();
			
			this.MakeDirty ();
		}


		public BrowserViewController			BrowserViewController
		{
			get
			{
				return this.MainViewController.BrowserViewController;
			}
		}

		public NavigationHistory				History
		{
			get
			{
				return this.history;
			}
		}

		private MainViewController				MainViewController
		{
			get
			{
				return this.orchestrator.MainViewController;
			}
		}

		private DataViewController				DataViewController
		{
			get
			{
				return this.MainViewController.DataViewController;
			}
		}


		/// <summary>
		/// Navigates to the tiles defined by the entities, starting from the currently
		/// selected entity.
		/// </summary>
		/// <param name="entities">The entities (excluding the one selected in the browser).</param>
		public void NavigateToTiles(params AbstractEntity[] entities)
		{
			var activePath = this.GetLeafNavigationPath ();
			var activeRoot = activePath.Root;
			var newPath    = NavigationPath.CreateTileNavigationPath (activeRoot, entities);

			newPath.Navigate (this);
		}

		public void ToggleView(int view)
		{
			this.DataViewController.ToggleSummaryViewSubview (view);
		}

		public void CloseLeafSubview()
		{
			if (this.DataViewController.ViewControllerCount > 1)
			{
				var leaf = this.orchestrator.GetLeafViewController ();
				this.orchestrator.CloseView (leaf);
			}
		}

		/// <summary>
		/// Adds a node in the navigation history for the controller which was just
		/// opened.
		/// </summary>
		/// <param name="parentController">The parent controller.</param>
		/// <param name="controller">The controller which was just opened.</param>
		public void Add(INavigationPathElementProvider parentController, INavigationPathElementProvider controller)
		{
			System.Diagnostics.Debug.Assert (parentController != controller);
			System.Diagnostics.Debug.Assert (controller != null);

			if (controller.NavigationPathElement == null)
			{
				return;
			}

			this.RecordStateBeforeChange ();

			Node node = new Node (parentController, controller, this.currentHistoryId);
			
			this.liveNodes.Add (node);
			this.MakeDirty ();
			
			this.OnNodeAdded ();
		}

		/// <summary>
		/// Removes the node in the navigation history for the controller which was
		/// just closed.
		/// </summary>
		/// <param name="parentController">The parent controller.</param>
		/// <param name="controller">The controller which was just closed.</param>
		public void Remove(INavigationPathElementProvider parentController, INavigationPathElementProvider controller)
		{
			System.Diagnostics.Debug.Assert (parentController != controller);
			System.Diagnostics.Debug.Assert (controller != null);

			if (controller.NavigationPathElement == null)
			{
				return;
			}
			
			this.RecordStateBeforeChange ();
			
			this.liveNodes.RemoveAll (node => node.Item == controller);
			this.MakeDirty ();
			
			this.OnNodeRemoved ();
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

		/// <summary>
		/// Gets the top navigation path (the last one which was added to the navigator, ie the
		/// active one).
		/// </summary>
		/// <returns>The navigation path.</returns>
		public NavigationPath GetTopNavigationPath()
		{
			var topController = this.GetTopController ();

			if (topController == null)
			{
				return null;
			}
			else
			{
				return this.GetNavigationPath (topController);
			}
		}

		/// <summary>
		/// Gets the navigation path of the leaf controller (the one on the right, which has
		/// itself no children).
		/// </summary>
		/// <returns>The navigation path.</returns>
		public NavigationPath GetLeafNavigationPath()
		{
			INavigationPathElementProvider leafViewController = this.DataViewController.GetLeafViewController ();

			if ((leafViewController == null) &&
				(this.liveNodes.Count > 0))
			{
				//	There is no leaf view controller; maybe this is because we don't rely on the
				//	data view controller to manage the active controllers? If so, try to extract
				//	the navigation path of the active 'controller' by walking the live nodes:

				this.Refresh ();
				this.SortLiveNodes ();

				leafViewController = this.liveNodes[0].Item;
			}

			return this.GetNavigationPath (leafViewController);
		}

		/// <summary>
		/// Gets a meta click simulator which contains all registered click simulators for the
		/// current controller level.
		/// </summary>
		/// <returns>The click simulator.</returns>
		public IClickSimulator GetLeafClickSimulator()
		{
			//	Make sure the UI is in a stable state before returning a click
			//	simulator, or else we might work with an outdated UI:
			Dispatcher.ExecutePending ();

			var key = this.GetLeafViewControllerKey ();
			
			ClickSimulatorCollection collection;

			if (this.clickSimulators.TryGetValue (key, out collection))
			{
				return collection;
			}
			else
			{
				return ClickSimulatorCollection.Empty;
			}
		}


		/// <summary>
		/// Executes the specified action without changing the navigation view. If the action
		/// switches to another view (i.e. closes some panels, moves to another active item
		/// in the browser, etc.), these changes will be rolled back automatically.
		/// </summary>
		/// <param name="action">The action.</param>
		public void PreserveNavigation(System.Action action)
		{
			var history            = this.History;
			var dataViewController = this.orchestrator.DataViewController;
			var navigationPath     = this.GetLeafNavigationPath ();

			using (history.SuspendRecording ())
			{
				var focus = dataViewController.SaveFocus ();
				action ();
				history.NavigateInPlace (navigationPath);
				dataViewController.RestoreFocus (focus);
			}
		}


		/// <summary>
		/// Notifies the navigation orchestrator that we are about to navigate in the
		/// history. This should record the current state immediately, if needed.
		/// </summary>
		internal void NotifyAboutToNavigateHistory()
		{
			this.RecordStateBeforeChange ();
		}


		/// <summary>
		/// Registers the specified click simulator for the current controller level.
		/// </summary>
		/// <param name="clickSimulator">The click simulator.</param>
		internal void Register(IClickSimulator clickSimulator)
		{
			var key = this.GetLeafViewControllerKey ();

			ClickSimulatorCollection collection;
			
			if (this.clickSimulators.TryGetValue (key, out collection) == false)
			{
				collection = new ClickSimulatorCollection ();
				this.clickSimulators[key] = collection;
			}

			collection.Add (clickSimulator);
		}

		/// <summary>
		/// Unregisters the specified click simulator. This will browse through all registered
		/// controller levels.
		/// </summary>
		/// <param name="clickSimulator">The click simulator.</param>
		internal void Unregister(IClickSimulator clickSimulator)
		{
			this.clickSimulators.Values.ForEach (x => x.Remove (clickSimulator));
		}


		private Key GetLeafViewControllerKey()
		{
			var leafViewController = this.DataViewController.GetLeafViewController ();

			//	TODO: what to do when there is no leafViewController ?
			
			int level = leafViewController == null ? -1 : leafViewController.GetNavigationLevel ();

			return new Key (level);
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

				this.RecordTopNode ();
			}
		}

		private Node GetTopNode()
		{
			var sortedNodes = this.liveNodes.OrderByDescending (x => x.Id);
			var topNode     = sortedNodes.FirstOrDefault ();
			
			return topNode;
		}

		private INavigationPathElementProvider GetTopController()
		{
			var topNode = this.GetTopNode ();

			if (topNode == null)
			{
				return null;
			}
			else
			{
				return topNode.Item;
			}
		}

		private void RecordTopNode()
		{
			this.history.Record (this.GetTopNavigationPath ());
		}

		private NavigationPath GetNavigationPath(INavigationPathElementProvider topController)
		{
			var fullPath = new NavigationPath ();

			fullPath.AddRange (this.WalkToRoot (topController).Select (node => node.Item.NavigationPathElement).Reverse ());

			return fullPath;
		}

		private IEnumerable<Node> WalkToRoot(INavigationPathElementProvider controller)
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
					node.Depth = -1;
					node.Link  = this.liveNodes.Find (x => x.Item == node.Parent);
				}

				this.isDirty = false;
			}
		}

		private void SortLiveNodes()
		{
			System.Diagnostics.Debug.Assert (this.isDirty == false);

			//	Update in turn the depth of every node; then sort them so that the deepest one
			//	will be the first in the list...

			this.liveNodes.ForEach (node => node.UpdateDepth ());
			this.liveNodes.Sort ((a, b) => b.Depth - a.Depth);
		}

		#region Node Class

		private class Node
		{
			public Node(INavigationPathElementProvider parent, INavigationPathElementProvider item, long id)
			{
				this.parent = parent;
				this.item   = item;
				this.id     = id;
			}

			public int								Depth
			{
				get;
				set;
			}
			
			public Node								Link
			{
				get;
				set;
			}

			public INavigationPathElementProvider	Parent
			{
				get
				{
					return this.parent;
				}
			}

			public INavigationPathElementProvider	Item
			{
				get
				{
					return this.item;
				}
			}

			public long								Id
			{
				get
				{
					return this.id;
				}
			}


			public void UpdateDepth()
			{
				if (this.Depth == -1)
				{
					if (this.Link == null)
					{
						this.Depth = 0;
					}
					else
					{
						this.Link.UpdateDepth ();
						this.Depth = this.Link.Depth;
					}
				}
			}


			private readonly INavigationPathElementProvider	parent;
			private readonly INavigationPathElementProvider	item;
			private readonly long							id;
		}

		#endregion

		#region ClickSimulatorCollection Class

		/// <summary>
		/// The <c>ClickSimulatorCollection</c> implements a meta click simulator which dispatches the
		/// <c>SimulateClick</c> action to any of the simulators found in the collection.
		/// </summary>
		private class ClickSimulatorCollection : IClickSimulator
		{
			public ClickSimulatorCollection()
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

			public static readonly IClickSimulator Empty = new ClickSimulatorCollection ();

			private readonly HashSet<IClickSimulator> simulators;
		}

		#endregion

		#region Key Structure

		private struct Key : System.IEquatable<Key>
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

		#endregion

		#region KeyedClickSimulators Class

		private class KeyedClickSimulators : Dictionary<Key, ClickSimulatorCollection>
		{
		}
		
		#endregion

		
		private void OnNodeAdded()
		{
			this.NodeAdded.Raise (this);
		}

		private void OnNodeRemoved()
		{
			this.NodeRemoved.Raise (this);
		}
		
		private static long GetCurrentActionId()
		{
			return Epsitec.Common.Widgets.Message.CurrentUserMessageId;
		}


		public event EventHandler				NodeAdded;
		public event EventHandler				NodeRemoved;


		private readonly List<Node>				liveNodes;
		private readonly KeyedClickSimulators	clickSimulators;
		private readonly DataViewOrchestrator	orchestrator;
		private readonly NavigationHistory		history;
		
		private bool							isDirty;
		private long							currentActionId;
		private long							currentHistoryId;
		private long							recordedHistoryId;
	}
}