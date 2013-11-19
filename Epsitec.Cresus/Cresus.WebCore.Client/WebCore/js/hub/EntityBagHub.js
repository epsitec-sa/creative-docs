function EntityBagHub() {
  this.hub = $.connection.entityBagHub;
  var entityBagClient = null;

  //Initialize
  this.init = function(con,username, client) {   
    this.hub = con.entityBagHub;
    this.hub.state.connectionId = this.hub.connection.id;
    this.hub.state.userName = username;
    entityBagClient = client;
    this.hub.server.setupUserConnection();
  };

  //Entry points for calling hub
  this.hub.client.AddToBag = function(title, summary, entityId) {
    var app = Epsitec.Cresus.Core.getApplication();
    var entity = {
          summary: summary,
          entityType: title,
          id: entityId
        };

    app.addEntityToBag(entity);
  };

  this.hub.client.RemoveFromBag = function(entityId) {
    var app = Epsitec.Cresus.Core.getApplication();
    var entity = {
          id: entityId
        };

    app.removeEntityFromBag(entity);
  };

  this.hub.client.SetLoading = function(state) {
    var app = Epsitec.Cresus.Core.getApplication();
    app.viewport.setLoading(state);
  };
  
}
