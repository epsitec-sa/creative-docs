//	Copyright © 2003-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Support
{
	using Hashtable = System.Collections.Hashtable;
	using ArrayList = System.Collections.ArrayList;
	
	/// <summary>
	/// La classe ImageProvider permet d'obtenir des images à partir de leur
	/// nom. Cette implémentation supporte les protocoles suivants :
	/// - "file:name", accès direct à une image dans un fichier (name)
	/// - "res:id#field", accès direct à une image dans un bundle de ressources
	/// - "dyn:tag", accès à une image dynamique
	/// </summary>
	public class ImageProvider : IImageProvider
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
			
			ImageProvider.default_provider = new ImageProvider ();
			ImageProvider.default_paths    = new string[4];
			ImageProvider.default_paths[0] = System.Windows.Forms.Application.StartupPath;
			ImageProvider.default_paths[1] = path;
			ImageProvider.default_paths[2] = other;
			ImageProvider.default_paths[3] = "";
		}
		
		
		public static void Initialise()
		{
			//	En appelant cette méthode statique, on peut garantir que le constructeur
			//	statique de ImageProvider a bien été exécuté.
		}
		
		
		public static ImageProvider		Default
		{
			get { return ImageProvider.default_provider; }
		}
		
		public bool						CheckFilePath
		{
			get
			{
				return this.check_path;
			}
			set
			{
				this.check_path = value;
			}
		}
		
		public bool						EnableLongLifeCache
		{
			get
			{
				if (this.keep_alive_images == null)
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
						this.keep_alive_images = new ArrayList ();
					}
					else
					{
						this.keep_alive_images = null;
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
			
			if (name.StartsWith ("dyn:"))
			{
				string full_name = name.Substring (4);
				
				int pos = full_name.IndexOf ('/');
				
				if (pos < 0)
				{
					return null;
				}
				
				string base_name = full_name.Substring (0, pos);
				string argument = full_name.Substring (pos+1);
				
				Drawing.DynamicImage image = this.dynamic_images[base_name] as Drawing.DynamicImage;
				
				if (image != null)
				{
					image = image.GetImageForArgument (argument);
				}
				
				return image;
			}
			
			if (this.images.ContainsKey (name))
			{
				System.WeakReference weak_ref = this.images[name] as System.WeakReference;
				Drawing.Image image = weak_ref.Target as Drawing.Image;
				
				if (weak_ref.IsAlive)
				{
					return image;
				}
				
				this.images.Remove (name);
			}
			
			if (name.StartsWith ("file:"))
			{
				//	TODO: vérifier le nom du fichier pour éviter de faire des bêtises ici
				//	(pour améliorer la sécurité, mais ce n'est probablement pas un problème).
				
				Drawing.Image                image     = null;
				string                       base_name = name.Remove (0, 5);
				System.Collections.ArrayList attempts  = new System.Collections.ArrayList ();
				
				if ((base_name.StartsWith ("/")) ||
					(! RegexFactory.PathName.IsMatch (base_name)))
				{
					if (this.CheckFilePath)
					{
						throw new System.ArgumentException (string.Format ("Illegal file name for image ({0}).", base_name));
					}
				}
				
				for (int i = 0; i < ImageProvider.default_paths.Length; i++)
				{
					string path = ImageProvider.default_paths[i];
					
					//	Il se peut que cette option ne soit pas définie :
					
					if (path == null)
					{
						continue;
					}
					
					//	Nom du chemin complet.
					
					string file_name;
					
					if (path.Length > 0)
					{
						file_name = path + System.IO.Path.DirectorySeparatorChar + base_name;
					}
					else
					{
						file_name = base_name;
					}
					
					try
					{
						image = Drawing.Bitmap.FromFile (file_name);
						break;
					}
					catch
					{
						attempts.Add (file_name);
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
					this.images[name] = new System.WeakReference (image);
					
					if (this.keep_alive_images != null)
					{
						this.keep_alive_images.Add (image);
					}
				}
				
				return image;
			}
			
			if ((name.StartsWith ("res:")) &&
				(resourceManager != null))
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
				
				string res_full = name.Remove (0, 4);
				string res_bundle;
				string res_field;
				
				if (res_full.IndexOf (':') < 0)
				{
					res_full = this.default_resource_provider + res_full;
				}
				
				Drawing.Image image = null;

				if (Resources.SplitFieldId (res_full, out res_bundle, out res_field))
				{
					ResourceBundle bundle = resourceManager.GetBundle (res_bundle);
					
					if (bundle != null)
					{
						image = this.CreateBitmapFromBundle (bundle, res_field);
					}
				}
				else
				{
					byte[] data = resourceManager.GetBinaryData (res_full);
					image = Drawing.Bitmap.FromData (data);
				}
				
				if (image != null)
				{
					this.images[name] = new System.WeakReference (image);
					
					if (this.keep_alive_images != null)
					{
						this.keep_alive_images.Add (image);
					}
				}
				
				return image;
			}
			
			if (name.StartsWith ("manifest:"))
			{
				//	L'image décrite est stockée dans les ressources du manifeste de l'assembly .NET.
				//	Il faut en faire une copie locale, car les bits d'origine ne sont pas copiés par
				//	.NET et des transformations futures pourraient ne pas fonctionner.
				
				string res_name = name.Remove (0, 9);
				
				System.AppDomain             domain     = System.AppDomain.CurrentDomain;
				System.Reflection.Assembly[] assemblies = domain.GetAssemblies ();
				System.Reflection.Assembly   assembly   = null;
				
				for (int i = 0; i < assemblies.Length; i++)
				{
					object assembly_object = assemblies[i];
					
					if (assembly_object is System.Reflection.Emit.AssemblyBuilder)
					{
						//	Saute les assembly dont on sait qu'elles n'ont pas de ressources intéressantes,
						//	puisqu'elles ont été générées dynamiquement.
						
						continue;
					}
					
					string[] names = assemblies[i].GetManifestResourceNames ();
					string lower_res_name = res_name.ToLower (System.Globalization.CultureInfo.InvariantCulture);
					
					for (int j = 0; j < names.Length; j++)
					{
						if (names[j].ToLower (System.Globalization.CultureInfo.InvariantCulture) == lower_res_name)
						{
							assembly = assemblies[i];
							res_name = names[j];
							break;
						}
					}
					
					if (assembly != null)
					{
						break;
					}
				}
				
				if (assembly == null)
				{
					return null;
				}
				
				Drawing.Image image = Drawing.Bitmap.FromManifestResource (res_name, assembly);
//				image = Drawing.Bitmap.CopyImage (image);
				
				System.Diagnostics.Debug.WriteLine ("Loaded image " + res_name + " from assembly " + assembly.GetName ());
				
				if (image != null)
				{
					this.images[name] = new System.WeakReference (image);
					
					if (this.keep_alive_images != null)
					{
						this.keep_alive_images.Add (image);
					}
				}
				
				return image;
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
				for (int i = 0; i < ImageProvider.default_paths.Length; i++)
				{
					string path = ImageProvider.default_paths[i];
					
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
			if (this.keep_alive_images == null)
			{
				return new Drawing.Image[0];
			}
			
			object[]        objects = this.keep_alive_images.ToArray ();
			Drawing.Image[] images  = new Drawing.Image[objects.Length];
			
			for (int i = 0; i < objects.Length; i++)
			{
				images[i] = objects[i] as Drawing.Image;
			}
			
			return images;
		}
		
		
		public void AddDynamicImage(string tag, Drawing.DynamicImage image)
		{
			this.dynamic_images[tag] = image;
		}
		
		public void RemoveDynamicImage(string tag)
		{
			this.dynamic_images.Remove (tag);
		}
		
		public void ClearDynamicImageCache(string full_name)
		{
			int pos = full_name.IndexOf ('/');
			
			string base_name;
			string argument;
			
			if (pos < 0)
			{
				base_name = full_name;
				argument  = null;
			}
			else
			{
				base_name = full_name.Substring (0, pos);
				argument = full_name.Substring (pos+1);
			}
			
			Drawing.DynamicImage image = this.dynamic_images[base_name] as Drawing.DynamicImage;
			
			if (image != null)
			{
				image.ClearCache (argument);
			}
		}
		
		public void PrefillManifestIconCache()
		{
			System.AppDomain             domain     = System.AppDomain.CurrentDomain;
			System.Reflection.Assembly[] assemblies = domain.GetAssemblies ();
			
			for (int i = 0; i < assemblies.Length; i++)
			{
				object assembly_object = assemblies[i];
				
				if (assembly_object is System.Reflection.Emit.AssemblyBuilder)
				{
					//	Saute les assembly dont on sait qu'elles n'ont pas de ressources intéressantes,
					//	puisqu'elles ont été générées dynamiquement.
					
					continue;
				}
				
				string[] names = assemblies[i].GetManifestResourceNames ();
				
				for (int j = 0; j < names.Length; j++)
				{
					if (names[j].EndsWith (".icon"))
					{
						string                     res_name = names[j];
						System.Reflection.Assembly assembly = assemblies[i];
						
						string name = string.Concat ("manifest:", res_name);
						
						System.WeakReference weak_ref = this.images[name] as System.WeakReference;
						
						if ((weak_ref == null) ||
							(weak_ref.IsAlive == false))
						{
							try
							{
								Drawing.Image image = Drawing.Bitmap.FromManifestResource (res_name, assembly);
								
								if (image != null)
								{
									System.Diagnostics.Debug.WriteLine ("Pre-loaded image " + res_name + " from assembly " + assembly.GetName ());
									
									this.images[name] = new System.WeakReference (image);
									
									if (this.keep_alive_images != null)
									{
										this.keep_alive_images.Add (image);
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
			else
			{
				System.WeakReference weak_ref = this.images[name] as System.WeakReference;
				Drawing.Image        image    = weak_ref.Target as Drawing.Image;
				
				this.images.Remove (name);
				
				if (image != null)
				{
					image.RemoveFromCache ();
				}
			}
			
			if (this.keep_alive_images != null)
			{
				this.keep_alive_images.Clear ();
			}
		}
		
		
		public static string[] GetManifestResourceNames(System.Text.RegularExpressions.Regex regex)
		{
			List<string> list = new List<string> ();
			
			System.AppDomain             domain     = System.AppDomain.CurrentDomain;
			System.Reflection.Assembly[] assemblies = domain.GetAssemblies ();
			
			for (int i = 0; i < assemblies.Length; i++)
			{
				object assembly_object = assemblies[i];
				
				if (assembly_object is System.Reflection.Emit.AssemblyBuilder)
				{
					//	Saute les assembly dont on sait qu'elles n'ont pas de ressources intéressantes,
					//	puisqu'elles ont été générées dynamiquement.
					
					continue;
				}
				
				foreach (string name in assemblies[i].GetManifestResourceNames ())
				{
					if (regex.IsMatch (name))
					{
						list.Add (name);
					}
				}
			}

			return list.ToArray ();
		}
		
		
		protected Drawing.Image CreateBitmapFromBundle(ResourceBundle bundle, string image_name)
		{
			string field_name = "i." + image_name;
			
			Drawing.Image cache = this.bundle_hash[bundle] as Drawing.Image;
			
			if (cache == null)
			{
				if (bundle["image.data"].Type != ResourceFieldType.Binary)
				{
					throw new ResourceException (string.Format ("Bundle does not contain image"));
				}
				
				byte[] image_data = bundle["image.data"].AsBinary;
				string image_args = bundle["image.size"].AsString;
				
				Drawing.Size size = Drawing.Size.Parse (image_args);
				
				cache = Drawing.Bitmap.FromData (image_data, Drawing.Point.Zero, size);
				
				this.bundle_hash[bundle] = cache;
			}
			
			System.Diagnostics.Debug.Assert (cache != null);
			
			if (bundle.Contains (field_name))
			{
				//	Une image est définie par un champ 'i.name' qui contient une chaîne composée
				//	de 'x;y;dx;dy;ox;oy' définissant l'origine dans l'image mère, la taille et
				//	l'offset de l'origine dans la sous-image. 'oy;oy' sont facultatifs.
				
				string[] args = bundle[field_name].AsString.Split (';', ':');
				
				if ((args.Length != 4) && (args.Length != 6))
				{
					throw new ResourceException (string.Format ("Invalid image specification for '{0}', {1} arguments", image_name, args.Length));
				}
				
				Drawing.Point rect_pos = Drawing.Point.Parse (args[0] + ";" + args[1]);
				Drawing.Size  rect_siz = Drawing.Size.Parse (args[2] + ";" + args[3]);
				Drawing.Point origin   = Drawing.Point.Zero;
				
				if (args.Length >= 6)
				{
					origin = Drawing.Point.Parse (args[4] + ";" + args[5]);
				}
				
				return Drawing.Bitmap.FromLargerImage (cache, new Drawing.Rectangle (rect_pos, rect_siz), origin);
			}
			
			return null;
		}

		
		protected Hashtable				images = new Hashtable ();
		protected Hashtable				dynamic_images = new Hashtable ();
		protected ArrayList				keep_alive_images = null;
		protected Hashtable				bundle_hash = new Hashtable ();
		protected string				default_resource_provider = "file:";
		protected bool					check_path = true;
		
		protected static ImageProvider	default_provider;
		protected static string[]		default_paths;
	}
}
