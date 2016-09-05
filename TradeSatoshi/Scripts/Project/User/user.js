$(function () {

	$('#table-tradepair > .tradepair-row').on('click', function (e) {
		$('#table-tradepair > .tradepair-row').removeClass('active');
		$(this).addClass('active');
		var marketUrl = $(this).data('externallink');
		if (marketUrl) {
			window.location = marketUrl;
		}
	});

	$("#newApiKey").on("click", function () {
		var action = $(this).data("action");
		postJson(action, {}, function (data) {
			$("#Key").val(data.Key);
			$("#Secret").val(data.Secret);
		});
	});

	$("#account-tab").on("click", function () {
		History.pushState({}, "TradeSatoshi - Account", "Account");
	});

	$("#security-tab").on("click", function () {
		History.pushState({}, "TradeSatoshi - Security", "Security");
	});

	$("#settings-tab").on("click", function () {
		History.pushState({}, "TradeSatoshi - Settings", "Settings");
	});

});