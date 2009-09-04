//	Copyright © 2005-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
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
		
		internal TextStyle(string name, TextStyleClass textStyleClass)
		{
			this.name             = name;
			this.textStyleClass = textStyleClass;
		}
		
		internal TextStyle(string name, TextStyleClass textStyleClass, System.Collections.ICollection properties) : base (properties)
		{
			this.name             = name;
			this.textStyleClass = textStyleClass;
			
			this.SaveStyleProperties (properties);
		}
		
		internal TextStyle(string name, TextStyleClass textStyleClass, System.Collections.ICollection properties, System.Collections.ICollection parentStyles)
		{
			this.name             = name;
			this.textStyleClass = textStyleClass;
			
			if ((parentStyles == null) ||
				(parentStyles.Count == 0))
			{
				this.Initialize (properties);
				this.SaveStyleProperties (properties);
			}
			else
			{
				this.Initialize (properties, parentStyles);
			}
		}
		
		
		public string							MetaId
		{
			get
			{
				return this.metaId;
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
				return this.textStyleClass;
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
				return this.isFlagged;
			}
		}
		
		public Property[]						StyleProperties
		{
			get
			{
				if (this.styleProperties != null)
				{
					return (Property[]) this.styleProperties.Clone ();
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
				if (this.parentStyles != null)
				{
					System.Collections.ArrayList list = new System.Collections.ArrayList ();
					
					foreach (TextStyle style in this.parentStyles)
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
				if (this.nextStyle == null)
				{
					return this.nextStyle;
				}
				else if (this.nextStyle.IsDeleted)
				{
					return null;
				}
				else
				{
					return this.nextStyle;
				}
			}
		}
		
		
		internal bool							IsDeleted
		{
			get
			{
				return this.isDeleted;
			}
			set
			{
				this.isDeleted = value;
			}
		}
		
		
		public static System.Collections.IComparer	Comparer
		{
			get
			{
				return new TextStyleComparer ();
			}
		}
		
		
		internal void DefineMetaId(string metaId)
		{
			this.metaId = metaId;
		}
		
		internal void DefinePriority(int priority)
		{
			this.priority = priority;
		}
		
		internal void DefineIsFlagged(bool value)
		{
			this.isFlagged = value;
		}
		
		internal void DefineNextStyle(TextStyle nextStyle)
		{
			this.nextStyle = nextStyle;
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
			
			this.parentStyles = null;
			this.styleProperties = null;
			this.nextStyle = null;
			this.Invalidate ();
		}
		
		internal void Initialize(System.Collections.ICollection properties, System.Collections.ICollection parentStyles)
		{
			if ((parentStyles == null) ||
				(parentStyles.Count == 0))
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
				
				this.parentStyles    = TextStyle.FilterNullStyles (parentStyles);
				this.styleProperties = new Property[properties.Count];
				
				properties.CopyTo (this.styleProperties, 0);
				
				this.GenerateStyleProperties ();
			}
		}
		
		internal void SaveStyleProperties(System.Collections.ICollection properties)
		{
			if (properties != null)
			{
				this.styleProperties = new Property[properties.Count];
				properties.CopyTo (this.styleProperties, 0);
			}
			else
			{
				this.styleProperties = new Property[0];
			}
		}
		
		
		public override void UpdateContentsSignature(Epsitec.Common.IO.IChecksum checksum)
		{
			//	Ignore le nom dans le calcul de la signature. C'est voulu !
			
			checksum.UpdateValue ((int) this.textStyleClass);
			
			if ((this.parentStyles != null) &&
				(this.parentStyles.Length > 0))
			{
				foreach (TextStyle style in this.parentStyles)
				{
					checksum.UpdateValue (style.GetContentsSignature ());
				}
			}
			
			base.UpdateContentsSignature (checksum);
		}
		
		public override bool Update(long currentVersion)
		{
			bool flag = this.IsFlagged;
			
			if (base.Update (currentVersion))
			{
				if ((this.parentStyles != null) &&
					(this.parentStyles.Length > 0))
				{
					foreach (TextStyle style in this.parentStyles)
					{
						style.Update (currentVersion);
						
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
			Property[] properties = this.styleProperties;
			
			if (properties == null)
			{
				properties = new Property[0];
			}
			
			int nStyles;
			
			nStyles  = this.parentStyles == null ? 0 : this.parentStyles.Length;
			nStyles += this.nextStyle == null ? 0 : 1;
			
			buffer.Append (SerializerSupport.SerializeString (this.name));
			buffer.Append ("/");
			buffer.Append (SerializerSupport.SerializeString (this.metaId));
			buffer.Append ("/");
			buffer.Append (SerializerSupport.SerializeInt (this.priority));
			buffer.Append ("/");
			buffer.Append (SerializerSupport.SerializeEnum (this.TextStyleClass));
			buffer.Append ("/");
			buffer.Append (SerializerSupport.SerializeInt (nStyles));
			buffer.Append ("/");
			buffer.Append (SerializerSupport.SerializeInt (properties == null ? 0 : properties.Length));
			
			if (this.parentStyles != null)
			{
				for (int i = 0; i < this.parentStyles.Length; i++)
				{
					TextStyle parent = this.parentStyles[i] as TextStyle;
					string    name   = TextStyle.GetStyleNameAndFilterDeletedStyles (parent);
					
					buffer.Append ("/");
					buffer.Append (SerializerSupport.SerializeString (name));
				}
			}

			//	S'il y a un style de paragraphe suivant chaîné à ce style, on le
			//	sérialise en utilisant le même principe que pour les styles parents,
			//	avec un préfixe "=>" :
			
			if (this.nextStyle != null)
			{
				string name = TextStyle.GetStyleNameAndFilterDeletedStyles (this.nextStyle);
				
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
			string metaId  = SerializerSupport.DeserializeString (args[offset++]);
			int    priority = SerializerSupport.DeserializeInt (args[offset++]);
			
			TextStyleClass tsc = (TextStyleClass) SerializerSupport.DeserializeEnum (typeof (TextStyleClass), args[offset++]);
			
			int nStyles = SerializerSupport.DeserializeInt (args[offset++]);
			int nProps  = SerializerSupport.DeserializeInt (args[offset++]);
			
			this.name     = name;
			this.metaId  = metaId;
			this.priority = priority;
			
			this.textStyleClass = tsc;
			
			//	S'il y a des styles "parents" pour le style courant, on récupère leur
			//	nom; ce n'est qu'au moment du DeserializeFixups que les noms seront
			//	remplacés par des instances d'objets réels :
			
			if (nStyles > 0)
			{
				this.parentStyles = new string[nStyles];
				
				for (int i = 0; i < nStyles; i++)
				{
					this.parentStyles[i] = SerializerSupport.DeserializeString (args[offset++]);
				}
			}
			
			//	Désérialise encore les propriétés propres au style, s'il y en a :
			
			if (nProps > 0)
			{
				this.styleProperties = new Property[nProps];
				
				for (int i = 0; i < nProps; i++)
				{
					string definition = SerializerSupport.DeserializeString (args[offset++]);
					this.styleProperties[i] = Property.Deserialize (context, version, definition);
				}
			}
			
			this.isFixupRequired = true;
		}
		
		internal void DeserializeFixups(StyleList list)
		{
			if (this.isFixupRequired)
			{
				this.isFixupRequired = false;
				
				if ((this.parentStyles != null) &&
					(this.parentStyles.Length > 0))
				{
					int    n         = this.parentStyles.Length;
					string lastName = this.parentStyles[n-1] as string;
					
					if ((lastName != null) && (lastName.StartsWith ("=>")))
					{
						this.nextStyle = list.GetTextStyle (lastName.Substring (2));
						this.nextStyle.DeserializeFixups (list);
						
						n--;
					}
					
					TextStyle[] parentStyles = new TextStyle[n];
					
					for (int i = 0; i < parentStyles.Length; i++)
					{
						string fullName = this.parentStyles[i] as string;
						
						if (fullName != null)
						{
							parentStyles[i] = list.GetTextStyle (fullName);
							
							//	Il faut s'assurer que le style duquel nous dérivons est
							//	prête à l'emploi :
							
							parentStyles[i].DeserializeFixups (list);
						}
					}
					
					Property[] styleProperties = this.styleProperties;
					
					this.parentStyles    = null;
					this.styleProperties = null;
					
					if (styleProperties == null)
					{
						styleProperties = new Property[0];
					}
					
					System.Diagnostics.Debug.Assert (styleProperties != null);
					System.Diagnostics.Debug.Assert (parentStyles != null);
					
					this.Initialize (styleProperties, parentStyles);
				}
				else
				{
					Property[] styleProperties = this.styleProperties;
					
					this.styleProperties = null;
					
					if (styleProperties == null)
					{
						styleProperties = new Property[0];
					}
					
					System.Diagnostics.Debug.Assert (styleProperties != null);
					
					this.Initialize (styleProperties);
					this.SaveStyleProperties (styleProperties);
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
				
				if (px.textStyleClass < py.textStyleClass)
				{
					return -1;
				}
				if (px.textStyleClass > py.textStyleClass)
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
			if ((a.textStyleClass == b.textStyleClass) &&
				(a.metaId == b.metaId) &&
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
		
		public static TextStyle[] FilterStyles(System.Collections.ICollection styles, params TextStyleClass[] textStyleClasses)
		{
			int count = 0;
			
			foreach (TextStyle style in styles)
			{
				for (int i = 0; i < textStyleClasses.Length; i++)
				{
					if (style.TextStyleClass == textStyleClasses[i])
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
				for (int i = 0; i < textStyleClasses.Length; i++)
				{
					if (style.TextStyleClass == textStyleClasses[i])
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
			
			System.Diagnostics.Debug.Assert (this.styleProperties != null);
			System.Diagnostics.Debug.Assert (this.parentStyles != null);
			
			Styles.PropertyContainer.Accumulator accumulator = new Accumulator ();
			
			foreach (TextStyle style in this.parentStyles)
			{
				accumulator.Accumulate (style);
			}
			
			accumulator.Accumulate (this.styleProperties);
			
			this.Initialize (accumulator.AccumulatedProperties);
			
			this.ClearContentsSignature ();
		}
		
		
		private string							name;
		private int								priority;
		private string							metaId;
		private TextStyleClass					textStyleClass;
		private object[]						parentStyles;
		private TextStyle						nextStyle;
		private Property[]						styleProperties;
		private System.Collections.ArrayList	wrappers;
		
		private bool							isFlagged;
		private bool							isFixupRequired;
		private bool							isDeleted;
	}
}
