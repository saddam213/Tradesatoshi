var sum_buyOrders = 0;
var sum_sellOrders = 0;
var tradefee = $('#chart-container').data('tradefee')
var tradePairId = $('#chart-container').data('tradepairid')

$('[id^="table-buyOrders-"]').dataTable({
	"order": [[0, "desc"]],
	"lengthChange": false,
	"processing": true,
	"bServerSide": true,
	"searching": false,
	"scrollCollapse": false,
	"scrollY": "250px",
	"sPaginationType": "simple_numbers",
	"pageLength": 15,
	"sort": false,
	"paging": false,
	"info": false,
	"language": { "emptyTable": "No data avaliable." },
	"sAjaxSource": $('#buyOrders-host').data('action'),
	"sServerMethod": "POST",
	"fnServerParams": function(aoData) {
		aoData.push({ "name": "tradePairId", "value": $('#buyOrders-host').data('tradepair') });
		aoData.push({ "name": "tradeType", "value": $('#buyOrders-host').data('tradetype') });
	},
	"columnDefs": [
		{
			"targets": 2,
			"render": function(data, type, full, meta) {
				var total = (full[0] * full[1]);
				sum_buyOrders += total;
				return total.toFixed(8);
			}
		},
		{
			"targets": 3,
			"visible": false,
			"render": function(data, type, full, meta) {
				return sum_buyOrders.toFixed(8);
			}
		}
	],
	"footerCallback": function(row, data, start, end, display) {
		sum_buyOrders = 0;
	},
	"rowCallback": function(row, data, index) {
		$(row).on('click', function() {
			$('#amount-Sell, #amount-Buy').val(data[1]);
			$('#rate-Sell, #rate-Buy').val(data[0]).trigger('change');
		});
	},
	"fnDrawCallback": function(oSettings) {

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
	"pageLength": 15,
	"sort": false,
	"paging": false,
	"info": false,
	"language": { "emptyTable": "No data avaliable." },
	"sAjaxSource": $('#sellOrders-host').data('action'),
	"sServerMethod": "POST",
	"fnServerParams": function(aoData) {
		aoData.push({ "name": "tradePairId", "value": $('#sellOrders-host').data('tradepair') });
		aoData.push({ "name": "tradeType", "value": $('#sellOrders-host').data('tradetype') });
	},
	"columnDefs": [
		{
			"targets": 2,
			"render": function(data, type, full, meta) {
				var total = (full[0] * full[1]);
				sum_sellOrders += total;
				return total.toFixed(8);
			}
		},
		{
			"targets": 3,
			"visible": false,
			"render": function(data, type, full, meta) {
				return sum_sellOrders.toFixed(8);
			}
		}
	],
	"footerCallback": function(row, data, start, end, display) {
		sum_sellOrders = 0;
	},
	"rowCallback": function(row, data, index) {
		$(row).on('click', function() {
			$('#amount-Sell, #amount-Buy').val(data[1]);
			$('#rate-Sell, #rate-Buy').val(data[0]).trigger('change');
		});
	},
	"fnDrawCallback": function(oSettings) {

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
	"fnServerParams": function(aoData) {
		aoData.push({ "name": "tradePairId", "value": $('#tradehistory-host').data('tradepair') });
	},
	"columnDefs": [
		{
			"targets": 0,
			"render": function(data, type, full, meta) {
				return moment(new Date(data)).format('h:mm:ss a');
			}
		}
	],
	"rowCallback": function(row, data, index) {
		var textClass = data[1] === 'Buy' ? 'text-success' : 'text-danger';
		$(row).addClass(textClass);
	}
});

$('#table-canceltradepair').on('click', function() {
	var action = $(this).data('action');
	var tradepair = $(this).data('tradepair');
	cancelTradePairOrders(tradepair, action);
});


function cancelOrder(tradeId, action) {
	confirmModal("Cancel Order", "Are you sure you want to cancel order #" + tradeId + "?", function() {
		postJson(action, { id: tradeId, cancelType: 'Trade' });
	});
}

function cancelTradePairOrders(tradePair, action) {
	var count = $('[id^="table-useropenorders-"] > tbody > tr > td').length;
	if (count > 1) {
		confirmModal("Cancel Orders", "Are you sure you want to cancel ALL orders for this market?", function() {
			postJson(action, { id: tradePair, cancelType: 'TradePair' });
		});
	}
}


var orderTemplate = $('#orderTemplate').html();
var historyTemplate = $('#tradeHistoryTemplate').html();

$(document).off("OnTradeHistoryUpdate").on("OnTradeHistoryUpdate", function (event, notification) {
	if (tradePairId !== notification.TradePairId)
		return;

	var table = '#table-tradehistory-' + tradePairId;
	prependTradeHistory(table, notification);
});

$(document).off("OnOrderBookUpdate").on("OnOrderBookUpdate", function (event, notification) {
	if (tradePairId !== notification.TradePairId)
		return;

	var type = notification.Type;
	var table = type === "Buy"
		? "#table-buyOrders-" + tradePairId
		: "#table-sellOrders-" + tradePairId;

	var existingRow = $(table + " > tbody td").filter(function (index) {
		return +$(this).text() == notification.Price;
	}).closest("tr");

	if (notification.Action === "Fill" || notification.Action === "Partial" || notification.Action === "Cancel") {
		mergeOrder(existingRow, notification);
	}
	else if (notification.Action === "New") {

		// If no rows exist, create one
		var firstPrice = $(table + " > tbody tr:first > td:nth-child(1)").text() || "No data avaliable."
		if (firstPrice === "No data avaliable.") {
			$(table + " > tbody tr:first").remove();
			appendOrder(table, notification);
			return;
		}
		
		var lastPrice = $(table + " > tbody tr:last > td:nth-child(1)").text()
		var existingRowPrice = existingRow.find("td:nth-child(1)").text()
		if (existingRow && existingRowPrice == notification.Price) {
			// update existing
			mergeOrder(existingRow, notification);
		}
		else if ((notification.Type === "Sell" && firstPrice > notification.Price) || (notification.Type === "Buy" && notification.Price > firstPrice)) {
			// less than first
			prependOrder(table, notification);
		}
		else if ((notification.Type === "Sell" && notification.Price > +lastPrice) || (notification.Type === "Buy" && notification.Price < +lastPrice)) {
			// more than last
			appendOrder(table, notification);
		}
		else {
			// somewhere in middle
			insertOrder(table, notification)
		}
	}
});

function appendOrder(table, notification) {
	$(table + ' > tbody').append(Mustache.render(orderTemplate, {
		highlight: "buyhighlight",
		price: notification.Price.toFixed(8),
		amount: notification.Amount.toFixed(8),
		total: (notification.Amount * notification.Price).toFixed(8)
	}));
	$(table + " > tbody tr:last").on('click', function () {
		calculateOrder(notification.Amount, notification.Price)
	});
}

function prependOrder(table, notification) {
	var row = $(table + ' > tbody').prepend(Mustache.render(orderTemplate, {
		highlight: "buyhighlight",
		price: notification.Price.toFixed(8),
		amount: notification.Amount.toFixed(8),
		total: (notification.Amount * notification.Price).toFixed(8)
	}));
	$(table + " > tbody tr:first").on('click', function () {
		calculateOrder(notification.Amount, notification.Price)
	});
}

function insertOrder(table, notification) {
	var row = $(table + " > tbody td:nth-child(1)").filter(function () {
		return notification.Type === "Buy"
		? +$(this).text() < notification.Price
		: +$(this).text() > notification.Price;
	}).first().closest("tr");

	var html = Mustache.render(orderTemplate, {
		highlight: "buyhighlight",
		price: notification.Price.toFixed(8),
		amount: notification.Amount.toFixed(8),
		total: (notification.Amount * notification.Price).toFixed(8)
	});
	row.before(html);
	$(row.prev()).on('click', function () {
		calculateOrder(notification.Amount, notification.Price)
	});
}

function mergeOrder(existingRow, notification) {
	var amountColumn = existingRow.find("td:nth-child(2)");
	var totalColumn = existingRow.find("td:nth-child(3)");
	var existingAmount = +amountColumn.text();

	var newAmount = notification.Action == "Cancel" || notification.Action == "Fill" || notification.Action == "Partial"
		? (existingAmount - notification.Amount).toFixed(8)
		: (existingAmount + notification.Amount).toFixed(8);

	var newTotal = (newAmount * notification.Price).toFixed(8)
	if (newAmount <= 0) {
		existingRow.remove();
	}
	else {
		amountColumn.text(newAmount);
		totalColumn.text(newTotal);
		if (notification.Action !== "Fill" && notification.Action !== "Partial") {
			if (existingRow.hasClass("buyhighlight") || existingRow.hasClass("sellhighlight")) {
				existingRow.removeClass("buyhighlight sellhighlight").addClass(notification.Action == "Cancel" ? "sellhighlight2" : "buyhighlight2");
			} else {
				existingRow.removeClass("buyhighlight2 sellhighlight2").addClass(notification.Action == "Cancel" ? "sellhighlight" : "buyhighlight")
			}
		}

		$(existingRow).off().on('click', function () {
			calculateOrder(newAmount, notification.Price)
		});
	}
}

function prependTradeHistory(table, notification) {
	$(table + ' > tbody').prepend(Mustache.render(historyTemplate, {
		time: moment(new Date(notification.Timestamp)).format('h:mm:ss a'),
		type: notification.Type === 'Buy' ? 'text-success' : 'text-danger',
		highlight: notification.Type === 'Buy' ? 'buyhighlight' : 'sellhighlight',
		typeText: notification.Type,
		price: notification.Price.toFixed(8),
		amount: notification.Amount.toFixed(8)
	}));
}

function calculateOrder(amount, price) {
	$('#amount-Sell, #amount-Buy').val((+amount).toFixed(8));
	$('#rate-Sell, #rate-Buy').val((+price).toFixed(8)).trigger('change');
}

$(".data-balance-sell").on("click", function () {
	var rate = $('#rate-Sell').val();
	var balance = $(this).html();
	if (balance && rate) {
		calculateOrder(balance, rate)
	}
});

$(".data-balance-buy").on("click", function () {
	var fee = +tradefee
	var rate = +$('#rate-Buy').val();
	var balance = +$(this).html();
	if (balance && rate) {
		var rateWithFee = rate + (rate / 100.0 * fee);
		var amount = balance / rateWithFee;
		if ((amount * rateWithFee) > balance) {
			amount = amount - 0.00000001;
		}
		calculateOrder(truncateDecimal(amount), rate)
	}
});

