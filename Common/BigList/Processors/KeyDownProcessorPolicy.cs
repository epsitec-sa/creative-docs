//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Common.BigList.Processors
{
    public class KeyDownProcessorPolicy : EventProcessorPolicy
    {
        public KeyDownProcessorPolicy()
        {
            this.PassiveScrollMode = ScrollMode.MoveVisible;
        }

        public ScrollMode PassiveScrollMode { get; set; }
    }
}
