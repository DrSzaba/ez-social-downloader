; EZ Social Downloader - per-user installer (no admin needed).
; Mirrors the 1.1.0 installer: same folder, shortcuts and uninstall entry.
Unicode true
SetCompressor /SOLID lzma

!define APPNAME "EZ Social Downloader"
!define VERSION "1.2.0"
!define EXE     "EZSocialDownloader.exe"
!define UNKEY   "Software\Microsoft\Windows\CurrentVersion\Uninstall\EZSocialDownloader"

!include "MUI2.nsh"

Name "${APPNAME}"
OutFile "EZ-Social-Downloader-Setup.exe"
InstallDir "$LOCALAPPDATA\Programs\${APPNAME}"
RequestExecutionLevel user
BrandingText "${APPNAME} ${VERSION}  -  El Szaba"
VIProductVersion "1.2.0.0"
VIAddVersionKey "ProductName" "${APPNAME}"
VIAddVersionKey "FileVersion" "${VERSION}"
VIAddVersionKey "ProductVersion" "${VERSION}"
VIAddVersionKey "CompanyName" "El Szaba"
VIAddVersionKey "FileDescription" "${APPNAME} Setup"
VIAddVersionKey "LegalCopyright" "Dr. Szaba"

!define MUI_ICON "payload\app.ico"
!define MUI_UNICON "payload\app.ico"
!define MUI_WELCOMEFINISHPAGE_BITMAP "wizard.bmp"
!define MUI_WELCOMEPAGE_TITLE "Welcome to ${APPNAME} ${VERSION}"
!define MUI_WELCOMEPAGE_TEXT "Download videos from TikTok, Instagram Reels, Facebook Reels, YouTube, YouTube Shorts and X as clean MP4s - or just the MP3, or just the transcript / lyrics.$\r$\n$\r$\nEverything is included. Nothing else to install.$\r$\n$\r$\nClick Install to begin."
!define MUI_FINISHPAGE_RUN "$INSTDIR\${EXE}"
!define MUI_FINISHPAGE_RUN_TEXT "Open ${APPNAME} now"

!insertmacro MUI_PAGE_WELCOME
!insertmacro MUI_PAGE_INSTFILES
!insertmacro MUI_PAGE_FINISH
!insertmacro MUI_UNPAGE_CONFIRM
!insertmacro MUI_UNPAGE_INSTFILES
!insertmacro MUI_LANGUAGE "English"

Section "Install"
  ; close a running copy so its files can be replaced
  nsExec::Exec 'taskkill /F /IM ${EXE}'
  Pop $0
  Sleep 500
  SetOverwrite on
  SetOutPath "$INSTDIR"
  File "payload\${EXE}"
  File "payload\app.ico"
  File "payload\THIRD-PARTY.txt"
  File "payload\Microsoft.Web.WebView2.Core.dll"
  File "payload\Microsoft.Web.WebView2.WinForms.dll"
  File "payload\WebView2Loader.dll"
  SetOutPath "$INSTDIR\bin"
  File "payload\bin\yt-dlp.exe"
  File "payload\bin\ffmpeg.exe"
  File "payload\bin\ffprobe.exe"
  File "payload\bin\qjs.exe"
  Delete "$INSTDIR\bin\node.exe"
  ; use the fresh engine from this setup (it updates itself after that)
  Delete "$LOCALAPPDATA\EZ Social Downloader\engine\yt-dlp.exe"
  SetOutPath "$INSTDIR"

  WriteUninstaller "$INSTDIR\Uninstall.exe"
  CreateShortcut "$DESKTOP\${APPNAME}.lnk" "$INSTDIR\${EXE}" "" "$INSTDIR\app.ico"
  CreateDirectory "$SMPROGRAMS\${APPNAME}"
  CreateShortcut "$SMPROGRAMS\${APPNAME}\${APPNAME}.lnk" "$INSTDIR\${EXE}" "" "$INSTDIR\app.ico"
  CreateShortcut "$SMPROGRAMS\${APPNAME}\Uninstall ${APPNAME}.lnk" "$INSTDIR\Uninstall.exe"

  WriteRegStr HKCU "${UNKEY}" "DisplayName" "${APPNAME}"
  WriteRegStr HKCU "${UNKEY}" "DisplayVersion" "${VERSION}"
  WriteRegStr HKCU "${UNKEY}" "Publisher" "El Szaba"
  WriteRegStr HKCU "${UNKEY}" "DisplayIcon" "$INSTDIR\app.ico"
  WriteRegStr HKCU "${UNKEY}" "InstallLocation" "$INSTDIR"
  WriteRegStr HKCU "${UNKEY}" "UninstallString" '"$INSTDIR\Uninstall.exe"'
  WriteRegDWORD HKCU "${UNKEY}" "NoModify" 1
  WriteRegDWORD HKCU "${UNKEY}" "NoRepair" 1
SectionEnd

Section "Uninstall"
  nsExec::Exec 'taskkill /F /IM ${EXE}'
  Pop $0
  Delete "$DESKTOP\${APPNAME}.lnk"
  RMDir /r "$SMPROGRAMS\${APPNAME}"
  RMDir /r "$INSTDIR\bin"
  Delete "$INSTDIR\${EXE}"
  Delete "$INSTDIR\app.ico"
  Delete "$INSTDIR\THIRD-PARTY.txt"
  Delete "$INSTDIR\Microsoft.Web.WebView2.Core.dll"
  Delete "$INSTDIR\Microsoft.Web.WebView2.WinForms.dll"
  Delete "$INSTDIR\WebView2Loader.dll"
  Delete "$INSTDIR\Uninstall.exe"
  RMDir "$INSTDIR"
  DeleteRegKey HKCU "${UNKEY}"
SectionEnd
