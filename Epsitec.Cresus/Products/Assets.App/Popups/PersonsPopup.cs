//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Helpers;
using Epsitec.Cresus.Assets.App.Widgets;
using Epsitec.Cresus.Assets.Data;
using Epsitec.Cresus.Assets.Server.DataFillers;
using Epsitec.Cresus.Assets.Server.NodeGetters;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Popups
{
	public class PersonsPopup : AbstractPopup
	{
		public PersonsPopup(DataAccessor accessor, Guid selectedGuid)
		{
			this.accessor = accessor;

			this.controller = new NavigationTreeTableController ();

			var primary     = this.accessor.GetNodeGetter (BaseType.Persons);
			var secondary   = new SortableNodeGetter (primary, this.accessor, BaseType.Persons);
			this.nodeGetter = new SorterNodeGetter (secondary);

			var field = this.accessor.GetMainStringField (BaseType.Persons);
			var si = new SortingInstructions (field, SortedType.Ascending, ObjectField.Unknown, SortedType.None);

			secondary.SetParams (null, si);
			this.nodeGetter.SetParams (si);

			this.visibleSelectedRow = this.nodeGetter.GetNodes ().ToList ().FindIndex (x => x.Guid == selectedGuid);

			this.dataFiller = new PersonsTreeTableFiller (this.accessor, this.nodeGetter);
		}


		protected override Size DialogSize
		{
			get
			{
				return this.GetSize ();
			}
		}

		public override void CreateUI()
		{
			this.CreateTitle (Res.Strings.Popup.Persons.Title.ToString ());
			this.CreateCloseButton ();

			var frame = new FrameBox
			{
				Parent = this.mainFrameBox,
				Dock   = DockStyle.Fill,
			};

			this.controller.CreateUI (frame, headerHeight: PersonsPopup.headerHeight, footerHeight: 0);
			this.controller.AllowsMovement = false;
			this.controller.AllowsSorting  = false;

			TreeTableFiller<SortableNode>.FillColumns (this.controller, this.dataFiller, "Popup.Persons");
			this.UpdateController ();

			//	Connexion des événements.
			this.controller.ContentChanged += delegate (object sender, bool crop)
			{
				this.UpdateController (crop);
			};

			this.controller.RowClicked += delegate (object sender, int row, int column)
			{
				this.visibleSelectedRow = this.controller.TopVisibleRow + row;
				this.UpdateController ();

				var node = this.nodeGetter[this.visibleSelectedRow];
				this.OnNavigate (node.Guid);
				this.ClosePopup ();
			};
		}


		private Size GetSize()
		{
			var parent = this.GetParent ();

			//	On calcule une hauteur adaptée au contenu, mais qui ne dépasse
			//	évidement pas la hauteur de la fenêtre principale.
			double h = parent.ActualHeight
					 - PersonsPopup.headerHeight
					 - AbstractScroller.DefaultBreadth;

			//	Utilise au maximum les 4/10 de la hauteur.
			int max = (int) (h*0.4) / PersonsPopup.rowHeight;

			int rows = System.Math.Min (this.nodeGetter.Count, max);
			rows = System.Math.Max (rows, 3);

			int dx = PersonsPopup.popupWidth
				   + (int) AbstractScroller.DefaultBreadth;

			int dy = AbstractPopup.titleHeight
				   + PersonsPopup.headerHeight
				   + rows * PersonsPopup.rowHeight
				   + (int) AbstractScroller.DefaultBreadth;

			return new Size (dx, dy);
		}

		private void UpdateController(bool crop = true)
		{
			TreeTableFiller<SortableNode>.FillContent (this.controller, this.dataFiller, this.visibleSelectedRow, crop);
		}


		#region Events handler
		private void OnNavigate(Guid guid)
		{
			this.Navigate.Raise (this, guid);
		}

		public event EventHandler<Guid> Navigate;
		#endregion


		private const int headerHeight     = 22;
		private const int rowHeight        = 18;
		private const int popupWidth       = 600;

		private readonly DataAccessor					accessor;
		private readonly NavigationTreeTableController	controller;
		private readonly SorterNodeGetter				nodeGetter;
		private readonly PersonsTreeTableFiller			dataFiller;

		private int										visibleSelectedRow;
	}
}