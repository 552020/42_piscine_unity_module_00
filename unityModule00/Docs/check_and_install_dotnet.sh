#!/usr/bin/env bash
#
# check_and_install_dotnet.sh
# Script to check for .NET SDK and optionally install it
#

set -e

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

echo -e "${BLUE}=== .NET SDK Check and Installation Script ===${NC}\n"

# Function to check if dotnet is available
check_dotnet() {
    if command -v dotnet &> /dev/null; then
        echo -e "${GREEN}✓ .NET SDK is installed${NC}"
        echo -e "\nVersion information:"
        dotnet --version
        echo -e "\nInstalled SDKs:"
        dotnet --list-sdks
        return 0
    else
        echo -e "${RED}✗ .NET SDK is not found${NC}"
        return 1
    fi
}

# Function to detect OS
detect_os() {
    if [[ "$OSTYPE" == "linux-gnu"* ]]; then
        if [ -f /etc/os-release ]; then
            . /etc/os-release
            echo "$ID"
        else
            echo "linux"
        fi
    elif [[ "$OSTYPE" == "darwin"* ]]; then
        echo "macos"
    else
        echo "unknown"
    fi
}

# Function to install on Ubuntu/Debian
install_ubuntu() {
    echo -e "${YELLOW}Installing .NET SDK on Ubuntu/Debian...${NC}\n"
    
    # Get Ubuntu version
    UBUNTU_VERSION=$(lsb_release -rs)
    
    # Download and install Microsoft package repository
    wget https://packages.microsoft.com/config/ubuntu/${UBUNTU_VERSION}/packages-microsoft-prod.deb -O /tmp/packages-microsoft-prod.deb
    sudo dpkg -i /tmp/packages-microsoft-prod.deb
    rm /tmp/packages-microsoft-prod.deb
    
    # Update and install
    sudo apt-get update
    sudo apt-get install -y dotnet-sdk-8.0
    
    echo -e "\n${GREEN}✓ .NET SDK installation complete${NC}"
}

# Function to install on macOS
install_macos() {
    echo -e "${YELLOW}Installing .NET SDK on macOS...${NC}\n"
    
    if command -v brew &> /dev/null; then
        brew install --cask dotnet-sdk
        echo -e "\n${GREEN}✓ .NET SDK installation complete${NC}"
    else
        echo -e "${RED}✗ Homebrew is not installed${NC}"
        echo -e "Please install Homebrew first: https://brew.sh/"
        echo -e "Or download .NET SDK manually: https://dotnet.microsoft.com/download"
        exit 1
    fi
}

# Main script logic
main() {
    echo "Checking for .NET SDK..."
    echo ""
    
    if check_dotnet; then
        echo -e "\n${GREEN}No action needed. .NET SDK is already available.${NC}"
        exit 0
    fi
    
    echo ""
    echo -e "${YELLOW}Would you like to install .NET SDK? (y/n)${NC}"
    read -r response
    
    if [[ "$response" =~ ^[Yy]$ ]]; then
        OS=$(detect_os)
        
        case "$OS" in
            ubuntu|debian)
                install_ubuntu
                ;;
            macos)
                install_macos
                ;;
            *)
                echo -e "${RED}Unsupported OS: $OS${NC}"
                echo -e "Please install .NET SDK manually: https://dotnet.microsoft.com/download"
                exit 1
                ;;
        esac
        
        echo ""
        echo "Verifying installation..."
        if check_dotnet; then
            echo -e "\n${GREEN}✓ Installation successful!${NC}"
            echo -e "\nYou may need to restart VS Code for changes to take effect."
        else
            echo -e "\n${RED}✗ Installation verification failed${NC}"
            echo -e "You may need to add dotnet to your PATH manually."
            exit 1
        fi
    else
        echo -e "\n${YELLOW}Installation skipped.${NC}"
        echo -e "If you want .NET debugging and enhanced IntelliSense in VS Code,"
        echo -e "you can install it later by visiting: https://dotnet.microsoft.com/download"
        echo -e "\nNote: Unity C# scripts will still compile without .NET SDK."
        exit 0
    fi
}

# Run main function
main
