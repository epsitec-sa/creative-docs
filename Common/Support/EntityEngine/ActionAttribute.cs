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


namespace Epsitec.Common.Support.EntityEngine
{
    /// <summary>
    /// The <c>ActionAttribute</c> class defines a <c>[Action]</c> attribute,
    /// which is used by the <see cref="ActionDispatcher"/> to locate methods
    /// implementing publicly available actions.
    /// </summary>

    [System.Serializable]
    [System.AttributeUsage(System.AttributeTargets.Method, AllowMultiple = true)]
    public sealed class ActionAttribute : System.Attribute
    {
        public ActionAttribute(long captionId, double weight = 0.0)
            : this(ActionClasses.None, captionId, weight) { }

        public ActionAttribute(ActionClasses actionClass, long captionId, double weight = 0.0)
        {
            this.actionClass = actionClass;
            this.captionId = captionId;
            this.weight = weight;
        }

        public ActionClasses ActionClass
        {
            get { return this.actionClass; }
        }

        public Druid CaptionId
        {
            get { return this.captionId; }
        }

        public double Weight
        {
            get { return this.weight; }
        }

        private readonly ActionClasses actionClass;
        private readonly Druid captionId;
        private readonly double weight;
    }
}
