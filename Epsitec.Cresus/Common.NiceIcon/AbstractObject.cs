using System.Xml.Serialization;

namespace Epsitec.Common.NiceIcon
{
	/// <summary>
	/// La classe AbstractObject est la classe de base des objets graphiques.
	/// </summary>
	public abstract class AbstractObject
	{
		public AbstractObject()
		{
		}

		public virtual void CreateProperties()
		{
		}


		public System.Collections.ArrayList Properties
		{
			get { return this.properties; }
			set { this.properties = value; }
		}

		public System.Collections.ArrayList Handles
		{
			get { return this.handles; }
			set { this.handles = value; }
		}


		// Nom de l'ic�ne.
		public virtual string IconName
		{
			get { return @""; }
		}


		// Nombre de poign�es.
		public int TotalHandle
		{
			get
			{
				return this.handles.Count;
			}
		}

		// Ajoute une poign�e.
		public void HandleAdd(Drawing.Point pos, HandleType type)
		{
			Handle handle = new Handle();
			handle.Position = pos;
			handle.Type = type;
			this.handles.Add(handle);
		}

		// Ins�re une poign�e.
		public void HandleInsert(int rank, Handle handle)
		{
			this.handles.Insert(rank, handle);
		}

		// Supprime une poign�e.
		public void HandleDelete(int rank)
		{
			this.handles.RemoveAt(rank);
		}

		// Donne une poign�e de l'objet.
		public Handle Handle(int rank)
		{
			System.Diagnostics.Debug.Assert(this.handles[rank] != null);
			return this.handles[rank] as Handle;
		}

		// D�tecte la poign�e point�e par la souris.
		public virtual int DetectHandle(Drawing.Point pos)
		{
			int total = this.handles.Count;
			for ( int i=0 ; i<total ; i++ )
			{
				if ( this.Handle(i).Detect(pos) )  return i;
			}
			return -1;
		}

		// D�place une poign�e.
		public virtual void MoveHandle(int rank, Drawing.Point pos)
		{
			this.Handle(rank).Position = pos;
		}

		// D�place tout l'objet.
		public virtual void MoveAll(Drawing.Point move)
		{
			foreach ( Handle handle in this.handles )
			{
				handle.Position += move;
			}
		}


		// Etat survol� de l'objet.
		[XmlIgnore]
		public bool IsHilite
		{
			get
			{
				return this.isHilite;
			}

			set
			{
				this.isHilite = value;
			}
		}


		// S�lectionne ou d�s�lectionne toutes les poign�es de l'objet.
		public void SelectObject(bool select)
		{
			if ( select )  this.SelectObject();
			else           this.DeselectObject();
		}

		// S�lectionne toutes les poign�es de l'objet.
		public void SelectObject()
		{
			foreach ( Handle handle in this.handles )
			{
				handle.IsSelected = true;
			}
		}

		// D�s�lectionne toutes les poign�es de l'objet.
		public void DeselectObject()
		{
			foreach ( Handle handle in this.handles )
			{
				handle.IsSelected = false;
			}
		}

		// Indique si l'objet est s�lectionn�.
		public bool IsSelected()
		{
			foreach ( Handle handle in this.handles )
			{
				if ( handle.IsSelected )  return true;
			}
			return false;
		}


		// Nombre de prori�t�s.
		public int TotalProperty
		{
			get
			{
				return this.properties.Count;
			}
		}

		// Ajoute une prori�t� � l'objet.
		public void AddProperty(AbstractProperty property)
		{
			this.properties.Add(property);
		}

		// Donne une propri�t� de l'objet.
		public AbstractProperty Property(int rank)
		{
			System.Diagnostics.Debug.Assert(this.properties[rank] != null);
			return this.properties[rank] as AbstractProperty;
		}

		// Donne une propri�t� de l'objet.
		public PropertyBool PropertyBool(int rank)
		{
			System.Diagnostics.Debug.Assert(this.properties[rank] != null);
			return this.properties[rank] as PropertyBool;
		}

		// Donne une propri�t� de l'objet.
		public PropertyDouble PropertyDouble(int rank)
		{
			System.Diagnostics.Debug.Assert(this.properties[rank] != null);
			return this.properties[rank] as PropertyDouble;
		}

		// Donne une propri�t� de l'objet.
		public PropertyColor PropertyColor(int rank)
		{
			System.Diagnostics.Debug.Assert(this.properties[rank] != null);
			return this.properties[rank] as PropertyColor;
		}

		// Donne une propri�t� de l'objet.
		public PropertyGradient PropertyGradient(int rank)
		{
			System.Diagnostics.Debug.Assert(this.properties[rank] != null);
			return this.properties[rank] as PropertyGradient;
		}

		// Donne une propri�t� de l'objet.
		public PropertyLine PropertyLine(int rank)
		{
			System.Diagnostics.Debug.Assert(this.properties[rank] != null);
			return this.properties[rank] as PropertyLine;
		}

		// Donne une propri�t� de l'objet.
		public PropertyString PropertyString(int rank)
		{
			System.Diagnostics.Debug.Assert(this.properties[rank] != null);
			return this.properties[rank] as PropertyString;
		}

		// Donne une propri�t� de l'objet.
		public PropertyList PropertyList(int rank)
		{
			System.Diagnostics.Debug.Assert(this.properties[rank] != null);
			return this.properties[rank] as PropertyList;
		}

		// Cherche une propri�t� d'apr�s son type.
		protected AbstractProperty SearchProperty(PropertyType type)
		{
			foreach ( AbstractProperty property in this.properties )
			{
				if ( property.Type == type )  return property;
			}
			return null;
		}

		// Retourne une propri�t�.
		public AbstractProperty GetProperty(PropertyType type)
		{
			AbstractProperty actual = this.SearchProperty(type);
			if ( actual == null )  return null;

			AbstractProperty copy = AbstractProperty.NewProperty(type);
			if ( copy == null )  return null;

			actual.CopyTo(copy);
			return copy;
		}

		// Modifie une propri�t�.
		public void SetProperty(AbstractProperty property)
		{
			AbstractProperty actual = this.SearchProperty(property.Type);
			if ( actual == null )  return;

			property.CopyTo(actual);
		}

		// Modifie juste l'�tat "�tendu" d'une propri�t�.
		public void SetPropertyExtended(AbstractProperty property)
		{
			AbstractProperty actual = this.SearchProperty(property.Type);
			if ( actual == null )  return;

			actual.ExtendedSize = property.ExtendedSize;
		}

		// Reprend toutes les propri�t�s d'un objet source.
		public void CloneProperties(AbstractObject src)
		{
			if ( src == null )  return;
			foreach ( AbstractProperty property in this.properties )
			{
				AbstractProperty p = src.SearchProperty(property.Type);
				if ( p == null )  continue;
				p.CopyTo(property);
			}
		}

		// Reprend toutes les informations des propri�t�s d'un objet source.
		public void CloneInfoProperties(AbstractObject src)
		{
			if ( src == null )  return;
			foreach ( AbstractProperty property in this.properties )
			{
				AbstractProperty p = src.SearchProperty(property.Type);
				if ( p == null )  continue;
				p.CopyInfoTo(property);
			}
		}


		// D�tecte si la souris est sur un objet.
		public virtual bool Detect(Drawing.Point pos)
		{
			return false;
		}

		// D�tecte si l'objet est dans un rectangle.
		public virtual bool Detect(Drawing.Rectangle rect)
		{
			foreach ( Handle handle in this.handles )
			{
				if ( !rect.Contains(handle.Position) )  return false;
			}
			return true;
		}

		
		// Donne le contenu du menu contextuel.
		public virtual void ContextMenu(System.Collections.ArrayList list, Drawing.Point pos, int handleRank)
		{
		}

		// Ex�cute une commande du menu contextuel.
		public virtual void ContextCommand(string cmd, Drawing.Point pos, int handleRank)
		{
		}


		// D�but de la cr�ation d'un objet.
		public virtual void CreateMouseDown(Drawing.Point pos, IconContext iconContext)
		{
			iconContext.ConstrainFixStarting(pos);
		}

		// D�placement pendant la cr�ation d'un objet.
		public virtual void CreateMouseMove(Drawing.Point pos, IconContext iconContext)
		{
		}

		// Fin de la cr�ation d'un objet.
		public virtual void CreateMouseUp(Drawing.Point pos, IconContext iconContext)
		{
			iconContext.ConstrainDelStarting();
		}

		// Indique si la cr�ation de l'objet est termin�e.
		public virtual bool CreateIsEnding(IconContext iconContext)
		{
			return true;
		}

		// Indique si l'objet peut exister. Retourne false si l'objet ne peut
		// pas exister et doit �tre d�truit.
		public virtual bool CreateIsExist(IconContext iconContext)
		{
			return true;
		}

		// Termine la cr�ation de l'objet. Retourne false si l'objet ne peut
		// pas exister et doit �tre d�truit.
		public virtual bool CreateEnding(IconContext iconContext)
		{
			return false;
		}


		// Effectue une copie de l'objet courant.
		public bool DuplicateObject(ref AbstractObject newObject)
		{
			newObject = null;
			if ( this is ObjectLine      )  newObject = new ObjectLine();
			if ( this is ObjectArrow     )  newObject = new ObjectArrow();
			if ( this is ObjectRectangle )  newObject = new ObjectRectangle();
			if ( this is ObjectCircle    )  newObject = new ObjectCircle();
			if ( this is ObjectEllipse   )  newObject = new ObjectEllipse();
			if ( this is ObjectRegular   )  newObject = new ObjectRegular();
			if ( this is ObjectPoly      )  newObject = new ObjectPoly();
			if ( this is ObjectBezier    )  newObject = new ObjectBezier();
			if ( this is ObjectText      )  newObject = new ObjectText();
			if ( this == null )  return false;

			newObject.CreateProperties();
			newObject.CloneObject(this);
			return true;
		}

		// Reprend toutes les caract�ristiques d'un objet.
		public virtual void CloneObject(AbstractObject src)
		{
			int total = src.TotalHandle;
			for ( int i=0 ; i<total ; i++ )
			{
				Handle srcHandle = src.Handle(i);
				Handle newHandle = new Handle();
				srcHandle.CopyTo(newHandle);
				this.handles.Add(newHandle);
			}

			this.CloneProperties(src);
		}


		// Dessine la g�om�trie de l'objet.
		public virtual void DrawGeometry(Drawing.Graphics graphics, IconContext iconContext)
		{
			if ( iconContext.IsEditable )
			{
				this.scaleX       = iconContext.ScaleX;
				this.scaleY       = iconContext.ScaleY;
				this.minimalSize  = iconContext.MinimalSize;
				this.minimalWidth = iconContext.MinimalWidth;
				this.closeMargin  = iconContext.CloseMargin;
			}
		}

		// Dessine les poign�es de l'objet.
		public virtual void DrawHandle(Drawing.Graphics graphics, IconContext iconContext)
		{
			foreach ( Handle handle in this.handles )
			{
				if ( handle.Type != HandleType.Hide )
				{
					handle.Draw(graphics, iconContext);
				}
			}
		}


		protected bool							isHilite;
		protected double						scaleX;
		protected double						scaleY;
		protected double						minimalSize;
		protected double						minimalWidth;
		protected double						closeMargin;

		[XmlAttribute]
		protected System.Collections.ArrayList	properties = new System.Collections.ArrayList();
		protected System.Collections.ArrayList	handles = new System.Collections.ArrayList();
	}
}
