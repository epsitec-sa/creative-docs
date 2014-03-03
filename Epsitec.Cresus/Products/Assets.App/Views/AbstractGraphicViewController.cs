﻿//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
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
	public abstract class AbstractGraphicViewController<T>
		where T : struct
	{
		public AbstractGraphicViewController(DataAccessor accessor, BaseType baseType)
		{
			this.accessor = accessor;
			this.baseType = baseType;

			this.selectedGuid = Guid.Empty;
		}


		public GraphicViewMode					GraphicViewMode
		{
			get
			{
				return this.graphicViewMode;
			}
			set
			{
				this.graphicViewMode = value;
			}
		}

		public Guid								SelectedGuid
		{
			get
			{
				return this.selectedGuid;
			}
			set
			{
				this.selectedGuid = value;
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
			var name = this.selectedGuid.ToString ();

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
			var w = new GraphicViewTile (level, this.graphicViewState.ColumnWidth, nodeType, this.graphicViewMode)
			{
				Parent           = parent,
				Name             = guid.ToString (),
				ContentAlignment = ContentAlignment.TopLeft,
				Dock             = DockStyle.Left,
			};

			w.SetContent (texts, fontFactors);

			ToolTip.Default.SetToolTip (w, string.Join (" ", texts));

			w.Clicked += delegate
			{
				this.selectedGuid = guid;
				this.OnSelectedTileChanged (this.selectedGuid);
			};

			w.DoubleClicked += delegate
			{
				this.selectedGuid = guid;
				this.OnTileDoubleClicked (this.selectedGuid);
			};

			return w;
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

			foreach (var field in this.graphicViewState.Fields)
			{
				list.Add (field);
			}

			return list.ToArray ();
		}

		protected double[] GetFontFactors()
		{
			var list = new List<double> ();

			foreach (var fontFactor in this.graphicViewState.FontFactors)
			{
				list.Add (fontFactor);
			}

			return list.ToArray ();
		}


		#region Events handler
		protected void OnSelectedTileChanged(Guid guid)
		{
			this.SelectedTileChanged.Raise (this, guid);
		}

		public event EventHandler<Guid> SelectedTileChanged;


		protected void OnTileDoubleClicked(Guid guid)
		{
			this.TileDoubleClicked.Raise (this, guid);
		}

		public event EventHandler<Guid> TileDoubleClicked;
		#endregion


		protected readonly DataAccessor			accessor;
		protected readonly BaseType				baseType;

		protected AbstractNodeGetter<T>			nodeGetter;
		protected Scrollable					scrollable;
		protected GraphicViewMode				graphicViewMode;
		protected GraphicViewState				graphicViewState;
		protected Guid							selectedGuid;
	}
}
