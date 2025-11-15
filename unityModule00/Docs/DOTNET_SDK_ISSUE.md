# .NET SDK Not Found Issue

## Problem Description

The .NET SDK cannot be located on the system, causing debugging functionality to be disabled.

## Error Messages

```
The .NET SDK cannot be located: Error running dotnet --info: Error: Command failed: dotnet --info
/bin/sh: 1: dotnet: not found

/bin/sh: 1: dotnet: not found
```

## Impact

- .NET debugging will not be enabled
- C# IntelliSense and code analysis may be limited
- Unity C# script compilation relies on Mono/Unity's built-in compiler, but VS Code extensions require .NET SDK

## Root Cause

The `dotnet` command is not available in the system PATH. This typically means:
1. .NET SDK is not installed on the system
2. .NET SDK is installed but not added to PATH
3. The system is using a non-standard .NET installation location

## Solution Options

### Option 1: Install .NET SDK (Recommended for Development)

**For Ubuntu/Debian:**
```bash
# Add Microsoft package repository
wget https://packages.microsoft.com/config/ubuntu/$(lsb_release -rs)/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
sudo dpkg -i packages-microsoft-prod.deb
rm packages-microsoft-prod.deb

# Update package list and install .NET SDK
sudo apt-get update
sudo apt-get install -y dotnet-sdk-8.0
```

**For macOS:**
```bash
# Using Homebrew
brew install --cask dotnet-sdk
```

**For other systems:**
Visit: https://dotnet.microsoft.com/download

### Option 2: Add Existing .NET SDK to PATH

If .NET SDK is already installed in a custom location:

```bash
# Find dotnet installation
find / -name "dotnet" -type f 2>/dev/null

# Add to PATH (add to ~/.bashrc or ~/.zshrc)
export PATH="$PATH:/path/to/dotnet"
```

### Option 3: Disable .NET SDK Check (Quick Fix)

If you don't need .NET debugging features and only want Unity's built-in C# compilation:

1. Open VS Code Settings (Ctrl+,)
2. Search for "omnisharp"
3. Disable or configure OmniSharp C# extension
4. Or uninstall the C# extension if not needed

## Verification

After installing/configuring .NET SDK, verify it works:

```bash
# Check .NET SDK version
dotnet --version

# Check .NET SDK info
dotnet --info

# List installed SDKs
dotnet --list-sdks
```

## Notes

- Unity projects don't strictly require .NET SDK for compilation (Unity uses Mono)
- .NET SDK is primarily needed for:
  - Enhanced VS Code C# IntelliSense
  - Debugging C# scripts in VS Code
  - Using Roslyn analyzers
  - Building .NET applications outside Unity

## Status

- [ ] .NET SDK installed
- [ ] `dotnet` command available in PATH
- [ ] VS Code C# extension functioning
- [ ] Debugging enabled

## Related Files

- This issue affects VS Code C# extension functionality
- Unity C# scripts will still compile in Unity Editor regardless of this issue

## Last Updated

November 15, 2025
