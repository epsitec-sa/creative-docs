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
using System.Linq;
using System.Text;

namespace Epsitec.Common.Widgets.Collections
{
    /// <summary>
    /// La classe FlatChildrenCollection stocke un ensemble de widgets de manière
    /// ordonnée.
    /// </summary>
    public class FlatChildrenCollection
        : IList<Visual>,
            ICollection<Types.DependencyObject>,
            System.Collections.ICollection
    {
        internal FlatChildrenCollection(Visual host)
        {
            this.host = host;
            this.visuals = new List<Visual>();
            this.id = FlatChildrenCollection.idSource++;
        }

        public Widget[] Widgets
        {
            get
            {
                Visual[] visuals = this.Visuals.ToArray();
                Widget[] widgets = new Widget[visuals.Length];
                visuals.CopyTo(widgets, 0);
                return widgets;
            }
        }

        public int AnchorLayoutCount
        {
            get
            {
                this.RefreshLayoutStatistics();
                return this.anchorLayoutCount;
            }
        }
        public int DockLayoutCount
        {
            get
            {
                this.RefreshLayoutStatistics();
                return this.dockLayoutCount;
            }
        }
        public int StackLayoutCount
        {
            get
            {
                this.RefreshLayoutStatistics();
                return this.stackLayoutCount;
            }
        }
        public int GridLayoutCount
        {
            get
            {
                this.RefreshLayoutStatistics();
                return this.gridLayoutCount;
            }
        }

        public Visual FindNext(Visual find)
        {
            int index = this.Visuals.IndexOf(find);

            if ((index < 0) || (index > this.Visuals.Count - 2))
            {
                return null;
            }
            else
            {
                return this.Visuals[index + 1];
            }
        }

        public Visual FindPrevious(Visual find)
        {
            int index = this.Visuals.IndexOf(find);

            if (index < 1)
            {
                return null;
            }
            else
            {
                return this.Visuals[index - 1];
            }
        }

        public int ZOrderOf(Visual visual)
        {
            int index = this.IndexOf(visual);

            if (index < 0)
            {
                return -1;
            }

            return this.Count - index - 1;
        }

        public void ChangeZOrder(Visual visual, int z)
        {
            if (this.Contains(visual) == false)
            {
                throw new System.ArgumentException(
                    "Cannot change Z order of visual; it does not belong to this children collection"
                );
            }
            z = System.Math.Max(0, System.Math.Min(this.Count - 1, z));

            int newIndex = this.Count - z - 1;
            int oldIndex = this.IndexOf(visual);

            if (oldIndex != newIndex)
            {
                this.Visuals.RemoveAt(oldIndex);
                this.Visuals.Insert(newIndex, visual);

                Visual parent = this.host;

                if (parent != null)
                {
                    Layouts.LayoutContext.AddToMeasureQueue(parent);
                    Layouts.LayoutContext.AddToArrangeQueue(parent);
                }
            }
        }

        public void AddRange(IEnumerable<Visual> collection)
        {
            if (collection != null)
            {
                Snapshot snapshot = Snapshot.RecordTree(collection);

                this.Visuals.AddRange(collection);

                foreach (Visual item in collection)
                {
                    this.AttachVisual(item);
                }

                this.NotifyChanges(snapshot);
            }
        }

        public void ReplaceAll(IEnumerable<Visual> collection)
        {
            this.Change(x => collection);
        }

        public void Change(System.Func<IEnumerable<Visual>, IEnumerable<Visual>> changeFunction)
        {
            var oldItems = this.Visuals.ToArray();
            var newItems = changeFunction(this.Visuals).ToArray();

            var delta = new HashSet<Visual>(oldItems);
            delta.SymmetricExceptWith(newItems);

            Snapshot snapshot = Snapshot.RecordTree(delta);

            this.Visuals.Clear();
            this.Visuals.AddRange(newItems);

            foreach (Visual visual in oldItems.Except(newItems))
            {
                this.DetachVisual(visual);
            }
            foreach (Visual visual in newItems.Except(oldItems))
            {
                this.AttachVisual(visual);
            }

            this.NotifyChanges(snapshot);
        }

        internal void RefreshLayoutStatistics()
        {
            this.dockLayoutCount = 0;
            this.anchorLayoutCount = 0;
            this.stackLayoutCount = 0;
            this.gridLayoutCount = 0;

            foreach (Visual visual in this.Visuals)
            {
                switch (Layouts.LayoutEngine.GetLayoutMode(visual))
                {
                    case Layouts.LayoutMode.Docked:
                        this.dockLayoutCount++;
                        break;

                    case Layouts.LayoutMode.Anchored:
                        this.anchorLayoutCount++;
                        break;

                    case Layouts.LayoutMode.Stacked:
                        this.stackLayoutCount++;
                        break;

                    case Layouts.LayoutMode.Grid:
                        this.gridLayoutCount++;
                        break;
                }
            }
        }

        private void NotifyChanges(Snapshot snapshot)
        {
            snapshot.NotifyChanges();
        }

        private void AttachVisual(Visual visual)
        {
            //	Attache le visual à son nouveau parent; il est au préalable détaché
            //	de son ancien parent. Cette méthode ne s'occupe pas de la question
            //	des propriétés héritées.

            System.Diagnostics.Debug.Assert(visual != null);
            System.Diagnostics.Debug.Assert(this.Visuals.Contains(visual));

            Visual parent = visual.Parent;

            if (parent == null)
            {
                //	Le visual n'a pas de parent, ce qui simplifie la gestion. Il
                //	suffit de lui en attribuer un :

                visual.SetParentVisual(this.host);
                visual.InheritedPropertyCache.InheritValuesFromParent(visual, this.host);
            }
            else if (this.host == parent)
            {
                //	Le visual a déjà le même parent; il n'y a donc rien à faire
                //	au niveau des liens.
            }
            else
            {
                //	Le visual est encore attaché à un parent. Il faut commencer par
                //	le détacher de son ancien parent, puis notifier l'ancien parent
                //	du changement :

                FlatChildrenCollection others = parent.Children;

                others.Visuals.Remove(visual);
                others.DetachVisual(visual);

                System.Diagnostics.Debug.Assert(visual.Parent == null);

                visual.SetParentVisual(this.host);
                visual.InheritedPropertyCache.InheritValuesFromParent(visual, this.host);
            }

            System.Diagnostics.Debug.Assert(visual.Parent == this.host);

            this.NotifyChanged();
        }

        private void DetachVisual(Visual visual)
        {
            //	Détache le visual de son parent.

            System.Diagnostics.Debug.Assert(visual != null);
            System.Diagnostics.Debug.Assert(this.Visuals.Contains(visual) == false);
            System.Diagnostics.Debug.Assert(this.host == visual.Parent);

            visual.SetParentVisual(null);
            visual.InheritedPropertyCache.ClearAllValues(visual);

            System.Diagnostics.Debug.Assert(visual.Parent == null);

            this.NotifyChanged();
        }

        private void NotifyChanged()
        {
            this.host.NotifyChildrenChanged();
        }

        #region IList<Visual> Members

        public Visual this[int index]
        {
            get { return this.Visuals[index]; }
            set
            {
                if (value == null)
                {
                    throw new System.ArgumentNullException(
                        FlatChildrenCollection.NullVisualMessage
                    );
                }
                Visual oldValue = this.Visuals[index];
                Visual newValue = value;

                if (oldValue != newValue)
                {
                    if (value.Parent == this.host)
                    {
                        throw new System.InvalidOperationException(
                            FlatChildrenCollection.NotTwiceMessage
                        );
                    }

                    Snapshot snapshot = Snapshot.RecordTree(oldValue, newValue);

                    this.Visuals[index] = null;
                    this.DetachVisual(oldValue);
                    this.Visuals[index] = value;
                    this.AttachVisual(newValue);

                    this.NotifyChanges(snapshot);
                }
            }
        }

        public int IndexOf(Visual item)
        {
            return this.Visuals.IndexOf(item);
        }

        public void Insert(int index, Visual item)
        {
            if (item == null)
            {
                throw new System.ArgumentNullException(FlatChildrenCollection.NullVisualMessage);
            }
            if (item.Parent == this.host)
            {
                throw new System.InvalidOperationException(FlatChildrenCollection.NotTwiceMessage);
            }

            Snapshot snapshot = Snapshot.RecordTree(item);

            this.Visuals.Insert(index, item);
            this.AttachVisual(item);

            this.NotifyChanges(snapshot);
        }

        public void RemoveAt(int index)
        {
            Visual item = this.Visuals[index];

            Snapshot snapshot = Snapshot.RecordTree(item);

            this.Visuals.RemoveAt(index);
            this.DetachVisual(item);

            this.NotifyChanges(snapshot);
        }

        #endregion

        #region ICollection<Visual> Members

        public int Count
        {
            get { return this.Visuals.Count; }
        }
        public bool IsReadOnly
        {
            get { return false; }
        }

        public void Add(Visual item)
        {
            if (item == null)
            {
                throw new System.ArgumentNullException(FlatChildrenCollection.NullVisualMessage);
            }
            if (item.Parent == this.host)
            {
                //	If the caller tries to add a visual to the same parent, we
                //	simply accept it; don't do anything here...

                return;
            }
            Snapshot snapshot = Snapshot.RecordTree(item);

            this.Visuals.Add(item);
            this.AttachVisual(item);

            this.NotifyChanges(snapshot);
        }

        public bool Remove(Visual item)
        {
            if (item == null)
            {
                throw new System.ArgumentNullException(FlatChildrenCollection.NullVisualMessage);
            }

            Snapshot snapshot = Snapshot.RecordTree(item);

            if (this.Visuals.Remove(item))
            {
                this.DetachVisual(item);
                this.NotifyChanges(snapshot);

                Layouts.LayoutContext.AddToArrangeQueue(this.host);

                return true;
            }
            else
            {
                return false;
            }
        }

        public void Clear()
        {
            if (this.Visuals.Count > 0)
            {
                Visual[] copy = this.Visuals.ToArray();
                Snapshot snapshot = Snapshot.RecordTree(copy);

                this.Visuals.Clear();

                foreach (Visual visual in copy)
                {
                    this.DetachVisual(visual);
                }

                this.NotifyChanges(snapshot);
            }
        }

        public bool Contains(Visual item)
        {
            return this.Visuals.Contains(item);
        }

        public void CopyTo(Visual[] array, int index)
        {
            this.Visuals.CopyTo(array, index);
        }

        #endregion

        #region ICollection<Types.DependencyObject> Members

        public void Add(Types.DependencyObject item)
        {
            this.Add(item as Visual);
        }

        public bool Contains(Types.DependencyObject item)
        {
            return this.Contains(item as Visual);
        }

        public void CopyTo(Types.DependencyObject[] array, int index)
        {
            Visual[] temp = this.Visuals.ToArray();
            temp.CopyTo(array, index);
        }

        public bool Remove(Types.DependencyObject item)
        {
            return this.Remove(item as Visual);
        }

        #endregion

        #region IEnumerable<Visual> Members

        public IEnumerator<Visual> GetEnumerator()
        {
            return this.Visuals.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        #endregion

        #region IEnumerable<Types.DependencyObject> Members

        IEnumerator<Types.DependencyObject> IEnumerable<Types.DependencyObject>.GetEnumerator()
        {
            foreach (Visual item in this.Visuals)
            {
                yield return item;
            }
        }

        #endregion

        #region ICollection Members

        void System.Collections.ICollection.CopyTo(System.Array array, int index)
        {
            System.Collections.ICollection collection = this.Visuals;
            collection.CopyTo(array, index);
        }

        int System.Collections.ICollection.Count
        {
            get { return this.Count; }
        }

        bool System.Collections.ICollection.IsSynchronized
        {
            get { return false; }
        }

        object System.Collections.ICollection.SyncRoot
        {
            get { return this.Visuals; }
        }

        #endregion

        #region Shapshot Class

        class Snapshot
        {
            private Snapshot()
            {
                this.snapshot = new Types.DependencyObjectTreeSnapshot();
                this.visuals = new List<Visual>();
            }

            public void NotifyChanges()
            {
                //	Maintenant que tous les visuals ont leur parent définitif, il
                //	faut tous les passer en revue pour déterminer les changements
                //	de propriétés héritées.

                foreach (Visual visual in this.visuals)
                {
                    visual.InheritedPropertyCache.NotifyChanges(visual);
                }

                this.snapshot.InvalidateDifferentProperties();
            }

            public static Snapshot RecordTree(Visual visual)
            {
                Snapshot snapshot = new Snapshot();
                snapshot.Add(visual);
                return snapshot;
            }

            public static Snapshot RecordTree(params Visual[] visuals)
            {
                IEnumerable<Visual> collection = visuals;
                return Snapshot.RecordTree(collection);
            }

            public static Snapshot RecordTree(IEnumerable<Visual> collection)
            {
                Snapshot snapshot = new Snapshot();
                foreach (Visual item in collection)
                {
                    snapshot.Add(item);
                }
                return snapshot;
            }

            private void Add(Visual visual)
            {
                if (this.visuals.Contains(visual))
                {
                    //	Rien à faire, car ce visual est déjà connu et a déjà été analysé.
                }
                else
                {
                    this.visuals.Add(visual);

                    this.snapshot.Record(visual, Visual.ParentProperty);
                }
            }

            Types.DependencyObjectTreeSnapshot snapshot;
            List<Visual> visuals;
        }

        #endregion

        public List<Visual> Visuals
        {
            get
            {
                //this.DebugShow();
                return this.visuals;
            }
        }

        private const string NullVisualMessage = "Visual children may not be null";
        private const string NotTwiceMessage = "Visual may not be inserted twice";

        private readonly Visual host;
        private readonly List<Visual> visuals;

        private int dockLayoutCount;
        private int anchorLayoutCount;
        private int stackLayoutCount;
        private int gridLayoutCount;

        private readonly int id;
        private static int idSource;
    }
}
