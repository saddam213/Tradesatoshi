var isDepthChartSelected = false;
$(function () {

	$('#table-tradepair > .tradepair-row').on('click', function (e) {
		$('#table-tradepair > .tradepair-row').removeClass('active');
		$(this).addClass('active');

		var marketUrl = $(this).data('marketurl');
		if (marketUrl) {
			History.pushState({}, "TradeSatoshi - " + marketUrl, "?market=" + marketUrl);
		}

		var targetUrl = $(this).data('action');
		if (targetUrl) {
			var targetName = '#tradepairTarget';
			var tradePairTarget = $(targetName);
			if (tradePairTarget) {
				tradePairTarget.unbind();
			}
			getPartial(targetName, targetUrl);
		}
	});
	
	var defaultMarket = $('#tradepairTarget').data('default');
	if (defaultMarket) {
		$('.tradepair-row[data-marketurl="' + defaultMarket + '"]').trigger('click');
	} else {
		$('#table-tradepair .tradepair-row:first').trigger('click');
	}
		
});