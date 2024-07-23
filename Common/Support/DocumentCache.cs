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


using Epsitec.Common.Drawing;
using System.Collections.Generic;

namespace Epsitec.Common.Support
{
    /// <summary>
    /// The <c>DocumentCache</c> manages a static cache which stores document
    /// informations and thumbnails.
    /// </summary>
    public static class DocumentCache
    {
        public static void Add(string path)
        {
            //	Ajoute une miniature et une statistique dans le cache, si elles n'y sont pas déjà.
            //	Si la miniature ou la statistique ont changé, il faut au préalable exécuter Remove.
            path = DocumentCache.GetCleanPath(path);

            lock (DocumentCache.cache)
            {
                if (DocumentCache.cache.ContainsKey(path))
                {
                    return;
                }
            }

            //	Pour l'instant, le cache ne contient aucune information pour ce document; on va
            //	le charger, extraire l'image et les statistiques, puis mettre à jour le cache.

            IDocumentInfo info = DocumentManager.GetDocumentInfo(path);

            if (info != null)
            {
                lock (DocumentCache.cache)
                {
                    //	Il faut encore s'assurer que le cache n'a pas été mis à jour entre temps
                    //	par quelqu'un d'autre, auquel cas on peut simplement mettre à la poubelle
                    //	les informations (item) :

                    if (!DocumentCache.cache.ContainsKey(path))
                    {
                        DocumentCache.cache.Add(path, info);
                    }
                }
            }
        }

        public static void Remove(string path)
        {
            //	Supprime une miniature et une statistique du cache.
            path = DocumentCache.GetCleanPath(path);

            lock (DocumentCache.cache)
            {
                DocumentCache.cache.Remove(path);
            }
        }

        public static Image FindImage(string path)
        {
            //	Retourne une miniature contenue dans le cache.
            //	Retourne null si la miniature n'est pas dans le cache.

            IDocumentInfo info = DocumentCache.FindDocumentInfo(path);

            return info == null ? null : info.GetThumbnail();
        }

        public static IDocumentInfo FindDocumentInfo(string path)
        {
            //	Retourne une statistique contenue dans le cache.
            //	Retourne null si la statistique n'est pas dans le cache.

            path = DocumentCache.GetCleanPath(path);

            lock (DocumentCache.cache)
            {
                IDocumentInfo info;
                DocumentCache.cache.TryGetValue(path, out info);
                return info;
            }
        }

        private static string GetCleanPath(string path)
        {
            path = path.ToLower();

            //	Il faut considérer 'Abc' = 'abc' à cause de certaines anomalies.
            //	Par exemple, Document.DirectoryMySamples retourne :
            //	C:\Documents and Settings\Daniel Roux\Application Data\Epsitec\Crésus Documents\Mes exemples
            //	'Crésus Documents' au lieu de 'Crésus documents' qui est le vrai nom !

            return path;
        }

        private static Dictionary<string, IDocumentInfo> cache =
            new Dictionary<string, IDocumentInfo>();

        public static void CreateDefaultImageAssociations(DocumentManager documentManager)
        {
            documentManager.Associate(".bmp", DocumentCache.GetImageDocumentInfo);
            documentManager.Associate(".jpg", DocumentCache.GetImageDocumentInfo);
            documentManager.Associate(".png", DocumentCache.GetImageDocumentInfo);
            documentManager.Associate(".gif", DocumentCache.GetImageDocumentInfo);
            documentManager.Associate(".tif", DocumentCache.GetImageDocumentInfo);
            documentManager.Associate(".jpeg", DocumentCache.GetImageDocumentInfo);
            documentManager.Associate(".tiff", DocumentCache.GetImageDocumentInfo);
        }

        private static IDocumentInfo GetImageDocumentInfo(string path)
        {
            ImageManager imageManager = ImageManager.Instance;

            if (imageManager == null)
            {
                return null;
            }

            ImageData data = imageManager.GetImageFromFile(path);

            if (data == null)
            {
                return null;
            }
            else
            {
                return new Internal.ImageDocumentInfo(data);
            }
        }
    }
}
