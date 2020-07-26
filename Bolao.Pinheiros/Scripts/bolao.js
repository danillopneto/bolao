var loading = $('.lds-container');
var dataDesejada = $('#datepicker');
var dadosDosJogos = $('.dados-jogos');
var gettingUpdates = false;

$(document).ready(function () {
    dataDesejada.datepicker({
        dateFormat: "dd/mm/yy"
    });

    dataDesejada.on('change', function () {
        getGamesOnDate(this.value);
    });

    $(document).on('click', '.detalhar-estatisticas', function () {
        showStatistics(this);
    });

    $(document).on('click', '.flip-card-icon', function () {
        $(this).siblings('.card').toggleClass('is-flipped');
    });

    $(document).on('click', '.detalhar-apostas', function () {
        //var estatisticas = $(this).parent().find('.apostas');
        //if (estatisticas.is(':visible')) {
        //    estatisticas.slideUp();
        //} else {
        //    estatisticas.slideDown();
        //}
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
        $('.game-playing .game-card-status-badge').toggleClass('badge-visible');
    }, 800);

    setInterval(function () {
        getGamesDataUpdate();
    }, 10000);

    setSelect2();
});

var getGamesOnDate = function () {
    if (dataDesejada.val() === '') {
        return;
    }

    try {
        gettingUpdates = false;
        $.ajax({
            url: 'Home/GetGamesData',
            type: 'POST',
            dataType: 'html',
            cache: false,
            data: {
                date: dataDesejada.val()
            }
        }).done(function (result) {
            dadosDosJogos.html(result);
            setSelect2();
        }).fail(function (ex) {
            // TODO
        });
    } catch (error) {
        // TODO
    }
};

var getGamesDataUpdate = function () {
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

var getStatistics = function (element) {
    try {
        var gameId = $(element).parents('.container-jogo').attr('id');

        $.ajax({
            url: 'Home/GetStatistics',
            type: 'POST',
            dataType: 'html',
            cache: false,
            data: {
                gameId: gameId
            }
        }).done(function (result) {
            var statistics = $(element).parent().find('.detalhar-estatisticas');
            statistics.after(result);

            showStatistics(element);
        }).fail(function (ex) {
            // TODO
        });
    } catch (error) {
        // TODO
    }
};

var setSelect2 = function () {
    $('.select2-component').select2();
    $('.select2-component').show();
    $('[name="competition-selector"]').on('change', function () {
        showCompetitionsSelected();
    });

    $('[name="games-selector"]').on('change', function () {
        showGamesSelected();
    });

    $('.component-live').on('click', function () {
        showLiveGames();
    });
};

var showCompetitionsSelected = function () {
    var competitions = $('.competition');
    competitions.hide();

    var competitionsSelected = $('[name="competition-selector"]').val();
    if (competitionsSelected === ''
        || competitionsSelected.length === 0) {
        competitions.show();
    } else {
        var selector = '';
        for (var i = 0; i < competitionsSelected.length; i++) {
            selector += '#' + competitionsSelected[i] + ',';
        }

        selector = selector.slice(0, -1);
        $(selector).show();
    }
};

var showGamesSelected = function () {
    var games = $('.container-jogo');

    var competitions = $('.competition');
    competitions.show();
    games.hide();

    var gamesSelected = gamesSelector.val();
    if (gamesSelected === ''
            || gamesSelected.length === 0) {
        games.show();
        competitions.show();
    } else {
        var selector = '';
        for (var i = 0; i < gamesSelected.length; i++) {
            selector += '#' + gamesSelected[i] + ',';
        }

        selector = selector.slice(0, -1);
        $(selector).show();

        showOrHideCompetitions();
    }
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

        showOrHideCompetitions();
    } else {
        containerJogos.show();
    }
};

var showOrHideCompetitions = function () {
    var competitions = $('.competition');
    for (var c = 0; c < competitions.length; c++) {
        if ($(competitions[c]).find('.container-jogo:visible').length === 0) {
            $(competitions[c]).hide();
        }
    }
};

var showStatistics = function (element) {
    var statistics = $(element).parent().find('.container-estatisticas');
    if (statistics.length) {
        if (statistics.is(':visible')) {
            statistics.slideUp();
        } else {
            statistics.slideDown();
        }
    } else {
        getStatistics(element);
    }
};

var updateGamesPlaying = function (result) {
    if (result === null
        || result.games === null) {
        return;
    }

    for (var i = 0; i < result.games.length; i++) {
        var gameId = result.games[i].id;
        var gameContainer = $('#' + gameId);
        if (gameContainer.length && gameContainer.is(':visible')) {
            var gamePlaying = result.games[i].gameTimeAndStatusDisplayType !== 1;
            var gameStatus = gameContainer.find('.game-card-status-badge');
            if (gamePlaying) {
                gameStatus.html(result.games[i].gameTimeDisplay.replace('\'', ''));
                var score = gameContainer.find('.game-card-content-score');

                var homeScore = result.games[i].homeCompetitor.score;
                var awayScore = result.games[i].awayCompetitor.score;
                if (homeScore >= 0) {
                    score.html(homeScore + ' - ' + awayScore);
                }
                score.html();
            } else {
                gameStatus.html(result.games[i].shortStatusText);
            }
        }
    }
};