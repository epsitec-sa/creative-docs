//$.getScript('signalr/hubs', function() {
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
        this.registeredHubs = {};
        this.username = username;
        this.ready = false;
      },

      registerHub: function(name,HubFunc) {
        var me = this;
          var hubInstance = new HubFunc();
          me.registeredHubs[name] = hubInstance;
      },

      getHubByName: function(name)
      {
        return this.registeredHubs[name];
      },

      initHubs: function(con) {
        for (var key in this.registeredHubs)
        {
          this.registeredHubs[key].init(con,this.username, this);
        }
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
//});