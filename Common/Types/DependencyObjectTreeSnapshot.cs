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
    /// The DependencyObjectTreeSnapshot is used to compute differences between sets of
    /// object properties.
    /// </summary>
    public class DependencyObjectTreeSnapshot
    {
        public DependencyObjectTreeSnapshot() { }

        public void RecordInherited(DependencyObject obj)
        {
            //	Record the state of all inherited properties.

            System.Type type = obj.GetType();

            foreach (DependencyProperty property in obj.ObjectType.GetProperties())
            {
                DependencyPropertyMetadata metadata = property.GetMetadata(type);

                if ((metadata != null) && (metadata.InheritsValue))
                {
                    this.list.Add(new SnapshotValue(obj, property));
                }
            }
        }

        public void Record(DependencyObject obj, DependencyProperty property)
        {
            System.Diagnostics.Debug.Assert(property != null);

            this.list.Add(new SnapshotValue(obj, property));
        }

        public void Record(
            DependencyObject obj,
            DependencyProperty property1,
            DependencyProperty property2
        )
        {
            System.Diagnostics.Debug.Assert(property1 != null);
            System.Diagnostics.Debug.Assert(property2 != null);

            this.list.Add(new SnapshotValue(obj, property1));
            this.list.Add(new SnapshotValue(obj, property2));
        }

        public void Record(DependencyObject obj, IEnumerable<DependencyProperty> properties)
        {
            System.Diagnostics.Debug.Assert(properties != null);

            foreach (DependencyProperty property in properties)
            {
                this.list.Add(new SnapshotValue(obj, property));
            }
        }

        public void RecordUndefinedTree(DependencyObject obj, DependencyProperty property)
        {
            if (obj.ContainsLocalValue(property) == false)
            {
                this.list.Add(new SnapshotValue(obj, property, UndefinedValue.Value));

                if (DependencyObjectTree.GetHasChildren(obj))
                {
                    foreach (DependencyObject child in DependencyObjectTree.GetChildren(obj))
                    {
                        this.RecordUndefinedTree(child, property);
                    }
                }
            }
        }

        public void RecordSubtree(DependencyObject root, DependencyProperty property)
        {
            if (DependencyObjectTree.GetHasChildren(root))
            {
                foreach (DependencyObject child in DependencyObjectTree.GetChildren(root))
                {
                    this.Record(child, property);
                    this.RecordSubtree(child, property);
                }
            }
        }

        public void RecordSubtree(
            DependencyObject root,
            DependencyProperty property1,
            DependencyProperty property2
        )
        {
            if (DependencyObjectTree.GetHasChildren(root))
            {
                foreach (DependencyObject child in DependencyObjectTree.GetChildren(root))
                {
                    this.Record(child, property1, property2);
                    this.RecordSubtree(child, property1, property2);
                }
            }
        }

        public void RecordSubtree(DependencyObject root, IEnumerable<DependencyProperty> properties)
        {
            if (DependencyObjectTree.GetHasChildren(root))
            {
                foreach (DependencyObject child in DependencyObjectTree.GetChildren(root))
                {
                    this.Record(child, properties);
                    this.RecordSubtree(child, properties);
                }
            }
        }

        public void AddNewInheritedProperties(DependencyObject obj)
        {
            //	Analyse the object tree starting at 'obj' and add any new properties
            //	which were not yet known by this snapshot.

            this.AddNewProperties(DependencyObjectTree.CreateInheritedPropertyTreeSnapshot(obj));
        }

        public void AddNewProperties(DependencyObjectTreeSnapshot other)
        {
            foreach (SnapshotValue snapshot in other.list)
            {
                DependencyObject obj = snapshot.Object;
                DependencyProperty property = snapshot.Property;

                for (int i = 0; i < this.list.Count; i++)
                {
                    if ((this.list[i].Object == obj) && (this.list[i].Property == property))
                    {
                        property = null;
                        break;
                    }
                }

                if (property != null)
                {
                    this.list.Add(new SnapshotValue(obj, property, UndefinedValue.Value));
                }
            }
        }

        public void InvalidateDifferentProperties()
        {
            foreach (SnapshotValue snapshot in this.list)
            {
                ChangeRecord record = new ChangeRecord(
                    snapshot.Object,
                    snapshot.Property,
                    snapshot.Value
                );
                record.InvalidateIfChanged();
            }
        }

        public ChangeRecord[] GetChanges()
        {
            List<ChangeRecord> records = new List<ChangeRecord>();

            foreach (SnapshotValue snapshot in this.list)
            {
                object oldValue = snapshot.Value;
                object newValue = snapshot.Object.GetValue(snapshot.Property);

                if (oldValue != newValue)
                {
                    if (
                        (oldValue == null)
                        || (UndefinedValue.IsUndefinedValue(oldValue))
                        || (oldValue.Equals(newValue) == false)
                    )
                    {
                        records.Add(
                            new ChangeRecord(snapshot.Object, snapshot.Property, oldValue, newValue)
                        );
                    }
                }
            }

            return records.ToArray();
        }

        #region ChangeRecord Structure
        public struct ChangeRecord
        {
            public ChangeRecord(DependencyObject obj, DependencyProperty property, object oldValue)
            {
                this.obj = obj;
                this.property = property;
                this.oldValue = oldValue;
                this.newValue = obj.GetValue(property);
            }

            public ChangeRecord(
                DependencyObject obj,
                DependencyProperty property,
                object oldValue,
                object newValue
            )
            {
                this.obj = obj;
                this.property = property;
                this.oldValue = oldValue;
                this.newValue = newValue;
            }

            public DependencyObject Object
            {
                get { return this.obj; }
            }
            public DependencyProperty Property
            {
                get { return this.property; }
            }
            public object OldValue
            {
                get { return this.oldValue; }
            }
            public object NewValue
            {
                get { return this.newValue; }
            }

            public void InvalidateIfChanged()
            {
                if (this.oldValue != this.newValue)
                {
                    if (
                        (this.oldValue == null)
                        || (UndefinedValue.IsUndefinedValue(this.oldValue))
                        || (this.oldValue.Equals(this.newValue) == false)
                    )
                    {
                        this.obj.InvalidateProperty(this.property, this.oldValue, this.newValue);
                    }
                }
            }

            private DependencyObject obj;
            private DependencyProperty property;
            private object oldValue;
            private object newValue;
        }
        #endregion

        #region Private SnapshotValue Structure
        private struct SnapshotValue
        {
            public SnapshotValue(DependencyObject obj, DependencyProperty property)
            {
                this.obj = obj;
                this.property = property;
                this.value = obj.GetValue(property);
            }

            public SnapshotValue(DependencyObject obj, DependencyProperty property, object value)
            {
                this.obj = obj;
                this.property = property;
                this.value = value;
            }

            public DependencyObject Object
            {
                get { return this.obj; }
            }
            public DependencyProperty Property
            {
                get { return this.property; }
            }
            public object Value
            {
                get { return this.value; }
            }

            private DependencyObject obj;
            private DependencyProperty property;
            private object value;
        }
        #endregion

        private List<SnapshotValue> list = new List<SnapshotValue>();
    }
}
