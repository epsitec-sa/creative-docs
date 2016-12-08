@echo off
echo Pull from git and update submodules.

git pull
git submodule update --init --recursive
git submodule foreach --recursive git checkout master
git submodule foreach --recursive git pull

