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

[Files]
Source: "Builds\Windows\SmolbeanPlanet.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "Builds\Windows\SmolbeanPlanet_Data\*"; DestDir: "{app}\SmolbeanPlanet_Data"; Flags: ignoreversion recursesubdirs createallsubdirs

[Icons]
Name: "{group}\Smolbean Planet"; Filename: "{app}\SmolbeanPlanet.exe"
Name: "{commondesktop}\Smolbean Planet"; Filename: "{app}\SmolbeanPlanet.exe"; Tasks: desktopicon

[Tasks]
Name: "desktopicon"; Description: "Create a &desktop icon"; GroupDescription: "Additional icons:"; Flags: unchecked
