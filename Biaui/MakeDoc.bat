echo off
cd /d %~dp0

if exist source\docfx_project\obj (
rd /S /Q source\docfx_project\obj
)

pushd source\docfx_project

..\..\external\docfx\docfx metadata
..\..\external\docfx\docfx build

popd

