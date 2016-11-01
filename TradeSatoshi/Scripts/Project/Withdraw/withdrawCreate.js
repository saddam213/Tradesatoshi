$(function () {

	$('#Amount').on("change, paste, keyup", function () {
		var fee = +$('#Fee').val();
		var min = +$('#MinWithdraw').val();
		var max = +$('#MaxWithdraw').val();
		var balance = +$('#Balance').val();
		var totalAmount = +$(this).val();
		var netamount = Math.max((totalAmount - fee), 0);
		$('#netamount').html(netamount.toFixed(8));
		$('#netamount').removeClass('text-danger text-success');
		if (totalAmount > 0) {
			if (totalAmount < min || totalAmount > max || totalAmount > balance) {
				$('#netamount').addClass('text-danger');
			}
			else {
				$('#netamount').addClass('text-success');
			}
		}
		$('#submit').removeAttr('disabled');
	});

	$('#balance').on("click", function () {
		var balance = +$('#Balance').val();
		$('#Amount').val(balance.toFixed(8));
		$('#Amount').trigger('keyup');
		$('#submit').removeAttr('disabled');
	});

	$('#submit').on("click", function () {
		$(this).attr('disabled', 'disabled');
	});

});