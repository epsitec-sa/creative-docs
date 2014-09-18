//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Support;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Helpers;
using Epsitec.Cresus.Assets.App.Popups;
using Epsitec.Cresus.Assets.App.Views.CommandToolbars;
using Epsitec.Cresus.Assets.App.Widgets;
using Epsitec.Cresus.Assets.Data;
using Epsitec.Cresus.Assets.Server.NodeGetters;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Views.ToolbarControllers
{
	/// <summary>
	/// Classe de base pour tous les contrôleurs TreeTable et/ou TreeGraphic.
	/// </summary>
	public abstract class AbstractToolbarTreeController<T> : System.IDisposable
		where T : struct
	{
		public AbstractToolbarTreeController(DataAccessor accessor, CommandContext commandContext, BaseType baseType)
		{
			this.accessor       = accessor;
			this.commandContext = commandContext;
			this.baseType       = baseType;

			this.commandDispatcher = new CommandDispatcher (this.GetType ().FullName, CommandDispatcherLevel.Primary, CommandDispatcherOptions.AutoForwardCommands);
			this.commandDispatcher.RegisterController (this);  // nécesaire pour [Command (Res.CommandIds...)]
		}

		public virtual void Dispose()
		{
			if (this.toolbar != null)
			{
				this.toolbar.Dispose ();
				this.toolbar = null;
			}

			this.commandDispatcher.Dispose ();
		}

		public virtual void Close()
		{
			this.toolbar.Close ();
		}



		public bool								ShowGraphic
		{
			get
			{
				return this.showGraphic;
			}
			set
			{
				if (this.showGraphic != value)
				{
					this.showGraphic = value;
					this.OnUpdateView ();
				}
			}
		}

		public SortingInstructions				SortingInstructions
		{
			get
			{
				return this.sortingInstructions;
			}
			set
			{
				if (this.sortingInstructions != value)
				{
					this.sortingInstructions = value;
					this.UpdateData ();
				}
			}
		}


		public void CreateUI(Widget parent)
		{
			var common = new FrameBox
			{
				Parent = parent,
				Dock   = DockStyle.Fill,
			};

			CommandDispatcher.SetDispatcher (common, this.commandDispatcher);  // nécesaire pour [Command (Res.CommandIds...)]

			this.topTitle = new TopTitle
			{
				Parent = common,
			};

			this.controllerFrame = new FrameBox
			{
				Parent = common,
				Dock   = DockStyle.Fill,
			};

			this.topTitle.SetTitle (this.title);

			//?this.toolbar = new TreeTableToolbar (this.accessor, this.commandContext);
			this.CreateToolbar ();
			//?this.AdaptToolbarCommand();

			this.toolbar.CreateUI (common);

			this.CreateControllerUI (this.controllerFrame);

			//?this.toolbar.CommandClicked += delegate (object sender, ToolbarCommand command)
			//?{
			//?	switch (command)
			//?	{
			//?		case ToolbarCommand.Graphic:
			//?			this.OnGraphic ();
			//?			break;
			//?
			//?		case ToolbarCommand.Filter:
			//?			this.OnFilter ();
			//?			break;
			//?
			//?		case ToolbarCommand.DateRange:
			//?			this.OnDateRange ();
			//?			break;
			//?
			//?		case ToolbarCommand.First:
			//?			this.OnFirst ();
			//?			break;
			//?
			//?		case ToolbarCommand.Last:
			//?			this.OnLast ();
			//?			break;
			//?
			//?		case ToolbarCommand.Prev:
			//?			this.OnPrev ();
			//?			break;
			//?
			//?		case ToolbarCommand.Next:
			//?			this.OnNext ();
			//?			break;
			//?
			//?		case ToolbarCommand.CompactAll:
			//?			this.OnCompactAll ();
			//?			break;
			//?
			//?		case ToolbarCommand.CompactOne:
			//?			this.OnCompactOne ();
			//?			break;
			//?
			//?		case ToolbarCommand.ExpandOne:
			//?			this.OnExpandOne ();
			//?			break;
			//?
			//?		case ToolbarCommand.ExpandAll:
			//?			this.OnExpandAll ();
			//?			break;
			//?
			//?		case ToolbarCommand.MoveTop:
			//?			this.OnMoveTop ();
			//?			break;
			//?
			//?		case ToolbarCommand.MoveUp:
			//?			this.OnMoveUp ();
			//?			break;
			//?
			//?		case ToolbarCommand.MoveDown:
			//?			this.OnMoveDown ();
			//?			break;
			//?
			//?		case ToolbarCommand.MoveBottom:
			//?			this.OnMoveBottom ();
			//?			break;
			//?
			//?		case ToolbarCommand.New:
			//?			this.OnNew ();
			//?			break;
			//?
			//?		case ToolbarCommand.Delete:
			//?			this.OnDelete ();
			//?			break;
			//?
			//?		case ToolbarCommand.Deselect:
			//?			this.OnDeselect ();
			//?			break;
			//?
			//?		case ToolbarCommand.Copy:
			//?			this.OnCopy ();
			//?			break;
			//?
			//?		case ToolbarCommand.Paste:
			//?			this.OnPaste ();
			//?			break;
			//?
			//?		case ToolbarCommand.Export:
			//?			this.OnExport ();
			//?			break;
			//?
			//?		case ToolbarCommand.Import:
			//?			this.OnImport ();
			//?			break;
			//?
			//?		case ToolbarCommand.Goto:
			//?			this.OnGoto ();
			//?			break;
			//?	}
			//?};
		}

		protected abstract void CreateToolbar();

		//?protected virtual void AdaptToolbarCommand()
		//?{
		//?}

		protected virtual void CreateControllerUI(Widget parent)
		{
		}

		protected virtual void CreateNodeFiller()
		{
		}


		public virtual void UpdateData()
		{
		}

		protected virtual void UpdateController(bool crop = true)
		{
		}

		public virtual void UpdateGraphicMode()
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


		protected virtual void OnGraphic()
		{
			this.ShowGraphic = !this.showGraphic;
		}

		protected virtual void OnFirst()
		{
			var index = this.FirstRowIndex;

			if (index.HasValue)
			{
				this.VisibleSelectedRow = index.Value;
				this.SetFocus ();
			}
		}

		protected virtual void OnPrev()
		{
			var index = this.PrevRowIndex;

			if (index.HasValue)
			{
				this.VisibleSelectedRow = index.Value;
				this.SetFocus ();
			}
		}

		protected virtual void OnNext()
		{
			var index = this.NextRowIndex;

			if (index.HasValue)
			{
				this.VisibleSelectedRow = index.Value;
				this.SetFocus ();
			}
		}

		protected virtual void OnLast()
		{
			var index = this.LastRowIndex;

			if (index.HasValue)
			{
				this.VisibleSelectedRow = index.Value;
				this.SetFocus ();
			}
		}

		protected void OnCompactOrExpand(int row)
		{
			//	Etend ou compacte une ligne (inverse son mode actuel).
			using (new SaveSelectedGuid (this))
			{
				this.TreeNodeGetter.CompactOrExpand (row);
				this.UpdateController ();
				this.UpdateToolbar ();
			}
		}

		protected virtual void OnCompactAll()
		{
			//	Compacte toutes les lignes.
			using (new SaveSelectedGuid (this))
			{
				this.TreeNodeGetter.CompactAll ();
				this.UpdateController ();
				this.UpdateToolbar ();
			}
		}

		protected virtual void OnCompactOne()
		{
			//	Compacte une ligne.
			using (new SaveSelectedGuid (this))
			{
				this.TreeNodeGetter.CompactOne ();
				this.UpdateController ();
				this.UpdateToolbar ();
			}
		}

		protected virtual void OnExpandOne()
		{
			//	Etend une ligne.
			using (new SaveSelectedGuid (this))
			{
				this.TreeNodeGetter.ExpandOne ();
				this.UpdateController ();
				this.UpdateToolbar ();
			}
		}

		protected virtual void OnExpandAll()
		{
			//	Etend toutes les lignes.
			using (new SaveSelectedGuid (this))
			{
				this.TreeNodeGetter.ExpandAll ();
				this.UpdateController ();
				this.UpdateToolbar ();
			}
		}

		protected virtual void OnCopy(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			var obj = this.accessor.GetObject(this.baseType, this.SelectedGuid);
			this.accessor.Clipboard.CopyObject (this.accessor, this.baseType, obj);

			this.UpdateToolbar ();
		}

		protected virtual void OnPaste(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			var obj = this.accessor.Clipboard.PasteObject (this.accessor, this.baseType);

			if (obj == null)
			{
				var target = this.toolbar.GetTarget (e);
				MessagePopup.ShowPasteError (target);
			}
			else
			{
				this.UpdateData ();

				this.SelectedGuid = obj.Guid;
				this.OnUpdateAfterCreate (obj.Guid, EventType.Input, Timestamp.Now);  // Timestamp quelconque !
			}
		}

		protected virtual void OnExport(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			var target = this.toolbar.GetTarget (e);
			MessagePopup.ShowTodo (target);
		}


		protected virtual void SetFocus()
		{
		}


		protected virtual void UpdateToolbar()
		{
			//?int row = this.VisibleSelectedRow;
			//?
			//?this.toolbar.SetActiveState (Res.Commands.TreeTable.Graphic, this.showGraphic);
			//?
			//?this.UpdateSelCommand (Res.Commands.TreeTable.First, row, this.FirstRowIndex);
			//?this.UpdateSelCommand (Res.Commands.TreeTable.Prev,  row, this.PrevRowIndex);
			//?this.UpdateSelCommand (Res.Commands.TreeTable.Next,  row, this.NextRowIndex);
			//?this.UpdateSelCommand (Res.Commands.TreeTable.Last,  row, this.LastRowIndex);
			//?
			//?this.UpdateMoveCommand (Res.Commands.TreeTable.MoveTop,    row, this.FirstRowIndex);
			//?this.UpdateMoveCommand (Res.Commands.TreeTable.MoveUp,     row, this.PrevRowIndex);
			//?this.UpdateMoveCommand (Res.Commands.TreeTable.MoveDown,   row, this.NextRowIndex);
			//?this.UpdateMoveCommand (Res.Commands.TreeTable.MoveBottom, row, this.LastRowIndex);
		}

		protected virtual bool IsCopyEnable
		{
			get
			{
				return this.VisibleSelectedRow != -1;
			}
		}

		protected void UpdateSelCommand(Command command, int selectedCell, int? newSelection)
		{
			bool enable = (newSelection.HasValue && selectedCell != newSelection.Value);
			this.toolbar.SetEnable (command, enable);
		}

		protected void UpdateMoveCommand(Command command, int selectedCell, int? newSelection)
		{
			bool enable = (newSelection.HasValue && selectedCell != -1 && selectedCell != newSelection.Value);
			this.toolbar.SetEnable (command, enable);
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
					i = System.Math.Min (i, this.nodeGetter.Count - 1);
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
					i = System.Math.Min (i, this.nodeGetter.Count - 1);
					return i;
				}
			}
		}

		protected int? LastRowIndex
		{
			get
			{
				return this.nodeGetter.Count - 1;
			}
		}


		protected class SaveSelectedGuid : System.IDisposable
		{
			public SaveSelectedGuid(AbstractToolbarTreeController<T> controller)
			{
				this.controller = controller;
				this.currentGuid = this.controller.SelectedGuid;
			}

			public void Dispose()
			{
				this.controller.SelectedGuid = this.currentGuid;
			}

			private readonly AbstractToolbarTreeController<T>	controller;
			private readonly Guid								currentGuid;
		}


		private ITreeFunctions TreeNodeGetter
		{
			get
			{
				return this.nodeGetter as ITreeFunctions;
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


		protected void OnUpdateAfterCreate(Guid guid, EventType eventType, Timestamp timestamp)
		{
			this.UpdateAfterCreate.Raise (this, guid, eventType, timestamp);
		}

		public event EventHandler<Guid, EventType, Timestamp> UpdateAfterCreate;


		protected void OnUpdateAfterDelete()
		{
			this.UpdateAfterDelete.Raise (this);
		}

		public event EventHandler UpdateAfterDelete;


		protected void OnUpdateView()
		{
			this.UpdateView.Raise (this);
		}

		public event EventHandler UpdateView;


		protected void OnChangeView(ViewType viewType)
		{
			this.ChangeView.Raise (this, viewType);
		}

		public event EventHandler<ViewType> ChangeView;
		#endregion


		protected readonly DataAccessor			accessor;
		protected readonly CommandDispatcher	commandDispatcher;
		protected readonly CommandContext		commandContext;
		protected readonly BaseType				baseType;

		protected string						title;

		protected FrameBox						controllerFrame;

		protected INodeGetter<T>				nodeGetter;
		protected TopTitle						topTitle;
		protected AbstractCommandToolbar		toolbar;
		protected SortingInstructions			sortingInstructions;
		protected int							selectedRow;
		protected bool							showGraphic;
	}
}
