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
			if (entity is Entities.NaturalPersonEntity)
			{
				return new NaturalPersonViewController (name);
			}

			if (entity is Entities.LegalPersonEntity)
			{
				return new LegalPersonViewController (name);
			}

			// todo...

			return null;
		}


		/// <summary>
		/// Crée un tuile simple empilée de haut en bas.
		/// </summary>
		/// <param name="container">The container.</param>
		/// <param name="entity">The entity.</param>
		/// <param name="iconUri">The icon URI.</param>
		/// <param name="title">The title.</param>
		/// <param name="content">The content.</param>
		protected void CreateSimpleTile(AbstractEntity entity, string iconUri, string title, string content)
		{
			System.Diagnostics.Debug.Assert (this.container != null);

			var tile = new Widgets.SimpleTile
			{
				Parent = this.container,
				Dock = DockStyle.Top,
				Margins = new Margins (0, 0, 0, -1),  // léger chevauchement vertical
				ArrowLocation = Direction.Right,
				Entity = entity,
				TopLeftIconUri = iconUri,
				Title = title,
				Content = content,
			};

			tile.PreferredHeight = tile.ContentHeight;
			tile.Clicked += new EventHandler<MessageEventArgs> (this.HandleTileClicked);
		}

		/// <summary>
		/// Ajuste la dernière tuile de l'empilement (celle qui est tout en bas) pour mettre à zéro sa marge inférieure.
		/// </summary>
		/// <param name="container">The container.</param>
		protected void AdjustLastTile()
		{
			System.Diagnostics.Debug.Assert (this.container != null);

			if (this.container.Children.Count != 0)
			{
				var tile = this.container.Children[this.container.Children.Count-1] as Widgets.AbstractTile;
				if (tile != null)
				{
					tile.Margins = new Margins (0);  // la dernière va jusqu'en bas normalement
				}
			}
		}


		private void DeselectAllTiles()
		{
			System.Diagnostics.Debug.Assert (this.container != null);

			foreach (Widget widget in this.container.Children)
			{
				var tile = widget as Widgets.AbstractTile;

				if (tile != null)
				{
					tile.SetSelected (false);
				}
			}
		}


		private void HandleTileClicked(object sender, MessageEventArgs e)
		{
			//	Appelé lorsque une tuile quelconque est cliquée.
			var tile = sender as Widgets.AbstractTile;
			CoreViewController controller = EntityViewController.CreateViewController ("ViewController", this.Entity, ViewControllerMode.Compact, this.Orchestrator);
			
			if (controller != null)
			{
				this.DeselectAllTiles ();
				tile.SetSelected (true);
				this.Orchestrator.ShowSubView (this, controller);
			}
		}


		protected Widget container;
	}
}
