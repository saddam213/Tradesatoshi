(function () {

	$("#Type").on('change', function () {
		var _this = $(this);
		clearMessages();
		$('.input-submit').show();
		$(".data-input-email, .data-input-google, .data-input-pincode, .data-input-none").hide();
		var selection = _this.val();
		if (selection === '0') { $(".data-input-none").show(); $('.input-submit').hide(); }
		if (selection === '1') { $(".data-input-email").show(); }
		if (selection === '2') { $(".data-input-google").show(); }
		if (selection === '3') { $(".data-input-pincode").show(); }
	}).trigger('change');


	$("#send-email").on('click', function () {
		clearMessages();
		var _this = $(this);
		var valid = $('.form-horizontal').valid();
		var email = $("#DataEmail").val();
		if (email && valid) {
			var action = _this.data('action');
			var error = _this.data('error');
			var success = _this.data('success');
			var component = _this.data('component');
			postJson(action, { componentType: component, dataEmail: email }, function (data) {
				if (data.Success) {
					$('#send-email').removeClass('btn-danger').addClass('btn-success');
					showSuccess(success)
					return;
				}
				$('#send-email').removeClass('btn-success').addClass('btn-danger');
				showError(error)
			});
		}
	});

	$("#verify-email").on('click', function () {
		clearMessages();
		var _this = $(this);
		var code = $("#code-email").val();
		if (code) {
			var action = _this.data('action');
			var error = _this.data('error');
			var success = _this.data('success');
			var component = _this.data('component');
			postJson(action, { componentType: component, code: code }, function (data) {
				if (data.Success) {
					$('#verify-email').removeClass('btn-danger').addClass('btn-success');
					showSuccess(success)
					return;
				}
				$('#verify-email').removeClass('btn-success').addClass('btn-danger');
				showError(error)
			});
		}
	});

	$("#verify-google").on('click', function () {
		clearMessages();
		var _this = $(this);
		var code = $("#code-google").val();
		if (code) {
			var action = _this.data('action');
			var error = _this.data('error');
			var success = _this.data('success');
			var key = _this.data('key');
			postJson(action, { key: key, code: code }, function (data) {
				if (data.Success) {
					$('#verify-google').removeClass('btn-danger').addClass('btn-success');
					showSuccess(success)
					return;
				}
				$('#verify-google').removeClass('btn-success').addClass('btn-danger');
				showError(error)
			});
		}
	});


})();


function clearMessages() {
	$('button').removeClass('btn-success btn-danger');
	$('#validationMessage').addClass('validation-summary-valid');
	$('#validationMessage').removeClass('validation-summary-errors')
	$('#errorMessage').addClass('hidden');
	$('#successMessage').addClass('hidden');
	$('#EmailCode, #GoogleCode').removeClass('has-success');
}

function showError(message) {
	$('#errorMessage').removeClass('hidden');;
	$('#successMessage').addClass('hidden');
	$('#errorMessage > h5').text(message);
}

function showSuccess(message) {
	$('#errorMessage').addClass('hidden');
	$('#successMessage').removeClass('hidden');
	$('#successMessage > h5').text(message);
}