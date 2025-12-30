# 🎴 AI Magic Cards Generator

> Generate custom Magic: The Gathering cards using AI-powered creativity! ✨

An ASP.NET Core MVC application that harnesses the power of Groq's LLM to create unique MTG cards based on existing card data from Scryfall. Generate, visualize, and explore AI-crafted cards with authentic MTG styling.

![.NET](https://img.shields.io/badge/.NET-10.0-512BD4?logo=dotnet)
![License](https://img.shields.io/badge/license-MIT-blue.svg)
![AI Powered](https://img.shields.io/badge/AI-Groq%20LLM-orange)

---

## ✨ Features

🎨 **AI Card Generation** - Leverage Groq's Llama 3.3 70B model to generate balanced, creative MTG cards

🖼️ **AI Card Artwork** - Generate custom card artwork using HuggingFace's Stable Diffusion XL

📥 **Scryfall Integration** - Import bulk card data directly from Scryfall's comprehensive database

🎴 **Card Sharing & Gallery** - Share your generated cards in a public gallery for others to see

❤️ **Like System** - Like your favorite cards with real-time updates via Server-Sent Events

💾 **Card Persistence** - All generated cards saved to database with image data and metadata

🔐 **User Authentication** - ASP.NET Core Identity integration for card ownership and likes

🎯 **CMC-Based Balance** - Cards are generated based on converted mana cost for power-level consistency

🔍 **Card Browser** - Browse and search through imported cards with pagination

🎭 **Multi-Color Support** - Full support for all color identities (W/U/B/R/G/Multicolor/Colorless)

⏱️ **Rate Limiting** - Built-in rate limiting to prevent API abuse on card generation

📊 **Database Storage** - SQLite-based persistence with EF Core for fast querying

---

## 🚀 Quick Start

### Prerequisites

- ✅ [.NET 10.0 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- ✅ [Groq API Key](https://console.groq.com/) (free tier available)
- ✅ [HuggingFace API Key](https://huggingface.co/settings/tokens) (for card artwork generation)
- ✅ SQLite support (included with .NET)

### Installation

1. **Clone the repository**
```bash
git clone https://github.com/yourusername/AiMagicCardsGenerator.git
cd AiMagicCardsGenerator
```

2. **Configure your API keys**

Edit `appsettings.Development.json`:
```json
{
  "Groq": {
    "ApiKey": "your-groq-api-key-here"
  },
  "HuggingFace": {
    "ApiKey": "your-huggingface-api-key-here"
  }
}
```

Or use .NET User Secrets (recommended):
```bash
dotnet user-secrets set "Groq:ApiKey" "your-groq-api-key-here"
dotnet user-secrets set "HuggingFace:ApiKey" "your-huggingface-api-key-here"
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
3. View the generated card on the result page
4. **Share** your card to the public gallery
5. **Download** as PNG image
6. Rate-limited to prevent abuse

### Browse and Like Shared Cards ❤️

1. Visit the **Home** page to see the gallery of shared cards
2. View recently shared cards and top-liked cards
3. **Sign in** to like cards (ASP.NET Core Identity)
4. Real-time like count updates via Server-Sent Events

### Browse Imported Cards 🔍

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
- 🤖 [Groq](https://groq.com/) - Llama 3.3 70B LLM for card design
- 🎨 [HuggingFace](https://huggingface.co/) - Stable Diffusion XL for card artwork
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
- 📡 Server-Sent Events (SSE) for real-time like updates

---

## 📂 Project Structure

```
AiMagicCardsGenerator/
├── 🎮 Controllers/          # MVC Controllers
│   ├── HomeController.cs    # Gallery with likes & SSE
│   ├── DataController.cs    # Scryfall import
│   ├── CardsController.cs   # Card browsing
│   └── GeneratorController.cs # Generation & sharing
├── 📊 Models/
│   ├── Entities/            # Domain models
│   │   ├── Card.cs          # Imported Scryfall cards
│   │   ├── GeneratedCard.cs # User-generated cards
│   │   └── CardLike.cs      # Like relationships
│   └── Dto/                 # Data transfer objects
│       └── ScryfallModels.cs
├── 🗄️ Data/
│   ├── ApplicationDbContext.cs
│   └── Migrations/          # EF Core migrations
├── 🔧 Services/             # Business logic
│   ├── Interfaces/
│   │   ├── ICardService.cs
│   │   ├── IGeneratorService.cs
│   │   ├── IScryfallService.cs
│   │   ├── ICardRenderService.cs
│   │   ├── IImageGeneratorService.cs
│   │   ├── ICardLikeService.cs
│   │   └── ILikesBroadcastService.cs
│   ├── CardService.cs
│   ├── GeneratorService.cs
│   ├── ScryfallService.cs
│   ├── CardRenderService.cs
│   ├── ImageGeneratorService.cs # HuggingFace integration
│   ├── CardLikeService.cs
│   ├── LikesBroadcastService.cs # SSE broadcasting
│   └── CardRenderConfig.cs
├── 📦 Repositories/         # Data access
│   ├── Interfaces/
│   │   ├── ICardRepository.cs
│   │   ├── IGeneratedCardRepository.cs
│   │   └── ICardLikeRepository.cs
│   ├── CardRepository.cs
│   ├── GeneratedCardRepository.cs
│   └── CardLikeRepository.cs
├── 🎨 Views/                # Razor templates
├── 🔐 Areas/Identity/       # ASP.NET Core Identity pages
├── 🧪 AiMagicCardsGenerator.Tests/ # Unit tests
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
5. 🎨 **Artwork Generation** - Generate custom artwork via HuggingFace Stable Diffusion
6. 🖼️ **Card Rendering** - Generate card image using ImageSharp
   - Load appropriate color frame
   - Overlay generated artwork in art box
   - Render card name, mana cost, type line
   - Process oracle text with mana symbols
   - Add power/toughness for creatures
7. 💾 **Database Storage** - Save generated card with image data and metadata
8. ✅ **Return Result** - Redirect to result page with share and download options

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
  },
  "HuggingFace": {
    "ApiKey": "your-huggingface-api-key-here"
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
# Run all tests
dotnet test

# Run specific test project
dotnet test AiMagicCardsGenerator.Tests/AiMagicCardsGenerator.Tests.csproj

# Run with detailed output
dotnet test --verbosity detailed
```

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
