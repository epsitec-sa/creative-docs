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
	/// Le graphique est constitué de widgets TreeGraphicTile. Grace au mode DockStyle.Left,
	/// il a été très simple de réaliser cette vue.
	/// </summary>
	public abstract class AbstractTreeGraphicController<T>
		where T : struct
	{
		public AbstractTreeGraphicController(DataAccessor accessor, BaseType baseType)
		{
			this.accessor = accessor;
			this.baseType = baseType;
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

		public virtual void UpdateController(AbstractNodeGetter<T> nodeGetter, Guid selectedGuid, bool crop = true)
		{
		}


		protected void UpdateSelection(Guid selectedGuid, bool crop)
		{
			this.UpdateSelection (this.scrollable.Viewport, selectedGuid, crop);
		}

		private void UpdateSelection(Widget parent, Guid selectedGuid, bool crop)
		{
			var name = selectedGuid.ToString ();

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

				this.UpdateSelection (tile, selectedGuid, crop);
			}
		}


		protected Widget CreateTile(Widget parent, Guid guid, int level, double fontSize, NodeType nodeType, TreeGraphicValue[] values, double[] fontFactors)
		{
			var tile = new TreeGraphicTile (level, fontSize, this.treeGraphicViewState.ColumnWidth, nodeType, this.treeGraphicViewMode)
			{
				Parent           = parent,
				Name             = guid.ToString (),
				ContentAlignment = ContentAlignment.TopLeft,
				Dock             = DockStyle.Left,
				MinAmount        = this.minAmount,
				MaxAmount        = this.maxAmount,
			};

			tile.SetContent (values, fontFactors);

			ToolTip.Default.SetToolTip (tile, string.Join (" ", values.Select (x => x.Text)));

			tile.Clicked += delegate
			{
				this.OnTileClicked (guid);
			};

			tile.DoubleClicked += delegate
			{
				this.OnTileDoubleClicked (guid);
			};

			tile.TreeButtonClicked += delegate
			{
				this.OnTileCompactOrExpand (guid);
			};

			return tile;
		}

		protected virtual TreeGraphicValue[] GetValues(BaseType baseType, Guid guid, ObjectField[] fields)
		{
			var obj = this.accessor.GetObject (baseType, guid);
			return this.GetValues (obj, fields);
		}

		private TreeGraphicValue[] GetValues(DataObject obj, ObjectField[] fields)
		{
			var list = new List<TreeGraphicValue> ();

			foreach (var field in fields)
			{
				var type = this.accessor.GetFieldType (field);
				var value = TreeGraphicValue.Empty;

				switch (type)
				{
					case FieldType.String:
						var text = ObjectProperties.GetObjectPropertyString (obj, null, field);
						value = TreeGraphicValue.CreateText (text);
						break;

					case FieldType.AmortizedAmount:
						var aa = ObjectProperties.GetObjectPropertyAmortizedAmount(obj, null, field);
						if (aa != null)
						{
							value = TreeGraphicValue.CreateAmount (aa.Value.FinalAmortizedAmount);
						}
						break;

					case FieldType.ComputedAmount:
						var ca = ObjectProperties.GetObjectPropertyComputedAmount (obj, null, field);
						if (ca != null)
						{
							value = TreeGraphicValue.CreateAmount (ca.Value.FinalAmount);
						}
						break;

					default:
						throw new System.InvalidOperationException (string.Format ("Unsupported FieldType {0}", type.ToString ()));
				}

				list.Add (value);
			}

			return list.ToArray ();
		}

		protected ObjectField[] GetFieds()
		{
			return this.treeGraphicViewState.Fields.ToArray ();
		}

		protected double[] GetFontFactors()
		{
			return this.treeGraphicViewState.FontFactors.ToArray ();
		}

		protected static double GetFontSize(int deep, int level)
		{
			return 10.0 + 2.0 * (deep-level);
		}


		#region Events handler
		protected void OnTileClicked(Guid guid)
		{
			this.TileClicked.Raise (this, guid);
		}

		public event EventHandler<Guid> TileClicked;


		protected void OnTileDoubleClicked(Guid guid)
		{
			this.TileDoubleClicked.Raise (this, guid);
		}

		public event EventHandler<Guid> TileDoubleClicked;


		protected void OnTileCompactOrExpand(Guid guid)
		{
			this.TileCompactOrExpand.Raise (this, guid);
		}

		public event EventHandler<Guid> TileCompactOrExpand;
		#endregion


		protected readonly DataAccessor			accessor;
		protected readonly BaseType				baseType;

		protected Scrollable					scrollable;
		protected TreeGraphicMode				treeGraphicViewMode;
		protected TreeGraphicState				treeGraphicViewState;
		protected decimal						minAmount;
		protected decimal						maxAmount;
	}
}
