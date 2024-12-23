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


[assembly: Epsitec.Common.Types.DependencyClass(typeof(Epsitec.Common.Types.BinaryType))]

namespace Epsitec.Common.Types
{
    /// <summary>
    /// The <c>BinaryType</c> class describes data stored as arrays of <c>byte</c>.
    /// </summary>
    public class BinaryType : AbstractType
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BinaryType"/> class.
        /// </summary>
        public BinaryType()
            : base("Binary") { }

        /// <summary>
        /// Initializes a new instance of the <see cref="BinaryType"/> class.
        /// </summary>
        /// <param name="caption">The type caption.</param>
        public BinaryType(Caption caption)
            : base(caption) { }

        /// <summary>
        /// Gets the type code for the type.
        /// </summary>
        /// <value>The type code.</value>
        public override TypeCode TypeCode
        {
            get { return TypeCode.Binary; }
        }

        /// <summary>
        /// Gets the MIME type (or types) associated with this binary type.
        /// </summary>
        /// <value>The MIME types separated by <c>";"</c>.</value>
        public string MimeType
        {
            get { return (string)this.Caption.GetValue(BinaryType.MimeTypeProperty); }
        }

        public static BinaryType Default
        {
            get
            {
                TypeRosetta.InitializeKnownTypes();

                if (BinaryType.defaultValue == null)
                {
                    //	TODO: fix DRUID

                    BinaryType.defaultValue = (BinaryType)
                        TypeRosetta.CreateTypeObject(Support.Druid.Parse("[xxxx]"));
                }

                return BinaryType.defaultValue;
            }
        }

        #region ISystemType Members

        /// <summary>
        /// Gets the system type described by this object.
        /// </summary>
        /// <value>The system type described by this object.</value>
        public override System.Type SystemType
        {
            get { return typeof(byte[]); }
        }

        #endregion

        #region IDataConstraint Members

        /// <summary>
        /// Determines whether the specified value is valid according to the
        /// constraint.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        /// 	<c>true</c> if the specified value is valid; otherwise, <c>false</c>.
        /// </returns>
        public override bool IsValidValue(object value)
        {
            if (this.IsNullValue(value))
            {
                return this.IsNullable;
            }

            if (value is byte[])
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        #endregion

        /// <summary>
        /// Defines the MIME type (or types) for this binary type.
        /// </summary>
        /// <param name="value">The MIME types separated by <c>";"</c>.</param>
        public void DefineMimeType(string value)
        {
            if ((string.IsNullOrEmpty(value)) || (value.Trim().Length == 0))
            {
                this.Caption.ClearValue(BinaryType.MimeTypeProperty);
            }
            else
            {
                if (value.Replace(";", "").Trim().Length == 0)
                {
                    throw new System.ArgumentException(
                        string.Format("Invalid MIME type specification: '{0}'", value)
                    );
                }

                this.Caption.SetValue(BinaryType.MimeTypeProperty, value);
            }
        }

        public static readonly DependencyProperty MimeTypeProperty =
            DependencyProperty.RegisterAttached("MimeType", typeof(string), typeof(BinaryType));

        private static BinaryType defaultValue;
    }
}
