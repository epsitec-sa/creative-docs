using Epsitec.Common.Widgets;
using System.Xml.Serialization;
using System.IO;

namespace Epsitec.Common.Pictogram.Data
{
	/// <summary>
	/// La classe IconObjects contient tous les objets qui forment une ic�ne.
	/// </summary>

	[XmlRootAttribute("EpsitecPictogram")]

	[
	XmlInclude(typeof(ObjectBezier)),
	XmlInclude(typeof(ObjectCircle)),
	XmlInclude(typeof(ObjectEllipse)),
	XmlInclude(typeof(ObjectGroup)),
	XmlInclude(typeof(ObjectLayer)),
	XmlInclude(typeof(ObjectLine)),
	XmlInclude(typeof(ObjectPage)),
	XmlInclude(typeof(ObjectPoly)),
	XmlInclude(typeof(ObjectRectangle)),
	XmlInclude(typeof(ObjectRegular)),
	XmlInclude(typeof(ObjectText)),

	XmlInclude(typeof(Handle)),

	XmlInclude(typeof(PropertyBool)),
	XmlInclude(typeof(PropertyColor)),
	XmlInclude(typeof(PropertyDouble)),
	XmlInclude(typeof(PropertyGradient)),
	XmlInclude(typeof(PropertyShadow)),
	XmlInclude(typeof(PropertyLine)),
	XmlInclude(typeof(PropertyList)),
	XmlInclude(typeof(PropertyCombo)),
	XmlInclude(typeof(PropertyString)),
	XmlInclude(typeof(PropertyArrow)),
	XmlInclude(typeof(PropertyCorner)),
	XmlInclude(typeof(PropertyRegular)),
	XmlInclude(typeof(PropertyModColor)),
	]

	public class IconObjects
	{
		public IconObjects()
		{
		}

		// Taille pr�f�rentielle de l'ic�ne en pixels.
		public Drawing.Size Size
		{
			get
			{
				return this.size;
			}

			set
			{
				this.size = value;
				this.sizeArea.Width = this.size.Width*3;  // TODO: provisoire !
				this.sizeArea.Height = this.size.Height*3;
			}
		}

		// Taille de la zone de travail en pixels.
		public Drawing.Size SizeArea
		{
			get
			{
				return this.sizeArea;
			}
		}

		// Origine de la zone de travail en pixels.
		public Drawing.Point OriginArea
		{
			get
			{
				Drawing.Point origin = new Drawing.Point();
				origin.X = -(this.sizeArea.Width-this.size.Width)/2;
				origin.Y = -(this.sizeArea.Height-this.size.Height)/2;
				return origin;
			}
		}

		// Origine de l'ic�ne en pixels (hot spot).
		public Drawing.Point Origin
		{
			get
			{
				return this.origin;
			}

			set
			{
				this.origin = value;
			}
		}

		// Collection des styles.
		public StylesCollection StylesCollection
		{
			get { return this.styles; }
			set { this.styles = value; }
		}
		
		// Liste des objets.
		[XmlArrayItem("Bezier",    Type=typeof(ObjectBezier))]
		[XmlArrayItem("Circle",    Type=typeof(ObjectCircle))]
		[XmlArrayItem("Ellipse",   Type=typeof(ObjectEllipse))]
		[XmlArrayItem("Group",     Type=typeof(ObjectGroup))]
		[XmlArrayItem("Layer",     Type=typeof(ObjectLayer))]
		[XmlArrayItem("Line",      Type=typeof(ObjectLine))]
		[XmlArrayItem("Page",      Type=typeof(ObjectPage))]
		[XmlArrayItem("Polyline",  Type=typeof(ObjectPoly))]
		[XmlArrayItem("Rectangle", Type=typeof(ObjectRectangle))]
		[XmlArrayItem("Polygon",   Type=typeof(ObjectRegular))]
		[XmlArrayItem("Text",      Type=typeof(ObjectText))]
		public System.Collections.ArrayList Objects
		{
			get { return this.objects; }
			set { this.objects = value; }
		}

		public AbstractObject this[int index]
		{
			get
			{
				System.Diagnostics.Debug.Assert(this.CurrentGroup[index] != null);
				return this.CurrentGroup[index] as AbstractObject;
			}
			
			set
			{
				System.Diagnostics.Debug.Assert(this.CurrentGroup[index] != null);
				this.CurrentGroup[index] = value;
			}
		}

		public int Count
		{
			get { return this.CurrentGroup.Count; }
		}

		public void Clear()
		{
			this.objects.Clear();
			ObjectPage  page  = new ObjectPage();
			ObjectLayer layer = new ObjectLayer();
			this.objects.Add(page);
			page.Objects.Add(layer);
			this.UsePageLayer(0, 0);

			this.styles.ClearProperty();
		}

		public int Add(AbstractObject obj)
		{
			return this.CurrentGroup.Add(obj);
		}

		public void Insert(int index, AbstractObject obj)
		{
			this.CurrentGroup.Insert(index, obj);
		}

		public void RemoveAt(int index)
		{
			AbstractObject obj = this.CurrentGroup[index] as AbstractObject;
			if ( obj != null )  obj.Dispose();
			this.CurrentGroup.RemoveAt(index);
		}


		// Retourne le nombre total d'objets.
		public int TotalCount()
		{
			return this.TotalCount(this.objects);
		}

		protected int TotalCount(System.Collections.ArrayList objects)
		{
			int total = objects.Count;
			int count = 0;
			for ( int index=0 ; index<total ; index++ )
			{
				count ++;

				AbstractObject obj = objects[index] as AbstractObject;
				if ( obj.Objects != null && obj.Objects.Count > 0 )
				{
					count += this.TotalCount(obj.Objects);
				}
			}
			return count;
		}


		// Retourne le nombre d'objets s�lectionn�s.
		public int TotalSelected()
		{
			return this.TotalSelected(this.CurrentGroup);
		}

		protected int TotalSelected(System.Collections.ArrayList objects)
		{
			int total = objects.Count;
			int count = 0;
			for ( int index=0 ; index<total ; index++ )
			{
				AbstractObject obj = objects[index] as AbstractObject;
				if ( obj.IsSelected() )  count ++;
			}
			return count;
		}


		// Ajoute les propri�t�s des objets s�lectionn�s dans la liste.
		// Un type de propri�t� donn� n'est qu'une fois dans la liste.
		public void PropertiesList(System.Collections.ArrayList list,
								   AbstractObject objectMemory)
		{
			this.PropertiesList(this.CurrentGroup, list, objectMemory, false);
		}

		protected void PropertiesList(System.Collections.ArrayList objects,
									  System.Collections.ArrayList list,
									  AbstractObject objectMemory,
									  bool all)
		{
			int total = objects.Count;
			for ( int index=0 ; index<total ; index++ )
			{
				AbstractObject obj = objects[index] as AbstractObject;
				if ( !all && !obj.IsSelected() )  continue;

				obj.CloneInfoProperties(objectMemory);

				int tp = obj.TotalProperty;
				for ( int i=0 ; i<tp ; i++ )
				{
					AbstractProperty property = obj.Property(i);
					AbstractProperty existing = property.Search(list);
					if ( existing == null )
					{
						property.Multi = false;
						list.Add(property);
					}
					else
					{
						if ( !property.Compare(existing) )
						{
							existing.Multi = true;
						}
					}
				}

				if ( obj.Objects != null && obj.Objects.Count > 0 )
				{
					this.PropertiesList(obj.Objects, list, objectMemory, true);
				}
			}
		}


		// Modifie juste l'�tat "�tendu" d'une propri�t�.
		public void SetPropertyExtended(AbstractProperty property)
		{
			this.SetPropertyExtended(this.CurrentGroup, property, false);
		}

		protected void SetPropertyExtended(System.Collections.ArrayList objects, AbstractProperty property, bool all)
		{
			int total = objects.Count;
			for ( int index=0 ; index<total ; index++ )
			{
				AbstractObject obj = objects[index] as AbstractObject;
				if ( !all && !obj.IsSelected() )  continue;

				obj.SetPropertyExtended(property);

				if ( obj.Objects != null && obj.Objects.Count > 0 )
				{
					this.SetPropertyExtended(obj.Objects, property, true);
				}
			}
		}


		// Modifie une propri�t� des objets s�lectionn�s.
		public void SetProperty(AbstractProperty property, ref Drawing.Rectangle bbox, bool changeStylesCollection)
		{
			if ( property.StyleID == 0 )  // propri�t� ind�pendante ?
			{
				this.SetPropertyFree(this.CurrentGroup, property, ref bbox, false);
			}
			else	// propri�t� li�e � un style ?
			{
				this.SetPropertyStyle(this.objects, property, ref bbox);

				if ( changeStylesCollection )
				{
					this.styles.ChangeProperty(property);
				}
			}
		}

		// Mets une propri�t� piqu�e avec la pipette aux objets s�lectionn�s.
		public void SetPropertyPicker(AbstractProperty property, ref Drawing.Rectangle bbox)
		{
			this.SetPropertyFree(this.CurrentGroup, property, ref bbox, false);
		}

		protected void SetPropertyFree(System.Collections.ArrayList objects, AbstractProperty property, ref Drawing.Rectangle bbox, bool all)
		{
			int total = objects.Count;
			for ( int index=0 ; index<total ; index++ )
			{
				AbstractObject obj = objects[index] as AbstractObject;
				if ( !all && !obj.IsSelected() )  continue;

				bbox.MergeWith(obj.BoundingBox);
				obj.SetProperty(property);
				bbox.MergeWith(obj.BoundingBox);

				ObjectGroup group = obj as ObjectGroup;
				if ( group != null && group.Objects != null && group.Objects.Count > 0 )
				{
					this.SetPropertyFree(group.Objects, property, ref bbox, true);

					Drawing.Rectangle gbox = this.RetBbox(group.Objects);
					group.UpdateDim(gbox);
					bbox.MergeWith(gbox);
				}
			}
		}

		protected void SetPropertyStyle(System.Collections.ArrayList objects, AbstractProperty property, ref Drawing.Rectangle bbox)
		{
			int total = objects.Count;
			for ( int index=0 ; index<total ; index++ )
			{
				AbstractObject obj = objects[index] as AbstractObject;

				if ( obj.IsLinkProperty(property) )
				{
					bbox.MergeWith(obj.BoundingBox);
					obj.SetProperty(property);
					bbox.MergeWith(obj.BoundingBox);
				}

				if ( obj.Objects != null && obj.Objects.Count > 0 )
				{
					this.SetPropertyStyle(obj.Objects, property, ref bbox);

					ObjectGroup group = obj as ObjectGroup;
					if ( group != null )
					{
						Drawing.Rectangle gbox = this.RetBbox(group.Objects);
						group.UpdateDim(gbox);
						bbox.MergeWith(gbox);
					}
				}
			}
		}


		// Retourne une propri�t�.
		public AbstractProperty GetProperty(PropertyType type, AbstractObject objectMemory)
		{
			return this.GetProperty(this.CurrentGroup, type, objectMemory, false);
		}

		protected AbstractProperty GetProperty(System.Collections.ArrayList objects, PropertyType type, AbstractObject objectMemory, bool all)
		{
			int total = objects.Count;
			for ( int index=0 ; index<total ; index++ )
			{
				AbstractObject obj = objects[index] as AbstractObject;
				if ( !all && !obj.IsSelected() )  continue;

				AbstractProperty property = obj.GetProperty(type);
				if ( property != null )
				{
					objectMemory.SetProperty(property);  // m�morise l'�tat
					return property;
				}

				if ( obj.Objects != null && obj.Objects.Count > 0 )
				{
					AbstractProperty p = this.GetProperty(obj.Objects, type, objectMemory, true);
					if ( p != null )  return p;
				}
			}
			return null;
		}


		// Lib�re les objets s�lectionn�s du style.
		public void StyleFree(PropertyType type)
		{
			this.StyleFree(this.CurrentGroup, type, false);
		}

		protected void StyleFree(System.Collections.ArrayList objects, PropertyType type, bool all)
		{
			int total = objects.Count;
			for ( int index=0 ; index<total ; index++ )
			{
				AbstractObject obj = objects[index] as AbstractObject;
				if ( !all && !obj.IsSelected() )  continue;

				AbstractProperty property = obj.GetProperty(type);
				if ( property != null )
				{
					property.StyleName = "";
					property.StyleID = 0;
					obj.SetProperty(property);
				}

				if ( obj.Objects != null && obj.Objects.Count > 0 )
				{
					this.StyleFree(obj.Objects, type, true);
				}
			}
		}

		// Lib�re tous les objets du style.
		public void StyleFreeAll(AbstractProperty property)
		{
			this.StyleFreeAll(this.CurrentGroup, property);
		}

		protected void StyleFreeAll(System.Collections.ArrayList objects, AbstractProperty property)
		{
			int total = objects.Count;
			for ( int index=0 ; index<total ; index++ )
			{
				AbstractObject obj = objects[index] as AbstractObject;
				if ( !obj.IsLinkProperty(property) )  continue;

				AbstractProperty objProp = obj.GetProperty(property.Type);
				if ( objProp != null )
				{
					objProp.StyleName = "";
					objProp.StyleID = 0;
					obj.SetProperty(objProp);
				}

				if ( obj.Objects != null && obj.Objects.Count > 0 )
				{
					this.StyleFreeAll(obj.Objects, property);
				}
			}
		}


		// Utilise un style donn�.
		public void StyleUse(AbstractProperty property)
		{
			this.StyleUse(this.CurrentGroup, property, false);
		}

		protected void StyleUse(System.Collections.ArrayList objects, AbstractProperty property, bool all)
		{
			int total = objects.Count;
			for ( int index=0 ; index<total ; index++ )
			{
				AbstractObject obj = objects[index] as AbstractObject;
				if ( !all && !obj.IsSelected() )  continue;

				obj.SetProperty(property);

				if ( obj.Objects != null && obj.Objects.Count > 0 )
				{
					this.StyleUse(obj.Objects, property, true);
				}
			}
		}


		// Retourne la bounding box de tous les objets.
		public Drawing.Rectangle RetBbox()
		{
			return this.RetBbox(this.CurrentGroup);
		}

		protected Drawing.Rectangle RetBbox(System.Collections.ArrayList objects)
		{
			int total = objects.Count;
			Drawing.Rectangle bbox = Drawing.Rectangle.Empty;
			for ( int index=0 ; index<total ; index++ )
			{
				AbstractObject obj = objects[index] as AbstractObject;
				bbox.MergeWith(obj.BoundingBoxGeom);
			}
			return bbox;
		}


		// Retourne la bounding box des objets s�lectionn�s.
		public Drawing.Rectangle RetSelectedBbox()
		{
			return this.RetSelectedBbox(this.CurrentGroup);
		}

		protected Drawing.Rectangle RetSelectedBbox(System.Collections.ArrayList objects)
		{
			int total = objects.Count;
			Drawing.Rectangle bbox = Drawing.Rectangle.Empty;
			for ( int index=0 ; index<total ; index++ )
			{
				AbstractObject obj = objects[index] as AbstractObject;
				if ( !obj.IsSelected() )  continue;

				bbox.MergeWith(obj.BoundingBoxGeom);
			}
			return bbox;
		}


		// D�tecte l'objet point� par la souris.
		public AbstractObject Detect(Drawing.Point mouse)
		{
			return this.Detect(this.CurrentGroup, mouse);
		}

		protected AbstractObject Detect(System.Collections.ArrayList objects, Drawing.Point mouse)
		{
			int total = objects.Count;
			for ( int index=total-1 ; index>=0 ; index-- )
			{
				AbstractObject obj = objects[index] as AbstractObject;

				if ( obj.Detect(mouse) )  return obj;
			}
			return null;
		}


		// D�tecte en porfondeur l'objet point� par la souris.
		public AbstractObject DeepDetect(Drawing.Point mouse)
		{
			return this.DeepDetect(this.CurrentGroup, mouse);
		}

		protected AbstractObject DeepDetect(System.Collections.ArrayList objects, Drawing.Point mouse)
		{
			int total = objects.Count;
			for ( int index=total-1 ; index>=0 ; index-- )
			{
				AbstractObject obj = objects[index] as AbstractObject;

				if ( obj is ObjectGroup )
				{
					if ( obj.Objects != null && obj.Objects.Count > 0 )
					{
						AbstractObject item = this.DeepDetect(obj.Objects, mouse);
						if ( item != null )  return item;
					}
				}
				else
				{
					if ( obj.Detect(mouse) )  return obj;
				}
			}
			return null;
		}


		// D�tecte la poign�e point�e par la souris.
		public bool DetectHandle(Drawing.Point mouse, out AbstractObject obj, out int rank)
		{
			return this.DetectHandle(this.CurrentGroup, mouse, out obj, out rank);
		}

		// D�tecte la poign�e point�e par la souris.
		protected bool DetectHandle(System.Collections.ArrayList objects, Drawing.Point mouse, out AbstractObject obj, out int rank)
		{
			int total = objects.Count;
			for ( int index=total-1 ; index>=0 ; index-- )
			{
				obj = objects[index] as AbstractObject;

				if ( !obj.IsSelected() )  continue;

				rank = obj.DetectHandle(mouse);
				if ( rank != -1 )  return true;
			}

			obj = null;
			rank = -1;
			return false;
		}


		// Hilite un objet.
		public void Hilite(AbstractObject item, ref Drawing.Rectangle bbox)
		{
			this.Hilite(this.CurrentGroup, item, ref bbox);
		}

		protected void Hilite(System.Collections.ArrayList objects, AbstractObject item, ref Drawing.Rectangle bbox)
		{
			int total = objects.Count;
			for ( int index=0 ; index<total ; index++ )
			{
				AbstractObject obj = objects[index] as AbstractObject;
				if ( obj.IsHilite != (obj == item) )
				{
					obj.IsHilite = (obj == item);
					bbox.MergeWith(obj.BoundingBox);
				}
			}
		}


		// Hilite un objet en profondeur.
		public void DeepHilite(AbstractObject item, ref Drawing.Rectangle bbox)
		{
			this.DeepHilite(this.CurrentGroup, item, ref bbox);
		}

		protected void DeepHilite(System.Collections.ArrayList objects, AbstractObject item, ref Drawing.Rectangle bbox)
		{
			int total = objects.Count;
			for ( int index=0 ; index<total ; index++ )
			{
				AbstractObject obj = objects[index] as AbstractObject;

				if ( obj.IsHilite != (obj == item) )
				{
					obj.IsHilite = (obj == item);
					bbox.MergeWith(obj.BoundingBox);
				}

				if ( obj.Objects != null && obj.Objects.Count > 0 )
				{
					this.DeepHilite(obj.Objects, item, ref bbox);
				}
			}
		}


		// Retourne le premier objet s�lectionn�.
		public AbstractObject RetFirstSelected()
		{
			return this.RetFirstSelected(this.CurrentGroup);
		}

		protected AbstractObject RetFirstSelected(System.Collections.ArrayList objects)
		{
			int total = objects.Count;
			for ( int index=0 ; index<total ; index++ )
			{
				AbstractObject obj = objects[index] as AbstractObject;
				if ( obj.IsSelected() )  return obj;

				if ( obj.Objects != null && obj.Objects.Count > 0 )
				{
					AbstractObject item = this.RetFirstSelected(obj.Objects);
					if ( item != null )  return item;
				}
			}
			return null;
		}


		// S�lectionne un objet et d�s�lectionne tous les autres.
		public void Select(AbstractObject item, bool add)
		{
			this.Select(this.CurrentGroup, item, add);
		}

		protected void Select(System.Collections.ArrayList objects, AbstractObject item, bool add)
		{
			int total = objects.Count;
			for ( int index=0 ; index<total ; index++ )
			{
				AbstractObject obj = objects[index] as AbstractObject;
				if ( obj == item )
				{
					obj.Select();
				}
				else
				{
					if ( !add )  obj.Deselect();
				}
			}
		}


		// S�lectionne tous les objets dans le rectangle.
		// all = true  -> toutes les poign�es doivent �tre dans le rectangle
		// all = false -> une seule poign�e doit �tre dans le rectangle
		public void Select(Drawing.Rectangle rect, bool add, bool all)
		{
			this.Select(this.CurrentGroup, rect, add, all);
		}

		protected void Select(System.Collections.ArrayList objects, Drawing.Rectangle rect, bool add, bool all)
		{
			int total = objects.Count;
			for ( int index=0 ; index<total ; index++ )
			{
				AbstractObject obj = objects[index] as AbstractObject;
				if ( all )
				{
					if ( obj.Detect(rect, all) )
					{
						obj.Select();
					}
					else
					{
						if ( !add )  obj.Deselect();
					}
				}
				else
				{
					if ( obj.Detect(rect, all) )
					{
						obj.Select(rect);
					}
					else
					{
						obj.Deselect();
					}
				}
			}
		}


		// Indique que tous les objets s�lectionn�s le sont globalement.
		public void GlobalSelect(bool global)
		{
			this.GlobalSelect(this.CurrentGroup, global);
		}

		protected void GlobalSelect(System.Collections.ArrayList objects, bool global)
		{
			int total = objects.Count;
			for ( int index=0 ; index<total ; index++ )
			{
				AbstractObject obj = objects[index] as AbstractObject;
				obj.GlobalSelect(global);
			}
		}


		// D�s�lectionne tous les objets, dans toutes les pages et tous les calques.
		public void DeselectAll()
		{
			this.DeselectAll(this.objects);
		}

		protected void DeselectAll(System.Collections.ArrayList objects)
		{
			int total = objects.Count;
			for ( int index=0 ; index<total ; index++ )
			{
				AbstractObject obj = objects[index] as AbstractObject;
				obj.Deselect();

				if ( obj.Objects != null && obj.Objects.Count > 0 )
				{
					this.DeselectAll(obj.Objects);
				}
			}
		}


		// Adapte le mode d'�dition des propri�t�s.
		public void UpdateEditProperties()
		{
			this.UpdateEditProperties(this.CurrentGroup);
		}

		protected void UpdateEditProperties(System.Collections.ArrayList objects)
		{
			int sel = this.TotalSelected();
			int total = objects.Count;
			for ( int index=0 ; index<total ; index++ )
			{
				AbstractObject obj = objects[index] as AbstractObject;
				if ( obj.IsSelected() )
				{
					obj.EditProperties = ( sel == 1 );
				}
				else
				{
					obj.EditProperties = false;
				}
			}
		}

		
		// D�truit tous les objets s�lectionn�s.
		public void DeleteSelection()
		{
			this.DeleteSelection(this.CurrentGroup);
		}

		protected void DeleteSelection(System.Collections.ArrayList objects)
		{
			bool bDo = false;
			do
			{
				bDo = false;
				int total = objects.Count;
				for ( int index=0 ; index<total ; index++ )
				{
					AbstractObject obj = objects[index] as AbstractObject;
					if ( obj.IsSelected() )
					{
						objects.RemoveAt(index);
						bDo = true;
						break;
					}
				}
			}
			while ( bDo );
		}

		// Duplique tous les objets s�lectionn�s.
		public void DuplicateSelection(Drawing.Point move)
		{
			this.DuplicateSelection(this.CurrentGroup, this.CurrentGroup, move, false);
		}

		protected void DuplicateSelection(System.Collections.ArrayList objects,
										  System.Collections.ArrayList dst,
										  Drawing.Point move,
										  bool all)
		{
			int total = objects.Count;
			for ( int index=0 ; index<total ; index++ )
			{
				AbstractObject obj = objects[index] as AbstractObject;
				if ( !all && !obj.IsSelected() )  continue;

				AbstractObject newObject = null;
				if ( !obj.DuplicateObject(ref newObject) )  continue;
				newObject.MoveAll(move, all);
				dst.Add(newObject);

				obj.Deselect();

				if ( obj.Objects != null && obj.Objects.Count > 0 )
				{
					this.DuplicateSelection(obj.Objects, newObject.Objects, move, true);
				}
			}
		}

		// Copie tous les objets s�lectionn�s dans le bloc-notes.
		public void CopySelection()
		{
			this.clipboard.Clear();
			this.CopySelection(this.CurrentGroup, this.clipboard, false);
		}

		protected void CopySelection(System.Collections.ArrayList objects,
									 System.Collections.ArrayList dst,
									 bool all)
		{
			int total = objects.Count;
			for ( int index=0 ; index<total ; index++ )
			{
				AbstractObject obj = objects[index] as AbstractObject;
				if ( !all && !obj.IsSelected() )  continue;

				AbstractObject newObject = null;
				if ( !obj.DuplicateObject(ref newObject) )  continue;
				dst.Add(newObject);

				if ( obj.Objects != null && obj.Objects.Count > 0 )
				{
					this.CopySelection(obj.Objects, newObject.Objects, true);
				}
			}
		}

		// Colle tous les objets contenus dans le bloc-notes.
		public void PasteSelection()
		{
			this.PasteSelection(this.clipboard, this.CurrentGroup, false);
			this.StylesCollection.CollectionChanged();  // si PasteAdaptStyles a cr�� un nouveau style
		}

		protected void PasteSelection(System.Collections.ArrayList objects,
									  System.Collections.ArrayList dst,
									  bool all)
		{
			int total = objects.Count;
			for ( int index=0 ; index<total ; index++ )
			{
				AbstractObject obj = objects[index] as AbstractObject;
				if ( !all && !obj.IsSelected() )  continue;

				AbstractObject newObject = null;
				if ( !obj.DuplicateObject(ref newObject) )  continue;
				dst.Add(newObject);
				this.PasteAdaptStyles(newObject);  // adapte les styles

				if ( obj.Objects != null && obj.Objects.Count > 0 )
				{
					this.PasteSelection(obj.Objects, newObject.Objects, true);
				}
			}
		}

		// Adapte les styles de l'objet coll�, qui peut provenir d'un autre fichier,
		// donc d'une autre collection de styles. On se base sur le nom des styles
		// (StyleName) pour faire la correspondance.
		// Si on trouve un nom identique -> le style de l'objet coll� est modifi�
		// en fonction du style existant.
		// Si on ne trouve pas un nom identique -> on cr�e un nouveau style, en
		// modifiant bien entendu l'identificateur (StyleID) de l'objet coll�.
		protected void PasteAdaptStyles(AbstractObject obj)
		{
			int total = obj.TotalProperty;
			for ( int i=0 ; i<total ; i++ )
			{
				AbstractProperty property = obj.Property(i);
				if ( property.StyleID == 0 )  continue;  // n'utilise pas un style ?

				AbstractProperty style = this.StylesCollection.SearchProperty(property);
				if ( style == null )
				{
					int rank = this.StylesCollection.AddProperty(property);
					style = this.StylesCollection.GetProperty(rank);
				}
				style.CopyTo(property);
			}
		}

		// Indique si le bloc-notes est vide.
		public bool IsEmptyClipboard()
		{
			return (this.clipboard.Count == 0);
		}

		// Change l'ordre de tous les objets s�lectionn�s.
		public void OrderSelection(int dir)
		{
			this.OrderSelection(this.CurrentGroup, dir);
		}

		protected void OrderSelection(System.Collections.ArrayList objects, int dir)
		{
			System.Collections.ArrayList extract = new System.Collections.ArrayList();

			// Extrait tous les objets s�lectionn�s dans la liste extract.
			int total = objects.Count;
			for ( int index=0 ; index<total ; index++ )
			{
				AbstractObject obj = objects[index] as AbstractObject;
				if ( obj.IsSelected() )
				{
					extract.Add(obj);
				}
			}

			// Supprime les objets s�lectionn�s de la liste principale.
			this.DeleteSelection(objects);

			// Remet les objets extraits au d�but ou � la fin de la liste principale.
			int i = 0;
			if ( dir > 0 )  i = objects.Count;
			foreach ( AbstractObject obj in extract )
			{
				objects.Insert(i++, obj);
			}
		}

		// Associe tous les objets s�lectionn�s.
		public void GroupSelection()
		{
			this.GroupSelection(this.CurrentGroup);
		}

		protected void GroupSelection(System.Collections.ArrayList objects)
		{
			System.Collections.ArrayList extract = new System.Collections.ArrayList();

			// Extrait tous les objets s�lectionn�s dans la liste extract.
			Drawing.Rectangle bbox = Drawing.Rectangle.Empty;
			int total = objects.Count;
			for ( int index=0 ; index<total ; index++ )
			{
				AbstractObject obj = objects[index] as AbstractObject;
				if ( obj.IsSelected() )
				{
					extract.Add(obj);
					obj.Deselect();
					bbox.MergeWith(obj.BoundingBox);
					obj.Select();
				}
			}

			// Supprime les objets s�lectionn�s de la liste principale.
			this.DeleteSelection(objects);

			// Cr�e l'objet groupe.
			ObjectGroup group = new ObjectGroup();
			objects.Add(group);
			group.UpdateDim(bbox);
			group.Select();

			// Remet les objets extraits dans le groupe.
			foreach ( AbstractObject obj in extract )
			{
				obj.Deselect();
				group.Objects.Add(obj);
			}
		}

		// Dissocie tous les objets s�lectionn�s.
		public void UngroupSelection()
		{
			this.UngroupSelection(this.CurrentGroup);
		}

		protected void UngroupSelection(System.Collections.ArrayList objects)
		{
			int total = objects.Count;
			int index = 0;
			do
			{
				AbstractObject obj = objects[index] as AbstractObject;
				if ( obj.IsSelected() && obj is ObjectGroup )
				{
					int rank = index+1;
					foreach ( AbstractObject inside in obj.Objects )
					{
						inside.Select();
						objects.Insert(rank++, inside);
					}
					objects.RemoveAt(index);
					index += obj.Objects.Count-1;
					total = objects.Count;
				}
				index ++;
			}
			while ( index < total );
		}

		// Dissocie tous les niveaux des objets s�lectionn�s.
		public void UngroupAllSelection()
		{
			this.UngroupAllSelection(this.CurrentGroup);
		}

		protected void UngroupAllSelection(System.Collections.ArrayList objects)
		{
			bool bDo = false;
			do
			{
				bDo = false;
				int total = objects.Count;
				for ( int index=0 ; index<total ; index++ )
				{
					AbstractObject obj = objects[index] as AbstractObject;
					if ( obj.IsSelected() && obj is ObjectGroup )
					{
						int rank = index+1;
						foreach ( AbstractObject inside in obj.Objects )
						{
							inside.Select();
							objects.Insert(rank++, inside);
						}
						objects.RemoveAt(index);
						bDo = true;
						break;
					}
				}
			}
			while ( bDo );
		}


		// D�place tous les objets s�lectionn�s.
		public void MoveSelection(Drawing.Point move, ref Drawing.Rectangle bbox)
		{
			this.MoveSelection(this.CurrentGroup, move, ref bbox, false);
		}

		protected void MoveSelection(System.Collections.ArrayList objects, Drawing.Point move, ref Drawing.Rectangle bbox, bool all)
		{
			int total = objects.Count;
			for ( int index=0 ; index<total ; index++ )
			{
				AbstractObject obj = objects[index] as AbstractObject;
				if ( !all && !obj.IsSelected() )  continue;

				bbox.MergeWith(obj.BoundingBox);
				obj.MoveAll(move, all);
				bbox.MergeWith(obj.BoundingBox);

				if ( obj.Objects != null && obj.Objects.Count > 0 )
				{
					this.MoveSelection(obj.Objects, move, ref bbox, true);
				}
			}
		}


		// D�place globalement tous les objets s�lectionn�s.
		public void MoveSelection(GlobalModifierData initial, GlobalModifierData final, ref Drawing.Rectangle bbox)
		{
			this.MoveSelection(this.CurrentGroup, initial, final, ref bbox, false);
		}

		protected void MoveSelection(System.Collections.ArrayList objects, GlobalModifierData initial, GlobalModifierData final, ref Drawing.Rectangle bbox, bool all)
		{
			int total = objects.Count;
			for ( int index=0 ; index<total ; index++ )
			{
				AbstractObject obj = objects[index] as AbstractObject;
				if ( !all && !obj.IsSelected() )  continue;

				bbox.MergeWith(obj.BoundingBox);
				obj.MoveGlobal(initial, final, all);
				bbox.MergeWith(obj.BoundingBox);

				if ( obj.Objects != null && obj.Objects.Count > 0 )
				{
					this.MoveSelection(obj.Objects, initial, final, ref bbox, true);
				}
			}
		}


		// D�place une poign�e d'un objet.
		public void MoveHandleProcess(AbstractObject obj, int rank, Drawing.Point pos, IconContext iconContext, ref Drawing.Rectangle bbox)
		{
			bbox.MergeWith(obj.BoundingBox);
			obj.MoveHandleProcess(rank, pos, iconContext);
			bbox.MergeWith(obj.BoundingBox);

			AbstractProperty property = obj.MoveHandleProperty(rank);
			if ( property != null )
			{
				bool init = property.EditProperties;
				property.EditProperties = false;
				this.SetProperty(property, ref bbox, true);
				property.EditProperties = init;
			}
		}


		// Cache la s�lection.
		public void HideSelection()
		{
			this.HideObject(this.CurrentGroup, true, false, true);
		}

		// Cache ce qui n'est pas s�lectionn�.
		public void HideRest()
		{
			this.HideObject(this.objects, false, true, true);
		}

		// Annule tous les objets cach�s (montre tout).
		public void HideCancel()
		{
			this.HideObject(this.objects, false, false, false);
		}

		// Cache un objet et tous ses fils.
		protected void HideObject(AbstractObject obj, bool onlySelected, bool onlyDeselected, bool hide)
		{
			if ( !(obj is ObjectPage) && !(obj is ObjectLayer) )
			{
				if ( hide )
				{
					obj.Deselect();
				}
				else
				{
					obj.Select(obj.IsHide);
				}
				obj.IsHide = hide;
			}

			if ( obj.Objects != null && obj.Objects.Count > 0 )
			{
				this.HideObject(obj.Objects, onlySelected, onlyDeselected, hide);
			}
		}

		protected void HideObject(System.Collections.ArrayList objects, bool onlySelected, bool onlyDeselected, bool hide)
		{
			int total = objects.Count;
			for ( int index=0 ; index<total ; index++ )
			{
				AbstractObject obj = objects[index] as AbstractObject;

				if ( onlySelected )
				{
					if ( !obj.IsSelected() )  continue;
				}
				if ( onlyDeselected )
				{
					if ( obj.IsSelected() )  continue;
				}

				if ( !(obj is ObjectPage) && !(obj is ObjectLayer) )
				{
					if ( hide )
					{
						obj.Deselect();
					}
					else
					{
						obj.Select(obj.IsHide);
					}
					obj.IsHide = hide;
				}

				if ( obj.Objects != null && obj.Objects.Count > 0 )
				{
					this.HideObject(obj.Objects, onlySelected, onlyDeselected, hide);
				}
			}
		}

		// Retourne le nombre d'objets cach�s.
		public int RetTotalHide()
		{
			return this.RetTotalHide(this.objects);
		}

		protected int RetTotalHide(System.Collections.ArrayList objects)
		{
			int count = 0;
			int total = objects.Count;
			for ( int index=0 ; index<total ; index++ )
			{
				AbstractObject obj = objects[index] as AbstractObject;
				if ( obj.IsHide )  count ++;

				if ( obj.Objects != null && obj.Objects.Count > 0 )
				{
					count += this.RetTotalHide(obj.Objects);
				}
			}
			return count;
		}

		
		// Dessine la g�om�trie de tous les objets.
		public void DrawGeometry(Drawing.Graphics graphics,
								 IconContext iconContext,
								 object adorner,
								 Drawing.Rectangle clipRect,
								 bool showAllLayers)
		{
			if ( this.objects.Count == 0 )  return;
			ObjectPage page = this.objects[this.currentPage] as ObjectPage;
			this.DrawGeometry(page.Objects, graphics, iconContext, adorner, clipRect, showAllLayers, !showAllLayers);
		}

		protected void DrawGeometry(System.Collections.ArrayList objects,
									Drawing.Graphics graphics,
									IconContext iconContext,
									object adorner,
									Drawing.Rectangle clipRect,
									bool showAllLayers,
									bool dimmed)
		{
			System.Collections.ArrayList root = this.CurrentGroup;
			if ( objects == root )  dimmed = false;
			iconContext.IsDimmed = dimmed;

			int total = objects.Count;
			for ( int index=0 ; index<total ; index++ )
			{
				AbstractObject obj = objects[index] as AbstractObject;
				ObjectLayer layer = obj as ObjectLayer;

				if ( layer == null && !obj.BoundingBox.IntersectsWith(clipRect) )  continue;

				if ( obj.Objects != null && obj.Objects.Count > 0 )
				{
					if ( layer != null )
					{

						PropertyModColor modColor = layer.PropertyModColor(0);
						iconContext.modifyColor = new IconContext.ModifyColor(modColor.ModifyColor);

						if ( layer.Actif || showAllLayers )
						{
							this.DrawGeometry(obj.Objects, graphics, iconContext, adorner, clipRect, showAllLayers, dimmed);
						}
						else
						{
							if ( layer.Type != LayerType.Hide )
							{
								bool newDimmed = dimmed;
								if ( layer.Type == LayerType.Show )  newDimmed = false;
								this.DrawGeometry(obj.Objects, graphics, iconContext, adorner, clipRect, showAllLayers, newDimmed);
							}
						}
					}
					else
					{
						this.DrawGeometry(obj.Objects, graphics, iconContext, adorner, clipRect, showAllLayers, dimmed);
					}
				}

				if ( obj is ObjectGroup )
				{
					if ( objects != root )  continue;
				}

				iconContext.IsDimmed = dimmed;
				obj.DrawGeometry(graphics, iconContext);
			}
		}


		// Dessine les poign�es de tous les objets.
		public void DrawHandle(Drawing.Graphics graphics, IconContext iconContext)
		{
			this.DrawHandle(this.CurrentGroup, graphics, iconContext);
		}

		protected void DrawHandle(System.Collections.ArrayList objects, Drawing.Graphics graphics, IconContext iconContext)
		{
			int total = objects.Count;
			for ( int index=0 ; index<total ; index++ )
			{
				AbstractObject obj = objects[index] as AbstractObject;

				if ( obj.Objects != null && obj.Objects.Count > 0 )
				{
					this.DrawHandle(obj.Objects, graphics, iconContext);
				}

				obj.DrawHandle(graphics, iconContext);
			}
		}


		// Copie tous les objets.
		public void CopyTo(System.Collections.ArrayList dst)
		{
			this.CopyTo(this.objects, dst);
		}

		protected void CopyTo(System.Collections.ArrayList objects, System.Collections.ArrayList dst)
		{
			dst.Clear();
			foreach ( AbstractObject obj in objects )
			{
				AbstractObject newObject = null;
				if ( !obj.DuplicateObject(ref newObject) )  continue;
				dst.Add(newObject);

				if ( obj.Objects != null && obj.Objects.Count > 0 )
				{
					this.CopyTo(obj.Objects, newObject.Objects);
				}
			}
		}


		// Entre dans un groupe.
		public void InsideGroup(AbstractObject obj)
		{
			this.Select(null, false);
			this.roots.Add(obj);
		}

		// Sort d'un groupe.
		public void OutsideGroup()
		{
			if ( this.roots.Count <= 2 )  return;
			this.Select(null, false);
			this.UpdateEditProperties();
			AbstractObject obj = this.roots[this.roots.Count-1] as AbstractObject;
			this.roots.RemoveAt(this.roots.Count-1);
			this.Select(obj, false);
		}

		// Indique si on est � la racine.
		public bool IsInitialGroup()
		{
			return ( this.roots.Count <= 2 );
		}

		// Retourne la racine courante.
		[XmlIgnore]
		public System.Collections.ArrayList CurrentGroup
		{
			get
			{
				if ( this.roots.Count == 0 )
				{
					return this.objects;
				}
				else
				{
					AbstractObject group = this.roots[this.roots.Count-1] as AbstractObject;
					return group.Objects;
				}
			}
		}

		// Adapte les dimensions de tous les groupes inclus.
		public void GroupUpdate(ref Drawing.Rectangle bbox)
		{
			this.GroupUpdate(this.CurrentGroup, ref bbox, false);
		}

		protected void GroupUpdate(System.Collections.ArrayList objects, ref Drawing.Rectangle bbox, bool all)
		{
			int total = objects.Count;
			for ( int index=0 ; index<total ; index++ )
			{
				ObjectGroup group = objects[index] as ObjectGroup;
				if ( group == null || (!all && !group.IsSelected()) )  continue;

				if ( group.Objects != null && group.Objects.Count > 0 )
				{
					this.GroupUpdate(group.Objects, ref bbox, true);

					Drawing.Rectangle actualBbox = group.BoundingBox;
					Drawing.Rectangle newBbox = this.RetBbox(group.Objects);
					group.UpdateDim(newBbox);

					bbox.MergeWith(actualBbox);
					bbox.MergeWith(newBbox);
				}
			}
		}

		// Adapte les dimensions de tous les groupes parents.
		public void GroupUpdateParents(ref Drawing.Rectangle bbox)
		{
			if ( this.roots.Count <= 2 )  return;
			for ( int i=this.roots.Count-1 ; i>=2 ; i-- )
			{
				ObjectGroup group = this.roots[i] as ObjectGroup;
				if ( group == null || group.Objects == null || group.Objects.Count == 0 )  continue;

				Drawing.Rectangle actualBbox = group.BoundingBox;
				Drawing.Rectangle newBbox = this.RetBbox(group.Objects);
				group.UpdateDim(newBbox);

				bbox.MergeWith(actualBbox);
				bbox.MergeWith(newBbox);
			}
		}


		// Retourne le nombre total de pages.
		public int TotalPages()
		{
			return this.objects.Count;
		}

		// Retourne le nombre total de calques dans la page courante.
		public int TotalLayers()
		{
			System.Diagnostics.Debug.Assert(this.roots.Count >= 2);
			ObjectPage page = this.roots[0] as ObjectPage;
			System.Diagnostics.Debug.Assert(page != null);
			return page.Objects.Count;
		}

		// Gestion de la page en cours.
		public int CurrentPage
		{
			get
			{
				return this.currentPage;
			}

			set
			{
				if ( value != this.currentPage )
				{
					this.UsePageLayer(value, -1);  // -1 = dernier calque utilis�
				}
			}
		}

		// Gestion du calque en cours.
		public int CurrentLayer
		{
			get
			{
				return this.currentLayer;
			}

			set
			{
				if ( value != this.currentLayer )
				{
					this.UsePageLayer(this.currentPage, value);
				}
			}
		}

		// Utilise une page et un calque donn�.
		protected void UsePageLayer(int rankPage, int rankLayer)
		{
			ObjectPage	page;
			ObjectLayer	layer;

			if ( this.roots.Count >= 2 )
			{
				page = this.objects[this.currentPage] as ObjectPage;
				page.CurrentLayer = this.currentLayer;
			}

			this.roots.Clear();

			System.Diagnostics.Debug.Assert(rankPage < this.objects.Count);
			page = this.objects[rankPage] as ObjectPage;
			System.Diagnostics.Debug.Assert(page != null);
			this.roots.Add(page);

			if ( rankLayer == -1 )  // dernier calque utilis� dans cette page ?
			{
				rankLayer = page.CurrentLayer;
			}
			System.Diagnostics.Debug.Assert(rankLayer < page.Objects.Count);
			layer = page.Objects[rankLayer] as ObjectLayer;
			System.Diagnostics.Debug.Assert(layer != null);
			this.roots.Add(layer);

			this.currentPage  = rankPage;
			this.currentLayer = rankLayer;

			int total = page.Objects.Count;
			for ( int i=0 ; i<total ; i++ )
			{
				layer = page.Objects[i] as ObjectLayer;
				layer.Actif = (i == this.currentLayer);
			}
		}

		// Donne une page.
		public ObjectPage Page(int rank)
		{
			System.Diagnostics.Debug.Assert(this.roots.Count >= 2);
			System.Diagnostics.Debug.Assert(rank < this.objects.Count);
			return this.objects[rank] as ObjectPage;
		}

		// Donne un calque de la page courante.
		public ObjectLayer Layer(int rank)
		{
			System.Diagnostics.Debug.Assert(this.roots.Count >= 2);
			ObjectPage page = this.roots[0] as ObjectPage;
			System.Diagnostics.Debug.Assert(rank < page.Objects.Count);
			return page.Objects[rank] as ObjectLayer;
		}

		// Donne la transformation de couleur du calque courant.
		public PropertyModColor LayerModColor()
		{
			System.Diagnostics.Debug.Assert(this.roots.Count >= 2);
			ObjectLayer layer = this.roots[1] as ObjectLayer;
			System.Diagnostics.Debug.Assert(layer != null);
			return layer.PropertyModColor(0);
		}

		// Cr�e une nouvelle page (avec un calque) apr�s le rang donn�.
		public void CreatePage(int rank, bool duplicate)
		{
			System.Diagnostics.Debug.Assert(this.roots.Count >= 2);
			ObjectPage page = new ObjectPage();
			this.objects.Insert(rank+1, page);

			if ( duplicate )
			{
				this.DuplicateSelection(this.Page(rank).Objects, this.Page(rank+1).Objects, new Drawing.Point(0, 0), true);
				ObjectPage srcPage = this.Page(rank);
				if ( srcPage.Name == "" )
				{
					page.Name = "Copie";
				}
				else
				{
					page.Name = "Copie de " + srcPage.Name;
				}
			}
			else
			{
				ObjectLayer layer = new ObjectLayer();
				page.Objects.Add(layer);
			}

			this.UsePageLayer(rank+1, 0);
		}

		// Cr�e un nouveau calque dans la page courante apr�s le rang donn�.
		public void CreateLayer(int rank, bool duplicate)
		{
			System.Diagnostics.Debug.Assert(this.roots.Count >= 2);
			ObjectPage page = this.roots[0] as ObjectPage;
			ObjectLayer layer = new ObjectLayer();
			page.Objects.Insert(rank+1, layer);

			if ( duplicate )
			{
				this.DuplicateSelection(this.Layer(rank).Objects, this.Layer(rank+1).Objects, new Drawing.Point(0, 0), true);
				ObjectLayer srcLayer = this.Layer(rank);
				if ( srcLayer.Name == "" )
				{
					layer.Name = "Copie";
				}
				else
				{
					layer.Name = "Copie de " + srcLayer.Name;
				}
			}

			this.UsePageLayer(this.currentPage, rank+1);
		}

		// Supprime une page.
		public void DeletePage(int rank)
		{
			System.Diagnostics.Debug.Assert(this.roots.Count >= 2);
			System.Diagnostics.Debug.Assert(this.objects.Count > 1);
			System.Diagnostics.Debug.Assert(rank < this.objects.Count);
			this.objects.RemoveAt(rank);

			if ( this.currentPage > this.objects.Count-1 )  this.currentPage = this.objects.Count-1;
			this.roots[0] = this.objects[this.currentPage];

			ObjectPage page = this.roots[0] as ObjectPage;
			this.currentLayer = 0;
			this.roots[1] = page.Objects[this.currentLayer];
		}

		// Supprime un calque.
		public void DeleteLayer(int rank)
		{
			System.Diagnostics.Debug.Assert(this.roots.Count >= 2);
			ObjectPage page = this.roots[0] as ObjectPage;
			System.Diagnostics.Debug.Assert(page.Objects.Count > 1);
			System.Diagnostics.Debug.Assert(rank < page.Objects.Count);
			page.Objects.RemoveAt(rank);

			this.currentLayer --;
			if ( this.currentLayer < 0 )  this.currentLayer = 0;
			this.roots[1] = page.Objects[this.currentLayer];
		}

		// Permute deux pages.
		public void SwapPage(int rank1, int rank2)
		{
			System.Diagnostics.Debug.Assert(this.roots.Count >= 2);
			System.Diagnostics.Debug.Assert(rank1 < this.objects.Count);
			System.Diagnostics.Debug.Assert(rank2 < this.objects.Count);
			ObjectPage temp = this.objects[rank1] as ObjectPage;
			this.objects[rank1] = this.objects[rank2];
			this.objects[rank2] = temp;
		}

		// Permute deux calques.
		public void SwapLayer(int rank1, int rank2)
		{
			System.Diagnostics.Debug.Assert(this.roots.Count >= 2);
			ObjectPage page = this.roots[0] as ObjectPage;
			System.Diagnostics.Debug.Assert(rank1 < page.Objects.Count);
			System.Diagnostics.Debug.Assert(rank2 < page.Objects.Count);
			ObjectLayer temp = page.Objects[rank1] as ObjectLayer;
			page.Objects[rank1] = page.Objects[rank2];
			page.Objects[rank2] = temp;
		}


		// Sauve tous les objets.
		public bool Write(string filename)
		{
			try
			{
				XmlSerializer serializer = new XmlSerializer(typeof(IconObjects));
				TextWriter writer = new StreamWriter(filename);
				serializer.Serialize(writer, this);
				writer.Close();
			}
			catch ( System.Exception )
			{
				return false;
			}
			return true;
		}

		// Lit tous les objets.
		public bool Read(string filename)
		{
			try
			{
				using (FileStream fs = new FileStream(filename, FileMode.Open))
				{
					return this.Read(fs);
				}
			}
			catch ( System.Exception )
			{
				return false;
			}
		}

		public bool Read(System.IO.Stream stream)
		{
			try
			{
				XmlSerializer serializer = new XmlSerializer(typeof(IconObjects));
				//serializer.UnknownNode += new XmlNodeEventHandler(serializer_UnknownNode);
				//serializer.UnknownAttribute += new XmlAttributeEventHandler(serializer_UnknownAttribute);
				
				IconObjects obj;
				obj = (IconObjects)serializer.Deserialize(stream);

				this.size = obj.size;
				this.origin = obj.origin;

				obj.styles.CopyTo(this.styles);
				this.styles.InitNextID();
				this.styles.CollectionChanged();

				this.objects.Clear();
				if ( obj.objects.Count > 0 && obj.objects[0] is ObjectPage )
				{
					foreach ( AbstractObject src in obj.Objects )
					{
						this.objects.Add(src);
					}
				}
				else
				{
					ObjectPage  page  = new ObjectPage();
					ObjectLayer layer = new ObjectLayer();
					this.objects.Add(page);
					page.Objects.Add(layer);

					foreach ( AbstractObject src in obj.Objects )
					{
						layer.Objects.Add(src);
					}
				}
				this.ArrangeAfterRead(this.objects);

				this.UsePageLayer(0, 0);
			}
			catch ( System.Exception )
			{
				return false;
			}
			return true;
		}

		protected void ArrangeAfterRead(System.Collections.ArrayList objects)
		{
			int total = objects.Count;
			for ( int index=0 ; index<total ; index++ )
			{
				AbstractObject obj = objects[index] as AbstractObject;
				obj.ArrangeAfterRead();

				if ( obj.Objects != null && obj.Objects.Count > 0 )
				{
					this.ArrangeAfterRead(obj.Objects);
				}
			}
		}


		protected Drawing.Size					size = new Drawing.Size(20, 20);
		protected Drawing.Size					sizeArea = new Drawing.Size(20*3, 20*3);
		protected Drawing.Point					origin = new Drawing.Point(0, 0);
		protected StylesCollection				styles = new StylesCollection();
		protected System.Collections.ArrayList	objects = new System.Collections.ArrayList();
		protected System.Collections.ArrayList	roots = new System.Collections.ArrayList();
		protected System.Collections.ArrayList	clipboard = new System.Collections.ArrayList();
		protected int							currentPage = 0;
		protected int							currentLayer = 0;
	}
}
