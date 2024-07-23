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


namespace Epsitec.Common.Widgets.Collections
{
    /// <summary>
    /// L'interface IVisualCollectionHost définit des méthodes de notification
    /// appelées par la classe VisualCollection. Cette manière de faire est bien
    /// plus efficace que des événements pour signaler des changements.
    /// </summary>
    public interface IVisualCollectionHost
    {
        void NotifyVisualCollectionBeforeInsertion(VisualCollection collection, Visual visual);
        void NotifyVisualCollectionAfterInsertion(VisualCollection collection, Visual visual);
        void NotifyVisualCollectionBeforeRemoval(VisualCollection collection, Visual visual);
        void NotifyVisualCollectionAfterRemoval(VisualCollection collection, Visual visual);
        void NotifyVisualCollectionChanged(VisualCollection collection);
    }
}
