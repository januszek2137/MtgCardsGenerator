# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

AiMagicCardsGenerator is an ASP.NET Core 10.0 MVC application that generates custom Magic: The Gathering cards using AI. The application imports card data from Scryfall, uses Groq's LLM API to generate new card designs based on existing cards, and renders them as images using ImageSharp.

## Essential Commands

### Building and Running
```bash
# Build the project
dotnet build

# Run the application (development mode with hot reload)
dotnet run

# Run with specific launch profile
dotnet run --launch-profile https
```

The application runs on:
- HTTPS: https://localhost:7073
- HTTP: http://localhost:5041

### Database Migrations
```bash
# Add a new migration
dotnet ef migrations add MigrationName

# Apply migrations to database
dotnet ef database update

# Remove last migration (if not applied)
dotnet ef migrations remove
```

### Testing (when tests are added)
```bash
# Run all tests
dotnet test

# Run tests with detailed output
dotnet test --verbosity detailed
```

## Architecture Overview

### Data Flow Architecture

1. **Card Import Pipeline** (Scryfall → Database)
   - `IScryfallService` downloads bulk card data from Scryfall API
   - `ICardService` orchestrates import and saves to SQLite via `ICardRepository`
   - User triggers via `DataController.Import()`

2. **Card Generation Pipeline** (Database → LLM → Rendered Image)
   - `IGeneratorService` selects random cards by CMC, calls Groq API for new card design
   - Returns `CardGenerationResult` containing generated card and source examples
   - `ICardRenderService` renders the card as PNG using frames, fonts, and mana symbols
   - Optional: `IImageGeneratorService` generates card artwork (placeholder implementation)

3. **Rendering Pipeline** (Card Data → Visual Output)
   - Loads colored frame from `wwwroot/assets/frames/{color}.png`
   - Overlays generated artwork in art box area
   - Draws card name, type line, oracle text with mana symbols, P/T
   - Handles text wrapping, keyword formatting, and symbol rendering from SVG files

### Service Layer Architecture

Services follow dependency injection pattern registered in Program.cs:

- **Repository Layer**: `ICardRepository` (scoped) - EF Core data access
- **External API Services**:
  - `IScryfallService` (HttpClient) - Scryfall API integration
  - `IGeneratorService` (HttpClient) - Groq LLM API calls
  - `IImageGeneratorService` (HttpClient) - Card artwork generation
- **Business Logic**:
  - `ICardService` (scoped) - Card management and import orchestration
  - `ICardRenderService` (scoped) - Image generation with ImageSharp

### Card Entity Model

The `Card` entity (Models/Entities/Card.cs) stores:
- Scryfall metadata: `ScryfallId` (unique indexed)
- Core attributes: `Name`, `ManaCost`, `Cmc`, `TypeLine`, `Colors`
- Game text: `OracleText`, `FlavorText`, `Keywords`
- Creature stats: `Power`, `Toughness`
- Rarity information

Database indexes on: `ScryfallId`, `Name`, `Cmc`, `TypeLine`, `Colors`

### Card Generation Logic

`GeneratorService.GenerateRandomCardAsync()` workflow:
1. Randomly selects target CMC (1-7)
2. Retrieves 5 random existing cards with that CMC from database
3. Builds prompt with examples for Groq API (llama-3.3-70b-versatile)
4. Parses JSON response into new `Card` object
5. Returns result with both generated card and source examples

Key configuration: Groq API key must be set in `appsettings.json` under `Groq:ApiKey`

### Card Rendering System

`CardRenderService` uses SixLabors.ImageSharp with specific requirements:

**Assets Structure** (wwwroot/assets/):
- `frames/*.png` - Card frames by color (W/U/B/R/G/M/C)
- `symbols/*.svg` - Mana and tap symbols referenced by {symbol} syntax
- `fonts/` - MTG fonts: Beleren Small Caps, Matrix-Bold, PlantinMTPro

**Rendering Configuration** (CardRenderConfig.cs):
- Defines precise pixel coordinates for name, type, oracle text, P/T boxes
- Controls font sizes, line heights, text wrapping widths
- Determines art box dimensions and positioning

**Text Processing**:
- Automatically formats keyword abilities (one per line)
- Handles mana symbols in text via `{symbol}` pattern
- Wraps oracle text with proper spacing
- Supports activated ability detection (cost: effect pattern)

### Controller Responsibilities

- `HomeController` - Landing page
- `DataController` - Card import from Scryfall, displays bulk data info
- `CardsController` - Browse existing cards (paginated), view card details
- `GeneratorController` - Generate new cards (JSON or rendered PNG endpoints)

## Configuration Requirements

### Required appsettings.json Configuration

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

Use `appsettings.Development.json` for local development or User Secrets for sensitive data.

### Required Assets

The application expects these assets in `wwwroot/assets/`:
- Card frame images for each color identity
- SVG mana symbol files (named by symbol code: W.svg, U.svg, 1.svg, etc.)
- MTG-specific fonts for authentic card appearance

PowerShell scripts are provided: `download_frames.ps1` and `download_symbols.ps1`

## Database

- **Provider**: SQLite (Microsoft.EntityFrameworkCore.Sqlite 10.0.1)
- **Context**: `ApplicationDbContext` extends IdentityDbContext
- **Database File**: `mtg.db` (with WAL mode: mtg.db-shm, mtg.db-wal)
- **Identity**: ASP.NET Core Identity for user authentication (requires confirmed account)

## Key Dependencies

- **ImageSharp Stack**: SixLabors.ImageSharp (3.1.12), ImageSharp.Drawing (2.1.7), Fonts (2.1.3)
- **Svg**: Svg (3.4.7) for rendering mana symbols
- **EF Core**: 10.0.1 with Sqlite provider
- **ASP.NET Core Identity**: 10.0.1 with EF Core stores

## Common Development Patterns

### Adding New Card Properties

When adding properties to the Card entity:
1. Update `Models/Entities/Card.cs`
2. Create migration: `dotnet ef migrations add AddPropertyName`
3. Update `Models/Dto/ScryfallModels.cs` if importing from Scryfall
4. Modify `CardService.cs` mapping logic
5. Update `CardRenderService.cs` if property needs visual rendering
6. Apply migration: `dotnet ef database update`

### Modifying Card Rendering

Card visual layout is controlled by:
- `CardRenderConfig.cs` - Layout constants (coordinates, sizes)
- `CardRenderService.RenderCardAsync()` - Rendering logic
- Frame templates in `wwwroot/assets/frames/`

To adjust text positioning, modify the corresponding constants (NAME_X, TYPE_Y, etc.)

### Extending Generation Logic

The generation prompt is in `GeneratorService.BuildPrompt()`. Key aspects:
- Examples are filtered by CMC to ensure power-level consistency
- Temperature is 0.8 for creative variation
- Response must be valid JSON matching Card entity structure
- Error handling via JSON extraction from potentially verbose LLM responses

## Notes

- The application stores large binary data (mtg.db is ~237 MB when fully imported)
- Card artwork generation (`IImageGeneratorService`) is a placeholder - implement with your preferred image generation API
- Oracle text rendering handles complex formatting including mana symbols, keywords, and activated abilities
- Color identity is determined from both ManaCost and Colors fields, with special handling for artifacts and lands
