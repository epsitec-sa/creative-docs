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
	}
}
