function NotificationsToastr() {
  this.hub = $.connection.notificationHub;
  var callbackFunc = null;

  //Initialize
  this.init = function(username,afterNavigationCallback) {
    this.hub.state.connectionId = this.hub.connection.id;
    this.hub.state.userName = username;
    callbackFunc = afterNavigationCallback;
  };

  //Entry points for calling hub

  this.WarningToastTo = function(connectionId, title, message, datasetId, entityId) {
    this.hub.server.warningToast(connectionId, title, message, datasetId, entityId);

  };

  this.hub.client.StickyWarningNavToast = function(title, msg, datasetId, entityId) {
    var path = {};
    path.id = entityId;
    path.name = datasetId;

    toastr.options = {
      'debug': false,
      'positionClass': 'toast-bottom-full-width',
      'onclick': function() {
          Epsitec.Cresus.Core.app.showEditableEntity(path,callbackFunc);
      },
      'fadeIn': 300,
      'fadeOut': 1000,
      'timeOut': 0
    };
    toastr.warning(msg, title);
  };

  this.hub.client.Toast = function(title, msg, datasetId, entityId) {
    var path = {};
    path.id = entityId;
    path.name = datasetId;

    toastr.options = {
      'debug': false,
      'positionClass': 'toast-bottom-full-width',
      'fadeOut': 1000,
      'timeOut': 5000,
      'extendedTimeOut': 1000
    };
    toastr.info(msg, title);
  };
}
