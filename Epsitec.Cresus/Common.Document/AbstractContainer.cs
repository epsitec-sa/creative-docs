using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Document
{
	/// <summary>
	/// La classe AbstractContainer est la classe de base pour tous les agrégats de panneaux.
	/// </summary>
	[SuppressBundleSupport]
	public abstract class AbstractContainer : Widgets.Widget
	{
		public AbstractContainer(Document document)
		{
			this.document = document;
		}
		
		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
		}


		// Met en évidence l'objet survolé par la souris.
		public virtual void Hilite(AbstractObject hiliteObject)
		{
		}


		// Appelé par Widget lorsque la visibilité change.
		protected override void OnVisibleChanged()
		{
			base.OnVisibleChanged();
			this.Update();  // màj si visible et sale

			if ( this.IsVisible )
			{
				this.document.Modifier.ActiveContainer = this;
			}
		}

		// Indique qu'il faudra mettre à jour tout le contenu.
		public void SetDirtyContent()
		{
			this.isDirtyContent = true;
			this.Update();  // màj immédiate si l'agrégat est visible
		}

		// Indique qu'il faudra mettre à jour les valeurs contenues.
		public void SetDirtyProperties(System.Collections.ArrayList propertyList)
		{
			this.dirtyProperties.Clear();
			foreach ( AbstractProperty property in propertyList )
			{
				this.dirtyProperties.Add(property);
			}
			this.Update();  // màj immédiate si l'agrégat est visible
		}

		// Indique qu'il faudra mettre à jour un objet.
		public void SetDirtyObject(AbstractObject obj)
		{
			this.dirtyObject = obj;
			this.Update();  // màj immédiate si l'agrégat est visible
		}

		// Met à jour le contenu, si nécessaire.
		protected void Update()
		{
			if ( !this.IsVisible )  return;

			if ( this.isDirtyContent )
			{
				this.DoUpdateContent();
				this.isDirtyContent = false;  // propre
				this.dirtyProperties.Clear();  // les propriétés sont forcément aussi à jour
				this.dirtyObject = null;
			}

			if ( this.dirtyProperties.Count > 0 )
			{
				this.DoUpdateProperties(this.dirtyProperties);
				this.dirtyProperties.Clear();  // propre
			}

			if ( this.dirtyObject != null )
			{
				this.DoUpdateObject(this.dirtyObject);
				this.dirtyObject = null;
			}
		}

		// Effectue la mise à jour du contenu.
		protected virtual void DoUpdateContent()
		{
		}

		// Effectue la mise à jour des propriétés.
		protected virtual void DoUpdateProperties(System.Collections.ArrayList propertyList)
		{
		}

		// Effectue la mise à jour d'un objet.
		protected virtual void DoUpdateObject(AbstractObject obj)
		{
		}


		// Vérifie si une propriété fait référence à un objet.
		protected static bool IsObjectUseByProperty(AbstractProperty property, AbstractObject hiliteObject)
		{
			int total = property.Owners.Count;
			for ( int i=0 ; i<total ; i++ )
			{
				AbstractObject obj = property.Owners[i] as AbstractObject;
				if ( obj != null && obj == hiliteObject )  return true;
			}
			return false;
		}


		protected Document						document;
		protected bool							isDirtyContent;
		protected System.Collections.ArrayList	dirtyProperties = new System.Collections.ArrayList();
		protected AbstractObject				dirtyObject = null;
	}
}
