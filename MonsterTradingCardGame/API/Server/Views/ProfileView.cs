using MonsterTradingCardGame.API.Server.DTOs;

namespace MonsterTradingCardGame.API.Server.Views;

public class ProfileView
{
    private readonly string _cssPath;

    public ProfileView()
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
    <title>MTCG - Profil</title>
    <style>
        " + cssContent + @"
    </style>
</head>
<body>
    <div class='container'>
        <h1>Mein Profil</h1>
        <div id='profileInfo'>
            <div class='profile-image-container'>
                <span id='profileImage'>🎮</span>
            </div>
            <div class='profile-name'>
                <h2 id='profileName'>Lade...</h2>
            </div>
            <div class='profile-details'>
                <p><strong>Benutzername:</strong> <span id='username'></span></p>
                <p><strong>Bio:</strong> <span id='userBio'></span></p>
                <p><strong>Münzen:</strong> <span id='userCoins'></span></p>
                <button onclick='toggleEditForm(true)' class='cta-button'>Profil bearbeiten</button>
            </div>
        </div>
        
        <div id='editForm' class='feature-card' style='display: none;'>
            <h2>Profil bearbeiten</h2>
            <form id='updateProfileForm'>
                <div class='form-group'>
                    <label for='name'>Name:</label>
                    <input type='text' id='name' name='name' class='form-control'>
                </div>
                <div class='form-group'>
                    <label for='bio'>Bio:</label>
                    <textarea id='bio' name='bio' class='form-control'></textarea>
                </div>
                <div class='form-group'>
                    <label for='image'>Bild:</label>
                    <input type='text' id='image' name='image' class='form-control' placeholder='🎮'>
                </div>
                <button type='submit' class='cta-button'>Speichern</button>
                <button type='button' onclick='toggleEditForm(false)' class='cta-button secondary'>Abbrechen</button>
            </form>
        </div>

        
        <div class='button-group'>
            <a href='/deck' class='cta-button'>Mein Deck</a>
            <a href='/stats' class='cta-button'>Statistiken</a>
            <a href='/trading' class='cta-button'>Handel</a>
        </div>
        <button onclick='logout()' class='cta-button'>Ausloggen</button>
        <a href='/' class='back-link'>← Zurück zur Startseite</a>
    </div>

    <script>
        function toggleEditForm(show) {
            document.getElementById('editForm').style.display = show ? 'block' : 'none';
        }

        async function loadProfile() {
            const token = localStorage.getItem('token');
            if (!token) {
                window.location.href = '/login';
                return;
            }

            const username = token.split('-')[0];
            try {
                const response = await fetch('/users/' + username, {
                    headers: {
                        'Authorization': 'Bearer ' + token
                    }
                });

                if (response.ok) {
                    const userData = await response.json();
                    document.getElementById('profileImage').textContent = userData.Image || '🎮';
                    document.getElementById('profileName').textContent = userData.Name || userData.Username;
                    document.getElementById('username').textContent = userData.Username;
                    document.getElementById('userBio').textContent = userData.Bio || 'Keine Biografie vorhanden';
                    document.getElementById('userCoins').textContent = userData.Coins;
                    
                    document.getElementById('name').value = userData.Name || '';
                    document.getElementById('bio').value = userData.Bio || '';
                    document.getElementById('image').value = userData.Image || '';
                } else {
                    alert('Fehler beim Laden der Profildaten');
                }
            } catch (error) {
                alert('Fehler beim Laden der Profildaten: ' + error);
            }
        }

        document.getElementById('updateProfileForm').addEventListener('submit', async (e) => {
            e.preventDefault();
            const token = localStorage.getItem('token');
            const username = token.split('-')[0];

            const updateData = {
                Name: document.getElementById('name').value,
                Bio: document.getElementById('bio').value,
                Image: document.getElementById('image').value
            };

            try {
                const response = await fetch('/users/' + username, {
                    method: 'PUT',
                    headers: {
                        'Content-Type': 'application/json',
                        'Authorization': 'Bearer ' + token
                    },
                    body: JSON.stringify(updateData)
                });

                if (response.ok) {
                    alert('Profil erfolgreich aktualisiert');
                    toggleEditForm(false);
                    loadProfile();
                } else {
                    alert('Fehler beim Aktualisieren des Profils');
                }
            } catch (error) {
                alert('Fehler beim Aktualisieren des Profils: ' + error);
            }
        });

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

        loadProfile();
    </script>
</body>
</html>"; 
}