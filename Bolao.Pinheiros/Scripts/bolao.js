var loading = $('.lds-container');
var dataDesejada = $('#datepicker');
var searchFilter = $('#search-field');
var dadosDosJogos = $('.dados-jogos');
var gettingUpdates = false;

$(document).ready(function () {
    try {
        Notify.requestPermission(onPermissionGranted, onPermissionDenied);
    } catch {
        console.log('Notify not supported.');
    }

    dataDesejada.datepicker({ dateFormat: "dd/mm/yy" });

    dataDesejada.on('change', function () {
        getGamesOnDate(this.value);
        searchFilter.val('');
    });

    searchFilter.on('keydown keyup keypress', function () {
        filterGames();
    });

    $(document).on('click', '.detalhar-estatisticas', function () {
        showStatistics(this);
    });

    $(document).on('click', '.flip-card-icon', function () {
        $(this).siblings('.card').toggleClass('is-flipped');
    });

    $(document).on('click', '.analisis-button', function () {
        $(this).parents('.competition').find('.detalhar-estatisticas').click();
    });

    $(document).on('click', '.component-live', function () {
        showLiveGames();
        searchFilter.val('');
    });

    $(document).on('click', '.component-alert', function () {
        $(this).find('.alert-button').toggleClass('alert-active');
    });

    $(document).on('click', '.update-icon', function () {
        getStatistics(this, false);
    });

    var loadTimeout = {};
    $(document).ajaxStart(function () {
        loadTimeout = setTimeout(function () {
            if (!gettingUpdates) {
                loading.show();
            }
        }, 300);
    });
    $(document).ajaxStop(function () {
        if (!$.active) {
            loading.hide();
            clearTimeout(loadTimeout);
        }
    });

    $(document).tooltip({
        classes: { "ui-tooltip": "tooltip-bolao" },
        position: { my: "left+10 top+5", at: "left bottom", collision: "flipfit" },
        track: true
    });

    setInterval(function () {
        $('.game-live .game-card-status-badge').toggleClass('badge-visible');
    }, 800);

    setInterval(function () {
        getGamesDataUpdate();
    }, 10000);
});

var getBetsStatistic = function (filter) {
    var gamesEnded = filter !== null ? filter : $('[data-ended="True"]');
    var winner = gamesEnded.find('[data-bet-winner="True"]');
    var over = 0;
    var both = 0;
    var less = 0;
    var winnerTeam = 0;
    for (var i = 0; i < winner.length; i++) {
        if ($(winner[i]).html().indexOf('Jogo acima de') !== -1) {
            over++;
        } else if ($(winner[i]).html().indexOf('Ambos marcam') !== -1) {
            both++;
        } else if ($(winner[i]).html().indexOf('Jogo abaixo de') !== -1) {
            less++;
        } else if ($(winner[i]).html().indexOf('Possível vencedor') !== -1) {
            winnerTeam++;
        }
    }

    alert('Total: ' + gamesEnded.length + ' - Acima: ' + over + ' - Ambos marcam: ' + both + ' - Abaixo de: ' + less + ' - Vencedor: ' + winnerTeam);
};

var getGamesOnDate = function () {
    if (dataDesejada.val() === '') {
        return;
    }

    try {
        $.ajax({
            url: 'Home/GetGamesData',
            type: 'POST',
            dataType: 'html',
            cache: false,
            data: {
                date: dataDesejada.val()
            },
            beforeSend: function () {
                loading.show();
            }
        }).done(function (result) {
            dadosDosJogos.html($(result).html());
        }).fail(function (ex) {
            // TODO
        });
    } catch (error) {
        // TODO
    }
};

var getGamesDataUpdate = function () {
    if ($.active || $('#GetUpdates').val() === 'False') {
        return;
    }

    try {
        gettingUpdates = true;
        $.ajax({
            url: 'Home/GetGamesDataUpdate',
            type: 'POST',
            dataType: 'html',
            cache: false,
            data: {
                date: dataDesejada.val()
            }
        }).done(function (result) {
            gettingUpdates = false;
            updateGamesPlaying(JSON.parse(result));
        }).fail(function (ex) {
            // TODO
        });
    } catch (error) {
        // TODO
    }
};

var getStatistics = function (element, show) {
    try {
        var gameId = $(element).parents('.container-jogo').attr('id');

        $.ajax({
            url: 'Home/GetStatistics',
            type: 'POST',
            dataType: 'html',
            cache: false,
            data: {
                gameId: gameId
            },
            beforeSend: function () {
                loading.show();
            }
        }).done(function (result) {
            var parent = $(element).parents('.container-jogo');
            if (show) {
                var statistics = parent.find('.detalhar-estatisticas');
                statistics.after(result);
                showStatistics(element);
            } else {
                parent.find('.container-estatisticas').html($(result).html());
            }
        }).fail(function (ex) {
            // TODO
        });
    } catch (error) {
        // TODO
    }
};

var filterGames = function () {
    var filter = latinize(searchFilter.val()).toLowerCase();
    var competitions = $('.competition');
    var containerJogos = $('.container-jogo');
    competitions.hide();
    containerJogos.hide();
    if (filter === null
        || filter === '') {
        competitions.show();
        containerJogos.show();
        return;
    } else {
        for (var i = 0; i < competitions.length; i++) {
            var competition = $(competitions[i]);
            var games = competition.find('.container-jogo');
            var title = competition.find('.competition-title label').html().trim();
            if (latinize(title.toLowerCase()).indexOf(filter) !== -1) {
                competition.show();
                games.show();
            } else {
                for (var g = 0; g < games.length; g++) {
                    var game = $(games[g]);
                    var teams = game.find('.team-name');
                    if (latinize($(teams[0]).html()).trim().toLowerCase().indexOf(filter) !== -1) {
                        competition.show();
                        game.show();
                    } else if ($(teams[1]).html().trim().toLowerCase().indexOf(filter) !== -1) {
                        competition.show();
                        game.show();
                    }
                }
            }
        }
    }

    setGamesVisible();
};

var setGamesVisible = function () {
    $('.games-count').html('(' + $('.container-jogo').filter(':visible').length + ')');
};

var showLiveGames = function () {
    var liveClass = 'live-active';
    var liveButton = $('.live-button').toggleClass(liveClass);
    var containerJogos = $('.container-jogo');
    containerJogos.show();
    if (liveButton.hasClass(liveClass)) {
        for (var i = 0; i < containerJogos.length; i++) {
            var isLive = $(containerJogos[i]).find('.game-playing').length;
            if (isLive) {
                $(containerJogos[i]).show();
            } else {
                $(containerJogos[i]).hide();
            }
        }
    } else {
        containerJogos.show();
    }

    showOrHideCompetitions();
    setGamesVisible();
};

var showOrHideCompetitions = function () {
    var competitions = $('.competition');
    competitions.show();
    for (var c = 0; c < competitions.length; c++) {
        if ($(competitions[c]).find('.container-jogo:visible').length === 0) {
            $(competitions[c]).hide();
        }
    }
};

var showStatistics = function (element) {
    var statistics = $(element).parents('.container-jogo').find('.container-estatisticas');
    if (statistics.length) {
        if (statistics.is(':visible')) {
            statistics.slideUp();
        } else {
            statistics.slideDown();
        }
    } else {
        getStatistics(element, true);
    }
};

var updateGamesPlaying = function (result) {
    if (result === null
        || result.games === null) {
        return;
    }

    for (var i = 0; i < result.games.length; i++) {
        var game = result.games[i];
        var gameId = game.id;
        var gameContainer = $('#' + gameId);
        gameContainer.data('game', JSON.stringify(result.games[i]));
        if (gameContainer.length) {
            var gameLive = game.gameTimeAndStatusDisplayType !== 1;
            var gamePlaying = game.gameTimeAndStatusDisplayType === 2;
            var gameStatus = gameContainer.find('.game-card-status-badge');
            if (gamePlaying) {
                if (!gameStatus.parent().hasClass('game-live')) {
                    gameStatus.parent().addClass('game-live');
                }

                gameStatus.html(game.gameTimeDisplay.replace('\'', ''));
                var score = gameContainer.find('.game-card-content-score');

                var home = game.homeCompetitor;
                var away = game.awayCompetitor;
                if (home.score >= 0) {
                    var scoreText = home.score + ' - ' + away.score;
                    if (!Notify.needsPermission
                        && score.html().trim() !== scoreText
                        && gameContainer.parents('.competition').find('.alert-active').length) {
                        var gameText = home.name + ' - ' + away.name;
                        doNotification(gameText, scoreText);
                    }

                    score.html(scoreText);
                }
            } else {
                gameStatus.removeClass('game-live');
                if (gameLive) {
                    if (!gameStatus.hasClass('game-playing')) {
                        gameStatus.addClass('game-playing');
                    }
                } else {
                    gameStatus.removeClass('game-playing');
                }

                if (game.winDescription !== null
                    && game.winDescription !== '') {
                    gameStatus.html(result.games[i].winDescription);
                } else {
                    gameStatus.html(result.games[i].statusText);
                }
            }

            /* Odds */
            if (game.odds !== null
                    && game.odds.options !== null
                    && game.odds.options.length > 0) {
                var homeOdd = game.odds.options[0];
                if (homeOdd.rate.Decimal > 0) {
                    gameContainer.find('.home-odd').html(homeOdd.rate.Decimal);
                    var drawOdd = game.odds.options[1];
                    gameContainer.find('.draw-odd').html(drawOdd.rate.Decimal);
                    var awayOdd = game.odds.options[2];
                    gameContainer.find('.away-odd').html(awayOdd.rate.Decimal);
                }
            }
        }
    }
};

/* Notification */
function onShowNotification() {
    console.log('notification is shown!');
}

function onCloseNotification() {
    console.log('notification is closed!');
}

function onClickNotification() {
    console.log('notification was clicked!');
}

function onErrorNotification() {
    console.error('Error showing notification. You may need to request permission.');
}

function onPermissionGranted() {
    console.log('Permission has been granted by the user');
}

function onPermissionDenied() {
    console.warn('Permission has been denied by the user');
}

function doNotification(title, content) {
    var myNotification = new Notify(title, {
        body: content,
        notifyShow: onShowNotification,
        notifyClose: onCloseNotification,
        notifyClick: onClickNotification,
        notifyError: onErrorNotification
    });

    myNotification.show();
}