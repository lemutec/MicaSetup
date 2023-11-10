@echo off
cd /d %USERPROFILE%\AppData\Roaming\npm\node_modules\icomoon-cli
set SCRIPT_FILE=cli.js
echo #!/usr/bin/env node> %SCRIPT_FILE%
echo const yargs = require('yargs')>> %SCRIPT_FILE%
echo const pipeline = require('./index')>> %SCRIPT_FILE%
echo.>> %SCRIPT_FILE%
echo const argv = yargs>> %SCRIPT_FILE%
echo   .alias('h', 'help')>> %SCRIPT_FILE%
echo   .option('s', {>> %SCRIPT_FILE%
echo     alias : 'selection',>> %SCRIPT_FILE%
echo     demand: true,>> %SCRIPT_FILE%
echo     describe: 'path to icomoon selection file',>> %SCRIPT_FILE%
echo   })>> %SCRIPT_FILE%
echo   .option('i', {>> %SCRIPT_FILE%
echo     alias: 'icons',>> %SCRIPT_FILE%
echo     describe: 'paths to icons need to be imported, separated by comma',>> %SCRIPT_FILE%
echo     default: '',>> %SCRIPT_FILE%
echo   })>> %SCRIPT_FILE%
echo   .option('n', {>> %SCRIPT_FILE%
echo     alias: 'names',>> %SCRIPT_FILE%
echo     describe: 'rename icons, separated by comma, matched by index',>> %SCRIPT_FILE%
echo     default: '',>> %SCRIPT_FILE%
echo   })>> %SCRIPT_FILE%
echo   .option('o', {>> %SCRIPT_FILE%
echo     alias: 'output',>> %SCRIPT_FILE%
echo     default: './output',>> %SCRIPT_FILE%
echo     describe: 'output directory',>> %SCRIPT_FILE%
echo   })>> %SCRIPT_FILE%
echo   .option('f', {>> %SCRIPT_FILE%
echo     alias: 'force',>> %SCRIPT_FILE%
echo     default: false,>> %SCRIPT_FILE%
echo     describe: 'force override current icon when icon name duplicated',>> %SCRIPT_FILE%
echo   })>> %SCRIPT_FILE%
echo   .option('v', {>> %SCRIPT_FILE%
echo     alias: 'visible',>> %SCRIPT_FILE%
echo     default: false,>> %SCRIPT_FILE%
echo     describe: 'run a GUI chrome instead of headless mode',>> %SCRIPT_FILE%
echo   })>> %SCRIPT_FILE%
echo   .argv;>> %SCRIPT_FILE%
echo.>> %SCRIPT_FILE%
echo pipeline({>> %SCRIPT_FILE%
echo   selectionPath: argv.s,>> %SCRIPT_FILE%
echo   icons: argv.i.toString().includes(',') ? argv.i.split(',') : [argv.i],>> %SCRIPT_FILE%
echo   names: argv.n.toString().includes(',') ? argv.n.split(',') : [argv.n],>> %SCRIPT_FILE%
echo   outputDir: argv.o,>> %SCRIPT_FILE%
echo   forceOverride: argv.f,>> %SCRIPT_FILE%
echo   visible: argv.visible,>> %SCRIPT_FILE%
echo });>> %SCRIPT_FILE%
echo %SCRIPT_FILE% overrived
cd /d "%~dp0"
