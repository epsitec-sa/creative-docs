using Epsitec.Common.Drawing;
using Epsitec.Common.Support;
using Epsitec.Common.Widgets;
using System.Runtime.Serialization;

namespace Epsitec.Common.Document.Settings
{
	/// <summary>
	/// La classe Double contient un r�glage num�rique.
	/// </summary>
	[System.Serializable()]
	public class Double : Abstract
	{
		public Double(Document document, string name) : base(document, name)
		{
			this.Initialise();
		}

		protected void Initialise()
		{
			switch ( this.name )
			{
				case "PrintDpi":
					this.text = "R�solution (dpi)";
					this.integer = true;
					this.factorMinValue = 150.0;
					this.factorMaxValue = 600.0;
					this.factorStep = 50.0;
					break;
			}
		}

		public double Value
		{
			get
			{
				switch ( this.name )
				{
					case "PrintDpi":
						return this.document.Settings.PrintInfo.Dpi;
				}

				return 0.0;
			}

			set
			{
				switch ( this.name )
				{
					case "PrintDpi":
						this.document.Settings.PrintInfo.Dpi = value;
						break;
				}
			}
		}

		public double FactorMinValue
		{
			get
			{
				return this.factorMinValue;
			}
		}

		public double FactorMaxValue
		{
			get
			{
				return this.factorMaxValue;
			}
		}

		public double FactorStep
		{
			get
			{
				return this.factorStep;
			}
		}

		public bool Integer
		{
			get
			{
				return this.integer;
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
		protected Double(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.Value = info.GetDouble("Value");
			this.Initialise();
		}
		#endregion


		protected double			factorMinValue = -1.0;
		protected double			factorMaxValue = 1.0;
		protected double			factorStep = 1.0;
		protected bool				integer = false;
	}
}
