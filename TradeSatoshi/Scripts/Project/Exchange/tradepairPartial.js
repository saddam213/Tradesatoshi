setVisibleChart()
var tradefee = $('#chart-container').data('tradefee')
var tradePairId = $('#chart-container').data('tradepairid')
var chartAction = $('#chart-container').data('action-chart');
var charttitle = $('#chart-container').data('title')
var chartbaseSymbol = $('#chart-container').data('basesymbol');


$('#table-buyOrders-' + tradePairId).dataTable({
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
	"fnServerParams": function (aoData) {
		aoData.push({ "name": "tradePairId", "value": $('#buyOrders-host').data('tradepair') });
		aoData.push({ "name": "tradeType", "value": $('#buyOrders-host').data('tradetype') });
	},
	"columnDefs": [{
			"targets": 2,
			"render": function (data, type, full, meta) {
				return (full[0] * full[1]).toFixed(8);
			}
		}
	],
	"footerCallback": function (row, data, start, end, display) {
	},
	"rowCallback": function (row, data, index) {
		$(row).on('click', function () {
			calculateOrder(data[1], data[0], true);
		});
	},
	"fnDrawCallback": function (oSettings) {
		updateDepthChart();
	}
});

$('#table-sellOrders-' + tradePairId).dataTable({
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
	"fnServerParams": function (aoData) {
		aoData.push({ "name": "tradePairId", "value": $('#sellOrders-host').data('tradepair') });
		aoData.push({ "name": "tradeType", "value": $('#sellOrders-host').data('tradetype') });
	},
	"columnDefs": [{
		"targets": 2,
		"render": function (data, type, full, meta) {
			return (full[0] * full[1]).toFixed(8);
		}
	}],
	"footerCallback": function (row, data, start, end, display) {
	},
	"rowCallback": function (row, data, index) {
		$(row).on('click', function () {
			calculateOrder(data[1], data[0], false);
		});
	},
	"fnDrawCallback": function (oSettings) {
		updateDepthChart();
	}
});

$('#table-tradehistory-'+ tradePairId).dataTable({
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
	"columnDefs": [
		{
			"targets": 0,
			"render": function (data, type, full, meta) {
				return moment.utc(data).local().format('DD/MM HH:mm:ss');
			}
		}
	],
	"rowCallback": function (row, data, index) {
		var textClass = data[1] === 'Buy' ? 'text-success' : 'text-danger';
		$(row).addClass(textClass);
	}
});
$('.exchange-historyPanel').height($('.exchange-panel-sell').height() - 2)

$('#table-openorders-' + tradePairId).dataTable({
	"order": [[0, "desc"]],
	"lengthChange": false,
	"processing": false,
	"bServerSide": true,
	"searching": false,
	"scrollCollapse": true,
	"scrollY": "340px",
	"sort": true,
	"paging": false,
	"info": false,
	"language": { "emptyTable": "No data avaliable." },
	"sAjaxSource": $('#openorders-host').data('action'),
	"sServerMethod": "POST",
	"fnServerParams": function (aoData) {
		aoData.push({ "name": "tradePairId", "value": $('#openorders-host').data('tradepair') });
	},
	"columnDefs": [
		{
			"targets": 1,
			"render": function (data, type, full, meta) {
				return moment.utc(data).local().format('DD/MM HH:mm:ss');
			}
		},
		{
			"targets": 7,
			"sortable": false,
			"render": function (data, type, full, meta) {
				return '<button class="btn btn-xs btn-danger pull-right" style="width:80px;margin-right:15px" onclick="cancelOrder(' + full[0] + ');">Cancel</button>';
			}
		}
	],
	"rowCallback": function (row, data, index) {
	}
});

$('#table-canceltradepair').on('click', function () {
	var action = $(this).data('action');
	var tradepair = $(this).data('tradepair');
	cancelTradePairOrders(tradepair, action);
});

var cancelAction = $('#openorders-host').data('cancel')
function cancelOrder(orderId) {
	confirmModal("Cancel Order", "Are you sure you want to cancel order #" + orderId + "?", function () {
		postJson(cancelAction, { orderId: orderId, cancelType: 'Single' }, function (data) {
			if (data.Success) {
				var table = "#table-openorders-" + tradePairId;
				var existingRow = $(table + " > tbody tr > td:nth-child(1)").filter(function () {
					return $(this).text() == orderId;
				}).closest("tr")
				existingRow.remove();
			}
		});
	});
}

function cancelTradePairOrders(market) {
	var count = $('#table-openorders-' + tradePairId + ' > tbody > tr > td').length;
	if (count > 1) {
		confirmModal("Cancel Orders", "Are you sure you want to cancel ALL orders for this market?", function () {
			postJson(cancelAction, { market: market, cancelType: 'Market' }, function (data) {
				if (data.Success) {
					$("#table-openorders-" + tradePairId + " > tbody").empty();
				}
			});
		});
	}
}

$(document).off("OnOpenOrderUserUpdate").on("OnOpenOrderUserUpdate", function (event, notification) {
	if (tradePairId !== notification.TradePairId)
		return;

	var type = notification.Action;
	var table = '#table-openorders-' + tradePairId;
	if (type === 'New') {
		$(table + ' > tbody').prepend(Mustache.render(openOrderTemplate, {
			id: notification.OrderId,
			time: moment.utc(notification.Timestamp).local().format('DD/MM HH:mm:ss'),
			type: notification.Type,
			price: notification.Price.toFixed(8),
			amount: notification.Amount.toFixed(8),
			remaining: notification.Remaining.toFixed(8),
			status: "Pending"
		}));
	}

	if (type === 'Fill') {
		var existingRow = $(table + " > tbody tr > td:nth-child(1)").filter(function () {
			return $(this).text() == notification.OrderId;
		}).closest("tr")
		existingRow.remove();
	}

	if (type === 'Partial') {
		var row = $(table + " > tbody tr > td:nth-child(1)").filter(function () {
			return $(this).text() == notification.OrderId;
		}).closest("tr");
		row.find("td:nth-child(7)").text(type);
		row.find("td:nth-child(6)").text(notification.Remaining.toFixed(8));
	}

});

$(document).off("OnTradeHistoryUpdate").on("OnTradeHistoryUpdate", function (event, notification) {
	if (tradePairId !== notification.TradePairId)
		return;

	var table = '#table-tradehistory-' + tradePairId;
	prependTradeHistory(table, notification);
	updateLastPrice(notification.Price);
});

function updateLastPrice(price) {
	var high = $(".chart-high");
	var low = $(".chart-low");
	var last = $(".chart-last");
	var lowPrice = +low.html();
	var highPrice = +high.html();
	if (price < lowPrice || lowPrice == 0) {
		low.html(price.toFixed(8))
	}
	if (price > highPrice) {
		high.html(price.toFixed(8))
	}
	last.html(price.toFixed(8))
}

$(document).off("OnOrderBookUpdate").on("OnOrderBookUpdate", function (event, notification) {
	if (tradePairId !== notification.TradePairId)
		return;

	var type = notification.Type;
	var isBuyOrder = type === "Buy";
	var table = isBuyOrder
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

		var newPoint = false;
		var lastPrice = $(table + " > tbody tr:last > td:nth-child(1)").text()
		var existingRowPrice = existingRow.find("td:nth-child(1)").text()
		if (existingRow && existingRowPrice == notification.Price) {
			// update existing
			mergeOrder(existingRow, notification);
		}
		else if ((!isBuyOrder && firstPrice > notification.Price) || (isBuyOrder && notification.Price > firstPrice)) {
			// less than first
			prependOrder(table, notification);
		}
		else if ((!isBuyOrder && notification.Price > +lastPrice) || (isBuyOrder && notification.Price < +lastPrice)) {
			// more than last
			appendOrder(table, notification);

		}
		else {
			// somewhere in middle
			insertOrder(table, notification)
		}
	}

	updateDepthChart();
});





function appendOrder(table, notification) {
	$(table + ' > tbody').append(Mustache.render(orderTemplate, {
		highlight: "buyhighlight",
		price: notification.Price.toFixed(8),
		amount: notification.Amount.toFixed(8),
		total: (notification.Amount * notification.Price).toFixed(8)
	}));
	$(table + " > tbody tr:last").on('click', function () {
		calculateOrder(notification.Amount, notification.Price, notification.Type === "Buy")
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
		calculateOrder(notification.Amount, notification.Price, notification.Type === "Buy")
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
		calculateOrder(notification.Amount, notification.Price, notification.Type === "Buy")
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
	if (isNaN(newAmount) || isNaN(newTotal) || newAmount <= 0 || newTotal <= 0) {
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
			calculateOrder(newAmount, notification.Price, notification.Type === "Buy")
		});
	}
}

function calculateOrder(amount, price, isBuy) {
	var volume = 0;
	var selectedPrice = +price;
	if (isBuy) {
		$("#table-buyOrders-" + tradePairId + "> tbody  > tr").each(function () {
			var row = $(this);
			var rowprice = +row.find("td:nth-child(1)").text();
			if (rowprice >= selectedPrice) {
				volume += +row.find("td:nth-child(2)").text();
			}
		});
	}
	else {
		$("#table-sellOrders-" + tradePairId + "> tbody  > tr").each(function () {
			var row = $(this);
			var rowprice = +row.find("td:nth-child(1)").text();
			if (rowprice <= selectedPrice) {
				volume += +row.find("td:nth-child(2)").text();
			}
		});
	}
	$('#amount-Sell, #amount-Buy').val((+volume).toFixed(8));
	$('#rate-Sell, #rate-Buy').val((+price).toFixed(8)).trigger('change');
}

function calculateOrderSimple(amount, price) {
	$('#amount-Sell, #amount-Buy').val((+amount).toFixed(8));
	$('#rate-Sell, #rate-Buy').val((+price).toFixed(8)).trigger('change');
}

function prependTradeHistory(table, notification) {
	$(table + ' > tbody').prepend(Mustache.render(historyTemplate, {
		time: moment.utc(notification.Timestamp).local().format('DD/MM HH:mm:ss'),
		type: notification.Type === 'Buy' ? 'text-success' : 'text-danger',
		highlight: notification.Type === 'Buy' ? 'buyhighlight' : 'sellhighlight',
		typeText: notification.Type,
		price: notification.Price.toFixed(8),
		amount: notification.Amount.toFixed(8)
	}));
}


$(".data-balance-sell").on("click", function () {
	var rate = $('#rate-Sell').val();
	var balance = $(this).html();
	if (balance && rate) {
		calculateOrderSimple(balance, rate)
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
		calculateOrderSimple(truncateDecimal(amount), rate)
	}
});


function updateDepthChart() {
	var newBuyData = [];
	var newSellData = [];
	var buysum = 0;
	var sellsum = 0;
	var sellVolume = 0;

	$("#table-buyOrders-" + tradePairId + "> tbody  > tr").each(function () {
		var row = $(this);
		var price = +row.find("td:first").text();
		var amount = +row.find("td:nth-child(2)").text();
		buysum += price * amount;
		if (price && buysum)
			newBuyData.push([price, buysum])
	});

	$("#table-sellOrders-" + tradePairId + "> tbody  > tr").each(function () {
		var row = $(this);
		var price = +row.find("td:first").text();
		var amount = +row.find("td:nth-child(2)").text();
		sellVolume += amount;
		sellsum += price * amount;
		if (price && sellsum)
			newSellData.push([price, sellsum]);
	});

	$(".sum-buyorders").html((+buysum || 0).toFixed(8));
	$(".sum-sellorders").html((+sellVolume || 0).toFixed(8));

	if (isDepthChartSelected) {
		var depth = $('#depthdata').highcharts();
		if (depth) {
			depth.showLoading();
			depth.series[0].setData(newBuyData, true, true, true);
			depth.series[1].setData(newSellData, true, true, true);
			depth.reflow();
			depth.hideLoading();
		}
	}
}





$('.chart-option').on('click', function () {
	$('.chart-option').removeClass('active');
	$(this).addClass('active');
	var selectedChart = $(this).data('type');
	if (selectedChart === 'chart') {
		isDepthChartSelected = false;
		$('#depthdata').hide();
		$('#chartdata').show();
		getChartData();
	}
	else if (selectedChart === 'depth') {
		isDepthChartSelected = true;
		$('#chartdata').hide();
		$('#depthdata').show();
		updateDepthChart();
	}
});


function setVisibleChart() {
	if (isDepthChartSelected) {
		$('.chart-option').removeClass('active');
		$('.chart-option-depth').addClass('active');
		$('#depthdata').addClass('active').show();
		$('#chartdata').removeClass('active').hide();
	}
}


function updateChart(chartData) {
	setVisibleChart();
	var cdata = chartData ? chartData.Candle : [[0, 0, 0, 0, 0, 0]];
	var vdata = chartData ? chartData.Volume : [[0, 0]];
	var chart = $('#chartdata').highcharts();
	if (chart) {
		chart.showLoading();
		chart.series[0].setData(cdata)
		chart.series[1].setData(cdata)
		chart.series[2].setData(vdata)
		chart.xAxis[0].setExtremes();
		chart.rangeSelector.buttons[0].setState(2);
		chart.rangeSelector.clickButton(0, 1, true);
		chart.reflow();
		chart.hideLoading();
	}
}

function getChartData() {
	getJson(chartAction, {}, function (data) {
		updateChart(data);
	});
}

if (!isDepthChartSelected) {
	getChartData();
}

$('#chartdata').highcharts('StockChart', {
	chart: {
		height: 274,
		zoomType: 'x',
		backgroundColor: 'transparent',
		events: {
			load: function () {
				setInterval(function () {
					getChartData()
				}, 60000);
			}
		}
	},

	credits: { enabled: false },
	scrollbar: { enabled: true, liveRedraw: false },
	rangeSelector: { selected: 0 },
	navigator: { enabled: false },
	xAxis: { tickPosition: 'inside' },
	yAxis: [{
		labels: {
			format: '{value:.8f}',
			align: 'right',
			x: -3
		},
		title: {
			text: 'OHLC'
		},
		height: '70%',
		offset: 0,
		lineWidth: 2,
		tickPosition: 'inside'
	},
	{
		labels: {
			format: '{value:.8f}',
			align: 'right',
			x: -3
		},
		title: {
			text: 'Volume'
		},
		top: '72%',
		height: '28%',
		offset: 0,
		lineWidth: 2,
		tickPosition: 'inside'
	}],

	candlestick: { pointWidth: '2px' },

	series: [{
		type: 'candlestick',
		name: charttitle,
		color: '#ee5f5b',
		upColor: '#5cb85c',
	},
	{
		name: 'Mean',
		data: [[0, 0, 0, 0, 0, 0]],
		type: 'spline',
		color: 'rgba(0, 0, 0, 0.2)',
		tooltip: {
			valueDecimals: 8
		}
	},
	{
		type: 'column',
		color: '#666666',
		name: 'Volume',
		data: [[0, 0]],
		yAxis: 1,
	}],

	tooltip:
	{
		changeDecimals: 8,
		valueDecimals: 8,
		followPointer: false
	},

	rangeSelector: {
		inputEnabled: false,
		allButtonsEnabled: true,
		buttons: [{
			type: 'day',
			count: 1,
			text: 'Day',
			dataGrouping: {
				forced: true,
				units: [['minute', [30]]]
			}
		},
		{
			type: 'week',
			count: 1,
			text: 'Week',
			dataGrouping: {
				forced: true,
				units: [['hour', [4]]]
			}
		},
		{
			type: 'week',
			text: 'Month',
			count: 4,
			dataGrouping: {
				forced: true,
				units: [['hour', [12]]]
			}
		},
		{
			type: 'week',
			text: '3 Month',
			count: 12,
			dataGrouping: {
				forced: true,
				units: [['day', [1]]]
			}
		},
		{
			type: 'all',
			text: 'All',
			dataGrouping: {
				forced: true,
				units: [['day', [1]]]
			}
		}],

		buttonTheme: { width: 60 },
		selected: 0
	},
});

$('#depthdata').highcharts({
	chart: {
		type: 'area',
		zoomType: 'xy',
		height: 260,
		backgroundColor: 'transparent'
	},
	title: {
		text: ''
	},
	legend: {
		enabled: false
	},
	xAxis: {
		type: "logarithmic",
		name: "Price",
		labels: {
			format: '{value:.8f}',
			align: 'right',
			x: -3
		},
		tickPosition: 'inside',
		tickInterval: 0.4,
		showFirstLabel: false,
		showLastLabel: false
	},
	yAxis: {
		type: "logarithmic",
		labels: {
			format: '{value:.8f}',
			align: 'right',
			x: -3,
			y: 0
		},
		title: {
			text: 'Volume ' + chartbaseSymbol
		},
		offset: 0,
		lineWidth: 2,
		tickPosition: 'inside',
		opposite: true,
		showFirstLabel: false,
		showLastLabel: false
	},
	credits: {
		enabled: false
	},
	tooltip:
		{
			changeDecimals: 8,
			valueDecimals: 8,
			followPointer: false,
			headerFormat: '<b style="font-size:14px">{series.name} Depth</b><br/><span>Price: <b>{point.key:.8f}</b> ' + chartbaseSymbol + '</span><br/>',
			pointFormat: '<span>Volume: <b>{point.y:.8f}</b> ' + chartbaseSymbol + '</span><br/>'
		},
	series: [{

		name: 'Buy',
		data: [],
		color: "#5cb85c",
		fillOpacity: 0.5,
		lineWidth: 1,
		marker: {
			enabled: true,
			radius: 2,
			symbol: 'circle'
		},
		yAxis: 0
	},
	{
		name: 'Sell',
		color: "#d9534f",
		fillOpacity: 0.5,
		data: [],
		lineWidth: 1,
		marker: {
			enabled: true,
			radius: 2,
			symbol: 'circle'
		},
		yAxis: 0
	}]
});
