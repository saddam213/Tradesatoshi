$(function () {

	$('#table-tradepair > .tradepair-row').on('click', function (e) {
		$('#table-tradepair > .tradepair-row').removeClass('active');
		$(this).addClass('active');
		var marketUrl = $(this).data('externallink');
		if (marketUrl) {
			window.location = marketUrl;
		}
	});

	$(".section-option").on("click", function () {
		$(".section-option").removeClass('active');
		$(this).addClass('active');
		var url = $(this).data("action");
		var section = $(this).data("section");
		if (url) {
			$('#historyTarget').unbind();
			getPartial('#historyTarget', url);
			var currency = $('#historyTarget').data("currency");
			var queryString = "?area=" + section;
			if (currency) {
				queryString += "&coin=" + currency;
			}
			History.pushState({}, "TradeSatoshi - History", queryString);
		}
	});

	$("[data-section='" + $("#historyTarget").data("section") + "']").trigger("click");

	$('#CurrencyFilter').on('change', function () {
		var url = $(this).data('action');
		var currency = $(this).val();
		var section = $('#history-menu').find('.active').data('section') || "Deposit";
		window.location = url + "?area=" + section + "&coin=" + currency;
	});

});