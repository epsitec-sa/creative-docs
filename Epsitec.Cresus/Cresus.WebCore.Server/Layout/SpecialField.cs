//	Copyright © 2012-2014, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Cresus.WebCore.Server.Layout
{
	/// <summary>
	/// This class represents a special field that is used to edit a value.
	/// </summary>
	internal sealed class SpecialField : AbstractField<SpecialField>
	{
		public string							EntityId
		{
			get;
			set;
		}


		public string							ControllerName
		{
			get;
			set;
		}


		public string							FieldName
		{
			get;
			set;
		}



		public override Dictionary<string, object> ToDictionary()
		{
			var field = base.ToDictionary ();

			field["entityId"] = this.EntityId;
			field["controllerName"] = this.ControllerName;
			field["fieldName"] = this.FieldName;

			return field;
		}
	}
}
