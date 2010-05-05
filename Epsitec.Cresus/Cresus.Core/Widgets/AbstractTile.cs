//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Core.Controllers;

namespace Epsitec.Cresus.Core.Widgets
{
	/// <summary>
	/// Classe de base de SummaryTile et EditionTile, qui représente une entité.
	/// Son parent est forcément un TileGrouping.
	/// </summary>
	public abstract class AbstractTile : ContainerTile
	{
		public AbstractTile()
		{
		}

		public AbstractTile(Widget embedder)
			: this ()
		{
			this.SetEmbedder (embedder);
		}


		public Accessors.AbstractEntityAccessor EntitiesAccessor
		{
			get;
			set;
		}

		public AbstractEntity Entity
		{
			get;
			set;
		}

		public Controllers.ViewControllerMode ChildrenMode
		{
			get;
			set;
		}

		public bool EnableCreateAndRemoveButton
		{
			get;
			set;
		}

		
		public void CloseSubView(Orchestrators.DataViewOrchestrator orchestrator)
		{
			System.Diagnostics.Debug.Assert (this.subViewController != null);
			System.Diagnostics.Debug.Assert (orchestrator != null);

			orchestrator.CloseView (this.subViewController);
			
			System.Diagnostics.Debug.Assert (this.subViewController == null);
			System.Diagnostics.Debug.Assert (!this.IsSelected);
		}

		public void OpenSubView(Orchestrators.DataViewOrchestrator orchestrator, CoreViewController parentController)
		{
			this.subViewController = EntityViewController.CreateEntityViewController ("ViewController", this.Entity, this.ChildrenMode, orchestrator);
			this.subViewController.Disposing += this.HandleSubViewControllerDisposing;

			orchestrator.ShowSubView (parentController, this.subViewController);
			
			this.SetSelected (true);

			System.Diagnostics.Debug.Assert (this.subViewController != null);
			System.Diagnostics.Debug.Assert (this.IsSelected);
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				this.subViewController = null;
			}

			base.Dispose (disposing);
		}

		private void HandleSubViewControllerDisposing(object sender)
		{
			this.SetSelected (false);
			this.subViewController = null;
		}

		private CoreViewController subViewController;
	}
}
