#!/bin/zsh
# Cleanup script for removing problematic URP source files from Lava folder
# These files cause compilation errors and should not be in the Assets folder

set -e  # Exit on error

PROJECT_ROOT="/goinfre/slombard/unityModule00/unityModule00"
LAVA_DIR="$PROJECT_ROOT/Assets/Lava"

echo "====================================="
echo "Unity Lava Scripts Cleanup"
echo "====================================="
echo ""

# Check if Lava directory exists
if [ ! -d "$LAVA_DIR" ]; then
    echo "Error: Lava directory not found at $LAVA_DIR"
    exit 1
fi

echo "Target directory: $LAVA_DIR"
echo ""

# List of problematic C# files to remove
FILES_TO_REMOVE=(
    "AssetVersion.cs"
    "Bloom.cs"
    "MotionBlur.cs"
    "Tonemapping.cs"
    "UniversalAdditionalCameraData.cs"
    "UniversalAdditionalLightData.cs"
    "Vignette.cs"
    "Volume.cs"
    "VolumeProfile.cs"
)

echo "Files to be removed:"
for file in "${FILES_TO_REMOVE[@]}"; do
    if [ -f "$LAVA_DIR/$file" ]; then
        echo "  ✓ $file (exists)"
    else
        echo "  ✗ $file (not found)"
    fi
done
echo ""

# Ask for confirmation
read "REPLY?Proceed with removal? (y/n) "
echo ""

if [[ ! $REPLY =~ ^[Yy]$ ]]; then
    echo "Cleanup cancelled."
    exit 0
fi

# Remove files and their .meta files
REMOVED_COUNT=0
for file in "${FILES_TO_REMOVE[@]}"; do
    if [ -f "$LAVA_DIR/$file" ]; then
        echo "Removing $file..."
        rm "$LAVA_DIR/$file"
        
        # Remove .meta file if it exists
        if [ -f "$LAVA_DIR/$file.meta" ]; then
            rm "$LAVA_DIR/$file.meta"
            echo "  + Removed $file.meta"
        fi
        
        ((REMOVED_COUNT++))
    fi
done

echo ""
echo "====================================="
echo "Cleanup Complete"
echo "====================================="
echo "Removed $REMOVED_COUNT file(s)"
echo ""
echo "Next steps:"
echo "1. Open Unity Editor"
echo "2. Let Unity recompile the project"
echo "3. Check Console for any remaining errors"
echo ""
echo "Note: The Lava assets (materials, textures, shaders) are preserved."
