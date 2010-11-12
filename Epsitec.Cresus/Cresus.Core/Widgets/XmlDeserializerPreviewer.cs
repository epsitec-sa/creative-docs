//	Copyright © 2008, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Support;
using Epsitec.Common.Types;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Common.Printing;

using Epsitec.Common.Support.EntityEngine;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Widgets
{
	public class XmlDeserializerPreviewer : Widget
	{
		public XmlDeserializerPreviewer()
		{
		}


		public string XmlSource
		{
			get
			{
				return this.xmlSource;
			}
			set
			{
				if (this.xmlSource != value)
				{
					this.xmlSource = value;
					this.Invalidate ();
				}
			}
		}


		protected override void PaintBackgroundImplementation(Graphics graphics, Rectangle clipRect)
		{
			var rect = this.Client.Bounds;
			rect.Deflate (0.5);

			graphics.AddFilledRectangle (rect);
			graphics.RenderSolid (Color.FromName ("White"));

			graphics.AddRectangle (rect);
			graphics.RenderSolid (Color.FromName ("Black"));

			var xmlPort = new XmlPort ();
			xmlPort.Deserialize (this.xmlSource, graphics);
		}


		private string							xmlSource;
	}
}
