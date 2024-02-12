//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Support;

namespace Epsitec.Common.Widgets.Behaviors
{
    public class SlimFieldMenuSelectionEventArgs : CancelEventArgs
    {
        public SlimFieldMenuSelectionEventArgs(SlimFieldMenuItem item)
        {
            this.item = item;
        }

        public SlimFieldMenuItem Item
        {
            get { return this.item; }
        }

        private readonly SlimFieldMenuItem item;
    }
}
