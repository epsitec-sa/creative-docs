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


namespace Epsitec.Common.Types.Serialization.IO
{
    public abstract class AbstractReader
    {
        protected AbstractReader() { }

        public abstract void BeginStorageBundle(
            out int rootId,
            out int externalCount,
            out int typeCount,
            out int objectCount
        );
        public abstract void EndStorageBundle();

        public abstract string ReadExternalReference();
        public abstract string ReadTypeDefinition(int id);
        public abstract int ReadObjectDefinition(int id);

        public abstract void BeginObject(int id, DependencyObject obj);
        public abstract bool ReadObjectFieldValue(
            DependencyObject obj,
            out string field,
            out string value
        );
        public abstract void EndObject(int id, DependencyObject obj);
    }
}
