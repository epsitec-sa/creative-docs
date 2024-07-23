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


using System.Collections.Concurrent;

namespace Epsitec.Common.Types.Collections.Concurrent
{
    /// <summary>
    /// The <c>BlockingQueue&lt;T&gt;</c> class implements a concurrent queue, wrapped
    /// by a <see cref="BlockingCollection"/>.
    /// </summary>
    /// <typeparam name="T">The type of elements in the collection.</typeparam>
    public sealed class BlockingQueue<T> : BlockingCollection<T>
    {
        public BlockingQueue()
            : base(new ConcurrentQueue<T>()) { }
    }
}
