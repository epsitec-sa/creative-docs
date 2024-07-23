/*
This file is part of CreativeDocs.

Copyright © 2003-2024, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland

CreativeDocs is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
any later version.

CreativeDocs is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/


using Epsitec.Common.Drawing;
using Epsitec.Common.Support.Extensions;
using System.Collections.Generic;

namespace Epsitec.Common.BigList.Processors
{
    public class MouseDragProcessorPolicy : EventProcessorPolicy
    {
        public MouseDragProcessorPolicy()
        {
            this.ResizePolicy = ResizePolicy.Independent;
        }

        public ResizePolicy ResizePolicy { get; set; }

        public IEnumerable<MouseDragFrame> Filter(IEnumerable<MouseDragFrame> frames)
        {
            MouseDragFrame previous = default(MouseDragFrame);

            foreach (var frame in frames)
            {
                if (this.Filter(frame, previous))
                {
                    yield return frame;
                }

                previous = frame;
            }
        }

        private bool Filter(MouseDragFrame frame, MouseDragFrame previous)
        {
            switch (this.ResizePolicy)
            {
                case ResizePolicy.None:
                    return false;

                case ResizePolicy.Independent:
                    if (frame.Elasticity == MouseDragElasticity.None)
                    {
                        if (frame.Grip == GripId.EdgeRight || frame.Grip == GripId.EdgeBottom)
                        {
                            return true;
                        }
                        if (frame.Grip == GripId.EdgeLeft || frame.Grip == GripId.EdgeTop)
                        {
                            if (
                                (previous.IsEmpty == false)
                                && (previous.Elasticity == MouseDragElasticity.Stretch)
                            )
                            {
                                return true;
                            }
                        }
                    }
                    return false;

                case ResizePolicy.Coupled:
                    return true;

                default:
                    throw new System.NotSupportedException(
                        string.Format("{0} not supported", this.ResizePolicy.GetQualifiedName())
                    );
            }
        }
    }
}
