//	Copyright � 2003-2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : en chantier/PA

namespace Epsitec.Common.Support
{
	using Hashtable = System.Collections.Hashtable;
	
	/// <summary>
	/// La classe ImageProvider permet d'obtenir des images � partir de leur
	/// nom. Cette impl�mentation supporte les protocoles suivants :
	/// - "file:name", acc�s direct � une image dans un fichier (name)
	/// - "res:id#field", acc�s direct � une image dans un bundle de ressources
	/// </summary>
	public class ImageProvider : Drawing.IImageProvider
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
			//	En appelant cette m�thode statique, on peut garantir que le constructeur
			//	statique de ImageProvider a bien �t� ex�cut�.
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
		
		
		public Drawing.Image GetImage(string name)
		{
			if ((name == null) ||
				(name.Length < 1))
			{
				return null;
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
				//	TODO: v�rifier le nom du fichier pour �viter de faire des b�tises ici
				//	(pour am�liorer la s�curit�, mais ce n'est probablement pas un probl�me).
				
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
					
					//	Il se peut que cette option ne soit pas d�finie :
					
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
				}
				
				return image;
			}
			
			if (name.StartsWith ("res:"))
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
				
				string res_full = name.Remove (0, 4);
				string res_bundle;
				string res_field;
				
				if (res_full.IndexOf (':') < 0)
				{
					res_full = this.default_resource_provider + res_full;
				}
				
				Drawing.Image image = null;
				
				if (ResourceBundle.SplitTarget (res_full, out res_bundle, out res_field))
				{
					ResourceBundle bundle = Resources.GetBundle (res_bundle);
					
					if (bundle != null)
					{
						image = this.CreateBitmapFromBundle (bundle, res_field);
					}
				}
				else
				{
					byte[] data = Resources.GetBinaryData (res_full);
					image = Drawing.Bitmap.FromData (data);
				}
				
				if (image != null)
				{
					this.images[name] = new System.WeakReference (image);
				}
				
				return image;
			}
			
			if (name.StartsWith ("manifest:"))
			{
				//	L'image d�crite est stock�e dans les ressources du manifeste de l'assembly .NET.
				//	Il faut en faire une copie locale, car les bits d'origine ne sont pas copi�s par
				//	.NET et des transformations futures pourraient ne pas fonctionner.
				
				string res_name = name.Remove (0, 9);
				
				System.AppDomain             domain     = System.AppDomain.CurrentDomain;
				System.Reflection.Assembly[] assemblies = domain.GetAssemblies ();
				System.Reflection.Assembly   assembly   = null;
				
				for (int i = 0; i < assemblies.Length; i++)
				{
					try
					{
						string[] names = assemblies[i].GetManifestResourceNames ();
					
						for (int j = 0; j < names.Length; j++)
						{
							if (names[j] == res_name)
							{
								assembly = assemblies[i];
								break;
							}
						}
					
						if (assembly != null)
						{
							break;
						}
					}
					catch
					{
					}
				}
				
				if (assembly == null)
				{
					throw new System.ArgumentException (string.Format ("Illegal assembly or resource name for manifest image ({0}).", res_name));
				}
				
				Drawing.Image image = Drawing.Bitmap.FromManifestResource (res_name, assembly);
				image = Drawing.Bitmap.CopyImage (image);
				
				System.Diagnostics.Debug.WriteLine ("Loaded image " + res_name + " from assembly " + assembly.GetName ());
				
				if (image != null)
				{
					this.images[name] = new System.WeakReference (image);
				}
				
				return image;
			}
			
			return null;
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
				
				Drawing.Size size = Drawing.Size.Parse (image_args, System.Globalization.CultureInfo.InvariantCulture);
				
				cache = Drawing.Bitmap.FromData (image_data, Drawing.Point.Empty, size);
				
				this.bundle_hash[bundle] = cache;
			}
			
			System.Diagnostics.Debug.Assert (cache != null);
			
			if (bundle.Contains (field_name))
			{
				//	Une image est d�finie par un champ 'i.name' qui contient une cha�ne compos�e
				//	de 'x;y;dx;dy;ox;oy' d�finissant l'origine dans l'image m�re, la taille et
				//	l'offset de l'origine dans la sous-image. 'oy;oy' sont facultatifs.
				
				string[] args = bundle[field_name].AsString.Split (';', ':');
				
				if ((args.Length != 4) && (args.Length != 6))
				{
					throw new ResourceException (string.Format ("Invalid image specification for '{0}', {1} arguments", image_name, args.Length));
				}
				
				Drawing.Point rect_pos = Drawing.Point.Parse (args[0] + ";" + args[1], System.Globalization.CultureInfo.InvariantCulture);
				Drawing.Size  rect_siz = Drawing.Size.Parse (args[2] + ";" + args[3], System.Globalization.CultureInfo.InvariantCulture);
				Drawing.Point origin   = Drawing.Point.Empty;
				
				if (args.Length >= 6)
				{
					origin = Drawing.Point.Parse (args[4] + ";" + args[5], System.Globalization.CultureInfo.InvariantCulture);
				}
				
				return Drawing.Bitmap.FromLargerImage (cache, new Drawing.Rectangle (rect_pos, rect_siz), origin);
			}
			
			return null;
		}

		
		protected Hashtable				images = new Hashtable ();
		protected Hashtable				bundle_hash = new Hashtable ();
		protected string				default_resource_provider = "file:";
		protected bool					check_path = true;
		
		protected static ImageProvider	default_provider;
		protected static string[]		default_paths;
	}
}
