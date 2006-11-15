//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using Epsitec.Common.Types;
using Epsitec.Common.UI;
using Epsitec.Common.UI.Panels;

using System.Collections.Generic;

[assembly: DependencyClass (typeof (EditPanel))]

namespace Epsitec.Common.UI.Panels
{
	internal class EditPanel : Panel
	{
		public EditPanel()
		{
		}
		
		public EditPanel(Panel owner)
		{
			this.owner = owner;
		}

		public override Panel Owner
		{
			get
			{
				return this.owner;
			}
		}

		public override DataSourceMetadata DataSourceMetadata
		{
			get
			{
				return this.owner.DataSourceMetadata;
			}
		}

		public override Panel GetPanel(PanelMode mode)
		{
			return this.owner.GetPanel (mode);
		}

		public override PanelMode PanelMode
		{
			get
			{
				return PanelMode.Edition;
			}
		}

		protected override void PaintBackgroundImplementation(Drawing.Graphics graphics, Drawing.Rectangle clipRect)
		{
			graphics.AddFilledRectangle (Drawing.Rectangle.Intersection (clipRect, this.Client.Bounds));
			graphics.RenderSolid (Drawing.Color.FromRgb (1, 1, 1));
		}

		private static object GetOwnerValue(DependencyObject obj)
		{
			EditPanel panel = (EditPanel) obj;
			return panel.Owner;
		}

		private static void SetOwnerValue(DependencyObject obj, object value)
		{
			EditPanel panel = (EditPanel) obj;
			panel.owner = (Panel) value;
		}

		public static DependencyProperty OwnerProperty = DependencyProperty.Register ("Owner", typeof (Panel), typeof (EditPanel), new DependencyPropertyMetadata (EditPanel.GetOwnerValue, EditPanel.SetOwnerValue));
		
		private Panel owner;
	}
}
