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

	$('#Recipient-search').on("click", function () {
		var _this = $(this);
		var searchValue = $('#Recipient').val();
		var searchAction = _this.data('action');
		_this.removeClass('btn-success btn-danger');
		$('#Recipient-group').removeClass('has-success has-error');
		$('#Recipient-msg').removeClass('text-success text-danger').html('');
		if (searchValue && searchAction) {
			postJson(searchAction, { searchTerm: searchValue }, function (data) {
				if (data.Success) {
					_this.addClass('btn-success');
					$('#Recipient-group').addClass('has-success');
					$('#Recipient-msg').addClass('text-success').html(data.Message);
				}
				else {
					_this.addClass('btn-danger');
					$('#Recipient-group').addClass('has-error');
					$('#Recipient-msg').addClass('text-danger').html(data.Message);
				}
			}, function (a, b, c) {
				_this.addClass('btn-danger');
				$('#Recipient-group').addClass('has-error');
				$('#Recipient-msg').addClass('text-danger').html('Error: ' + c);
			});
		}
	});

});