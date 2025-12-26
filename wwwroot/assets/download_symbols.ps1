$symbols = @(
    "W", "U", "B", "R", "G", "C",
    "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "10",
    "11", "12", "13", "14", "15", "16", "17", "18", "19", "20",
    "X", "Y", "Z",
    "T", "Q",
    "WU", "WB", "UB", "UR", "BR", "BG", "RG", "RW", "GW", "GU",
    "2W", "2U", "2B", "2R", "2G",
    "WP", "UP", "BP", "RP", "GP",
    "S", "E"
)

$outputPath = "/symbols"
New-Item -ItemType Directory -Force -Path $outputPath | Out-Null

foreach ($symbol in $symbols) {
    $url = "https://svgs.scryfall.io/card-symbols/$symbol.svg"
    $file = "$outputPath/$symbol.svg"
    
    if (Test-Path $file) {
        Write-Host "Skipping: $symbol.svg (exists)"
        continue
    }
    
    try {
        Invoke-WebRequest -Uri $url -OutFile $file
        Write-Host "Downloaded: $symbol.svg"
    }
    catch {
        Write-Host "Failed: $symbol.svg"
    }
}

Write-Host "Done!"