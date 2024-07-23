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

using Epsitec.Common.Widgets;

namespace Epsitec.Common.Document.Widgets
{
    /// <summary>
    /// La classe AbstractSample est un widget affichant un échantillon quelconque.
    /// </summary>
    public abstract class AbstractSample : Widget
    {
        public AbstractSample() { }

        public AbstractSample(Widget embedder)
            : this()
        {
            this.SetEmbedder(embedder);
        }

        public Document Document
        {
            //	Document associé.
            get { return this.document; }
            set { this.document = value; }
        }

        public bool IsDeep
        {
            get { return this.isDeep; }
            set { this.isDeep = value; }
        }

        protected Document document;
        protected bool isDeep;
    }
}
