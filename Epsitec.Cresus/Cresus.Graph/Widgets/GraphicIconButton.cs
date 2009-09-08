//	Copyright © 2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Drawing;

using Epsitec.Common.Support;
using Epsitec.Common.Support.Extensions;
using Epsitec.Common.Types;
using Epsitec.Common.Widgets;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Graph.Widgets
{
	public class GraphicIconButton : IconButton
	{
		public GraphicIconButton()
		{
			
		}


		public string OnIconName
		{
			get;
			set;
		}

		public string OffIconName
		{
			get
			{
				return base.IconName;
			}
			set
			{
				base.IconName = value;
			}
		}

		public string PressIconName
		{
			get;
			set;
		}

		public string OnHiliteIconName
		{
			get;
			set;
		}

		public string OffHiliteIconName
		{
			get;
			set;
		}


		public string IconFamilyName
		{
			get
			{
				string[] elems = this.IconName.Split ('.');
				return string.Join (".", elems.Take (elems.Length - 2).ToArray ());
			}
			set
			{
				this.OffIconName = value + ".Off.icon";
				this.OnIconName  = value + ".On.icon";
				this.OffHiliteIconName = value + ".OffHilite.icon";
				this.OnHiliteIconName = value + ".OnHilite.icon";
				this.PressIconName = value + ".Press.icon";
			}
		}


		protected override void PaintBackgroundImplementation(Graphics graphics, Rectangle clipRect)
		{
			string icon = this.IconName;
			var rectangle = this.Client.Bounds;

			if (this.IsEngaged)
			{
				icon = this.PressIconName;
			}
			else
			{
				if (this.ActiveState == ActiveState.Yes)
				{
					icon = this.IsEntered ? this.OnHiliteIconName : this.OnIconName;
				}
				else
				{
					icon = this.IsEntered ? this.OffHiliteIconName : this.OffIconName;
				}
			}

			var image = ImageProvider.Default.GetImage (icon, Resources.DefaultManager);
			graphics.PaintImage (image, rectangle);
		}
	}
}
