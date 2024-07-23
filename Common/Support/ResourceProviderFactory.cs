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

namespace Epsitec.Common.Support
{
    /// <summary>
    /// The <c>ResourceProviderFactory</c> class loads the resource provider implementation
    /// from an external assembly and publishes the available <see cref="IResourceProvider"/>
    /// allocators.
    /// </summary>
    public sealed class ResourceProviderFactory
    {
        internal ResourceProviderFactory()
        {
            this.LoadImplementation();
        }

        public IEnumerable<Allocator<IResourceProvider, ResourceManager>> Allocators
        {
            get { return this.allocators; }
        }

        private void LoadImplementation()
        {
            System.Type[] constructorArgumentTypes = new System.Type[] { typeof(ResourceManager) };
            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetAssembly(
                typeof(ResourceManager)
            );

            System.Diagnostics.Debug.Assert(assembly != null);

            foreach (var type in assembly.GetTypes())
            {
                if ((type.IsClass) && (!type.IsAbstract))
                {
                    if (
                        (type.GetInterface("IResourceProvider") != null)
                        && (type.GetConstructor(constructorArgumentTypes) != null)
                    )
                    {
                        this.allocators.Add(
                            DynamicCodeFactory.CreateAllocator<IResourceProvider, ResourceManager>(
                                type
                            )
                        );
                    }
                }
            }
        }

        private readonly List<Allocator<IResourceProvider, ResourceManager>> allocators =
            new List<Allocator<IResourceProvider, ResourceManager>>();
    }
}
