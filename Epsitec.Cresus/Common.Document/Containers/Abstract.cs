using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Document.Containers
{
	/// <summary>
	/// La classe Containers.Abstract est la classe de base pour tous les agrégats de panneaux.
	/// </summary>
	public abstract class Abstract : Common.Widgets.Widget
	{
		public Abstract(Document document)
		{
			this.document = document;
			this.IsVisibleChanged += this.HandleIsVisibleChanged;
		}
		
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				this.IsVisibleChanged -= this.HandleIsVisibleChanged;
			}
			
			base.Dispose (disposing);
		}

		public Document Document
		{
			get
			{
				return this.document;
			}
		}

		public virtual void Hilite(Objects.Abstract hiliteObject)
		{
			//	Met en évidence l'objet survolé par la souris.
		}


		private void HandleIsVisibleChanged(object sender, Types.DependencyPropertyChangedEventArgs e)
		{
			//	Appelé par Widget lorsque la visibilité change.
			
			bool visible = (bool) e.NewValue;
			
			if ( visible )
			{
				this.Update();  // màj si visible et sale
				this.document.Modifier.ActiveContainer = this;
			}
		}

		public void SetDirtyContent()
		{
			//	Indique qu'il faudra mettre à jour tout le contenu.
			this.isDirtyContent = true;
			this.Update();  // màj immédiate si l'agrégat est visible
		}

		public void SetDirtyGeometry()
		{
			//	Indique qu'il faudra mettre à jour selon les géométries.
			this.isDirtyGeometry = true;
			this.Update();  // màj immédiate si l'agrégat est visible
		}

		public void SetDirtyProperties(System.Collections.ArrayList propertyList)
		{
			//	Indique qu'il faudra mettre à jour les valeurs contenues.
			this.dirtyProperties.Clear();
			foreach ( Properties.Abstract property in propertyList )
			{
				this.dirtyProperties.Add(property);
			}
			this.Update();  // màj immédiate si l'agrégat est visible
		}

		public void SetDirtyAggregates(System.Collections.ArrayList aggregateList)
		{
			//	Indique qu'il faudra mettre à jour les valeurs contenues.
			this.dirtyAggregates.Clear();
			foreach ( Properties.Aggregate agg in aggregateList )
			{
				this.dirtyAggregates.Add(agg);
			}
			this.Update();  // màj immédiate si l'agrégat est visible
		}

		public void SetDirtyTextStyles(System.Collections.ArrayList textStyleList)
		{
			//	Indique qu'il faudra mettre à jour les valeurs contenues.
			this.dirtyTextStyles.Clear();
			foreach ( Text.TextStyle textStyle in textStyleList )
			{
				this.dirtyTextStyles.Add(textStyle);
			}
			this.Update();  // màj immédiate si le style est visible
		}

		public void SetDirtyObject(Objects.Abstract obj)
		{
			//	Indique qu'il faudra mettre à jour un objet.
			this.dirtyObject = obj;
			this.Update();  // màj immédiate si l'agrégat est visible
		}

		public void SetDirtySelNames()
		{
			//	Indique qu'il faudra mettre à jour la sélection par noms.
			this.isDirtySelNames = true;
			this.Update();  // màj immédiate si l'agrégat est visible
		}

		protected void Update()
		{
			//	Met à jour le contenu, si nécessaire.
			if ( !this.IsVisible )  return;

			if ( this.isDirtyContent )
			{
				this.DoUpdateContent();
				this.isDirtyContent = false;  // propre
				this.dirtyProperties.Clear();  // les propriétés sont forcément aussi à jour
				this.dirtyAggregates.Clear();  // les propriétés sont forcément aussi à jour
				this.dirtyTextStyles.Clear();  // les propriétés sont forcément aussi à jour
				this.dirtyObject = null;
				this.isDirtySelNames = false;
			}

			if (this.dirtyProperties.Count > 0)
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

			if (this.isDirtyGeometry)
			{
				this.DoUpdateGeometry();
				this.isDirtyGeometry = false;  // propre
			}

			if (this.isDirtySelNames)
			{
				this.DoUpdateSelNames();
				this.isDirtySelNames = false;
			}
		}

		protected virtual void DoUpdateContent()
		{
			//	Effectue la mise à jour du contenu.
		}

		protected virtual void DoUpdateGeometry()
		{
			//	Effectue la mise à jour selon la géométrie.
		}

		protected virtual void DoUpdateProperties(System.Collections.ArrayList propertyList)
		{
			//	Effectue la mise à jour des propriétés.
		}

		protected virtual void DoUpdateAggregates(System.Collections.ArrayList propertyList)
		{
			//	Effectue la mise à jour des agrégats.
		}

		protected virtual void DoUpdateTextStyles(System.Collections.ArrayList textStyleList)
		{
			//	Effectue la mise à jour des styles de texte.
		}

		protected virtual void DoUpdateObject(Objects.Abstract obj)
		{
			//	Effectue la mise à jour d'un objet.
		}

		protected virtual void DoUpdateSelNames()
		{
			//	Effectue la mise à jour de la sélection par noms.
		}


		protected static bool IsObjectUseByProperty(Properties.Abstract property, Objects.Abstract hiliteObject)
		{
			//	Vérifie si une propriété fait référence à un objet.
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
		protected bool							isDirtyGeometry;
		protected System.Collections.ArrayList	dirtyProperties = new System.Collections.ArrayList();
		protected System.Collections.ArrayList	dirtyAggregates = new System.Collections.ArrayList();
		protected System.Collections.ArrayList	dirtyTextStyles = new System.Collections.ArrayList();
		protected Objects.Abstract				dirtyObject = null;
		protected bool							isDirtySelNames;
	}
}
