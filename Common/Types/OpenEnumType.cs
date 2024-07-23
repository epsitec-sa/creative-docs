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


[assembly: Epsitec.Common.Types.DependencyClass(typeof(Epsitec.Common.Types.OpenEnumType))]

namespace Epsitec.Common.Types
{
    /// <summary>
    /// La classe OpenEnumType décrit une énumération, basée sur une enum
    /// native, mais avec la possibilité d'accepter d'autres valeurs. En ce
    /// sens, l'énumération est "ouverte".
    /// Les noms des éléments ajoutés à l'énumération native sont décorés par
    /// une paire d'accolades; par exemple "{Xyz}"
    /// </summary>
    public class OpenEnumType : EnumType
    {
        public OpenEnumType(System.Type enumType)
            : base(enumType) { }

        public OpenEnumType(System.Type enumType, IDataConstraint constraint)
            : this(enumType)
        {
            this.constraint = constraint;
        }

        public IDataConstraint Constraint
        {
            get { return this.constraint; }
        }

        public override bool IsCustomizable
        {
            get { return true; }
        }

        public override System.Type SystemType
        {
            get { return typeof(string); }
        }

        public override bool IsValidValue(object value)
        {
            if (this.IsNullValue(value))
            {
                return this.IsNullable;
            }

            if ((OpenEnumType.IsCustomName(value as string)) || (base.IsValidValue(value)))
            {
                if (this.constraint != null)
                {
                    return this.constraint.IsValidValue(value);
                }

                return true;
            }

            return false;
        }

        public static bool IsCustomName(string value)
        {
            if (
                (value != null)
                && (value.Length > 1)
                && (value[0] == '{')
                && (value[value.Length - 1] == '}')
            )
            {
                return true;
            }

            return false;
        }

        public static string ToCustomName(string value)
        {
            if (value != null)
            {
                return string.Concat("{", value, "}");
            }

            return "{}";
        }

        public static string FromCustomName(string value)
        {
            if (OpenEnumType.IsCustomName(value))
            {
                return value.Substring(1, value.Length - 2);
            }

            throw new System.ArgumentException(
                string.Format("Specified name ({0}) is not a valid custom name", value),
                "value"
            );
        }

        private IDataConstraint constraint;
    }
}
