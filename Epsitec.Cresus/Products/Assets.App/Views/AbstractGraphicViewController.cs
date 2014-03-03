//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Widgets;
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

		public virtual void Update()
		{
		}


		protected Widget CreateNode(Widget parent, int level, NodeType nodeType, string[] texts, double[] fontFactors)
		{
			var w = new GraphicViewTile (level, this.graphicViewState.ColumnWidth, nodeType, this.graphicViewMode)
			{
				Parent           = parent,
				ContentAlignment = ContentAlignment.TopLeft,
				Dock             = DockStyle.Left,
			};

			w.SetContent (texts, fontFactors);

			ToolTip.Default.SetToolTip (w, string.Join (" ", texts));

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


		protected readonly DataAccessor			accessor;
		protected readonly BaseType				baseType;

		protected AbstractNodeGetter<T>			nodeGetter;
		protected GraphicViewMode				graphicViewMode;
		protected GraphicViewState				graphicViewState;
		protected Scrollable					scrollable;
	}
}
