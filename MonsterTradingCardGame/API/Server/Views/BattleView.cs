using MonsterTradingCardGame.API.Server.DTOs;

namespace MonsterTradingCardGame.API.Server.Views;

public class BattleView
{
    private readonly string _cssPath;

    public BattleView()
    {
        var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
        _cssPath = Path.Combine(baseDirectory, "Assets", "css", "style.css");
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

    private string GenerateHtml(string cssContent) => $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='UTF-8'>
    <title>MTCG - Battle</title>
    <style>
        {cssContent}
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
        <a href='/stats.html' class='nav-button'>Statistiken</a>
        <a href='/trading.html' class='nav-button'>Handel</a>
        <a href='javascript:void(0)' onclick='logout()' class='nav-button'>Ausloggen</a>
    </div>

    <div class='container battle-container'>
        <h1>Battle Arena</h1>
        
        <div id='battleStatus' class='battle-status'>
            <p>Status: <span id='statusText'>Bereit zum Kampf</span></p>
        </div>

        <div class='battle-controls'>
            <button id='startBattleBtn' onclick='startBattle()' class='cta-button'>Schlacht beginnen</button>
        </div>

        <div id='battleLog' class='battle-log' style='display: none;'>
            <h2>Kampfprotokoll</h2>
            <div id='logContent' class='log-content'></div>
        </div>

        <div id='message' class='message' style='display: none;'></div>
        <a href='/' class='back-link'>← Zurück zur Startseite</a>
    </div>

    <script>
        let battleInterval;
        let waitingForOpponent = false;
        let battleStarted = false;

        async function startBattle() {{
            const token = localStorage.getItem('token');
            if (!token) {{
                window.location.href = '/login';
                return;
            }}

            if (waitingForOpponent) {{
                showMessage('Du wartest bereits auf einen Gegner...', 'info');
                return;
            }}

            // Prüfe Deck vor dem Battle
            try {{
                const deckResponse = await fetch('/deck', {{
                    headers: {{
                        'Authorization': 'Bearer ' + token
                    }}
                }});

                if (deckResponse.ok) {{
                    const deck = await deckResponse.json();
                    if (!Array.isArray(deck) || deck.length !== 4) {{
                        showMessage('Dein Deck muss genau 4 Karten enthalten!', 'error');
                        return;
                    }}
                }} else {{
                    throw new Error('Fehler beim Laden des Decks');
                }}

                document.getElementById('startBattleBtn').disabled = true;
                document.getElementById('statusText').textContent = 'Suche nach Gegner...';
                waitingForOpponent = true;

                const response = await fetch('/battles', {{
                    method: 'POST',
                    headers: {{
                        'Authorization': 'Bearer ' + token
                    }}
                }});

                const battleLog = await response.text();
                
                if (response.status === 202) {{
                    showMessage('Warte auf einen Gegner...', 'info');
                    startPolling();
                }} else if (response.ok) {{
                    displayBattleLog(battleLog);
                    waitingForOpponent = false;
                    document.getElementById('startBattleBtn').disabled = false;
                    document.getElementById('statusText').textContent = 'Kampf beendet';
                    stopPolling();
                }} else {{
                    throw new Error(battleLog);
                }}
            }} catch (error) {{
                showMessage('Fehler: ' + error, 'error');
                waitingForOpponent = false;
                document.getElementById('startBattleBtn').disabled = false;
                document.getElementById('statusText').textContent = 'Bereit zum Kampf';
                stopPolling();
            }}
        }}

        function startPolling() {{
            battleInterval = setInterval(checkBattleStatus, 2000);
        }}

        function stopPolling() {{
            if (battleInterval) {{
                clearInterval(battleInterval);
                battleInterval = null;
            }}
            battleStarted = false;
        }}

        async function checkBattleStatus() {{
            const token = localStorage.getItem('token');
            if (!token) return;

            try {{
                const response = await fetch('/battles', {{
                    method: 'POST',
                    headers: {{
                        'Authorization': 'Bearer ' + token
                    }}
                }});

                const battleLog = await response.text();
                
                if (response.status === 202) {{
                    // Weiteres Polling, da noch kein Gegner gefunden wurde
                    document.getElementById('statusText').textContent = 'Warte auf Gegner...';
                    return;
                }} else if (response.ok && battleLog && battleLog.trim() !== '') {{
                    // Zeige das Battle-Log an
                    displayBattleLog(battleLog);
                    waitingForOpponent = false;
                    document.getElementById('startBattleBtn').disabled = false;
                    document.getElementById('statusText').textContent = 'Kampf beendet';
                    stopPolling();
                }}
                // Ignoriere alle anderen Status-Codes und setze das Polling fort
            }} catch (error) {{
                console.error('Fehler beim Prüfen des Kampfstatus:', error);
            }}
        }}

        function displayBattleLog(log) {{
            const logContent = document.getElementById('logContent');
            const battleLog = document.getElementById('battleLog');
            
            // Formatiere den Battle-Log für bessere Lesbarkeit
            logContent.innerHTML = log.split('\\n').map(function(line) {{ return '<p>' + line + '</p>'; }}).join('');
            battleLog.style.display = 'block';
        }}

        function showMessage(text, type) {{
            const messageDiv = document.getElementById('message');
            messageDiv.textContent = text;
            messageDiv.className = 'message ' + type;
            messageDiv.style.display = 'block';
            setTimeout(() => {{
                messageDiv.style.display = 'none';
            }}, 3000);
        }}

        function logout() {{
            const token = localStorage.getItem('token');
            if (token) {{
                fetch('/sessions', {{
                    method: 'DELETE',
                    headers: {{
                        'Authorization': 'Bearer ' + token
                    }}
                }}).then(() => {{
                    localStorage.removeItem('token');
                    window.location.href = '/';
                }}).catch(error => {{
                    console.error('Fehler beim Ausloggen:', error);
                    localStorage.removeItem('token');
                    window.location.href = '/';
                }});
            }} else {{
                window.location.href = '/';
            }}
        }}

        function toggleSidebar() {{
            const sidebar = document.querySelector('.sidebar');
            const hamburger = document.querySelector('.hamburger');
            sidebar.classList.toggle('active');
            hamburger.classList.toggle('active');
        }}

        document.addEventListener('click', function(event) {{
            const sidebar = document.querySelector('.sidebar');
            const hamburger = document.querySelector('.hamburger');
            if (!sidebar.contains(event.target) && !hamburger.contains(event.target) && sidebar.classList.contains('active')) {{
                sidebar.classList.remove('active');
                hamburger.classList.remove('active');
            }}
        }});

        async function checkDeckStatus() {{
            const token = localStorage.getItem('token');
            if (!token) return;

            try {{
                const deckResponse = await fetch('/deck', {{
                    headers: {{
                        'Authorization': 'Bearer ' + token
                    }}
                }});

                if (deckResponse.ok) {{
                    const deck = await deckResponse.json();
                    if (!Array.isArray(deck) || deck.length !== 4) {{
                        document.getElementById('statusText').textContent = 'Deck unvollständig (4 Karten benötigt)';
                        document.getElementById('startBattleBtn').disabled = true;
                    }} else {{
                        document.getElementById('statusText').textContent = 'Bereit zum Kampf';
                        document.getElementById('startBattleBtn').disabled = false;
                    }}
                }}
            }} catch (error) {{
                console.error('Fehler beim Prüfen des Decks:', error);
            }}
        }}

        // Füge einen Event-Listener hinzu, der beim Laden der Seite den Status prüft
        document.addEventListener('DOMContentLoaded', checkDeckStatus);
    </script>
</body>
</html>";
} 