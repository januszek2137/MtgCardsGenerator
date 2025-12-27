# 🎴 AI Magic Cards Generator

> Generate custom Magic: The Gathering cards using AI-powered creativity! ✨

An ASP.NET Core MVC application that harnesses the power of Groq's LLM to create unique MTG cards based on existing card data from Scryfall. Generate, visualize, and explore AI-crafted cards with authentic MTG styling.

![.NET](https://img.shields.io/badge/.NET-10.0-512BD4?logo=dotnet)
![License](https://img.shields.io/badge/license-MIT-blue.svg)
![AI Powered](https://img.shields.io/badge/AI-Groq%20LLM-orange)

---

## ✨ Features

🎨 **AI Card Generation** - Leverage Groq's Llama 3.3 70B model to generate balanced, creative MTG cards

📥 **Scryfall Integration** - Import bulk card data directly from Scryfall's comprehensive database

🖼️ **Professional Rendering** - Generate PNG images with authentic MTG fonts, frames, and mana symbols

🎯 **CMC-Based Balance** - Cards are generated based on converted mana cost for power-level consistency

🔍 **Card Browser** - Browse and search through imported cards with pagination

🎭 **Multi-Color Support** - Full support for all color identities (W/U/B/R/G/Multicolor/Colorless)

📊 **Database Storage** - SQLite-based persistence with EF Core for fast querying

---

## 🚀 Quick Start

### Prerequisites

- ✅ [.NET 10.0 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- ✅ [Groq API Key](https://console.groq.com/) (free tier available)
- ✅ SQLite support (included with .NET)

### Installation

1. **Clone the repository**
```bash
git clone https://github.com/yourusername/AiMagicCardsGenerator.git
cd AiMagicCardsGenerator
```

2. **Configure your API key**

Edit `appsettings.Development.json`:
```json
{
  "Groq": {
    "ApiKey": "your-groq-api-key-here"
  }
}
```

Or use .NET User Secrets (recommended):
```bash
dotnet user-secrets set "Groq:ApiKey" "your-groq-api-key-here"
```

3. **Download assets**

Run PowerShell scripts to download card frames and mana symbols:
```powershell
cd wwwroot/assets
./download_frames.ps1
./download_symbols.ps1
```

4. **Apply database migrations**
```bash
dotnet ef database update
```

5. **Run the application**
```bash
dotnet run
```

Visit `https://localhost:7073` 🎉

---

## 🎮 Usage

### Import Card Data 📦

1. Navigate to **Data** section
2. View Scryfall bulk data information
3. Click **Import Cards** to download and populate the database
4. Wait for import completion (~100k+ cards)

### Generate New Cards ✨

**Via Web Interface:**
1. Go to **Generator** page
2. Click **Generate Random Card**
3. View the generated card with its source examples
4. Download as PNG image

**Via API:**
```bash
# Get JSON card data
curl https://localhost:7073/Generator/GenerateJson

# Get rendered PNG image
curl https://localhost:7073/Generator/GenerateImage -o card.png
```

### Browse Cards 🔍

1. Visit **Cards** section
2. Browse paginated card list (20 per page)
3. Click any card to view full details
4. Search and filter capabilities

---

## 🛠️ Tech Stack

**Backend:**
- 🔷 ASP.NET Core 10.0 MVC
- 🗄️ Entity Framework Core with SQLite
- 🔐 ASP.NET Core Identity
- 🌐 HttpClient for external APIs

**AI & APIs:**
- 🤖 [Groq](https://groq.com/) - Llama 3.3 70B LLM
- 🃏 [Scryfall API](https://scryfall.com/docs/api) - MTG card database

**Image Processing:**
- 🎨 SixLabors.ImageSharp - Image manipulation
- ✏️ ImageSharp.Drawing - Text and shape rendering
- 🔤 ImageSharp.Fonts - Custom font rendering
- 📐 Svg - SVG mana symbol rendering

**Frontend:**
- 🎭 Razor Views with Bootstrap 5
- 💅 Custom CSS styling
- ⚡ Vanilla JavaScript

---

## 📂 Project Structure

```
AiMagicCardsGenerator/
├── 🎮 Controllers/          # MVC Controllers
│   ├── HomeController.cs
│   ├── DataController.cs    # Scryfall import
│   ├── CardsController.cs   # Card browsing
│   └── GeneratorController.cs
├── 📊 Models/
│   ├── Entities/            # Domain models
│   │   └── Card.cs
│   └── Dto/                 # Data transfer objects
│       └── ScryfallModels.cs
├── 🗄️ Data/
│   ├── ApplicationDbContext.cs
│   └── Migrations/          # EF Core migrations
├── 🔧 Services/             # Business logic
│   ├── ICardService.cs
│   ├── CardService.cs
│   ├── IGeneratorService.cs
│   ├── GeneratorService.cs
│   ├── IScryfallService.cs
│   ├── ScryfallService.cs
│   ├── ICardRenderService.cs
│   ├── CardRenderService.cs
│   └── CardRenderConfig.cs
├── 📦 Repositories/         # Data access
│   ├── ICardRepository.cs
│   └── CardRepository.cs
├── 🎨 Views/                # Razor templates
├── 🌐 wwwroot/
│   └── assets/
│       ├── frames/          # Card frame images
│       ├── fonts/           # MTG fonts
│       └── symbols/         # Mana symbols (SVG)
└── ⚙️ Program.cs            # Application entry point
```

---

## 🎨 Card Generation Process

1. 🎯 **CMC Selection** - Randomly select target converted mana cost (1-7)
2. 🔍 **Example Gathering** - Fetch 5 random cards with matching CMC from database
3. 🤖 **LLM Prompt** - Send structured prompt to Groq API with examples
4. 📝 **JSON Parsing** - Extract and validate card data from LLM response
5. 🖼️ **Rendering** - Generate card image using ImageSharp
   - Load appropriate color frame
   - Render card name, mana cost, type line
   - Process oracle text with mana symbols
   - Add power/toughness for creatures
6. ✅ **Return Result** - Deliver card data and/or PNG image

---

## ⚙️ Configuration

### Required Settings

**appsettings.json / appsettings.Development.json:**
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "DataSource=mtg.db;Cache=Shared"
  },
  "Groq": {
    "ApiKey": "your-groq-api-key-here"
  }
}
```

### Asset Requirements

The application requires these assets in `wwwroot/assets/`:

- 🖼️ **Card Frames** - PNG images for each color (W/U/B/R/G/M/C)
- 🔤 **Fonts** - MTG-specific fonts (Beleren, Matrix, PlantinMTPro)
- ⚡ **Symbols** - SVG files for mana and tap symbols

Use provided PowerShell scripts to download automatically.

---

## 🔧 Development

### Building

```bash
# Build the project
dotnet build

# Run with hot reload
dotnet watch run
```

### Database Migrations

```bash
# Add new migration
dotnet ef migrations add MigrationName

# Apply migrations
dotnet ef database update

# Remove last migration (if not applied)
dotnet ef migrations remove
```

### Testing

```bash
# Run tests (when available)
dotnet test
```

---

## 📝 API Endpoints

| Endpoint | Method | Description |
|----------|--------|-------------|
| `/` | GET | Landing page |
| `/Data` | GET | View Scryfall bulk data info |
| `/Data/Import` | POST | Import cards from Scryfall |
| `/Cards` | GET | Browse cards (paginated) |
| `/Cards/Details/{id}` | GET | View single card details |
| `/Generator` | GET | Card generation page |
| `/Generator/GenerateJson` | GET | Generate card (JSON response) |
| `/Generator/GenerateImage` | GET | Generate card (PNG image) |

---

## 🤝 Contributing

Contributions are welcome! Here's how you can help:

1. 🍴 Fork the repository
2. 🌿 Create a feature branch (`git checkout -b feature/AmazingFeature`)
3. 💾 Commit your changes (`git commit -m '✨ Add some AmazingFeature'`)
4. 📤 Push to the branch (`git push origin feature/AmazingFeature`)
5. 🎯 Open a Pull Request

---

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

---

## 🙏 Acknowledgments

- 🃏 [Scryfall](https://scryfall.com/) for providing comprehensive MTG card data
- 🤖 [Groq](https://groq.com/) for fast and powerful LLM inference
- 🎨 Wizards of the Coast for creating Magic: The Gathering
- 🖼️ [SixLabors](https://github.com/SixLabors) for ImageSharp library

---

## ⚠️ Disclaimer

This project is a fan-made tool and is not affiliated with, endorsed by, or associated with Wizards of the Coast. Magic: The Gathering is a trademark of Wizards of the Coast LLC. Card frames, fonts, and symbols are used for educational and demonstration purposes only.

---

<div align="center">

**Made with ❤️ and AI**

⭐ Star this repo if you find it useful!

</div>
