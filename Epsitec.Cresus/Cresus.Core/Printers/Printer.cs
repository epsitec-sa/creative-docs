//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Support;

using System.Collections.Generic;
using System.IO;
using System.Globalization;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace Epsitec.Cresus.Core.Printers
{

	class Printer
	{
		public Printer()
		{
		}

		public Printer(string logicalName, string physicalName, string tray, bool horizontal, double xOffset, double yOffset, string comment)
		{
			this.LogicalName  = logicalName;
			this.PhysicalName = physicalName;
			this.Tray         = tray;
			this.Horizontal   = horizontal;
			this.XOffset      = xOffset;
			this.YOffset      = yOffset;
			this.Comment      = comment;
		}

		public string LogicalName
		{
			get;
			set;
		}

		public string PhysicalName
		{
			get;
			set;
		}

		public string Tray
		{
			get;
			set;
		}

		public bool Horizontal
		{
			get;
			set;
		}

		public double XOffset
		{
			get;
			set;
		}

		public double YOffset
		{
			get;
			set;
		}

		public string Comment
		{
			get;
			set;
		}


		public string NiceDescription
		{
			//	Retourne une description consise et claire de l'imprimante.
			get
			{
				if (string.IsNullOrWhiteSpace (this.Comment))
				{
					return TextFormatter.FormatText (this.LogicalName, "(", this.PhysicalName, ",~", this.Tray, ")").ToString ();
				}
				else
				{
					return TextFormatter.FormatText (this.LogicalName, "(", this.Comment, ")").ToString ();
				}
			}
		}


		public string GetSerializableContent()
		{
			return string.Concat
				(
					"LogicalName=",
					this.LogicalName,

					Printer.serializableSeparator,
					
					"PhysicalName=",
					this.PhysicalName,

					Printer.serializableSeparator,
					
					"Tray=",
					this.Tray,

					Printer.serializableSeparator,
					
					"XOffset=",
					this.XOffset.ToString (CultureInfo.InvariantCulture),

					Printer.serializableSeparator,
					
					"YOffset=",
					this.YOffset.ToString (CultureInfo.InvariantCulture),

					Printer.serializableSeparator,
					
					"Comment=",
					this.Comment
				);
		}

		public void SetSerializableContent(string content)
		{
			var list = content.Split (new string[] { Printer.serializableSeparator }, System.StringSplitOptions.RemoveEmptyEntries);

			foreach (var line in list)
			{
				var words = line.Split (new string[] { "=" }, System.StringSplitOptions.None);

				if (words.Length == 2)
				{
					switch (words[0])
					{
						case "LogicalName":
							this.LogicalName = words[1];
							break;

						case "PhysicalName":
							this.PhysicalName = words[1];
							break;

						case "Tray":
							this.Tray = words[1];
							break;

						case "XOffset":
							this.XOffset = double.Parse (words[1]);
							break;

						case "YOffset":
							this.YOffset = double.Parse (words[1]);
							break;

						case "Comment":
							this.Comment = words[1];
							break;
					}
				}
			}
		}


		private static readonly string serializableSeparator = "•";  // puce, unicode 2022
	}
}
