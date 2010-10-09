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
		NodeComment,
		NodeInfo,
		NodeExtend,
		NodeClose,
		NodeHeader,
		NodeEdgeName,
		NodeEdgeType,
		NodeEdgeExpression,
		NodeEdgeAdd,
		NodeEdgeRemove,
		NodeEdgeMovable,
		NodeEdgeMoving,
		NodeEdgeTitle,
		NodeChangeWidth,
		NodeMoveColumnsSeparator1,
		NodeColor1,
		NodeColor2,
		NodeColor3,
		NodeColor4,
		NodeColor5,
		NodeColor6,
		NodeColor7,
		NodeColor8,

		EdgeOpenLeft,
		EdgeOpenRight,
		EdgeClose,
		EdgeHilited,
		EdgeMove1,
		EdgeMove2,
		EdgeChangeDst,
		EdgeComment,

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
		CommentAttachToEdge,

		InfoEdit,
		InfoMove,
		InfoWidth,
		InfoClose,
	}
}
