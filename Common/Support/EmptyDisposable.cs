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

using System;

namespace Epsitec.Common.Support
{
    public sealed class EmptyDisposable : IDisposable
    {
        private EmptyDisposable() { }

        #region IDisposable Members


        public void Dispose() { }

        #endregion


        public static IDisposable Instance
        {
            get { return EmptyDisposable.instance; }
        }

        private static readonly IDisposable instance = new EmptyDisposable();
    }
}
