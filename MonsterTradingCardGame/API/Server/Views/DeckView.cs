using MonsterTradingCardGame.API.Server.DTOs;

namespace MonsterTradingCardGame.API.Server.Views;

public class DeckView
{
    private readonly string _cssPath;

    public DeckView()
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
    <title>MTCG - Mein Deck</title>
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
        <a href='/shop' class='nav-button'>Shop</a>
        <a href='/stats' class='nav-button'>Statistiken</a>
        <a href='/trading' class='nav-button'>Handel</a>
        <a href='javascript:void(0)' onclick='logout()' class='nav-button'>Ausloggen</a>
    </div>

    <div class='container deck-container'>
        <h1>Mein Deck</h1>
        
        <div class='deck-section'>
            <div id='currentDeck' class='card-grid'>
                <h2>Aktuelles Deck</h2>
                <div id='deckCards' class='cards-container'>
                    <!-- Deck-Karten werden hier dynamisch eingefügt -->
                </div>
            </div>

            <div id='availableCards' class='card-grid'>
                <h2>Verfügbare Karten</h2>
                <div id='cardStack' class='cards-container'>
                    <!-- Verfügbare Karten werden hier dynamisch eingefügt -->
                </div>
            </div>
        </div>

        <button onclick='saveDeck()' class='cta-button'>Deck speichern</button>
        <div id='message' class='message' style='display: none;'></div>
        
        <a href='/' class='back-link'>← Zurück zur Startseite</a>
    </div>

    <script>
        let selectedCards = new Set();
        
        async function loadDeckAndCards() {
            const token = localStorage.getItem('token');
            if (!token) {
                window.location.href = '/login';
                return;
            }

            try {
                // Lade aktuelles Deck
                const deckResponse = await fetch('/deck', {
                    headers: { 'Authorization': 'Bearer ' + token }
                });
                const deckData = await deckResponse.json();

                // Lade alle Karten
                const cardsResponse = await fetch('/cards', {
                    headers: { 'Authorization': 'Bearer ' + token }
                });
                const cardsData = await cardsResponse.json();

                // Aktualisiere UI
                updateDeckDisplay(deckData);
                updateCardStackDisplay(cardsData, deckData);

                // Initialisiere selectedCards mit aktuellem Deck
                selectedCards = new Set(deckData.map(card => card.Id));

            } catch (error) {
                showMessage('Fehler beim Laden der Karten: ' + error, 'error');
            }
        }

        function updateDeckDisplay(deckCards) {
            const deckContainer = document.getElementById('deckCards');
            deckContainer.innerHTML = '';

            deckCards.forEach(card => {
                const cardElement = createCardElement(card, true);
                deckContainer.appendChild(cardElement);
            });
        }

        function updateCardStackDisplay(allCards, deckCards) {
            const stackContainer = document.getElementById('cardStack');
            stackContainer.innerHTML = '';

            const deckCardIds = new Set(deckCards.map(card => card.Id));
            const availableCards = allCards.filter(card => !deckCardIds.has(card.Id));

            availableCards.forEach(card => {
                const cardElement = createCardElement(card, false);
                stackContainer.appendChild(cardElement);
            });
        }

        function createCardElement(card, inDeck) {
            const div = document.createElement('div');
            div.className = 'card' + (selectedCards.has(card.Id) ? ' selected' : '');
            div.onclick = () => toggleCardSelection(card.Id, div);
            
            div.innerHTML = `
                <h3>${card.Name}</h3>
                <p>Schaden: ${card.Damage}</p>
                <p>Element: ${card.Element}</p>
            `;
            
            return div;
        }

        function toggleCardSelection(cardId, element) {
            if (selectedCards.has(cardId)) {
                selectedCards.delete(cardId);
                element.classList.remove('selected');
            } else if (selectedCards.size < 4) {
                selectedCards.add(cardId);
                element.classList.add('selected');
            } else {
                showMessage('Maximal 4 Karten erlaubt!', 'error');
            }
        }

        async function saveDeck() {
            if (selectedCards.size !== 4) {
                showMessage('Bitte wähle genau 4 Karten aus!', 'error');
                return;
            }

            const token = localStorage.getItem('token');
            try {
                const response = await fetch('/deck', {
                    method: 'PUT',
                    headers: {
                        'Content-Type': 'application/json',
                        'Authorization': 'Bearer ' + token
                    },
                    body: JSON.stringify(Array.from(selectedCards))
                });

                if (response.ok) {
                    showMessage('Deck erfolgreich gespeichert!', 'success');
                    loadDeckAndCards();
                } else {
                    showMessage('Fehler beim Speichern des Decks', 'error');
                }
            } catch (error) {
                showMessage('Fehler beim Speichern: ' + error, 'error');
            }
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

        loadDeckAndCards();
    </script>
</body>
</html>";
} 