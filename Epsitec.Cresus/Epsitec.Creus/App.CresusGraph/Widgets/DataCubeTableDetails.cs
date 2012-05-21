//	Copyright © 2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Drawing;
using Epsitec.Common.UI;
using Epsitec.Common.Support;
using Epsitec.Common.Support.Extensions;
using Epsitec.Common.Types;
using Epsitec.Common.Widgets;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Graph.Widgets
{
	public class DataCubeTableDetails : FrameBox
	{
		public DataCubeTableDetails()
		{
			this.ContainerLayoutMode = ContainerLayoutMode.VerticalFlow;
			this.Padding = new Margins (4, 4, 4, 4);

			this.CreateUI ();
		}

		public DataCubeTableButton Button
		{
			get
			{
				return this.button;
			}
			set
			{
				this.button = value;
				this.Cube = (value == null) ? null : value.Cube;
			}
		}

		public bool IsSourceNameReadOnly
		{
			get
			{
				return this.sourceNameIsReadOnly;
			}
			set
			{
				if (this.sourceNameIsReadOnly != value)
                {
					this.sourceNameIsReadOnly = value;
					this.InvalidateUI ();
                }
			}
		}

        public GraphDataCube Cube
		{
			get
			{
				return this.cube;
			}
			set
			{
				if (this.cube != value)
				{
					this.cube = value;
					this.InvalidateUI ();
				}
			}
		}

		
		public string GetSourceName()
		{
			if (this.button == null)
			{
				return "";
			}
			else
			{
				return this.button.Sources.FirstOrDefault () ?? "";
			}
		}

		
		protected override void PaintBackgroundImplementation(Graphics graphics, Rectangle clipRect)
		{
			var rect = this.Client.Bounds;

			graphics.AddFilledRectangle (rect);
			graphics.RenderSolid (Color.FromBrightness (1));
			
			var adorner = Epsitec.Common.Widgets.Adorners.Factory.Active;
			var border  = adorner.ColorBorder;

//-			graphics.AddFilledRectangle (0, rect.Height-1, rect.Width, 1);
//-			graphics.AddFilledRectangle (0, 0, rect.Width, 1);
//-			graphics.RenderSolid (border);
		}
		
		protected override void PaintForegroundImplementation(Graphics graphics, Rectangle clipRect)
		{
			base.PaintForegroundImplementation (graphics, clipRect);
		}


		private void InvalidateUI()
		{
			this.DeleteUI ();
			this.CreateUI ();
		}


		private void CreateUI()
		{
			if (this.button == null)
            {
				return;
            }

			var cube        = this.button.Cube;
			var cardinality = this.button.Cardinality;

			if (cardinality == DataCubeCardinality.Table)
            {
				var oldValue = this.GetSourceName();

				new StaticText ()
				{
					Parent = this,
					Dock = DockStyle.Stacked,
					PreferredHeight = 32,
					BackColor = Color.FromRgb (0, 0, 0.25),
					Text = "<font color=\"#ffffff\"><b>Réglages de la table</b></font>",
				};

				var frame = new FrameBox ()
				{
					Parent = this,
					Dock = DockStyle.Stacked,
					PreferredHeight = 32,
				};
				new StaticText ()
				{
					Parent = frame,
					Dock = DockStyle.Left,
					Text = "Source",
					PreferredWidth = 70,
				};
				var field = new TextFieldEx ()
				{
					Parent = frame,
					Dock = DockStyle.Fill,
					Text = FormattedText.Escape (oldValue),
					VerticalAlignment = VerticalAlignment.Center,
					IsReadOnly = this.sourceNameIsReadOnly,
				};

				field.EditionAccepted +=
					delegate
					{
						var handler = this.SourceEdited;
						var newValue = FormattedText.Unescape (field.Text);
						
						if (handler != null)
                        {
							handler (this, new DependencyPropertyChangedEventArgs ("Source", oldValue, newValue));
                        }
					};
            }
		}

		private void DeleteUI()
		{
			this.Children.Widgets.ForEach (widget => widget.Dispose ());
		}

		public event EventHandler<DependencyPropertyChangedEventArgs> SourceEdited;

		private GraphDataCube cube;
		private DataCubeTableButton button;
		private bool sourceNameIsReadOnly;
	}
}
