$.getScript('signalr/hubs', function() {
  Ext.require([
  ],
  function() {
    Ext.define('Epsitec.cresus.webcore.hub.Hubs', {
      alternateClassName: ['Epsitec.Hubs'],

      hub: null,
      username: null,
      registeredHubs: null,
      ready: null,
      constructor: function(username) {
        this.registeredHubs = [];
        this.username = username;
        this.ready = false;
      },

      registerHub: function(HubFunc) {
        var me = this;
          var hubInstance = new HubFunc();
          me.registeredHubs.push(hubInstance);
      },

      initHubs: function(con) {
        var me = this;
        Ext.Array.each(this.registeredHubs, function(rhub){
          rhub.init(con,me.username, me);
        });
      },

      start: function() {
        var me = this;
        
          $.connection.hub.logging = false;
          // Start the connection
          $.connection.hub.start().done(function () {
            me.initHubs($.connection);
          });
        
      }

    });
  });
});