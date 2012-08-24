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

namespace Epsitec.Cresus.Core.Data.Metadata
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
				new XAttribute (Strings.DisplayMode, this.Mode.ToString ()));
		}

		public static EntityColumnDisplay Restore(XElement xml)
		{
			if (xml == null)
			{
				return null;
			}

			return new EntityColumnDisplay ()
			{
				Mode = InvariantConverter.ToEnum (xml.Attribute (Strings.DisplayMode), ColumnDisplayMode.Visible)
			};
		}


		#region Strings Class

		private static class Strings
		{
			public static readonly string		DisplayMode = "mode";
		}

		#endregion
	}
}
