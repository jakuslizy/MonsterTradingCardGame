using MonsterTradingCardGame.API.Server.DTOs;

namespace MonsterTradingCardGame.API.Server.Views;

public class LoginView
{
    private readonly string _cssPath;

    public LoginView()
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
            <title>MTCG - Login</title>
            <style>
                {cssContent}
            </style>
        </head>
        <body>
            <div class='container'>
                <h1>Login</h1>
                <div class='feature-card'>
                    <form id='loginForm'>
                        <div class='form-group'>
                            <label for='username'>Benutzername</label>
                            <input type='text' id='username' name='username' required>
                        </div>
                        <div class='form-group'>
                            <label for='password'>Passwort</label>
                            <input type='password' id='password' name='password' required>
                        </div>
                        <button type='submit' class='cta-button'>Einloggen</button>
                    </form>
                </div>
                <a href='/' class='back-link'>← Zurück zur Startseite</a>
            </div>

            <script>
                document.getElementById('loginForm').addEventListener('submit', async (e) => {{
                    e.preventDefault();
                    const username = document.getElementById('username').value;
                    const password = document.getElementById('password').value;
                    
                    try {{
                        const response = await fetch('/sessions', {{
                            method: 'POST',
                            headers: {{
                                'Content-Type': 'application/json'
                            }},
                            body: JSON.stringify({{ Username: username, Password: password }})
                        }});
                        
                        if (response.ok) {{
                            const token = await response.text();
                            localStorage.setItem('token', token);
                            alert('Login erfolgreich!');
                            window.location.href = '/';
                        }} else {{
                            const error = await response.text();
                            alert('Fehler beim Login: ' + error);
                        }}
                    }} catch (error) {{
                        alert('Fehler beim Login: ' + error);
                    }}
                }});
            </script>
        </body>
        </html>";
} 