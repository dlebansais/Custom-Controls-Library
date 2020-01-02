@echo off
echo Parameter: %2
start "BusyIndicator" /B ".\packages\OpenCover.4.7.922\tools\OpenCover.Console.exe" -register:Path64 -target:".\Test\Test-BusyIndicator\bin\x64\%1\Test-BusyIndicator.exe" -targetargs:%2 -output:".\Test\Coverage-BusyIndicator-%1_coverage.xml"
