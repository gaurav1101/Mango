//let table = new DataTable('#tblData');

var dataTable;
$(document).ready(function () {
    
    // Initialize DataTable
   loaddata();
});
    // Make AJAX call to fetch data
    //$.ajax({
    //    url: '/Order/getOrders/', // Replace with your controller action URL
    //   // method: 'GET',
    //    //dataSrc: 'data',
    //    columns: [
    //        { data: 'userId', "width": "5%" },
    //        { data: 'email', "width": "25%" }
    //    ]
   
    //});
    //});

function loaddata()
{
    dataTable = $('#tblData').DataTable({
        "ajax": {
            url: "/Order/getOrders/"},
        "datasrc":"data",
        "columns": [
            { "data": "orederHeaderId", "width": "5%" }, // Corrected property name
            { "data": "email", "width": "15%" },
            { "data": "name", "width": "15%" },
            { "data": "phone", "width": "15%" },
            { "data": "orderTime", "width": "15%" },
            { "data": "status", "width": "15%" },

            {
                "data": 'orederHeaderId',
                "render": function (data) {
                   return `<div class="w-75 btn-group" role="group">
                        <a href="/Order/OrderDetails?orderId=${data}" class="btn btn-primary mx-2"><i class="bi bi-pencil-square"></i></a>
                    </div>`
                },
                "width":"10%"
            }
            // Add more columns as needed
        ]
    });
    //dataTable = $('#tblData').DataTable({
    //    "ajax": { url: "/Order/getOrders/" },
    //    "columns": [
    //        { data: 'orederHeaderId', "width": "5%" },
    //        { data: 'email', "width": "25%" }

    //    ]
    //})
}



