//	Copyright © 2008, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using Epsitec.Common.Types;
using Epsitec.Common.Widgets;

[assembly: DependencyClass(typeof(FlatTextField))]

namespace Epsitec.Common.Widgets
{
    public class FlatTextField : AbstractTextField
    {
        public FlatTextField()
            : base(TextFieldStyle.Flat)
        {
            this.BackColor = Drawing.Color.Transparent;
        }
    }
}
