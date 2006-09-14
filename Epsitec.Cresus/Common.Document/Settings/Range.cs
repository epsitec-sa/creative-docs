using Epsitec.Common.Drawing;
using Epsitec.Common.Support;
using Epsitec.Common.Widgets;
using System.Runtime.Serialization;

namespace Epsitec.Common.Document.Settings
{
	/// <summary>
	/// La classe Range contient une plage "de à".
	/// </summary>
	[System.Serializable()]
	public class Range : Abstract
	{
		public Range(Document document, string name) : base(document, name)
		{
			this.Initialize();
		}

		protected void Initialize()
		{
			this.conditionName = "";
			this.conditionState = false;

			switch ( this.name )
			{
				case "PrintRange":
					this.text = "";
					break;

				case "ExportPDFRange":
					this.text = "";
					break;
			}
		}

		public PrintRange PrintRange
		{
			get
			{
				switch ( this.name )
				{
					case "PrintRange":
						return this.document.Settings.PrintInfo.PrintRange;

					case "ExportPDFRange":
						return this.document.Settings.ExportPDFInfo.PageRange;
				}

				return 0;
			}

			set
			{
				switch ( this.name )
				{
					case "PrintRange":
						this.document.Settings.PrintInfo.PrintRange = value;
						break;

					case "ExportPDFRange":
						this.document.Settings.ExportPDFInfo.PageRange = value;
						break;
				}
			}
		}

		public int From
		{
			get
			{
				switch ( this.name )
				{
					case "PrintRange":
						return this.document.Settings.PrintInfo.PrintFrom;

					case "ExportPDFRange":
						return this.document.Settings.ExportPDFInfo.PageFrom;
				}

				return 0;
			}

			set
			{
				switch ( this.name )
				{
					case "PrintRange":
						this.document.Settings.PrintInfo.PrintFrom = value;
						break;

					case "ExportPDFRange":
						this.document.Settings.ExportPDFInfo.PageFrom = value;
						break;
				}
			}
		}

		public int To
		{
			get
			{
				switch ( this.name )
				{
					case "PrintRange":
						return this.document.Settings.PrintInfo.PrintTo;

					case "ExportPDFRange":
						return this.document.Settings.ExportPDFInfo.PageTo;
				}

				return 0;
			}

			set
			{
				switch ( this.name )
				{
					case "PrintRange":
						this.document.Settings.PrintInfo.PrintTo = value;
						break;

					case "ExportPDFRange":
						this.document.Settings.ExportPDFInfo.PageTo = value;
						break;
				}
			}
		}

		public int Min
		{
			get
			{
				switch ( this.name )
				{
					case "PrintRange":
					case "ExportPDFRange":
						return 1;
				}

				return 0;
			}
		}

		public int Max
		{
			get
			{
				switch ( this.name )
				{
					case "PrintRange":
					case "ExportPDFRange":
						return System.Math.Max(1, this.document.Modifier.PrintableTotalPages());
				}

				return 10000;
			}
		}



		#region Serialization
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			//	Sérialise le réglage.
			base.GetObjectData(info, context);
			info.AddValue("PrintRange", this.PrintRange);
			info.AddValue("PrintFrom", this.From);
			info.AddValue("PrintTo", this.To);
		}

		protected Range(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			//	Constructeur qui désérialise le réglage.
			this.PrintRange = (PrintRange) info.GetValue("PrintRange", typeof(PrintRange));
			this.From = info.GetInt32("PrintFrom");
			this.To = info.GetInt32("PrintTo");
			this.Initialize();
		}
		#endregion
	}
}
