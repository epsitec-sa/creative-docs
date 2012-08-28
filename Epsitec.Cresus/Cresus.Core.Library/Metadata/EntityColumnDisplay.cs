//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Support.Extensions;
using Epsitec.Common.Types;
using Epsitec.Cresus.DataLayer.Expressions;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Xml.Linq;

namespace Epsitec.Cresus.Core.Metadata
{
	public class EntityColumnDisplay
	{
		public EntityColumnDisplay()
		{
		}


		public ColumnDisplayMode				Mode
		{
			get;
			set;
		}


		public XElement Save(string xmlNodeName)
		{
			return new XElement (xmlNodeName,
				new XAttribute (Xml.DisplayMode, this.Mode.ToString ()));
		}

		public static EntityColumnDisplay Restore(XElement xml)
		{
			if (xml == null)
			{
				return null;
			}

			return new EntityColumnDisplay ()
			{
				Mode = InvariantConverter.ToEnum (xml.Attribute (Xml.DisplayMode), ColumnDisplayMode.Visible)
			};
		}


		#region Xml Class

		private static class Xml
		{
			public static readonly string		DisplayMode = "m";
		}

		#endregion
	}
}
