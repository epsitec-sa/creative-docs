using Epsitec.Common.Pictogram.Widgets;
using Epsitec.Common.Widgets;
using System.Xml.Serialization;
using System.IO;

namespace Epsitec.Common.Pictogram.Data
{
	/// <summary>
	/// La classe IconObjects contient tous les objets qui forment une icône.
	/// </summary>

	[XmlRootAttribute("EpsitecPictogram")]

	[
	XmlInclude(typeof(ObjectArray)),
	XmlInclude(typeof(ObjectBezier)),
	XmlInclude(typeof(ObjectCircle)),
	XmlInclude(typeof(ObjectEllipse)),
	XmlInclude(typeof(ObjectGroup)),
	XmlInclude(typeof(ObjectImage)),
	XmlInclude(typeof(ObjectLayer)),
	XmlInclude(typeof(ObjectLine)),
	XmlInclude(typeof(ObjectPage)),
	XmlInclude(typeof(ObjectPattern)),
	XmlInclude(typeof(ObjectPoly)),
	XmlInclude(typeof(ObjectRectangle)),
	XmlInclude(typeof(ObjectRegular)),
	XmlInclude(typeof(ObjectTextBox)),
	XmlInclude(typeof(ObjectTextLine)),

	XmlInclude(typeof(Handle)),

	XmlInclude(typeof(PropertyName)),
	XmlInclude(typeof(PropertyBool)),
	XmlInclude(typeof(PropertyColor)),
	XmlInclude(typeof(PropertyDouble)),
	XmlInclude(typeof(PropertyGradient)),
	XmlInclude(typeof(PropertyShadow)),
	XmlInclude(typeof(PropertyLine)),
	XmlInclude(typeof(PropertyList)),
	XmlInclude(typeof(PropertyCombo)),
	XmlInclude(typeof(PropertyFont)),
	XmlInclude(typeof(PropertyJustif)),
	XmlInclude(typeof(PropertyTextLine)),
	XmlInclude(typeof(PropertyString)),
	XmlInclude(typeof(PropertyArrow)),
	XmlInclude(typeof(PropertyCorner)),
	XmlInclude(typeof(PropertyRegular)),
	XmlInclude(typeof(PropertyImage)),
	XmlInclude(typeof(PropertyModColor)),
	]

	public class IconObjects
	{
		public IconObjects()
		{
		}

		public Drawer Drawer
		{
			set { this.drawer = value; }
		}

		// Taille préférentielle de l'icône en pixels.
		public Drawing.Size Size
		{
			get
			{
				if ( this.currentPattern == 0 )
				{
					return this.size;
				}
				else
				{
					return this.motifSize;
				}
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
				if ( this.currentPattern == 0 )
				{
					return this.sizeArea;
				}
				else
				{
					return this.motifSizeArea;
				}
			}
		}

		// Origine de la zone de travail en pixels.
		public Drawing.Point OriginArea
		{
			get
			{
				if ( this.currentPattern == 0 )
				{
					Drawing.Point origin = new Drawing.Point();
					origin.X = -(this.sizeArea.Width-this.size.Width)/2;
					origin.Y = -(this.sizeArea.Height-this.size.Height)/2;
					return origin;
				}
				else
				{
					Drawing.Point origin = new Drawing.Point();
					origin.X = -(this.motifSizeArea.Width-this.motifSize.Width)/2;
					origin.Y = -(this.motifSizeArea.Height-this.motifSize.Height)/2;
					return origin;
				}
			}
		}

		// Origine de l'icône en pixels (hot spot).
		public Drawing.Point Origin
		{
			get
			{
				if ( this.currentPattern == 0 )
				{
					return this.origin;
				}
				else
				{
					return this.motifOrigin;
				}
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
		[XmlArrayItem("Array",     Type=typeof(ObjectArray))]
		[XmlArrayItem("Bezier",    Type=typeof(ObjectBezier))]
		[XmlArrayItem("Circle",    Type=typeof(ObjectCircle))]
		[XmlArrayItem("Ellipse",   Type=typeof(ObjectEllipse))]
		[XmlArrayItem("Group",     Type=typeof(ObjectGroup))]
		[XmlArrayItem("Image",     Type=typeof(ObjectImage))]
		[XmlArrayItem("Layer",     Type=typeof(ObjectLayer))]
		[XmlArrayItem("Line",      Type=typeof(ObjectLine))]
		[XmlArrayItem("Page",      Type=typeof(ObjectPage))]
		[XmlArrayItem("Pattern",   Type=typeof(ObjectPattern))]
		[XmlArrayItem("Polyline",  Type=typeof(ObjectPoly))]
		[XmlArrayItem("Rectangle", Type=typeof(ObjectRectangle))]
		[XmlArrayItem("Polygon",   Type=typeof(ObjectRegular))]
		[XmlArrayItem("TextBox",   Type=typeof(ObjectTextBox))]
		[XmlArrayItem("TextLine",  Type=typeof(ObjectTextLine))]
		public UndoList Objects
		{
			get { return this.objects; }
			set { this.objects = value; }
		}

		public System.Collections.ArrayList Roots
		{
			get { return this.roots; }
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
			ObjectPattern pattern = new ObjectPattern();
			ObjectPage    page    = new ObjectPage();
			ObjectLayer   layer   = new ObjectLayer();
			this.objects.Add(pattern);
			pattern.Objects.Add(page);
			page.Objects.Add(layer);
			this.roots.Clear();
			this.UsePatternPageLayer(0, 0, 0);

			this.styles.ClearProperty();
		}

		public int Add(AbstractObject obj)
		{
			return this.CurrentGroup.Add(obj);
		}

		public int Add(AbstractObject obj, bool selectAfterCreate)
		{
			return this.CurrentGroup.Add(obj, selectAfterCreate);
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

		protected int TotalCount(UndoList objects)
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


		// Retourne le nombre d'objets sélectionnés.
		public int TotalSelected()
		{
			return this.TotalSelected(this.CurrentGroup);
		}

		protected int TotalSelected(UndoList objects)
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


		// Copie tous les objets sélectionnés et leurs fils dans UndoList.Operations.
		public void SelectionWillBeChanged()
		{
			this.SelectionWillBeChanged(this.CurrentGroup, false);
		}

		protected void SelectionWillBeChanged(UndoList objects, bool all)
		{
			int total = objects.Count;
			for ( int index=0 ; index<total ; index++ )
			{
				AbstractObject obj = objects[index] as AbstractObject;
				if ( !all && !obj.IsSelected() )  continue;

				objects.WillBeChanged(index);

				if ( obj.Objects != null && obj.Objects.Count > 0 )
				{
					this.SelectionWillBeChanged(obj.Objects, true);
				}
			}
		}


		// Ajoute les propriétés des objets sélectionnés dans la liste.
		// Un type de propriété donné n'est qu'une fois dans la liste.
		public void PropertiesList(System.Collections.ArrayList list)
		{
			this.PropertiesList(this.CurrentGroup, list, false);
		}

		protected void PropertiesList(UndoList objects,
									  System.Collections.ArrayList list,
									  bool all)
		{
			int total = objects.Count;
			for ( int index=0 ; index<total ; index++ )
			{
				AbstractObject obj = objects[index] as AbstractObject;
				if ( !all && !obj.IsSelected() )  continue;

				obj.PropertiesList(list, !all);

				if ( obj.Objects != null && obj.Objects.Count > 0 )
				{
					this.PropertiesList(obj.Objects, list, true);
				}
			}
		}


		// Mets une propriété piquée avec la pipette aux objets sélectionnés.
		public void SetPropertyPicker(AbstractProperty property, ref Drawing.Rectangle bbox)
		{
			this.SetProperty(this.CurrentGroup, property, ref bbox, false);
		}

		// Modifie une propriété des objets sélectionnés.
		public void SetProperty(AbstractProperty property, ref Drawing.Rectangle bbox, bool changeStylesCollection)
		{
			if ( property.StyleID == 0 )  // propriété indépendante ?
			{
				this.SetProperty(this.CurrentGroup, property, ref bbox, false);
			}
			else	// propriété liée à un style ?
			{
				this.SetProperty(this.objects, property, ref bbox, false);

				if ( changeStylesCollection )
				{
					this.styles.ChangeProperty(property);
				}
			}
		}

		protected void SetProperty(UndoList objects, AbstractProperty property, ref Drawing.Rectangle bbox, bool all)
		{
			int total = objects.Count;
			for ( int index=0 ; index<total ; index++ )
			{
				AbstractObject obj = objects[index] as AbstractObject;

				bool deepAll = all;
				if ( property.StyleID == 0 )  // propriété indépendante ?
				{
					if ( !all && !obj.IsSelected() )  continue;

					bbox.MergeWith(obj.BoundingBox);
					obj.SetProperty(property);
					bbox.MergeWith(obj.BoundingBox);
					deepAll = true;
				}
				else	// propriété liée à un style ?
				{
					if ( all || obj.IsSelected() )
					{
						bbox.MergeWith(obj.BoundingBox);
						obj.SetProperty(property);
						bbox.MergeWith(obj.BoundingBox);
						deepAll = true;
					}
					else
					{
						if ( obj.IsLinkProperty(property) )
						{
							bbox.MergeWith(obj.BoundingBox);
							obj.SetProperty(property);
							bbox.MergeWith(obj.BoundingBox);
						}
					}
				}

				if ( obj.Objects != null && obj.Objects.Count > 0 &&
					 property.Type != PropertyType.Name           )
				{
					this.SetProperty(obj.Objects, property, ref bbox, deepAll);

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


		// Retourne une propriété.
		public AbstractProperty GetProperty(PropertyType type, AbstractObject objectMemory)
		{
			return this.GetProperty(this.CurrentGroup, type, objectMemory, false);
		}

		protected AbstractProperty GetProperty(UndoList objects, PropertyType type, AbstractObject objectMemory, bool all)
		{
			int total = objects.Count;
			for ( int index=0 ; index<total ; index++ )
			{
				AbstractObject obj = objects[index] as AbstractObject;
				if ( !all && !obj.IsSelected() )  continue;

				AbstractProperty property = obj.GetProperty(type);
				if ( property != null )
				{
					objectMemory.SetProperty(property);  // mémorise l'état
					return property;
				}

				if ( obj.Objects != null && obj.Objects.Count > 0 && type != PropertyType.Name )
				{
					AbstractProperty p = this.GetProperty(obj.Objects, type, objectMemory, true);
					if ( p != null )  return p;
				}
			}
			return null;
		}


		// Libère les objets sélectionnés du style.
		public void StyleFree(PropertyType type)
		{
			this.StyleFree(this.CurrentGroup, type, false);
		}

		protected void StyleFree(UndoList objects, PropertyType type, bool all)
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

		// Libère tous les objets du style.
		public void StyleFreeAll(AbstractProperty property)
		{
			this.StyleFreeAll(this.CurrentGroup, property);
		}

		protected void StyleFreeAll(UndoList objects, AbstractProperty property)
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


		// Utilise un style donné.
		public void StyleUse(AbstractProperty property)
		{
			this.StyleUse(this.CurrentGroup, property, false);
		}

		protected void StyleUse(UndoList objects, AbstractProperty property, bool all)
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

		protected Drawing.Rectangle RetBbox(UndoList objects)
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


		// Retourne la bounding box d'un pattern.
		public Drawing.Rectangle RetPatternBbox(int rank)
		{
			ObjectPattern pattern = this.Pattern(rank);
			if ( pattern == null )  return Drawing.Rectangle.Empty;
			return this.RetPatternBbox(pattern.Objects);
		}

		protected Drawing.Rectangle RetPatternBbox(UndoList objects)
		{
			int total = objects.Count;
			Drawing.Rectangle bbox = Drawing.Rectangle.Empty;
			for ( int index=0 ; index<total ; index++ )
			{
				AbstractObject obj = objects[index] as AbstractObject;

				if ( !(obj is ObjectPage) && !(obj is ObjectLayer) )
				{
					bbox.MergeWith(obj.BoundingBoxGeom);
				}

				if ( obj.Objects != null && obj.Objects.Count > 0 )
				{
					bbox.MergeWith(this.RetPatternBbox(obj.Objects));
				}
			}
			return bbox;
		}


		// Retourne la bounding box des objets sélectionnés.
		public Drawing.Rectangle RetSelectedBbox()
		{
			return this.RetSelectedBbox(this.CurrentGroup);
		}

		protected Drawing.Rectangle RetSelectedBbox(UndoList objects)
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


		// Détecte l'objet pointé par la souris.
		public AbstractObject Detect(Drawing.Point mouse)
		{
			return this.Detect(this.CurrentGroup, mouse);
		}

		protected AbstractObject Detect(UndoList objects, Drawing.Point mouse)
		{
			int total = objects.Count;
			for ( int index=total-1 ; index>=0 ; index-- )
			{
				AbstractObject obj = objects[index] as AbstractObject;

				if ( obj.Detect(mouse) )  return obj;
			}
			return null;
		}


		// Détecte l'objet pointé par la souris, pour l'édition.
		public AbstractObject DetectEdit(Drawing.Point mouse)
		{
			return this.DetectEdit(this.CurrentGroup, mouse);
		}

		protected AbstractObject DetectEdit(UndoList objects, Drawing.Point mouse)
		{
			int total = objects.Count;
			for ( int index=total-1 ; index>=0 ; index-- )
			{
				AbstractObject obj = objects[index] as AbstractObject;

				if ( obj.DetectEdit(mouse) )  return obj;
			}
			return null;
		}


		// Détecte en porfondeur l'objet pointé par la souris.
		public AbstractObject DeepDetect(Drawing.Point mouse)
		{
			return this.DeepDetect(this.CurrentGroup, mouse);
		}

		protected AbstractObject DeepDetect(UndoList objects, Drawing.Point mouse)
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


		// Détecte la poignée pointée par la souris.
		public bool DetectHandle(Drawing.Point mouse, out AbstractObject obj, out int rank)
		{
			return this.DetectHandle(this.CurrentGroup, mouse, out obj, out rank);
		}

		// Détecte la poignée pointée par la souris.
		protected bool DetectHandle(UndoList objects, Drawing.Point mouse, out AbstractObject obj, out int rank)
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

		protected void Hilite(UndoList objects, AbstractObject item, ref Drawing.Rectangle bbox)
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

		protected void DeepHilite(UndoList objects, AbstractObject item, ref Drawing.Rectangle bbox)
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


		// Détecte la cellule pointée par la souris.
		public bool DetectCell(Drawing.Point mouse, out AbstractObject obj, out int rank)
		{
			obj = null;
			rank = -1;

			if ( this.TotalSelected() != 1 )  return false;

			obj = this.RetFirstSelected();
			if ( obj == null )  return false;

			rank = obj.DetectCell(mouse);
			return ( rank != -1 );
		}


		// Retourne le premier objet sélectionné.
		public AbstractObject RetFirstSelected()
		{
			return this.RetFirstSelected(this.CurrentGroup);
		}

		protected AbstractObject RetFirstSelected(UndoList objects)
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


		// Sélectionne un objet et désélectionne tous les autres.
		// Retourne le nombre d'objets sélectionnés.
		public int Select(AbstractObject item, bool edit, bool add)
		{
			return this.Select(this.CurrentGroup, item, edit, add);
		}

		protected int Select(UndoList objects, AbstractObject item, bool edit, bool add)
		{
			int nb = 0;
			int total = objects.Count;
			for ( int index=0 ; index<total ; index++ )
			{
				AbstractObject obj = objects[index] as AbstractObject;
				if ( obj == item )
				{
					if ( !obj.IsSelected() || obj.IsEdited() != edit )
					{
						this.CurrentGroup.WillBeChanged(obj);
						nb ++;
					}
					obj.Select(true, edit);
				}
				else
				{
					if ( !add )
					{
						if ( obj.IsSelected() )
						{
							nb ++;
						}
						obj.Deselect();
					}
				}
			}
			return nb;
		}


		// Sélectionne tous les objets dans le rectangle.
		// all = true  -> toutes les poignées doivent être dans le rectangle
		// all = false -> une seule poignée doit être dans le rectangle
		public int Select(Drawing.Rectangle rect, bool add, bool all)
		{
			return this.Select(this.CurrentGroup, rect, add, all);
		}

		protected int Select(UndoList objects, Drawing.Rectangle rect, bool add, bool all)
		{
			int nb = 0;
			int total = objects.Count;
			for ( int index=0 ; index<total ; index++ )
			{
				AbstractObject obj = objects[index] as AbstractObject;
				if ( all )
				{
					if ( obj.Detect(rect, all) )
					{
						this.CurrentGroup.WillBeChanged(obj);
						nb ++;
						obj.Select();
					}
					else
					{
						if ( !add )
						{
							if ( obj.IsSelected() )
							{
								nb ++;
							}
							obj.Deselect();
						}
					}
				}
				else
				{
					if ( obj.Detect(rect, all) )
					{
						this.CurrentGroup.WillBeChanged(obj);
						nb ++;
						obj.Select(rect);
					}
					else
					{
						if ( obj.IsSelected() )
						{
							nb ++;
						}
						obj.Deselect();
					}
				}
			}
			return nb;
		}


		// Indique que tous les objets sélectionnés le sont globalement.
		public void GlobalSelect(bool global)
		{
			this.GlobalSelect(this.CurrentGroup, global);
		}

		protected void GlobalSelect(UndoList objects, bool global)
		{
			int total = objects.Count;
			for ( int index=0 ; index<total ; index++ )
			{
				AbstractObject obj = objects[index] as AbstractObject;
				obj.GlobalSelect(global);
			}
		}


		// Désélectionne tous les objets, dans toutes les pages et tous les calques.
		// Retourne le nombre d'objets désélectionnés.
		public int DeselectAll()
		{
			return this.DeselectAll(this.objects);
		}

		protected int DeselectAll(UndoList objects)
		{
			int nb = 0;
			int total = objects.Count;
			for ( int index=0 ; index<total ; index++ )
			{
				AbstractObject obj = objects[index] as AbstractObject;

				if ( obj.IsSelected() )
				{
					obj.Deselect();
					nb ++;
				}

				if ( obj.Objects != null && obj.Objects.Count > 0 )
				{
					nb += this.DeselectAll(obj.Objects);
				}
			}
			return nb;
		}


		// Désélectionne tous les objets qui n'ont pas le "undo stamp",
		// dans toutes les pages et tous les calques.
		public void DeselectNoUndoStamp()
		{
			this.DeselectNoUndoStamp(this.objects);
		}

		protected void DeselectNoUndoStamp(UndoList objects)
		{
			int total = objects.Count;
			for ( int index=0 ; index<total ; index++ )
			{
				AbstractObject obj = objects[index] as AbstractObject;

				if ( obj.UndoStamp )
				{
					obj.UndoStamp = false;
				}
				else
				{
					obj.Deselect();
				}

				if ( obj.Objects != null && obj.Objects.Count > 0 )
				{
					this.DeselectNoUndoStamp(obj.Objects);
				}
			}
		}


		// Adapte le mode d'édition des propriétés.
		public void UpdateEditProperties(AbstractObject objectMemory)
		{
			this.UpdateEditProperties(this.CurrentGroup, objectMemory);
		}

		protected void UpdateEditProperties(UndoList objects, AbstractObject objectMemory)
		{
			int sel = this.TotalSelected();
			int total = objects.Count;
			for ( int index=0 ; index<total ; index++ )
			{
				AbstractObject obj = objects[index] as AbstractObject;
				if ( obj.IsSelected() )
				{
					obj.EditProperties = ( sel == 1 );
					obj.SetPropertyExtended(objectMemory);
				}
				else
				{
					obj.EditProperties = false;
				}
			}
		}

		
		// Détruit tous les objets sélectionnés.
		public void DeleteSelection()
		{
			this.DeleteSelection(this.CurrentGroup);
		}

		protected void DeleteSelection(UndoList objects)
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

		// Duplique tous les objets sélectionnés.
		public void DuplicateSelection(Drawing.Point move)
		{
			this.DuplicateSelection(this.CurrentGroup, this.CurrentGroup, move, false);
		}

		protected void DuplicateSelection(UndoList objects,
										  UndoList dst,
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
				newObject.DuplicateAdapt();
				newObject.MoveAll(move, all);
				dst.Add(newObject);

				obj.Deselect();

				if ( obj.Objects != null && obj.Objects.Count > 0 )
				{
					this.DuplicateSelection(obj.Objects, newObject.Objects, move, true);
				}
			}
		}

		// Copie tous les objets sélectionnés dans le bloc-notes.
		public void CopySelection()
		{
			this.clipboard.Clear();
			this.CopySelection(this.CurrentGroup, this.clipboard, false);
		}

		protected void CopySelection(UndoList objects,
									 UndoList dst,
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
			this.StylesCollection.CollectionChanged();  // si PasteAdaptStyles a créé un nouveau style
		}

		protected void PasteSelection(UndoList objects,
									  UndoList dst,
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
				newObject.PasteAdaptStyles(this.StylesCollection);  // adapte les styles
				newObject.PasteAdaptProperties(this.currentPattern == 0);

				if ( obj.Objects != null && obj.Objects.Count > 0 )
				{
					this.PasteSelection(obj.Objects, newObject.Objects, true);
				}
			}
		}

		// Indique si le bloc-notes est vide.
		public bool IsEmptyClipboard()
		{
			return (this.clipboard.Count == 0);
		}

		// Change l'ordre de tous les objets sélectionnés.
		public void OrderSelection(int dir)
		{
			this.OrderSelection(this.CurrentGroup, dir);
		}

		protected void OrderSelection(UndoList objects, int dir)
		{
			System.Collections.ArrayList extract = new System.Collections.ArrayList();

			// Extrait tous les objets sélectionnés dans la liste extract.
			int total = objects.Count;
			for ( int index=0 ; index<total ; index++ )
			{
				AbstractObject obj = objects[index] as AbstractObject;
				if ( obj.IsSelected() )
				{
					extract.Add(obj);
				}
			}

			// Supprime les objets sélectionnés de la liste principale.
			this.DeleteSelection(objects);

			// Remet les objets extraits au début ou à la fin de la liste principale.
			int i = 0;
			if ( dir > 0 )  i = objects.Count;
			foreach ( AbstractObject obj in extract )
			{
				objects.Insert(i++, obj);
			}
		}

		// Associe tous les objets sélectionnés.
		public void GroupSelection()
		{
			this.GroupSelection(this.CurrentGroup);
		}

		protected void GroupSelection(UndoList objects)
		{
			System.Collections.ArrayList extract = new System.Collections.ArrayList();

			// Extrait tous les objets sélectionnés dans la liste extract.
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

			// Supprime les objets sélectionnés de la liste principale.
			this.DeleteSelection(objects);

			// Crée l'objet groupe.
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

		// Dissocie tous les objets sélectionnés.
		public void UngroupSelection()
		{
			this.UngroupSelection(this.CurrentGroup);
		}

		protected void UngroupSelection(UndoList objects)
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

		// Dissocie tous les niveaux des objets sélectionnés.
		public void UngroupAllSelection()
		{
			this.UngroupAllSelection(this.CurrentGroup);
		}

		protected void UngroupAllSelection(UndoList objects)
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

		// Retourne le nom du premier groupe sélectionné rencontré.
		public string GroupName()
		{
			UndoList objects = this.CurrentGroup;
			int total = objects.Count;
			for ( int index=0 ; index<total ; index++ )
			{
				AbstractObject obj = objects[index] as AbstractObject;
				if ( !obj.IsSelected() )  continue;
				if ( !(obj is ObjectGroup) )  continue;
				PropertyName prop = obj.Properties[0] as PropertyName;
				if ( prop == null )  continue;
				return prop.String;
			}
			return "";
		}

		// Spécifie le nom du premier groupe sélectionné rencontré.
		public void GroupName(string name)
		{
			UndoList objects = this.CurrentGroup;
			int total = objects.Count;
			for ( int index=0 ; index<total ; index++ )
			{
				AbstractObject obj = objects[index] as AbstractObject;
				if ( !obj.IsSelected() )  continue;
				if ( !(obj is ObjectGroup) )  continue;
				PropertyName prop = obj.Properties[0] as PropertyName;
				if ( prop == null )  continue;
				prop.String = name;
				return;
			}
		}


		// Déplace tous les objets sélectionnés.
		public void MoveSelection(Drawing.Point move, ref Drawing.Rectangle bbox)
		{
			this.MoveSelection(this.CurrentGroup, move, ref bbox, false);
		}

		protected void MoveSelection(UndoList objects, Drawing.Point move, ref Drawing.Rectangle bbox, bool all)
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


		// Déplace globalement tous les objets sélectionnés.
		public void MoveSelection(GlobalModifierData initial, GlobalModifierData final, ref Drawing.Rectangle bbox)
		{
			this.MoveSelection(this.CurrentGroup, initial, final, ref bbox, false);
		}

		protected void MoveSelection(UndoList objects, GlobalModifierData initial, GlobalModifierData final, ref Drawing.Rectangle bbox, bool all)
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


		// Déplace une poignée d'un objet.
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


		// Cache la sélection.
		public void HideSelection()
		{
			this.HideObject(this.CurrentGroup, true, false, true);
		}

		// Cache ce qui n'est pas sélectionné.
		public void HideRest()
		{
			this.HideObject(this.objects, false, true, true);
		}

		// Annule tous les objets cachés (montre tout).
		public void HideCancel()
		{
			this.HideObject(this.objects, false, false, false);
		}

		// Cache un objet et tous ses fils.
		protected void HideObject(UndoList objects, bool onlySelected, bool onlyDeselected, bool hide)
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
					objects.WillBeChanged(index);

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

		// Retourne le nombre d'objets cachés.
		public int RetTotalHide()
		{
			return this.RetTotalHide(this.objects);
		}

		protected int RetTotalHide(UndoList objects)
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

		
		// Dessine la géométrie de tous les objets.
		public void DrawGeometry(Drawing.Graphics graphics,
								 IconContext iconContext,
								 IconObjects iconObjects,
								 object adorner,
								 Drawing.Rectangle clipRect,
								 bool showAllLayers)
		{
			if ( this.objects.Count == 0 )  return;
			ObjectPattern pattern = this.objects[this.currentPattern] as ObjectPattern;
			if ( pattern.Objects.Count == 0 )  return;
			ObjectPage page = pattern.Objects[this.currentPage] as ObjectPage;
			this.DrawGeometry(page.Objects, graphics, iconContext, iconObjects, adorner, clipRect, showAllLayers, !showAllLayers);
		}

		public void DrawGeometry(UndoList objects,
								 Drawing.Graphics graphics,
								 IconContext iconContext,
								 IconObjects iconObjects,
								 object adorner,
								 Drawing.Rectangle clipRect,
								 bool showAllLayers,
								 bool dimmed)
		{
			UndoList root = this.CurrentGroup;
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
							this.DrawGeometry(obj.Objects, graphics, iconContext, iconObjects, adorner, clipRect, showAllLayers, dimmed);
						}
						else
						{
							if ( layer.Type != LayerType.Hide )
							{
								bool newDimmed = dimmed;
								if ( layer.Type == LayerType.Show )  newDimmed = false;
								this.DrawGeometry(obj.Objects, graphics, iconContext, iconObjects, adorner, clipRect, showAllLayers, newDimmed);
							}
						}
					}
					else
					{
						this.DrawGeometry(obj.Objects, graphics, iconContext, iconObjects, adorner, clipRect, showAllLayers, dimmed);
					}
				}

				if ( obj is ObjectGroup )
				{
					if ( objects != root )  continue;
				}

				iconContext.IsDimmed = dimmed;
				obj.DrawGeometry(graphics, iconContext, iconObjects);
			}
		}


		// Dessine les poignées de tous les objets.
		public void DrawHandle(Drawing.Graphics graphics, IconContext iconContext)
		{
			this.DrawHandle(this.CurrentGroup, graphics, iconContext);
		}

		protected void DrawHandle(UndoList objects, Drawing.Graphics graphics, IconContext iconContext)
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
		public void CopyTo(UndoList dst)
		{
			this.CopyTo(this.objects, dst);
		}

		protected void CopyTo(UndoList objects, UndoList dst)
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
			this.Select(null, false, false);
			this.roots.Add(obj);
		}

		// Sort d'un groupe.
		public void OutsideGroup()
		{
			if ( this.roots.Count <= 3 )  return;
			this.Select(null, false, false);
			AbstractObject obj = this.roots[this.roots.Count-1] as AbstractObject;
			this.roots.RemoveAt(this.roots.Count-1);
			this.Select(obj, false, false);
		}

		// Indique si on est à la racine.
		public bool IsInitialGroup()
		{
			return ( this.roots.Count <= 3 );
		}

		// Retourne la racine courante.
		[XmlIgnore]
		public UndoList CurrentGroup
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

		protected void GroupUpdate(UndoList objects, ref Drawing.Rectangle bbox, bool all)
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
			if ( this.roots.Count <= 3 )  return;
			for ( int i=this.roots.Count-1 ; i>=3 ; i-- )
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


		// Adapte toutes les bbox en fonction des patterns.
		public void UpdatePattern()
		{
			AbstractObject doc = this.objects[0] as AbstractObject;
			this.UpdatePattern(doc.Objects);
		}

		protected void UpdatePattern(UndoList objects)
		{
			int total = objects.Count;
			for ( int index=0 ; index<total ; index++ )
			{
				AbstractObject obj = objects[index] as AbstractObject;

				if ( obj.Objects != null && obj.Objects.Count > 0 )
				{
					this.UpdatePattern(obj.Objects);
				}

				if ( !(obj is ObjectPage) && !(obj is ObjectLayer) )
				{
					obj.PatternUpdateBoundingBox(this);
				}
			}
		}

		
		// Adapte tous les styles après une suppression de pattern.
		public void StylesDeletePattern(int rank)
		{
			int total = this.styles.TotalProperty;
			for ( int i=0 ; i<total ; i++ )
			{
				PropertyLine line = this.styles.GetProperty(i) as PropertyLine;
				if ( line != null )
				{
					line.AdaptDeletePattern(rank);
				}
			}
		}

		// Adapte tous les objets après une suppression de pattern.
		public void UpdateDeletePattern(int rank)
		{
			AbstractObject doc = this.objects[0] as AbstractObject;
			this.UpdateDeletePattern(doc.Objects, rank);
		}

		protected void UpdateDeletePattern(UndoList objects, int rank)
		{
			int total = objects.Count;
			for ( int index=0 ; index<total ; index++ )
			{
				AbstractObject obj = objects[index] as AbstractObject;
				obj.DeletePattern(rank);

				if ( obj.Objects != null && obj.Objects.Count > 0 )
				{
					this.UpdateDeletePattern(obj.Objects, rank);
				}
			}
		}

		
		// Retourne le nombre total de patterns.
		public int TotalPatterns()
		{
			return this.objects.Count;
		}

		// Retourne le nombre total de pages.
		public int TotalPages()
		{
			System.Diagnostics.Debug.Assert(this.roots.Count >= 3);
			ObjectPattern pattern = this.roots[0] as ObjectPattern;
			System.Diagnostics.Debug.Assert(pattern != null);
			return pattern.Objects.Count;
		}

		// Retourne le nombre total de calques dans la page courante.
		public int TotalLayers()
		{
			System.Diagnostics.Debug.Assert(this.roots.Count >= 3);
			ObjectPage page = this.roots[1] as ObjectPage;
			System.Diagnostics.Debug.Assert(page != null);
			return page.Objects.Count;
		}

		// Gestion du pattern en cours.
		[XmlIgnore]
		public int CurrentPattern
		{
			get
			{
				return this.currentPattern;
			}

			set
			{
				if ( value != this.currentPattern )
				{
					this.UsePatternPageLayer(value, -1, -1);  // -1 = dernier calque utilisé

					if ( value == 0 )
					{
						this.UpdatePattern();  // màj les bbox
					}
				}
			}
		}

		// Gestion de la page en cours.
		[XmlIgnore]
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
					this.UsePatternPageLayer(this.currentPattern, value, -1);  // -1 = dernier calque utilisé
				}
			}
		}

		// Gestion du calque en cours.
		[XmlIgnore]
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
					this.UsePatternPageLayer(this.currentPattern, this.currentPage, value);
				}
			}
		}

		// Défini la page et le calque.
		public void SwapPatternPageLayer(ref int rankPattern, ref int rankPage, ref int rankLayer)
		{
			Misc.Swap(ref this.currentPattern, ref rankPattern);
			Misc.Swap(ref this.currentPage,    ref rankPage   );
			Misc.Swap(ref this.currentLayer,   ref rankLayer  );
		}

		// Utilise une page et un calque donné.
		protected void UsePatternPageLayer(int rankPattern, int rankPage, int rankLayer)
		{
			ObjectPattern pattern;
			ObjectPage	  page;
			ObjectLayer	  layer;

			if ( this.roots.Count >= 3 )
			{
				pattern = this.objects[this.currentPattern] as ObjectPattern;
				pattern.CurrentPage = this.currentPage;
				if ( this.drawer != null )
				{
					pattern.CurrentZoom    = this.drawer.Zoom;
					pattern.CurrentOriginX = this.drawer.OriginX;
					pattern.CurrentOriginY = this.drawer.OriginY;
				}

				page = pattern.Objects[this.currentPage] as ObjectPage;
				page.CurrentLayer = this.currentLayer;
			}

			this.roots.Clear();

			System.Diagnostics.Debug.Assert(rankPattern < this.objects.Count);
			pattern = this.objects[rankPattern] as ObjectPattern;
			System.Diagnostics.Debug.Assert(pattern != null);
			this.roots.Add(pattern);

			if ( rankPage == -1 )  // dernière page utilisée dans ce pattern ?
			{
				rankPage = pattern.CurrentPage;
				if ( this.drawer != null )
				{
					this.drawer.Zoom    = pattern.CurrentZoom;
					this.drawer.OriginX = pattern.CurrentOriginX;
					this.drawer.OriginY = pattern.CurrentOriginY;
				}
			}
			System.Diagnostics.Debug.Assert(rankPage < pattern.Objects.Count);
			page = pattern.Objects[rankPage] as ObjectPage;
			System.Diagnostics.Debug.Assert(page != null);
			this.roots.Add(page);

			if ( rankLayer == -1 )  // dernier calque utilisé dans cette page ?
			{
				rankLayer = page.CurrentLayer;
			}
			System.Diagnostics.Debug.Assert(rankLayer < page.Objects.Count);
			layer = page.Objects[rankLayer] as ObjectLayer;
			System.Diagnostics.Debug.Assert(layer != null);
			this.roots.Add(layer);

			this.currentPattern = rankPattern;
			this.currentPage    = rankPage;
			this.currentLayer   = rankLayer;

			int total = page.Objects.Count;
			for ( int i=0 ; i<total ; i++ )
			{
				layer = page.Objects[i] as ObjectLayer;
				layer.Actif = (i == this.currentLayer);
			}
		}

		// Donne un pattern.
		public ObjectPattern Pattern(int rank)
		{
			if ( rank < 0 )  rank = 0;  // traitillé ?
			System.Diagnostics.Debug.Assert(this.roots.Count >= 3);
			System.Diagnostics.Debug.Assert(rank < this.objects.Count);
			return this.objects[rank] as ObjectPattern;
		}

		// Donne une page.
		public ObjectPage Page(int rank)
		{
			System.Diagnostics.Debug.Assert(this.roots.Count >= 3);
			ObjectPattern pattern = this.roots[0] as ObjectPattern;
			System.Diagnostics.Debug.Assert(rank < pattern.Objects.Count);
			return pattern.Objects[rank] as ObjectPage;
		}

		// Donne un calque de la page courante.
		public ObjectLayer Layer(int rank)
		{
			System.Diagnostics.Debug.Assert(this.roots.Count >= 3);
			ObjectPage page = this.roots[1] as ObjectPage;
			System.Diagnostics.Debug.Assert(rank < page.Objects.Count);
			return page.Objects[rank] as ObjectLayer;
		}

		// Donne la transformation de couleur du calque courant.
		public PropertyModColor LayerModColor()
		{
			System.Diagnostics.Debug.Assert(this.roots.Count >= 3);
			ObjectLayer layer = this.roots[2] as ObjectLayer;
			System.Diagnostics.Debug.Assert(layer != null);
			return layer.PropertyModColor(0);
		}

		// Crée un nouveau pattern (avec une page et un calque) après le rang donné.
		public void CreatePattern(int rank, bool duplicate)
		{
			System.Diagnostics.Debug.Assert(this.roots.Count >= 3);
			ObjectPattern pattern = new ObjectPattern();

			int id = 0;
			for ( int i=0 ; i<this.objects.Count ; i++ )
			{
				ObjectPattern pat = this.objects[i] as ObjectPattern;
				if ( id < pat.Id )  id = pat.Id;
			}
			pattern.Id = id+1;

			this.objects.Insert(rank+1, pattern);

			if ( duplicate )
			{
				this.DuplicateSelection(this.Pattern(rank).Objects, this.Pattern(rank+1).Objects, new Drawing.Point(0, 0), true);
				ObjectPattern srcPattern = this.Pattern(rank);
				pattern.Name = Misc.CopyName(srcPattern.Name);
			}
			else
			{
				ObjectPage page = new ObjectPage();
				pattern.Objects.Add(page);

				ObjectLayer layer = new ObjectLayer();
				page.Objects.Add(layer);
			}

			this.UsePatternPageLayer(rank+1, 0, 0);
		}

		// Crée une nouvelle page (avec un calque) après le rang donné.
		public void CreatePage(int rank, bool duplicate)
		{
			System.Diagnostics.Debug.Assert(this.roots.Count >= 3);
			ObjectPattern pattern = this.roots[0] as ObjectPattern;
			ObjectPage page = new ObjectPage();
			pattern.Objects.Insert(rank+1, page);

			if ( duplicate )
			{
				this.DuplicateSelection(this.Page(rank).Objects, this.Page(rank+1).Objects, new Drawing.Point(0, 0), true);
				ObjectPage srcPage = this.Page(rank);
				page.Name = Misc.CopyName(srcPage.Name);
			}
			else
			{
				ObjectLayer layer = new ObjectLayer();
				page.Objects.Add(layer);
			}

			this.UsePatternPageLayer(this.currentPattern, rank+1, 0);
		}

		// Crée un nouveau calque dans la page courante après le rang donné.
		public void CreateLayer(int rank, bool duplicate)
		{
			System.Diagnostics.Debug.Assert(this.roots.Count >= 3);
			ObjectPage page = this.roots[1] as ObjectPage;
			ObjectLayer layer = new ObjectLayer();
			page.Objects.Insert(rank+1, layer);

			if ( duplicate )
			{
				this.DuplicateSelection(this.Layer(rank).Objects, this.Layer(rank+1).Objects, new Drawing.Point(0, 0), true);
				ObjectLayer srcLayer = this.Layer(rank);
				layer.Name = Misc.CopyName(srcLayer.Name);
			}

			this.UsePatternPageLayer(this.currentPattern, this.currentPage, rank+1);
		}

		// Supprime un pattern.
		public void DeletePattern(int rank)
		{
			System.Diagnostics.Debug.Assert(this.roots.Count >= 3);
			System.Diagnostics.Debug.Assert(this.objects.Count > 1);
			System.Diagnostics.Debug.Assert(rank < this.objects.Count);
			this.objects.RemoveAt(rank);

			if ( this.currentPattern > this.objects.Count-1 )  this.currentPattern = this.objects.Count-1;
			this.roots[0] = this.objects[this.currentPattern];

			ObjectPattern pattern = this.roots[0] as ObjectPattern;
			this.currentPage = pattern.CurrentPage;
			this.roots[1] = pattern.Objects[this.currentPage];

			ObjectPage page = this.roots[1] as ObjectPage;
			this.currentLayer = page.CurrentLayer;
			this.roots[2] = page.Objects[this.currentLayer];
		}

		// Supprime une page.
		public void DeletePage(int rank)
		{
			System.Diagnostics.Debug.Assert(this.roots.Count >= 3);
			ObjectPattern pattern = this.roots[0] as ObjectPattern;
			System.Diagnostics.Debug.Assert(pattern.Objects.Count > 1);
			System.Diagnostics.Debug.Assert(rank < pattern.Objects.Count);
			pattern.Objects.RemoveAt(rank);

			if ( this.currentPage > pattern.Objects.Count-1 )  this.currentPage = pattern.Objects.Count-1;
			this.roots[1] = pattern.Objects[this.currentPage];

			ObjectPage page = this.roots[1] as ObjectPage;
			this.currentLayer = page.CurrentLayer;
			this.roots[2] = page.Objects[this.currentLayer];
		}

		// Supprime un calque.
		public void DeleteLayer(int rank)
		{
			System.Diagnostics.Debug.Assert(this.roots.Count >= 3);
			ObjectPage page = this.roots[1] as ObjectPage;
			System.Diagnostics.Debug.Assert(page.Objects.Count > 1);
			System.Diagnostics.Debug.Assert(rank < page.Objects.Count);
			page.Objects.RemoveAt(rank);

			this.currentLayer --;
			if ( this.currentLayer < 0 )  this.currentLayer = 0;
			this.roots[2] = page.Objects[this.currentLayer];
		}

		// Permute deux patterns.
		public void SwapPattern(int rank1, int rank2)
		{
			System.Diagnostics.Debug.Assert(this.roots.Count >= 3);
			System.Diagnostics.Debug.Assert(rank1 < this.objects.Count);
			System.Diagnostics.Debug.Assert(rank2 < this.objects.Count);

			ObjectPattern temp = this.objects[rank1] as ObjectPattern;
			this.objects.RemoveAt(rank1);
			this.objects.Insert(rank2, temp);
		}

		// Permute deux pages.
		public void SwapPage(int rank1, int rank2)
		{
			System.Diagnostics.Debug.Assert(this.roots.Count >= 3);
			ObjectPattern pattern = this.roots[0] as ObjectPattern;
			System.Diagnostics.Debug.Assert(rank1 < pattern.Objects.Count);
			System.Diagnostics.Debug.Assert(rank2 < pattern.Objects.Count);

			ObjectPage temp = pattern.Objects[rank1] as ObjectPage;
			pattern.Objects.RemoveAt(rank1);
			pattern.Objects.Insert(rank2, temp);
		}

		// Permute deux calques.
		public void SwapLayer(int rank1, int rank2)
		{
			System.Diagnostics.Debug.Assert(this.roots.Count >= 3);
			ObjectPage page = this.roots[1] as ObjectPage;
			System.Diagnostics.Debug.Assert(rank1 < page.Objects.Count);
			System.Diagnostics.Debug.Assert(rank2 < page.Objects.Count);

			ObjectLayer temp = page.Objects[rank1] as ObjectLayer;
			page.Objects.RemoveAt(rank1);
			page.Objects.Insert(rank2, temp);
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
				if ( obj.objects.Count > 0 && obj.objects[0] is ObjectPattern )
				{
					foreach ( AbstractObject src in obj.Objects )
					{
						this.objects.Add(src);
					}
				}
				else if ( obj.objects.Count > 0 && obj.objects[0] is ObjectPage )
				{
					ObjectPattern pattern = new ObjectPattern();
					this.objects.Add(pattern);

					foreach ( AbstractObject src in obj.Objects )
					{
						pattern.Objects.Add(src);
					}
				}
				else
				{
					ObjectPattern pattern = new ObjectPattern();
					ObjectPage    page    = new ObjectPage();
					ObjectLayer   layer   = new ObjectLayer();
					this.objects.Add(pattern);
					pattern.Objects.Add(page);
					page.Objects.Add(layer);

					foreach ( AbstractObject src in obj.Objects )
					{
						layer.Objects.Add(src);
					}
				}
				this.ArrangeAfterRead(this.objects);

				this.UsePatternPageLayer(0, 0, 0);
				this.UpdatePattern();  // màj les bbox
			}
			catch ( System.Exception )
			{
				return false;
			}
			return true;
		}

		protected void ArrangeAfterRead(UndoList objects)
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


		protected Drawer						drawer;
		protected Drawing.Size					size = new Drawing.Size(20, 20);
		protected Drawing.Size					sizeArea = new Drawing.Size(20*3, 20*3);
		protected Drawing.Point					origin = new Drawing.Point(0, 0);
		protected Drawing.Size					motifSize = new Drawing.Size(10, 10);
		protected Drawing.Size					motifSizeArea = new Drawing.Size(10*3, 10*3);
		protected Drawing.Point					motifOrigin = new Drawing.Point(5, 5);
		protected StylesCollection				styles = new StylesCollection();
		protected UndoList						objects = new UndoList();
		protected System.Collections.ArrayList	roots = new System.Collections.ArrayList();
		protected UndoList						clipboard = new UndoList();
		protected int							currentPattern = 0;
		protected int							currentPage = 0;
		protected int							currentLayer = 0;
	}
}
