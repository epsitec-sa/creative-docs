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


namespace Epsitec.Common.Types
{
    /// <summary>
    /// The IDictionaryHost interface is implemented by the class which hosts a
    /// HostedDictionary (usually through an Items property), so that the HostedDictionary
    /// can notify the host of its changes.
    /// </summary>
    /// <typeparam name="K">Key type used by HostedDictionary</typeparam>
    /// <typeparam name="V">Value type used by HostedDictionary</typeparam>
    public interface IDictionaryHost<K, V>
    {
        Collections.HostedDictionary<K, V> Items { get; }

        void NotifyDictionaryInsertion(K key, V value);
        void NotifyDictionaryRemoval(K key, V value);
    }
}
