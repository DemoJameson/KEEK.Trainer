@echo Starting KEEK with trainer
@ren winhttp.dll.off winhttp.dll
@start KEEK.exe
@ping 127.0.0.1 -n 3 >nul
@ren winhttp.dll winhttp.dll.off