Ext.require([
],
function() {
  Ext.define('Epsitec.cresus.webcore.hub.Notifications', {
    alternateClassName: ['Epsitec.Notifications'],

    hub: null,

    constructor: function(ToastrFunc, username) {
      var me = this;
      $.getScript('signalr/hubs', function() {
        $.connection.hub.logging = true;
        // Start the connection
        var toastrInstance = new ToastrFunc();
        $.connection.hub.start(function() {
          toastrInstance.init(username, me);
          me.initHub();
        });
      });
    },

    initHub: function() {
      this.hub = $.connection.notificationHub;
      this.hub.server.setupUserConnection();
    }
  });
});
