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


[assembly: Epsitec.Common.Types.DependencyClass(typeof(Epsitec.Common.Widgets.TextField))]

namespace Epsitec.Common.Widgets
{
    /// <summary>
    /// The <c>TextField</c> class implements a simple text field.
    /// </summary>
    public class TextField : AbstractTextField
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TextField"/> class.
        /// </summary>
        public TextField()
            : this(TextFieldStyle.Normal) { }

        public TextField(TextFieldStyle style)
            : base(style) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="TextField"/> class.
        /// </summary>
        /// <param name="embedder">The embedder.</param>
        public TextField(Widget embedder)
            : this()
        {
            this.SetEmbedder(embedder);
        }
    }
}
