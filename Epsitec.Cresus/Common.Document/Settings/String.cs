using Epsitec.Common.Drawing;
using Epsitec.Common.Support;
using Epsitec.Common.Widgets;
using System.Runtime.Serialization;

namespace Epsitec.Common.Document.Settings
{
	/// <summary>
	/// La classe String contient un r�glage num�rique.
	/// </summary>
	[System.Serializable()]
	public class String : Abstract
	{
		public String(Document document, string name) : base(document, name)
		{
			this.Initialise();
		}

		protected void Initialise()
		{
			this.conditionName = "";
			this.conditionState = false;

			switch ( this.name )
			{
				case "PrintName":
					this.text = "";
					break;

				case "PrintFilename":
					this.text = "";
					break;
			}
		}

		public string Value
		{
			get
			{
				switch ( this.name )
				{
					case "PrintName":
						return this.document.Settings.PrintInfo.PrintName;

					case "PrintFilename":
						return this.document.Settings.PrintInfo.PrintFilename;
				}

				return "";
			}

			set
			{
				switch ( this.name )
				{
					case "PrintName":
						this.document.Settings.PrintInfo.PrintName = value;
						break;

					case "PrintFilename":
						this.document.Settings.PrintInfo.PrintFilename = value;
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
		protected String(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.Value = info.GetString("Value");
			this.Initialise();
		}
		#endregion
	}
}
