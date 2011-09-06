//	Copyright © 2010-2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Binders
{
	/// <summary>
	/// The <c>IFieldBinder</c> interface provides conversion and validation
	/// methods used by the UI binding code, when reading/writing data stored in
	/// entity fields.
	/// </summary>
	public interface IFieldBinder
	{
		string ConvertToUI(string value);
		string ConvertFromUI(string value);
		
		IValidationResult ValidateFromUI(string value);
	}
}
