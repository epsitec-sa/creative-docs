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

using System.Xml.Linq;

namespace Epsitec.Common.Text.Styles
{
    /// <summary>
    /// La classe AdditionalSettings sert de base pour les réglages aditionnels,
    /// en particulier LocalSettings et ExtraSettings.
    /// </summary>
    public abstract class AdditionalSettings : PropertyContainer, IContentsComparer
    {
        protected AdditionalSettings() { }

        protected AdditionalSettings(System.Collections.ICollection properties)
            : base(properties) { }

        public int SettingsIndex
        {
            get { return this.settingsIndex; }
            set
            {
                Debug.Assert.IsInBounds(value, 0, BaseSettings.MaxSettingsCount);
                this.settingsIndex = (byte)value;
            }
        }

        #region IContentsComparer Members
        public abstract bool CompareEqualContents(object value);
        #endregion


        protected AdditionalSettings(XElement xml)
            : base(xml) { }

        private byte settingsIndex;
    }
}
