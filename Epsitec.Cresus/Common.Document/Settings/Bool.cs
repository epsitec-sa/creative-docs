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
			this.Initialise();
		}

		protected void Initialise()
		{
			switch ( this.name )
			{
				case "GridActive":
					this.text = "Grille active";
					break;

				case "GridShow":
					this.text = "Grille visible";
					break;

				case "GuidesActive":
					this.text = "Repères actifs";
					break;

				case "GuidesShow":
					this.text = "Repères visibles";
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


		#region Serialization
		// Sérialise le réglage.
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("Value", this.Value);
		}

		// Constructeur qui désérialise le réglage.
		protected Bool(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.Value = info.GetBoolean("Value");
			this.Initialise();
		}
		#endregion
	}
}
