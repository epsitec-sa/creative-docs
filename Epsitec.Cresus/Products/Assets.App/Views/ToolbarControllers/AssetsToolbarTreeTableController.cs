//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Helpers;
using Epsitec.Cresus.Assets.App.Popups;
using Epsitec.Cresus.Assets.App.Views.CommandToolbars;
using Epsitec.Cresus.Assets.App.Views.TreeGraphicControllers;
using Epsitec.Cresus.Assets.Core.Helpers;
using Epsitec.Cresus.Assets.Data;
using Epsitec.Cresus.Assets.Server.BusinessLogic;
using Epsitec.Cresus.Assets.Server.DataFillers;
using Epsitec.Cresus.Assets.Server.NodeGetters;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Views.ToolbarControllers
{
	public class AssetsToolbarTreeTableController : AbstractToolbarBothTreesController<SortableCumulNode>, IDirty
	{
		public AssetsToolbarTreeTableController(DataAccessor accessor, BaseType baseType)
			: base (accessor, baseType)
		{
			this.hasGraphic        = true;
			this.hasFilter         = true;
			this.hasTreeOperations = true;
			this.hasMoveOperations = false;

			this.title = AbstractView.GetViewTitle (this.accessor, ViewType.Assets);

			//	GuidNode -> ParentPositionNode -> LevelNode -> TreeNode -> SortableCumulNode
			var groupNodeGetter  = this.accessor.GetNodeGetter (BaseType.Groups);
			var objectNodeGetter = this.accessor.GetNodeGetter (BaseType.Assets);
			this.nodeGetter = new ObjectsNodeGetter (this.accessor, groupNodeGetter, objectNodeGetter);
		}


		#region IDirty Members
		public bool InUse
		{
			get;
			set;
		}

		public bool DirtyData
		{
			get;
			set;
		}
		#endregion


		public Guid								FilterGuid
		{
			get
			{
				return this.rootGuid;
			}
			set
			{
				if (this.rootGuid != value)
				{
					this.rootGuid = value;
					this.UpdateData ();
				}
			}
		}


		protected override void CreateControllerUI(Widget parent)
		{
			base.CreateControllerUI (parent);

			this.bottomFrame.Visibility = true;
			this.CreateStateAt (this.bottomFrame);
		}

		private void CreateStateAt(Widget parent)
		{
			this.stateAtController = new StateAtController (this.accessor);
			var frame = this.stateAtController.CreateUI (parent);
			frame.Dock = DockStyle.Left;

			this.stateAtController.DateChanged += delegate
			{
				this.SetDate (this.stateAtController.Date);
			};

			this.SetDate (Timestamp.Now.Date);
		}

		protected override void CreateGraphicControllerUI()
		{
			this.graphicController = new AssetsTreeGraphicController (this.accessor, this.baseType);
		}


		public override void UpdateData()
		{
			this.NodeGetter.SetParams (this.timestamp, this.rootGuid, this.sortingInstructions);
			this.dataFiller.Timestamp = this.timestamp;

			this.UpdateController ();
			this.UpdateToolbar ();
		}


		protected override int					VisibleSelectedRow
		{
			get
			{
				return this.NodeGetter.AllToVisible (this.selectedRow);
			}
			set
			{
				this.SelectedRow = this.NodeGetter.VisibleToAll (value);
			}
		}

		public Timestamp?						SelectedTimestamp
		{
			get
			{
				return this.selectedTimestamp;
			}
			set
			{
				this.selectedTimestamp = value;
			}
		}

		public override Guid					SelectedGuid
		{
			//	Retourne le Guid de l'objet actuellement sélectionné.
			get
			{
				int sel = this.VisibleSelectedRow;
				if (sel != -1 && sel < this.nodeGetter.Count)
				{
					return this.nodeGetter[sel].Guid;
				}
				else
				{
					return Guid.Empty;
				}
			}
			//	Sélectionne l'objet ayant un Guid donné. Si la ligne correspondante
			//	est cachée, on est assez malin pour sélectionner la prochaine ligne
			//	visible, vers le haut.
			set
			{
				this.VisibleSelectedRow = this.NodeGetter.SearchBestIndex (value);
			}
		}


		private void SetDate(System.DateTime? date)
		{
			//	Choix du timestamp visible dans tout le TreeTable.
			this.timestamp = null;

			if (date.HasValue)
			{
				this.timestamp = new Timestamp (date.Value, 0);
			}

			this.stateAtController.Date = date;

			using (new SaveSelectedGuid (this))
			{
				this.UpdateData ();
			}
		}


		protected override void AdaptToolbarCommand()
		{
			this.toolbar.SetCommandDescription (ToolbarCommand.New,      "TreeTable.New.Asset",   Res.Strings.ToolbarControllers.AssetsTreeTable.New.ToString ());
			this.toolbar.SetCommandDescription (ToolbarCommand.Delete,   "TreeTable.Delete",      Res.Strings.ToolbarControllers.AssetsTreeTable.Delete.ToString ());
			this.toolbar.SetCommandDescription (ToolbarCommand.Deselect, null,                    Res.Strings.ToolbarControllers.AssetsTreeTable.Deselect.ToString ());
			this.toolbar.SetCommandDescription (ToolbarCommand.Copy,     "TreeTable.Copy.Asset",  Res.Strings.ToolbarControllers.AssetsTreeTable.Copy.ToString ());
			this.toolbar.SetCommandDescription (ToolbarCommand.Paste,    "TreeTable.Paste.Asset", Res.Strings.ToolbarControllers.AssetsTreeTable.Paste.ToString ());
			this.toolbar.SetCommandDescription (ToolbarCommand.Export,   null,                    Res.Strings.ToolbarControllers.AssetsTreeTable.Export.ToString ());
			this.toolbar.SetCommandDescription (ToolbarCommand.Import,   CommandDescription.Empty);
			this.toolbar.SetCommandDescription (ToolbarCommand.Goto,     CommandDescription.Empty);
		}

		protected override void CreateNodeFiller()
		{
			this.dataFiller = new AssetsTreeTableFiller (this.accessor, this.nodeGetter);
			this.UpdateFillerTitle ();

			TreeTableFiller<SortableCumulNode>.FillColumns (this.treeTableController, this.dataFiller, "View.Assets");

			this.sortingInstructions = TreeTableFiller<SortableCumulNode>.GetSortingInstructions (this.treeTableController);
		}

		protected override void UpdateFillerTitle()
		{
			this.dataFiller.Title = this.FullTitle;
		}

		private string FullTitle
		{
			get
			{
				var builder = new System.Text.StringBuilder ();
				builder.Append(this.title);

#if false
				if (!this.SelectedGuid.IsEmpty)
				{
					builder.Append (" — ");

					if (this.stateAtController.Date.HasValue)
					{
						var timestamp = new Timestamp (this.stateAtController.Date.Value, 0);
						var name = AssetsLogic.GetSummary (this.accessor, this.SelectedGuid, timestamp);
						builder.Append (name);
					}
					else
					{
						var name = AssetsLogic.GetSummary (this.accessor, this.SelectedGuid);
						builder.Append (name);
					}
				}
#endif

				if (this.stateAtController != null && this.stateAtController.Date.HasValue)
				{
					builder.Append (" — ");
					builder.Append (TypeConverters.DateToString (this.stateAtController.Date));
				}

				return builder.ToString ();
			}
		}


		protected override void OnFilter()
		{
			var target = this.toolbar.GetTarget (ToolbarCommand.Filter);
			var popup = new FilterPopup (this.accessor, this.rootGuid);

			popup.Create (target, leftOrRight: true);

			popup.Navigate += delegate (object sender, Guid guid)
			{
				this.rootGuid = guid;

				using (new SaveSelectedGuid (this))
				{
					this.UpdateData ();
				}
			};
		}

		protected override void OnDeselect()
		{
			this.VisibleSelectedRow = -1;
		}

		protected override void OnNew()
		{
			var target = this.toolbar.GetTarget (ToolbarCommand.New);
			this.ShowCreatePopup (target);
		}

		protected override void OnDelete()
		{
			var target = this.toolbar.GetTarget (ToolbarCommand.Delete);

			YesNoPopup.Show (target, Res.Strings.ToolbarControllers.AssetsTreeTable.DeleteQuestion.ToString (), delegate
			{
				this.accessor.RemoveObject (BaseType.Assets, this.SelectedGuid);
				this.UpdateData ();
				this.OnUpdateAfterDelete ();
			});
		}

		protected override void OnCopy()
		{
			//	Copier un objet d'immobilisation requiert un popup pour choisir la date à considérer.
			var target = this.toolbar.GetTarget (ToolbarCommand.Copy);
			var obj = this.accessor.GetObject (this.baseType, this.SelectedGuid);

			AssetCopyPopup.Show (target, this.accessor, obj, delegate (System.DateTime date)
			{
				var timestamp = new Timestamp (date, 0);
				this.accessor.Clipboard.CopyObject (this.accessor, this.baseType, obj, timestamp);
				this.UpdateToolbar ();
			});
		}

		protected override void OnPaste()
		{
			//	Coller un objet d'immobilisation requiert un popup pour choisir la date d'entrée.
			var target = this.toolbar.GetTarget (ToolbarCommand.Paste);
			var summary = this.accessor.Clipboard.GetObjectSummary (this.baseType);

			AssetPastePopup.Show (target, this.accessor, summary, delegate (System.DateTime inputDate)
			{
				var obj = this.accessor.Clipboard.PasteObject (this.accessor, this.baseType, inputDate);
				if (obj == null)
				{
					MessagePopup.ShowPasteError (target);
				}
				else
				{
					this.UpdateData ();
					this.SelectedGuid = obj.Guid;
					this.OnUpdateAfterCreate (obj.Guid, EventType.Input, new Timestamp (inputDate, 0));
				}
			});
		}


		private void ShowCreatePopup(Widget target)
		{
			CreateAssetPopup.Show (target, this.accessor, delegate (System.DateTime date, string name, decimal? value, Guid cat)
			{
				this.CreateAsset (date, name, value, cat);
			});
		}

		private void CreateAsset(System.DateTime date, string name, decimal? value, Guid cat)
		{
			var asset = AssetsLogic.CreateAsset (this.accessor, date, name, value, cat);
			var guid = asset.Guid;

			this.UpdateData ();

			this.SelectedGuid = guid;
			this.SelectedTimestamp = AssetCalculator.GetLastTimestamp (asset);
			
			this.OnUpdateAfterCreate (guid, EventType.Input, this.selectedTimestamp.GetValueOrDefault ());
		}


		protected override void UpdateToolbar()
		{
			base.UpdateToolbar ();

			this.toolbar.SetCommandActivate (ToolbarCommand.Filter, !this.rootGuid.IsEmpty);

			bool compactEnable = !this.NodeGetter.IsAllCompacted;
			bool expandEnable  = !this.NodeGetter.IsAllExpanded;

			this.toolbar.SetCommandEnable (ToolbarCommand.CompactAll, compactEnable);
			this.toolbar.SetCommandEnable (ToolbarCommand.CompactOne, compactEnable);
			this.toolbar.SetCommandEnable (ToolbarCommand.ExpandOne,  expandEnable);
			this.toolbar.SetCommandEnable (ToolbarCommand.ExpandAll,  expandEnable);
		}

		protected override bool IsCopyEnable
		{
			get
			{
				var obj = this.accessor.GetObject (this.baseType, this.SelectedGuid);
				return obj != null;
			}
		}


		private ObjectsNodeGetter NodeGetter
		{
			get
			{
				return this.nodeGetter as ObjectsNodeGetter;
			}
		}


		private StateAtController					stateAtController;
		private Timestamp?							selectedTimestamp;
		private Timestamp?							timestamp;
		private Guid								rootGuid;
	}
}
