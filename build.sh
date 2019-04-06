#!/usr/bin/env bash
set -e

cd Retina

dotnet restore
dotnet build
#dotnet test
dotnet publish -c Release --self-contained false --runtime win-x64 -o bin/win-x64
dotnet publish -c Release --self-contained false --runtime linux-x64 -o bin/linux-x64
