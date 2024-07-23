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


using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Common.Support.EntityEngine
{
    /// <summary>
    /// The <c>ActionInfo</c> class represents a method which can be called to perform
    /// an action. It has an associated caption.
    /// </summary>
    public sealed class ActionInfo
    {
        internal ActionInfo(ActionAttribute attribute, System.Action<AbstractEntity> action)
        {
            this.attribute = attribute;
            this.action = action;
        }

        public ActionClasses ActionClass
        {
            get { return this.attribute.ActionClass; }
        }

        public Druid CaptionId
        {
            get { return this.attribute.CaptionId; }
        }

        public double Weight
        {
            get { return this.attribute.Weight; }
        }

        public void ExecuteAction(AbstractEntity entity)
        {
            if (this.action == null)
            {
                throw new System.ArgumentNullException("action");
            }
            else
            {
                this.action(entity);
            }
        }

        private readonly ActionAttribute attribute;
        private readonly System.Action<AbstractEntity> action;
    }
}
