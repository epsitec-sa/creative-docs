using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Document.Containers
{
	/// <summary>
	/// La classe Containers.Abstract est la classe de base pour tous les agr�gats de panneaux.
	/// </summary>
	[SuppressBundleSupport]
	public abstract class Abstract : Common.Widgets.Widget
	{
		public Abstract(Document document)
		{
			this.document = document;
			this.CommandDispatcher = document.CommandDispatcher;
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
		protected override void OnIsVisibleChanged(Types.PropertyChangedEventArgs e)
		{
			base.OnIsVisibleChanged(e);
			
			bool visible = (bool) e.NewValue;
			
			if ( visible )
			{
				this.Update();  // m�j si visible et sale
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

		// Indique qu'il faudra mettre � jour les valeurs contenues.
		public void SetDirtyAggregates(System.Collections.ArrayList aggregateList)
		{
			this.dirtyAggregates.Clear();
			foreach ( Properties.Aggregate agg in aggregateList )
			{
				this.dirtyAggregates.Add(agg);
			}
			this.Update();  // m�j imm�diate si l'agr�gat est visible
		}

		// Indique qu'il faudra mettre � jour un objet.
		public void SetDirtyObject(Objects.Abstract obj)
		{
			this.dirtyObject = obj;
			this.Update();  // m�j imm�diate si l'agr�gat est visible
		}

		// Indique qu'il faudra mettre � jour la s�lection par noms.
		public void SetDirtySelNames()
		{
			this.isDirtySelNames = true;
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
				this.dirtyAggregates.Clear();  // les propri�t�s sont forc�ment aussi � jour
				this.dirtyObject = null;
				this.isDirtySelNames = false;
			}

			if ( this.dirtyProperties.Count > 0 )
			{
				this.DoUpdateProperties(this.dirtyProperties);
				this.dirtyProperties.Clear();  // propre
			}

			if ( this.dirtyAggregates.Count > 0 )
			{
				this.DoUpdateAggregates(this.dirtyAggregates);
				this.dirtyAggregates.Clear();  // propre
			}

			if ( this.dirtyObject != null )
			{
				this.DoUpdateObject(this.dirtyObject);
				this.dirtyObject = null;
			}

			if ( this.isDirtySelNames )
			{
				this.DoUpdateSelNames();
				this.isDirtySelNames = false;
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

		// Effectue la mise � jour des agr�gats.
		protected virtual void DoUpdateAggregates(System.Collections.ArrayList propertyList)
		{
		}

		// Effectue la mise � jour d'un objet.
		protected virtual void DoUpdateObject(Objects.Abstract obj)
		{
		}

		// Effectue la mise � jour de la s�lection par noms.
		protected virtual void DoUpdateSelNames()
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
		protected System.Collections.ArrayList	dirtyAggregates = new System.Collections.ArrayList();
		protected Objects.Abstract				dirtyObject = null;
		protected bool							isDirtySelNames;
	}
}
