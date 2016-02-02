$(function () {

	$('#table-tradepair > tbody > tr').on('click', function (e) {
		$('#table-tradepair > tbody > tr').removeClass('active');
		$(this).addClass('active');
		var marketUrl = $(this).data('externallink');
		if (marketUrl) {
			window.location = marketUrl;
		}
	});

});