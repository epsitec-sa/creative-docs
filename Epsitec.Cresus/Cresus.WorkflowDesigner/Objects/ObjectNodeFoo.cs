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
	public class ObjectNodeFoo : LinkableObject
	{
		public ObjectNodeFoo(Editor editor, AbstractEntity entity)
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
			Point p1 = Point.Move (this.bounds.Center, dstPos, 1);
			Point p2 = Point.Move (this.bounds.Center, dstPos, 2);

			return new Vector (p1, p2);
		}
	}
}
