//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Drawing;
using Epsitec.Common.Support;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Helpers;
using Epsitec.Cresus.Assets.App.Widgets;
using Epsitec.Cresus.Assets.Data;
using Epsitec.Cresus.Assets.Server.BusinessLogic;
using Epsitec.Cresus.Assets.Server.DataFillers;
using Epsitec.Cresus.Assets.Server.NodeGetters;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Popups
{
	/// <summary>
	/// Choix d'un code TVA.
	/// </summary>
	public class VatCodesPopup : AbstractPopup
	{
		private VatCodesPopup(DataAccessor accessor, BaseType baseType, string title, string selectedVatCode)
		{
			this.accessor = accessor;
			this.baseType = baseType;
			this.title    = title;

			this.controller = new NavigationTreeTableController(this.accessor);

			var primary     = this.accessor.GetNodeGetter (baseType);
			var secondary   = new SortableNodeGetter (primary, this.accessor, baseType);
			this.nodeGetter = new SorterNodeGetter (secondary);

			this.dataFiller = new SingleVatCodesTreeTableFiller (this.accessor, this.nodeGetter)
			{
				BaseType = this.baseType,
			};

			secondary.SetParams (null, this.dataFiller.DefaultSorting);
			this.nodeGetter.SetParams (this.dataFiller.DefaultSorting);

			this.visibleSelectedRow = this.nodeGetter.GetNodes ().ToList ().FindIndex (x => this.GetVatCode (x.Guid) == selectedVatCode);
			this.UpdateSelectedGuid ();

			//	Connexion des événements.
			this.controller.ContentChanged += delegate (object sender, bool crop)
			{
				this.UpdateController (crop);
			};

			this.controller.RowClicked += delegate (object sender, int row, int column)
			{
				this.visibleSelectedRow = this.controller.TopVisibleRow + row;
				this.UpdateController ();

				this.OnNavigate (this.SelectedVatCode);
				this.ClosePopup ();
			};
		}


		private string							SelectedVatCode
		{
			get
			{
				var node = this.nodeGetter[this.visibleSelectedRow];
				return this.GetVatCode (node.Guid);
			}
		}


		protected override Size					DialogSize
		{
			get
			{
				return this.GetSize ();
			}
		}

		protected override void CreateUI()
		{
			this.CreateTitle (this.title);
			this.CreateCloseButton ();

			var frame = new FrameBox
			{
				Parent = this.mainFrameBox,
				Dock   = DockStyle.Fill,
			};

			this.controller.CreateUI (frame, headerHeight: 0, footerHeight: 0);
			this.controller.AllowsMovement = false;
			this.controller.AllowsSorting  = false;

			TreeTableFiller<SortableNode>.FillColumns (this.controller, this.dataFiller, "Popup.VatCodes");

			this.UpdateController ();
		}


		[Command (Res.CommandIds.Popup.TreeTable.Prev)]
		private void DoPrev()
		{
			//	Appelé lorsque l'utilisateur presse sur "flèche en haut".
			if (this.visibleSelectedRow > 0)
			{
				this.visibleSelectedRow--;
				this.UpdateSelectedGuid ();
				this.UpdateController ();
			}
		}

		[Command (Res.CommandIds.Popup.TreeTable.Next)]
		private void DoNext()
		{
			//	Appelé lorsque l'utilisateur presse sur "flèche en bas".
			if (this.visibleSelectedRow < this.nodeGetter.Count-1)
			{
				this.visibleSelectedRow++;
				this.UpdateSelectedGuid ();
				this.UpdateController ();
			}
		}


		private string GetVatCode(Guid guid)
		{
			return VatCodesLogic.GetVatCode (this.accessor, this.baseType, guid);
		}


		private Size GetSize()
		{
			// TODO: faire autrement, car le mode est leftOrRight = false !
			var parent = this.GetParent ();

			//	On calcule une hauteur adaptée au contenu, mais qui ne dépasse
			//	évidement pas la hauteur de la fenêtre principale.
			double h = parent.ActualHeight
					 - AbstractScroller.DefaultBreadth;

			//	Utilise au maximum les 1/2 de la hauteur.
			int max = (int) (h*0.5) / VatCodesPopup.rowHeight;

			int rows = System.Math.Min (this.nodeGetter.Count, max);
			rows = System.Math.Max (rows, 3);

			int dx = this.PopupWidth
				   + (int) AbstractScroller.DefaultBreadth;

			int dy = AbstractPopup.titleHeight
				   + rows * VatCodesPopup.rowHeight
				   + (int) AbstractScroller.DefaultBreadth
				   + AbstractFilterController.height;

			return new Size (dx, dy);
		}

		private int								PopupWidth
		{
			get
			{
				return SingleVatCodesTreeTableFiller.TotalWidth;
			}
		}

		private void UpdateSelectedGuid()
		{
			if (this.visibleSelectedRow == -1)
			{
				this.selectedGuid = Guid.Empty;
			}
			else
			{
				this.selectedGuid = this.nodeGetter[this.visibleSelectedRow].Guid;
			}
		}

		private void UpdateController(bool crop = true)
		{
			TreeTableFiller<SortableNode>.FillContent (this.controller, this.dataFiller, this.visibleSelectedRow, crop);
		}


		#region Events handler
		private void OnNavigate(string account)
		{
			this.Navigate.Raise (this, account);
		}

		public event EventHandler<string> Navigate;
		#endregion


		#region Helpers
		public static void Show(Widget target, DataAccessor accessor, BaseType baseType, string title, string selectedVatCode, System.Action<string> action)
		{
			//	Affiche le popup pour choisir un code TVA.
			var popup = new VatCodesPopup (accessor, baseType, title, selectedVatCode);
			
			popup.Create (target, leftOrRight: true);
			
			popup.Navigate += delegate (object sender, string vatCode)
			{
				action (vatCode);
			};

			popup.Closed += delegate (object sender, ReasonClosure raison)
			{
				if (raison == ReasonClosure.AcceptKey)
				{
					action (popup.SelectedVatCode);
				}
			};
		}
		#endregion


		private const int rowHeight     = 18;

		private readonly DataAccessor					accessor;
		private readonly BaseType						baseType;
		private readonly string							title;
		private readonly NavigationTreeTableController	controller;
		private readonly SorterNodeGetter				nodeGetter;
		private readonly SingleVatCodesTreeTableFiller	dataFiller;

		private int										visibleSelectedRow;
		private Guid									selectedGuid;
	}
}