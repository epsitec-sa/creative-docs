//	Copyright © 2006-2012, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Widgets.Layouts
{
    /// <summary>
    /// The <c>GridUnitType</c> enumeration describes the kind of value a
    /// <see cref="T:GridLength" /> object is holding.
    /// </summary>
    public enum GridUnitType
    {
        /// <summary>
        /// Size is determined by the content of the grid.
        /// </summary>
        Auto,

        /// <summary>
        /// The value is an absolute size.
        /// </summary>
        Absolute,

        /// <summary>
        /// The value is a weighted proportion of the available size.
        /// </summary>
        Proportional
    }
}
