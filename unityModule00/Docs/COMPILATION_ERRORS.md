# Unity Compilation Errors - Lava Asset Scripts

## Issue Summary
The project contains URP (Universal Render Pipeline) source code scripts in the `Assets/Lava/` folder that are causing compilation errors. These scripts were likely imported as part of a Lava shader/material asset pack but should not be in the Assets folder as they conflict with Unity's internal URP package.

## Error Details

### 1. URPHelpURLAttribute Accessibility Errors
**Affected Files:**
- `Assets/Lava/Bloom.cs` (line 141)
- `Assets/Lava/MotionBlur.cs` (line 140)
- `Assets/Lava/Tonemapping.cs` (line 179)
- `Assets/Lava/Vignette.cs` (line 108)
- `Assets/Lava/UniversalAdditionalLightData.cs` (line 33)
- `Assets/Lava/UniversalAdditionalCameraData.cs` (line 441)

**Error Message:**
```
error CS0122: 'URPHelpURLAttribute' is inaccessible due to its protection level
```

**Cause:** `URPHelpURLAttribute` is an internal Unity attribute used only within URP package code. It cannot be used in user scripts.

### 2. MotionVectorsPersistentData Accessibility Errors
**Affected Files:**
- `Assets/Lava/UniversalAdditionalCameraData.cs` (lines 488, 838)

**Error Messages:**
```
error CS0122: 'MotionVectorsPersistentData' is inaccessible due to its protection level
```

**Cause:** `MotionVectorsPersistentData` is an internal URP class that is not accessible outside the URP package assembly.

### 3. Duplicate Player Class Definition
**Affected Files:**
- `Assets/PlayerController.cs`
- `Assets/Lava/PlayerController.cs` (duplicate - already removed)

**Error Message:**
```
error CS0101: The namespace '<global namespace>' already contains a definition for 'Player'
```

**Status:** ✅ Resolved - duplicate file was removed

## Root Cause
The Lava folder contains URP internal source code that should only exist within Unity's `com.unity.render-pipelines.universal` package. These files were accidentally included with the asset pack and should not be in the user's Assets folder.

## Solution Options

### Option 1: Remove All Problematic Scripts (Recommended)
Remove all C# scripts from the Lava folder, keeping only the actual assets (materials, textures, shaders, etc.).

**Files to Remove:**
- `Assets/Lava/AssetVersion.cs`
- `Assets/Lava/Bloom.cs`
- `Assets/Lava/MotionBlur.cs`
- `Assets/Lava/Tonemapping.cs`
- `Assets/Lava/UniversalAdditionalCameraData.cs`
- `Assets/Lava/UniversalAdditionalLightData.cs`
- `Assets/Lava/Vignette.cs`
- `Assets/Lava/Volume.cs`
- `Assets/Lava/VolumeProfile.cs`

### Option 2: Comment Out Problematic Code (Partial Fix)
If the scripts are needed for some reason, comment out the internal attributes and types. However, this may break functionality.

## Recommended Action
Execute the cleanup script to remove all problematic C# files from the Lava folder while preserving the actual shader/material assets.

## Script
A cleanup script is provided: `Scripts/cleanup_lava_scripts.sh`

## Date Reported
November 15, 2025
