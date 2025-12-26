$frames = @{
    "W" = "https://raw.githubusercontent.com/MrTeferi/Proxyshop/master/templates/Normal/white.png"
    "U" = "https://raw.githubusercontent.com/MrTeferi/Proxyshop/master/templates/Normal/blue.png"
    "B" = "https://raw.githubusercontent.com/MrTeferi/Proxyshop/master/templates/Normal/black.png"
    "R" = "https://raw.githubusercontent.com/MrTeferi/Proxyshop/master/templates/Normal/red.png"
    "G" = "https://raw.githubusercontent.com/MrTeferi/Proxyshop/master/templates/Normal/green.png"
    "C" = "https://raw.githubusercontent.com/MrTeferi/Proxyshop/master/templates/Normal/colorless.png"
    "M" = "https://raw.githubusercontent.com/MrTeferi/Proxyshop/master/templates/Normal/gold.png"
    "A" = "https://raw.githubusercontent.com/MrTeferi/Proxyshop/master/templates/Normal/artifact.png"
    "L" = "https://raw.githubusercontent.com/MrTeferi/Proxyshop/master/templates/Normal/land.png"
}

$outputPath = "wwwroot/assets/frames"
New-Item -ItemType Directory -Force -Path $outputPath | Out-Null

foreach ($frame in $frames.GetEnumerator()) {
    $file = "$outputPath/$($frame.Key).png"
    
    if (Test-Path $file) {
        Write-Host "Skipping: $($frame.Key).png (exists)"
        continue
    }
    
    try {
        Invoke-WebRequest -Uri $frame.Value -OutFile $file
        Write-Host "Downloaded: $($frame.Key).png"
    }
    catch {
        Write-Host "Failed: $($frame.Key).png - $($_.Exception.Message)"
    }
}

Write-Host "Done!"