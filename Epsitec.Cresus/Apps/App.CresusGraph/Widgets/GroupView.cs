//	Copyright © 2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Drawing;
using Epsitec.Common.Graph.Widgets;
using Epsitec.Common.Widgets;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Graph.Widgets
{
	public sealed class GroupView : FrameBox
	{
		public GroupView()
		{
			this.frame = new FrameBox ()
			{
				Dock = DockStyle.Fill,
				Parent = this,
				ContainerLayoutMode = ContainerLayoutMode.VerticalFlow,
				Margins = new Margins (4, 0, 0, 0),
				Visibility = false,
			};
		}
		
		public MiniChartView View
		{
			get
			{
				return this.view;
			}
			set
			{
				if (this.view != value)
				{
					if (this.view != null)
					{
						this.view.Parent = null;
					}

					this.view = value;

					if (this.view != null)
					{
						this.view.Parent = this;
						this.view.Dock   = DockStyle.Left;
					}
				}
			}
		}

		public FrameBox ButtonSurface
		{
			get
			{
				return this.frame;
			}
		}


		protected override void OnEntered(MessageEventArgs e)
		{
			base.OnEntered (e);
			this.UpdateButtons ();
		}

		protected override void OnExited(MessageEventArgs e)
		{
			base.OnExited (e);
			this.UpdateButtons ();
		}

		protected override void OnSelected()
		{
			base.OnSelected ();
			this.UpdateButtons ();
		}

		protected override void OnDeselected()
		{
			base.OnDeselected ();
			this.UpdateButtons ();
		}

		protected override void OnPaintBackground(PaintEventArgs e)
		{
			base.OnPaintBackground (e);
		}

		private void UpdateButtons()
		{
			this.frame.Visibility = this.IsEntered | this.IsSelected;
		}


		private MiniChartView view;
		private FrameBox frame;
	}
}
