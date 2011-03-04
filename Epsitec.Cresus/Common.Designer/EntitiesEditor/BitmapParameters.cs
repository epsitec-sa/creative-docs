using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Types;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Designer.EntitiesEditor
{
	public class BitmapParameters
	{
		public BitmapParameters()
		{
			this.Zoom = 1.0;
			this.GenerateUserCartridge = true;
			this.GenerateDateCartridge = true;
			this.GenerateSamplesCartridge = true;
		}


		public double Zoom
		{
			get;
			set;
		}

		public bool GenerateUserCartridge
		{
			get;
			set;
		}

		public bool GenerateDateCartridge
		{
			get;
			set;
		}

		public bool GenerateSamplesCartridge
		{
			get;
			set;
		}


		public string GetSerializeData()
		{
			var list = new List<string> ();

			list.Add (this.Zoom.ToString (System.Globalization.CultureInfo.InvariantCulture));
			list.Add (this.GenerateUserCartridge.ToString (System.Globalization.CultureInfo.InvariantCulture));
			list.Add (this.GenerateDateCartridge.ToString (System.Globalization.CultureInfo.InvariantCulture));
			list.Add (this.GenerateSamplesCartridge.ToString (System.Globalization.CultureInfo.InvariantCulture));

			return string.Join ("●", list);
		}

		public void SetSerializeData(string data)
		{
			if (!string.IsNullOrEmpty (data))
			{
				var parts = data.Split ('●');
				double d;
				bool b;

				if (parts.Length > 0 && double.TryParse (parts[0], out d))
				{
					this.Zoom = d;
				}

				if (parts.Length > 1 && bool.TryParse (parts[1], out b))
				{
					this.GenerateUserCartridge = b;
				}

				if (parts.Length > 2 && bool.TryParse (parts[2], out b))
				{
					this.GenerateDateCartridge = b;
				}

				if (parts.Length > 3 && bool.TryParse (parts[3], out b))
				{
					this.GenerateSamplesCartridge = b;
				}
			}
		}
	}
}
