function NotificationsToastr() {

        this.hub = $.connection.notificationHub;
        //this.app = Epsitec.Cresus.Core.getApplication();

        //Initialize
        this.init = function() {
        };

        //Entry points for calling hub

        this.WarningToastTo = function(connectionId, title, message, datasetId, entityId) {
            this.hub.server.warningToast(connectionId, title, message, datasetId, entityId);

        };


        //Handlers for our Hub callbacks
        this.hub.client.StickyWarningNavToast = function(title, msg, datasetId, entityId) {
            var path = {};
            path.id = entityId;
            path.name = datasetId;

            toastr.options = {
                'debug': false,
                'positionClass': 'toast-bottom-full-width',
                'onclick': function() {
                    //this.app.showEditableEntity(path);
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
