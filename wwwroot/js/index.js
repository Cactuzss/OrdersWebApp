const url = "https://localhost:7158/";

const filters = {
    "OrderNumber": "OrderNumber",
    "OrderItemName": "OrderItemName",
    "OrderItemUnit": "OrderItemUnit",
    "ProviderName": "ProviderName",
    "ProviderId": "ProviderId",
}

const pageSize = 10;
let currentPage = 0;
let tableRows = [];
let filterVals = [];
let providerTable = {};

let ordersRequest = () => {

    let startDate = document.getElementById("start-date")
    let endDate = document.getElementById("end-date")

    let filterStr = "";

    for (let i = 0; i < filterVals.length; i++) {
        filterVals[i] = filterVals[i].replace(" ", "_");
        filterVals[i] = filterVals[i].replace(":", "=");
        filterStr += "&" + filterVals[i] ;
    }

    return url + "api/Orders/" + currentPage +
        "?pageSize=" + pageSize +
        "&startDate=" + startDate.value +
        "&endDate=" + endDate.value + filterStr;
}

let filtersRequest = (str) => {
    return url + "api/Filters/" + str;
}

function setDate() {
    let from = document.getElementById("start-date")
    let until = document.getElementById("end-date")

    until.value = getDateStr(new Date(Date.now()))

    let toFrom = new Date(Date.now())
    toFrom.setMonth(toFrom.getMonth() - 1)
    from.value = getDateStr(toFrom)
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
function getFilters(string) {
    let res = [];

    $.get(filtersRequest(string), (data, status) => {
        res = data;
        setFilters(res, string);
    });
    
}
function setFilters(array, string) {
    let filter = document.getElementById(string)
    for (let i = 0; i < array.length; i++) {
        let toPush = new Option(array[i]);
        toPush.value = array[i];
        filter.appendChild(toPush);
    }
}

function getTableItems() {
    $.get(url + "api/Providers", (data, status) => {
        for (let i = 0; i < data.length; i++) {
            
            let xd = data[i].indexOf(":");
            let left = data[i].slice(0, xd);
            let right = data[i].slice(xd + 1, data[i].length)

            providerTable[left] = right;
        }

        $.get(ordersRequest(), (data, status) => {
            tableRows = data;
            fillTable(tableRows);
        });
    })
    
}

function fillTable(array) {
    let table = document.getElementById("OrdersTable");
    while (table.rows.length > 1) {
        table.deleteRow(1);
    }

    for (let i = 0; i < array.length ; i++) {

        let newRow = table.insertRow();

        let number = newRow.insertCell(0);
        number.textContent = tableRows[i].number;

        let date = newRow.insertCell(1);
        date.textContent = tableRows[i].date;

        let providerid = newRow.insertCell(2);
        providerid.textContent = providerTable[tableRows[i].providerid + ""];
        providerid.value = tableRows[i].providerid;

        newRow.addEventListener("click", () => {
            const cells = newRow.querySelectorAll("td");
            window.location.replace(url + "ViewOrder?id=" + cells[0].textContent + "&provider=" + cells[2].value);
        });
    }
}

function newOrder() {
    let id = -1;
    window.location.replace(url + "OrderCE?id=" + id);
}

function applyFilters() {
    filterVals = []
    for (var el in filters) {                       
        let filter = document.getElementById(el);
        for (let i = 0; i < filter.options.length; i++) {
            if (filter.options[i].selected) {
                filterVals.push(el + ":" + filter.options[i].value);
            }
        }
    }

    getTableItems();
}

function prevPage() {
    if (currentPage >= 1) {
        currentPage--;  
    }

    getTableItems();
}

function nextPage() {
    currentPage++;

    getTableItems();
}


// Initialization

for (var key in filters) {
    let filter = document.getElementById(key);

    if (filter == null) {
        console.log("Cannot find the | " + key + " | filter");
        continue;
    }

    getFilters(filters[key]);
}

setDate();
getTableItems();