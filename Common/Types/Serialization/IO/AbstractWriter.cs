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

namespace Epsitec.Common.Types.Serialization.IO
{
    public abstract class AbstractWriter
    {
        protected AbstractWriter() { }

        public virtual void BeginStorageBundle(
            int id,
            int externalCount,
            int typeCount,
            int objectCount
        ) { }

        public virtual void EndStorageBundle() { }

        public abstract void WriteAttributeStrings();
        public abstract void WriteExternalReference(string name);
        public abstract void WriteTypeDefinition(int id, string name);
        public abstract void WriteObjectDefinition(int id, int typeId);

        public virtual void BeginObject(int id, DependencyObject obj) { }

        public virtual void EndObject(int id, DependencyObject obj) { }

        public abstract void WriteObjectFieldReference(DependencyObject obj, string name, int id);
        public abstract void WriteObjectFieldReferenceList(
            DependencyObject obj,
            string name,
            IList<int> ids
        );
        public abstract void WriteObjectFieldValue(DependencyObject obj, string name, string value);
    }
}
