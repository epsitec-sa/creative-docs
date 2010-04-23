//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using System.Text;
using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;

namespace Epsitec.Cresus.Core.Controllers
{
	public abstract class EntityViewController : CoreViewController
	{
		public EntityViewController(string name)
			: base (name)
		{
		}

		public AbstractEntity Entity
		{
			get;
			set;
		}

		public ViewControllerMode Mode
		{
			get;
			set;
		}

		public Orchestrators.DataViewOrchestrator Orchestrator
		{
			get;
			set;
		}

		public static EntityViewController CreateViewController(string name, AbstractEntity entity, ViewControllerMode mode, Orchestrators.DataViewOrchestrator orchestrator)
		{
			EntityViewController controller = EntityViewController.ResolveViewController (name, entity);

			if (controller == null)
			{
				return null;
			}

			controller.Entity = entity;
			controller.Mode = mode;
			controller.Orchestrator = orchestrator;

			return controller;
		}
		
		private static EntityViewController ResolveViewController(string name, AbstractEntity entity)
		{
			if (entity is Entities.AbstractPersonEntity)
			{
				return new PersonViewController (name);
			}

			return null;
		}


		protected void CreateSimpleTile(Widget container, string iconUri, string title, string content)
		{
			Widgets.SimpleTile tile = new Widgets.SimpleTile
			{
				Parent = container,
				Dock = DockStyle.Top,
				Margins = new Margins (0, 0, 0, -1),  // léger chevauchement vertical
				ArrowLocation = Direction.Right,
				IconUri = iconUri,
				Title = title,
				Content = content,
			};

			tile.PreferredHeight = tile.ContentHeight;
		}

		protected void AdjustLastTile(Widget container)
		{
			if (container.Children.Count != 0)
			{
				var tile = container.Children[container.Children.Count-1] as Widgets.AbstractTile;
				if (tile != null)
				{
					tile.Margins = new Margins (0);
				}
			}
		}

	}
}
