

var sbhOffline = {
    indexedDb: {},
    DB_NAME: "softBanHang",
    ORDER_TABLE: "order",
    RESERVE_TABLE: "reserve"
};

sbhOffline.indexedDb.db = null;

sbhOffline.indexedDb.open = function () {
    var version = 1;
    var request = indexedDB.open(sbhOffline.DB_NAME, version);
    request.onsuccess = function (e) {
        sbhOffline.indexedDb.db = e.target.result;
        sbhOffline.indexedDb.getAllOrders();
        sbhOffline.indexedDb.getAllReserves();
    };

    request.onerror = sbhOffline.indexedDb.db;

    request.onupgradeneeded = function (e) {
        var db = e.target.result;
        e.target.transaction.onerror = sbhOffline.indexedDb.onerror;

        if (db.objectStoreNames.contains(sbhOffline.ORDER_TABLE)) {
            db.deleteObjectStore(sbhOffline.ORDER_TABLE);
        }

        var store = db.createObjectStore(sbhOffline.ORDER_TABLE, { keyPath: "DeliveryTime" });

        if (db.objectStoreNames.contains(sbhOffline.RESERVE_TABLE)) {
            db.deleteObjectStore(sbhOffline.RESERVE_TABLE);
        }

        store = db.createObjectStore(sbhOffline.RESERVE_TABLE, { keyPath: "DeliveryTime" });
    };

};


sbhOffline.indexedDb.addOrder = function (order) {
    var db = sbhOffline.indexedDb.db;
    var trans = db.transaction([sbhOffline.ORDER_TABLE], "readwrite");
    var store = trans.objectStore(sbhOffline.ORDER_TABLE);
    
    var request = store.put(order);
    trans.oncomplete = function (e) {
        sbhOffline.indexedDb.getAllOrders();

    };

    request.onerror = function (e) {
        console.log(e.value);
    };
};


sbhOffline.indexedDb.addReserve = function (order) {
    var db = sbhOffline.indexedDb.db;
    var trans = db.transaction([sbhOffline.RESERVE_TABLE], "readwrite");
    var store = trans.objectStore(sbhOffline.RESERVE_TABLE);

    var request = store.put(order);
    trans.oncomplete = function (e) {
        sbhOffline.indexedDb.getAllReserves();

    };

    request.onerror = function (e) {
        console.log(e.value);
    };
};


sbhOffline.indexedDb.clearAllOrders = function() {
    var db = sbhOffline.indexedDb.db;
    var trans = db.transaction([sbhOffline.ORDER_TABLE], "readwrite");
    var store = trans.objectStore(sbhOffline.ORDER_TABLE);
    
    var request = store.clear();
    trans.oncomplete = function (e) {
        sbhOffline.indexedDb.getAllOrders();
    };

    request.onerror = function (e) {
        console.log(e.value);
    };
}


sbhOffline.indexedDb.clearAllReserves = function () {
    var db = sbhOffline.indexedDb.db;
    var trans = db.transaction([sbhOffline.RESERVE_TABLE], "readwrite");
    var store = trans.objectStore(sbhOffline.RESERVE_TABLE);

    var request = store.clear();
    trans.oncomplete = function (e) {
        sbhOffline.indexedDb.getAllReserves();
    };

    request.onerror = function (e) {
        console.log(e.value);
    };
}


sbhOffline.indexedDb.getAllOrders = function(cbSuccess) {
    var db = sbhOffline.indexedDb.db;
    var trans = db.transaction([sbhOffline.ORDER_TABLE], "readwrite");
    var store = trans.objectStore(sbhOffline.ORDER_TABLE);

    var keyRange = IDBKeyRange.lowerBound(0);
    var cursorRequest = store.openCursor(keyRange);

    var objOrders = [];
    cursorRequest.onsuccess = function (e) {
        var result = e.target.result;
        if (!!result == false) {
            renderOrderCount(objOrders.length);

            if (typeof cbSuccess != "undefined") {
                cbSuccess(objOrders);
            }

            return;
        }
        
        objOrders.push(result.value);
        //renderOrder(result.value);
        
        result.continue();
    };

    cursorRequest.onerror = sbhOffline.indexedDb.onerror;
};


sbhOffline.indexedDb.getAllReserves = function (cbSuccess) {
    var db = sbhOffline.indexedDb.db;
    var trans = db.transaction([sbhOffline.RESERVE_TABLE], "readwrite");
    var store = trans.objectStore(sbhOffline.RESERVE_TABLE);

    var keyRange = IDBKeyRange.lowerBound(0);
    var cursorRequest = store.openCursor(keyRange);

    var objs = [];
    cursorRequest.onsuccess = function (e) {
        var result = e.target.result;
        if (!!result == false) {
            renderReserveCount(objs.length);

            if (typeof cbSuccess != "undefined") {
                cbSuccess(objs);
            }

            return;
        }

        objs.push(result.value);
        //renderOrder(result.value);

        result.continue();
    };

    cursorRequest.onerror = sbhOffline.indexedDb.onerror;
};


function renderOrderCount(count) {
    $("#lblOfflineOrderCount").text(count);
}


function renderReserveCount(count) {
    $("#lblOfflineReserveCount").text(count);
}

function clearAll() {
    $("input[type=text]", "#frmCustomer").val(null);
    $("textarea", "#frmCustomer").val(null);

    $("#tblOrderDetails tbody").html("");
    $("#lblTotal").html("");
    $("#txtDiscount").val(0.0);
    $("#lblTotalCost").html("");
    $("#dlgReserve").modal("hide");
    
}


function syncOrder() {
    if (navigator.onLine) {
        sbhOffline.indexedDb.getAllOrders(function (orders) {

            $.ajax({
                url: "/Order/CreateMany",
                type: "POST",
                contentType: "application/json",
                data: JSON.stringify(orders),
                success: function (data, stt, jqXHR) {
                    console.log("created " + orders.length + " order");
                    sbhOffline.indexedDb.clearAllOrders();
                },
                error: function (jqXHR, stt, err) {
                    console.log(stt);
                }
            });
        });
    }
    else {
        $("#btnSyncOrders").prop("disabled", true);
        alert("no network connection!");
    }
}


function syncReserve() {
    if (navigator.onLine) {
        sbhOffline.indexedDb.getAllReserves(function (reserves) {
            $.ajax({
                url: "/Reserve/CreateMany",
                type: "POST",
                contentType: "application/json",
                data: JSON.stringify(reserves),
                success: function (data, stt, jqXHR) {
                    console.log("created " + reserves.length + " reserves");
                    sbhOffline.indexedDb.clearAllReserves();
                },
                error: function (jqXHR, stt, err) {
                    console.log(stt);
                }
            });
        });
    }
    else {
        $("#btnSyncReserves").prop("disabled", true);
        alert("no network connection!");
    }
}


function addCustomer(customer){
    $.ajax({
        url: "/Customer",
        type: "POST",
        contentType: "application/json",
        data: JSON.stringify(customer),
        success: function (data, stt, jqXHR) {
            console.log("created customer");
        },
        error: function (jqXHR, stt, err) {
            console.log(stt);
        }
    });
}

function printInvoice(order, numb) {
    //currentdate.getDate()
    //        + "/" 
    //        + (currentdate.getMonth() + 1) 
    //        + "/" 
    //        + currentdate.getFullYear() 
    //        + " " 
    //        + currentdate.getHours()
    //        + ":"
    //        + currentdate.getMinutes(),
    var reportHtml = openPrint($("#tmplInvoicePrint").html(), {
        AccountFullName: strAccountFullName,
        CurrentDate: order.DeliveryTime,
        Order: order,
        SequenceNumber: numb,
        CopyNumber: 1
    });
    return reportHtml;
}


function previewInvoice(order, numb) {
    previewPrint($("#tmplReservePrint").html(), {
        AccountFullName: strAccountFullName,
        Order: order,
        SequenceNumber: numb
    });

}

window.addEventListener("DOMContentLoaded", function () {
    sbhOffline.indexedDb.open();
}, false);

