onboardapp.service('httpClient', function ($http, $log) {
    // Call get method
    this.get = function (url, config, callback) {
        $http.get(url,config).then(function (res) {
            // Success
            //debugger;
            callback(res.data);
        }, function (res) {
            // Fail
            $log.error("Error occurred")
        });
    };

    this.post = function (url, data, config, callback) {
        $http.post(url,data,config).then(function (res) {
            // Success
            //debugger;
            callback(res.data);
        }, function (res) {
            // Fail
            $log.error("Error occurred");
        });
    };
});

onboardapp.service('Validator', function ($log) {
    //debugger;
    this.isNullOrEmptySpace = function (data) {
        var res = (data == null || data == "" || data===undefined) ? true : false;
        return res;
    };

    this.isAlphaNumeric = function (data) {
        var res = data.match("^[a-zA-Z0-9 ,._-]+$");
        if (res != null) {
            return true;
        } else {
            return false;
        }
    };
});