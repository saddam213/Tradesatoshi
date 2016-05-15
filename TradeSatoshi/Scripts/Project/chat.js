var chatHub = $.connection.Chat;
var currentChatUser = $('#chat-table').data('user');
var chatMessageTemplate = $('#chatMessageTemplate').html();
chatHub.client.OnlineCount = function (count, timestamp) {
	$('#chat-count').text(count);
	$('#chat-table > tbody tr').each(function () {
		$(this).find('.time').each(function () {
			$(this).html(timeSince(new Date(timestamp), new Date($(this).data('val'))));
		});
	});
};
chatHub.client.Messages = function (messages, timestamp) {
	$.each(messages, function (i, message) {
		addChatMessage(message, timestamp);
	});
	setChatScrollPosition(true);
};
chatHub.client.NewMessage = function (message, timestamp) {
	var shouldScroll = shouldChatScroll();
	addChatMessage(message, timestamp);
	setChatScrollPosition(shouldScroll);
};

chatHub.client.RemoveMessage = function (id) {
	$('#message-' + id).remove();
};

$.connection.hub.start().done(function () {
	chatHub.server.getOnlineCount();
	chatHub.server.getMessages();

	$('#chat-submit').on('click', function () {
		var input = $('#chat-input');
		var message = input.val();
		if (message) {
			chatHub.server.sendMessage(message);
			input.val('');
		}
	});

	$('#chat-input').keydown(function (event) {
		if (event.keyCode == 13 && !event.shiftKey) {
			$('#chat-submit').trigger('click');
			return false;
		}
	});

	setInterval(function () {
		if ($.connection.hub.state !== $.signalR.connectionState.disconnected) {
			chatHub.server.getOnlineCount();
		}
	}, 10000);
});


function addChatMessage(message, timestamp) {
	if (message) {
		var higlight = containsName(message.Message, currentChatUser) ? 'bg-success' : '';
		$('#chat-table > tbody').append(Mustache.render(chatMessageTemplate,
		{
			highlight: higlight,
			icon: message.Icon,
			user: message.UserName,
			time: message.Timestamp,
			since: timeSince(new Date(timestamp), new Date(message.Timestamp)),
			message: message.Message,
			id: message.Id
		}));
	}
}

function setChatScrollPosition(shouldScroll) {
	if (shouldScroll) {
		var container = $('#chat-table-container');
		container.scrollTop(container[0].scrollHeight);
	}
}

function shouldChatScroll() {
	var container = $('#chat-table-container');
	var scrollHeight = container[0].scrollHeight;
	var currentScrollPosition = container.scrollTop() + container.innerHeight() + 100;
	return scrollHeight < currentScrollPosition;
}

function containsName(message, name) {
	var username = '@' + currentChatUser;
	var t = message.indexOf(username + ' ');
	if (message.indexOf(username + ' ') != -1) {
		return true;
	}
	if (message.indexOf(username + ',') != -1) {
		return true;
	}
	if (message === username) {
		return true;
	}
	return false;
}

function quoteMessage(user) {
	var input = $('#chat-input');
	input.focus();
	var username = '@' + user;
	var currentValue = input.val();
	if (currentValue) {
		input.val(input.val() + username + ' ');
	} else {
		input.val(username + ',');
	}
	input.focus();
}

function removeMessage(id) {
	confirmModal('Remove Post?', 'Are you sure you want to remove this post?', function () {
		chatHub.server.removeMessage(id);
	});
}

var chatWindow = null;
function openChat(url) {
	settings = "width=480, height=640, scrollbars=no, location=no, directories=no, status=no, menubar=no, toolbar=no, resizable=yes, dependent=no";
	chatWindow = window.open(url, 'Chatbox', settings);
	chatWindow.focus();
}