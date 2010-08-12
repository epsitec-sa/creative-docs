//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;
using Epsitec.Common.Types.Converters;
using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Controllers;
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
		public NavigationOrchestrator()
		{
			this.nodes = new List<Node> ();
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

			this.RecordCurrentState ();

			this.nodes.Add (new Node ()
			{
				Parent = parentController,
				Item = controller,
				Id = this.currentActionId,
			});

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
			
			this.RecordCurrentState ();
			
			this.nodes.RemoveAll (node => node.Item == controller);
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


		private void RecordCurrentState()
		{
			var actionId = NavigationOrchestrator.GetCurrentActionId ();

			if (this.currentActionId != actionId)
			{
				this.currentActionId  = actionId;
				this.RecordTopNode ();
			}
		}

		private void RecordTopNode()
		{
			if (this.recordedHistoryId != this.currentHistoryId)
			{
				this.recordedHistoryId = this.currentHistoryId;

				var sorted = from node in this.nodes
							 orderby node.Id descending
							 select node;

				this.RecordTopNode (sorted.FirstOrDefault ());
			}
		}

		private void RecordTopNode(Node node)
		{
			if (node == null)
			{
				System.Diagnostics.Debug.WriteLine ("History: no nodes");
			}
			else
			{
				System.Diagnostics.Debug.WriteLine ("History: node = " + node.Item.GetType ().Name);
				foreach (var item in this.WalkToRoot (node.Item))
				{
					System.Diagnostics.Debug.Write (" <-- " + (item.Item.GetRelativeNavigationPath () ?? "<null>"));
				}

				System.Diagnostics.Debug.WriteLine ("");
			}
		}

		private IEnumerable<Node> WalkToRoot(CoreViewController controller)
		{
			this.Refresh ();

			Node node = this.nodes.Find (x => x.Item == controller);

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
				foreach (var node in this.nodes)
				{
					node.Link = this.nodes.Find (x => x.Item == node.Parent);
				}

				this.isDirty = false;
			}
		}


		class Node
		{
			public Node Link;
			public CoreViewController Parent;
			public CoreViewController Item;
			public long Id;
		}

		private static long GetCurrentActionId()
		{
			var message = Epsitec.Common.Widgets.Message.GetLastMessage ();
			return message == null ? 0 : message.MessageId;
		}


		private readonly List<Node> nodes;
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
