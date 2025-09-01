#!/bin/bash
APP_NAME="SmolbeanPlanet"
APP_BUNDLE="Builds/macOS/$APP_NAME.app"
DMG_NAME="$APP_NAME.dmg"
DMG_DIR="Builds/macOS"

# Create a temporary staging directory
STAGE="$DMG_DIR/dmg-staging"
rm -rf "$STAGE"
mkdir -p "$STAGE"

# Copy app bundle
cp -R "$APP_BUNDLE" "$STAGE/"

# Create Applications symlink
ln -s /Applications "$STAGE/Applications"

# Create DMG
hdiutil create -volname "$APP_NAME" -srcfolder "$STAGE" -ov -format UDZO "$DMG_DIR/$DMG_NAME"

echo "Created $DMG_DIR/$DMG_NAME"
