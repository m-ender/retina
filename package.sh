#!/usr/bin/env bash
set -e

cd Retina

tar -zcf retina-win-x64.tar.gz Retina/bin/win-x64
tar -zcf retina-linux-x64.tar.gz Retina/bin/linux-x64
