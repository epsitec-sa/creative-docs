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

      var path = {
          databaseName: doc.DatasetId,
          entityId: doc.EntityId
        };

      toastr.options = null;
      toastr.options = {
          debug: false,
          positionClass: 'toast-top-full-width',
          fadeOut: 1000,
          timeOut: 5000,
          extendedTimeOut: 1000
      };

      toastr.options.onclick = function() {
        Epsitec.Cresus.Core.app.showEntity(path, null);
      };

      toastr.info(doc.Text, doc.Name);
    });
      
  };

  this.query = function(q) {
    this.hub.server.query(q);
  };
}
