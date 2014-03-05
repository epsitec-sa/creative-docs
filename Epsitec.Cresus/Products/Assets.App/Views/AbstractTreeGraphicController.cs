//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Widgets;
using Epsitec.Cresus.Assets.App.Helpers;
using Epsitec.Cresus.Assets.Server.BusinessLogic;
using Epsitec.Cresus.Assets.Server.Helpers;
using Epsitec.Cresus.Assets.Server.NodeGetters;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Views
{
	/// <summary>
	/// Représente graphiquement le contenu d'un TreeTable, sous la forme d'une large zone
	/// qu'on peut scroller horizontalement.
	/// Le graphique est constitué de widgets TreeGraphicViewTile. Grace au mode DockStyle.Left,
	/// il a été très simple de réaliser cette vue.
	/// </summary>
	public abstract class AbstractTreeGraphicController<T>
		where T : struct
	{
		public AbstractTreeGraphicController(DataAccessor accessor, BaseType baseType, AbstractToolbarTreeController<T> treeTableController)
		{
			this.accessor            = accessor;
			this.baseType            = baseType;
			this.treeTableController = treeTableController;
		}


		public Guid								SelectedGuid
		{
			get
			{
				return this.treeTableController.SelectedGuid;
			}
			set
			{
				this.treeTableController.SelectedGuid = value;
				this.UpdateController ();
			}
		}


		public void CreateUI(Widget parent)
		{
			this.scrollable = new Scrollable
			{
				Parent                 = parent,
				HorizontalScrollerMode = ScrollableScrollerMode.ShowAlways,
				VerticalScrollerMode   = ScrollableScrollerMode.HideAlways,
				Dock                   = DockStyle.Fill,
				Margins                = new Margins (0, 0, 0, 0),
			};

			this.scrollable.Viewport.IsAutoFitting = true;
		}

		public virtual void CompactOrExpand(Guid guid)
		{
		}

		public virtual void SetParams(Timestamp? timestamp, Guid rootGuid, SortingInstructions instructions)
		{
		}

		public virtual void UpdateData()
		{
		}

		public virtual void UpdateController(bool crop = true)
		{
			this.UpdateSelection (crop);
		}


		private void UpdateSelection(bool crop)
		{
			this.UpdateSelection (this.scrollable.Viewport, crop);
		}

		private void UpdateSelection(Widget parent, bool crop)
		{
			var name = this.SelectedGuid.ToString ();

			foreach (Widget tile in parent.Children)
			{
				if (tile.Name == name)
				{
					tile.ActiveState = ActiveState.Yes;
				}
				else
				{
					tile.ActiveState = ActiveState.No;
				}

				this.UpdateSelection (tile, crop);
			}
		}


		protected Widget CreateNode(Widget parent, Guid guid, int level, NodeType nodeType, string[] texts, double[] fontFactors)
		{
			var tile = new TreeGraphicTile (level, this.treeGraphicViewState.ColumnWidth, nodeType, this.treeGraphicViewMode)
			{
				Parent           = parent,
				Name             = guid.ToString (),
				ContentAlignment = ContentAlignment.TopLeft,
				Dock             = DockStyle.Left,
			};

			tile.SetContent (texts, fontFactors);

			ToolTip.Default.SetToolTip (tile, string.Join (" ", texts));

			tile.Clicked += delegate
			{
				this.SelectedGuid = guid;
			};

			tile.DoubleClicked += delegate
			{
				this.SelectedGuid = guid;
				this.OnTileDoubleClicked (guid);
			};

			tile.TreeButtonClicked += delegate
			{
				this.CompactOrExpand (guid);
			};

			return tile;
		}

		protected virtual string[] GetTexts(BaseType baseType, Guid guid, ObjectField[] fields)
		{
			var obj = this.accessor.GetObject (baseType, guid);
			return this.GetTexts (obj, fields);
		}

		private string[] GetTexts(DataObject obj, ObjectField[] fields)
		{
			var list = new List<string> ();

			foreach (var field in fields)
			{
				var type = this.accessor.GetFieldType (field);
				string text = null;

				switch (type)
				{
					case FieldType.String:
						text = ObjectProperties.GetObjectPropertyString (obj, null, field);
						break;

					case FieldType.AmortizedAmount:
						var aa = ObjectProperties.GetObjectPropertyAmortizedAmount(obj, null, field);
						if (aa != null)
						{
							text = TypeConverters.AmountToString (aa.Value.FinalAmortizedAmount);
						}
						break;

					case FieldType.ComputedAmount:
						var ca = ObjectProperties.GetObjectPropertyComputedAmount (obj, null, field);
						if (ca != null)
						{
							text = TypeConverters.AmountToString (ca.Value.FinalAmount);
						}
						break;

					default:
						throw new System.InvalidOperationException (string.Format ("Unsupported FieldType {0}", type.ToString ()));
				}

				list.Add (text);
			}

			return list.ToArray ();
		}

		protected ObjectField[] GetFieds()
		{
			var list = new List<ObjectField> ();

			foreach (var field in this.treeGraphicViewState.Fields)
			{
				list.Add (field);
			}

			return list.ToArray ();
		}

		protected double[] GetFontFactors()
		{
			var list = new List<double> ();

			foreach (var fontFactor in this.treeGraphicViewState.FontFactors)
			{
				list.Add (fontFactor);
			}

			return list.ToArray ();
		}


		#region Events handler
		protected void OnTileDoubleClicked(Guid guid)
		{
			this.TileDoubleClicked.Raise (this, guid);
		}

		public event EventHandler<Guid> TileDoubleClicked;
		#endregion


		protected readonly DataAccessor			accessor;
		protected readonly BaseType				baseType;
		protected readonly AbstractToolbarTreeController<T> treeTableController;

		protected AbstractNodeGetter<T>			nodeGetter;
		protected Scrollable					scrollable;
		protected TreeGraphicMode			treeGraphicViewMode;
		protected TreeGraphicState			treeGraphicViewState;
	}
}
