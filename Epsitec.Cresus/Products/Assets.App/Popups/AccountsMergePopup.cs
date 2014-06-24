//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.DataFillers;
using Epsitec.Cresus.Assets.App.Helpers;
using Epsitec.Cresus.Assets.App.NodeGetters;
using Epsitec.Cresus.Assets.App.Widgets;
using Epsitec.Cresus.Assets.Data;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Popups
{
	public class AccountsMergePopup : AbstractPopup
	{
		public AccountsMergePopup(DataAccessor accessor, Dictionary<DataObject, DataObject> todo)
		{
			this.accessor = accessor;

			this.controller = new NavigationTreeTableController();

			this.nodeGetter = new AccountsMergeNodeGetter ();
			this.nodeGetter.SetParams (todo);

			this.dataFiller = new AccountsMergeTreeTableFiller (this.accessor, this.nodeGetter);

			this.visibleSelectedRow = -1;

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
				//-this.OnNavigate (node.Guid);
				//-this.ClosePopup ();
			};

			//-this.controller.TreeButtonClicked += delegate (object sender, int row, NodeType type)
			//-{
			//-	this.OnCompactOrExpand (this.controller.TopVisibleRow + row);
			//-};
		}


		protected override Size DialogSize
		{
			get
			{
				return new Size (500, 300);
			}
		}

		public override void CreateUI()
		{
			this.CreateTitle ("Fusion des comptes importés dans le plan comptable");
			this.CreateCloseButton ();

			var frame = new FrameBox
			{
				Parent = this.mainFrameBox,
				Dock   = DockStyle.Fill,
			};

			this.controller.CreateUI (frame, headerHeight: 0, footerHeight: 0);
			this.controller.AllowsMovement = false;

			TreeTableFiller<AccountsMergeNode>.FillColumns (this.controller, this.dataFiller, "Popup.Groups");
			//?this.nodeGetter.SetParams (null, TreeTableFiller<TreeNode>.GetSortingInstructions (this.controller));

			this.UpdateController ();
		}


		//-private void OnCompactOrExpand(int row)
		//-{
		//-	//	Etend ou compacte une ligne (inverse son mode actuel).
		//-	var guid = this.SelectedGuid;
		//-
		//-	this.nodeGetter.CompactOrExpand (row);
		//-	this.UpdateController ();
		//-
		//-	this.SelectedGuid = guid;
		//-}

		//-private Guid SelectedGuid
		//-{
		//-	//	Retourne le Guid de l'objet actuellement sélectionné.
		//-	get
		//-	{
		//-		int sel = this.visibleSelectedRow;
		//-		if (sel != -1 && sel < this.nodeGetter.Count)
		//-		{
		//-			return this.nodeGetter[sel].Guid;
		//-		}
		//-		else
		//-		{
		//-			return Guid.Empty;
		//-		}
		//-	}
		//-	//	Sélectionne l'objet ayant un Guid donné. Si la ligne correspondante
		//-	//	est cachée, on est assez malin pour sélectionner la prochaine ligne
		//-	//	visible, vers le haut.
		//-	set
		//-	{
		//-		this.visibleSelectedRow = this.nodeGetter.SearchBestIndex (value);
		//-		this.UpdateController ();
		//-	}
		//-}


		private void UpdateController(bool crop = true)
		{
			TreeTableFiller<AccountsMergeNode>.FillContent (this.controller, this.dataFiller, this.visibleSelectedRow, crop);
		}


		#region Events handler
		private void OnNavigate(Guid guid)
		{
			this.Navigate.Raise (this, guid);
		}

		public event EventHandler<Guid> Navigate;
		#endregion


		private const int rowHeight        = 18;

		private readonly DataAccessor					accessor;
		private readonly NavigationTreeTableController	controller;
		private readonly AccountsMergeNodeGetter		nodeGetter;
		private readonly AccountsMergeTreeTableFiller	dataFiller;

		private int										visibleSelectedRow;
	}
}