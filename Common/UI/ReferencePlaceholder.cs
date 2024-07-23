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


using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;
using Epsitec.Common.UI;

[assembly: DependencyClass(typeof(ReferencePlaceholder))]

namespace Epsitec.Common.UI
{
    public class ReferencePlaceholder : Placeholder
    {
        public ReferencePlaceholder() { }

        public StructuredType EntityType { get; set; }

        public Druid EntityId
        {
            get { return this.EntityType == null ? Druid.Empty : this.EntityType.CaptionId; }
        }

        public EntityFieldPath EntityFieldPath { get; set; }

        protected override void GetAssociatedController(
            out string newControllerName,
            out string newControllerParameters
        )
        {
            newControllerName = "Reference";
            newControllerParameters = Controllers.ControllerParameters.MergeParameters(
                string.Concat("EntityId=", this.EntityId.ToString()),
                this.ControllerParameters
            );
        }
    }
}
