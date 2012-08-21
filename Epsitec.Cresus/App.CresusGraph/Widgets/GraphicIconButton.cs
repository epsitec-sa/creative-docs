//	Copyright © 2009-2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
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


		public string OnIconUri
		{
			get;
			set;
		}

		public string OffIconUri
		{
			get
			{
				return base.IconUri;
			}
			set
			{
				base.IconUri = value;
			}
		}

		public string PressIconUri
		{
			get;
			set;
		}

		public string OnHiliteIconUri
		{
			get;
			set;
		}

		public string OffHiliteIconUri
		{
			get;
			set;
		}


		public string IconFamilyUri
		{
			get
			{
				string[] elems = this.IconUri.Split ('.');
				return string.Join (".", elems.Take (elems.Length - 2).ToArray ());
			}
			set
			{
				this.OffIconUri = value + ".Off.icon";
				this.OnIconUri  = value + ".On.icon";
				this.OffHiliteIconUri = value + ".OffHilite.icon";
				this.OnHiliteIconUri = value + ".OnHilite.icon";
				this.PressIconUri = value + ".Press.icon";
			}
		}


		protected override void PaintBackgroundImplementation(Graphics graphics, Rectangle clipRect)
		{
			string icon = this.IconUri;
			var rectangle = this.Client.Bounds;

			if (this.IsEngaged)
			{
				icon = this.PressIconUri;
			}
			else
			{
				if (this.ActiveState == ActiveState.Yes)
				{
					icon = this.IsEntered ? this.OnHiliteIconUri : this.OnIconUri;
				}
				else
				{
					icon = this.IsEntered ? this.OffHiliteIconUri : this.OffIconUri;
				}
			}

			var image = ImageProvider.Instance.GetImage (icon, Resources.DefaultManager);
			graphics.PaintImage (image, rectangle);
		}
	}
}
