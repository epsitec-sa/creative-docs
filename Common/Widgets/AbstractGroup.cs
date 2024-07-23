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

namespace Epsitec.Common.Widgets
{
    /// <summary>
    /// La classe AbstractGroup sert de base aux autres classes qui
    /// implémentent des groupes de widgets.
    /// </summary>
    public abstract class AbstractGroup : Widget
    {
        public AbstractGroup()
        {
            this.InternalState |= WidgetInternalState.PossibleContainer;
            this.InternalState |= WidgetInternalState.Focusable;

            base.TabNavigationMode = TabNavigationMode.ForwardTabPassive;
        }

        public AbstractGroup(Widget embedder)
            : this()
        {
            this.SetEmbedder(embedder);
        }

        public override TabNavigationMode DefaultTabNavigationMode
        {
            get { return TabNavigationMode.ForwardTabPassive; }
        }

        public override TabNavigationMode TabNavigationMode
        {
            get { return base.TabNavigationMode; }
            set
            {
                //	Un groupe transmet toujours les pressions de TAB à ses enfants
                //	sans les consommer lui-même; c'est pourquoi on rajoute les deux
                //	bits ci-dessous :

                if (value == TabNavigationMode.None)
                {
                    this.SetTabNavigation(value);
                }
                else
                {
                    this.SetTabNavigation(
                        value | TabNavigationMode.ForwardToChildren | TabNavigationMode.ForwardOnly
                    );
                }
            }
        }

        public void SetTabNavigation(TabNavigationMode value)
        {
            //	Comme this.TabNavigation, mais sans aucun garde-fou.

            base.TabNavigationMode = value;
        }

        #region ChildrenLayoutMode
        public Layouts.LayoutMode ChildrenLayoutMode
        {
            get
            {
                return (Layouts.LayoutMode)this.GetValue(AbstractGroup.ChildrenLayoutModeProperty);
            }
            set { this.SetValue(AbstractGroup.ChildrenLayoutModeProperty, value); }
        }

        public static Layouts.LayoutMode GetChildrenLayoutMode(DependencyObject o)
        {
            return (Layouts.LayoutMode)o.GetValue(AbstractGroup.ChildrenLayoutModeProperty);
        }

        public static void SetChildrenLayoutMode(DependencyObject o, Layouts.LayoutMode value)
        {
            o.SetValue(AbstractGroup.ChildrenLayoutModeProperty, value);
        }

        public static readonly DependencyProperty ChildrenLayoutModeProperty =
            DependencyProperty.Register(
                "ChildrenLayoutMode",
                typeof(Layouts.LayoutMode),
                typeof(AbstractGroup),
                new DependencyPropertyMetadata(Layouts.LayoutMode.None)
            );
        #endregion
    }
}
