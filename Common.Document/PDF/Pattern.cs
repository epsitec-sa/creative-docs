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

using Epsitec.Common.Drawing;

namespace Epsitec.Common.Document.PDF
{
    /// <summary>
    /// La classe Pattern enregistre les informations d'un pattern.
    /// </summary>
    public class Pattern
    {
        public int Page
        {
            get { return this.page; }
            set { this.page = value; }
        }

        public Objects.Abstract Object
        {
            get { return this.obj; }
            set { this.obj = value; }
        }

        public Properties.Abstract Property
        {
            get { return this.property; }
            set { this.property = value; }
        }

        public int Rank
        {
            get { return this.rank; }
            set { this.rank = value; }
        }

        public int Id
        {
            get { return this.id; }
            set { this.id = value; }
        }

        protected int page; // 1..n
        protected Objects.Abstract obj;
        protected Properties.Abstract property;
        protected int rank;
        protected int id;
    }
}
