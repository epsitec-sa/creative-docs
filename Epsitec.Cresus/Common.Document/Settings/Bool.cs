using Epsitec.Common.Drawing;
using Epsitec.Common.Support;
using Epsitec.Common.Widgets;
using System.Runtime.Serialization;

namespace Epsitec.Common.Document.Settings
{
	/// <summary>
	/// La classe Bool contient un r�glage num�rique.
	/// </summary>
	public class Bool : Abstract
	{
		public Bool(Document document) : base(document)
		{
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
				}
			}
		}


		// Indique quel est le widget qui �dite ce r�glage.
		public void CheckButton(CheckButton widget)
		{
			this.checkButton = widget;
		}

		// Met � jour la valeur du r�glage.
		public override void UpdateValue()
		{
			this.checkButton.ActiveState = this.Value ? WidgetState.ActiveYes : WidgetState.ActiveNo;
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
			this.document = Document.ReadDocument;
			this.Value = info.GetBoolean("Value");
		}
		#endregion


		protected CheckButton		checkButton;
	}
}
