@echo off
echo OpenCover Version: %1
echo Application: %2
echo Parameter: %4
if not exist ".\Test\Coverage-%3_coverage.xml" goto nomerge
set MERGE=-mergeoutput
:nomerge
start "%2" /B ".\packages\OpenCover.%1\tools\OpenCover.Console.exe" -register:Path64 -target:".\Test\Test-%2\bin\x64\%3\Test-%2.exe" -targetargs:%4 -output:".\Test\Coverage-%3_coverage.xml" %MERGE%
set MERGE=
