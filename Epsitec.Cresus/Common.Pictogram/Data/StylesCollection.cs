using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Pictogram.Widgets;
using System.Xml.Serialization;

namespace Epsitec.Common.Pictogram.Data
{
	public delegate void StyleEventHandler(int styleID);

	/// <summary>
	/// La classe StylesCollection contient une collection de propriétés.
	/// </summary>

	public class StylesCollection
	{
		public StylesCollection()
		{
		}

		// Collection de tous les styles.
		[XmlArrayItem("Bool",     Type=typeof(PropertyBool))]
		[XmlArrayItem("Color",    Type=typeof(PropertyColor))]
		[XmlArrayItem("Double",   Type=typeof(PropertyDouble))]
		[XmlArrayItem("Gradient", Type=typeof(PropertyGradient))]
		[XmlArrayItem("Shadow",   Type=typeof(PropertyShadow))]
		[XmlArrayItem("Line",     Type=typeof(PropertyLine))]
		[XmlArrayItem("List",     Type=typeof(PropertyList))]
		[XmlArrayItem("Combo",    Type=typeof(PropertyCombo))]
		[XmlArrayItem("String",   Type=typeof(PropertyString))]
		[XmlArrayItem("Arrow",    Type=typeof(PropertyArrow))]
		[XmlArrayItem("Corner",   Type=typeof(PropertyCorner))]
		[XmlArrayItem("Regular",  Type=typeof(PropertyRegular))]
		[XmlArrayItem("ModColor", Type=typeof(PropertyModColor))]
		public System.Collections.ArrayList Styles
		{
			get { return this.styles; }
			set { this.styles = value; }
		}

		// Donne un identificateur non encore utilisé.
		[XmlIgnore]
		public int NextID
		{
			get { return this.nextID; }
			set { this.nextID = value; }
		}

		// Supprime tous les styles.
		public void ClearProperty()
		{
			this.nextID = 1;
			this.styles.Clear();
			this.OnStyleListChanged();
		}

		// Retourne le nombre de styles.
		public int TotalProperty
		{
			get { return this.styles.Count; }
		}

		// Ajoute un nouveau style.
		public int AddProperty(AbstractProperty property)
		{
			return this.styles.Add(property);
		}

		// Retourne un style existant.
		public AbstractProperty GetProperty(int index)
		{
			System.Diagnostics.Debug.Assert(index >= 0 && index < this.styles.Count);
			return this.styles[index] as AbstractProperty;
		}

		// Crée un nouveau style et retourne son rank.
		public int CreateProperty(AbstractProperty property)
		{
			int id = this.nextID ++;
			property.StyleName = string.Format("{0} {1}", property.Text, id);
			property.StyleID = id;
			return this.AddProperty(property);
		}

		// Supprime un style existant.
		public void RemoveProperty(int index)
		{
			System.Diagnostics.Debug.Assert(index >= 0 && index < this.styles.Count);
			this.styles.RemoveAt(index);
			if ( this.styles.Count == 0 )
			{
				this.nextID = 1;
			}
		}

		// Permute deux styles existants.
		public void SwapProperty(int i, int j)
		{
			System.Diagnostics.Debug.Assert(i >= 0 && i < this.styles.Count);
			System.Diagnostics.Debug.Assert(j >= 0 && j < this.styles.Count);
			AbstractProperty temp = this.styles[i] as AbstractProperty;
			this.styles[i] = this.styles[j];
			this.styles[j] = temp;
		}

		// Change un style après une modification d'une propriété d'un objet.
		public void ChangeProperty(AbstractProperty property)
		{
			foreach ( AbstractProperty p in this.styles )
			{
				if ( p.StyleID == property.StyleID )
				{
					System.Diagnostics.Debug.Assert(p.Type == property.Type);
					property.CopyTo(p);
					this.OnOneStyleChanged(property.StyleID);
					return;
				}
			}
		}

		// Adapte toutes les propriétés pour leurs redonner les informations communes.
		public void AdaptInfoProperties(Drawer drawer)
		{
			foreach ( AbstractProperty property in this.styles )
			{
				AbstractProperty refProp = drawer.NewProperty(property.Type);
				if ( refProp == null )  continue;
				refProp.CopyInfoTo(property);
			}
		}

		// Cherche une propriété d'après son nom et ayant le même type.
		public AbstractProperty SearchProperty(AbstractProperty property)
		{
			foreach ( AbstractProperty search in this.styles )
			{
				if ( search.Type      == property.Type      &&
					 search.StyleName == property.StyleName )  return search;
			}
			return null;
		}

		// Initialise le prochain identificateur.
		public void InitNextID()
		{
			int max = 0;
			foreach ( AbstractProperty property in this.styles )
			{
				max = System.Math.Max(max, property.StyleID);
			}
			this.nextID = max+1;
		}

		// Copie toute la collection.
		public void CopyTo(StylesCollection dst)
		{
			dst.styles.Clear();
			foreach ( AbstractProperty srcProp in this.styles )
			{
				AbstractProperty newProp = AbstractProperty.NewProperty(srcProp.Type);
				srcProp.CopyTo(newProp);
				dst.styles.Add(newProp);
			}

			dst.nextID = this.nextID;
		}

		// Signale que la collection a changé.
		public void CollectionChanged()
		{
			this.OnStyleListChanged();
		}


		// Construit le menu des styles.
		public VMenu CreateMenu(PropertyType type, int styleID)
		{
			VMenu menu = new VMenu();
			MenuItem item;

			if ( styleID == 0 )
			{
				item = new MenuItem("StyleMake(this.Name)", @"file:images/stylemake.icon", "Créer un nouveau style", "", AbstractProperty.TypeName(type));
				menu.Items.Add(item);
			}
			else
			{
				item = new MenuItem("StyleFree(this.Name)", @"file:images/stylefree.icon", "Rendre indépendant du style", "", AbstractProperty.TypeName(type));
				menu.Items.Add(item);
			}

			bool first = true;
			for ( int i=0 ; i<this.styles.Count ; i++ )
			{
				AbstractProperty property = this.styles[i] as AbstractProperty;
				if ( property.Type != type )  continue;

				if ( first )
				{
					menu.Items.Add(new MenuSeparator());
					first = false;
				}

				string icon = @"file:images/activeno.icon";
				if ( property.StyleID == styleID )
				{
					icon = @"file:images/activeyes.icon";
				}

				item = new MenuItem("StyleUse(this.Name)", icon, property.StyleName, "", i.ToString());
				menu.Items.Add(item);
			}

			menu.AdjustSize();
			return menu;
		}


		// Génère un événement pour dire que la liste de styles a changé.
		protected void OnStyleListChanged()
		{
			if ( this.StyleListChanged != null )  // qq'un écoute ?
			{
				this.StyleListChanged(this);
			}
		}

		public event EventHandler StyleListChanged;


		// Génère un événement pour dire qu'un style de la collection a changé.
		protected void OnOneStyleChanged(int styleID)
		{
			if ( this.OneStyleChanged != null )  // qq'un écoute ?
			{
				this.OneStyleChanged(styleID);
			}
		}

		public event StyleEventHandler OneStyleChanged;


		protected int							nextID = 1;
		protected System.Collections.ArrayList	styles = new System.Collections.ArrayList();
	}
}
