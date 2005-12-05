//	Copyright © 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text
{
	/// <summary>
	/// La classe TextStyle définit un style de texte de haut niveau. Il s'agit
	/// d'une collection de propriétés.
	/// </summary>
	public sealed class TextStyle : Styles.PropertyContainer, IContentsComparer
	{
		internal TextStyle()
		{
		}
		
		internal TextStyle(string name, TextStyleClass text_style_class)
		{
			this.name             = name;
			this.text_style_class = text_style_class;
		}
		
		internal TextStyle(string name, TextStyleClass text_style_class, System.Collections.ICollection properties) : base (properties)
		{
			this.name             = name;
			this.text_style_class = text_style_class;
		}
		
		internal TextStyle(string name, TextStyleClass text_style_class, System.Collections.ICollection properties, System.Collections.ICollection parent_styles)
		{
			this.name             = name;
			this.text_style_class = text_style_class;
			
			if ((parent_styles == null) ||
				(parent_styles.Count == 0))
			{
				base.Initialise (properties);
			}
			else
			{
				this.Initialise (properties, parent_styles);
			}
		}
		
		
		public string							MetaId
		{
			get
			{
				return this.meta_id;
			}
		}
		
		public int								Priority
		{
			get
			{
				return this.priority;
			}
		}
		
		public string							Name
		{
			get
			{
				return this.name;
			}
		}
		
		public TextStyleClass					TextStyleClass
		{
			get
			{
				return this.text_style_class;
			}
		}
		
		public bool								RequiresUniformParagraph
		{
			get
			{
				Property[] properties = this.GetProperties ();
				
				for (int i = 0; i < properties.Length; i++)
				{
					if (properties[i].RequiresUniformParagraph)
					{
						return true;
					}
				}
				
				return false;
			}
		}
		
		public bool								IsFlagged
		{
			get
			{
				return this.is_flagged;
			}
		}
		
		
		public static System.Collections.IComparer	Comparer
		{
			get
			{
				return new TextStyleComparer ();
			}
		}
		
		
		internal void DefineMetaId(string meta_id)
		{
			this.meta_id = meta_id;
		}
		
		internal void DefinePriority(int priority)
		{
			this.priority = priority;
		}
		
		internal void DefineIsFlagged(bool value)
		{
			this.is_flagged = value;
		}
		
		
		internal void Clear()
		{
			//	Oublie les dépendances d'un style avec d'éventuels styles parents
			//	pour redevenir un style "plat".
			
			this.parent_styles = null;
			this.style_properties = null;
			this.Invalidate ();
		}
		
		internal void Initialise(System.Collections.ICollection properties, System.Collections.ICollection parent_styles)
		{
			//	Initialise un style dérivé d'autres styles. Il faut d'une part
			//	prendre note des styles parents et d'autre part se souvenir des
			//	propriétés additionnelles. En effet, PropertyContainer stockant
			//	toutes les propriétés au même niveau, il ne serait pas possible
			//	d'en regénérer la liste si ces infos n'étaient pas conservées.
			
			this.parent_styles    = new TextStyle[parent_styles.Count];
			this.style_properties = new Property[properties.Count];
				
			parent_styles.CopyTo (this.parent_styles, 0);
			properties.CopyTo (this.style_properties, 0);
				
			this.GenerateStyleProperties ();
		}
		
		
		public override void UpdateContentsSignature(Epsitec.Common.IO.IChecksum checksum)
		{
			//	Ignore le nom dans le calcul de la signature. C'est voulu !
			
			checksum.Update ((int) this.text_style_class);
			
			if ((this.parent_styles != null) &&
				(this.parent_styles.Length > 0))
			{
				foreach (TextStyle style in this.parent_styles)
				{
					checksum.Update (style.GetContentsSignature ());
				}
			}
			
			base.UpdateContentsSignature (checksum);
		}
		
		public override bool Update(long current_version)
		{
			bool flag = this.IsFlagged;
			
			if (base.Update (current_version))
			{
				if ((this.parent_styles != null) &&
					(this.parent_styles.Length > 0))
				{
					foreach (TextStyle style in this.parent_styles)
					{
						style.Update (current_version);
						
						if (flag == false)
						{
							flag = style.IsFlagged;
						}
					}
				
					this.GenerateStyleProperties ();
				}
				
				this.DefineIsFlagged (flag);
				
				return true;
			}
			else
			{
				return false;
			}
		}

		public override string ToString()
		{
			System.Text.StringBuilder buffer = new System.Text.StringBuilder ();
			
			foreach (Property property in this)
			{
				buffer.Append (property.GetType ().Name);
				buffer.Append (":");
				property.SerializeToText (buffer);
				buffer.Append ("\n");
			}
			
			return buffer.ToString ();
		}
		
		
		internal void Serialize(System.Text.StringBuilder buffer)
		{
			Property[] properties = null;
			
			if ((this.style_properties == null) ||
				(this.style_properties.Length == 0))
			{
				properties = this.GetProperties ();
			}
			else
			{
				properties = this.style_properties;
			}
			
			buffer.Append (SerializerSupport.SerializeString (this.name));
			buffer.Append ("/");
			buffer.Append (SerializerSupport.SerializeString (this.meta_id));
			buffer.Append ("/");
			buffer.Append (SerializerSupport.SerializeInt (this.priority));
			buffer.Append ("/");
			buffer.Append (SerializerSupport.SerializeEnum (this.TextStyleClass));
			buffer.Append ("/");
			buffer.Append (SerializerSupport.SerializeInt (this.parent_styles == null ? 0 : this.parent_styles.Length));
			buffer.Append ("/");
			buffer.Append (SerializerSupport.SerializeInt (properties == null ? 0 : properties.Length));
			
			if (this.parent_styles != null)
			{
				for (int i = 0; i < this.parent_styles.Length; i++)
				{
					TextStyle parent = this.parent_styles[i] as TextStyle;
					
					buffer.Append ("/");
					buffer.Append (SerializerSupport.SerializeString (StyleList.GetFullName (parent.Name, parent.TextStyleClass)));
				}
			}
			
			if (properties != null)
			{
				for (int i = 0; i < properties.Length; i++)
				{
					Property property = properties[i];
				
					buffer.Append ("/");
					buffer.Append (SerializerSupport.SerializeString (Property.Serialize (property)));
				}
			}
		}
		
		internal void Deserialize(TextContext context, int version, string[] args, ref int offset)
		{
			string name     = SerializerSupport.DeserializeString (args[offset++]);
			string meta_id  = SerializerSupport.DeserializeString (args[offset++]);
			int    priority = SerializerSupport.DeserializeInt (args[offset++]);
			
			TextStyleClass tsc = (TextStyleClass) SerializerSupport.DeserializeEnum (typeof (TextStyleClass), args[offset++]);
			
			int n_styles = SerializerSupport.DeserializeInt (args[offset++]);
			int n_props  = SerializerSupport.DeserializeInt (args[offset++]);
			
			this.name     = name;
			this.meta_id  = meta_id;
			this.priority = priority;
			
			this.text_style_class = tsc;
			
			//	S'il y a des styles "parents" pour le style courant, on récupère leur
			//	nom; ce n'est qu'au moment du DeserializeFixups que les noms seront
			//	remplacés par des instances d'objets réels :
			
			if (n_styles > 0)
			{
				this.parent_styles = new string[n_styles];
				
				for (int i = 0; i < n_styles; i++)
				{
					this.parent_styles[i] = SerializerSupport.DeserializeString (args[offset++]);
				}
			}
			
			//	Désérialise encore les propriétés propres au style, s'il y en a :
			
			if (n_props > 0)
			{
				this.style_properties = new Property[n_props];
				
				for (int i = 0; i < n_props; i++)
				{
					string definition = SerializerSupport.DeserializeString (args[offset++]);
					this.style_properties[i] = Property.Deserialize (context, version, definition);
				}
			}
		}
		
		internal void DeserializeFixups(StyleList list)
		{
			if (this.parent_styles != null)
			{
				TextStyle[] parent_styles = new TextStyle[this.parent_styles.Length];
				
				for (int i = 0; i < this.parent_styles.Length; i++)
				{
					parent_styles[i] = list.GetTextStyle (this.parent_styles[i] as string);
				}
				
				Property[] style_properties = this.style_properties;
				
				this.parent_styles    = null;
				this.style_properties = null;
				
				this.Initialise (style_properties, parent_styles);
			}
			else
			{
				Property[] style_properties = this.style_properties;
				
				this.style_properties = null;
				
				this.Initialise (style_properties);
			}
		}
		
		
		#region IContentsComparer Members
		public bool CompareEqualContents(object value)
		{
			return TextStyle.CompareEqualContents (this, value as TextStyle);
		}
		#endregion
		
		#region TextStyleComparer Class
		private class TextStyleComparer : System.Collections.IComparer
		{
			#region IComparer Members
			public int Compare(object x, object y)
			{
				TextStyle px = x as TextStyle;
				TextStyle py = y as TextStyle;
				
				if (px.text_style_class < py.text_style_class)
				{
					return -1;
				}
				if (px.text_style_class > py.text_style_class)
				{
					return 1;
				}
				
				if (px.priority < py.priority)
				{
					return -1;
				}
				if (px.priority > py.priority)
				{
					return 1;
				}
				
				int result = string.Compare (px.MetaId, py.MetaId);
				
				if (result != 0)
				{
					return result;
				}
				
				return string.Compare (px.Name, py.Name);
			}
			#endregion
		}
		#endregion
		
		public static bool CompareEqualContents(TextStyle a, TextStyle b)
		{
			if ((a.text_style_class == b.text_style_class) &&
				(a.meta_id == b.meta_id) &&
				(a.priority == b.priority) &&
				(Styles.PropertyContainer.CompareEqualContents (a, b)))
			{
				//	Ignore le nom dans la comparaison du contenu. C'est voulu !
				
				//	TODO: compléter
				
				return true;
			}
			else
			{
				return false;
			}
		}
		
		
		public static TextStyle[] FilterStyles(System.Collections.ICollection styles, params TextStyleClass[] text_style_classes)
		{
			int count = 0;
			
			foreach (TextStyle style in styles)
			{
				for (int i = 0; i < text_style_classes.Length; i++)
				{
					if (style.TextStyleClass == text_style_classes[i])
					{
						count++;
						break;
					}
				}
			}
			
			TextStyle[] filtered = new TextStyle[count];
			
			int index = 0;
			
			foreach (TextStyle style in styles)
			{
				for (int i = 0; i < text_style_classes.Length; i++)
				{
					if (style.TextStyleClass == text_style_classes[i])
					{
						filtered[index++] = style;
						break;
					}
				}
			}
			
			return filtered;
		}
		
		
		private void GenerateStyleProperties()
		{
			//	Génère les propriétés accumulées/combinées correspondant à tous
			//	les styles parents et aux propriétés locales, puis initialise le
			//	PropertyContainer avec ces propriétés "à plat".
			
			System.Diagnostics.Debug.Assert (this.style_properties != null);
			System.Diagnostics.Debug.Assert (this.parent_styles != null);
			
			Styles.PropertyContainer.Accumulator accumulator = new Accumulator ();
			
			foreach (TextStyle style in this.parent_styles)
			{
				accumulator.Accumulate (style);
			}
			
			accumulator.Accumulate (this.style_properties);
			
			base.Initialise (accumulator.AccumulatedProperties);
			
			this.ClearContentsSignature ();
		}
		
		
		private string							name;
		private int								priority;
		private string							meta_id;
		private TextStyleClass					text_style_class;
		private object[]						parent_styles;
		private Property[]						style_properties;
		
		private bool							is_flagged;
	}
}
