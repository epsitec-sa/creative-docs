//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Helpers;
using Epsitec.Cresus.Assets.App.Widgets;
using Epsitec.Cresus.Assets.Server.DataFillers;
using Epsitec.Cresus.Assets.Server.NodesGetter;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Views
{
	public class GuidRatioFieldsController : AbstractFieldController
	{
		public GuidRatioFieldsController(DataAccessor accessor)
		{
			this.accessor = accessor;

			this.treeTable = new NavigationTreeTableController ();

			this.editor = new GuidRatioFieldController
			{
				Accessor   = this.accessor,
				LabelWidth = 10,
				EditWidth  = 450,
			};

			this.nodesGetter = new GuidRatioEditedNodesGetter (this.accessor);
			this.dataFiller  = new GuidRatioEditedTreeTableFiller (this.accessor, this.nodesGetter);
		}


		public override void CreateUI(Widget parent)
		{
			var topBox = new FrameBox
			{
				Parent    = parent,
				Dock      = DockStyle.Fill,
				Padding   = new Margins (2),
				BackColor = ColorManager.WindowBackgroundColor,
			};

			var bottomBox = new FrameBox
			{
				Parent    = parent,
				Dock      = DockStyle.Bottom,
				Margins   = new Margins (0, 0, 2, 0),
				BackColor = ColorManager.WindowBackgroundColor,
			};

			this.treeTable.CreateUI (topBox, footerHeight: 0);
			this.treeTable.AllowsMovement = false;

			TreeTableFiller<ObjectField>.FillColumns (this.treeTable, this.dataFiller, dockToLeftCount: 0);
			this.UpdateTreeTable ();

			this.editor.CreateUI (bottomBox);

			//	Connexion des événements.
			this.treeTable.RowClicked += delegate (object sender, int row)
			{
				this.SelectedRow = this.treeTable.TopVisibleRow + row;
				this.UpdateTreeTable ();
				this.UpdateEditor ();
			};

			this.treeTable.ContentChanged += delegate (object sender, bool crop)
			{
				this.UpdateTreeTable (crop);
			};

			this.editor.ValueEdited += delegate (object sender, ObjectField of)
			{
				this.accessor.EditionAccessor.SetField (of, this.editor.Value);

				this.editor.Value         = this.accessor.EditionAccessor.GetFieldGuidRatio (of);
				this.editor.PropertyState = this.accessor.EditionAccessor.GetEditionPropertyState (of);

				this.UpdateTreeTable ();

				this.OnValueEdited (of);
			};

			this.editor.ShowHistory += delegate (object sender, Widget target, ObjectField of)
			{
				this.OnShowHistory (target, of);
			};
		}


		public void Update()
		{
			this.nodesGetter.SetParams ();
			this.selectedField = this.nodesGetter.Nodes.Last ();
			this.UpdateTreeTable ();
			this.UpdateEditor ();
		}


		private void UpdateTreeTable(bool crop = true)
		{
			this.nodesGetter.SetParams ();
			TreeTableFiller<ObjectField>.FillContent (this.treeTable, this.dataFiller, this.SelectedRow, crop);
		}

		private void UpdateEditor()
		{
			if (this.selectedField != ObjectField.Unknown)
			{
				this.editor.Field         = this.selectedField;
				this.editor.Value         = this.accessor.EditionAccessor.GetFieldGuidRatio (this.selectedField);
				this.editor.PropertyState = this.accessor.EditionAccessor.GetEditionPropertyState (this.selectedField);
			}
		}


		private int SelectedRow
		{
			get
			{
				int count = this.nodesGetter.Count;
				for (int row=0; row<count; row++)
				{
					var field = this.nodesGetter[row];
					if (field == this.selectedField)
					{
						return row;
					}
				}

				return -1;
			}
			set
			{
				int count = this.nodesGetter.Count;
				if (value >= 0 && value < count)
				{
					this.selectedField = this.nodesGetter[value];
				}
			}
		}


		private readonly DataAccessor					accessor;
		private readonly NavigationTreeTableController	treeTable;
		private readonly GuidRatioFieldController		editor;
		private readonly GuidRatioEditedNodesGetter		nodesGetter;
		private readonly GuidRatioEditedTreeTableFiller	dataFiller;


		private ObjectField								selectedField;
	}
}
