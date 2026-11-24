@echo off
echo Compiling MakeVideoApp (.NET 8)...
dotnet publish -c Release -r win-x64 --self-contained false -p:PublishSingleFile=true -o ./output
echo.
echo Done! Output file is in ./output/MakeVideoApp.exe
echo Please place ffmpeg.exe in the ./output folder before running.
pause
