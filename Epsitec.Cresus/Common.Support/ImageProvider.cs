//	Copyright � 2003-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Common.Support
{
	/// <summary>
	/// La classe ImageProvider permet d'obtenir des images � partir de leur
	/// nom. Cette impl�mentation supporte les protocoles suivants :
	/// - "file:name", acc�s direct � une image dans un fichier (name)
	/// - "res:id#field", acc�s direct � une image dans un bundle de ressources
	/// - "dyn:tag", acc�s � une image dynamique
	/// </summary>
	public sealed class ImageProvider : IImageProvider
	{
		private ImageProvider()
		{
		}
		
		static ImageProvider()
		{
			string   path   = System.IO.Directory.GetCurrentDirectory ();
			string   other  = null;
			string[] strips = new string[] { @"\bin\Debug", @"\bin\Release" };
			
			for (int i = 0; i < strips.Length; i++)
			{
				if (path.EndsWith (strips[i]))
				{
					other = path.Substring (0, path.Length - strips[i].Length);
					break;
				}
			}
			
			ImageProvider.defaultProvider = new ImageProvider ();
			ImageProvider.defaultPaths    = new string[4];
			ImageProvider.defaultPaths[0] = Globals.Directories.ExecutableRoot;
			ImageProvider.defaultPaths[1] = path;
			ImageProvider.defaultPaths[2] = other;
			ImageProvider.defaultPaths[3] = "";
		}
		
		
		public static void Initialize()
		{
			//	En appelant cette m�thode statique, on peut garantir que le constructeur
			//	statique de ImageProvider a bien �t� ex�cut�.
		}
		
		
		public static ImageProvider		Default
		{
			get { return ImageProvider.defaultProvider; }
		}
		
		public bool						CheckFilePath
		{
			get
			{
				return this.checkPath;
			}
			set
			{
				this.checkPath = value;
			}
		}
		
		public bool						EnableLongLifeCache
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
						this.keepAliveImages = new List<Drawing.Image> ();
					}
					else
					{
						this.keepAliveImages = null;
					}
				}
			}
		}
		
		
		public Drawing.Image GetImage(string name, Support.ResourceManager resourceManager)
		{
			if (string.IsNullOrEmpty (name))
			{
				return null;
			}

			if (name.StartsWith ("stockicon:"))
			{
				switch (name)
				{
					case "stockicon:shield":
						return PrivilegeManager.Current.GetShieldIcon (Epsitec.Common.Drawing.IconSize.Normal);
					case "stockicon:shield.small":
						return PrivilegeManager.Current.GetShieldIcon (Epsitec.Common.Drawing.IconSize.Small);

					default:
						break;
				}

				return null;
			}

			if (name.StartsWith ("foldericon:"))
			{
				long id = long.Parse (name.Substring (11), System.Globalization.CultureInfo.InvariantCulture);

				return FolderItemIconCache.Instance.Resolve (id);
			}
			
			if (name.StartsWith ("dyn:"))
			{
				string fullName = name.Substring (4);
				
				int pos = fullName.IndexOf ('/');
				
				if (pos < 0)
				{
					return null;
				}
				
				string baseName = fullName.Substring (0, pos);
				string argument = fullName.Substring (pos+1);

				Drawing.DynamicImage image;

				if (this.dynamicImages.TryGetValue (baseName, out image))
				{
					image = image.GetImageForArgument (argument);
				}
				
				return image;
			}
			
			if (this.images.ContainsKey (name))
			{
				Types.Weak<Drawing.Image> weakRef = this.images[name];
				Drawing.Image image = weakRef.Target;
				
				if (weakRef.IsAlive)
				{
					return image;
				}
				
				this.images.Remove (name);
			}
			
			if (name.StartsWith ("file:"))
			{
				//	TODO: v�rifier le nom du fichier pour �viter de faire des b�tises ici
				//	(pour am�liorer la s�curit�, mais ce n'est probablement pas un probl�me).
				
				Drawing.Image                image     = null;
				string                       baseName = name.Remove (0, 5);
				System.Collections.ArrayList attempts  = new System.Collections.ArrayList ();
				
				if ((baseName.StartsWith ("/")) ||
					(! RegexFactory.PathName.IsMatch (baseName)))
				{
					if (this.CheckFilePath)
					{
						throw new System.ArgumentException (string.Format ("Illegal file name for image ({0}).", baseName));
					}
				}
				
				for (int i = 0; i < ImageProvider.defaultPaths.Length; i++)
				{
					string path = ImageProvider.defaultPaths[i];
					
					//	Il se peut que cette option ne soit pas d�finie :
					
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
						image = Drawing.Bitmap.FromFile (fileName);
						break;
					}
					catch
					{
						attempts.Add (fileName);
					}
				}
				
				if (image == null)
				{
					System.Diagnostics.Debug.WriteLine (string.Format ("Tried to resolve '{0}' and failed while checking", name));
					
					foreach (string attempt in attempts)
					{
						System.Diagnostics.Debug.WriteLine (string.Format ("  here: {0}", attempt));
					}
				}
				else
				{
					this.images[name] = new Types.Weak<Drawing.Image> (image);
					
					if (this.keepAliveImages != null)
					{
						this.keepAliveImages.Add (image);
					}
				}
				
				return image;
			}
			
			if ((name.StartsWith ("res:")) &&
				(resourceManager != null))
			{
				//	L'image d�crite par l'identificateur de ressources est d�finie au moyen
				//	d'un bundle comportant au minimum le champ "image" et le champ sp�cifi�
				//	par le format "res:id#field".
				//
				//	Le champ "image" se r�f�re aux donn�es binaires de l'image (par exemple
				//	une image au format PNG).
				//
				//	Le champ sp�cifi� d�crit quant � lui quelques informations au sujet de
				//	la partie de l'image qui nous int�resse ici. Dans la partie "id", il
				//	faut en principe sp�cifier le provider � utiliser, sinon le provider
				//	de ressources par d�faut sera pris.
				
				string resFull = name.Remove (0, 4);
				string resBundle;
				string resField;
				
				if (resFull.IndexOf (':') < 0)
				{
					resFull = this.defaultResourceProvider + resFull;
				}
				
				Drawing.Image image = null;

				if (Resources.SplitFieldId (resFull, out resBundle, out resField))
				{
					ResourceBundle bundle = resourceManager.GetBundle (resBundle);
					
					if (bundle != null)
					{
						image = this.CreateBitmapFromBundle (bundle, resField);
					}
				}
				else
				{
					byte[] data = resourceManager.GetBinaryData (resFull);
					image = Drawing.Bitmap.FromData (data);
				}
				
				if (image != null)
				{
					this.images[name] = new Types.Weak<Drawing.Image> (image);
					
					if (this.keepAliveImages != null)
					{
						this.keepAliveImages.Add (image);
					}
				}
				
				return image;
			}
			
			if (name.StartsWith ("manifest:"))
			{
				//	L'image d�crite est stock�e dans les ressources du manifeste de l'assembly .NET.
				//	Il faut en faire une copie locale, car les bits d'origine ne sont pas copi�s par
				//	.NET et des transformations futures pourraient ne pas fonctionner.

				var assemblies   = ImageProvider.GetAssemblies ();
				var resourceName = name.Remove (0, 9).ToLowerInvariant ();

				Drawing.Image image = null;
				
				foreach (var assembly in assemblies)
				{
					var resourceNames = assembly.GetManifestResourceNames ();
					var matchingName  = resourceNames.Where (x => x.ToLowerInvariant () == resourceName).FirstOrDefault ();

					image = Drawing.Bitmap.FromManifestResource (matchingName, assembly);

					if (image != null)
					{
						this.images[name] = new Types.Weak<Drawing.Image> (image);

						if (this.keepAliveImages != null)
						{
							this.keepAliveImages.Add (image);
						}

						return image;
					}
				}
				
				return null;
			}
			
			return null;
		}

		public string[] GetImageNames(string provider, Support.ResourceManager resourceManager)
		{
			if (string.IsNullOrEmpty (provider))
			{
				return new string[0];
			}

			List<string> list = new List<string> ();

			if (provider == "file")
			{
				for (int i = 0; i < ImageProvider.defaultPaths.Length; i++)
				{
					string path = ImageProvider.defaultPaths[i];
					
					if (string.IsNullOrEmpty (path))
					{
						continue;
					}

					foreach (string file in System.IO.Directory.GetFiles (path, "*.icon", System.IO.SearchOption.AllDirectories))
					{
						string name = string.Concat (provider, ":", file.Remove (0, path.Length+1));
						
						if (list.Contains (name) == false)
						{
							list.Add (name);
						}
					}
				}
			}
			else if (provider == "manifest")
			{
				System.Text.RegularExpressions.Regex regex = RegexFactory.FromSimpleJoker ("*.icon", RegexFactory.Options.IgnoreCase);
				
				foreach (string res in ImageProvider.GetManifestResourceNames (regex))
				{
					string name = string.Concat (provider, ":", res);

					if (list.Contains (name) == false)
					{
						list.Add (name);
					}
				}
			}

			list.Sort ();

			return list.ToArray ();
		}

		public Drawing.Image[] GetLongLifeCacheContents()
		{
			if (this.keepAliveImages == null)
			{
				return new Drawing.Image[0];
			}
			else
			{
				return this.keepAliveImages.ToArray ();
			}
		}
		
		
		public void AddDynamicImage(string tag, Drawing.DynamicImage image)
		{
			this.dynamicImages[tag] = image;
		}
		
		public void RemoveDynamicImage(string tag)
		{
			this.dynamicImages.Remove (tag);
		}
		
		public void ClearDynamicImageCache(string fullName)
		{
			int pos = fullName.IndexOf ('/');
			
			string baseName;
			string argument;
			
			if (pos < 0)
			{
				baseName = fullName;
				argument  = null;
			}
			else
			{
				baseName = fullName.Substring (0, pos);
				argument = fullName.Substring (pos+1);
			}
			
			Drawing.DynamicImage image;
			
			if (this.dynamicImages.TryGetValue (baseName, out image))
			{
				image.ClearCache (argument);
			}
		}
		
		public void PrefillManifestIconCache()
		{
			var assemblies = ImageProvider.GetAssemblies ();

			foreach (var assemblyObject in assemblies)
			{
				var names = assemblyObject.GetManifestResourceNames ().Where (x => x.EndsWith (".icon"));
				
				foreach (var resName in names)
				{
					string name = string.Concat ("manifest:", resName);

					if (this.images.ContainsKey (name))
					{
						Types.Weak<Drawing.Image> weakRef = this.images[name];

						if (weakRef.IsAlive == false)
						{
							try
							{
								Drawing.Image image = Drawing.Bitmap.FromManifestResource (resName, assemblyObject);

								if (image != null)
								{
									System.Diagnostics.Debug.WriteLine ("Pre-loaded image " + resName + " from assembly " + assemblyObject.GetName ());

									this.images[name] = new Types.Weak<Drawing.Image> (image);

									if (this.keepAliveImages != null)
									{
										this.keepAliveImages.Add (image);
									}
								}
							}
							catch
							{
							}
						}
					}
				}
			}
		}
		
		public void ClearImageCache(string name)
		{
			if (name == null)
			{
				string[] names = new string[this.images.Count];
				
				this.images.Keys.CopyTo (names, 0);
				
				for (int i = 0; i < names.Length; i++)
				{
					this.ClearImageCache (names[i]);
				}
			}
			else if (this.images.ContainsKey (name))
			{
				Types.Weak<Drawing.Image> weakRef = this.images[name];
				Drawing.Image             image    = weakRef.Target;
				
				this.images.Remove (name);
				
				if (image != null)
				{
					image.RemoveFromCache ();
				}
			}
			
			if (this.keepAliveImages != null)
			{
				this.keepAliveImages.Clear ();
			}
		}
		
		
		public static string[] GetManifestResourceNames(System.Text.RegularExpressions.Regex regex)
		{
			List<string> list = new List<string> ();

			var assemblies = ImageProvider.GetAssemblies ();

			foreach (var assembly in assemblies)
			{
				list.AddRange (assembly.GetManifestResourceNames ().Where (x => regex.IsMatch (x)));
			}

			return list.ToArray ();
		}
		
		
		private Drawing.Image CreateBitmapFromBundle(ResourceBundle bundle, string imageName)
		{
			string fieldName = "i." + imageName;
			
			Drawing.Image cache = this.bundleImages[bundle];
			
			if (cache == null)
			{
				if (bundle["image.data"].Type != ResourceFieldType.Binary)
				{
					throw new ResourceException (string.Format ("Bundle does not contain image"));
				}
				
				byte[] imageData = bundle["image.data"].AsBinary;
				string imageArgs = bundle["image.size"].AsString;
				
				Drawing.Size size = Drawing.Size.Parse (imageArgs);
				
				cache = Drawing.Bitmap.FromData (imageData, Drawing.Point.Zero, size);
				
				this.bundleImages[bundle] = cache;
			}
			
			System.Diagnostics.Debug.Assert (cache != null);
			
			if (bundle.Contains (fieldName))
			{
				//	Une image est d�finie par un champ 'i.name' qui contient une cha�ne compos�e
				//	de 'x;y;dx;dy;ox;oy' d�finissant l'origine dans l'image m�re, la taille et
				//	l'offset de l'origine dans la sous-image. 'oy;oy' sont facultatifs.
				
				string[] args = bundle[fieldName].AsString.Split (';', ':');
				
				if ((args.Length != 4) && (args.Length != 6))
				{
					throw new ResourceException (string.Format ("Invalid image specification for '{0}', {1} arguments", imageName, args.Length));
				}
				
				Drawing.Point rectPos  = Drawing.Point.Parse (args[0] + ";" + args[1]);
				Drawing.Size  rectSize = Drawing.Size.Parse (args[2] + ";" + args[3]);
				Drawing.Point origin   = Drawing.Point.Zero;
				
				if (args.Length >= 6)
				{
					origin = Drawing.Point.Parse (args[4] + ";" + args[5]);
				}
				
				return Drawing.Bitmap.FromLargerImage (cache, new Drawing.Rectangle (rectPos, rectSize), origin);
			}
			
			return null;
		}


		private static IEnumerable<System.Reflection.Assembly> GetAssemblies()
		{
			var domain     = System.AppDomain.CurrentDomain;
			var assemblies = domain.GetAssemblies ().Where (x => x.IsDynamic == false);
			return assemblies;
		}


		private Dictionary<string, Types.Weak<Drawing.Image>>	images = new Dictionary<string, Types.Weak<Drawing.Image>> ();
		private Dictionary<string, Drawing.DynamicImage>		dynamicImages = new Dictionary<string, Drawing.DynamicImage> ();
		private List<Drawing.Image>								keepAliveImages;
		private Dictionary<ResourceBundle, Drawing.Image>		bundleImages = new Dictionary<ResourceBundle, Drawing.Image> ();
		private string											defaultResourceProvider = "file:";
		private bool											checkPath = true;
		
		private static ImageProvider							defaultProvider;
		private static string[]									defaultPaths;
	}
}
