using Epsitec.Common.Drawing;
using Epsitec.Common.Support;
using Epsitec.Common.Widgets;
using System.Runtime.Serialization;

namespace Epsitec.Common.Document.Settings
{
	/// <summary>
	/// La classe Integer contient un réglage numérique.
	/// </summary>
	[System.Serializable()]
	public class Integer : Abstract
	{
		public Integer(Document document, string name) : base(document, name)
		{
			this.Initialise();
		}

		protected void Initialise()
		{
			switch ( this.name )
			{
				case "DefaultUnit":
					this.text = "Unité de travail";
					this.minValue = 0;
					this.maxValue = 100;
					this.step = 1;
					break;
			}
		}

		public int Value
		{
			get
			{
				switch ( this.name )
				{
					case "DefaultUnit":
						return (int) this.document.Modifier.RealUnitDimension;
				}

				return 0;
			}

			set
			{
				switch ( this.name )
				{
					case "DefaultUnit":
						this.document.Modifier.RealUnitDimension = (RealUnitType) value;
						break;
				}
			}
		}

		public int MinValue
		{
			get
			{
				return this.minValue;
			}
		}

		public int MaxValue
		{
			get
			{
				return this.maxValue;
			}
		}

		public int Step
		{
			get
			{
				return this.step;
			}
		}


		public void InitCombo(TextFieldCombo combo)
		{
			switch ( this.name )
			{
				case "DefaultUnit":
					combo.Items.Add("Millimètres");
					combo.Items.Add("Centimètres");
					combo.Items.Add("Pouces");
					combo.SelectedIndex = Integer.TypeToInt(this.document.Modifier.RealUnitDimension);
					break;
			}
		}

		public static int TypeToInt(RealUnitType type)
		{
			if ( type == RealUnitType.DimensionMillimeter )  return 0;
			if ( type == RealUnitType.DimensionCentimeter )  return 1;
			if ( type == RealUnitType.DimensionInch       )  return 2;
			return -1;
		}

		public static RealUnitType IntToType(int rank)
		{
			if ( rank == 0 )  return RealUnitType.DimensionMillimeter;
			if ( rank == 1 )  return RealUnitType.DimensionCentimeter;
			if ( rank == 2 )  return RealUnitType.DimensionInch;
			return RealUnitType.None;
		}


		#region Serialization
		// Sérialise le réglage.
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("Value", this.Value);
		}

		// Constructeur qui désérialise le réglage.
		protected Integer(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.Value = info.GetInt32("Value");
			this.Initialise();
		}
		#endregion


		protected int				minValue;
		protected int				maxValue;
		protected int				step;
	}
}
