onboardapp.controller('Customers', function ($scope, $log, httpClient, Validator) {
    // Initiate customer fields
    $scope.id = "";
    $scope.name = "";
    $scope.address1 = "";
    $scope.address2 = "";
    $scope.city = "";
    $scope.sortType = { type: "", reverse: null };
    $scope.errorMessage = "";
    $scope.validationsForNullOrEmpty = [];
    $scope.validationsForAlphaNumeric = [];
    // List of Customers
    $scope.customers = [];

    // Get list of customers
    httpClient.get('/Customer/Get',null,function(data){
        $scope.customers = data;
    });
    $scope.config = {
        headers: { 'Content-Type': 'application/json' }
    };
    // Add a new Customer
    $scope.addNewCustomer = function () {
        if (!$scope.checkValidation($scope.name, $scope.address1, $scope.address2, $scope.city)) {
            $log.log("Data input is invalid");
            return;
        }

        var customer = {
            Name:$scope.name,
            Address1:$scope.address1,
            Address2:$scope.address2,
            City : $scope.city
        };
        var url = '/Customer/AddCustomer';
        
        httpClient.post(url, customer, $scope.config, function (id) {
            if (typeof id === "string") {
                $log.log(id);
                $scope.errorMessage = id;
                $scope.reset();
                $("#ErrorModal").modal('toggle');
                return;
            }
            //push a new customer to customers list
            $scope.customers.push({
                Id: id,
                Name: $scope.name,
                Address1: $scope.address1,
                Address2: $scope.address2,
                City: $scope.city
            });
            // dismiss/hide the popup modal
            $('#PopUpModal').modal('hide');
            // Reset
            $scope.reset();
        });
    };

    $scope.checkValidation = function () {
        //debugger;
        $scope.validations = [];
        var res = true;
        for (var i = 0; i < arguments.length; i++) {
            var arg = arguments[i];
            if (arg === undefined) arg = "";
            // Checking null or empty space
            if (Validator.isNullOrEmptySpace(arg)) {
                if (i == 2) { // i==2 is address2 which is allow null/empty
                    $scope.validations.push({ isValid: true, message: '' });
                }
                else {
                    $scope.validations.push({ isValid: false, message: 'Required' });
                    res = false;
                }
            } else {
                if (Validator.isAlphaNumeric(arg)) {
                    $scope.validations.push({ isValid: true, message: '' });
                } else {
                    $scope.validations.push({ isValid: false, message: 'Only Number or Alphabetic inlcuding (,/./_/-) Accepted' });
                    res = false;
                }
                
            }
        }
        return res;
    };

    $scope.sortTable = function (column) {
        $scope.sortType.type = column;
        $scope.sortType.reverse = $scope.sortType.reverse ? false : true;
    }

    // Get a customer
    $scope.selectCustomer = function (customer) {
        $scope.id = customer.Id;
        $scope.name = customer.Name;
        $scope.address1 = customer.Address1;
        $scope.address2 = customer.Address2;
        $scope.city = customer.City;
    };

    // Reset $scope fields
    $scope.reset = function () {
        $scope.id = "";
        $scope.name = "";
        $scope.address1 = "";
        $scope.address2 = "";
        $scope.city = "";
        $scope.validations = [];
    };

    // Save customer after edited
    $scope.saveEditedCustomer = function () {
        if (!$scope.checkValidation($scope.name, $scope.address1, $scope.address2, $scope.city)) {
            $log.log("Data input is invalid");
            return;
        }

        var url = '/Customer/EditCustomer';
        var customer = {
            Id: $scope.id,
            Name: $scope.name,
            Address1: $scope.address1,
            Address2: $scope.address2,
            City: $scope.city
        };
      
        httpClient.post(url, customer, $scope.config, function (id) {
            if (typeof id === "string") {
                $log.log(id);
                $scope.errorMessage = id;
                $scope.reset();
                $("#ErrorModal").modal('toggle');
                return;
            }
            // Find index of a customer in customers array who id match with id
            var index = $scope.customers.map(function (x) { return x.Id; }).indexOf(id);
            // Update customers list
            if (index >= 0) {
                $scope.customers[index].Id = $scope.id;
                $scope.customers[index].Name = $scope.name;
                $scope.customers[index].Address1 = $scope.address1;
                $scope.customers[index].Address2 = $scope.address2;
                $scope.customers[index].City = $scope.city;
                // dismiss/hide the popup modal
                $('#PopUpModal').modal('hide');
                // Reset
                $scope.reset();
            }
        });
    };

    // Delete Customer
    $scope.deleteCustomer = function (customer) {
        //var url = '/Customer/DeleteCustomer?id=' + customer.Id;
        var url = '/Customer/DeleteCustomer';
        var config = {
            headers: { 'Content-Type': 'application/json' }
        };
        httpClient.post(url, { id: customer.Id }, $scope.config, function (id) {
            if (typeof id === "string") {
                $log.log(id);
                $scope.errorMessage = id;
                $("#ErrorModal").modal('toggle');
                return;
            }
            // Find index of a customer in customers array who id match with id
            var index = $scope.customers.map(function (x) { return x.Id; }).indexOf(id);
            if (index >= 0) {
                // Delete a specific customer in customer list by index
                $scope.customers.splice(index, 1);
                // Reset
                $scope.reset();
            }
        });
    };
});

onboardapp.controller('Orders', function ($scope, $log, $filter, httpClient, Validator) {
    $scope.selectedCustomer = $scope.selectedProduct = null;
    $scope.validationsForNullOrEmpty = [];
    $scope.orderId = "";
    $scope.date = "";
    $scope.orders = [];
    $scope.sortType = { type: "", reverse: null };
    $scope.config = {
        headers: { 'Content-Type': 'application/json' }
    };
    // Get list of Orders   
    httpClient.get('/Order/Get',null, function (data) {
        $scope.orders = data;
        //debugger;
    });

    $scope.customers = [];
    // Get list of customers   
    httpClient.get('/Customer/Get',null, function (data) {
        $scope.customers = data;
        //debugger;
    });

    // List of Products
    $scope.products = [];
    // Get list of customers   
    httpClient.get('/Product/Get',null, function (data) {
        $scope.products = data;
        //debugger;
    });
    // Add and save new order then push the new order to order list
    $scope.addNewOrder = function () {
        //debugger;
        if (!$scope.checkValidation($scope.date, $scope.selectedCustomer, $scope.selectedProduct)) {
            $log.log("Data input is invalid");
            return;
        }

        var url = '/Order/AddNewOrder?customerId'

        httpClient.post(
            url,
            {
                customerId: $scope.selectedCustomer.Id,
                date: $scope.date,
                productId: $scope.selectedProduct.Id
            },
            $scope.config,
            function (id) {
                $scope.orders.push({
                    OrderId: id,
                    Date: $filter('date')($scope.date, 'MM/dd/yyyy'),
                    CustomerId: $scope.selectedCustomer.Id,
                    Name: $scope.selectedCustomer.Name,
                    ProductId: $scope.selectedProduct.Id,
                    ProductName: $scope.selectedProduct.ProductName,
                    Price: $scope.selectedProduct.Price
                });
                // dismiss/hide the popup modal
                $('#PopUpModalOrders').modal('hide');
                //Reset
                $scope.reset();
            });
    };

    $scope.saveEditedOrder = function () {
        if (!$scope.checkValidation($scope.date, $scope.selectedCustomer, $scope.selectedProduct)) {
            $log.log("Data input is invalid");
            return;
        }

        var url = '/Order/SaveEditedOrder';

        httpClient.post(url,
            {
                orderId: $scope.orderId,
                date: $scope.date,
                customerId: $scope.selectedCustomer.Id,
                productId: $scope.selectedProduct.Id
            },
            $scope.config, function (id) {
            // Find index of an order in orders array which order id match with id
            //debugger;
            var index = $scope.orders.map(function (x) { return x.OrderId; }).indexOf(id);
            //debugger;
            if (index >= 0) {
                $log.log($filter('date')($scope.date, 'MM/dd/yyyy'));
                $scope.orders[index].Date = $filter('date')($scope.date, 'MM/dd/yyyy');
                $scope.orders[index].Name = $scope.selectedCustomer.Name;
                $scope.orders[index].ProductId = $scope.selectedProduct.Id;
                $scope.orders[index].ProductName = $scope.selectedProduct.ProductName;
                $scope.orders[index].Price = $scope.selectedProduct.Price;
                // dismiss/hide the popup modal
                $('#PopUpModalOrders').modal('hide');
                //Reset
                $scope.reset();
            }
        });
    };

    $scope.deleteOrder = function (order) {
        var url = '/Order/DeleteOrder';
        httpClient.post(url,
            {
                orderId: order.OrderId
            }, $scope.config, function (id) {
            // Find index of an order in orders array which order id match with id
            //debugger;
            var index = $scope.orders.map(function (x) { return x.OrderId; }).indexOf(id);
            if (index >= 0) {
                $scope.orders.splice(index, 1);
                // reset
                $scope.reset();
            }
        });
    };

    // Checking validation
    $scope.checkValidation = function () {
        $scope.validations = [];
        var res = true;
        debugger;
        for (var i = 0; i < arguments.length; i++) {
            var arg = arguments[i];
            if (arg === undefined) arg = "";
            // Checking null or empty space
            if (Validator.isNullOrEmptySpace(arg)) {
                $scope.validations.push(false);
                res = false;
            } else {
                $scope.validations.push(true);
            }
        }
        return res;
    };

    // Sort table column
    $scope.sortTable = function (column) {
        $scope.sortType.type = column;
        $scope.sortType.reverse = $scope.sortType.reverse ? false : true;
    }

    // Reset
    $scope.reset = function () {
        $scope.selectedCustomer = $scope.selectedProduct = null;
        $scope.date = new Date();
        $scope.orderId = "";
        $scope.validations = [];
    };

    // Select an order from list
    $scope.selectOrder = function (order) {
        $scope.orderId = order.OrderId;
        var subStr = order.Date.split("/");
        $scope.date = new Date(parseInt(subStr[2]), parseInt(subStr[0])-1, parseInt(subStr[1]));

        $scope.selectedCustomer = {
            Id: order.CustomerId,
            Name: order.Name,
        };

        $scope.selectedProduct = {
            Id: order.ProductId,
            ProductName: order.ProductName,
            Price: order.Price
        };
    };

    // Customers <select>
    $scope.selectCustomerUpdate = function () {
        //debugger;
        $scope.selectedCustomer = JSON.parse($scope.selectedCustomer);
        $log.log($scope.selectedCustomer.Id + "-" + $scope.selectedCustomer.Name);
    };

    $scope.isCustomerSelected = function (selection) {
        //debugger;
        if ($scope.selectedCustomer == null) return false;
        return selection.Id === $scope.selectedCustomer.Id;
    };

    // Products <select>
    $scope.selectProductUpdate = function () {
        $scope.selectedProduct = JSON.parse($scope.selectedProduct);
        $log.log($scope.selectedProduct.Price);
        //debugger;
    };

    $scope.isProductSelected = function (selection) {
        //debugger;
        if ($scope.selectedProduct == null) return false;
        return selection.Id === $scope.selectedProduct.Id;
    };

    // Display modal
    $scope.displayModal = function () {
        $scope.date = new Date()
        $log.log($scope.date);
    };
})