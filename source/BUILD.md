# Building EZ Social Downloader

Source for the Windows app (C#, WinForms, .NET Framework 4.8).

1. Copy `Microsoft.Web.WebView2.Core.dll` and `Microsoft.Web.WebView2.WinForms.dll`
   from an installed copy (or the setup) into `source/ref/`.
2. `dotnet build -c Release` (works on Windows or Linux with the .NET 8 SDK).
3. Put the built `EZSocialDownloader.exe`, the WebView2 DLLs, `WebView2Loader.dll`,
   `app.ico`, `THIRD-PARTY.txt` in `payload/`, and `yt-dlp.exe`, `ffmpeg.exe`,
   `ffprobe.exe`, `qjs.exe` in `payload/bin/`, plus `wizard.bmp` (NSIS wizard image).
4. `makensis installer.nsi` makes `EZ-Social-Downloader-Setup.exe`.
