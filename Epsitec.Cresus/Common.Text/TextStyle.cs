//	Copyright © 2005-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
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
			
			this.SaveStyleProperties (properties);
		}
		
		internal TextStyle(string name, TextStyleClass text_style_class, System.Collections.ICollection properties, System.Collections.ICollection parent_styles)
		{
			this.name             = name;
			this.text_style_class = text_style_class;
			
			if ((parent_styles == null) ||
				(parent_styles.Count == 0))
			{
				this.Initialize (properties);
				this.SaveStyleProperties (properties);
			}
			else
			{
				this.Initialize (properties, parent_styles);
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
		
		public Property[]						StyleProperties
		{
			get
			{
				if (this.style_properties != null)
				{
					return (Property[]) this.style_properties.Clone ();
				}
				else
				{
					return new Property[0];
				}
			}
		}
		
		public TextStyle[]						ParentStyles
		{
			get
			{
				if (this.parent_styles != null)
				{
					System.Collections.ArrayList list = new System.Collections.ArrayList ();
					
					foreach (TextStyle style in this.parent_styles)
					{
						if ((style != null) &&
							(style.IsDeleted == false))
						{
							list.Add (style);
						}
					}
					
					return (TextStyle[]) list.ToArray (typeof (TextStyle));
				}
				else
				{
					return new TextStyle[0];
				}
			}
		}
		
		public TextStyle						NextStyle
		{
			get
			{
				if (this.next_style == null)
				{
					return this.next_style;
				}
				else if (this.next_style.IsDeleted)
				{
					return null;
				}
				else
				{
					return this.next_style;
				}
			}
		}
		
		
		internal bool							IsDeleted
		{
			get
			{
				return this.is_deleted;
			}
			set
			{
				this.is_deleted = value;
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
		
		internal void DefineNextStyle(TextStyle next_style)
		{
			this.next_style = next_style;
		}
		
		
		internal void NotifyAttach(Wrappers.AbstractWrapper wrapper)
		{
			if (this.wrappers == null)
			{
				this.wrappers = new System.Collections.ArrayList ();
			}
			
			this.wrappers.Add (wrapper);
		}
		
		internal void NotifyDetach(Wrappers.AbstractWrapper wrapper)
		{
			this.wrappers.Remove (wrapper);
		}
		
		internal Wrappers.AbstractWrapper[] GetWrappers()
		{
			int n = (this.wrappers == null) ? 0 : this.wrappers.Count;
			
			Wrappers.AbstractWrapper[] wrappers = new Wrappers.AbstractWrapper[n];
			
			if (n > 0)
			{
				this.wrappers.CopyTo (wrappers, 0);
			}
			
			return wrappers;
		}
		
		
		internal void Clear()
		{
			//	Oublie les dépendances d'un style avec d'éventuels styles parents
			//	pour redevenir un style "plat".
			
			this.parent_styles = null;
			this.style_properties = null;
			this.next_style = null;
			this.Invalidate ();
		}
		
		internal void Initialize(System.Collections.ICollection properties, System.Collections.ICollection parent_styles)
		{
			if ((parent_styles == null) ||
				(parent_styles.Count == 0))
			{
				this.Initialize (properties);
				this.SaveStyleProperties (properties);
			}
			else
			{
				//	Initialise un style dérivé d'autres styles. Il faut d'une part
				//	prendre note des styles parents et d'autre part se souvenir des
				//	propriétés additionnelles. En effet, PropertyContainer stockant
				//	toutes les propriétés au même niveau, il ne serait pas possible
				//	d'en regénérer la liste si ces infos n'étaient pas conservées.
				
				this.parent_styles    = TextStyle.FilterNullStyles (parent_styles);
				this.style_properties = new Property[properties.Count];
				
				properties.CopyTo (this.style_properties, 0);
				
				this.GenerateStyleProperties ();
			}
		}
		
		internal void SaveStyleProperties(System.Collections.ICollection properties)
		{
			if (properties != null)
			{
				this.style_properties = new Property[properties.Count];
				properties.CopyTo (this.style_properties, 0);
			}
			else
			{
				this.style_properties = new Property[0];
			}
		}
		
		
		public override void UpdateContentsSignature(Epsitec.Common.IO.IChecksum checksum)
		{
			//	Ignore le nom dans le calcul de la signature. C'est voulu !
			
			checksum.UpdateValue ((int) this.text_style_class);
			
			if ((this.parent_styles != null) &&
				(this.parent_styles.Length > 0))
			{
				foreach (TextStyle style in this.parent_styles)
				{
					checksum.UpdateValue (style.GetContentsSignature ());
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
			Property[] properties = this.style_properties;
			
			if (properties == null)
			{
				properties = new Property[0];
			}
			
			int n_styles;
			
			n_styles  = this.parent_styles == null ? 0 : this.parent_styles.Length;
			n_styles += this.next_style == null ? 0 : 1;
			
			buffer.Append (SerializerSupport.SerializeString (this.name));
			buffer.Append ("/");
			buffer.Append (SerializerSupport.SerializeString (this.meta_id));
			buffer.Append ("/");
			buffer.Append (SerializerSupport.SerializeInt (this.priority));
			buffer.Append ("/");
			buffer.Append (SerializerSupport.SerializeEnum (this.TextStyleClass));
			buffer.Append ("/");
			buffer.Append (SerializerSupport.SerializeInt (n_styles));
			buffer.Append ("/");
			buffer.Append (SerializerSupport.SerializeInt (properties == null ? 0 : properties.Length));
			
			if (this.parent_styles != null)
			{
				for (int i = 0; i < this.parent_styles.Length; i++)
				{
					TextStyle parent = this.parent_styles[i] as TextStyle;
					string    name   = TextStyle.GetStyleNameAndFilterDeletedStyles (parent);
					
					buffer.Append ("/");
					buffer.Append (SerializerSupport.SerializeString (name));
				}
			}

			//	S'il y a un style de paragraphe suivant chaîné à ce style, on le
			//	sérialise en utilisant le même principe que pour les styles parents,
			//	avec un préfixe "=>" :
			
			if (this.next_style != null)
			{
				string name = TextStyle.GetStyleNameAndFilterDeletedStyles (this.next_style);
				
				System.Diagnostics.Debug.Assert (name != null);
				
				buffer.Append ("/");
				buffer.Append (SerializerSupport.SerializeString (string.Concat ("=>", name)));
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
			
			this.is_fixup_required = true;
		}
		
		internal void DeserializeFixups(StyleList list)
		{
			if (this.is_fixup_required)
			{
				this.is_fixup_required = false;
				
				if ((this.parent_styles != null) &&
					(this.parent_styles.Length > 0))
				{
					int    n         = this.parent_styles.Length;
					string last_name = this.parent_styles[n-1] as string;
					
					if ((last_name != null) && (last_name.StartsWith ("=>")))
					{
						this.next_style = list.GetTextStyle (last_name.Substring (2));
						this.next_style.DeserializeFixups (list);
						
						n--;
					}
					
					TextStyle[] parent_styles = new TextStyle[n];
					
					for (int i = 0; i < parent_styles.Length; i++)
					{
						string full_name = this.parent_styles[i] as string;
						
						if (full_name != null)
						{
							parent_styles[i] = list.GetTextStyle (full_name);
							
							//	Il faut s'assurer que le style duquel nous dérivons est
							//	prête à l'emploi :
							
							parent_styles[i].DeserializeFixups (list);
						}
					}
					
					Property[] style_properties = this.style_properties;
					
					this.parent_styles    = null;
					this.style_properties = null;
					
					if (style_properties == null)
					{
						style_properties = new Property[0];
					}
					
					System.Diagnostics.Debug.Assert (style_properties != null);
					System.Diagnostics.Debug.Assert (parent_styles != null);
					
					this.Initialize (style_properties, parent_styles);
				}
				else
				{
					Property[] style_properties = this.style_properties;
					
					this.style_properties = null;
					
					if (style_properties == null)
					{
						style_properties = new Property[0];
					}
					
					System.Diagnostics.Debug.Assert (style_properties != null);
					
					this.Initialize (style_properties);
					this.SaveStyleProperties (style_properties);
				}
			}
		}
		
		
		internal string SaveState(StyleList list)
		{
			System.Text.StringBuilder buffer = new System.Text.StringBuilder ();
			this.Serialize (buffer);
			return buffer.ToString ();
		}
		
		internal void RestoreState(StyleList list, string state)
		{
			string[] args    = state.Split ('/');
			int      version = TextContext.SerializationVersion;
			int      offset  = 0;
			
			this.Deserialize (list.TextContext, version, args, ref offset);
			this.DeserializeFixups (list);
		}
		
		
		private static string GetStyleNameAndFilterDeletedStyles(TextStyle style)
		{
			//	Retourne le nom du style; si le style a été détruit, on retourne
			//	le nom du style par défaut (que si c'est un style de paragraphe).
			
			System.Diagnostics.Debug.Assert (style != null);
			
			if (style.IsDeleted)
			{
				//	Remplace le style détruit par le style par défaut correspondant.
				
				if (style.TextStyleClass == TextStyleClass.Paragraph)
				{
					return "P.Default";
				}
				else
				{
					return null;
				}
			}
			else
			{
				return StyleList.GetFullName (style);
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
		
		
		public static TextStyle[] FilterNullStyles(System.Collections.ICollection styles)
		{
			//	Supprime les styles nuls (s'il y en a dans la collection).
			
			System.Collections.ArrayList list = new System.Collections.ArrayList ();
			
			foreach (TextStyle style in styles)
			{
				if (style != null)
				{
					list.Add (style);
				}
			}
			
			return (TextStyle[]) list.ToArray (typeof (TextStyle));
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
			
			this.Initialize (accumulator.AccumulatedProperties);
			
			this.ClearContentsSignature ();
		}
		
		
		private string							name;
		private int								priority;
		private string							meta_id;
		private TextStyleClass					text_style_class;
		private object[]						parent_styles;
		private TextStyle						next_style;
		private Property[]						style_properties;
		private System.Collections.ArrayList	wrappers;
		
		private bool							is_flagged;
		private bool							is_fixup_required;
		private bool							is_deleted;
	}
}
