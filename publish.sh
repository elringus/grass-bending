#!/usr/bin/env sh

set -e
cd Assets/GrassBending
git init
git add -A
git commit -m 'publish'
git push -f git@github.com:elringus/grass-bending.git main:package
cd -
