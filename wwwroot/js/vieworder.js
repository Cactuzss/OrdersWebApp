const url = "https://localhost:7158/";
class Order {
    constructor() {
        this.id = null;
        this.number = null;
        this.date = null;
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


let order = new Order();

function getOrder() {

    order.number = document.getElementById("orderId").innerHTML;
    order.number = order.number.trim();

    order.providerId = document.getElementById("provider").innerHTML;
    order.providerId = order.providerId.trim();

    let today = new Date(Date.now());
    let requestUrl = url + "api/Orders/0?pageSize=1&OrderNumber=" + order.number +
        "&ProviderId=" + order.providerId +
        "&startDate=1000-01-01" +
        "&endDate=" + getDateStr(today);

    $.get(requestUrl,
        (data, status) => {
            order = new Order();

            order.id = data[0].id;
            order.number = data[0].number;
            order.providerId = data[0].providerid;
            order.date = data[0].date;

            document.getElementById("start-date").value = parseDateFromDB(order.date);

            updateProviders(order);
            updateTable(order);
        });

}

function parseDateFromDB(date) {
    return date[6] + date[7] + date[8] + date[9] + "-" + date[3] + date[4] + "-" + date[0] + date[1] ;
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
function updateProviders(order) {
    let select = document.getElementById("provider");

    $.get(url + "api/Providers/name/" + order.providerId, (data, status) => {
        select.innerHTML = data;
        order.providerName = data;
    })
    
}

function onEdit() {
    window.location.replace(url + "OrderCE?id=" + order.id);
}
function updateTable(order) {
    let table = document.getElementById("itemsTable");

    $.get(url + "api/Items/" + order.id, (data, result) => {
        console.log(order.id);
        console.log(data);

        order.orderItems = data;

        for (let i = 0; i < order.orderItems.length; i++) {
            let newRow = table.insertRow();

            let nameCell = newRow.insertCell(0);
            let quantityCell = newRow.insertCell(1);
            let unitCell = newRow.insertCell(2);

            nameCell.textContent = order.orderItems[i].name;
            quantityCell.textContent = order.orderItems[i].quantity;
            unitCell.textContent = order.orderItems[i].unit;
        }
    });
}

function onDelete() {
    const modal = document.getElementById("confirmationModal");
    const openModalButton = document.getElementById("deleteButton");
    const confirmYesButton = document.getElementById("confirmYes");
    const confirmNoButton = document.getElementById("confirmNo");
    openModalButton.onclick = function () {
        modal.style.display = "block";
    }
    
    confirmYesButton.onclick = function () {
        modal.style.display = "none";
        window.location.replace(url);
        $.ajax({
            url: url + "api/Orders/" + order.id,
            type: 'DELETE',
            success: function (result) {
                alert("Success");
            }
        })
    }
    
    confirmNoButton.onclick = function () {
        modal.style.display = "none";
    }
    
    window.onclick = function (event) {
        if (event.target === modal) {
            modal.style.display = "none";
        }
    }
}

// Initialization

getOrder();
onDelete();     