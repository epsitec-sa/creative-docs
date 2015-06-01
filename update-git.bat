@echo off
echo Pull from git and update submodules.

git pull
git submodule update --init --recursive
git submodule update --remote --merge
