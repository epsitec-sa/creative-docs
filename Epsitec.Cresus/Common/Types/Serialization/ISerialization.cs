//	Copyright © 2007-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Common.Types.Serialization
{
	/// <summary>
	/// The <c>ISerialization</c> interface is used to notify objects immediately
	/// before their serialization.
	/// </summary>
	public interface ISerialization
	{
		bool NotifySerializationStarted(Serialization.Context context);
		void NotifySerializationCompleted(Serialization.Context context);
	}
}
