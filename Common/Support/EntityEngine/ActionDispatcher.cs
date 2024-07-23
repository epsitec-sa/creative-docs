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


using Epsitec.Common.Support.Extensions;
using System.Collections.Generic;
using System.Reflection;

namespace Epsitec.Common.Support.EntityEngine
{
    /// <summary>
    /// The <c>ActionDispatcher</c> class is used to analyze a class and provide access
    /// to the published actions (methods decorated with the <c>[Action]</c> attribute)
    /// as a collection of <see cref="ActionInfo"/> instances.
    /// </summary>
    public static class ActionDispatcher
    {
        public static IList<ActionInfo> GetActionInfos(System.Type type)
        {
            lock (ActionDispatcher.cache)
            {
                ActionInfos infos;

                if (ActionDispatcher.cache.TryGetValue(type, out infos))
                {
                    return infos;
                }

                infos = new ActionInfos(ActionDispatcher.CreateActionInfos(type));

                ActionDispatcher.cache[type] = infos;

                return infos;
            }
        }

        private static IEnumerable<ActionInfo> CreateActionInfos(System.Type type)
        {
            var methods = type.GetMethods(
                System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public
            );

            foreach (var method in methods)
            {
                var parameters = method.GetParameters();

                if (parameters.Length == 0)
                {
                    foreach (var attribute in method.GetCustomAttributes<ActionAttribute>(true))
                    {
                        yield return ActionDispatcher.CreateActionInfo(method, attribute);
                    }
                }
            }
        }

        private static ActionInfo CreateActionInfo(
            System.Reflection.MethodInfo method,
            ActionAttribute attribute
        )
        {
            return new ActionInfo(attribute, ActionDispatcher.CreateAction(method));
        }

        private static System.Action<AbstractEntity> CreateAction(
            System.Reflection.MethodInfo method
        )
        {
            return x => ActionDispatcher.InvokeAction(x, method);
        }

        private static void InvokeAction(AbstractEntity entity, System.Reflection.MethodInfo method)
        {
            method.Invoke(entity, ActionDispatcher.EmptyParameters);
        }

        #region ActionInfos Class

        private class ActionInfos : IList<ActionInfo>
        {
            public ActionInfos(IEnumerable<ActionInfo> collection)
            {
                this.list = new List<ActionInfo>(collection);
            }

            #region IList<ActionInfo> Members

            public int IndexOf(ActionInfo item)
            {
                return this.list.IndexOf(item);
            }

            public void Insert(int index, ActionInfo item)
            {
                throw new System.InvalidOperationException();
            }

            public void RemoveAt(int index)
            {
                throw new System.InvalidOperationException();
            }

            public ActionInfo this[int index]
            {
                get { return this.list[index]; }
                set { throw new System.InvalidOperationException(); }
            }

            #endregion

            #region ICollection<ActionInfo> Members

            public void Add(ActionInfo item)
            {
                throw new System.InvalidOperationException();
            }

            public void Clear()
            {
                throw new System.InvalidOperationException();
            }

            public bool Contains(ActionInfo item)
            {
                return this.list.Contains(item);
            }

            public void CopyTo(ActionInfo[] array, int arrayIndex)
            {
                this.list.CopyTo(array, arrayIndex);
            }

            public int Count
            {
                get { return this.list.Count; }
            }

            public bool IsReadOnly
            {
                get { return true; }
            }

            public bool Remove(ActionInfo item)
            {
                throw new System.InvalidOperationException();
            }

            #endregion

            #region IEnumerable<ActionInfo> Members

            public IEnumerator<ActionInfo> GetEnumerator()
            {
                return this.list.GetEnumerator();
            }

            #endregion

            #region IEnumerable Members

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return this.list.GetEnumerator();
            }

            #endregion

            private readonly List<ActionInfo> list;
        }

        #endregion

        private static readonly object[] EmptyParameters = new object[0];

        private static readonly Dictionary<System.Type, ActionInfos> cache =
            new Dictionary<System.Type, ActionInfos>();
    }
}
