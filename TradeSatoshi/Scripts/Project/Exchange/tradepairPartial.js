var sum_buyOrders = 0;
var sum_sellOrders = 0;





$('[id^="table-buyOrders-"]').dataTable({
	"order": [[0, "desc"]],
	"lengthChange": false,
	"processing": true,
	"bServerSide": true,
	"searching": false,
	"scrollCollapse": false,
	"scrollY": "250px",
	"sPaginationType": "simple_numbers",
	"pageLength": "15",
	"sort": false,
	"paging": false,
	"info": false,
	"language": { "emptyTable": "No data avaliable." },
	"sAjaxSource": $('#buyOrders-host').data('action'),
	"sServerMethod": "POST",
	"fnServerParams": function (aoData) {
		aoData.push({ "name": "tradePairId", "value": $('#buyOrders-host').data('tradepair') });
		aoData.push({ "name": "tradeType", "value": $('#buyOrders-host').data('tradetype') });
	},
	"columnDefs": [{
		"targets": 2,
		"render": function (data, type, full, meta) {
			var total = (full[0] * full[1]);
			sum_buyOrders += total;
			return total.toFixed(8);
		}
	},
	{
		"targets": 3,
		"visible": false,
		"render": function (data, type, full, meta) {
			return sum_buyOrders.toFixed(8);
		}
	}],
	"footerCallback": function (row, data, start, end, display) {
		sum_buyOrders = 0;
	},
	"rowCallback": function (row, data, index) {
		$(row).on('click', function () {
			$('#amount-Sell, #amount-Buy').val(data[1]);
			$('#rate-Sell, #rate-Buy').val(data[0]).trigger('change');
		});
	},
	"fnDrawCallback": function (oSettings) {
	
	}
});

$('[id^="table-sellOrders-"]').dataTable({
	"order": [[0, "desc"]],
	"lengthChange": false,
	"processing": true,
	"bServerSide": true,
	"searching": false,
	"scrollCollapse": false,
	"scrollY": "250px",
	"sPaginationType": "simple_numbers",
	"pageLength": "15",
	"sort": false,
	"paging": false,
	"info": false,
	"language": { "emptyTable": "No data avaliable." },
	"sAjaxSource": $('#sellOrders-host').data('action'),
	"sServerMethod": "POST",
	"fnServerParams": function (aoData) {
		aoData.push({ "name": "tradePairId", "value": $('#sellOrders-host').data('tradepair') });
		aoData.push({ "name": "tradeType", "value": $('#sellOrders-host').data('tradetype') });
	},
	"columnDefs": [{
		"targets": 2,
		"render": function (data, type, full, meta) {
			var total = (full[0] * full[1]);
			sum_sellOrders += total;
			return total.toFixed(8);
		}
	},
	{
		"targets": 3,
		"visible": false,
		"render": function (data, type, full, meta) {
			return sum_sellOrders.toFixed(8);
		}
	}],
	"footerCallback": function (row, data, start, end, display) {
		sum_sellOrders = 0;
	},
	"rowCallback": function (row, data, index) {
		$(row).on('click', function () {
			$('#amount-Sell, #amount-Buy').val(data[1]);
			$('#rate-Sell, #rate-Buy').val(data[0]).trigger('change');
		});
	},
	"fnDrawCallback": function (oSettings) {
	
	}

});


$('[id^="table-tradehistory-"]').dataTable({
	"order": [[0, "desc"]],
	"lengthChange": false,
	"processing": false,
	"bServerSide": true,
	"searching": false,
	"scrollCollapse": false,
	"scrollY": "472px",
	"sort": false,
	"paging": false,
	"info": false,
	"language": { "emptyTable": "No data avaliable." },
	"sAjaxSource": $('#tradehistory-host').data('action'),
	"sServerMethod": "POST",
	"fnServerParams": function (aoData) {
		aoData.push({ "name": "tradePairId", "value": $('#tradehistory-host').data('tradepair') });
	},
	"columnDefs": [{
		"targets": 0,
		"render": function (data, type, full, meta) {
			return moment(new Date(data)).format('h:mm:ss a');
		}
	}],
	"rowCallback": function (row, data, index) {
		var textClass = data[1] === 'Buy' ? 'text-success' : 'text-danger';
		$(row).addClass(textClass);
	}
});

$('#table-canceltradepair').on('click', function () {
	var action = $(this).data('action');
	var tradepair = $(this).data('tradepair');
	cancelTradePairOrders(tradepair, action);
});





function cancelOrder(tradeId, action) {
	confirmModal("Cancel Order", "Are you sure you want to cancel order #" + tradeId + "?", function () {
		postJson(action, { id: tradeId, cancelType: 'Trade' });
	});
}

function cancelTradePairOrders(tradePair, action) {
	var count = $('[id^="table-useropenorders-"] > tbody > tr > td').length;
	if (count > 1) {
		confirmModal("Cancel Orders", "Are you sure you want to cancel ALL orders for this market?", function () {
			postJson(action, { id: tradePair, cancelType: 'TradePair' });
		});
	}
}