using MonsterTradingCardGame.API.Server.DTOs;

namespace MonsterTradingCardGame.API.Server.Views;

public class RegisterView
{
    private readonly string _cssPath;

    public RegisterView()
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

    private string GenerateHtml(string cssContent) => $@"
        <!DOCTYPE html>
        <html>
        <head>
            <meta charset='UTF-8'>
            <title>MTCG - Registrierung</title>
            <style>
                {cssContent}
            </style>
        </head>
        <body>
            <div class='container'>
                <h1>Registrierung</h1>
                <div class='feature-card'>
                    <form id='registerForm'>
                        <div class='form-group'>
                            <label for='username'>Benutzername</label>
                            <input type='text' id='username' name='username' required>
                        </div>
                        <div class='form-group'>
                            <label for='password'>Passwort</label>
                            <input type='password' id='password' name='password' required>
                        </div>
                        <button type='submit' class='cta-button'>Registrieren</button>
                    </form>
                </div>
                <a href='/' class='back-link'>← Zurück zur Startseite</a>
            </div>

            <script>
                document.getElementById('registerForm').addEventListener('submit', async (e) => {{
                    e.preventDefault();
                    const username = document.getElementById('username').value;
                    const password = document.getElementById('password').value;
                    
                    try {{
                        const response = await fetch('/users', {{
                            method: 'POST',
                            headers: {{
                                'Content-Type': 'application/json'
                            }},
                            body: JSON.stringify({{ Username: username, Password: password }})
                        }});
                        
                        if (response.ok) {{
                            alert('Registrierung erfolgreich!');
                            window.location.href = '/';
                        }} else {{
                            const error = await response.text();
                            alert('Fehler bei der Registrierung: ' + error);
                        }}
                    }} catch (error) {{
                        alert('Fehler bei der Registrierung: ' + error);
                    }}
                }});
            </script>
        </body>
        </html>";
}