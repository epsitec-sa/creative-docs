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

namespace Epsitec.Common.Text.Wrappers
{
    /// <summary>
    /// ... StateProperty.
    /// </summary>
    public sealed class StateProperty
    {
        public StateProperty(System.Type state_type, string name, object default_value)
        {
            this.name = name;
            this.default_value = default_value;

            StateProperty.Register(state_type, this);
        }

        public string Name
        {
            get { return this.name; }
        }

        public object DefaultValue
        {
            get { return this.default_value; }
        }

        public static IEnumerable<StateProperty> GetProperties(System.Type state_type)
        {
            List<StateProperty> list;

            if (StateProperty.types.TryGetValue(state_type, out list))
            {
                return list;
            }
            else
            {
                return new StateProperty[0];
            }
        }

        public override int GetHashCode()
        {
            return this.name.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            StateProperty that = obj as StateProperty;

            if (that == null)
            {
                return false;
            }

            return (that.name == this.name);
        }

        private static void Register(System.Type state_type, StateProperty property)
        {
            List<StateProperty> list;

            if (StateProperty.types.TryGetValue(state_type, out list) == false)
            {
                list = new List<StateProperty>();
                StateProperty.types[state_type] = list;
            }

            foreach (StateProperty find in list)
            {
                if (find.Name == property.Name)
                {
                    throw new System.InvalidOperationException(
                        string.Format(
                            "Property {0} already defined for type {1}",
                            property.Name,
                            state_type.Name
                        )
                    );
                }
            }

            list.Add(property);
        }

        static Dictionary<System.Type, List<StateProperty>> types =
            new Dictionary<System.Type, List<StateProperty>>();

        private readonly string name;
        private readonly object default_value;
    }
}
