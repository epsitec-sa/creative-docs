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

			this.globalGuides = true;
			this.guides = new UndoableList(this.document, UndoableListType.Guides);

			this.quickFonts = new System.Collections.ArrayList();
			Settings.DefaultQuickFonts(this.quickFonts);

			this.printInfo = new PrintInfo(document);
			this.exportPDFInfo = new ExportPDFInfo(document);
		}

		// Crée tous les réglages par défaut, si nécessaire.
		// Il est possible d'en ajouter de nouveaux tout en restant compatible
		// avec les anciens fichiers sérialisés.
		protected void CreateDefault()
		{
			this.owners = new System.Collections.Hashtable();

			this.CreateDefaultPoint("Settings", "PageSize");
			this.CreateDefaultDouble("Settings", "OutsideArea");

			this.CreateDefaultBool("Settings", "GridActive");
			this.CreateDefaultBool("Settings", "GridShow");
			this.CreateDefaultPoint("Settings", "GridStep");
			this.CreateDefaultPoint("Settings", "GridSubdiv");
			this.CreateDefaultPoint("Settings", "GridOffset");

			this.CreateDefaultBool("Settings", "TextGridShow");
			this.CreateDefaultDouble("Settings", "TextGridStep");
			this.CreateDefaultDouble("Settings", "TextGridSubdiv");
			this.CreateDefaultDouble("Settings", "TextGridOffset");
			this.CreateDefaultBool("Settings", "TextShowControlCharacters");
			this.CreateDefaultBool("Settings", "TextFontFilter");
			this.CreateDefaultBool("Settings", "TextFontSampleAbc");
			this.CreateDefaultDouble("Settings", "TextFontSampleHeight");

			this.CreateDefaultBool("Settings", "GuidesActive");
			this.CreateDefaultBool("Settings", "GuidesShow");
			this.CreateDefaultBool("Settings", "GuidesMouse");

			this.CreateDefaultBool("", "PreviewActive");
			this.CreateDefaultBool("", "RulersShow");
			this.CreateDefaultBool("", "LabelsShow");
			this.CreateDefaultBool("", "MagnetActive");

			this.CreateDefaultPoint("Settings", "DuplicateMove");
			this.CreateDefaultBool("Settings", "RepeatDuplicateMove");
			this.CreateDefaultPoint("Settings", "ArrowMove");
			this.CreateDefaultDouble("Settings", "ArrowMoveMul");
			this.CreateDefaultDouble("Settings", "ArrowMoveDiv");
			this.CreateDefaultDouble("Settings", "ToLinePrecision");
			this.CreateDefaultInteger("Settings", "DefaultUnit");
			this.CreateDefaultDouble("Settings", "DimensionScale");
			this.CreateDefaultDouble("Settings", "DimensionDecimal");

			this.CreateDefaultString("Print", "PrintName");
			this.CreateDefaultRange("Print", "PrintRange");
			this.CreateDefaultInteger("Print", "PrintArea");
			this.CreateDefaultDouble("Print", "PrintCopies");
			this.CreateDefaultBool("Print", "PrintCollate");
			this.CreateDefaultBool("Print", "PrintReverse");
			this.CreateDefaultBool("Print", "PrintToFile");
			this.CreateDefaultString("Print", "PrintFilename");
			this.CreateDefaultBool("Print", "PrintAutoLandscape");
			this.CreateDefaultBool("Print", "PrintAutoZoom");
			this.CreateDefaultBool("Print", "PrintDraft");
			this.CreateDefaultBool("Print", "PrintAA");
			this.CreateDefaultBool("Print", "PrintPerfectJoin");
			this.CreateDefaultBool("Print", "PrintDebugArea");
			this.CreateDefaultDouble("Print", "PrintDpi");
			this.CreateDefaultInteger("Print", "PrintCentring");
			this.CreateDefaultDouble("Print", "PrintMargins");
			this.CreateDefaultDouble("Print", "PrintDebord");
			this.CreateDefaultBool("Print", "PrintTarget");

			this.CreateDefaultDouble("Export", "ImageDpi");
			this.CreateDefaultInteger("Export", "ImageDepth");
			this.CreateDefaultInteger("Export", "ImageCompression");
			this.CreateDefaultDouble("Export", "ImageQuality");
			this.CreateDefaultDouble("Export", "ImageAA");

			this.CreateDefaultRange("ExportPDF", "ExportPDFRange");
			this.CreateDefaultDouble("ExportPDF", "ExportPDFDebord");
			this.CreateDefaultBool("ExportPDF", "ExportPDFTarget");
			this.CreateDefaultBool("ExportPDF", "ExportPDFTextCurve");
			this.CreateDefaultInteger("ExportPDF", "ExportPDFColorConversion");
			this.CreateDefaultInteger("ExportPDF", "ExportPDFImageCompression");
			this.CreateDefaultDouble("ExportPDF", "ExportPDFJpegQuality");
			this.CreateDefaultDouble("ExportPDF", "ExportPDFImageMinDpi");
			this.CreateDefaultDouble("ExportPDF", "ExportPDFImageMaxDpi");
		}

		protected void CreateDefaultBool(string dialog, string name)
		{
			this.SetOwnerDialog(dialog, name);
			Bool sBool = this.Get(name) as Bool;
			if ( sBool == null )
			{
				sBool = new Bool(this.document, name);
				this.settings.Add(sBool);
			}
		}

		protected void CreateDefaultInteger(string dialog, string name)
		{
			this.SetOwnerDialog(dialog, name);
			Integer sInteger = this.Get(name) as Integer;
			if ( sInteger == null )
			{
				sInteger = new Integer(this.document, name);
				this.settings.Add(sInteger);
			}
		}

		protected void CreateDefaultDouble(string dialog, string name)
		{
			this.SetOwnerDialog(dialog, name);
			Double sDouble = this.Get(name) as Double;
			if ( sDouble == null )
			{
				sDouble = new Double(this.document, name);
				this.settings.Add(sDouble);
			}
		}

		protected void CreateDefaultString(string dialog, string name)
		{
			this.SetOwnerDialog(dialog, name);
			String sString = this.Get(name) as String;
			if ( sString == null )
			{
				sString = new String(this.document, name);
				this.settings.Add(sString);
			}
		}

		protected void CreateDefaultPoint(string dialog, string name)
		{
			this.SetOwnerDialog(dialog, name);
			Point sPoint = this.Get(name) as Point;
			if ( sPoint == null )
			{
				sPoint = new Point(this.document, name);
				this.settings.Add(sPoint);
			}
		}

		protected void CreateDefaultRange(string dialog, string name)
		{
			this.SetOwnerDialog(dialog, name);
			Range sRange = this.Get(name) as Range;
			if ( sRange == null )
			{
				sRange = new Range(this.document, name);
				this.settings.Add(sRange);
			}
		}

		// Spécifie quel est le dialogue propriétaire d'un réglage.
		protected void SetOwnerDialog(string dialog, string name)
		{
			if ( dialog == "" )  return;
			this.owners.Add(name, dialog);
		}

		// Indique à quel dialogue appartient un réglage.
		public string GetOwnerDialog(string name)
		{
			string dialog = this.owners[name] as string;
			if ( dialog == null )  return "";
			return dialog;
		}


		// Donne les réglages de l'impression.
		public PrintInfo PrintInfo
		{
			get { return this.printInfo; }
		}

		// Donne les réglages de la publication PDF.
		public ExportPDFInfo ExportPDFInfo
		{
			get { return this.exportPDFInfo; }
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


		#region Guides
		// Utilise le guides globaux ou locaux à la page courante.
		public bool GlobalGuides
		{
			get
			{
				return this.globalGuides;
			}

			set
			{
				if ( this.globalGuides != value )
				{
					this.globalGuides = value;
					this.document.Notifier.NotifyGuidesChanged();
					this.document.Notifier.NotifyArea(this.document.Modifier.ActiveViewer);
				}
			}
		}

		// Nombre total de guides.
		public int GuidesCount
		{
			get
			{
				return this.GuidesList.Count;
			}
		}

		// Guide sélectionné.
		public int GuidesSelected
		{
			get
			{
				return this.GuidesList.Selected;
			}

			set
			{
				this.GuidesList.Selected = value;
			}
		}

		// Supprime tous les guides.
		public void GuidesReset()
		{
			this.globalGuides = true;
			this.guides.Clear();
			this.document.Notifier.NotifyGuidesChanged();
			this.document.Notifier.NotifyArea(this.document.Modifier.ActiveViewer);
			this.document.IsDirtySerialize = true;
		}

		// Donne un guide.
		public Guide GuidesGet(int index)
		{
			return this.GuidesList[index] as Guide;
		}

		// Ajoute un nouveau guide.
		public int GuidesAdd(Guide guide)
		{
			int index = this.GuidesList.Add(guide);
			this.document.Notifier.NotifyGuidesChanged();
			this.document.Notifier.NotifyArea(this.document.Modifier.ActiveViewer);
			this.document.IsDirtySerialize = true;
			return index;
		}

		// Ajoute un nouveau guide dans l'autre (global/local) liste.
		public int GuidesAddOther(Guide guide)
		{
			int index = this.GuidesListOther.Add(guide);
			this.document.Notifier.NotifyGuidesChanged();
			this.document.Notifier.NotifyArea(this.document.Modifier.ActiveViewer);
			this.document.IsDirtySerialize = true;
			return index;
		}

		// Ajoute un nouveau guide.
		public void GuidesInsert(int index, Guide guide)
		{
			this.GuidesList.Insert(index, guide);
			this.document.Notifier.NotifyGuidesChanged();
			this.document.Notifier.NotifyArea(this.document.Modifier.ActiveViewer);
			this.document.IsDirtySerialize = true;
		}

		// Supprime un guide.
		public void GuidesRemoveAt(int index)
		{
			this.GuidesList.RemoveAt(index);
			this.document.Notifier.NotifyGuidesChanged();
			this.document.Notifier.NotifyArea(this.document.Modifier.ActiveViewer);
			this.document.IsDirtySerialize = true;
		}

		// Retourne la liste des repères.
		protected UndoableList GuidesList
		{
			get
			{
				if ( this.globalGuides )
				{
					return this.guides;
				}
				else
				{
					int cp = this.document.Modifier.ActiveViewer.DrawingContext.CurrentPage;
					Objects.Page page = this.document.GetObjects[cp] as Objects.Page;
					return page.Guides;
				}
			}
		}

		// Retourne l'autre (global/local) liste des repères.
		protected UndoableList GuidesListOther
		{
			get
			{
				if ( !this.globalGuides )
				{
					return this.guides;
				}
				else
				{
					int cp = this.document.Modifier.ActiveViewer.DrawingContext.CurrentPage;
					Objects.Page page = this.document.GetObjects[cp] as Objects.Page;
					return page.Guides;
				}
			}
		}

		// Retourne la liste des repères globaux.
		public UndoableList GuidesListGlobal
		{
			get
			{
				return this.guides;
			}
		}
		#endregion


		#region QuickFonts
		// Liste des polices rapides.
		public System.Collections.ArrayList QuickFonts
		{
			get
			{
				return this.quickFonts;
			}

			set
			{
				this.quickFonts = value;
			}
		}

		// Donne la liste des polices rapides par défaut.
		public static void DefaultQuickFonts(System.Collections.ArrayList list)
		{
			list.Clear();

			list.Add("Arial");
			list.Add("Courier New");
			list.Add("Tahoma");
			list.Add("Times New Roman");
		}
		#endregion


		#region Serialization
		// Sérialise les réglages.
		public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("Settings", this.settings);
			info.AddValue("GlobalGuides", this.globalGuides);
			info.AddValue("GuidesList", this.guides);
			info.AddValue("QuickFonts", this.quickFonts);
			info.AddValue("PrintInfo", this.printInfo);
			info.AddValue("ExportPDFInfo", this.exportPDFInfo);
		}

		// Constructeur qui désérialise les réglages.
		protected Settings(SerializationInfo info, StreamingContext context)
		{
			this.document = Document.ReadDocument;
			this.settings = (System.Collections.ArrayList) info.GetValue("Settings", typeof(System.Collections.ArrayList));
			this.guides = (UndoableList) info.GetValue("GuidesList", typeof(UndoableList));
			this.printInfo = (PrintInfo) info.GetValue("PrintInfo", typeof(PrintInfo));

			if ( this.document.IsRevisionGreaterOrEqual(1,0,10) )
			{
				this.globalGuides = info.GetBoolean("GlobalGuides");
			}
			else
			{
				this.globalGuides = true;
			}

			if ( this.document.IsRevisionGreaterOrEqual(1,2,5) )
			{
				this.quickFonts = (System.Collections.ArrayList) info.GetValue("QuickFonts", typeof(System.Collections.ArrayList));
			}
			else
			{
				this.quickFonts = new System.Collections.ArrayList();
				Settings.DefaultQuickFonts(this.quickFonts);
			}

			if ( this.document.IsRevisionGreaterOrEqual(1,0,21) )
			{
				this.exportPDFInfo = (ExportPDFInfo) info.GetValue("ExportPDFInfo", typeof(ExportPDFInfo));
			}
			else
			{
				this.exportPDFInfo = new ExportPDFInfo(this.document);
			}
		}

		// Adapte l'objet après une désérialisation.
		public void ReadFinalize()
		{
			this.CreateDefault();
		}
		#endregion


		protected Document						document;
		protected System.Collections.ArrayList	settings;
		protected System.Collections.Hashtable	owners;
		protected bool							globalGuides;
		protected UndoableList					guides;
		protected System.Collections.ArrayList	quickFonts;
		protected PrintInfo						printInfo;
		protected ExportPDFInfo					exportPDFInfo;
	}
}
