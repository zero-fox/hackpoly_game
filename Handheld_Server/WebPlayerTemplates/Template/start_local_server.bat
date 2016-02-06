@setlocal
@echo off
pushd %~dp0

REM --- kill the network servers
REM cmd /c nkill.bat

REM --- delete existing log files
REM del /q %BLADE_LOGS_DIR%\archive\*.log %BLADE_LOGS_DIR%\watcher.txt %BLADE_LOGS_DIR%\watcher_report.txt %BLADE_LOGS_DIR%\runner.txt >nul 2>nul


REM --- Is Node.js 0.10.xx or newer installed?
node --version > tempfile.txt
set /p nodeVer=<tempfile.txt

echo -----------------------------------------------------
echo Current Node Version...
echo %nodeVer%
echo -----------------------------------------------------

set /a min_ver_big=0
set /a min_ver_med=10
set /a min_ver_low=0

for /f "tokens=1,2,3 delims=." %%A in ("%nodeVer:~1,8%") do (
	if %%A LSS %min_ver_big%   goto version_fail
	if %%A EQU %min_ver_big% (
		if %%B LSS %min_ver_med%   goto version_fail
		if %%B EQU %min_ver_med% (
			if %%C LSS %min_ver_low%   goto version_fail
		)
	)
)

REM --- delete temp file
del tempfile.txt 2>nul

echo -----------------------------------------------------
echo Node Install...
echo -----------------------------------------------------
call npm install

REM -----------------------------------------------------
REM START NODE SERVER
REM -----------------------------------------------------
echo -----------------------------------------------------------------
echo Starting Node Server... (KEEP THIS WINDOW OPEN)
echo To run a controller:   use "localhost:8088/1" in your browser.
echo For multiplayer games run more controllers by changing the number X at the end of "localhost:8088/X"
echo -----------------------------------------------------------------

popd
endlocal

node server.js
goto end

:version_fail
echo -----------------------------------------------------
echo Node.js 0.10.0 or newer is required 
echo Available here:  https://nodejs.org/
echo -----------------------------------------------------
	
:fail
echo -----------------------------------------------------
echo FAILED!
echo -----------------------------------------------------
pause

:end

popd
endlocal
