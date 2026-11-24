dotnet publish -c Release --self-contained true -p:PublishSingleFile=true -f net10.0
$p = "2.0"
mkdir "bin\Release\net10.0\win-x64\publish\Dino-winx64-v$p" -Force
move -Path "bin\Release\net10.0\win-x64\publish\Dino.exe" -Destination "bin\Release\net10.0\win-x64\publish\Dino-winx64-v$p\Dino.exe" -Force
move -Path "bin\Release\net10.0\win-x64\publish\Assets" -Destination "bin\Release\net10.0\win-x64\publish\Dino-winx64-v$p\Assets" -Force
move -Path "bin\Release\net10.0\win-x64\publish\src\interpreter" -Destination "bin\Release\net10.0\win-x64\publish\Dino-winx64-v$p\interpreter" -Force
