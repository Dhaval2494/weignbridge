Generating release from folder:
C:\Program Files (x86)\WiX Toolset v3.11\bin>
heat dir "D:\Projects\WeighBridge\WeighingProject\WeighingSolution\WeighingSolution\bin\Release" -gg -g1 -cg CoreComponents -dr INSTALLFOLDER -sfrag -template fragment -var var.WeighingSolution_TargetDir -out "D:\Projects\WeighBridge\WeighingProject\WeighingSolution\SetupMSI\CoreComponents1.wxs"

Include preprocessor after generation:
  <?define WeighingSolution_TargetDir=$(var.WeighingSolution.TargetDir)?>


  Writing Log:
msiexec /i SampleFirst.msi /l* SampleFirst.log
msiexec /i MyApplication.msi /l*v MyLogFile.txt
Uninstall -> msiexec /x SampleFirst.msi



https://docs.microsoft.com/en-us/windows/desktop/Msi/property-reference#operating_system_properties

https://www.firegiant.com/wix/tutorial/com-expression-syntax-miscellanea/add-or-remove-programs-entries/