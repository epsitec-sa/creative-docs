//	Copyright © 2003, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : en chantier/PA

using Epsitec.Common.Support;

namespace Epsitec.Cresus.DataLayer
{
	/// <summary>
	/// La classe DataAttributes représente des attributs qui peuvent exister
	/// en diverses variantes (localisation).
	/// </summary>
	public class DataAttributes : System.ICloneable
	{
		public DataAttributes()
		{
		}
		
		
		#region ICloneable Members
		public object Clone()
		{
			DataAttributes attr = System.Activator.CreateInstance (this.GetType ()) as DataAttributes;
			
			if (this.attributes != null)
			{
				attr.attributes = this.attributes.Clone () as System.Collections.Hashtable;
			}
			
			return attr;
		}
		#endregion		
		
		public void SetFromInitialisationList(params string[] list)
		{
			for (int i = 0; i < list.Length; i++)
			{
				string init = list[i];
				int pos = init.IndexOf ('=');
				
				if (pos < 1)
				{
					throw new DataException ("Invalid attribute initialisation syntax");
				}
				
				string attr_name = init.Substring (0, pos);
				string attr_data = init.Substring (pos+1);
				
				this.SetAttribute (attr_name, attr_data, null);
			}
		}
		
		public override string ToString()
		{
			string[] names = new string[this.attributes.Keys.Count];
			
			this.attributes.Keys.CopyTo (names, 0);
			
			System.Array.Sort (names);
			
			System.Text.StringBuilder buffer = new System.Text.StringBuilder ();
			string sep = "";
			
			foreach (string name in names)
			{
				buffer.Append (sep);
				buffer.Append (name);
				buffer.Append (@"=""");
				buffer.Append (this.attributes[name] as string);
				buffer.Append (@"""");
				
				sep = "; ";
			}
			
			return buffer.ToString ();
		}

		
		
		public string[]							Names
		{
			get
			{
				if ((this.attributes == null) || (this.attributes.Count == 0))
				{
					return new string[0];
				}
				
				string[] names = new string[this.attributes.Count];
				
				this.attributes.Keys.CopyTo (names, 0);
				System.Array.Sort (names);
				
				return names;
			}
		}
		
		
		public string GetAttribute(string name)
		{
			return this.GetAttribute (name, ResourceLevel.Merged);
		}
		
		public string GetAttribute(string name, ResourceLevel level)
		{
			if (this.attributes == null)
			{
				return null;
			}
			
			string find;
			
			switch (level)
			{
				case ResourceLevel.Default:		find = name; break;
				case ResourceLevel.Customised:	find = DbTools.BuildCompositeName (name, Resources.CustomisedSuffix);	break;
				case ResourceLevel.Localised:	find = DbTools.BuildCompositeName (name, Resources.LocalisedSuffix);	break;
				
				case ResourceLevel.Merged:
					
					//	Cas spécial: on veut trouver automatiquement l'attribut le meilleur dans
					//	ce contexte; commence par chercher la variante personnalisée, puis la
					//	variante localisée, pour enfin chercher la variante de base.
					
					find = DbTools.BuildCompositeName (name, Resources.CustomisedSuffix);
					if (this.attributes.Contains (find)) return this.attributes[find] as string;
					
					find = DbTools.BuildCompositeName (name, Resources.LocalisedSuffix);
					if (this.attributes.Contains (find)) return this.attributes[find] as string;
					
					find = name;
					break;
				
				default:
					throw new ResourceException ("Invalid ResourceLevel");
			}
			
			return (this.attributes.Contains (find)) ? this.attributes[find] as string : null;
		}
		
		
		public void SetAttribute(string name, string value)
		{
			this.SetAttribute (name, value, "");
		}
		
		public void SetAttribute(string name, string value, ResourceLevel level)
		{
			switch (level)
			{
				case ResourceLevel.Default:		this.SetAttribute (name, value, "");							break;
				case ResourceLevel.Customised:	this.SetAttribute (name, value, Resources.CustomisedSuffix);	break;
				case ResourceLevel.Localised:	this.SetAttribute (name, value, Resources.LocalisedSuffix);		break;
				
				default:
					throw new System.ArgumentException ("Unsupported ResourceLevel");
			}
		}
		
		public void SetAttribute(string name, string value, string localisation_suffix)
		{
			if (this.attributes == null)
			{
				this.attributes = new System.Collections.Hashtable ();
			}
			
			if (localisation_suffix != null)
			{
				name = DbTools.BuildCompositeName (name, localisation_suffix);
			}
			
			this.attributes[name] = value;
		}
		
		
		public const string						TagId			= "id";
		public const string						TagName			= "name";
		public const string						TagData			= "data";
		public const string						TagLabel		= "label";
		public const string						TagDescription	= "descr";
		
		protected System.Collections.Hashtable	attributes;

	}
}
