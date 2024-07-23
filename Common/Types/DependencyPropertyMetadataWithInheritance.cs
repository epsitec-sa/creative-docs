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
    /// DependencyPropertyMetadataWithInheritance.
    /// </summary>
    public class DependencyPropertyMetadataWithInheritance : DependencyPropertyMetadata
    {
        public DependencyPropertyMetadataWithInheritance()
            : base() { }

        public DependencyPropertyMetadataWithInheritance(object defaultValue)
            : base(defaultValue) { }

        public DependencyPropertyMetadataWithInheritance(
            object defaultValue,
            PropertyInvalidatedCallback propertyInvalidatedCallback
        )
            : base(defaultValue, propertyInvalidatedCallback) { }

        public DependencyPropertyMetadataWithInheritance(
            GetValueOverrideCallback getValueOverrideCallback
        )
            : base(getValueOverrideCallback) { }

        public DependencyPropertyMetadataWithInheritance(
            GetValueOverrideCallback getValueOverrideCallback,
            SetValueOverrideCallback setValueOverrideCallback
        )
            : base(getValueOverrideCallback, setValueOverrideCallback) { }

        public DependencyPropertyMetadataWithInheritance(
            GetValueOverrideCallback getValueOverrideCallback,
            SetValueOverrideCallback setValueOverrideCallback,
            PropertyInvalidatedCallback propertyInvalidatedCallback
        )
            : base(getValueOverrideCallback, setValueOverrideCallback, propertyInvalidatedCallback)
        { }

        public DependencyPropertyMetadataWithInheritance(
            GetValueOverrideCallback getValueOverrideCallback,
            PropertyInvalidatedCallback propertyInvalidatedCallback
        )
            : base(getValueOverrideCallback, propertyInvalidatedCallback) { }

        public DependencyPropertyMetadataWithInheritance(
            object defaultValue,
            GetValueOverrideCallback getValueOverrideCallback,
            SetValueOverrideCallback setValueOverrideCallback
        )
            : base(defaultValue, getValueOverrideCallback, setValueOverrideCallback) { }

        public DependencyPropertyMetadataWithInheritance(
            object defaultValue,
            GetValueOverrideCallback getValueOverrideCallback,
            PropertyInvalidatedCallback propertyInvalidatedCallback
        )
            : base(defaultValue, getValueOverrideCallback, propertyInvalidatedCallback) { }

        public DependencyPropertyMetadataWithInheritance(
            object defaultValue,
            GetValueOverrideCallback getValueOverrideCallback
        )
            : base(defaultValue, getValueOverrideCallback) { }

        public DependencyPropertyMetadataWithInheritance(
            object defaultValue,
            SetValueOverrideCallback setValueOverrideCallback
        )
            : base(defaultValue, setValueOverrideCallback) { }

        public override bool InheritsValue
        {
            get { return true; }
        }

        protected override DependencyPropertyMetadata CloneNewObject()
        {
            return new DependencyPropertyMetadataWithInheritance();
        }
    }
}
