using Epsitec.Common.Drawing;
using Epsitec.Common.Support;
using Epsitec.Common.Widgets;
using System.Runtime.Serialization;

namespace Epsitec.Common.Document.Settings
{
	/// <summary>
	/// La classe Bool contient un réglage numérique.
	/// </summary>
	[System.Serializable()]
	public class Bool : Abstract
	{
		public Bool(Document document, string name) : base(document, name)
		{
			this.Initialize();
		}

		protected void Initialize()
		{
			this.conditionName = "";
			this.conditionState = false;

			switch ( this.name )
			{
				case "GridActive":
					this.text = Res.Strings.Dialog.Bool.GridActive;
					break;

				case "GridShow":
					this.text = Res.Strings.Dialog.Bool.GridShow;
					break;

				case "TextGridShow":
					this.text = Res.Strings.Dialog.Bool.TextGridShow;
					break;

				case Commands.TextShowControlCharacters:
					this.text = Res.Strings.Dialog.Bool.TextShowControlCharacters;
					break;

				case "TextFontFilter":
					this.text = Res.Strings.Dialog.Bool.TextFontFilter;
					break;

				case "TextFontSampleAbc":
					this.text = Res.Strings.Dialog.Bool.TextFontSampleAbc;
					break;

				case "GuidesActive":
					this.text = Res.Strings.Dialog.Bool.GuidesActive;
					break;

				case "GuidesShow":
					this.text = Res.Strings.Dialog.Bool.GuidesShow;
					break;

				case "GuidesMouse":
					this.text = Res.Strings.Dialog.Bool.GuidesMouse;
					break;

				case "PreviewActive":
					this.text = Res.Strings.Dialog.Bool.PreviewActive;
					break;

				case "RulersShow":
					this.text = Res.Strings.Dialog.Bool.RulersShow;
					break;

				case "LabelsShow":
					this.text = Res.Strings.Dialog.Bool.LabelsShow;
					break;

				case "MagnetActive":
					this.text = Res.Strings.Dialog.Bool.MagnetActive;
					break;

				case "PrintCollate":
					this.text = Res.Strings.Dialog.Bool.PrintCollate;
					break;

				case "PrintReverse":
					this.text = Res.Strings.Dialog.Bool.PrintReverse;
					break;

				case "PrintToFile":
					this.text = Res.Strings.Dialog.Bool.PrintToFile;
					break;

				case "PrintDraft":
					this.text = Res.Strings.Dialog.Bool.PrintDraft;
					break;

				case "PrintAutoLandscape":
					this.text = Res.Strings.Dialog.Bool.PrintAutoLandscape;
					break;

				case "PrintAutoZoom":
					this.text = Res.Strings.Dialog.Bool.PrintAutoZoom;
					break;

				case "PrintAA":
					this.text = Res.Strings.Dialog.Bool.PrintAA;
					break;

				case "PrintPerfectJoin":
					this.text = Res.Strings.Dialog.Bool.PrintPerfectJoin;
					this.conditionName = "PrintDraft";
					this.conditionState = true;
					break;

				case "PrintTarget":
					this.text = Res.Strings.Dialog.Bool.PrintTarget;
					this.conditionName = "PrintAutoZoom";
					this.conditionState = true;
					break;

				case "PrintDebugArea":
					this.text = Res.Strings.Dialog.Bool.PrintDebugArea;
					this.conditionName = "PrintDraft";
					this.conditionState = true;
					break;

				case "ImageOnlySelected":
				case "ExportICOOnlySelected":
					this.text = Res.Strings.Dialog.Bool.ImageOnlySelected;
					break;

				case "ExportPDFTarget":
					this.text = Res.Strings.Dialog.Bool.ExportPDFTarget;
					break;

				case "ExportPDFTextCurve":
					this.text = Res.Strings.Dialog.Bool.ExportPDFTextCurve;
					break;

				case "ExportPDFExecute":
					this.text = Res.Strings.Dialog.Bool.ExportPDFExecute;
					break;

				case "RepeatDuplicateMove":
					this.text = Res.Strings.Dialog.Bool.RepeatDuplicateMove;
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

					case "TextGridShow":
						return this.document.Modifier.ActiveViewer.DrawingContext.TextGridShow;

					case Commands.TextShowControlCharacters:
						return this.document.Modifier.ActiveViewer.DrawingContext.TextShowControlCharacters;

					case "TextFontFilter":
						return this.document.Modifier.ActiveViewer.DrawingContext.TextFontFilter;

					case "TextFontSampleAbc":
						return this.document.Modifier.ActiveViewer.DrawingContext.TextFontSampleAbc;

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

					case "MagnetActive":
						return this.document.Modifier.ActiveViewer.DrawingContext.MagnetActive;

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

					case "ImageOnlySelected":
					case "ExportICOOnlySelected":
						return this.document.Printer.ImageOnlySelected;

					case "ExportPDFTarget":
						return this.document.Settings.ExportPDFInfo.PrintCropMarks;

					case "ExportPDFTextCurve":
						return this.document.Settings.ExportPDFInfo.TextCurve;

					case "ExportPDFExecute":
						return this.document.Settings.ExportPDFInfo.Execute;

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

					case "TextGridShow":
						this.document.Modifier.ActiveViewer.DrawingContext.TextGridShow = value;
						break;

					case Commands.TextShowControlCharacters:
						this.document.Modifier.ActiveViewer.DrawingContext.TextShowControlCharacters = value;
						break;

					case "TextFontFilter":
						this.document.Modifier.ActiveViewer.DrawingContext.TextFontFilter = value;
						break;

					case "TextFontSampleAbc":
						this.document.Modifier.ActiveViewer.DrawingContext.TextFontSampleAbc = value;
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

					case "MagnetActive":
						this.document.Modifier.ActiveViewer.DrawingContext.MagnetActive = value;
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

					case "ImageOnlySelected":
					case "ExportICOOnlySelected":
						this.document.Printer.ImageOnlySelected = value;
						break;

					case "ExportPDFTarget":
						this.document.Settings.ExportPDFInfo.PrintCropMarks = value;
						break;

					case "ExportPDFTextCurve":
						this.document.Settings.ExportPDFInfo.TextCurve = value;
						break;

					case "ExportPDFExecute":
						this.document.Settings.ExportPDFInfo.Execute = value;
						break;

					case "RepeatDuplicateMove":
						this.document.Modifier.RepeatDuplicateMove = value;
						break;
				}
			}
		}


		public bool IsEnabled
		{
			get
			{
				bool enabled = true;

				switch (this.name)
				{
					case "ImageOnlySelected":
					case "ExportICOOnlySelected":
					case "ImageCropSelection":
					case "ExportICOCropSelection":
						enabled = (this.document.Modifier.TotalSelected > 0);
						break;
				}

				return enabled;
			}
		}


		#region Serialization
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			//	Sérialise le réglage.
			base.GetObjectData(info, context);
			info.AddValue("Value", this.Value);
		}

		protected Bool(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			//	Constructeur qui désérialise le réglage.
			this.Value = info.GetBoolean("Value");
			this.Initialize();
		}
		#endregion
	}
}
