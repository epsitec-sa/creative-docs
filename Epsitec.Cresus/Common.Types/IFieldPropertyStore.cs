//	Copyright © 2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Common.Types
{
	public interface IFieldPropertyStore
	{
		object GetValue(string fieldId, DependencyProperty property);
		bool ContainsValue(string fieldId, DependencyProperty property);
	}
}
