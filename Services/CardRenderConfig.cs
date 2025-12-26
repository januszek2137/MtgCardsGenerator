namespace AiMagicCardsGenerator.Services;

public static class CardRenderConfig
{
    // Card name (top bar)
    public const int NAME_X         = 115;
    public const int NAME_Y         = 90;
    public const int NAME_FONT_SIZE = 72;

    // Type line (above text box)
    public const int TYPE_X         = 152;
    public const int TYPE_Y         = 1287;
    public const int TYPE_FONT_SIZE = 48;

    // Oracle text (inside text box)
    public const int ORACLE_X           = 152;
    public const int ORACLE_Y           = 1387;
    public const int ORACLE_WIDTH       = 1195;
    public const int ORACLE_FONT_SIZE   = 44;
    public const int ORACLE_LINE_HEIGHT = 52;
    public const int ORACLE_SYMBOL_SIZE = 34;
    public const int ORACLE_SPACE_WIDTH = 12;

    // Flavor text
    public const int FLAVOR_X         = 152;
    public const int FLAVOR_Y         = 1750;
    public const int FLAVOR_FONT_SIZE = 40;

    // Power/Toughness
    public const int PT_X         = 1260;
    public const int PT_Y         = 1880;
    public const int PT_FONT_SIZE = 85;

    // Mana cost symbols (top right)
    public const int MANA_START_X     = 1400;
    public const int MANA_Y           = 90;
    public const int MANA_SYMBOL_SIZE = 72;
    public const int MANA_SPACING     = 10;
    
    // Card art
    public const int ART_X      = 0;
    public const int ART_Y      = 152;
    public const int ART_WIDTH  = 1500;
    public const int ART_HEIGHT = 1100;
}