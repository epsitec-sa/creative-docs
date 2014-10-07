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
using Epsitec.Cresus.Assets.Server.BusinessLogic;
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
					this.UpdateController ();
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

		public virtual bool						IsEmpty
		{
			get
			{
				return this.nodeGetter.Count == 0;
			}
		}

		public virtual bool						HelplineDesired
		{
			get
			{
				return this.IsEmpty;
			}
		}

		public bool								HelplineVisibility
		{
			get
			{
				return this.toolbar.ShowHelpline;
			}
			set
			{
				this.toolbar.ShowHelpline = value;
				this.controllerFrame.Visibility = !this.IsEmpty;
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

			this.CreateToolbar ();
			this.toolbar.CreateUI (common);

			this.CreateControllerUI (this.controllerFrame);

		}

		protected abstract void CreateToolbar();


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
			bool enable = (!this.IsEmpty && newSelection.HasValue && selectedCell != newSelection.Value);
			this.toolbar.SetEnable (command, enable);
		}

		protected void UpdateMoveCommand(Command command, int selectedCell, int? newSelection)
		{
			bool enable = (!this.IsEmpty && newSelection.HasValue && selectedCell != -1 && selectedCell != newSelection.Value);
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
