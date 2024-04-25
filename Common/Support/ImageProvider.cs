//	Copyright © 2003-2012, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Drawing;
using Epsitec.Common.Types;
using static Epsitec.Common.Support.Res.Fields;

namespace Epsitec.Common.Support
{
    /// <summary>
    /// La classe ImageProvider permet d'obtenir des images à partir de leur
    /// nom. Cette implémentation supporte les protocoles suivants :
    /// - "file:name", accès direct à une image dans un fichier (name)
    /// - "res:id#field", accès direct à une image dans un bundle de ressources
    /// - "dyn:tag", accès à une image dynamique
    /// </summary>
    public sealed class ImageProvider : IImageProvider
    {
        private ImageProvider() { }

        static ImageProvider()
        {
            string path = System.IO.Directory.GetCurrentDirectory();
            string otherPath = IO.PathTools.RemoveUntilDir("Common.Tests", path);
            string thirdPath = System.IO.Path.Join(
                IO.PathTools.RemoveUntilDir("cresus-core", path),
                "External"
            );

            ImageProvider.defaultProvider = new ImageProvider();
            ImageProvider.defaultPaths = new string[5];
            ImageProvider.defaultPaths[0] = Globals.Directories.ExecutableRoot;
            ImageProvider.defaultPaths[1] = otherPath;
            ImageProvider.defaultPaths[2] = thirdPath;
            ImageProvider.defaultPaths[3] = path;
            ImageProvider.defaultPaths[4] = "";
        }

        public static void Initialize()
        {
            //	En appelant cette méthode statique, on peut garantir que le constructeur
            //	statique de ImageProvider a bien été exécuté.
        }

        public static ImageProvider Instance
        {
            get { return ImageProvider.defaultProvider; }
        }

        public bool CheckFilePath
        {
            get { return this.checkPath; }
            set { this.checkPath = value; }
        }

        public bool EnableLongLifeCache
        {
            get
            {
                if (this.keepAliveImages == null)
                {
                    return false;
                }

                return true;
            }
            set
            {
                if (this.EnableLongLifeCache != value)
                {
                    if (value)
                    {
                        this.keepAliveImages = new List<Image>();
                    }
                    else
                    {
                        this.keepAliveImages = null;
                    }
                }
            }
        }

        public Image GetImage(string name, ResourceManager resourceManager)
        {
            if (string.IsNullOrEmpty(name))
            {
                return null;
            }

            if (name.StartsWith("stockicon:"))
            {
                return this.GetImageFromStockIcon(name);
            }

            if (name.StartsWith("foldericon:"))
            {
                return this.GetImageFromFolderIcon(name);
            }

            if (name.StartsWith("dyn:"))
            {
                return this.GetDynamicImage(name);
            }

            if (this.images.ContainsKey(name))
            {
                Weak<Image> weakRef = this.images[name];

                if (weakRef.IsAlive)
                {
                    return weakRef.Target;
                }

                this.images.Remove(name);
            }

            if (name.StartsWith("file:"))
            {
                return this.GetImageFromFile(name);
            }

            if ((name.StartsWith("res:")) && (resourceManager != null))
            {
                return this.GetImageFromResourceManager(name, resourceManager);
            }

            if (name.StartsWith("manifest:"))
            {
                return this.GetImageFromManifestResource(name);
            }

            return null;
        }

        public Image GetImageFromFolderIcon(string name)
        {
            long id = long.Parse(
                name.Substring(11),
                System.Globalization.CultureInfo.InvariantCulture
            );

            return FolderItemIconCache.Instance.Resolve(id);
        }

        public Image GetImageFromStockIcon(string name)
        {
            switch (name)
            {
                case "stockicon:shield":
                    return PrivilegeManager.Current.GetShieldIcon(IconSize.Normal);
                case "stockicon:shield.small":
                    return PrivilegeManager.Current.GetShieldIcon(IconSize.Small);

                default:
                    break;
            }

            return null;
        }

        public Image GetDynamicImage(string name)
        {
            string fullName = name.Substring(4);

            int pos = fullName.IndexOf('/');

            if (pos < 0)
            {
                return null;
            }

            string baseName = fullName.Substring(0, pos);
            string argument = fullName.Substring(pos + 1);

            DynamicImage image;

            if (this.dynamicImages.TryGetValue(baseName, out image))
            {
                image = image.GetImageForArgument(argument);
            }

            return image;
        }

        public Image GetImageFromFile(string name)
        {
            //	TODO: vérifier le nom du fichier pour éviter de faire des bêtises ici
            //	(pour améliorer la sécurité, mais ce n'est probablement pas un problème).

            Image image = null;
            string baseName = name.Remove(0, 5);
            System.Collections.ArrayList attempts = new System.Collections.ArrayList();

            if ((baseName.StartsWith("/")) || (!RegexFactory.PathName.IsMatch(baseName)))
            {
                if (this.CheckFilePath)
                {
                    throw new System.ArgumentException(
                        string.Format("Illegal file name for image ({0}).", baseName)
                    );
                }
            }

            for (int i = 0; i < ImageProvider.defaultPaths.Length; i++)
            {
                string path = ImageProvider.defaultPaths[i];

                //	Il se peut que cette option ne soit pas définie :

                if (path == null)
                {
                    continue;
                }

                //	Nom du chemin complet.

                string fileName;

                if (path.Length > 0)
                {
                    fileName = path + System.IO.Path.DirectorySeparatorChar + baseName;
                }
                else
                {
                    fileName = baseName;
                }

                try
                {
                    image = Bitmap.FromFile(fileName);
                    break;
                }
                catch
                {
                    attempts.Add(fileName);
                }
            }

            if (image == null)
            {
                System.Diagnostics.Debug.WriteLine(
                    string.Format("Tried to resolve '{0}' and failed while checking", name)
                );

                foreach (string attempt in attempts)
                {
                    System.Diagnostics.Debug.WriteLine(string.Format("  here: {0}", attempt));
                }
            }

            this.StoreImageToCache(name, image);

            return image;
        }

        public Image GetImageFromResourceManager(string name, ResourceManager resourceManager)
        {
            //	L'image décrite par l'identificateur de ressources est définie au moyen
            //	d'un bundle comportant au minimum le champ "image" et le champ spécifié
            //	par le format "res:id#field".
            //
            //	Le champ "image" se réfère aux données binaires de l'image (par exemple
            //	une image au format PNG).
            //
            //	Le champ spécifié décrit quant à lui quelques informations au sujet de
            //	la partie de l'image qui nous intéresse ici. Dans la partie "id", il
            //	faut en principe spécifier le provider à utiliser, sinon le provider
            //	de ressources par défaut sera pris.

            string resFull = name.Remove(0, 4);
            string resBundle;
            string resField;

            if (resFull.IndexOf(':') < 0)
            {
                resFull = this.providerPrefix + resFull;
            }

            Image image = null;

            if (Resources.SplitFieldId(resFull, out resBundle, out resField))
            {
                ResourceBundle bundle = resourceManager.GetBundle(resBundle);

                if (bundle != null)
                {
                    image = this.CreateBitmapFromBundle(bundle, resField);
                }
            }
            else
            {
                byte[] data = resourceManager.GetBinaryData(resFull);
                image = Bitmap.FromData(data);
            }

            this.StoreImageToCache(name, image);

            return image;
        }

        public Image GetImageFromManifestResource(string name)
        {
            //	L'image décrite est stockée dans les ressources du manifeste de l'assembly .NET.
            //	Il faut en faire une copie locale, car les bits d'origine ne sont pas copiés par
            //	.NET et des transformations futures pourraient ne pas fonctionner.

            var assemblies = ImageProvider.GetAssemblies();
            var resourceName = name.Remove(0, 9).ToLowerInvariant();

            Image image = null;

            foreach (var assembly in assemblies)
            {
                var resourceNames = assembly.GetManifestResourceNames();
                string matchingName = resourceNames
                    .Where(x => x.ToLowerInvariant() == resourceName)
                    .FirstOrDefault();

                if (matchingName == null)
                {
                    continue;
                }

                image = this.GetImageFromManifestResource(matchingName, assembly);

                this.StoreImageToCache(name, image);

                if (image != null)
                {
                    return image;
                }
            }
            return null;
        }

        public Image GetImageFromManifestResource(string name, System.Reflection.Assembly assembly)
        {
            using (System.IO.Stream stream = assembly.GetManifestResourceStream(name))
            {
                if (stream == null)
                {
                    return null;
                }

                long length = stream.Length;
                byte[] buffer = new byte[length];
                stream.Read(buffer, 0, (int)length);

                if (name.EndsWith(".icon"))
                {
                    // image au format vectoriel "maison" EPSITEC
                    return Canvas.FromData(buffer);
                }
                else
                {
                    return Bitmap.FromData(buffer);
                }
            }
        }

        public string[] GetImageNames(string provider, ResourceManager resourceManager)
        {
            if (string.IsNullOrEmpty(provider))
            {
                return new string[0];
            }

            List<string> list = new List<string>();

            if (provider == "file")
            {
                for (int i = 0; i < ImageProvider.defaultPaths.Length; i++)
                {
                    string path = ImageProvider.defaultPaths[i];

                    if (string.IsNullOrEmpty(path))
                    {
                        continue;
                    }

                    foreach (
                        string file in System.IO.Directory.GetFiles(
                            path,
                            "*.icon",
                            System.IO.SearchOption.AllDirectories
                        )
                    )
                    {
                        string name = string.Concat(provider, ":", file.Remove(0, path.Length + 1));

                        if (list.Contains(name) == false)
                        {
                            list.Add(name);
                        }
                    }
                }
            }
            else if (provider == "manifest")
            {
                System.Text.RegularExpressions.Regex regex = RegexFactory.FromSimpleJoker(
                    "*.icon",
                    RegexFactory.Options.IgnoreCase
                );

                foreach (string res in ImageProvider.GetManifestResourceNames(regex))
                {
                    string name = string.Concat(provider, ":", res);

                    if (list.Contains(name) == false)
                    {
                        list.Add(name);
                    }
                }
            }

            list.Sort();

            return list.ToArray();
        }

        public Image[] GetLongLifeCacheContents()
        {
            if (this.keepAliveImages == null)
            {
                return new Image[0];
            }
            else
            {
                return this.keepAliveImages.ToArray();
            }
        }

        public void AddDynamicImage(string tag, DynamicImage image)
        {
            this.dynamicImages[tag] = image;
        }

        public void RemoveDynamicImage(string tag)
        {
            this.dynamicImages.Remove(tag);
        }

        public void ClearDynamicImageCache(string fullName)
        {
            int pos = fullName.IndexOf('/');

            string baseName;
            string argument;

            if (pos < 0)
            {
                baseName = fullName;
                argument = null;
            }
            else
            {
                baseName = fullName.Substring(0, pos);
                argument = fullName.Substring(pos + 1);
            }

            DynamicImage image;

            if (this.dynamicImages.TryGetValue(baseName, out image))
            {
                image.ClearCache(argument);
            }
        }

        public void PrefillManifestIconCache()
        {
            var assemblies = ImageProvider.GetAssemblies();

            foreach (var assemblyObject in assemblies)
            {
                var names = assemblyObject
                    .GetManifestResourceNames()
                    .Where(x => x.EndsWith(".icon"));

                foreach (var resName in names)
                {
                    string name = string.Concat("manifest:", resName);

                    if (!this.images.ContainsKey(name))
                    {
                        continue;
                    }

                    Weak<Image> weakRef = this.images[name];

                    if (weakRef.IsAlive)
                    {
                        continue;
                    }
                    try
                    {
                        Image image = this.GetImageFromManifestResource(resName, assemblyObject);

                        if (image != null)
                        {
                            System.Diagnostics.Debug.WriteLine(
                                "Pre-loaded image "
                                    + resName
                                    + " from assembly "
                                    + assemblyObject.GetName()
                            );

                            this.StoreImageToCache(name, image);
                        }
                    }
                    catch { }
                }
            }
        }

        public void ClearImageCache(string name)
        {
            if (name == null)
            {
                var names = new string[this.images.Count];

                this.images.Keys.CopyTo(names, 0);

                for (int i = 0; i < names.Length; i++)
                {
                    this.ClearImageCache(names[i]);
                }
            }
            else if (this.images.ContainsKey(name))
            {
                var weakRef = this.images[name];
                var image = weakRef.Target;

                this.images.Remove(name);

                if (image != null)
                {
                    image.RemoveFromCache();
                }
            }

            if (this.keepAliveImages != null)
            {
                this.keepAliveImages.Clear();
            }
        }

        public static string[] GetManifestResourceNames(System.Text.RegularExpressions.Regex regex)
        {
            List<string> list = new List<string>();

            var assemblies = ImageProvider.GetAssemblies();

            foreach (var assembly in assemblies)
            {
                list.AddRange(assembly.GetManifestResourceNames().Where(x => regex.IsMatch(x)));
            }

            return list.ToArray();
        }

        private void StoreImageToCache(string name, Image image)
        {
            if (image == null)
            {
                return;
            }

            this.images[name] = new Weak<Image>(image);

            if (this.keepAliveImages != null)
            {
                this.keepAliveImages.Add(image);
            }
        }

        private Image CreateBitmapFromBundle(ResourceBundle bundle, string imageName)
        {
            string fieldName = "i." + imageName;

            Image cache = this.bundleImages[bundle];

            if (cache == null)
            {
                if (bundle["image.data"].Type != ResourceFieldType.Binary)
                {
                    throw new ResourceException(string.Format("Bundle does not contain image"));
                }

                byte[] imageData = bundle["image.data"].AsBinary;
                string imageArgs = bundle["image.size"].AsString;

                Size size = Size.Parse(imageArgs);

                cache = Bitmap.FromData(imageData, Point.Zero);

                this.bundleImages[bundle] = cache;
            }

            System.Diagnostics.Debug.Assert(cache != null);

            if (bundle.Contains(fieldName))
            {
                //	Une image est définie par un champ 'i.name' qui contient une chaîne composée
                //	de 'x;y;dx;dy;ox;oy' définissant l'origine dans l'image mère, la taille et
                //	l'offset de l'origine dans la sous-image. 'oy;oy' sont facultatifs.

                string[] args = bundle[fieldName].AsString.Split(';', ':');

                if ((args.Length != 4) && (args.Length != 6))
                {
                    throw new ResourceException(
                        string.Format(
                            "Invalid image specification for '{0}', {1} arguments",
                            imageName,
                            args.Length
                        )
                    );
                }

                var rectPos = Point.Parse(args[0] + ";" + args[1]);
                var rectSize = Size.Parse(args[2] + ";" + args[3]);
                var origin = Point.Zero;

                if (args.Length >= 6)
                {
                    origin = Point.Parse(args[4] + ";" + args[5]);
                }

                return Bitmap.FromLargerImage(cache, new Rectangle(rectPos, rectSize), origin);
            }

            return null;
        }

        private static IEnumerable<System.Reflection.Assembly> GetAssemblies()
        {
            var domain = System.AppDomain.CurrentDomain;

            var assemblies = domain.GetAssemblies().Where(x => x.IsDynamic == false);

            return assemblies;
        }

        private readonly Dictionary<string, Weak<Image>> images =
            new Dictionary<string, Weak<Image>>();
        private readonly Dictionary<string, DynamicImage> dynamicImages =
            new Dictionary<string, DynamicImage>();
        private readonly Dictionary<ResourceBundle, Image> bundleImages =
            new Dictionary<ResourceBundle, Image>();

        private List<Image> keepAliveImages;

        private readonly string providerPrefix = "file:";
        private bool checkPath = true;

        private static readonly ImageProvider defaultProvider;
        private static readonly string[] defaultPaths;
    }
}
