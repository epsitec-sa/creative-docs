//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

using Epsitec.Common.Widgets;
using Epsitec.Common.Types;
using Epsitec.Common.Drawing;
using Epsitec.Cresus.Assets.App.Widgets;

namespace Epsitec.Cresus.Assets.App.Views
{
	public class CategoriesView : AbstractView
	{
		public override void CreateUI(Widget parent, MainToolbar toolbar)
		{
			base.CreateUI (parent, toolbar);

			var topBox = new FrameBox
			{
				Parent  = parent,
				Dock    = DockStyle.Fill,
				Margins = new Margins (0, 0, 0, 2),
			};

			this.treeTableBox = new FrameBox
			{
				Parent = topBox,
				Dock   = DockStyle.Fill,
			};

			this.editBox = new FrameBox
			{
				Parent         = topBox,
				Dock           = DockStyle.Right,
				PreferredWidth = 500,
				Margins        = new Margins (5, 0, 0, 0),
				BackColor      = ColorManager.GetBackgroundColor (),
			};

			this.treeTableTopTitle = new TopTitle
			{
				Parent = this.treeTableBox,
			};

			this.treeTableTopTitle.SetTitle (this.Title);

			this.editTopTitle = new TopTitle
			{
				Parent = this.editBox,
			};

			this.editTopTitle.SetTitle ("Edition");

			this.lastSelectedRow = -2;

			this.treeTable = new SimpleTreeTableController ();
			this.treeTable.CreateUI (this.treeTableBox, footerHeight: 0);
			this.treeTable.SetColumns (this.Descriptions, 0);
			this.treeTable.Content.AddRange (this.Content);
			this.treeTable.UpdateContent ();

			this.treeTable.RowClicked += delegate
			{
				if (this.lastSelectedRow == this.treeTable.SelectedRow)
				{
					this.OnCommandEdit ();
				}
				else
				{
					this.lastSelectedRow = this.treeTable.SelectedRow;
					this.Update ();
				}
			};

			this.toolbar.CommandClicked += delegate (object sender, ToolbarCommand command)
			{
				switch (command)
				{
					case ToolbarCommand.Edit:
						this.OnCommandEdit ();
						break;

					case ToolbarCommand.Accept:
						this.OnCommandAccept ();
						break;

					case ToolbarCommand.Cancel:
						this.OnCommandCancel ();
						break;
				}
			};

			this.Update ();
		}


		public override string Title
		{
			get
			{
				return "Catégories d'immobilisation";
			}
		}


		private void OnCommandEdit()
		{
			this.editing = true;
			this.Update ();
		}

		private void OnCommandAccept()
		{
			this.editing = false;
			this.Update ();
		}

		private void OnCommandCancel()
		{
			if (this.editing)
			{
				this.editing = false;
			}
			else
			{
				this.treeTable.SelectedRow = -1;
			}

			this.Update ();
		}

		private void Update()
		{
			this.editBox.Visibility = this.editing;

			if (this.treeTable.SelectedRow == -1)
			{
				this.toolbar.SetCommandState (ToolbarCommand.Edit, ToolbarCommandState.Hide);
				this.toolbar.SetCommandState (ToolbarCommand.Accept, ToolbarCommandState.Hide);
				this.toolbar.SetCommandState (ToolbarCommand.Cancel, ToolbarCommandState.Hide);
			}
			else
			{
				if (this.editing)
				{
					this.toolbar.SetCommandState (ToolbarCommand.Edit, ToolbarCommandState.Hide);
					this.toolbar.SetCommandState (ToolbarCommand.Accept, ToolbarCommandState.Enable);
					this.toolbar.SetCommandState (ToolbarCommand.Cancel, ToolbarCommandState.Enable);
				}
				else
				{
					this.toolbar.SetCommandState (ToolbarCommand.Edit, ToolbarCommandState.Enable);
					this.toolbar.SetCommandState (ToolbarCommand.Accept, ToolbarCommandState.Disable);
					this.toolbar.SetCommandState (ToolbarCommand.Cancel, ToolbarCommandState.Enable);
				}

				//this.editTopTitle.SetTitle ("...");  // TODO
			}
		}


		private TreeTableColumnDescription[] Descriptions
		{
			get
			{
				var list = new List<TreeTableColumnDescription> ();

				list.Add (new TreeTableColumnDescription (TreeTableColumnType.String,  200, "Nom"));
				list.Add (new TreeTableColumnDescription (TreeTableColumnType.Decimal,  80, "Taux"));
				list.Add (new TreeTableColumnDescription (TreeTableColumnType.String,   80, "Type"));
				list.Add (new TreeTableColumnDescription (TreeTableColumnType.Decimal, 100, "Résidu"));

				return list.ToArray ();
			}
		}

		private List<List<AbstractSimpleTreeTableCell>> Content
		{
			get
			{
				var list = new List<List<AbstractSimpleTreeTableCell>> ();

				{
					var line = new List<AbstractSimpleTreeTableCell> ();
					line.Add (new SimpleTreeTableCellString ("Immobilier"));
					line.Add (new SimpleTreeTableCellDecimal (10.0m));
					line.Add (new SimpleTreeTableCellString ("Linéaire"));
					line.Add (new SimpleTreeTableCellDecimal (1000.0m));
					list.Add (line);
				}

				{
					var line = new List<AbstractSimpleTreeTableCell> ();
					line.Add (new SimpleTreeTableCellString ("Véhicules"));
					line.Add (new SimpleTreeTableCellDecimal (20.0m));
					line.Add (new SimpleTreeTableCellString ("Progressif"));
					line.Add (new SimpleTreeTableCellDecimal (1.0m));
					list.Add (line);
				}

				{
					var line = new List<AbstractSimpleTreeTableCell> ();
					line.Add (new SimpleTreeTableCellString ("Machines"));
					line.Add (new SimpleTreeTableCellDecimal (7.5m));
					line.Add (new SimpleTreeTableCellString ("Progressif"));
					line.Add (new SimpleTreeTableCellDecimal (1.0m));
					list.Add (line);
				}

				{
					var line = new List<AbstractSimpleTreeTableCell> ();
					line.Add (new SimpleTreeTableCellString ("Mobilier"));
					line.Add (new SimpleTreeTableCellDecimal (8.0m));
					line.Add (new SimpleTreeTableCellString ("Linéaire"));
					line.Add (new SimpleTreeTableCellDecimal (1.0m));
					list.Add (line);
				}

				return list;
			}
		}


		private FrameBox						treeTableBox;
		private FrameBox						editBox;

		private TopTitle						treeTableTopTitle;
		private TopTitle						editTopTitle;

		private bool							editing;

		private SimpleTreeTableController		treeTable;
		private int								lastSelectedRow;
	}
}
