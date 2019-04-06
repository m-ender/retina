#!/usr/bin/env bash
set -e

cd Retina

tar cfz retina-win-x64.tar.gz -C Retina/bin win-x64
tar cfz retina-linux-x64.tar.gz -C Retina/bin linux-x64
