using Epsitec.Common.Drawing;
using Epsitec.Common.Support;
using System.Runtime.Serialization;

namespace Epsitec.Common.Document.Settings
{
	/// <summary>
	/// La classe Settings contient tous les réglages.
	/// </summary>
	[System.Serializable()]
	public class Settings : ISerializable
	{
		public Settings(Document document)
		{
			this.document = document;

			this.settings = new System.Collections.ArrayList();
			this.CreateDefault();

			this.guides = new UndoableList(this.document, UndoableListType.Guides);

			this.printInfo = new PrintInfo(document);
		}

		// Crée tous les réglages par défaut, si nécessaire.
		// Il est possible d'en ajouter de nouveaux tout en restant compatible
		// avec les anciens fichiers sérialisés.
		protected void CreateDefault()
		{
			this.CreateDefaultPoint("PageSize");

			this.CreateDefaultBool("GridActive");
			this.CreateDefaultBool("GridShow");
			this.CreateDefaultPoint("GridStep");
			this.CreateDefaultPoint("GridSubdiv");
			this.CreateDefaultPoint("GridOffset");

			this.CreateDefaultBool("GuidesActive");
			this.CreateDefaultBool("GuidesShow");
			this.CreateDefaultBool("GuidesMouse");

			this.CreateDefaultBool("PreviewActive");
			this.CreateDefaultBool("RulersShow");
			this.CreateDefaultBool("LabelsShow");

			this.CreateDefaultPoint("DuplicateMove");
			this.CreateDefaultBool("RepeatDuplicateMove");
			this.CreateDefaultPoint("ArrowMove");
			this.CreateDefaultDouble("ArrowMoveMul");
			this.CreateDefaultDouble("ArrowMoveDiv");
			this.CreateDefaultDouble("ToLinePrecision");
			this.CreateDefaultInteger("DefaultUnit");

			this.CreateDefaultBool("PrintAutoLandscape");
			this.CreateDefaultBool("PrintAutoZoom");
			this.CreateDefaultBool("PrintDraft");
			this.CreateDefaultBool("PrintAA");
			this.CreateDefaultBool("PrintPerfectJoin");
			this.CreateDefaultBool("PrintDebugArea");
			this.CreateDefaultDouble("PrintDpi");
			this.CreateDefaultInteger("PrintCentring");
			this.CreateDefaultDouble("PrintMargins");
			this.CreateDefaultDouble("PrintDebord");
			this.CreateDefaultBool("PrintTarget");

			this.CreateDefaultDouble("ImageDpi");
			this.CreateDefaultInteger("ImageDepth");
			this.CreateDefaultInteger("ImageCompression");
			this.CreateDefaultDouble("ImageQuality");
			this.CreateDefaultDouble("ImageAA");
		}

		protected void CreateDefaultBool(string name)
		{
			Bool sBool = this.Get(name) as Bool;
			if ( sBool == null )
			{
				sBool = new Bool(this.document, name);
				this.settings.Add(sBool);
			}
		}

		protected void CreateDefaultInteger(string name)
		{
			Integer sInteger = this.Get(name) as Integer;
			if ( sInteger == null )
			{
				sInteger = new Integer(this.document, name);
				this.settings.Add(sInteger);
			}
		}

		protected void CreateDefaultDouble(string name)
		{
			Double sDouble = this.Get(name) as Double;
			if ( sDouble == null )
			{
				sDouble = new Double(this.document, name);
				this.settings.Add(sDouble);
			}
		}

		protected void CreateDefaultPoint(string name)
		{
			Point sPoint = this.Get(name) as Point;
			if ( sPoint == null )
			{
				sPoint = new Point(this.document, name);
				this.settings.Add(sPoint);
			}
		}


		// Donne les réglages de l'impression.
		public PrintInfo PrintInfo
		{
			get { return this.printInfo; }
		}

		// Remets tous les réglages par défaut.
		public void Reset()
		{
			this.GuidesReset();
		}

		// Nombre total de réglages.
		public int Count
		{
			get
			{
				return this.settings.Count;
			}
		}

		// Donne un réglage d'après son index.
		public Abstract Get(int index)
		{
			return this.settings[index] as Abstract;
		}

		// Donne un réglage d'après son nom.
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

		// Guide sélectionné.
		public int GuidesSelected
		{
			get
			{
				return this.guides.Selected;
			}

			set
			{
				this.guides.Selected = value;
			}
		}

		// Supprime tous les guides.
		public void GuidesReset()
		{
			this.guides.Clear();
			this.document.Notifier.NotifyGuidesChanged();
			this.document.Notifier.NotifyArea(this.document.Modifier.ActiveViewer);
			this.document.IsDirtySerialize = true;
		}

		// Donne un guide.
		public Guide GuidesGet(int index)
		{
			return this.guides[index] as Guide;
		}

		// Ajoute un nouveau guide.
		public int GuidesAdd(Guide guide)
		{
			int index = this.guides.Add(guide);
			this.document.Notifier.NotifyGuidesChanged();
			this.document.Notifier.NotifyArea(this.document.Modifier.ActiveViewer);
			this.document.IsDirtySerialize = true;
			return index;
		}

		// Ajoute un nouveau guide.
		public void GuidesInsert(int index, Guide guide)
		{
			this.guides.Insert(index, guide);
			this.document.Notifier.NotifyGuidesChanged();
			this.document.Notifier.NotifyArea(this.document.Modifier.ActiveViewer);
			this.document.IsDirtySerialize = true;
		}

		// Supprime un guide.
		public void GuidesRemoveAt(int index)
		{
			this.guides.RemoveAt(index);
			this.document.Notifier.NotifyGuidesChanged();
			this.document.Notifier.NotifyArea(this.document.Modifier.ActiveViewer);
			this.document.IsDirtySerialize = true;
		}


		#region Serialization
		// Sérialise les réglages.
		public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("Settings", this.settings);
			info.AddValue("GuidesList", this.guides);
			info.AddValue("PrintInfo", this.printInfo);
		}

		// Constructeur qui désérialise les réglages.
		protected Settings(SerializationInfo info, StreamingContext context)
		{
			this.document = Document.ReadDocument;
			this.settings = (System.Collections.ArrayList) info.GetValue("Settings", typeof(System.Collections.ArrayList));
			this.guides = (UndoableList) info.GetValue("GuidesList", typeof(UndoableList));
			this.printInfo = (PrintInfo) info.GetValue("PrintInfo", typeof(PrintInfo));
		}

		// Adapte l'objet après une désérialisation.
		public void ReadFinalize()
		{
			this.CreateDefault();
		}
		#endregion


		protected Document						document;
		protected System.Collections.ArrayList	settings;
		protected UndoableList					guides;
		protected PrintInfo						printInfo;
	}
}
