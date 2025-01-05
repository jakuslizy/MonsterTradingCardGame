using MonsterTradingCardGame.API.Server.DTOs;

namespace MonsterTradingCardGame.API.Server.Views;

public class TradingView
{
    private readonly string _cssPath;
    private readonly Dictionary<string, string> _imageCache;

    public TradingView()
    {
        var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
        _cssPath = Path.Combine(baseDirectory, "Assets", "css", "style.css");
        // Initialisiere Image Cache
        _imageCache = new Dictionary<string, string>();
        var imagesPath = Path.Combine(baseDirectory, "Assets", "images");
        
        // Lade alle Monsterbilder
        for (int i = 1; i <= 8; i++)
        {
            var imagePath = Path.Combine(imagesPath, $"m{i}.png");
            if (File.Exists(imagePath))
            {
                _imageCache[$"m{i}.png"] = Convert.ToBase64String(File.ReadAllBytes(imagePath));
            }
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

    private string GenerateHtml(string cssContent) => $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='UTF-8'>
    <title>MTCG - Handel</title>
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
        <a href='/stats.html' class='nav-button'>Statistiken</a>
        <a href='/battle.html' class='nav-button'>Battle Arena</a>
        <a href='javascript:void(0)' onclick='logout()' class='nav-button'>Ausloggen</a>
    </div>

    <div class='container'>
        <h1>Handelsplatz</h1>
        
        <div class='trading-section'>
            <div class='create-trade'>
                <h2>Neuen Handel erstellen</h2>
                <form id='createTradeForm'>
                    <select id='cardToTrade' class='form-control'>
                        <!-- Karten werden hier dynamisch eingefügt -->
                    </select>
                    <select id='type' class='form-control'>
                        <option value='monster'>Monster</option>
                        <option value='spell'>Zauber</option>
                    </select>
                    <input type='number' id='minimumDamage' class='form-control' placeholder='Mindestschaden'>
                    <button type='submit' class='cta-button'>Handel erstellen</button>
                </form>
            </div>

            <div class='available-trades'>
                <h2>Verfügbare Handelsangebote</h2>
                <select id='cardToOffer' class='form-control'>
                    <!-- Karten zum Tauschen werden hier dynamisch eingefügt -->
                </select>
                <div id='tradesList'>
                    <!-- Handelsangebote werden hier dynamisch eingefügt -->
                </div>
            </div>
        </div>

        <div id='message' class='message' style='display: none;'></div>
        <a href='/' class='back-link'>← Zurück zur Startseite</a>
    </div>

    <script>
        async function loadUserCards() {
            const token = localStorage.getItem('token');
            if (!token) {
                window.location.href = '/login';
                return;
            }

            try {
                const response = await fetch('/cards', {
                    headers: {
                        'Authorization': 'Bearer ' + token
                    }
                });

                if (response.ok) {
                    const cards = await response.json();
                    // Befülle beide Dropdowns
                    const createSelect = document.getElementById('cardToTrade');
                    const offerSelect = document.getElementById('cardToOffer');
                    
                    createSelect.innerHTML = '<option value="">Karte auswählen...</option>';
                    offerSelect.innerHTML = '<option value="">Karte zum Tauschen auswählen...</option>';
                    
                    cards.forEach(card => {
                        if (!card.InDeck) {
                            // Für ""Handel erstellen""
                            const option1 = document.createElement('option');
                            option1.value = card.Id;
                            option1.textContent = `${card.Name} (Schaden: ${card.Damage})`;
                            createSelect.appendChild(option1);
                            
                            // Für ""Handel annehmen""
                            const option2 = document.createElement('option');
                            option2.value = card.Id;
                            option2.textContent = `${card.Name} (Schaden: ${card.Damage})`;
                            offerSelect.appendChild(option2);
                        }
                    });
                }
            } catch (error) {
                showMessage('Fehler beim Laden der Karten: ' + error, 'error');
            }
        }

        async function loadTrades() {
            const token = localStorage.getItem('token');
            if (!token) {
                window.location.href = '/login';
                return;
            }

            try {
                const response = await fetch('/tradings', {
                    headers: {
                        'Authorization': 'Bearer ' + token
                    }
                });

                if (response.ok) {
                    const trades = await response.json();
                    const tradesList = document.getElementById('tradesList');
                    tradesList.innerHTML = '';

                    if (trades.length === 0) {
                        tradesList.innerHTML = '<p>Keine Handelsangebote verfügbar</p>';
                        return;
                    }

                    trades.forEach(trade => {
                        const tradeDiv = document.createElement('div');
                        tradeDiv.className = 'trade-item';
                        tradeDiv.innerHTML = `
                            <div class='trade-details'>
                                <h3>Handelsangebot</h3>
                                <div class='card-preview'>
                                    <img src='data:image/png;base64,${getImageBase64(getCardImage(trade.CardName))}' class='card-image' alt='${trade.CardName}'>
                                    <h4>${trade.CardName}</h4>
                                    <p>Schaden: ${trade.Damage}</p>
                                    <p>Element: ${trade.ElementType || 'Normal'}</p>
                                </div>
                                <div class='trade-requirements'>
                                    <p>Gewünschter Typ: ${trade.Type}</p>
                                    <p>Mindestschaden: ${trade.MinimumDamage}</p>
                                </div>
                            </div>
                            <div class='trade-actions'>
                                <button onclick='acceptTrade(""${trade.Id}"", ""${trade.Type}"", ${trade.MinimumDamage})' class='cta-button'>Annehmen</button>
                                ${isUsersTrade(trade) ? 
                                    `<button onclick='deleteTrade(""${trade.Id}"")' class='cta-button secondary'>Löschen</button>` : 
                                    ''}
                            </div>
                        `;
                        tradesList.appendChild(tradeDiv);
                    });
                }
            } catch (error) {
                showMessage('Fehler beim Laden der Handelsangebote: ' + error, 'error');
            }
        }

        function isUsersTrade(trade) {
            const token = localStorage.getItem('token');
            const username = token ? token.split('-')[0] : '';
            return trade.Username === username;
        }

        document.getElementById('createTradeForm').addEventListener('submit', async (e) => {
            e.preventDefault();
            const token = localStorage.getItem('token');
            if (!token) {
                window.location.href = '/login';
                return;
            }

            const cardId = document.getElementById('cardToTrade').value;
            const type = document.getElementById('type').value;
            const minimumDamage = document.getElementById('minimumDamage').value;

            if (!cardId) {
                showMessage('Bitte wähle eine Karte aus', 'error');
                return;
            }

            try {
                const response = await fetch('/tradings', {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json',
                        'Authorization': 'Bearer ' + token
                    },
                    body: JSON.stringify({
                        Id: generateUUID(),
                        CardToTrade: cardId,
                        Type: type,
                        MinimumDamage: parseInt(minimumDamage)
                    })
                });

                if (response.ok) {
                    showMessage('Handelsangebot erfolgreich erstellt!', 'success');
                    loadTrades();
                    loadUserCards();
                } else {
                    const error = await response.text();
                    showMessage('Fehler beim Erstellen des Handelsangebots: ' + error, 'error');
                }
            } catch (error) {
                showMessage('Fehler beim Erstellen des Handelsangebots: ' + error, 'error');
            }
        });

        async function acceptTrade(tradeId, requiredType, minimumDamage) {
            const token = localStorage.getItem('token');
            if (!token) {
                window.location.href = '/login';
                return;
            }

            const cardSelect = document.getElementById('cardToOffer');
            const selectedCardId = cardSelect.value;
            const selectedCard = cardSelect.options[cardSelect.selectedIndex];

            if (!selectedCardId) {
                showMessage('Bitte wähle eine Karte zum Tauschen aus', 'error');
                return;
            }

            // Extrahiere Kartentyp und Schaden aus dem Optionstext
            const cardText = selectedCard.textContent;
            const isSpell = cardText.toLowerCase().includes('spell');
            const cardType = isSpell ? 'spell' : 'monster';
            const damage = parseInt(cardText.match(/Schaden: (\d+)/)[1]);

            // Prüfe Anforderungen
            if (cardType !== requiredType.toLowerCase()) {
                showMessage(`Diese Karte ist nicht vom geforderten Typ (${requiredType})`, 'error');
                return;
            }

            if (damage < minimumDamage) {
                showMessage(`Die Karte hat zu wenig Schaden (Minimum: ${minimumDamage})`, 'error');
                return;
            }

            // Führe den Handel durch
            try {
                const response = await fetch(`/tradings/${tradeId}`, {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json',
                        'Authorization': 'Bearer ' + token
                    },
                    body: JSON.stringify(selectedCardId)
                });

                if (response.ok) {
                    showMessage('Handel erfolgreich durchgeführt!', 'success');
                    loadTrades();
                    loadUserCards();
                } else {
                    const error = await response.text();
                    showMessage('Fehler beim Handel: ' + error, 'error');
                }
            } catch (error) {
                showMessage('Fehler beim Handel: ' + error, 'error');
            }
        }

        async function deleteTrade(tradeId) {
            const token = localStorage.getItem('token');
            if (!token) {
                window.location.href = '/login';
                return;
            }

            try {
                const response = await fetch(`/tradings/${tradeId}`, {
                    method: 'DELETE',
                    headers: {
                        'Authorization': 'Bearer ' + token
                    }
                });

                if (response.ok) {
                    showMessage('Handelsangebot erfolgreich gelöscht!', 'success');
                    loadTrades();
                    loadUserCards();
                } else {
                    const error = await response.text();
                    showMessage('Fehler beim Löschen des Handelsangebots: ' + error, 'error');
                }
            } catch (error) {
                showMessage('Fehler beim Löschen des Handelsangebots: ' + error, 'error');
            }
        }

        function generateUUID() {
            return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function(c) {
                var r = Math.random() * 16 | 0,
                    v = c == 'x' ? r : (r & 0x3 | 0x8);
                return v.toString(16);
            });
        }

        function showMessage(text, type) {
            const messageDiv = document.getElementById('message');
            messageDiv.textContent = text;
            messageDiv.className = 'message ' + type;
            messageDiv.style.display = 'block';
            setTimeout(() => {
                messageDiv.style.display = 'none';
            }, 3000);
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

        // Lade initial die Daten
        loadUserCards();
        loadTrades();

        function getCardImage(cardName) {
            let cardImage = 'default.png';
            if (cardName.toLowerCase().includes('dragon')) cardImage = 'm6.png';
            if (cardName.toLowerCase().includes('knight')) cardImage = 'm4.png';
            if (cardName.toLowerCase().includes('spell')) cardImage = 'm2.png';
            if (cardName.toLowerCase().includes('elf')) cardImage = 'm5.png';
            if (cardName.toLowerCase().includes('goblin')) cardImage = 'm7.png';
            if (cardName.toLowerCase().includes('ork')) cardImage = 'm3.png';
            if (cardName.toLowerCase().includes('kraken')) cardImage = 'm8.png';
            return cardImage;
        }
        function getImageBase64(imageName) {
            const imageMap = {
                'm2.png': '" + _imageCache["m2.png"] + @"',
                'm3.png': '" + _imageCache["m3.png"] + @"',
                'm4.png': '" + _imageCache["m4.png"] + @"',
                'm5.png': '" + _imageCache["m5.png"] + @"',
                'm6.png': '" + _imageCache["m6.png"] + @"',
                'm7.png': '" + _imageCache["m7.png"] + @"',
                'm8.png': '" + _imageCache["m8.png"] + @"',
                'default.png': '" + _imageCache["m1.png"] + @"'
            };
            
            return imageMap[imageName] || imageMap['default.png'];
        }
    </script>
</body>
</html>";
} 