const url = "https://localhost:7158/";

class Order {
    constructor() {
        this.id = null;
        this.number = null;
        this.date = new Date();
        this.providerId = null;
        this.providerName = null;
        this.orderItems = new Array();
    }
}

class OrderItem {
    constructor() {
        this.name = null;
        this.quantity = null;
        this.unit = null;
    }
}

let method = "POST";

let table = document.getElementById("items");
let orderNumber = document.getElementById("orderId");
let dateField = document.getElementById("start-date");
let providerField = document.getElementById("provider");

let providers = [];

let tableRows = [];

class TableRow {
    constructor(name, quantity, unit, id) {
        this.name = name;
        this.quantity = quantity;
        this.unit = unit;
        this.id = id;
    }

    pushToRows() {
        this.row = items.insertRow();

        this.index = tableRows.length;
        this.tableIndex = table.rows.length;

        let createCell = (ind, baseValue) => {
            let cell = this.row.insertCell(ind);
            let res = document.createElement("textarea");
            res.style = 'resize: none';
            res.value = baseValue;
            cell.appendChild(res);
            return res;
        }

        this.nameField = createCell(0, this.name);
        this.quantityField = createCell(1, this.quantity);
        this.unitField = createCell(2, this.unit);

        this.nameField.addEventListener('input', () => {
            this.name = this.nameField.value;
        });

        this.quantityField.addEventListener('input', () => {
            this.quantity = this.quantityField.value;
        });

        this.unitField.addEventListener('input', () => {
            this.unit = this.unitField.value;
        });

        let cellInstruction = this.row.insertCell(-1);
        let removeButton = document.createElement("button");
        removeButton.textContent = "-";


        removeButton.onclick = () => {
            removeFromRows(this.tableIndex);
        };

        cellInstruction.appendChild(removeButton);

        tableRows.push(this);
    }

}

let order = new Order();

function removeFromRows(tableIndex) {

    let ind = null;
    for (let i = 0; i < tableRows.length; i++) {
        if (tableRows[i].tableIndex == tableIndex) {
            ind = tableRows[i].index;
            break;
        }
    }

    if (ind == null) {
        return;
    }

    for (let i = ind; i < tableRows.length; i++) {
        tableRows[i].tableIndex--;
        tableRows[i].index--;
    }

    table.deleteRow(tableIndex - 1);
    tableRows.splice(ind, 1);

}
function init() {
    order.id = document.getElementById("orderId").innerHTML.trim();

    $.get(url + "api/Orders/id/" + order.id,
        (data, status) => {

            order.id = data.id;
            order.number = data.number;
            order.providerId = data.providerid;
            order.date = data.date;

            document.getElementById("start-date").value = parseDateFromDB(order.date);
            document.getElementById("orderId").innerHTML = order.number;

            $.get(url + "api/Providers/name/" + order.providerId, (data, status) => {
                order.providerName = data;
                
                $.get(url + "api/Items/" + order.id, (data, result) => {
                    
                    order.orderItems = data;

                    for (let i = 0; i < order.orderItems.length; i++) {
                        (new TableRow(order.orderItems[i].name, order.orderItems[i].quantity, order.orderItems[i].unit, order.orderItems[i].id)).pushToRows();
                    }

                    getProviders();
                });
            })
        });
}
function parseDateFromDB(date) {
    return date[6] + date[7] + date[8] + date[9] + "-" + date[3] + date[4] + "-" + date[0] + date[1];
}
function onSave() {
    for (let i = 0; i < tableRows.length; i++) {
        if (/.*[a-zA-Z].*/.test(tableRows[i].quantity)) {
            alert("Количество продукции должно быть числом");
            return;
        }
        if (tableRows[i].name.length == 0 || tableRows[i].unit.length == 0 || tableRows[i].quantity.length == 0) {
            alert("Заполните все пустые поля в таблице или удалите строку!");
            return;
        }
    }

    let objcpy = order;

    let objs = [];
    for (let i = 0; i < tableRows.length; i++) {
        let obj = {};

        if (tableRows[i].id != null || tableRows[i].id != undefined) {
            obj.id = tableRows[i].id;
        } else {
            obj.id = -1;
        }
        
        obj.orderId = order.id;
        
        obj.name = encodeURIComponent(tableRows[i].name.replace(" ", "_"));
        obj.quantity = encodeURIComponent(tableRows[i].quantity);
        obj.unit = encodeURIComponent(tableRows[i].unit.replace(" ", "_"));

        objs.push(obj);
    }

    objcpy.orderItems = objs;
    objcpy.providerName = providerField.value;

    try {
        objcpy.date = objcpy.date.replace(" ", "_");
    }
    catch {
        try {
            objcpy.date = getDateStr(objcpy.date).replace(" ", "_");
        }
        catch {
            alert("Проверьте дату")
        }
    }

    objcpy.number = encodeURIComponent(objcpy.number);
    objcpy.providerName = encodeURIComponent(objcpy.providerName.replace(" ", "_"));
    

    console.log(JSON.stringify(objcpy))
    

    $.ajax({
        url: url + "api/Orders/" + (method == "PUT" ? order.id : ""),
        type: method,
        data: JSON.stringify(objcpy),  
        contentType: "application/json",  
        success: function (response) {
            alert("Успех!")
        },
        error: function (xhr, status, error) {
            alert("Проверьте номер заказа и его составляющие!\nВсе названия составляющих должны отличаться от номера заказа!\nПроверьте чтобы у поставщика не было заказов с таким же номером!");
            console.log(error);
            console.log(status);
        }
    });
}
function getDateStr(date) {
    let datestr = date.getFullYear() + "";
    let month = date.getMonth() + 1 + "";
    let day = date.getDate() + "";

    if (month.length < 2) {
        datestr += "-0" + month;
    } else {
        datestr += month;
    }

    if (day.length < 2) {
        return datestr + "-0" + day;
    }
    return datestr + "-" + day;
}
function addItem() {
    (new TableRow("", "", "")).pushToRows();
}
function getProviders() {
    $.get(url + "api/Filters/" + "ProviderName", (data, status) => {
        array = data;
        let filter = document.getElementById("provider")
        for (let i = 0; i < array.length; i++) {
            let toPush = new Option(array[i]);
            toPush.value = array[i];
            filter.appendChild(toPush);
        }
        console.log(order.providerName);
        filter.value = order.providerName;
    });
}


if (orderNumber.innerHTML.trim() != "-1") {
    init();
    method = "PUT";
}
else {
    getProviders();
    orderNumber.value = -1;
    order.id = "-1";
    order.providerId = "-1";
    dateField.value = getDateStr(new Date(Date.now()));
}

orderNumber.addEventListener('input', () => {
    order.number = orderNumber.value;
})
dateField.addEventListener('input', () => {
    order.date = dateField.value;
})
