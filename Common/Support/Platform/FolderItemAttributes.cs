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


namespace Epsitec.Common.Support.Platform
{
    [System.Flags]
    internal enum FolderItemAttributes
    {
        None = 0,

        Browsable = 0x0001,
        CanCopy = 0x0002,
        CanDelete = 0x0004,
        CanMove = 0x0008,
        CanRename = 0x0010,
        Compressed = 0x0020,
        Encrypted = 0x0040,
        Hidden = 0x0080,
        Shortcut = 0x0100,
        ReadOnly = 0x0200,
        Shared = 0x0400,
        Folder = 0x0800,
        FileSystemNode = 0x1000,
        WebLink = 0x2000
    }
}
