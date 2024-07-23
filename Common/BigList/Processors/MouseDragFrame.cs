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

namespace Epsitec.Common.BigList.Processors
{
    public struct MouseDragFrame
    {
        public MouseDragFrame(
            int index,
            GripId grip,
            Rectangle bounds,
            MouseDragDirection direction,
            Rectangle constraint,
            MouseDragElasticity elasticity
        )
        {
            this.index = index;
            this.grip = grip;
            this.bounds = bounds;
            this.direction = direction;
            this.constraint = constraint;
            this.elasticity = elasticity;
        }

        public MouseDragFrame(MouseDragFrame original, Rectangle bounds)
        {
            this.index = original.index;
            this.grip = original.grip;
            this.bounds = bounds;
            this.direction = original.direction;
            this.constraint = original.constraint;
            this.elasticity = original.elasticity;
        }

        public int Index
        {
            get { return this.index; }
        }

        public GripId Grip
        {
            get { return this.grip; }
        }

        public Rectangle Bounds
        {
            get { return this.bounds; }
        }

        public MouseDragDirection Direction
        {
            get { return this.direction; }
        }

        public Rectangle Constraint
        {
            get { return this.constraint; }
        }

        public MouseDragElasticity Elasticity
        {
            get { return this.elasticity; }
        }

        public bool IsEmpty
        {
            get { return this.grip == GripId.None; }
        }

        private readonly int index;
        private readonly GripId grip;
        private readonly Rectangle bounds;
        private readonly MouseDragDirection direction;
        private readonly Rectangle constraint;
        private readonly MouseDragElasticity elasticity;
    }
}
