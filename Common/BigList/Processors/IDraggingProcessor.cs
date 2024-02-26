//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Drawing;
using System.Collections.Generic;

namespace Epsitec.Common.BigList.Processors
{
    public interface IDraggingProcessor
    {
        IEnumerable<MouseDragFrame> DetectDrag(Point pos);

        void ApplyDrag(MouseDragFrame originalFrame, MouseDragFrame currentFrame);
    }
}
