# ApkInstallerDll
File .dll. Support to install apk to connected phone with adb.
## How to use?
**There's a function in class ApkInstaller named Install2Phone**</br>
**Here's the construct function:**</br>
*public int Install2Phone(String adbPath, String apkPath, Boolean toAll = false){}*</br>
*(adbPath:Your adb path needs specifically to adb.exe)*</br>
*(apkPath:Your apk path  needs specifically to xxx.apk)*</br>
*(toAll:It means if install on all connected devices)*</br>
</br>
**The return parameter are:**

* NO_APK_FILE = -3;
* NO_ADB_PATH = -2;
* NO_JAVA_ENVIRONMENT = -1;
* NO_DEVICE = 0;
* INSTALL_START = 1;

Also everyone can register the callBack *(DataReceivedEventHandler)HandleOutput* of class ApkInstaller to listen and handle message in process.
