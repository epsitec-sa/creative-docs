//	Copyright © 2003-2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : en chantier

namespace Epsitec.Common.Support
{
	using System.Collections;
	
	/// <summary>
	/// La classe ResourceBundle donne accès aux données d'une ressource XML sous
	/// une forme simplifiée.
	/// </summary>
	public abstract class ResourceBundle
	{
		public ResourceBundle(string name)
		{
			this.name = name;
		}
		
		
		public static ResourceBundle Create(string name)
		{
			return new ResourceBundleXmlDom (name);
		}
		
		public string					Name
		{
			get { return this.name; }
		}
		
		public bool						IsEmpty
		{
			get { return this.CountFields == 0; }
		}
		
		
		public abstract int				CountFields { get; }
		public abstract string[]		FieldNames { get; }
		public abstract object			this[string field] { get; }
		
		public abstract bool Contains(string field);
		
		public virtual ResourceFieldType GetFieldType(string field)
		{
			object data = this[field];
			
			if (data == null)
			{
				return ResourceFieldType.None;
			}
			if (data is string)
			{
				return ResourceFieldType.Data;
			}
			if (data is ResourceBundle)
			{
				return ResourceFieldType.Bundle;
			}
			if (data is byte[])
			{
				return ResourceFieldType.Binary;
			}
			if (data is System.Collections.IList)
			{
				return ResourceFieldType.List;
			}
			
			throw new ResourceException (string.Format ("Invalid field type in bundle: '{0}'", data.GetType ().Name));
		}
		
		public virtual string GetFieldString(string field)
		{
			return this[field] as string;
		}
		
		public virtual byte[] GetFieldBinary(string field)
		{
			return this[field] as byte[];
		}
		
		public virtual ResourceBundle GetFieldBundle(string field)
		{
			return this[field] as ResourceBundle;
		}
		
		public virtual System.Collections.IList GetFieldBundleList(string field)
		{
			return this[field] as System.Collections.IList;
		}
		
		public virtual int GetFieldBundleListLength(string field)
		{
			System.Collections.IList list = this[field] as System.Collections.IList;
			return (list == null) ? 0 : list.Count;
		}
		
		public virtual ResourceBundle GetFieldBundleListItem(string field, int index)
		{
			System.Collections.IList list = this[field] as System.Collections.IList;
			return (list == null) ? null : list[index] as ResourceBundle;
		}
		
		public virtual Drawing.Image GetBitmap(string image_name)
		{
			string field_name = "i." + image_name;
			
			if (this.bitmap_cache == null)
			{
				if (this.GetFieldType ("image.data") != ResourceFieldType.Binary)
				{
					throw new ResourceException (string.Format ("Bundle does not contain image"));
				}
				
				byte[] image_data = this.GetFieldBinary ("image.data");
				string image_args = this.GetFieldString ("image.size");
				
				Drawing.Size size = Drawing.Size.Parse (image_args, System.Globalization.CultureInfo.InvariantCulture);
				
				this.bitmap_cache = Drawing.Bitmap.FromData (image_data, Drawing.Point.Empty, size);
			}
			
			System.Diagnostics.Debug.Assert (this.bitmap_cache != null);
			
			if (this.Contains (field_name))
			{
				//	Une image est définie par un champ 'i.name' qui contient une chaîne composée
				//	de 'x;y;dx;dy;ox;oy' définissant l'origine dans l'image mère, la taille et
				//	l'offset de l'origine dans la sous-image. 'oy;oy' sont facultatifs.
				
				string[] args = this.GetFieldString (field_name).Split (';', ':');
				
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
				
				return Drawing.Bitmap.FromLargerImage (this.bitmap_cache, new Drawing.Rectangle (rect_pos, rect_siz), origin);
			}
			
			return null;
		}
		
		
		
		public void Compile(byte[] data)
		{
			this.Compile (data, null, ResourceLevel.Merged, 0);
		}
		
		public void Compile(byte[] data, string default_prefix, ResourceLevel level)
		{
			this.Compile (data, default_prefix, level, 0);
		}
		
		public abstract void Compile(byte[] data, string default_prefix, ResourceLevel level, int recursion);
		
		
		public static bool SplitTarget(string target, out string target_bundle, out string target_field)
		{
			int pos = target.IndexOf ("#");
			
			target_bundle = target;
			target_field  = null;
			
			if (pos >= 0)
			{
				target_bundle = target.Substring (0, pos);
				target_field  = target.Substring (pos+1);
				
				return true;
			}
			
			return false;
		}
		
		public static string ExtractName(string sort_name)
		{
			int pos = sort_name.IndexOf ('/');
			
			if (pos < 0)
			{
				throw new ResourceException (string.Format ("'{0}' is an invalid sort name", sort_name));
			}
			
			return sort_name.Substring (pos+1);
		}
		
		
		protected string				name;
		protected Drawing.Image			bitmap_cache;
		
		protected const int				MaxRecursion = 50;
	}
}
