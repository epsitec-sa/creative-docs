//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Helpers;
using Epsitec.Cresus.Assets.App.Widgets;
using Epsitec.Cresus.Assets.Server.DataFillers;
using Epsitec.Cresus.Assets.Server.NodesGetter;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Views
{
	public abstract class AbstractToolbarTreeTableController<T>
		where T : struct
	{
		public AbstractToolbarTreeTableController(DataAccessor accessor)
		{
			this.accessor = accessor;
		}


		public virtual void CreateUI(Widget parent)
		{
			this.topTitle = new TopTitle
			{
				Parent = parent,
			};

			this.topTitle.SetTitle (this.title);

			this.toolbar = new TreeTableToolbar ();
			this.toolbar.CreateUI (parent);
			this.toolbar.HasFilter         = this.hasFilter;
			this.toolbar.HasTreeOperations = this.hasTreeOperations;

			this.CreateTreeTable (parent);
			this.CreateNodeFiller ();

			this.toolbar.CommandClicked += delegate (object sender, ToolbarCommand command)
			{
				switch (command)
				{
					case ToolbarCommand.Filter:
						this.OnFilter ();
						break;

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

					case ToolbarCommand.CompactAll:
						this.OnCompactAll ();
						break;

					case ToolbarCommand.ExpandAll:
						this.OnExpandAll ();
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


		public virtual void UpdateData()
		{
		}


		public virtual Guid						SelectedGuid
		{
			get;
			set;
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

		protected virtual int					VisibleSelectedRow
		{
			get
			{
				return this.SelectedRow;
			}
			set
			{
				this.SelectedRow = value;
			}
		}


		protected virtual void OnFilter()
		{
		}

		protected void OnFirst()
		{
			var index = this.FirstRowIndex;

			if (index.HasValue)
			{
				this.VisibleSelectedRow = index.Value;
			}
		}

		protected void OnPrev()
		{
			var index = this.PrevRowIndex;

			if (index.HasValue)
			{
				this.VisibleSelectedRow = index.Value;
			}
		}

		protected void OnNext()
		{
			var index = this.NextRowIndex;

			if (index.HasValue)
			{
				this.VisibleSelectedRow = index.Value;
			}
		}

		protected void OnLast()
		{
			var index = this.LastRowIndex;

			if (index.HasValue)
			{
				this.VisibleSelectedRow = index.Value;
			}
		}

		private void OnCompactOrExpand(int row)
		{
			//	Etend ou compacte une ligne (inverse son mode actuel).
			using (new SaveCurrentGuid (this))
			{
				this.TreeNodesGetter.CompactOrExpand (row);
				this.UpdateController ();
				this.UpdateToolbar ();
			}
		}

		private void OnCompactAll()
		{
			//	Compacte toutes les lignes.
			using (new SaveCurrentGuid (this))
			{
				this.TreeNodesGetter.CompactAll ();
				this.UpdateController ();
				this.UpdateToolbar ();
			}
		}

		private void OnExpandAll()
		{
			//	Etend toutes les lignes.
			using (new SaveCurrentGuid (this))
			{
				this.TreeNodesGetter.ExpandAll ();
				this.UpdateController ();
				this.UpdateToolbar ();
			}
		}

		protected virtual void OnDeselect()
		{
		}

		protected virtual void OnNew()
		{
		}

		protected virtual void OnDelete()
		{
		}


		protected virtual void CreateNodeFiller()
		{
		}

		private void CreateTreeTable(Widget parent)
		{
			this.selectedRow = -1;

			this.controller = new NavigationTreeTableController ();

			var frame = new FrameBox
			{
				Parent = parent,
				Dock   = DockStyle.Fill,
			};

			this.controller.CreateUI (frame, footerHeight: 0);

			//	Pour que le calcul du nombre de lignes visibles soit correct.
			parent.Window.ForceLayout ();

			//	Connexion des événements.
			this.controller.ContentChanged += delegate (object sender, bool crop)
			{
				this.UpdateController (crop);
			};

			this.controller.SortingChanged += delegate
			{
				using (new SaveCurrentGuid (this))
				{
					this.UpdateSorting ();
				}
			};

			this.controller.RowClicked += delegate (object sender, int row)
			{
				this.VisibleSelectedRow = this.controller.TopVisibleRow + row;
			};

			this.controller.RowDoubleClicked += delegate (object sender, int row)
			{
				this.VisibleSelectedRow = this.controller.TopVisibleRow + row;
				this.OnRowDoubleClicked (this.VisibleSelectedRow);
			};

			this.controller.TreeButtonClicked += delegate (object sender, int row, NodeType type)
			{
				this.OnCompactOrExpand (this.controller.TopVisibleRow + row);
			};
		}


		private void UpdateSorting()
		{
			//	Met à jour les instructions de tri des getters en fonction des choix
			//	effectués dans le TreeTable.
			this.sortingInstructions = TreeTableFiller<T>.GetSortingInstructions (this.controller, this.dataFiller);
			this.UpdateData ();
		}


		protected void UpdateController(bool crop = true)
		{
			TreeTableFiller<T>.FillContent (this.controller, this.dataFiller, this.VisibleSelectedRow, crop);
		}

		protected virtual void UpdateToolbar()
		{
			int row = this.VisibleSelectedRow;

			this.UpdateCommand (ToolbarCommand.First, row, this.FirstRowIndex);
			this.UpdateCommand (ToolbarCommand.Prev,  row, this.PrevRowIndex);
			this.UpdateCommand (ToolbarCommand.Next,  row, this.NextRowIndex);
			this.UpdateCommand (ToolbarCommand.Last,  row, this.LastRowIndex);

			this.toolbar.UpdateCommand (ToolbarCommand.New,      true);
			this.toolbar.UpdateCommand (ToolbarCommand.Delete,   row != -1);
			this.toolbar.UpdateCommand (ToolbarCommand.Deselect, row != -1);
		}

		protected void UpdateCommand(ToolbarCommand command, int selectedCell, int? newSelection)
		{
			bool enable = (newSelection.HasValue && selectedCell != newSelection.Value);
			this.toolbar.UpdateCommand (command, enable);
		}


		protected int? FirstRowIndex
		{
			get
			{
				return 0;
			}
		}

		protected int? PrevRowIndex
		{
			get
			{
				if (this.VisibleSelectedRow == -1)
				{
					return null;
				}
				else
				{
					int i = this.VisibleSelectedRow - 1;
					i = System.Math.Max (i, 0);
					i = System.Math.Min (i, this.nodesGetter.Count - 1);
					return i;
				}
			}
		}

		protected int? NextRowIndex
		{
			get
			{
				if (this.VisibleSelectedRow == -1)
				{
					return null;
				}
				else
				{
					int i = this.VisibleSelectedRow + 1;
					i = System.Math.Max (i, 0);
					i = System.Math.Min (i, this.nodesGetter.Count - 1);
					return i;
				}
			}
		}

		protected int? LastRowIndex
		{
			get
			{
				return this.nodesGetter.Count - 1;
			}
		}


		protected class SaveCurrentGuid : System.IDisposable
		{
			public SaveCurrentGuid(AbstractToolbarTreeTableController<T> controller)
			{
				this.controller = controller;
				this.currentGuid = this.controller.SelectedGuid;
			}

			public void Dispose()
			{
				this.controller.SelectedGuid = this.currentGuid;
			}

			private readonly AbstractToolbarTreeTableController<T>	controller;
			private readonly Guid									currentGuid;
		}


		private ITreeFonctions TreeNodesGetter
		{
			get
			{
				return this.nodesGetter as ITreeFonctions;
			}
		}


		#region Events handler
		protected void OnSelectedRowChanged(int row)
		{
			this.SelectedRowChanged.Raise (this, row);
		}

		public event EventHandler<int> SelectedRowChanged;


		protected void OnRowDoubleClicked(int row)
		{
			this.RowDoubleClicked.Raise (this, row);
		}

		public event EventHandler<int> RowDoubleClicked;


		protected void OnStartEditing(EventType eventType, Timestamp timestamp)
		{
			this.StartEditing.Raise (this, eventType, timestamp);
		}

		public event EventHandler<EventType, Timestamp> StartEditing;
		#endregion


		protected readonly DataAccessor			accessor;

		protected string						title;
		protected bool							hasFilter;
		protected bool							hasTreeOperations;
		protected AbstractNodesGetter<T>		nodesGetter;
		protected AbstractTreeTableFiller<T>	dataFiller;
		protected TopTitle						topTitle;
		protected NavigationTreeTableController	controller;
		protected int							selectedRow;
		protected TreeTableToolbar				toolbar;
		protected SortingInstructions			sortingInstructions;
	}
}
