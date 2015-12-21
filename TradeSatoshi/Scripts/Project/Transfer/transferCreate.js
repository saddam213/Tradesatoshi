$(function () {

	$('#Amount').on("change, paste, keyup", function () {
		var fee = +$('#Fee').val();
		var balance = +$('#Balance').val();
		var totalAmount = +$(this).val();
		var netamount = Math.max((totalAmount - fee), 0);
		$('#netamount').html(netamount.toFixed(8));
		$('#netamount').removeClass('text-danger text-success');
		if (totalAmount > 0) {
			if (totalAmount > balance) {
				$('#netamount').addClass('text-danger');
			}
			else {
				$('#netamount').addClass('text-success');
			}
		}
	});

	$('#balance').on("click", function () {
		var balance = +$('#Balance').val();
		$('#Amount').val(balance.toFixed(8));
		$('#Amount').trigger('keyup');
	});

});