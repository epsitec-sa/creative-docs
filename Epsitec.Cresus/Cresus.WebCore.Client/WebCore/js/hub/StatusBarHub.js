function StatusBarHub() {
  this.hub = $.connection.statusBarHub;
  var statusBarClient = null;

  //Initialize
  this.init = function(con,username, client) {   
    this.hub = con.statusBarHub;
    this.hub.state.connectionId = this.hub.connection.id;
    this.hub.state.userName = username;
    statusBarClient = client;
    this.hub.server.setupUserConnection();
  };


  //Entry points for calling hub
  this.RemoveFromMyBar = function(entityId) {
    this.hub.server.removeFromMyBar(entityId);

  };

  this.AddToMyBar = function(title,summary,entityId) {
    this.hub.server.addToMyBar(title,summary,entityId);
  };

  //Entry points for hub call
  this.hub.client.AddToBar = function(title, summary, entityId) {
    var app = Epsitec.Cresus.Core.getApplication();
    var entity = {
          summary: summary,
          entityType: title,
          id: entityId
        };

    app.addEntityToStatusBar(entity);
  };

  this.hub.client.RemoveFromBar = function(entityId) {
    var app = Epsitec.Cresus.Core.getApplication();
    var entity = {
          id: entityId
        };

    app.removeEntityFromStatusBar(entity);
  };

  this.hub.client.SetLoading = function(state) {
    var app = Epsitec.Cresus.Core.getApplication();
    app.viewport.setLoading(state);
  };
}
