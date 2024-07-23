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


using Epsitec.Common.Types;
using System.Collections.Generic;

namespace Epsitec.Common.Widgets.Helpers
{
    /// <summary>
    /// La classe GroupController permet de gérer des groupes de widgets.
    /// </summary>
    public sealed class GroupController : DependencyObject, INotifyChanged
    {
        public GroupController(Widget parent, string group)
        {
            this.parent = parent;
            this.group = group;
        }

        public int ActiveIndex
        {
            get { return this.index; }
            set
            {
                if (this.index != value)
                {
                    //	On active le widget correspondant à l'index spécifié, ce
                    //	qui va appeler notre méthode SetActiveIndex pour mettre à
                    //	jour l'index réellement actif.

                    Widget widget = this.FindWidget(this.index);

                    if (widget != null)
                    {
                        widget.ActiveState = ActiveState.Yes;
                    }
                }
            }
        }

        public string Group
        {
            get { return this.group; }
        }

        public Widget[] Widgets
        {
            get { return Collection.ToArray(this.FindWidgets()); }
        }

        public static GroupController GetGroupController(Widget widget)
        {
            System.Diagnostics.Debug.Assert(widget.AutoRadio);
            System.Diagnostics.Debug.Assert(widget.Parent != null);
            System.Diagnostics.Debug.Assert(widget.Group != null);
            System.Diagnostics.Debug.Assert(widget.Group.Length > 0);

            return GroupController.GetGroupController(widget.Parent, widget.Group);
        }

        public static GroupController GetGroupController(Widget parent, string group)
        {
            if ((parent != null) && (group != null) && (group.Length > 0))
            {
                //	Trouve le contrôleur du groupe, lequel est en principe accessible depuis
                //	le parent. S'il n'existe pas pour ce groupe, on le crée :

                GroupControllerCollection collection =
                    parent.GetValue(GroupController.ControllerCollectionProperty)
                    as GroupControllerCollection;

                if (collection == null)
                {
                    collection = new GroupControllerCollection(parent);
                    parent.SetValue(GroupController.ControllerCollectionProperty, collection);
                }

                return collection.GetController(group);
            }

            return null;
        }

        public void TurnOffAllButOne(Widget keep)
        {
            //	Eteint tous les boutons radio du groupe, sauf celui spécifié par
            //	l'argument 'keep'.

            foreach (Widget radio in this.FindWidgets())
            {
                if ((radio != keep) && (radio.ActiveState != ActiveState.No))
                {
                    radio.ActiveState = ActiveState.No;
                }
            }
        }

        public IEnumerable<Widget> FindWidgets()
        {
            //	Trouve tous les boutons radio qui appartiennent à notre groupe.

            if (this.parent == null)
            {
                yield break;
            }

            foreach (Widget widget in this.parent.FindAllChildren())
            {
                if (widget.Group == this.group)
                {
                    yield return widget;
                }
            }
        }

        public Widget FindWidget(int index)
        {
            foreach (Widget widget in this.parent.FindAllChildren())
            {
                if ((widget.AutoRadio) && (widget.Group == this.group) && (widget.Index == index))
                {
                    return widget;
                }
            }

            return null;
        }

        public Widget FindActiveWidget()
        {
            foreach (Widget widget in this.parent.FindAllChildren())
            {
                if (
                    (widget.AutoRadio)
                    && (widget.Group == this.group)
                    && (widget.ActiveState == ActiveState.Yes)
                )
                {
                    return widget;
                }
            }

            return null;
        }

        public Widget FindXWidget(Widget widget, int direction)
        {
            System.Diagnostics.Debug.Assert((direction == 1) || (direction == -1));

            int index = widget.Index + direction;

            while (index >= 0)
            {
                widget = this.FindWidget(index);

                if (widget == null)
                {
                    break;
                }
                if (widget.IsVisible)
                {
                    return widget;
                }

                index += direction;
            }

            return null;
        }

        public Widget FindYWidget(Widget widget, int direction)
        {
            System.Diagnostics.Debug.Assert((direction == 1) || (direction == -1));

            int index = widget.Index + direction * 1000;

            while (index >= 0)
            {
                widget = this.FindWidget(index);

                if (widget == null)
                {
                    break;
                }
                if (widget.IsVisible)
                {
                    return widget;
                }

                index += direction * 1000;
            }

            return null;
        }

        internal void SetActiveIndex(int value)
        {
            if (this.index != value)
            {
                this.index = value;
                this.OnChanged();
            }
        }

        private void OnChanged()
        {
            if (this.Changed != null)
            {
                this.Changed(this);
            }
        }

        #region INotifyChanged Members
        public event Support.EventHandler Changed;
        #endregion

        #region GroupControllerCollection Class

        private class GroupControllerCollection
        {
            public GroupControllerCollection(Widget parent)
            {
                this.parent = parent;
            }

            public GroupController GetController(string group)
            {
                GroupController controller;

                if (this.dict.TryGetValue(group, out controller) == false)
                {
                    controller = new GroupController(this.parent, group);

                    Widget active = controller.FindActiveWidget();

                    controller.index = (active == null) ? -1 : active.Index;

                    this.dict[group] = controller;
                }

                return controller;
            }

            private Widget parent;
            private Dictionary<string, GroupController> dict =
                new Dictionary<string, GroupController>();
        }

        #endregion

        private static readonly DependencyProperty ControllerCollectionProperty =
            DependencyProperty.RegisterAttached(
                "ControllerCollection",
                typeof(GroupControllerCollection),
                typeof(GroupController),
                new DependencyPropertyMetadata().MakeNotSerializable()
            );

        private readonly Widget parent;
        private readonly string group;
        private int index;
    }
}
