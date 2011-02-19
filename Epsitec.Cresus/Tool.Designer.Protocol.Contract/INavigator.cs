using System.Collections.Generic;
using System.ServiceModel;

namespace Epsitec.Designer.Protocol
{
	[ServiceContract]
	public interface INavigator
	{
		[OperationContract]
		void NavigateToString(string id);
		
		[OperationContract]
		void NavigateToCaption(string id);
		
		[OperationContract]
		void NavigateToEntityField(string id);
	}
}
