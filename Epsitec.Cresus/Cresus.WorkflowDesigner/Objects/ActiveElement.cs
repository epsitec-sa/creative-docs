//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.WorkflowDesigner.Objects
{
	public enum ActiveElement
	{
		None,

		NodeInside,
		NodeHeader,
		NodeComment,
		NodeClose,
		NodeInfo,
		NodeAuto,
		NodeExtend,
		NodeOpenLink,
		NodeColor1,
		NodeColor2,
		NodeColor3,
		NodeColor4,
		NodeColor5,
		NodeColor6,
		NodeColor7,
		NodeColor8,

		EdgeInside,
		EdgeHeader,
		EdgeComment,
		EdgeType,
		EdgeClose,
		EdgeHilited,
		EdgeExtend,
		EdgeEditDescription,
		EdgeChangeWidth,
		EdgeColor1,
		EdgeColor2,
		EdgeColor3,
		EdgeColor4,
		EdgeColor5,
		EdgeColor6,
		EdgeColor7,
		EdgeColor8,

		LinkHilited,
		LinkClose,
		LinkComment,
		LinkChangeDst,
		LinkCreateDst,
		LinkCustomizeStart,
		LinkCustomizeEnd,

		CommentEdit,
		CommentMove,
		CommentWidth,
		CommentClose,
		CommentColor1,
		CommentColor2,
		CommentColor3,
		CommentColor4,
		CommentColor5,
		CommentColor6,
		CommentColor7,
		CommentColor8,
		CommentAttachTo,

		InfoEdit,
		InfoMove,
		InfoWidth,
		InfoClose,
	}
}
