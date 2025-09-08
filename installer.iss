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
; --- Main EXE
Source: "Builds\Windows\SmolbeanPlanet.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "Builds\Windows\UnityPlayer.dll"; DestDir: "{app}"; Flags: ignoreversion

; --- Core player DLLs
; UnityPlayer.dll exists for both Mono and IL2CPP players
Source: "Builds\Windows\UnityPlayer.dll"; DestDir: "{app}"; Flags: ignoreversion skipifsourcedoesntexist
; GameAssembly.dll is IL2CPP-only
Source: "Builds\Windows\GameAssembly.dll"; DestDir: "{app}"; Flags: ignoreversion skipifsourcedoesntexist

; --- Optional extras Unity sometimes emits
Source: "Builds\Windows\UnityCrashHandler64.exe"; DestDir: "{app}"; Flags: ignoreversion skipifsourcedoesntexist
Source: "Builds\Windows\WinPixEventRuntime.dll"; DestDir: "{app}"; Flags: ignoreversion skipifsourcedoesntexist

; --- Game data folder (required for both Mono and IL2CPP)
Source: "Builds\Windows\SmolbeanPlanet_Data\*"; DestDir: "{app}\SmolbeanPlanet_Data"; Flags: ignoreversion recursesubdirs createallsubdirs

; --- Mono runtime 
Source: "Builds\Windows\MonoBleedingEdge\*"; DestDir: "{app}\MonoBleedingEdge"; Flags: ignoreversion recursesubdirs createallsubdirs skipifsourcedoesntexist


[Icons]
Name: "{group}\Smolbean Planet"; Filename: "{app}\SmolbeanPlanet.exe"
Name: "{commondesktop}\Smolbean Planet"; Filename: "{app}\SmolbeanPlanet.exe"; Tasks: desktopicon

[Tasks]
Name: "desktopicon"; Description: "Create a &desktop icon"; GroupDescription: "Additional icons:"; Flags: unchecked

[Run]
; Optional: auto-launch after install (kept off by default—remove 'skipifsilent' if you want it to run in silent installs)
Filename: "{app}\SmolbeanPlanet.exe"; Description: "Launch Smolbean Planet"; Flags: nowait postinstall skipifsilent
