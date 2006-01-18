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
		}
		
		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
		}


		public virtual void Hilite(Objects.Abstract hiliteObject)
		{
			//	Met en �vidence l'objet survol� par la souris.
		}


		protected override void OnIsVisibleChanged(Types.PropertyChangedEventArgs e)
		{
			//	Appel� par Widget lorsque la visibilit� change.
			base.OnIsVisibleChanged(e);
			
			bool visible = (bool) e.NewValue;
			
			if ( visible )
			{
				this.Update();  // m�j si visible et sale
				this.document.Modifier.ActiveContainer = this;
			}
		}

		public void SetDirtyContent()
		{
			//	Indique qu'il faudra mettre � jour tout le contenu.
			this.isDirtyContent = true;
			this.Update();  // m�j imm�diate si l'agr�gat est visible
		}

		public void SetDirtyProperties(System.Collections.ArrayList propertyList)
		{
			//	Indique qu'il faudra mettre � jour les valeurs contenues.
			this.dirtyProperties.Clear();
			foreach ( Properties.Abstract property in propertyList )
			{
				this.dirtyProperties.Add(property);
			}
			this.Update();  // m�j imm�diate si l'agr�gat est visible
		}

		public void SetDirtyAggregates(System.Collections.ArrayList aggregateList)
		{
			//	Indique qu'il faudra mettre � jour les valeurs contenues.
			this.dirtyAggregates.Clear();
			foreach ( Properties.Aggregate agg in aggregateList )
			{
				this.dirtyAggregates.Add(agg);
			}
			this.Update();  // m�j imm�diate si l'agr�gat est visible
		}

		public void SetDirtyTextStyles(System.Collections.ArrayList textStyleList)
		{
			//	Indique qu'il faudra mettre � jour les valeurs contenues.
			this.dirtyTextStyles.Clear();
			foreach ( Text.TextStyle textStyle in textStyleList )
			{
				this.dirtyTextStyles.Add(textStyle);
			}
			this.Update();  // m�j imm�diate si le style est visible
		}

		public void SetDirtyObject(Objects.Abstract obj)
		{
			//	Indique qu'il faudra mettre � jour un objet.
			this.dirtyObject = obj;
			this.Update();  // m�j imm�diate si l'agr�gat est visible
		}

		public void SetDirtySelNames()
		{
			//	Indique qu'il faudra mettre � jour la s�lection par noms.
			this.isDirtySelNames = true;
			this.Update();  // m�j imm�diate si l'agr�gat est visible
		}

		protected void Update()
		{
			//	Met � jour le contenu, si n�cessaire.
			if ( !this.IsVisible )  return;

			if ( this.isDirtyContent )
			{
				this.DoUpdateContent();
				this.isDirtyContent = false;  // propre
				this.dirtyProperties.Clear();  // les propri�t�s sont forc�ment aussi � jour
				this.dirtyAggregates.Clear();  // les propri�t�s sont forc�ment aussi � jour
				this.dirtyTextStyles.Clear();  // les propri�t�s sont forc�ment aussi � jour
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

			if ( this.dirtyTextStyles.Count > 0 )
			{
				this.DoUpdateTextStyles(this.dirtyTextStyles);
				this.dirtyTextStyles.Clear();  // propre
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

		protected virtual void DoUpdateContent()
		{
			//	Effectue la mise � jour du contenu.
		}

		protected virtual void DoUpdateProperties(System.Collections.ArrayList propertyList)
		{
			//	Effectue la mise � jour des propri�t�s.
		}

		protected virtual void DoUpdateAggregates(System.Collections.ArrayList propertyList)
		{
			//	Effectue la mise � jour des agr�gats.
		}

		protected virtual void DoUpdateTextStyles(System.Collections.ArrayList textStyleList)
		{
			//	Effectue la mise � jour des styles de texte.
		}

		protected virtual void DoUpdateObject(Objects.Abstract obj)
		{
			//	Effectue la mise � jour d'un objet.
		}

		protected virtual void DoUpdateSelNames()
		{
			//	Effectue la mise � jour de la s�lection par noms.
		}


		protected static bool IsObjectUseByProperty(Properties.Abstract property, Objects.Abstract hiliteObject)
		{
			//	V�rifie si une propri�t� fait r�f�rence � un objet.
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
		protected System.Collections.ArrayList	dirtyTextStyles = new System.Collections.ArrayList();
		protected Objects.Abstract				dirtyObject = null;
		protected bool							isDirtySelNames;
	}
}
