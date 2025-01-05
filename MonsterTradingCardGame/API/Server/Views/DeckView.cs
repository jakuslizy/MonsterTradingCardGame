using MonsterTradingCardGame.API.Server.DTOs;

namespace MonsterTradingCardGame.API.Server.Views;

public class DeckView
{
    private readonly string _cssPath;
    private readonly Dictionary<string, string> _imageCache;

    public DeckView()
    {
        var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
        _cssPath = Path.Combine(baseDirectory, "Assets", "css", "style.css");
        
        // Initialisiere Image Cache
        _imageCache = new Dictionary<string, string>();
        var imagesPath = Path.Combine(baseDirectory, "Assets", "images");
        
        // Lade alle Bilder in den Cache
        foreach (var imagePath in Directory.GetFiles(imagesPath, "*.png"))
        {
            var fileName = Path.GetFileName(imagePath);
            _imageCache[fileName] = Convert.ToBase64String(File.ReadAllBytes(imagePath));
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
        <a href='/stats.html' class='nav-button'>Statistiken</a>
        <a href='/trading' class='nav-button'>Handel</a>
        <a href='javascript:void(0)' onclick='logout()' class='nav-button'>Ausloggen</a>
    </div>

    <div class='container'>
        <h1>Mein Deck</h1>
        
        <div class='card-grid'>
            <div id='deckCards' class='deck-cards-container'>
                <!-- Karten werden hier dynamisch eingefügt -->
            </div>
        </div>

        <div class='card-grid'>
            <h2>Verfügbare Karten</h2>
            <div id='availableCards' class='deck-cards-container'>
                <!-- Verfügbare Karten werden hier dynamisch eingefügt -->
            </div>
        </div>

        <button onclick='saveDeck()' class='cta-button'>Deck speichern</button>
        <div id='message' class='message' style='display: none;'></div>
        
        <a href='/' class='back-link'>← Zurück zur Startseite</a>
    </div>

    <script>
        let selectedCards = new Set();
        
        async function loadCards() {
            const token = localStorage.getItem('token');
            if (!token) {
                window.location.href = '/login';
                return;
            }

            try {
                // Lade alle Karten des Users
                const cardsResponse = await fetch('/cards', {
                    headers: {
                        'Authorization': 'Bearer ' + token
                    }
                });
                
                // Lade aktuelles Deck
                const deckResponse = await fetch('/deck', {
                    headers: {
                        'Authorization': 'Bearer ' + token
                    }
                });

                if (cardsResponse.ok && deckResponse.ok) {
                    const allCards = await cardsResponse.json();
                    const deckCards = await deckResponse.json();
                    
                    // Setze die ausgewählten Karten
                    selectedCards = new Set(deckCards.map(card => card.Id));
                    
                    // Zeige Deck-Karten
                    const deckContainer = document.getElementById('deckCards');
                    deckContainer.innerHTML = '';
                    deckCards.forEach(card => {
                        deckContainer.appendChild(createCardElement(card, true));
                    });
                    
                    // Zeige verfügbare Karten
                    const availableContainer = document.getElementById('availableCards');
                    availableContainer.innerHTML = '';
                    const availableCards = allCards.filter(card => !selectedCards.has(card.Id));
                    availableCards.forEach(card => {
                        availableContainer.appendChild(createCardElement(card, false));
                    });
                }
            } catch (error) {
                showMessage('Fehler beim Laden der Karten: ' + error, 'error');
            }
        }

        function createCardElement(card, inDeck) {
            const cardDiv = document.createElement('div');
            cardDiv.className = 'deck-card' + (inDeck ? ' selected' : '');
            cardDiv.onclick = () => toggleCard(card.Id, cardDiv);
            
            // Bestimme das passende Bild basierend auf dem Kartennamen
            let cardImage = 'default.png'; // Standard-Bild
            if (card.Name.toLowerCase().includes('dragon')) cardImage = 'm6.png';
            if (card.Name.toLowerCase().includes('knight')) cardImage = 'm4.png';
            if (card.Name.toLowerCase().includes('spell')) cardImage = 'm2.png';
            if (card.Name.toLowerCase().includes('elf')) cardImage = 'm5.png';
            if (card.Name.toLowerCase().includes('goblin')) cardImage = 'm7.png';
            if (card.Name.toLowerCase().includes('ork')) cardImage = 'm3.png';
            if (card.Name.toLowerCase().includes('kraken')) cardImage = 'm8.png';
            
            cardDiv.innerHTML = `
                <img src='data:image/png;base64,${getImageBase64(cardImage)}' class='card-image' alt='${card.Name}'>
                <h3>${card.Name}</h3>
                <p>Schaden: ${card.Damage}</p>
                <p>Element: ${card.ElementType || 'Normal'}</p>
            `;
            return cardDiv;
        }

        function toggleCard(cardId, cardElement) {
            if (selectedCards.has(cardId)) {
                selectedCards.delete(cardId);
                cardElement.classList.remove('selected');
            } else if (selectedCards.size < 4) {
                selectedCards.add(cardId);
                cardElement.classList.add('selected');
            } else {
                showMessage('Maximal 4 Karten im Deck erlaubt!', 'error');
            }
        }

        async function saveDeck() {
            const token = localStorage.getItem('token');
            if (!token) {
                window.location.href = '/login';
                return;
            }

            if (selectedCards.size !== 4) {
                showMessage('Das Deck muss genau 4 Karten enthalten!', 'error');
                return;
            }

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
                    loadCards();
                } else {
                    showMessage('Fehler beim Speichern des Decks', 'error');
                }
            } catch (error) {
                showMessage('Fehler beim Speichern des Decks: ' + error, 'error');
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

        // Lade initial die Karten
        loadCards();
    </script>
</body>
</html>";
}