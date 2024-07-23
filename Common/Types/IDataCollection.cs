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


using System.Collections.Generic;

namespace Epsitec.Common.Types
{
    /// <summary>
    /// L'interface IDataCollection donne accès à une collection de IDataItem.
    /// </summary>
    public interface IDataCollection : IEnumerable<IDataItem>, ICollection<IDataItem>
    {
        IDataItem this[int index] { get; }
        IDataItem this[string name] { get; }
        int IndexOf(IDataItem item);
        //	IEnumerable Members:
        //
        //	System.Collection.IEnumerator GetEnumerator();

        //	ICollection Members:
        //
        //	int		Count					{ get; }
        //	bool	IsSynchronized			{ get; }
        //	object	SyncRoot				{ get; }
        //
        //	void CopyTo(System.Array array, int index);
    }
}
