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


namespace Epsitec.Common.Support.Implementation
{
    using System.Globalization;

    /// <summary>
    /// La classe AbstractProvider regroupe les méthodes communes aux divers
    /// providers de ressources et offre une implémentation par défaut de
    /// l'interface IResourceProvider.
    /// </summary>
    public abstract class AbstractResourceProvider : Epsitec.Common.Support.IResourceProvider
    {
        protected AbstractResourceProvider(ResourceManager manager)
        {
            this.manager = manager;
        }

        #region IResourceProvider Members
        public abstract string Prefix { get; }

        public abstract bool SelectModule(ref ResourceModuleId module);

        public virtual bool ValidateId(string id)
        {
            return (id != null) && (id.Length > 0) && (id.Length < 100) && (id[0] != '.');
        }

        public abstract bool Contains(string id);

        public abstract byte[] GetData(
            string id,
            Epsitec.Common.Support.ResourceLevel level,
            System.Globalization.CultureInfo culture
        );
        public abstract string[] GetIds(
            string nameFilter,
            string typeFilter,
            ResourceLevel level,
            System.Globalization.CultureInfo culture
        );
        public abstract ResourceModuleId[] GetModules();

        public abstract bool SetData(
            string id,
            Epsitec.Common.Support.ResourceLevel level,
            System.Globalization.CultureInfo culture,
            byte[] data,
            Epsitec.Common.Support.ResourceSetMode mode
        );
        public abstract bool Remove(
            string id,
            Epsitec.Common.Support.ResourceLevel level,
            System.Globalization.CultureInfo culture
        );
        #endregion

        protected virtual void SelectLocale(System.Globalization.CultureInfo culture)
        {
            this.culture = culture;

            this.defaultSuffix = this.manager.MapToSuffix(ResourceLevel.Default, culture);
            this.localSuffix = this.manager.MapToSuffix(ResourceLevel.Localized, culture);
            this.customSuffix = this.manager.MapToSuffix(ResourceLevel.Customized, culture);
        }

        protected CultureInfo culture;
        protected ResourceManager manager;

        protected string defaultSuffix;
        protected string localSuffix;
        protected string customSuffix;
    }
}
