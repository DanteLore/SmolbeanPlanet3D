; Inno Setup Script for Smolbean Planet
[Setup]
AppName=Smolbean Planet
AppVersion=1.0.0
AppPublisher=DanteLore Games
DefaultDirName={pf}\Smolbean Planet
DefaultGroupName=Smolbean Planet
OutputBaseFilename=SmolbeanPlanetSetup
Compression=lzma
SolidCompression=yes
; Ensure 64-bit install path on 64-bit Windows
ArchitecturesInstallIn64BitMode=x64

[Files]
; Main EXE
Source: "Builds\Windows\SmolbeanPlanet.exe"; DestDir: "{app}"; Flags: ignoreversion

; Critical DLL
Source: "Builds\Windows\UnityPlayer.dll"; DestDir: "{app}"; Flags: ignoreversion

; Game data folder
Source: "Builds\Windows\SmolbeanPlanet_Data\*"; DestDir: "{app}\SmolbeanPlanet_Data"; Flags: ignoreversion recursesubdirs createallsubdirs

; Mono runtime folder 
Source: "Builds\Windows\MonoBleedingEdge\*"; DestDir: "{app}\MonoBleedingEdge"; Flags: ignoreversion recursesubdirs createallsubdirs

[Icons]
Name: "{group}\Smolbean Planet"; Filename: "{app}\SmolbeanPlanet.exe"
Name: "{commondesktop}\Smolbean Planet"; Filename: "{app}\SmolbeanPlanet.exe"; Tasks: desktopicon

[Tasks]
Name: "desktopicon"; Description: "Create a &desktop icon"; GroupDescription: "Additional icons:"; Flags: unchecked

[Run]
; Optional: auto-launch after install (kept off by default—remove 'skipifsilent' if you want it to run in silent installs)
Filename: "{app}\SmolbeanPlanet.exe"; Description: "Launch Smolbean Planet"; Flags: nowait postinstall skipifsilent
