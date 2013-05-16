Ext.define('Epsitec.cresus.webcore.hub.Notifications', {
    alternateClassName: ['Epsitec.Notifications'],

    hub: null,
    form: null,

    constructor: function (toastrFunc,formData) {

      this.form = formData._fields.items;
      var context = this;

      $.getScript('signalr/hubs', function () {
            $.connection.hub.logging = true;
            // Start the connection
            var toastrInstance = new toastrFunc();

            $.connection.hub.start(function () { toastrInstance.init(); context.initHub() });

      });
    },

    initHub: function () {
        this.hub = $.connection.notificationHub;
        this.hub.server.logIn(this.form[0].lastValue,this.form[1].lastValue,this.hub.connection.id);
    },

});
