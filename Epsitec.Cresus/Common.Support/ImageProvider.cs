//	Copyright © 2003, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : en chantier/PA

namespace Epsitec.Common.Support
{
	using Hashtable = System.Collections.Hashtable;
	
	/// <summary>
	/// La classe ImageProvider permet d'obtenir des images à partir de leur
	/// nom. Cette implémentation supporte les protocoles suivants :
	/// - "file:name", accès direct à une image dans un fichier (name)
	/// - "res:id#field", accès direct à une image dans un bundle de ressources
	/// </summary>
	public class ImageProvider : Drawing.IImageProvider
	{
		private ImageProvider()
		{
		}
		
		
		public static ImageProvider		Default
		{
			get { return ImageProvider.default_provider; }
		}
		
		
		public static void RegisterAssembly(string name, System.Reflection.Assembly assembly)
		{
			ImageProvider.Default.assemblies[name] = assembly;
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
				//	TODO: vérifier le nom du fichier pour éviter de faire des bêtises ici
				//	(pour améliorer la sécurité, mais ce n'est probablement pas un problème).
				
				string file_name = name.Remove (0, 5);
				Drawing.Image image = null;
				
				try
				{
					image = Drawing.Bitmap.FromFile (file_name);
				}
				catch
				{
					try
					{
						image = Drawing.Bitmap.FromFile (@"..\..\"+file_name);
					}
					catch
					{
						System.Diagnostics.Debug.WriteLine (string.Format ("Cannot open file '{0}'.", file_name));
					}
				}
				
				if (image != null)
				{
					this.images[name] = new System.WeakReference (image);
				}
				
				return image;
			}
			
			if (name.StartsWith ("res:"))
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
				//	L'image décrite est stockée dans les ressources du manifeste de l'assembly .NET.
				//	Il faut en faire une copie locale, car les bits d'origine ne sont pas copiés par
				//	.NET et des transformations futures pourraient ne pas fonctionner.
				
				string res_name = name.Remove (0, 9);
				string[] args = res_name.Split ('/');
				
				if (args.Length != 2)
				{
					throw new System.ArgumentException (string.Format ("Illegal name format for manifest image ({0}).", res_name));
				}
				
				string assembly_name = args[0];
				string resource_name = args[1];
				
				System.Reflection.Assembly assembly = this.assemblies[assembly_name] as System.Reflection.Assembly;
				
				if (assembly == null)
				{
					throw new System.ArgumentException (string.Format ("Illegal assembly name for manifest image ({0}).", res_name));
				}
				
				Drawing.Image image = Drawing.Bitmap.FromManifestResource (assembly_name + "." + resource_name, assembly);
				image = Drawing.Bitmap.CopyImage (image);
				
				System.Diagnostics.Debug.WriteLine ("Loaded image " + resource_name + " from assembly " + assembly_name);
				
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
				//	Une image est définie par un champ 'i.name' qui contient une chaîne composée
				//	de 'x;y;dx;dy;ox;oy' définissant l'origine dans l'image mère, la taille et
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
		protected Hashtable				assemblies = new Hashtable ();
		protected string				default_resource_provider = "file:";
		
		protected static ImageProvider	default_provider = new ImageProvider ();
	}
}
