//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;
using Epsitec.Common.Drawing;

using Epsitec.Cresus.Core.Entities;

using System.Xml;
using System.Xml.Serialization;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.WorkflowDesigner.Objects
{
	public class ObjectEdgeFoo : LinkableObject
	{
		public ObjectEdgeFoo(Editor editor, AbstractEntity entity)
			: base (editor, entity)
		{
		}


		public override Rectangle Bounds
		{
			get
			{
				return this.bounds;
			}
		}

		public override Vector GetLinkVector(LinkAnchor anchor, Point dstPos, bool isDst)
		{
			switch (anchor)
			{
				case LinkAnchor.Left:
					return new Vector (new Point (this.bounds.Center.X+1, this.bounds.Center.Y), new Size (1, 0));

				case LinkAnchor.Right:
					return new Vector (new Point (this.bounds.Center.X-1, this.bounds.Center.Y), new Size (-1, 0));

				case LinkAnchor.Bottom:
					return new Vector (new Point (this.bounds.Center.X, this.bounds.Center.Y+1), new Size (0, 1));

				case LinkAnchor.Top:
					return new Vector (new Point (this.bounds.Center.X, this.bounds.Center.Y-1), new Size (0, -1));
			}

			return Vector.Zero;
		}
	}
}
