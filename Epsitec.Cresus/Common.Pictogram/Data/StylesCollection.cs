using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Pictogram.Widgets;
using System.Xml.Serialization;

namespace Epsitec.Common.Pictogram.Data
{
	public delegate void StyleEventHandler(int styleID);

	/// <summary>
	/// La classe StylesCollection contient une collection de propri�t�s.
	/// </summary>

	public class StylesCollection
	{
		public StylesCollection()
		{
		}

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
		public UndoList Styles
		{
			//	Collection de tous les styles.
			get { return this.styles; }
			set { this.styles = value; }
		}

		[XmlIgnore]
		public int NextID
		{
			//	Donne un identificateur non encore utilis�.
			get { return this.nextID; }
			set { this.nextID = value; }
		}

		public void ClearProperty()
		{
			//	Supprime tous les styles.
			this.nextID = 1;
			this.styles.Clear();
			this.OnStyleListChanged();
		}

		public int TotalProperty
		{
			//	Retourne le nombre de styles.
			get { return this.styles.Count; }
		}

		public int AddProperty(AbstractProperty property)
		{
			//	Ajoute un nouveau style.
			return this.styles.Add(property);
		}

		public AbstractProperty GetProperty(int index)
		{
			//	Retourne un style existant.
			System.Diagnostics.Debug.Assert(index >= 0 && index < this.styles.Count);
			return this.styles[index] as AbstractProperty;
		}

		public int CreateProperty(AbstractProperty property)
		{
			//	Cr�e un nouveau style et retourne son rank.
			int id = this.nextID ++;
			property.StyleName = string.Format("{0} {1}", property.Text, id);
			property.StyleID = id;
			return this.AddProperty(property);
		}

		public void RemoveProperty(int index)
		{
			//	Supprime un style existant.
			System.Diagnostics.Debug.Assert(index >= 0 && index < this.styles.Count);
			this.styles.RemoveAt(index);
			if ( this.styles.Count == 0 )
			{
				this.nextID = 1;
			}
		}

		public void SwapProperty(int i, int j)
		{
			//	Permute deux styles existants.
			System.Diagnostics.Debug.Assert(i >= 0 && i < this.styles.Count);
			System.Diagnostics.Debug.Assert(j >= 0 && j < this.styles.Count);
			
			AbstractProperty temp = this.styles[i] as AbstractProperty;
			this.styles.RemoveAt(i);
			this.styles.Insert(j, temp);
		}

		public void ChangeProperty(AbstractProperty property)
		{
			//	Change un style apr�s une modification d'une propri�t� d'un objet.
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

		public AbstractProperty SearchProperty(AbstractProperty property)
		{
			//	Cherche une propri�t� d'apr�s son nom et ayant le m�me type.
			foreach ( AbstractProperty search in this.styles )
			{
				if ( search.Type      == property.Type      &&
					 search.StyleName == property.StyleName )  return search;
			}
			return null;
		}

		public void InitNextID()
		{
			//	Initialise le prochain identificateur.
			int max = 0;
			foreach ( AbstractProperty property in this.styles )
			{
				max = System.Math.Max(max, property.StyleID);
			}
			this.nextID = max+1;
		}

		public void CopyTo(StylesCollection dst)
		{
			//	Copie toute la collection.
			dst.styles.Clear();
			foreach ( AbstractProperty srcProp in this.styles )
			{
				AbstractProperty newProp = AbstractProperty.NewProperty(srcProp.Type);
				srcProp.CopyTo(newProp);
				dst.styles.Add(newProp);
			}

			dst.nextID = this.nextID;
		}

		public void CollectionChanged()
		{
			//	Signale que la collection a chang�.
			this.OnStyleListChanged();
		}

		public void UndoWillBeChanged(AbstractProperty property)
		{
			//	Indique qu'une propri�t� va changer, pour le undo.
			if ( property.StyleID == 0 )  return;
			this.styles.WillBeChanged(property.StyleID-1);
		}


		public VMenu CreateMenu(PropertyType type, int styleID)
		{
			//	Construit le menu des styles.
			VMenu menu = new VMenu();
			MenuItem item;

			if ( styleID == 0 )
			{
				item = new MenuItem("StyleMake(this.Name)", @"file:images/stylemake.icon", "Cr�er un nouveau style", "", AbstractProperty.TypeName(type));
				menu.Items.Add(item);
			}
			else
			{
				item = new MenuItem("StyleFree(this.Name)", @"file:images/stylefree.icon", "Rendre ind�pendant du style", "", AbstractProperty.TypeName(type));
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


		protected void OnStyleListChanged()
		{
			//	G�n�re un �v�nement pour dire que la liste de styles a chang�.
			if ( this.StyleListChanged != null )  // qq'un �coute ?
			{
				this.StyleListChanged(this);
			}
		}

		public event EventHandler StyleListChanged;


		//	G�n�re un �v�nement pour dire qu'un style de la collection a chang�.
		protected void OnOneStyleChanged(int styleID)
		{
			if ( this.OneStyleChanged != null )  // qq'un �coute ?
			{
				this.OneStyleChanged(styleID);
			}
		}

		public event StyleEventHandler OneStyleChanged;


		protected int					nextID = 1;
		protected UndoList				styles = new UndoList();
	}
}
