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
using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Common.Widgets
{
    public class SlimFieldMenuItem<T> : SlimFieldMenuItem
    {
        public SlimFieldMenuItem(
            T value,
            /**/IEnumerable<TextValue> texts,
            /**/ActiveState active = ActiveState.No,
            /**/EnableState enable = EnableState.Enabled,
            /**/SlimFieldMenuItemStyle style = SlimFieldMenuItemStyle.Value,
            /**/SlimFieldMenuItemHilite hilite = SlimFieldMenuItemHilite.None
        )
            : this(
                value,
                new StringArray(texts.Select(x => x.SimpleText)),
                active,
                enable,
                style,
                hilite
            ) { }

        public SlimFieldMenuItem(
            T value,
            /**/StringArray texts,
            /**/ActiveState active = ActiveState.No,
            /**/EnableState enable = EnableState.Enabled,
            /**/SlimFieldMenuItemStyle style = SlimFieldMenuItemStyle.Value,
            /**/SlimFieldMenuItemHilite hilite = SlimFieldMenuItemHilite.None
        )
            : base(texts, active, enable, style, hilite)
        {
            this.Value = value;
        }

        public T Value { get; set; }
    }
}
