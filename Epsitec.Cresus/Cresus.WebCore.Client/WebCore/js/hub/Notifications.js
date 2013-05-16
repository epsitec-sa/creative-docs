Ext.define('Epsitec.cresus.webcore.hub.Notifications', {
    alternateClassName: ['Epsitec.Notifications'],

    constructor: function (toastr) {
        $.getScript('signalr/hubs', function () {
            $.connection.hub.logging = false;
            // Start the connection
            var toastrInstance = new toastr();
            $.connection.hub.start(function () { toastrInstance.init() });
        });
    }
});
