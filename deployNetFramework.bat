@echo off
set SOLUTION_NAME=AutoTanpopo
set SOLUTION_FILE=%SOLUTION_NAME%.sln
set BUILD_CONFIG=Release
set MAIN_PROJECT_OUTDIR=%SOLUTION_NAME%\bin\%BUILD_CONFIG%
set ARTIFACTS_BASEDIR=Artifacts
set ARTIFACTS_SUBDIR=%SOLUTION_NAME%-netframework
set ARTIFACTS_NAME=%ARTIFACTS_SUBDIR%.zip

if not exist "%ARTIFACTS_BASEDIR%\%ARTIFACTS_SUBDIR%\" (
  mkdir %ARTIFACTS_BASEDIR%\%ARTIFACTS_SUBDIR%
) else (
  del /Q %ARTIFACTS_BASEDIR%\%ARTIFACTS_SUBDIR%\*
)

copy %MAIN_PROJECT_OUTDIR%\%SOLUTION_NAME%.exe %ARTIFACTS_BASEDIR%\%ARTIFACTS_SUBDIR%\

set CMD7Z_FULLPATH="C:\Program Files\7-Zip\7z.exe"
set CMD7Z=
where 7z.exe 2>NUL >NUL && set CMD7Z=7z.exe
if [%CMD7Z%]==[] (
  if exist %CMD7Z_FULLPATH% (
    set CMD7Z=%CMD7Z_FULLPATH%
  )
)

if exist %ARTIFACTS_NAME% (
  del %ARTIFACTS_NAME%
)

cd %ARTIFACTS_BASEDIR%
if [%CMD7Z%]==[] (
  powershell Compress-Archive -Path test\ -DestinationPath test.zip
) else (
  %CMD7Z% a -mmt=on -mm=Deflate -mfb=258 -mpass=15 -r ..\%ARTIFACTS_NAME% %ARTIFACTS_SUBDIR%
)
cd ..
