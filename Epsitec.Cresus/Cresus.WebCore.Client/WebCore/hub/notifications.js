///AIDER NotificationHub


var signalRClient = {};

$(function () {
   
    function Toastr() {

        this.hub = $.connection.notificationHub;

        //Initialize
        this.init = function () {
            signalRClient.connectionId = this.hub.connection.id;
        }

        //Test Hub
        this.WarningToastTo = function (connectionId,title,message,datasetId,entityId)
        {
            this.hub.server.warningToast(connectionId,title,message,datasetId,entityId);

        }

        //Handlers for our Hub callbacks
        this.hub.client.StickyWarningNavToast = function (title,msg,datasetId,entityId) {

            //ex. {"id":"[AVA]-1000000054","name":"[LVAHD]","level":"2"}
            var path = {};
            path.id = entityId;
            path.name = datasetId;

            toastr.options = {
                "debug": false,
                "positionClass": "toast-bottom-full-width",
                "onclick": function () {
                    var app = Epsitec.Cresus.Core.getApplication();
                    app.showEditableEntity(path);
                },
                "fadeIn": 300,
                "fadeOut": 1000,
                "timeOut": 0
            };
            toastr.warning(msg, title);
        }

    };

    if (epsitecConfig.featureNotifications)
    {    
        $.getScript('signalr/hubs', function () {

            $.connection.hub.logging = false;

            signalRClient.instance = new Toastr();
            // Start the connection
            $.connection.hub.start(function () { signalRClient.instance.init(); });
        });
    }
    

});