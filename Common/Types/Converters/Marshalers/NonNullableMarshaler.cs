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


using System.Linq.Expressions;

namespace Epsitec.Common.Types.Converters.Marshalers
{
    public sealed class NonNullableMarshaler<T> : GenericMarshaler<T, T>
    {
        public NonNullableMarshaler() { }

        public NonNullableMarshaler(
            System.Func<T> getter,
            System.Action<T> setter,
            Expression expression
        )
        {
            this.ValueGetter = getter;
            this.ValueSetter = setter;
            this.InitialValue = getter();
            this.ValueGetterExpression = expression;
        }

        public override string GetStringValue()
        {
            var value = this.GetValue();
            return this.Converter.ConvertToString(value);
        }

        public override void SetStringValue(string text)
        {
            var value = this.Converter.ConvertFromString(text);

            if (value.HasValue)
            {
                this.SetValue(value.Value);
            }
            else
            {
                this.SetValue(this.InitialValue);
            }
        }

        public override bool CanConvert(string text)
        {
            return this.Converter.CanConvertFromString(text);
        }
    }
}
