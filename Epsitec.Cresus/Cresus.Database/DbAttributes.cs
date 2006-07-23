//	Copyright © 2003-2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using Epsitec.Common.Support;

namespace Epsitec.Cresus.Database
{
	/// <summary>
	/// La classe DbAttributes représente des attributs qui peuvent exister
	/// en diverses variantes (localisation).
	/// </summary>
	public class DbAttributes : System.ICloneable
	{
		public DbAttributes()
		{
		}
		
		public DbAttributes(params string[] list) : this ()
		{
			this.SetFromInitialisationList (list);
		}
		
		
		public string[]							Names
		{
			get
			{
				if ((this.attributes == null) ||
					(this.attributes.Count == 0))
				{
					return new string[0];
				}
				
				string[] names = new string[this.attributes.Count];
				
				this.attributes.Keys.CopyTo (names, 0);
				System.Array.Sort (names);
				
				return names;
			}
		}
		
		
		public string							this[string name]
		{
			get { return this.GetAttribute (name); }
			set { this.SetAttribute (name, value); }
		}
		
		public string							this[string name, Epsitec.Common.Support.ResourceLevel level]
		{
			get { return this.GetAttribute (name, level); }
			set { this.SetAttribute (name, value, level); }
		}
		
		public string							this[string name, string suffix]
		{
			get { return this.GetAttribute (name, suffix); }
			set { this.SetAttribute (name, value, suffix); }
		}
		
		
		#region ICloneable Members
		public object Clone()
		{
			return this.CloneCopyToNewObject (this.CloneNewObject ());
		}
		#endregion
		
		protected virtual object CloneNewObject()
		{
			return new DbAttributes ();
		}
		
		protected virtual object CloneCopyToNewObject(object o)
		{
			DbAttributes that = o as DbAttributes;
			
			that.attributes = (this.attributes == null) ? null : this.attributes.Clone () as System.Collections.Hashtable;
			
			return that;
		}
		
		
		#region ToString override
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
		#endregion
		
		internal void SetFromInitialisationList(params string[] list)
		{
			for (int i = 0; i < list.Length; i++)
			{
				string[] args = Common.Support.Utilities.Split (list[i], '=');
				
				if (args.Length != 2)
				{
					throw new System.ArgumentException (string.Format ("Invalid attribute initialisation syntax in '{0}'.", list[i]));
				}
				
				string name  = args[0];
				string value = Common.Support.Utilities.StringSimplify (args[1]);
				
				this.SetAttribute (name, value);
			}
		}
		
		
		internal string GetAttribute(string name)
		{
			return this.GetAttribute (name, ResourceLevel.Merged);
		}
		
		internal string GetAttribute(string name, ResourceLevel level)
		{
			if (this.attributes == null)
			{
				return null;
			}
			
			string find;
			
			switch (level)
			{
				case ResourceLevel.Merged:
					
					//	Cas spécial: on veut trouver automatiquement l'attribut le meilleur dans
					//	ce contexte; commence par chercher la variante personnalisée, puis la
					//	variante localisée, pour enfin chercher la variante de base.
					
					find = DbTools.BuildLocalisedName (name, ResourceLevel.Customized);
					
					if (this.attributes.Contains (find))
					{
						return this.attributes[find] as string;
					}
					
					
					find = DbTools.BuildLocalisedName (name, ResourceLevel.Localized);
					
					if (this.attributes.Contains (find))
					{
						return this.attributes[find] as string;
					}
					
					find = DbTools.BuildLocalisedName (name, ResourceLevel.Default);
					break;
				
				default:
					find = DbTools.BuildLocalisedName (name, level);
					break;
			}
			
			return (this.attributes.Contains (find)) ? this.attributes[find] as string : null;
		}
		
		internal string GetAttribute(string name, string suffix)
		{
			if (suffix == null)
			{
				return this.GetAttribute (name);
			}
			else
			{
				return this.GetAttribute (DbTools.BuildCompositeName (name, suffix));
			}
		}
		
		
		internal void SetAttribute(string name, string value, ResourceLevel level)
		{
			this.SetAttribute (DbTools.BuildLocalisedName (name, level), value);
		}
		
		internal void SetAttribute(string name, string value)
		{
			if (this.attributes == null)
			{
				this.attributes = new System.Collections.Hashtable ();
			}
			
			this.attributes[name] = value;
		}
		
		
		internal void SetAttribute(string name, string value, string suffix)
		{
			if (suffix == null)
			{
				this.SetAttribute (name, value);
			}
			else
			{
				this.SetAttribute (DbTools.BuildCompositeName (name, suffix), value);
			}
		}
		
		internal void SerializeXmlAttributes(System.Text.StringBuilder buffer)
		{
			string[] names = this.Names;
			
			for (int i = 0; i < names.Length; i++)
			{
				buffer.Append (" attr.");
				buffer.Append (names[i]);
				buffer.Append (@"=""");
				buffer.Append (Common.Support.Utilities.TextToXml (this.GetAttribute (names[i])));
				buffer.Append (@"""");
			}
		}
		
		internal void DeserializeXmlAttributes(System.Xml.XmlElement xml)
		{
			foreach (System.Xml.XmlAttribute attr in xml.Attributes)
			{
				if (attr.Name.StartsWith ("attr."))
				{
					string name  = attr.Name.Substring (5);
					string value = attr.Value;
					
					this.SetAttribute (name, value);
				}
			}
		}
		
		
		
		protected System.Collections.Hashtable	attributes;
	}
}
