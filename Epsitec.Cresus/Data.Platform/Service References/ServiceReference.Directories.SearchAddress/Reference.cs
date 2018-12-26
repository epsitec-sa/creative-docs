﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Epsitec.Data.Platform.ServiceReference.Directories.SearchAddress {
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(Namespace="urn:directories/search/v4/searchaddress", ConfigurationName="ServiceReference.Directories.SearchAddress.SearchAddressSoap")]
    public interface SearchAddressSoap {
        
        [System.ServiceModel.OperationContractAttribute(Action="urn:directories/search/v4/searchaddress/SearchAddress", ReplyAction="*")]
        string SearchAddress(string addressParam);
        
        [System.ServiceModel.OperationContractAttribute(Action="urn:directories/search/v4/searchaddress/SearchLocation", ReplyAction="*")]
        string SearchLocation(string locationParam);
        
        [System.ServiceModel.OperationContractAttribute(Action="urn:directories/search/v4/searchaddress/SearchService", ReplyAction="*")]
        string SearchService(string serviceListParam);
        
        [System.ServiceModel.OperationContractAttribute(Action="urn:directories/search/v4/searchaddress/Version", ReplyAction="*")]
        string Version();
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface SearchAddressSoapChannel : Epsitec.Data.Platform.ServiceReference.Directories.SearchAddress.SearchAddressSoap, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class SearchAddressSoapClient : System.ServiceModel.ClientBase<Epsitec.Data.Platform.ServiceReference.Directories.SearchAddress.SearchAddressSoap>, Epsitec.Data.Platform.ServiceReference.Directories.SearchAddress.SearchAddressSoap {
        
        public SearchAddressSoapClient() {
        }
        
        public SearchAddressSoapClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public SearchAddressSoapClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public SearchAddressSoapClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public SearchAddressSoapClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public string SearchAddress(string addressParam) {
            return base.Channel.SearchAddress(addressParam);
        }
        
        public string SearchLocation(string locationParam) {
            return base.Channel.SearchLocation(locationParam);
        }
        
        public string SearchService(string serviceListParam) {
            return base.Channel.SearchService(serviceListParam);
        }
        
        public string Version() {
            return base.Channel.Version();
        }
    }
}
