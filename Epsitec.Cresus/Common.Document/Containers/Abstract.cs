using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Document.Containers
{
	/// <summary>
	/// La classe Containers.Abstract est la classe de base pour tous les agr�gats de panneaux.
	/// </summary>
	[SuppressBundleSupport]
	public abstract class Abstract : Widgets.Widget
	{
		public Abstract(Document document)
		{
			this.document = document;
		}
		
		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
		}


		// Met en �vidence l'objet survol� par la souris.
		public virtual void Hilite(Objects.Abstract hiliteObject)
		{
		}


		// Appel� par Widget lorsque la visibilit� change.
		protected override void OnVisibleChanged()
		{
			base.OnVisibleChanged();
			this.Update();  // m�j si visible et sale

			if ( this.IsVisible )
			{
				this.document.Modifier.ActiveContainer = this;
			}
		}

		// Indique qu'il faudra mettre � jour tout le contenu.
		public void SetDirtyContent()
		{
			this.isDirtyContent = true;
			this.Update();  // m�j imm�diate si l'agr�gat est visible
		}

		// Indique qu'il faudra mettre � jour les valeurs contenues.
		public void SetDirtyProperties(System.Collections.ArrayList propertyList)
		{
			this.dirtyProperties.Clear();
			foreach ( Properties.Abstract property in propertyList )
			{
				this.dirtyProperties.Add(property);
			}
			this.Update();  // m�j imm�diate si l'agr�gat est visible
		}

		// Indique qu'il faudra mettre � jour un objet.
		public void SetDirtyObject(Objects.Abstract obj)
		{
			this.dirtyObject = obj;
			this.Update();  // m�j imm�diate si l'agr�gat est visible
		}

		// Met � jour le contenu, si n�cessaire.
		protected void Update()
		{
			if ( !this.IsVisible )  return;

			if ( this.isDirtyContent )
			{
				this.DoUpdateContent();
				this.isDirtyContent = false;  // propre
				this.dirtyProperties.Clear();  // les propri�t�s sont forc�ment aussi � jour
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

		// Effectue la mise � jour du contenu.
		protected virtual void DoUpdateContent()
		{
		}

		// Effectue la mise � jour des propri�t�s.
		protected virtual void DoUpdateProperties(System.Collections.ArrayList propertyList)
		{
		}

		// Effectue la mise � jour d'un objet.
		protected virtual void DoUpdateObject(Objects.Abstract obj)
		{
		}


		// V�rifie si une propri�t� fait r�f�rence � un objet.
		protected static bool IsObjectUseByProperty(Properties.Abstract property, Objects.Abstract hiliteObject)
		{
			int total = property.Owners.Count;
			for ( int i=0 ; i<total ; i++ )
			{
				Objects.Abstract obj = property.Owners[i] as Objects.Abstract;
				if ( obj != null && obj == hiliteObject )  return true;
			}
			return false;
		}


		protected Document						document;
		protected bool							isDirtyContent;
		protected System.Collections.ArrayList	dirtyProperties = new System.Collections.ArrayList();
		protected Objects.Abstract				dirtyObject = null;
	}
}
