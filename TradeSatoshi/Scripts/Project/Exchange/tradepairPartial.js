$(function () {

	var sum_buy = 0;
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
				sum_buy += total;
				return total.toFixed(8);
			}
		},
		{
			"targets": 3,
			"visible": false,
			"render": function (data, type, full, meta) {
				return sum_buy.toFixed(8);
			}
		}],
		"footerCallback": function (row, data, start, end, display) {
			sum_buy = 0;
		},
		"rowCallback": function (row, data, index) {
			$(row).on('click', function () {
				$('#amount-Sell, #amount-Buy').val(data[1]);
				$('#rate-Sell, #rate-Buy').val(data[0]).trigger('change');
			});
		}
	});

	var sum_sell = 0;
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
				sum_sell += total;
				return total.toFixed(8);
			}
		},
		{
			"targets": 3,
			"visible":false,
			"render": function (data, type, full, meta) {
				return sum_sell.toFixed(8);
			}
		}],
		"footerCallback": function (row, data, start, end, display) {
			sum_sell = 0;
		},
		"rowCallback": function (row, data, index) {
			$(row).on('click', function () {
				$('#amount-Sell, #amount-Buy').val(data[1]);
				$('#rate-Sell, #rate-Buy').val(data[0]).trigger('change');
			});
		}
	});

	
	$('[id^="table-tradehistory-"]').dataTable({
		"order": [[0, "desc"]],
		"lengthChange": false,
		"processing": false,
		"bServerSide": true,
		"searching": false,
		"scrollCollapse": false,
		"scrollY": "504px",
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

	$('[id^="table-userhistory-"]').dataTable({
		"order": [[0, "desc"]],
		"lengthChange": false,
		"processing": true,
		"bServerSide": false,
		"searching": false,
		"scrollCollapse": false,
		"scrollY": '300px',
		"sort": false,
		"paging": false,
		"info": false,
		"language": { "emptyTable": "No data avaliable." },
		//"sAjaxSource": $('#userhistory-host').data('action'),
		"sServerMethod": "POST",
		"fnServerParams": function (aoData) {
			aoData.push({ "name": "tradePairId", "value": $('#userhistory-host').data('tradepair') });
		},
		"columnDefs": [{
			"targets": -1,
			"visible": false
		}]
	});
	
	$('[id^="table-useropenorders-"]').dataTable({
		"order": [[0, "desc"]],
		"lengthChange": false,
		"processing": true,
		"bServerSide": false,
		"searching": false,
		"scrollCollapse": false,
		"scrollY": '300px',
		"sort": false,
		"paging": false,
		"info": false,
		"language": { "emptyTable": "No data avaliable." },
		//"sAjaxSource": $('#useropenorders-host').data('action'),
		"sServerMethod": "POST",
		"fnServerParams": function (aoData) {
			aoData.push({ "name": "tradePairId", "value": $('#useropenorders-host').data('tradepair') });
		},
		//"columnDefs": [{
		//	"targets": 6,
		//	"render": function (data, type, full, meta) {
		//		var cancelAction = $('#useropenorders-host').data('cancel')
		//		return '<button class="btn btn-primary btn-xs" onclick="cancelOrder(' + full[0] + ',\'' + cancelAction + '\')" >Cancel</button>'
		//	}
		//}]
	});

	$('#table-canceltradepair').on('click', function () {
		var action = $(this).data('action');
		var tradepair = $(this).data('tradepair');
		cancelTradePairOrders(tradepair, action);
	});
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
