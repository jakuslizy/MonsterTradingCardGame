using MonsterTradingCardGame.API.Server.DTOs;

namespace MonsterTradingCardGame.API.Server.Views;

public class StatsView
{
    private readonly string _cssPath;

    public StatsView()
    {
        var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
        _cssPath = Path.Combine(baseDirectory, "Assets", "css", "style.css");

        if (!File.Exists(_cssPath))
        {
            throw new FileNotFoundException($"CSS-Datei nicht gefunden: {_cssPath}");
        }
    }

    public Response Render()
    {
        string cssContent = File.ReadAllText(_cssPath);
        return new Response(
            200,
            GenerateHtml(cssContent),
            "text/html"
        );
    }

    private string GenerateHtml(string cssContent) => @"
<!DOCTYPE html>
<html>
<head>
    <meta charset='UTF-8'>
    <title>MTCG - Statistiken</title>
    <style>
        " + cssContent + @"
    </style>
</head>
<body>
    <button class='hamburger' onclick='toggleSidebar()'>
        <span></span>
        <span></span>
        <span></span>
    </button>

    <div class='sidebar'>
        <a href='/profile' class='nav-button'>Mein Profil</a>
        <a href='/deck.html' class='nav-button'>Mein Deck</a>
        <a href='/shop' class='nav-button'>Shop</a>
        <a href='/trading.html' class='nav-button'>Handel</a>
        <a href='javascript:void(0)' onclick='logout()' class='nav-button'>Ausloggen</a>
    </div>

    <div class='container stats-container'>
        <h1>Statistiken</h1>
        
        <div class='stats-section'>
            <div class='stats-card'>
                <h2>Persönliche Statistiken</h2>
                <div id='personalStats' class='stats-details'>
                    <p>Elo: <span id='elo'>Lade...</span></p>
                    <p>Siege: <span id='wins'>Lade...</span></p>
                    <p>Niederlagen: <span id='losses'>Lade...</span></p>
                    <p>Unentschieden: <span id='draws'>Lade...</span></p>
                    <p>Gewinnrate: <span id='winRate'>Lade...</span></p>
                </div>
            </div>

            <div class='stats-card'>
                <h2>Bestenliste</h2>
                <div id='scoreboard' class='scoreboard-list'>
                    <!-- Scoreboard wird hier dynamisch eingefügt -->
                </div>
            </div>
        </div>

        <a href='/' class='back-link'>← Zurück zur Startseite</a>
    </div>

    <script>
        async function loadStats() {
            const token = localStorage.getItem('token');
            if (!token) {
                window.location.href = '/login';
                return;
            }

            try {
                // Debug-Ausgabe hinzufügen
                console.log('Token:', token);

                // Lade persönliche Statistiken
                const statsResponse = await fetch('/stats', {
                    headers: {
                        'Authorization': 'Bearer ' + token,
                        'Content-Type': 'application/json'  // Header hinzugefügt
                    }
                });

                // Debug-Ausgabe für die Antwort
                console.log('Stats Response:', await statsResponse.clone().text());
                
                if (!statsResponse.ok) {
                    throw new Error(`HTTP error! status: ${statsResponse.status}`);
                }
                
                const statsData = await statsResponse.json();
                console.log('Stats Data:', statsData);  // Debug-Ausgabe
                
                // Aktualisiere UI mit persönlichen Stats
                document.getElementById('elo').textContent = statsData.ELO || '100';
                document.getElementById('wins').textContent = statsData.GamesWon || '0';
                document.getElementById('losses').textContent = statsData.GamesLost || '0';
                document.getElementById('draws').textContent = '0';
                document.getElementById('winRate').textContent = statsData.WinRate || '0%';

                // Lade Scoreboard
                const scoreboardResponse = await fetch('/scoreboard', {
                    headers: {
                        'Authorization': 'Bearer ' + token,
                        'Content-Type': 'application/json'  // Header hinzugefügt
                    }
                });

                // Debug-Ausgabe für Scoreboard
                console.log('Scoreboard Response:', await scoreboardResponse.clone().text());
                
                if (!scoreboardResponse.ok) {
                    throw new Error(`HTTP error! status: ${scoreboardResponse.status}`);
                }
                
                const scoreboardData = await scoreboardResponse.json();
                console.log('Scoreboard Data:', scoreboardData);  // Debug-Ausgabe
                
                // Aktualisiere Scoreboard UI
                const scoreboardContainer = document.getElementById('scoreboard');
                scoreboardContainer.innerHTML = '';
                
                if (Array.isArray(scoreboardData) && scoreboardData.length > 0) {
                    scoreboardData.forEach((player, index) => {
                        const playerDiv = document.createElement('div');
                        playerDiv.className = 'scoreboard-item';
                        
                        // Berechne die Siege aus der Winrate und GamesPlayed
                        const winRate = parseFloat(player.WinRate) || 0;
                        const gamesPlayed = player.GamesPlayed || 0;
                        const wins = Math.round((winRate * gamesPlayed) / 100);
                        
                        playerDiv.innerHTML = `
                            <span class='rank'>#${index + 1}</span>
                            <span class='player-name'>${player.Name || player.Username || 'Unbekannt'}</span>
                            <span class='player-elo'>Elo: ${player.Elo || '0'}</span>
                            <span class='player-wins'>Siege: ${wins}</span>
                        `;
                        scoreboardContainer.appendChild(playerDiv);
                    });
                } else {
                    scoreboardContainer.innerHTML = '<p>Keine Daten verfügbar</p>';
                }

            } catch (error) {
                console.error('Fehler beim Laden der Statistiken:', error);
                // Zeige Fehlermeldung in der UI
                document.getElementById('personalStats').innerHTML = 
                    '<p class=""error"">Fehler beim Laden der Statistiken: ' + error.message + '</p>';
            }
        }

        function logout() {
            const token = localStorage.getItem('token');
            if (token) {
                fetch('/sessions', {
                    method: 'DELETE',
                    headers: {
                        'Authorization': 'Bearer ' + token
                    }
                }).then(() => {
                    localStorage.removeItem('token');
                    window.location.href = '/';
                }).catch(error => {
                    console.error('Fehler beim Ausloggen:', error);
                    localStorage.removeItem('token');
                    window.location.href = '/';
                });
            } else {
                window.location.href = '/';
            }
        }

        function toggleSidebar() {
            const sidebar = document.querySelector('.sidebar');
            const hamburger = document.querySelector('.hamburger');
            sidebar.classList.toggle('active');
            hamburger.classList.toggle('active');
        }

        document.addEventListener('click', function(event) {
            const sidebar = document.querySelector('.sidebar');
            const hamburger = document.querySelector('.hamburger');
            if (!sidebar.contains(event.target) && !hamburger.contains(event.target) && sidebar.classList.contains('active')) {
                sidebar.classList.remove('active');
                hamburger.classList.remove('active');
            }
        });

        loadStats();
    </script>
</body>
</html>";
} 