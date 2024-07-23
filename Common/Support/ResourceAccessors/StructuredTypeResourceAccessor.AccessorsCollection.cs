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

[assembly: Epsitec.Common.Types.DependencyClass(
    typeof(Epsitec.Common.Support.ResourceAccessors.StructuredTypeResourceAccessor)
)]

namespace Epsitec.Common.Support.ResourceAccessors
{
    public partial class StructuredTypeResourceAccessor : CaptionResourceAccessor
    {
        /// <summary>
        /// The <c>AccessorsCollection</c> maintains a collection of <see cref="StructuredTypeReourceAccessor"/>
        /// instances.
        /// </summary>
        private sealed class AccessorsCollection
        {
            public AccessorsCollection()
            {
                this.list = new List<Weak<StructuredTypeResourceAccessor>>();
            }

            public void Add(StructuredTypeResourceAccessor item)
            {
                this.list.Add(new Weak<StructuredTypeResourceAccessor>(item));
            }

            public void Remove(StructuredTypeResourceAccessor item)
            {
                this.list.RemoveAll(
                    delegate(Weak<StructuredTypeResourceAccessor> probe)
                    {
                        if (probe.IsAlive)
                        {
                            return probe.Target == item;
                        }
                        else
                        {
                            return true;
                        }
                    }
                );
            }

            public IEnumerable<StructuredTypeResourceAccessor> Collection
            {
                get
                {
                    foreach (Weak<StructuredTypeResourceAccessor> item in this.list)
                    {
                        StructuredTypeResourceAccessor accessor = item.Target;

                        if (accessor != null)
                        {
                            yield return accessor;
                        }
                    }
                }
            }

            private List<Weak<StructuredTypeResourceAccessor>> list;
        }
    }
}
