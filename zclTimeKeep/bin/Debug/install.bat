%SystemRoot%\Microsoft.NET\Framework\v4.0.30319\installutil.exe zclTimeKeep.exe
Net Start zclTimeKeep
sc config zclTimeKeep start= auto
pause