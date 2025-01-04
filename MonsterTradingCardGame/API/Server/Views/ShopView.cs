using MonsterTradingCardGame.API.Server.DTOs;

namespace MonsterTradingCardGame.API.Server.Views;

public class ShopView
{
    private readonly string _cssPath;

    public ShopView()
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
    <title>MTCG - Shop</title>
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
        <a href='/deck' class='nav-button'>Mein Deck</a>
        <a href='/stats' class='nav-button'>Statistiken</a>
        <a href='/trading' class='nav-button'>Handel</a>
        <a href='javascript:void(0)' onclick='logout()' class='nav-button'>Ausloggen</a>
    </div>

    <div class='container shop-container'>
        <h1>Kartenpaket-Shop</h1>
        
        <div class='shop-info'>
            <p>Ein Paket kostet 5 Münzen und enthält 5 zufällige Karten.</p>
            <p>Deine Münzen: <span id='userCoins'>Lade...</span></p>
            <button onclick='buyPackage()' class='cta-button'>Paket kaufen</button>
        </div>

        <div class='package-section'>
            <div class='package-preview'>
                <h2>Verfügbare Pakete</h2>
                <div id='availablePackages' class='packages-container'>
                    <!-- Pakete werden hier dynamisch eingefügt -->
                </div>
            </div>
        </div>

        <div id='message' class='message' style='display: none;'></div>
        <a href='/' class='back-link'>← Zurück zur Startseite</a>
    </div>

    <script>
        async function loadUserCoins() {
            const token = localStorage.getItem('token');
            if (!token) {
                window.location.href = '/login';
                return;
            }

            try {
                const username = token.split('-')[0];
                const response = await fetch('/users/' + username, {
                    headers: {
                        'Authorization': 'Bearer ' + token
                    }
                });
                const userData = await response.json();
                document.getElementById('userCoins').textContent = userData.Coins;
            } catch (error) {
                showMessage('Fehler beim Laden der Münzen: ' + error, 'error');
            }
        }

        async function loadAvailablePackages() {
            const token = localStorage.getItem('token');
            if (!token) {
                window.location.href = '/login';
                return;
            }

            try {
                const response = await fetch('/packages/available', {
                    headers: {
                        'Authorization': 'Bearer ' + token
                    }
                });

                if (response.ok) {
                    const packages = await response.json();
                    const container = document.getElementById('availablePackages');
                    container.innerHTML = '';

                    if (!packages || packages.length === 0) {
                        container.innerHTML = '<p>Aktuell keine Pakete verfügbar.</p>';
                        return;
                    }

                    packages.forEach((pack, index) => {
                        const packageDiv = document.createElement('div');
                        packageDiv.className = 'package-item';
                        packageDiv.innerHTML = `
                            <div class='package-card'>
                                <h3>Paket ${index + 1}</h3>
                                <div class='package-info'>
                                    <p>📦 5 Karten</p>
                                    <p>💰 5 Münzen</p>
                                </div>
                            </div>
                        `;
                        container.appendChild(packageDiv);
                    });
                } else {
                    console.error('Fehler beim Laden der Pakete:', response.status);
                    showMessage('Fehler beim Laden der Pakete', 'error');
                }
            } catch (error) {
                console.error('Fehler beim Laden der Pakete:', error);
                showMessage('Fehler beim Laden der Pakete: ' + error, 'error');
            }
        }

        async function buyPackage() {
            const token = localStorage.getItem('token');
            if (!token) {
                window.location.href = '/login';
                return;
            }

            try {
                const response = await fetch('/transactions/packages', {
                    method: 'POST',
                    headers: {
                        'Authorization': 'Bearer ' + token
                    }
                });

                if (response.ok) {
                    showMessage('Paket erfolgreich gekauft!', 'success');
                    await Promise.all([
                        loadUserCoins(),
                        loadAvailablePackages()
                    ]);
                } else {
                    const errorText = await response.text();
                    showMessage('Fehler beim Kauf: ' + errorText, 'error');
                }
            } catch (error) {
                showMessage('Fehler beim Kauf: ' + error, 'error');
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

        // Lade initial die Münzen
        loadUserCoins();
        loadAvailablePackages();
    </script>
</body>
</html>";
} 