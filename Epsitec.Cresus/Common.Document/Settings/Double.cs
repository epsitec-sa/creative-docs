using Epsitec.Common.Drawing;
using Epsitec.Common.Support;
using Epsitec.Common.Widgets;
using System.Runtime.Serialization;

namespace Epsitec.Common.Document.Settings
{
	/// <summary>
	/// La classe Double contient un réglage numérique.
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
		}

		public double Value
		{
			get
			{
				return 0.0;
			}

			set
			{
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


		#region Serialization
		// Sérialise le réglage.
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("Value", this.Value);
		}

		// Constructeur qui désérialise le réglage.
		protected Double(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.Value = info.GetDouble("Value");
			this.Initialise();
		}
		#endregion


		protected double			factorMinValue;
		protected double			factorMaxValue;
		protected double			factorStep;
		protected double			resolution;
	}
}
