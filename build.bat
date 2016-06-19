PowerShell -NoProfile -ExecutionPolicy Bypass -Command "& '.\build.ps1' -tp Test -tv '0.5.*'"
"C:\Program Files\nuget\nuget.exe" pack CrashReport.Client.csproj -prop Configuration=Release
pause