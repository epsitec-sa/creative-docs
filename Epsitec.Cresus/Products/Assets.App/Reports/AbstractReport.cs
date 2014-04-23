//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.App.Widgets;
using Epsitec.Cresus.Assets.Server.NodeGetters;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Views
{
	public abstract class AbstractReport
	{
		public AbstractReport(DataAccessor accessor, NavigationTreeTableController treeTableController)
		{
			this.accessor = accessor;
			this.treeTableController = treeTableController;
		}

		public virtual void Dispose()
		{
			this.treeTableController.RowClicked        -= this.HandleRowClicked;
			this.treeTableController.ContentChanged    -= this.HandleContentChanged;
			this.treeTableController.TreeButtonClicked -= this.HandleTreeButtonClicked;
		}


		public virtual void Initialize()
		{
			this.UpdateTreeTable ();

			//	Connexion des événements.
			this.treeTableController.RowClicked        += this.HandleRowClicked;
			this.treeTableController.ContentChanged    += this.HandleContentChanged;
			this.treeTableController.TreeButtonClicked += this.HandleTreeButtonClicked;
		}

		public void SetParams(AbstractParams reportParams)
		{
			this.reportParams = reportParams;
			this.UpdateParams ();
		}

		protected virtual void UpdateParams()
		{
		}


		protected void HandleRowClicked(object sender, int row, int column)
		{
			this.visibleSelectedRow = this.treeTableController.TopVisibleRow + row;
			this.UpdateTreeTable ();
		}

		protected void HandleContentChanged(object sender, bool row)
		{
			this.UpdateTreeTable ();
		}

		protected void HandleTreeButtonClicked(object sender, int row, NodeType type)
		{
			this.OnCompactOrExpand (this.treeTableController.TopVisibleRow + row);
		}


		protected void OnCompactOrExpand(int row)
		{
			//	Etend ou compacte une ligne (inverse son mode actuel).
			this.nodeGetter.CompactOrExpand (row);
			this.UpdateTreeTable ();
		}

		protected void OnCompactAll()
		{
			//	Compacte toutes les lignes.
			this.nodeGetter.CompactAll ();
			this.UpdateTreeTable ();
		}

		protected void OnCompactOne()
		{
			//	Compacte une ligne.
			this.nodeGetter.CompactOne ();
			this.UpdateTreeTable ();
		}

		protected void OnExpandOne()
		{
			//	Etend une ligne.
			this.nodeGetter.ExpandOne ();
			this.UpdateTreeTable ();
		}

		protected void OnExpandAll()
		{
			//	Etend toutes les lignes.
			this.nodeGetter.ExpandAll ();
			this.UpdateTreeTable ();
		}


		protected virtual void UpdateTreeTable()
		{
		}


		protected readonly DataAccessor			accessor;
		protected readonly NavigationTreeTableController treeTableController;

		protected ITreeFunctions				nodeGetter;
		protected AbstractParams				reportParams;
		protected int							visibleSelectedRow;
	}
}
