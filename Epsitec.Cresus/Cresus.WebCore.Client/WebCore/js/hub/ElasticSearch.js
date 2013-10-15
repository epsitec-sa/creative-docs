function ElasticSearch() {
  this.hub = $.connection.elasticSearchHub;

  //Initialize
  this.init = function(con) {
    this.hub = con.elasticSearchHub;
    //this.hub.state.connectionId = this.hub.connection.id;
    
  };

  //Entry points for calling hub
  this.hub.client.processResult = function(result) {
      Console.Log(result);
  };

  this.query = function(q) {
    this.hub.server.query(q);
  };

  
}
