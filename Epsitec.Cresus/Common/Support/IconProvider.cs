//	Copyright © 2012-2019, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Drawing;
using Epsitec.Common.Support.Extensions;
using Epsitec.Common.Types;
using Epsitec.Common.Types.Converters;

namespace Epsitec.Common.Support
{
    /// <summary>
    /// The <c>IconProvider</c> class provides URIs for icons, and related services.
    /// </summary>
    public sealed class IconProvider
    {
        public IconProvider(string @namespace)
        {
            this.@namespace = @namespace;
        }

        public string GetRichTextImg(
            string iconName,
            double verticalOffset,
            Size iconSize = default(Size)
        )
        {
            if (iconSize == Size.Zero)
            {
                return string.Concat(
                    @"<img src=""",
                    /**/this.GetResourceIconUri(iconName),
                    /**/@""" voff=""",
                    /**/verticalOffset.ToString(System.Globalization.CultureInfo.InvariantCulture),
                    /**/@"""/>"
                );
            }

            return string.Concat(
                @"<img src=""",
                /**/this.GetResourceIconUri(iconName),
                /**/@""" voff=""",
                /**/verticalOffset.ToString(System.Globalization.CultureInfo.InvariantCulture),
                /**/@""" dx=""",
                /**/iconSize.Width.ToString(System.Globalization.CultureInfo.InvariantCulture),
                /**/@""" dy=""",
                /**/iconSize.Height.ToString(System.Globalization.CultureInfo.InvariantCulture),
                /**/@"""/>"
            );
        }

        public string GetResourceIconUri(string icon, string namespaceOverride = null)
        {
            string name = FormattedText.Escape(icon);

            if ((string.IsNullOrEmpty(icon)) || (icon.Contains(':')))
            {
                return name;
            }

            return this.GetImageResourceUri(name + ".icon", namespaceOverride);
        }

        public string GetResourceIconUri(string icon, Marshaler marshaler)
        {
            if (marshaler == null)
            {
                return this.GetResourceIconUri(icon);
            }

            return this.GetResourceIconUri(icon, marshaler.MarshaledType);
        }

        public string GetResourceIconUri(string icon, System.Type type)
        {
            if (type == null)
            {
                return this.GetResourceIconUri(icon);
            }

            var typeName = type.FullName;
            int entitiesPos = typeName.IndexOf(".Entities.");
            var namespaceOverride = entitiesPos < 0 ? null : typeName.Substring(0, entitiesPos);

            return this.GetResourceIconUri(icon, namespaceOverride);
        }

        public static string GetEntityIconUri(string iconName, System.Type type)
        {
            iconName.ThrowIfNullOrEmpty("iconName");
            type.ThrowIfNull("type");

            var typeName = type.FullName;
            int entitiesPos = typeName.IndexOf(".Entities.");

            if (entitiesPos < 0)
            {
                throw new System.ArgumentException(
                    "The type does not belong to the 'Entities' namespace"
                );
            }

            var typePrefix = typeName.Substring(0, entitiesPos);

            return string.Concat(
                "manifest:",
                typePrefix,
                ".Images.",
                FormattedText.Escape(iconName),
                ".icon"
            );
        }

        /// <summary>
        /// Retourne l'URI complet d'une image contenue dans les ressources.
        /// </summary>
        /// <param name="icon">Nom de l'image, avec extension.</param>
        /// <returns>URI de l'image.</returns>
        public string GetImageResourceUri(string filename, string namespaceOverride = null)
        {
            return string.Concat(
                "manifest:",
                namespaceOverride ?? this.@namespace,
                ".Images.",
                filename
            );
        }

        private readonly string @namespace;
    }
}
