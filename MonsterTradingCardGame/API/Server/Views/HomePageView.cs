using MonsterTradingCardGame.API.Server.DTOs;

namespace MonsterTradingCardGame.API.Server.Views;

public class HomePageView
{
    private readonly string _imagePath;
    private readonly string _cssPath;

    public HomePageView()
    {
        var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
        _imagePath = Path.Combine(baseDirectory, "Assets", "images", "0_0.png");
        _cssPath = Path.Combine(baseDirectory, "Assets", "css", "style.css");

        // Überprüfen, ob die Dateien existieren
        if (!File.Exists(_imagePath))
        {
            throw new FileNotFoundException($"Bilddatei nicht gefunden: {_imagePath}");
        }

        if (!File.Exists(_cssPath))
        {
            throw new FileNotFoundException($"CSS-Datei nicht gefunden: {_cssPath}");
        }
    }

    public Response Render()
    {
        string imageBase64 = Convert.ToBase64String(File.ReadAllBytes(_imagePath));
        string cssContent = File.ReadAllText(_cssPath);

        return new Response(
            200,
            GenerateHtml(imageBase64, cssContent),
            "text/html"
        );
    }

    private string GenerateHtml(string imageBase64, string cssContent) => $@"
        <!DOCTYPE html>
        <html>
        <head>
            <meta charset='UTF-8'>
            <title>Monster Trading Card Game</title>
            <style>
                {cssContent}
            </style>
        </head>
        <body>
            <div class='container'>
                <h1>Monster Trading Card Game</h1>
                
                <div class='logo-container'>
                    <img src='data:image/png;base64,{imageBase64}' alt='MTCG Logo'>
                </div>

                <div class='button-group'>
                    <a href='/register' class='cta-button'>Jetzt Registrieren</a>
                    <a href='/login' class='cta-button'>Login</a>
                </div>

                <div class='homepage-features'>
                    <div class='card game-feature-card'>
                        <h3>🎮 Spannendes Gameplay</h3>
                        <p>Erlebe epische Kartenduelle mit einzigartigen Monster- und Zauberkarten!</p>
                    </div>
                    <div class='card game-feature-card'>
                        <h3>🏆 Wettbewerb</h3>
                        <p>Tritt gegen andere Spieler an und steige in der Rangliste auf!</p>
                    </div>
                    <div class='card game-feature-card'>
                        <h3>💎 Seltene Karten</h3>
                        <p>Sammle und handle mit wertvollen Karten, um dein Deck zu verstärken.</p>
                    </div>
                </div>

            </div>
        </body>
        </html>";
}