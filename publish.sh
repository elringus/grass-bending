#!/usr/bin/env sh

# abort on errors
set -e

cd Assets/GrassBending

git init
git add -A
git commit -m 'publish'
git push -f git@github.com:Elringus/GrassBending.git master:package

cd -