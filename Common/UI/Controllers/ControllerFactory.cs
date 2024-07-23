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


using Epsitec.Common.Types;

namespace Epsitec.Common.UI.Controllers
{
    /// <summary>
    /// The <c>ControllerFactory</c> class is used to create and configure
    /// controller instances used by the <see cref="Placeholder"/> class.
    /// </summary>
    public class ControllerFactory
        : Epsitec.Common.Support.PlugIns.PlugInFactory<IController, ControllerAttribute, string>
    {
        /// <summary>
        /// Creates the controller based on its name and parameter.
        /// </summary>
        /// <param name="name">The controller name.</param>
        /// <param name="parameters">The optional controller parameters.</param>
        /// <returns>An object implementing <see cref="IController"/> or
        /// <c>null</c> if the specified controller cannot be found.</returns>
        public static IController CreateController(string name, ControllerParameters parameters)
        {
            return ControllerFactory.CreateInstance(name + "Controller", parameters);
        }

        /// <summary>
        /// Gets the default controller and its parameters based on the binding.
        /// The information is derived from the data type.
        /// </summary>
        /// <param name="binding">The binding.</param>
        /// <param name="controllerName">The controller name.</param>
        /// <param name="controllerParameter">The controller parameter.</param>
        /// <returns><c>true</c> if a controller definition could be derived
        /// from the binding; otherwise, <c>false</c>.</returns>
        public static bool GetDefaultController(
            BindingExpression binding,
            out string controllerName,
            out string controllerParameter
        )
        {
            controllerName = null;
            controllerParameter = null;

            if (binding == null)
            {
                return false;
            }

            INamedType type = binding.GetSourceNamedType();

            if (type == null)
            {
                return false;
            }

            controllerName = type.DefaultController;
            controllerParameter = type.DefaultControllerParameters;

            return string.IsNullOrEmpty(controllerName) ? false : true;
        }
    }
}
