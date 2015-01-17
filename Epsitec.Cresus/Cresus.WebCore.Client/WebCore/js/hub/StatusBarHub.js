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

  //Entry points for hub call
  this.hub.client.AddToBar = function(type, text,iconClass, statusId) {
    var app = Epsitec.Cresus.Core.getApplication();
    var status = {
          type: type,
          text: text,
          icon: iconClass,
          id: statusId
        };

    app.addEntityToStatusBar(status);
  };

  this.hub.client.RemoveFromBar = function(statusId) {
    var app = Epsitec.Cresus.Core.getApplication();
    var status = {
          id: statusId
        };

    app.removeEntityFromStatusBar(status);
  };
}
