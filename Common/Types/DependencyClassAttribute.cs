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
    [System.AttributeUsage(
        System.AttributeTargets.Assembly,
        /* */AllowMultiple = true
    )]
    public class DependencyClassAttribute : System.Attribute
    {
        public DependencyClassAttribute(System.Type type)
        {
            this.type = type;
        }

        public System.Type Type
        {
            get { return this.type; }
        }

        /// <summary>
        /// Gets or sets the converter associated with the class.
        /// </summary>
        /// <value>The type of the converter to use for serialization conversions.</value>
        public System.Type Converter
        {
            get { return this.converter; }
            set { this.converter = value; }
        }

        public static IEnumerable<System.Type> GetRegisteredTypes(
            System.Reflection.Assembly assembly
        )
        {
            foreach (
                DependencyClassAttribute attribute in assembly.GetCustomAttributes(
                    typeof(DependencyClassAttribute),
                    false
                )
            )
            {
                yield return attribute.Type;
            }
        }

        public static IEnumerable<DependencyClassAttribute> GetConverterAttributes(
            System.Reflection.Assembly assembly
        )
        {
            foreach (
                DependencyClassAttribute attribute in assembly.GetCustomAttributes(
                    typeof(DependencyClassAttribute),
                    false
                )
            )
            {
                if (attribute.Converter != null)
                {
                    yield return attribute;
                }
            }
        }

        private readonly System.Type type;
        private System.Type converter;
    }
}
