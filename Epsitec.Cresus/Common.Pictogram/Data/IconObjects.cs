using System.Xml.Serialization;
using System.IO;

namespace Epsitec.Common.Pictogram.Data
{
	/// <summary>
	/// La classe IconObjects contient tous les objets qui forment une icône.
	/// </summary>

	[XmlRootAttribute("EpsitecPictogram")]

	[
	XmlInclude(typeof(ObjectArrow)),
	XmlInclude(typeof(ObjectBezier)),
	XmlInclude(typeof(ObjectCircle)),
	XmlInclude(typeof(ObjectEllipse)),
	XmlInclude(typeof(ObjectGroup)),
	XmlInclude(typeof(ObjectLine)),
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
	XmlInclude(typeof(PropertyString)),
	]

	public class IconObjects
	{
		public IconObjects()
		{
		}

		// Taille préférentielle de l'icône en pixels.
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

		// Origine de l'icône en pixels (hot spot).
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

		// Liste des objets.
		[XmlArrayItem("Arrow",     Type=typeof(ObjectArrow))]
		[XmlArrayItem("Bezier",    Type=typeof(ObjectBezier))]
		[XmlArrayItem("Circle",    Type=typeof(ObjectCircle))]
		[XmlArrayItem("Ellipse",   Type=typeof(ObjectEllipse))]
		[XmlArrayItem("Group",     Type=typeof(ObjectGroup))]
		[XmlArrayItem("Line",      Type=typeof(ObjectLine))]
		[XmlArrayItem("Polyline",  Type=typeof(ObjectPoly))]
		[XmlArrayItem("Rectangle", Type=typeof(ObjectRectangle))]
		[XmlArrayItem("Polygon",   Type=typeof(ObjectRegular))]
		[XmlArrayItem("Text",      Type=typeof(ObjectText))]
		public System.Collections.ArrayList Objects
		{
			get
			{
				return this.objects;
			}

			set
			{
				this.objects = value;
			}
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

		public int InitialCount
		{
			get { return this.objects.Count; }
		}

		public void Clear()
		{
			this.objects.Clear();
			this.roots.Clear();
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
			this.CurrentGroup.RemoveAt(index);
		}


		// Retourne le nombre d'objets sélectionnés.
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


		// Ajoute les propriétés des objets sélectionnés dans la liste.
		// Un type de propriété donné n'est qu'une fois dans la liste.
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

		// Modifie juste l'état "étendu" d'une propriété.
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


		// Modifie une propriété.
		public void SetProperty(AbstractProperty property, ref Drawing.Rectangle bbox)
		{
			this.SetProperty(this.CurrentGroup, property, ref bbox, false);
		}

		protected void SetProperty(System.Collections.ArrayList objects, AbstractProperty property, ref Drawing.Rectangle bbox, bool all)
		{
			bbox = Drawing.Rectangle.Empty;
			int total = objects.Count;
			for ( int index=0 ; index<total ; index++ )
			{
				AbstractObject obj = objects[index] as AbstractObject;
				if ( !all && !obj.IsSelected() )  continue;

				bbox.MergeWith(obj.BoundingBox);
				obj.SetProperty(property);
				bbox.MergeWith(obj.BoundingBox);

				if ( obj.Objects != null && obj.Objects.Count > 0 )
				{
					this.SetProperty(obj.Objects, property, ref bbox, true);

					Drawing.Rectangle gbox = this.RetBbox(obj.Objects);
					obj.Handle(0).Position = gbox.BottomLeft;
					obj.Handle(1).Position = gbox.TopRight;
					obj.UpdateBoundingBox();
					bbox.MergeWith(gbox);
				}
			}
		}


		// Retourne une propriété.
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
					objectMemory.SetProperty(property);  // mémorise l'état
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
				bbox.MergeWith(obj.BoundingBox);
			}
			return bbox;
		}


		// Retourne la bounding box des objets sélectionnés.
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

				bbox.MergeWith(obj.BoundingBox);
			}
			return bbox;
		}


		// Détecte l'objet pointé par la souris.
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


		// Détecte en porfondeur l'objet pointé par la souris.
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


		// Détecte la poignée pointée par la souris.
		public bool DetectHandle(Drawing.Point mouse, out AbstractObject obj, out int rank)
		{
			return this.DetectHandle(this.CurrentGroup, mouse, out obj, out rank);
		}

		// Détecte la poignée pointée par la souris.
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
		public void Hilite(AbstractObject item)
		{
			this.Hilite(this.CurrentGroup, item);
		}

		protected void Hilite(System.Collections.ArrayList objects, AbstractObject item)
		{
			int total = objects.Count;
			for ( int index=0 ; index<total ; index++ )
			{
				AbstractObject obj = objects[index] as AbstractObject;
				obj.IsHilite = (obj == item);
			}
		}


		// Hilite un objet en profondeur.
		public void DeepHilite(AbstractObject item)
		{
			this.DeepHilite(this.CurrentGroup, item);
		}

		protected void DeepHilite(System.Collections.ArrayList objects, AbstractObject item)
		{
			int total = objects.Count;
			for ( int index=0 ; index<total ; index++ )
			{
				AbstractObject obj = objects[index] as AbstractObject;

				obj.IsHilite = (obj == item);

				if ( obj.Objects != null && obj.Objects.Count > 0 )
				{
					this.DeepHilite(obj.Objects, item);
				}
			}
		}


		// Retourne le premier objet sélectionné.
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


		// Sélectionne un objet et désélectionne tous les autres.
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


		// Sélectionne tous les objets dans le rectangle.
		public void Select(Drawing.Rectangle rect, bool add)
		{
			this.Select(this.CurrentGroup, rect, add);
		}

		protected void Select(System.Collections.ArrayList objects, Drawing.Rectangle rect, bool add)
		{
			int total = objects.Count;
			for ( int index=0 ; index<total ; index++ )
			{
				AbstractObject obj = objects[index] as AbstractObject;
				if ( obj.Detect(rect) )
				{
					obj.Select();
				}
				else
				{
					if ( !add )  obj.Deselect();
				}
			}
		}


		// Adapte le mode d'édition des propriétés.
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

		
		// Détruit tous les objets sélectionnés.
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

		// Duplique tous les objets sélectionnés.
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
				newObject.MoveAll(move);
				dst.Add(newObject);

				obj.Deselect();

				if ( obj.Objects != null && obj.Objects.Count > 0 )
				{
					this.DuplicateSelection(obj.Objects, newObject.Objects, move, true);
				}
			}
		}

		// Change l'ordre de tous les objets sélectionnés.
		public void OrderSelection(int dir)
		{
			this.OrderSelection(this.CurrentGroup, dir);
		}

		protected void OrderSelection(System.Collections.ArrayList objects, int dir)
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

		protected void GroupSelection(System.Collections.ArrayList objects)
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
					bbox.MergeWith(obj.BoundingBox);
				}
			}

			// Supprime les objets sélectionnés de la liste principale.
			this.DeleteSelection(objects);

			// Crée l'objet groupe.
			ObjectGroup group = new ObjectGroup();
			objects.Add(group);
			group.SetBoundingBox(bbox);
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

		// Dissocie tous les niveaux des objets sélectionnés.
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
						Drawing.Point origin = obj.Origin;
						int rank = index+1;
						foreach ( AbstractObject inside in obj.Objects )
						{
							inside.Select();
							inside.MoveAll(origin);
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

		// Déplace tous les objets sélectionnés.
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
				obj.MoveAll(move);
				bbox.MergeWith(obj.BoundingBox);

				if ( obj.Objects != null && obj.Objects.Count > 0 )
				{
					this.MoveSelection(obj.Objects, move, ref bbox, true);
				}
			}
		}


		// Dessine la géométrie de tous les objets.
		public void DrawGeometry(Drawing.Graphics graphics,
								 IconContext iconContext,
								 object adorner)
		{
			this.DrawGeometry(this.objects, graphics, iconContext, adorner, true);
		}

		protected void DrawGeometry(System.Collections.ArrayList objects,
									Drawing.Graphics graphics,
									IconContext iconContext,
									object adorner,
									bool dimmed)
		{
			System.Collections.ArrayList root = this.CurrentGroup;
			if ( objects == root )  dimmed = false;
			iconContext.IsDimmed = dimmed;

			int total = objects.Count;
			for ( int index=0 ; index<total ; index++ )
			{
				AbstractObject obj = objects[index] as AbstractObject;

				if ( obj.Objects != null && obj.Objects.Count > 0 )
				{
					this.DrawGeometry(obj.Objects, graphics, iconContext, adorner, dimmed);
				}

				if ( obj is ObjectGroup )
				{
					if ( objects != root )  continue;
				}

				iconContext.IsDimmed = dimmed;
				obj.DrawGeometry(graphics, iconContext);
			}
		}


		// Dessine les poignées de tous les objets.
		public void DrawHandle(Drawing.Graphics graphics, IconContext iconContext)
		{
			this.DrawHandle(this.objects, graphics, iconContext);
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
			if ( this.roots.Count == 0 )  return;
			this.Select(null, false);
			AbstractObject obj = this.roots[this.roots.Count-1] as AbstractObject;
			this.roots.RemoveAt(this.roots.Count-1);
			this.Select(obj, false);
		}

		// Indique si on est à la racine.
		public bool IsInitialGroup()
		{
			return ( this.roots.Count == 0 );
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

		// Adapte les dimensions du groupe en fonction du contenu.
		public void GroupUpdate(ref Drawing.Rectangle bbox)
		{
			if ( this.roots.Count == 0 )  return;
			for ( int i=this.roots.Count-1 ; i>=0 ; i-- )
			{
				AbstractObject group = this.roots[i] as AbstractObject;
				if ( group.Objects == null || group.Objects.Count == 0 )  continue;

				Drawing.Rectangle actualBbox = group.BoundingBox;
				Drawing.Rectangle newBbox = this.RetBbox(group.Objects);
				group.Handle(0).Position = newBbox.BottomLeft;
				group.Handle(1).Position = newBbox.TopRight;
				group.UpdateBoundingBox();

				bbox.MergeWith(actualBbox);
				bbox.MergeWith(newBbox);
			}
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

				this.objects.Clear();
				foreach ( AbstractObject src in obj.Objects )
				{
					this.objects.Add(src);
				}
			}
			catch ( System.Exception )
			{
				return false;
			}
			return true;
		}


		[XmlAttribute]
		protected Drawing.Size					size = new Drawing.Size(20, 20);
		protected Drawing.Size					sizeArea = new Drawing.Size(20*3, 20*3);
		protected Drawing.Point					origin = new Drawing.Point(0, 0);
		protected System.Collections.ArrayList	objects = new System.Collections.ArrayList();

		[XmlIgnore]
		protected System.Collections.ArrayList	roots = new System.Collections.ArrayList();
	}
}
