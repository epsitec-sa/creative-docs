using Epsitec.Common.Drawing;
using Epsitec.Common.Support;
using Epsitec.Common.Widgets;
using System.Runtime.Serialization;

namespace Epsitec.Common.Document.Settings
{
	/// <summary>
	/// La classe Double contient un réglage numérique.
	/// </summary>
	public class Double : Abstract
	{
		public Double(Document document) : base(document)
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

		public double MinValue
		{
			get
			{
				return this.minValue;
			}

			set
			{
				this.minValue = value;
			}
		}

		public double MaxValue
		{
			get
			{
				return this.maxValue;
			}

			set
			{
				this.maxValue = value;
			}
		}

		public double Step
		{
			get
			{
				return this.step;
			}

			set
			{
				this.step = value;
			}
		}

		public double Resolution
		{
			get
			{
				return this.resolution;
			}

			set
			{
				this.resolution = value;
			}
		}


		// Indique quel est le widget qui édite ce réglage.
		public void TextField(TextFieldSlider widget)
		{
			this.textField = widget;
		}

		// Met à jour la valeur du réglage.
		public override void UpdateValue()
		{
			this.textField.Value = (decimal) this.Value;
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
			this.document = Document.ReadDocument;
			this.Value = info.GetDouble("Value");
		}
		#endregion


		protected double			minValue;
		protected double			maxValue;
		protected double			step;
		protected double			resolution;
		protected TextFieldSlider	textField;
	}
}
