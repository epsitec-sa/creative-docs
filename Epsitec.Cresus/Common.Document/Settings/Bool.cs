using Epsitec.Common.Drawing;
using Epsitec.Common.Support;
using Epsitec.Common.Widgets;
using System.Runtime.Serialization;

namespace Epsitec.Common.Document.Settings
{
	/// <summary>
	/// La classe Bool contient un r�glage num�rique.
	/// </summary>
	[System.Serializable()]
	public class Bool : Abstract
	{
		public Bool(Document document, string name) : base(document, name)
		{
			this.Initialise();
		}

		protected void Initialise()
		{
			this.conditionName = "";
			this.conditionState = false;

			switch ( this.name )
			{
				case "GridActive":
					this.text = "Grille active";
					break;

				case "GridShow":
					this.text = "Grille visible";
					break;

				case "GuidesActive":
					this.text = "Rep�res actifs";
					break;

				case "GuidesShow":
					this.text = "Rep�res visibles";
					break;

				case "GuidesMouse":
					this.text = "D�placements avec la souris";
					break;

				case "PreviewActive":
					this.text = "Comme imprim�";
					break;

				case "RulersShow":
					this.text = "R�gles visibles";
					break;

				case "LabelsShow":
					this.text = "Affiche le nom des objets";
					break;

				case "PrintCollate":
					this.text = "Copies assembl�es (1,2,3 - 1,2,3)";
					break;

				case "PrintReverse":
					this.text = "Ordre invers�";
					break;

				case "PrintToFile":
					this.text = "Imprimer dans un fichier";
					break;

				case "PrintDraft":
					this.text = "Brouillon (pas de d�grad� ni de transparence)";
					break;

				case "PrintAutoLandscape":
					this.text = "Portrait/paysage automatique";
					break;

				case "PrintAutoZoom":
					this.text = "Adapter l'impression � la taille de la page";
					break;

				case "PrintAA":
					this.text = "Anti-cr�nelage (pour imprimante couleur)";
					break;

				case "PrintPerfectJoin":
					this.text = "Jointures parfaites (entre les zones d'une page)";
					this.conditionName = "PrintDraft";
					this.conditionState = true;
					break;

				case "PrintTarget":
					this.text = "Traits de coupe (hors de la page)";
					this.conditionName = "PrintAutoZoom";
					this.conditionState = true;
					break;

				case "PrintDebugArea":
					this.text = "Imprime les zones d�tect�es (debug)";
					this.conditionName = "PrintDraft";
					this.conditionState = true;
					break;

				case "RepeatDuplicateMove":
					this.text = "Duplique avec r�p�tition du dernier d�placement";
					break;
			}
		}

		public bool Value
		{
			get
			{
				switch ( this.name )
				{
					case "GridActive":
						return this.document.Modifier.ActiveViewer.DrawingContext.GridActive;

					case "GridShow":
						return this.document.Modifier.ActiveViewer.DrawingContext.GridShow;

					case "GuidesActive":
						return this.document.Modifier.ActiveViewer.DrawingContext.GuidesActive;

					case "GuidesShow":
						return this.document.Modifier.ActiveViewer.DrawingContext.GuidesShow;

					case "GuidesMouse":
						return this.document.Modifier.ActiveViewer.DrawingContext.GuidesMouse;

					case "PreviewActive":
						return this.document.Modifier.ActiveViewer.DrawingContext.PreviewActive;

					case "RulersShow":
						return this.document.Modifier.ActiveViewer.DrawingContext.RulersShow;

					case "LabelsShow":
						return this.document.Modifier.ActiveViewer.DrawingContext.LabelsShow;

					case "PrintCollate":
						return this.document.Settings.PrintInfo.Collate;

					case "PrintReverse":
						return this.document.Settings.PrintInfo.Reverse;

					case "PrintToFile":
						return this.document.Settings.PrintInfo.PrintToFile;

					case "PrintDraft":
						return this.document.Settings.PrintInfo.ForceSimply;

					case "PrintAutoLandscape":
						return this.document.Settings.PrintInfo.AutoLandscape;

					case "PrintAutoZoom":
						return this.document.Settings.PrintInfo.AutoZoom;

					case "PrintAA":
						return (this.document.Settings.PrintInfo.Gamma != 0.0);

					case "PrintPerfectJoin":
						return this.document.Settings.PrintInfo.PerfectJoin;

					case "PrintTarget":
						return this.document.Settings.PrintInfo.Target;

					case "PrintDebugArea":
						return this.document.Settings.PrintInfo.DebugArea;

					case "RepeatDuplicateMove":
						return this.document.Modifier.RepeatDuplicateMove;
				}

				return false;
			}

			set
			{
				switch ( this.name )
				{
					case "GridActive":
						this.document.Modifier.ActiveViewer.DrawingContext.GridActive = value;
						break;

					case "GridShow":
						this.document.Modifier.ActiveViewer.DrawingContext.GridShow = value;
						break;

					case "GuidesActive":
						this.document.Modifier.ActiveViewer.DrawingContext.GuidesActive = value;
						break;

					case "GuidesShow":
						this.document.Modifier.ActiveViewer.DrawingContext.GuidesShow = value;
						break;

					case "GuidesMouse":
						this.document.Modifier.ActiveViewer.DrawingContext.GuidesMouse = value;
						break;

					case "PreviewActive":
						this.document.Modifier.ActiveViewer.DrawingContext.PreviewActive = value;
						break;

					case "RulersShow":
						this.document.Modifier.ActiveViewer.DrawingContext.RulersShow = value;
						break;

					case "LabelsShow":
						this.document.Modifier.ActiveViewer.DrawingContext.LabelsShow = value;
						break;

					case "PrintCollate":
						this.document.Settings.PrintInfo.Collate = value;
						break;

					case "PrintReverse":
						this.document.Settings.PrintInfo.Reverse = value;
						break;

					case "PrintToFile":
						this.document.Settings.PrintInfo.PrintToFile = value;
						break;

					case "PrintDraft":
						this.document.Settings.PrintInfo.ForceSimply = value;
						break;

					case "PrintAutoLandscape":
						this.document.Settings.PrintInfo.AutoLandscape = value;
						break;

					case "PrintAutoZoom":
						this.document.Settings.PrintInfo.AutoZoom = value;
						break;

					case "PrintAA":
						this.document.Settings.PrintInfo.Gamma = value ? 1.0 : 0.0;
						break;

					case "PrintPerfectJoin":
						this.document.Settings.PrintInfo.PerfectJoin = value;
						break;

					case "PrintTarget":
						this.document.Settings.PrintInfo.Target = value;
						break;

					case "PrintDebugArea":
						this.document.Settings.PrintInfo.DebugArea = value;
						break;

					case "RepeatDuplicateMove":
						this.document.Modifier.RepeatDuplicateMove = value;
						break;
				}
			}
		}


		#region Serialization
		// S�rialise le r�glage.
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("Value", this.Value);
		}

		// Constructeur qui d�s�rialise le r�glage.
		protected Bool(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.Value = info.GetBoolean("Value");
			this.Initialise();
		}
		#endregion
	}
}
