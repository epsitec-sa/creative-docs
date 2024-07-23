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


namespace Epsitec.Common.Drawing.Serializers
{
    public class TransformSerializer : AbstractSerializer
    {
        public TransformSerializer(int resolution = 2)
            : base(resolution) { }

        public string Serialize(Transform transform)
        {
            var buffer = new System.Text.StringBuilder();

            buffer.Append(this.Serialize(transform.XX));
            buffer.Append(" ");
            buffer.Append(this.Serialize(transform.XY));
            buffer.Append(" ");
            buffer.Append(this.Serialize(transform.YX));
            buffer.Append(" ");
            buffer.Append(this.Serialize(transform.YY));
            buffer.Append(" ");
            buffer.Append(this.Serialize(transform.TX));
            buffer.Append(" ");
            buffer.Append(this.Serialize(transform.TY));

            return buffer.ToString();
        }

        public static Transform Parse(string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                var list = value.Split(' ');

                if (list.Length == 6)
                {
                    double xx,
                        xy,
                        yx,
                        yy,
                        tx,
                        ty;

                    if (
                        (double.TryParse(list[0], out xx))
                        && (double.TryParse(list[1], out xy))
                        && (double.TryParse(list[2], out yx))
                        && (double.TryParse(list[3], out yy))
                        && (double.TryParse(list[4], out tx))
                        && (double.TryParse(list[5], out ty))
                    )
                    {
                        return new Transform(xx, xy, yx, yy, tx, ty);
                    }

                    throw new System.FormatException("Invalid transform format");
                }
            }

            return Transform.Identity;
        }
    }
}
