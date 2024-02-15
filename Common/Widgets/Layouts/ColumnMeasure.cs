//	Copyright © 2006-2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using Epsitec.Common.Types;
using Epsitec.Common.Widgets.Layouts;

[assembly: DependencyClass(typeof(ColumnMeasure))]

namespace Epsitec.Common.Widgets.Layouts
{
    /// <summary>
    /// The <c>ColumnMeasure</c> class is a <see cref="LayoutMeasure"/> applied to
    /// columns in a grid layout.
    /// </summary>
    public sealed class ColumnMeasure : LayoutMeasure
    {
        public ColumnMeasure()
            : this(0) { }

        public ColumnMeasure(int passId)
            : base(passId) { }
    }
}
