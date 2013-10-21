//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Widgets;
using Epsitec.Cresus.Assets.Server.NaiveEngine;

namespace Epsitec.Cresus.Assets.App.Views
{
	public abstract class AbstractToolbarTreeTableController
	{
		public AbstractToolbarTreeTableController(DataAccessor accessor)
		{
			this.accessor = accessor;
		}

		public void CreateUI(Widget parent)
		{
			this.topTitle = new TopTitle
			{
				Parent = parent,
			};

			this.topTitle.SetTitle (this.title);

			this.toolbar = new TreeTableToolbar ();
			this.toolbar.CreateUI (parent);

			this.CreateTreeTable (parent);

			this.toolbar.CommandClicked += delegate (object sender, ToolbarCommand command)
			{
				switch (command)
				{
					case ToolbarCommand.First:
						this.OnFirst ();
						break;

					case ToolbarCommand.Last:
						this.OnLast ();
						break;

					case ToolbarCommand.Prev:
						this.OnPrev ();
						break;

					case ToolbarCommand.Next:
						this.OnNext ();
						break;

					case ToolbarCommand.New:
						this.OnNew ();
						break;

					case ToolbarCommand.Delete:
						this.OnDelete ();
						break;

					case ToolbarCommand.Deselect:
						this.OnDeselect ();
						break;
				}
			};
		}


		public int								SelectedRow
		{
			get
			{
				return this.selectedRow;
			}
			set
			{
				if (this.selectedRow != value)
				{
					this.selectedRow = value;

					this.UpdateController ();
					this.UpdateToolbar ();

					this.OnSelectedRowChanged (this.selectedRow);
				}
			}
		}


		private void OnFirst()
		{
			var index = this.FirstRowIndex;

			if (index.HasValue)
			{
				this.SelectedRow = index.Value;
			}
		}

		private void OnPrev()
		{
			var index = this.PrevRowIndex;

			if (index.HasValue)
			{
				this.SelectedRow = index.Value;
			}
		}

		private void OnNext()
		{
			var index = this.NextRowIndex;

			if (index.HasValue)
			{
				this.SelectedRow = index.Value;
			}
		}

		private void OnLast()
		{
			var index = this.LastRowIndex;

			if (index.HasValue)
			{
				this.SelectedRow = index.Value;
			}
		}

		protected virtual void OnNew()
		{
		}

		protected virtual void OnDelete()
		{
		}

		private void OnDeselect()
		{
			this.SelectedRow = -1;
		}


		private void CreateTreeTable(Widget parent)
		{
			this.selectedRow = -1;

			this.controller = new NavigationTreeTableController();

			var frame = new FrameBox
			{
				Parent = parent,
				Dock   = DockStyle.Fill,
			};

			this.controller.CreateUI (frame, footerHeight: 0);
			this.controller.SetColumns (this.TreeTableColumns, 1);

			//	Pour que le calcul du nombre de lignes visibles soit correct.
			parent.Window.ForceLayout ();

			this.UpdateController ();
			this.UpdateToolbar ();

			//	Connexion des événements.
			this.controller.ContentChanged += delegate (object sender, bool crop)
			{
				this.UpdateController (crop);
			};

			this.controller.RowClicked += delegate (object sender, int row)
			{
				this.SelectedRow = this.controller.TopVisibleRow + row;
			};

			this.controller.RowDoubleClicked += delegate (object sender, int row)
			{
				this.SelectedRow = this.controller.TopVisibleRow + row;
				this.OnRowDoubleClicked (this.SelectedRow);
			};

			this.controller.TreeButtonClicked += delegate (object sender, int row, TreeTableTreeType type)
			{
			};
		}

		protected virtual TreeTableColumnDescription[] TreeTableColumns
		{
			get
			{
				return null;
			}
		}

		protected void UpdateController(bool crop = true)
		{
			this.controller.RowsCount = this.RowsCount;

			int visibleCount = this.controller.VisibleRowsCount;
			int rowsCount    = this.controller.RowsCount;
			int count        = System.Math.Min (visibleCount, rowsCount);
			int firstRow     = this.controller.TopVisibleRow;
			int selection    = this.selectedRow;

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

			this.UpdateContent (firstRow, count, selection, crop);
		}

		protected virtual int RowsCount
		{
			get
			{
				return 0;
			}
		}

		protected virtual void UpdateContent(int firstRow, int count, int selection, bool crop = true)
		{
		}

		protected void UpdateToolbar()
		{
			int row = this.SelectedRow;

			this.toolbar.SetCommandState (ToolbarCommand.First, ToolbarCommandState.Hide);
			this.toolbar.SetCommandState (ToolbarCommand.Last,  ToolbarCommandState.Hide);

//			this.UpdateCommand (ToolbarCommand.First, row, this.FirstRowIndex);
			this.UpdateCommand (ToolbarCommand.Prev,  row, this.PrevRowIndex);
			this.UpdateCommand (ToolbarCommand.Next,  row, this.NextRowIndex);
//			this.UpdateCommand (ToolbarCommand.Last,  row, this.LastRowIndex);

			this.UpdateCommand (ToolbarCommand.New,      true);
			this.UpdateCommand (ToolbarCommand.Delete,   row != -1);
			this.UpdateCommand (ToolbarCommand.Deselect, row != -1);
		}

		private void UpdateCommand(ToolbarCommand command, int selectedCell, int? newSelection)
		{
			bool enable = (newSelection.HasValue && selectedCell != newSelection.Value);
			this.UpdateCommand (command, enable);
		}

		private void UpdateCommand(ToolbarCommand command, bool enable)
		{
			if (enable)
			{
				this.toolbar.SetCommandState (command, ToolbarCommandState.Enable);
			}
			else
			{
				this.toolbar.SetCommandState (command, ToolbarCommandState.Disable);
			}
		}


		private int? FirstRowIndex
		{
			get
			{
				return 0;
			}
		}

		private int? PrevRowIndex
		{
			get
			{
				if (this.selectedRow == -1)
				{
					return null;
				}
				else
				{
					int i = this.selectedRow - 1;
					i = System.Math.Max (i, 0);
					i = System.Math.Min (i, this.controller.RowsCount - 1);
					return i;
				}
			}
		}

		private int? NextRowIndex
		{
			get
			{
				if (this.selectedRow == -1)
				{
					return null;
				}
				else
				{
					int i = this.selectedRow + 1;
					i = System.Math.Max (i, 0);
					i = System.Math.Min (i, this.controller.RowsCount - 1);
					return i;
				}
			}
		}

		private int? LastRowIndex
		{
			get
			{
				return this.controller.RowsCount - 1;
			}
		}


		#region Events handler
		private void OnSelectedRowChanged(int row)
		{
			if (this.SelectedRowChanged != null)
			{
				this.SelectedRowChanged (this, row);
			}
		}

		public delegate void SelectedRowChangedEventHandler(object sender, int row);
		public event SelectedRowChangedEventHandler SelectedRowChanged;


		private void OnRowDoubleClicked(int row)
		{
			if (this.RowDoubleClicked != null)
			{
				this.RowDoubleClicked (this, row);
			}
		}

		public delegate void RowDoubleClickedEventHandler(object sender, int row);
		public event RowDoubleClickedEventHandler RowDoubleClicked;
		#endregion


		protected readonly DataAccessor			accessor;

		protected string						title;
		protected TopTitle						topTitle;
		protected TreeTableToolbar				toolbar;
		protected NavigationTreeTableController	controller;
		protected int							selectedRow;
	}
}
