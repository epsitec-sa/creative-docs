//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Helpers;
using Epsitec.Cresus.Assets.App.Widgets;
using Epsitec.Cresus.Assets.Server.BusinessLogic;
using Epsitec.Cresus.Assets.Server.DataFillers;
using Epsitec.Cresus.Assets.Server.NodesGetter;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Popups
{
	/// <summary>
	/// Affiche une liste d'erreurs ou de messages, appelée "résultats".
	/// </summary>
	public class ErrorsPopup : AbstractPopup
	{
		public ErrorsPopup(DataAccessor accessor, List<Error> errors)
		{
			this.accessor = accessor;
			this.errors   = errors;

			this.controller = new NavigationTreeTableController ();

			this.nodesGetter = new ErrorNodesGetter (this.errors);
			this.dataFiller = new ErrorsTreeTableFiller (this.accessor, this.nodesGetter);

			this.visibleSelectedRow = -1;
		}


		protected override Size DialogSize
		{
			get
			{
				return new Size (ErrorsPopup.PopupWidth, ErrorsPopup.PopupHeight);
			}
		}

		public override void CreateUI()
		{
			this.CreateTitle ("Résultats");
			this.CreateCloseButton ();
			this.CreateTreeTable ();
		}

		private void CreateTreeTable()
		{
			var frame = new FrameBox
			{
				Parent = this.mainFrameBox,
				Dock   = DockStyle.Fill,
			};

			this.controller.CreateUI (frame, rowHeight: ErrorsPopup.LineHeight, headerHeight: 0, footerHeight: 0);
			this.controller.AllowsMovement = false;

			TreeTableFiller<Error>.FillColumns (this.dataFiller, this.controller);
			this.UpdateController ();

			//	Connexion des événements.
			this.controller.RowClicked += delegate (object sender, int row)
			{
				this.visibleSelectedRow = this.controller.TopVisibleRow + row;
				this.UpdateController ();
			};

			this.controller.ContentChanged += delegate (object sender, bool crop)
			{
				this.UpdateController (crop);
			};
		}

		private void UpdateController(bool crop = true)
		{
			this.controller.RowsCount = this.nodesGetter.Count;

			int visibleCount = this.controller.VisibleRowsCount;
			int rowsCount    = this.controller.RowsCount;
			int count        = System.Math.Min (visibleCount, rowsCount);
			int firstRow     = this.controller.TopVisibleRow;
			int selection    = this.visibleSelectedRow;

			if (selection != -1)
			{
				//	La sélection ne peut pas dépasser le nombre maximal de lignes.
				selection = System.Math.Min (selection, rowsCount-1);

				//	Si la sélection est hors de la zone visible, on choisit un autre cadrage.
				if (crop && (selection < firstRow || selection >= firstRow+count))
				{
					firstRow = this.controller.GetTopVisibleRow (selection);
				}

				if (this.controller.TopVisibleRow != firstRow)
				{
					this.controller.TopVisibleRow = firstRow;
				}

				selection -= this.controller.TopVisibleRow;
			}

			TreeTableFiller<Error>.FillContent (this.dataFiller, this.controller, firstRow, count, selection);
		}


		private static readonly int LineHeight  = 17;
		private static readonly int PopupWidth  = 400;
		private static readonly int PopupHeight = 230;

		private readonly DataAccessor						accessor;
		private readonly List<Error>						errors;
		private readonly NavigationTreeTableController		controller;
		private readonly ErrorNodesGetter					nodesGetter;
		private readonly ErrorsTreeTableFiller				dataFiller;

		private int											visibleSelectedRow;
	}
}