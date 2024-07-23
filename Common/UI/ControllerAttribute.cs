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


using Epsitec.Common.Support.Extensions;
using System.Collections.Generic;

namespace Epsitec.Common.UI
{
    /// <summary>
    /// The <c>ControllerAttribute</c> attribute is used to tag a class as
    /// compatible with the <see cref="Placeholder"/> and <see cref="IController"/>
    /// pattern. This attribute is applied at the assembly level.
    /// </summary>
    [System.AttributeUsage(System.AttributeTargets.Assembly, AllowMultiple = true)]
    public class ControllerAttribute
        : System.Attribute,
            Epsitec.Common.Support.PlugIns.IPlugInAttribute<string>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ControllerAttribute"/> class.
        /// </summary>
        /// <param name="type">The type of the class.</param>
        public ControllerAttribute(System.Type type)
        {
            this.type = type;
        }

        /// <summary>
        /// Gets or sets the type of the class which adheres to the
        /// <see cref="Placeholder"/> and <see cref="IController"/> pattern.
        /// </summary>
        /// <value>The type.</value>
        public System.Type Type
        {
            get { return this.type; }
            set { this.type = value; }
        }

        /// <summary>
        /// Gets the registered types for a specified assembly.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        /// <returns>The first registered type (as a collection).</returns>
        public static IEnumerable<System.Type> GetRegisteredTypes(
            System.Reflection.Assembly assembly
        )
        {
            foreach (var attribute in assembly.GetCustomAttributes<ControllerAttribute>())
            {
                if (attribute.Type.ContainsInterface<IController>())
                {
                    yield return attribute.Type;
                    break;
                }
            }
        }

        #region IPlugInAttribute<string> Members

        string Epsitec.Common.Support.PlugIns.IPlugInAttribute<string>.Id
        {
            get { return this.type.Name; }
        }

        System.Type Epsitec.Common.Support.PlugIns.IPlugInAttribute<string>.Type
        {
            get { return this.type; }
        }

        #endregion

        private System.Type type;
    }
}
