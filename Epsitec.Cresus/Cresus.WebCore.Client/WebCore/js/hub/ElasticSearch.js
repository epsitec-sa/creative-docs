function ElasticSearch() {
  this.hub = $.connection.elasticSearchHub;

  //Initialize
  this.init = function(con) {
    this.hub = con.elasticSearchHub;
    //this.hub.state.connectionId = this.hub.connection.id;
    
  };

  //Entry points for calling hub
  this.hub.client.processResult = function(result) {
    Ext.Array.each(result.Documents, function (doc)
    {
        toastr.options = {
          debug: false,
          positionClass: 'toast-top-right',
          fadeOut: 1000,
          timeOut: 5000,
          extendedTimeOut: 1000
        };
        toastr.info(doc.Text, doc.Name);
    });
      
  };

  this.query = function(q) {
    this.hub.server.query(q);
  };

  this.createDocument = function(entity) {
    this.hub.server.indexDocument(entity.id,entity.summary,entity.summary,entity.entityType);
  };

  
}
