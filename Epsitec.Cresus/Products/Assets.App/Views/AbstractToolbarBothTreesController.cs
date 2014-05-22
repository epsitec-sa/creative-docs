//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Export;
using Epsitec.Cresus.Assets.App.Helpers;
using Epsitec.Cresus.Assets.App.Popups;
using Epsitec.Cresus.Assets.App.Settings;
using Epsitec.Cresus.Assets.App.Widgets;
using Epsitec.Cresus.Assets.Data;
using Epsitec.Cresus.Assets.Server.DataFillers;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Views
{
	/// <summary>
	/// Classe de base pour tous les contrôleurs TreeTable et/ou TreeGraphic.
	/// Le TreeTable est toujours présent. Gère la présence optionnelle du TreeGraphic.
	/// </summary>
	public abstract class AbstractToolbarBothTreesController<T> : AbstractToolbarTreeController<T>
		where T : struct
	{
		public AbstractToolbarBothTreesController(DataAccessor accessor, BaseType baseType)
			: base (accessor, baseType)
		{
		}


		protected override void CreateControllerUI(Widget parent)
		{
			this.selectedRow = -1;

			this.treeTableFrame = new FrameBox
			{
				Parent = parent,
				Dock   = DockStyle.Fill,
			};

			this.graphicFrame = new FrameBox
			{
				Parent = parent,
				Dock   = DockStyle.Fill,
			};

			this.bottomFrame = new FrameBox
			{
				Parent          = parent,
				Dock            = DockStyle.Bottom,
				PreferredHeight = AbstractScroller.DefaultBreadth,
				Visibility      = false,
			};

			this.CreateTreeTableControllerUI (this.treeTableFrame);
			this.CreateGraphicControllerUI (this.graphicFrame);

			this.UpdateGraphicMode ();
		}

		private void CreateTreeTableControllerUI(Widget parent)
		{
			//	Crée le contrôleur TreeTable.
			this.treeTableController = new NavigationTreeTableController ();

			var frame = new FrameBox
			{
				Parent = parent,
				Dock   = DockStyle.Fill,
			};

			this.treeTableController.CreateUI (frame, footerHeight: 0);
			this.CreateNodeFiller ();
			this.UpdateController ();

			//	Pour que le calcul du nombre de lignes visibles soit correct.
			parent.Window.ForceLayout ();

			//	Connexion des événements.
			this.treeTableController.ContentChanged += delegate (object sender, bool crop)
			{
				this.UpdateController (crop);
			};

			this.treeTableController.SortingChanged += delegate
			{
				using (new SaveSelectedGuid (this))
				{
					this.UpdateSorting ();
				}
			};

			this.treeTableController.RowClicked += delegate (object sender, int row, int column)
			{
				this.VisibleSelectedRow = this.treeTableController.TopVisibleRow + row;
			};

			this.treeTableController.RowDoubleClicked += delegate (object sender, int row)
			{
				this.VisibleSelectedRow = this.treeTableController.TopVisibleRow + row;
				this.OnRowDoubleClicked (this.VisibleSelectedRow);
			};

			this.treeTableController.TreeButtonClicked += delegate (object sender, int row, NodeType type)
			{
				this.OnCompactOrExpand (this.treeTableController.TopVisibleRow + row);
			};
		}

		private void CreateGraphicControllerUI(Widget parent)
		{
			//	Crée le contrôleur TreeGraphic, si nécessaire.
			this.CreateGraphicControllerUI ();

			if (this.graphicController == null)  // pas de TreeGraphic ?
			{
				return;
			}

			this.graphicController.CreateUI (parent);

			this.graphicController.TileClicked += delegate (object sender, Guid guid)
			{
				this.SelectedGuid = guid;
			};

			this.graphicController.TileDoubleClicked += delegate (object sender, Guid guid)
			{
				this.SelectedGuid = guid;
				this.OnRowDoubleClicked (this.VisibleSelectedRow);
			};

			this.graphicController.TileCompactOrExpand += delegate (object sender, Guid guid)
			{
				this.SelectedGuid = guid;
				this.OnCompactOrExpand (this.VisibleSelectedRow);
			};
		}

		protected virtual void CreateGraphicControllerUI()
		{
			//	Les classes qui ont une représentation graphique n'ont qu'à surcharger
			//	cette méthode.
		}


		protected override void UpdateGraphicMode()
		{
			this.treeTableFrame.Visibility = !this.showGraphic;
			this.graphicFrame.Visibility   =  this.showGraphic;

			this.UpdateToolbar ();
		}


		private void UpdateSorting()
		{
			//	Met à jour les instructions de tri des getters en fonction des choix
			//	effectués dans le TreeTable.
			this.sortingInstructions = TreeTableFiller<T>.GetSortingInstructions (this.treeTableController, this.dataFiller);
			this.UpdateData ();
		}


		protected override void UpdateController(bool crop = true)
		{
			if (this.dataFiller != null)
			{
				TreeTableFiller<T>.FillContent (this.treeTableController, this.dataFiller, this.VisibleSelectedRow, crop);
			}

			if (this.graphicController != null)
			{
				this.graphicController.UpdateController (this.nodeGetter, this.SelectedGuid, crop);
			}
		}


		protected override void OnExport()
		{
			var target = this.toolbar.GetTarget (ToolbarCommand.Export);
			this.ShowExportPopup (target);
		}

		private void ShowExportPopup(Widget target)
		{
			var popup = new ExportPopup (this.accessor)
			{
				Inverted = false,
				Filename = LocalSettings.ExportFilename,
				Filters  = ReportsView.ExportFilters,
			};

			popup.Create (target, leftOrRight: true);

			popup.ButtonClicked += delegate (object sender, string name)
			{
				if (name == "ok")
				{
					LocalSettings.ExportFilename = popup.Filename;
					this.Export (popup.Filename);
				}
			};
		}

		protected void Export(string filename)
		{
			var engine = new ExportToText<T> ()
			{
				ExportTextProfile = ExportTextProfile.CsvProfile,
			};

			engine.Export (this.dataFiller, filename);
		}


		protected FrameBox							treeTableFrame;
		protected FrameBox							graphicFrame;
		protected FrameBox							bottomFrame;

		protected AbstractTreeTableFiller<T>		dataFiller;
		protected NavigationTreeTableController		treeTableController;
		protected AbstractTreeGraphicController<T>	graphicController;
	}
}
