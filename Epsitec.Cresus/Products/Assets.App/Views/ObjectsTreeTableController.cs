//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Popups;
using Epsitec.Cresus.Assets.App.Widgets;
using Epsitec.Cresus.Assets.Server.NaiveEngine;

namespace Epsitec.Cresus.Assets.App.Views
{
	public class ObjectsTreeTableController
	{
		public ObjectsTreeTableController(DataAccessor accessor)
		{
			this.accessor = accessor;
			this.timestamp = new Timestamp (System.DateTime.MaxValue, 0);
		}

		public void CreateUI(Widget parent)
		{
			this.topTitle = new TopTitle
			{
				Parent = parent,
			};

			this.topTitle.SetTitle ("Objets d'immobilisation");

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

		public Guid								SelectedGuid
		{
			get
			{
				return this.accessor.GetObjectGuid (this.selectedRow);
			}
			set
			{
				int count = this.accessor.ObjectsCount;
				for (int i=0; i<count; i++)
				{
					var guid = this.accessor.GetObjectGuid (i);
					if (guid == value)
					{
						this.SelectedRow = i;
						return;
					}
				}

				this.SelectedRow = -1;
			}
		}

		public Timestamp						Timestamp
		{
			get
			{
				return this.timestamp;
			}
			set
			{
				if (this.timestamp != value)
				{
					this.timestamp = value;

					this.UpdateController ();
					this.UpdateToolbar ();
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

		private void OnNew()
		{
		}

		private void OnDelete()
		{
			var target = this.toolbar.GetCommandWidget (ToolbarCommand.Delete);

			if (target != null)
			{
				var popup = new DeletePopup
				{
					Question = "Voulez-vous supprimer l'objet sélectionné ?",
				};

				popup.Create (target);

				popup.ButtonClicked += delegate (object sender, string name)
				{
					if (name == "yes")
					{
					}
				};
			}
		}

		private void OnDeselect()
		{
			this.SelectedRow = -1;
		}


		private void CreateTreeTable(Widget parent)
		{
			int rowsCount = this.accessor.ObjectsCount;
			this.selectedRow = -1;

			this.controller = new NavigationTreeTableController
			{
				RowsCount = rowsCount,
			};

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

		private TreeTableColumnDescription[] TreeTableColumns
		{
			get
			{
				var list = new List<TreeTableColumnDescription> ();

				list.Add (new TreeTableColumnDescription (TreeTableColumnType.Tree,           180, "Objet"));
				list.Add (new TreeTableColumnDescription (TreeTableColumnType.String,          50, "N°"));
				list.Add (new TreeTableColumnDescription (TreeTableColumnType.String,         120, "Responsable"));
				list.Add (new TreeTableColumnDescription (TreeTableColumnType.String,          60, "Couleur"));
				list.Add (new TreeTableColumnDescription (TreeTableColumnType.String,         200, "Numéro de série"));
				list.Add (new TreeTableColumnDescription (TreeTableColumnType.ComputedAmount, 120, "Valeur comptable"));
				list.Add (new TreeTableColumnDescription (TreeTableColumnType.ComputedAmount, 120, "Valeur assurance"));

				return list.ToArray ();
			}
		}

		private void UpdateController(bool crop = true)
		{
			int visibleCount = this.controller.VisibleRowsCount;
			int rowsCount    = this.accessor.ObjectsCount;
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

			var cf = new List<TreeTableCellTree> ();
			var c1 = new List<TreeTableCellString> ();
			var c2 = new List<TreeTableCellString> ();
			var c3 = new List<TreeTableCellString> ();
			var c4 = new List<TreeTableCellString> ();
			var c5 = new List<TreeTableCellComputedAmount> ();
			var c6 = new List<TreeTableCellComputedAmount> ();

			for (int i=0; i<count; i++)
			{
				if (firstRow+i >= this.accessor.ObjectsCount)
				{
					break;
				}

				var guid = this.accessor.GetObjectGuid (firstRow+i);
				var properties = this.accessor.GetObjectSyntheticProperties (guid, this.timestamp);

				int level = DataAccessor.GetIntProperty (properties, (int) ObjectField.Level).GetValueOrDefault (-1);

				var type = TreeTableTreeType.Extended;

				if (level == -1)
				{
					type = TreeTableTreeType.None;
				}
				else if (level == 3)
				{
					type = TreeTableTreeType.Final;
				}

				var nom         = DataAccessor.GetStringProperty (properties, (int) ObjectField.Nom);
				var numéro      = DataAccessor.GetStringProperty (properties, (int) ObjectField.Numéro);
				var responsable = DataAccessor.GetStringProperty (properties, (int) ObjectField.Responsable);
				var couleur     = DataAccessor.GetStringProperty (properties, (int) ObjectField.Couleur);
				var série       = DataAccessor.GetStringProperty (properties, (int) ObjectField.NuméroSérie);
				var valeur1     = DataAccessor.GetComputedAmountProperty (properties, (int) ObjectField.Valeur1);
				var valeur2     = DataAccessor.GetComputedAmountProperty (properties, (int) ObjectField.Valeur2);

				var sf = new TreeTableCellTree (true, level, type, nom, isSelected: (i == selection));
				var s1 = new TreeTableCellString (true, numéro, isSelected: (i == selection));
				var s2 = new TreeTableCellString (true, responsable, isSelected: (i == selection));
				var s3 = new TreeTableCellString (true, couleur, isSelected: (i == selection));
				var s4 = new TreeTableCellString (true, série, isSelected: (i == selection));
				var s5 = new TreeTableCellComputedAmount (true, valeur1, isSelected: (i == selection));
				var s6 = new TreeTableCellComputedAmount (true, valeur2, isSelected: (i == selection));

				cf.Add (sf);
				c1.Add (s1);
				c2.Add (s2);
				c3.Add (s3);
				c4.Add (s4);
				c5.Add (s5);
				c6.Add (s6);
			}

			this.controller.SetColumnCells (0, cf.ToArray ());
			this.controller.SetColumnCells (1, c1.ToArray ());
			this.controller.SetColumnCells (2, c2.ToArray ());
			this.controller.SetColumnCells (3, c3.ToArray ());
			this.controller.SetColumnCells (4, c4.ToArray ());
			this.controller.SetColumnCells (5, c5.ToArray ());
			this.controller.SetColumnCells (6, c6.ToArray ());
		}

		private void UpdateToolbar()
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


		private readonly DataAccessor			accessor;

		private TopTitle						topTitle;
		private TreeTableToolbar				toolbar;
		private NavigationTreeTableController	controller;
		private int								selectedRow;
		private Timestamp						timestamp;
	}
}
