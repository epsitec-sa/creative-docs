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
	public class NavigationOrchestrator
	{
		public NavigationOrchestrator()
		{
			this.nodes = new List<Node> ();
		}

		public void Add(CoreViewController parentController, CoreViewController controller)
		{
			this.nodes.Add (new Node ()
			{
				Parent = parentController,
				Item = controller
			});

			this.MakeDirty ();
		}

		public void Remove(CoreViewController parentController, CoreViewController controller)
		{
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
		}


		private readonly List<Node> nodes;
		private bool isDirty;
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
