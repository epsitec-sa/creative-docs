using Epsitec.Common.Drawing;
using Epsitec.Common.Support;
using System.Runtime.Serialization;

namespace Epsitec.Common.Document.Settings
{
	/// <summary>
	/// La classe Settings contient tous les r�glages.
	/// </summary>
	[System.Serializable()]
	public class Settings
	{
		public Settings(Document document)
		{
			this.document = document;

			this.settings = new System.Collections.ArrayList();
			this.CreateDefault();

			this.guides = new System.Collections.ArrayList();
		}

		// Cr�e tous les r�glages par d�faut.
		protected void CreateDefault()
		{
			Bool sBool;
			Point sPoint;
			
			sPoint = new Point(this.document);
			sPoint.Name = "PageSize";
			sPoint.TextX = "Largeur";
			sPoint.TextY = "Hauteur";
			if ( this.document.Type == DocumentType.Pictogram )
			{
				sPoint.MinValue = 5.0;
				sPoint.MaxValue = 100.0;
				sPoint.Step = 1.0;
				sPoint.Resolution = 1.0;
			}
			else
			{
				sPoint.MinValue = 10.0;
				sPoint.MaxValue = 1000.0;
				sPoint.Step = 1.0;
				sPoint.Resolution = 0.1;
			}
			this.settings.Add(sPoint);
			
			sBool = new Bool(this.document);
			sBool.Name = "GridActive";
			sBool.Text = "Grille active";
			this.settings.Add(sBool);
			
			sBool = new Bool(this.document);
			sBool.Name = "GridShow";
			sBool.Text = "Grille visible";
			this.settings.Add(sBool);
			
			sPoint = new Point(this.document);
			sPoint.Name = "GridStep";
			sPoint.TextX = "Pas horizontal";
			sPoint.TextY = "Pas vertical";
			if ( this.document.Type == DocumentType.Pictogram )
			{
				sPoint.MinValue = 0.5;
				sPoint.MaxValue = 10.0;
				sPoint.Step = 0.5;
				sPoint.Resolution = 0.5;
			}
			else
			{
				sPoint.MinValue = 0.1;
				sPoint.MaxValue = 100.0;
				sPoint.Step = 1.0;
				sPoint.Resolution = 0.1;
			}
			this.settings.Add(sPoint);
			
			sPoint = new Point(this.document);
			sPoint.Name = "GridOffset";
			sPoint.TextX = "D�calage horizontal";
			sPoint.TextY = "D�calage vertical";
			if ( this.document.Type == DocumentType.Pictogram )
			{
				sPoint.MinValue = -10.0;
				sPoint.MaxValue = 10.0;
				sPoint.Step = 0.5;
				sPoint.Resolution = 0.1;
			}
			else
			{
				sPoint.MinValue = -100.0;
				sPoint.MaxValue = 100.0;
				sPoint.Step = 0.5;
				sPoint.Resolution = 0.1;
			}
			this.settings.Add(sPoint);
			
			sBool = new Bool(this.document);
			sBool.Name = "GuidesActive";
			sBool.Text = "Rep�res magn�tiques";
			this.settings.Add(sBool);
			
			sBool = new Bool(this.document);
			sBool.Name = "GuidesShow";
			sBool.Text = "Rep�res visibles";
			this.settings.Add(sBool);
			
			sPoint = new Point(this.document);
			sPoint.Name = "DuplicateMove";
			sPoint.TextX = "D�placement � droite";
			sPoint.TextY = "D�placement en haut";
			if ( this.document.Type == DocumentType.Pictogram )
			{
				sPoint.MinValue = -100.0;
				sPoint.MaxValue = 100.0;
				sPoint.Step = 1.0;
				sPoint.Resolution = 0.5;
			}
			else
			{
				sPoint.MinValue = -1000.0;
				sPoint.MaxValue = 1000.0;
				sPoint.Step = 1.0;
				sPoint.Resolution = 0.1;
			}
			this.settings.Add(sPoint);
		}

		// Nombre total de r�glages.
		public int Count
		{
			get
			{
				return this.settings.Count;
			}
		}

		// Donne un r�glage d'apr�s son index.
		public Abstract Get(int index)
		{
			return this.settings[index] as Abstract;
		}

		// Donne un r�glage d'apr�s son nom.
		public Abstract Get(string name)
		{
			foreach ( Abstract settings in this.settings )
			{
				if ( settings.Name == name )  return settings;
			}
			return null;
		}


		// Nombre total de guides.
		public int GuidesCount
		{
			get
			{
				return this.guides.Count;
			}
		}

		// Ajoute un nouveau guide.
		public int GuidesAdd(Guide guide)
		{
			int index = this.guides.Add(guide);
			this.document.Notifier.NotifyGuidesChanged();
			this.document.Notifier.NotifyArea(this.document.Modifier.ActiveViewer);
			return index;
		}

		// Ajoute un nouveau guide.
		public void GuidesInsert(int index, Guide guide)
		{
			this.guides.Insert(index, guide);
			this.document.Notifier.NotifyGuidesChanged();
			this.document.Notifier.NotifyArea(this.document.Modifier.ActiveViewer);
		}

		// Supprime un guide.
		public void GuidesRemoveAt(int index)
		{
			this.guides.RemoveAt(index);
			this.document.Notifier.NotifyGuidesChanged();
			this.document.Notifier.NotifyArea(this.document.Modifier.ActiveViewer);
		}

		// Donne un guide.
		public Guide GuidesGet(int index)
		{
			return this.guides[index] as Guide;
		}


		#region Serialization
		// S�rialise les r�glages.
		public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("Settings", this.settings);
			info.AddValue("Guides", this.guides);
		}

		// Constructeur qui d�s�rialise les r�glages.
		protected Settings(SerializationInfo info, StreamingContext context)
		{
			this.document = Document.ReadDocument;
			this.settings = (System.Collections.ArrayList) info.GetValue("Settings", typeof(System.Collections.ArrayList));
			this.guides = (System.Collections.ArrayList) info.GetValue("Guides", typeof(System.Collections.ArrayList));
		}
		#endregion


		protected Document						document;
		protected System.Collections.ArrayList	settings;
		protected System.Collections.ArrayList	guides;
	}
}
