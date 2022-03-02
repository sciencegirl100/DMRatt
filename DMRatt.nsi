############################################################################################
#      NSIS Installation Script created by NSIS Quick Setup Script Generator v1.09.18
#               Entirely Edited with NullSoft Scriptable Installation System                
#              by Vlasis K. Barkas aka Red Wine red_wine@freemail.gr Sep 2006               
############################################################################################

!define APP_NAME "DMRatt"
!define COMP_NAME "Cray.LGBT"
!define WEB_SITE "https://cray.lgbt"
!define VERSION "00.00.01.00"
!define COPYRIGHT "Elizabeth Anne Cray ©? 2022"
!define DESCRIPTION "DMR Contact Tool"
!define LICENSE_TXT "C:\Users\liz\source\repos\DMRatt\LICENSE.txt"
!define INSTALLER_NAME "C:\Users\liz\source\repos\DMRatt\DMRatt.exe"
!define MAIN_APP_EXE "DMRatt.exe"
!define INSTALL_TYPE "SetShellVarContext current"
!define REG_ROOT "HKCU"
!define REG_APP_PATH "Software\Microsoft\Windows\CurrentVersion\App Paths\${MAIN_APP_EXE}"
!define UNINSTALL_PATH "Software\Microsoft\Windows\CurrentVersion\Uninstall\${APP_NAME}"

!define REG_START_MENU "Start Menu Folder"

var SM_Folder

######################################################################

VIProductVersion  "${VERSION}"
VIAddVersionKey "ProductName"  "${APP_NAME}"
VIAddVersionKey "CompanyName"  "${COMP_NAME}"
VIAddVersionKey "LegalCopyright"  "${COPYRIGHT}"
VIAddVersionKey "FileDescription"  "${DESCRIPTION}"
VIAddVersionKey "FileVersion"  "${VERSION}"

######################################################################

SetCompressor ZLIB
Name "${APP_NAME}"
Caption "${APP_NAME}"
OutFile "${INSTALLER_NAME}"
BrandingText "${APP_NAME}"
XPStyle on
InstallDirRegKey "${REG_ROOT}" "${REG_APP_PATH}" ""
InstallDir "$PROGRAMFILES\DMRatt"

######################################################################

!include "MUI.nsh"

!define MUI_ABORTWARNING
!define MUI_UNABORTWARNING

!insertmacro MUI_PAGE_WELCOME

!ifdef LICENSE_TXT
!insertmacro MUI_PAGE_LICENSE "${LICENSE_TXT}"
!endif

!insertmacro MUI_PAGE_DIRECTORY

!ifdef REG_START_MENU
!define MUI_STARTMENUPAGE_NODISABLE
!define MUI_STARTMENUPAGE_DEFAULTFOLDER "DMRatt"
!define MUI_STARTMENUPAGE_REGISTRY_ROOT "${REG_ROOT}"
!define MUI_STARTMENUPAGE_REGISTRY_KEY "${UNINSTALL_PATH}"
!define MUI_STARTMENUPAGE_REGISTRY_VALUENAME "${REG_START_MENU}"
!insertmacro MUI_PAGE_STARTMENU Application $SM_Folder
!endif

!insertmacro MUI_PAGE_INSTFILES

!define MUI_FINISHPAGE_RUN "$INSTDIR\${MAIN_APP_EXE}"
!insertmacro MUI_PAGE_FINISH

!insertmacro MUI_UNPAGE_CONFIRM

!insertmacro MUI_UNPAGE_INSTFILES

!insertmacro MUI_UNPAGE_FINISH

!insertmacro MUI_LANGUAGE "English"

######################################################################

Section -MainProgram
${INSTALL_TYPE}
SetOverwrite ifnewer
SetOutPath "$INSTDIR"
File "C:\Users\liz\source\repos\DMRatt\DMRatt\bin\Release\DMRatt.exe"
File "C:\Users\liz\source\repos\DMRatt\DMRatt\bin\Release\DMRatt.exe.config"
File "C:\Users\liz\source\repos\DMRatt\DMRatt\bin\Release\DMRatt.pdb"
File "C:\Users\liz\source\repos\DMRatt\DMRatt\bin\Release\Microsoft.Bcl.AsyncInterfaces.dll"
File "C:\Users\liz\source\repos\DMRatt\DMRatt\bin\Release\Microsoft.Bcl.AsyncInterfaces.xml"
File "C:\Users\liz\source\repos\DMRatt\DMRatt\bin\Release\RestSharp.dll"
File "C:\Users\liz\source\repos\DMRatt\DMRatt\bin\Release\RestSharp.xml"
File "C:\Users\liz\source\repos\DMRatt\DMRatt\bin\Release\System.Buffers.dll"
File "C:\Users\liz\source\repos\DMRatt\DMRatt\bin\Release\System.Buffers.xml"
File "C:\Users\liz\source\repos\DMRatt\DMRatt\bin\Release\System.Memory.dll"
File "C:\Users\liz\source\repos\DMRatt\DMRatt\bin\Release\System.Memory.xml"
File "C:\Users\liz\source\repos\DMRatt\DMRatt\bin\Release\System.Numerics.Vectors.dll"
File "C:\Users\liz\source\repos\DMRatt\DMRatt\bin\Release\System.Numerics.Vectors.xml"
File "C:\Users\liz\source\repos\DMRatt\DMRatt\bin\Release\System.Runtime.CompilerServices.Unsafe.dll"
File "C:\Users\liz\source\repos\DMRatt\DMRatt\bin\Release\System.Runtime.CompilerServices.Unsafe.xml"
File "C:\Users\liz\source\repos\DMRatt\DMRatt\bin\Release\System.Text.Encodings.Web.dll"
File "C:\Users\liz\source\repos\DMRatt\DMRatt\bin\Release\System.Text.Encodings.Web.xml"
File "C:\Users\liz\source\repos\DMRatt\DMRatt\bin\Release\System.Text.Json.dll"
File "C:\Users\liz\source\repos\DMRatt\DMRatt\bin\Release\System.Text.Json.xml"
File "C:\Users\liz\source\repos\DMRatt\DMRatt\bin\Release\System.Threading.Tasks.Extensions.dll"
File "C:\Users\liz\source\repos\DMRatt\DMRatt\bin\Release\System.Threading.Tasks.Extensions.xml"
File "C:\Users\liz\source\repos\DMRatt\DMRatt\bin\Release\System.ValueTuple.dll"
File "C:\Users\liz\source\repos\DMRatt\DMRatt\bin\Release\System.ValueTuple.xml"
SectionEnd

######################################################################

Section -Icons_Reg
SetOutPath "$INSTDIR"
WriteUninstaller "$INSTDIR\uninstall.exe"

!ifdef REG_START_MENU
!insertmacro MUI_STARTMENU_WRITE_BEGIN Application
CreateDirectory "$SMPROGRAMS\$SM_Folder"
CreateShortCut "$SMPROGRAMS\$SM_Folder\${APP_NAME}.lnk" "$INSTDIR\${MAIN_APP_EXE}"
CreateShortCut "$DESKTOP\${APP_NAME}.lnk" "$INSTDIR\${MAIN_APP_EXE}"
CreateShortCut "$SMPROGRAMS\$SM_Folder\Uninstall ${APP_NAME}.lnk" "$INSTDIR\uninstall.exe"

!ifdef WEB_SITE
WriteIniStr "$INSTDIR\${APP_NAME} website.url" "InternetShortcut" "URL" "${WEB_SITE}"
CreateShortCut "$SMPROGRAMS\$SM_Folder\${APP_NAME} Website.lnk" "$INSTDIR\${APP_NAME} website.url"
!endif
!insertmacro MUI_STARTMENU_WRITE_END
!endif

!ifndef REG_START_MENU
CreateDirectory "$SMPROGRAMS\DMRatt"
CreateShortCut "$SMPROGRAMS\DMRatt\${APP_NAME}.lnk" "$INSTDIR\${MAIN_APP_EXE}"
CreateShortCut "$DESKTOP\${APP_NAME}.lnk" "$INSTDIR\${MAIN_APP_EXE}"
CreateShortCut "$SMPROGRAMS\DMRatt\Uninstall ${APP_NAME}.lnk" "$INSTDIR\uninstall.exe"

!ifdef WEB_SITE
WriteIniStr "$INSTDIR\${APP_NAME} website.url" "InternetShortcut" "URL" "${WEB_SITE}"
CreateShortCut "$SMPROGRAMS\DMRatt\${APP_NAME} Website.lnk" "$INSTDIR\${APP_NAME} website.url"
!endif
!endif

WriteRegStr ${REG_ROOT} "${REG_APP_PATH}" "" "$INSTDIR\${MAIN_APP_EXE}"
WriteRegStr ${REG_ROOT} "${UNINSTALL_PATH}"  "DisplayName" "${APP_NAME}"
WriteRegStr ${REG_ROOT} "${UNINSTALL_PATH}"  "UninstallString" "$INSTDIR\uninstall.exe"
WriteRegStr ${REG_ROOT} "${UNINSTALL_PATH}"  "DisplayIcon" "$INSTDIR\${MAIN_APP_EXE}"
WriteRegStr ${REG_ROOT} "${UNINSTALL_PATH}"  "DisplayVersion" "${VERSION}"
WriteRegStr ${REG_ROOT} "${UNINSTALL_PATH}"  "Publisher" "${COMP_NAME}"

!ifdef WEB_SITE
WriteRegStr ${REG_ROOT} "${UNINSTALL_PATH}"  "URLInfoAbout" "${WEB_SITE}"
!endif
SectionEnd

######################################################################

Section Uninstall
${INSTALL_TYPE}
Delete "$INSTDIR\DMRatt.exe"
Delete "$INSTDIR\DMRatt.exe.config"
Delete "$INSTDIR\DMRatt.pdb"
Delete "$INSTDIR\Microsoft.Bcl.AsyncInterfaces.dll"
Delete "$INSTDIR\Microsoft.Bcl.AsyncInterfaces.xml"
Delete "$INSTDIR\RestSharp.dll"
Delete "$INSTDIR\RestSharp.xml"
Delete "$INSTDIR\System.Buffers.dll"
Delete "$INSTDIR\System.Buffers.xml"
Delete "$INSTDIR\System.Memory.dll"
Delete "$INSTDIR\System.Memory.xml"
Delete "$INSTDIR\System.Numerics.Vectors.dll"
Delete "$INSTDIR\System.Numerics.Vectors.xml"
Delete "$INSTDIR\System.Runtime.CompilerServices.Unsafe.dll"
Delete "$INSTDIR\System.Runtime.CompilerServices.Unsafe.xml"
Delete "$INSTDIR\System.Text.Encodings.Web.dll"
Delete "$INSTDIR\System.Text.Encodings.Web.xml"
Delete "$INSTDIR\System.Text.Json.dll"
Delete "$INSTDIR\System.Text.Json.xml"
Delete "$INSTDIR\System.Threading.Tasks.Extensions.dll"
Delete "$INSTDIR\System.Threading.Tasks.Extensions.xml"
Delete "$INSTDIR\System.ValueTuple.dll"
Delete "$INSTDIR\System.ValueTuple.xml"
 
 
Delete "$INSTDIR\uninstall.exe"
!ifdef WEB_SITE
Delete "$INSTDIR\${APP_NAME} website.url"
!endif

RmDir "$INSTDIR"

!ifdef REG_START_MENU
!insertmacro MUI_STARTMENU_GETFOLDER "Application" $SM_Folder
Delete "$SMPROGRAMS\$SM_Folder\${APP_NAME}.lnk"
Delete "$SMPROGRAMS\$SM_Folder\Uninstall ${APP_NAME}.lnk"
!ifdef WEB_SITE
Delete "$SMPROGRAMS\$SM_Folder\${APP_NAME} Website.lnk"
!endif
Delete "$DESKTOP\${APP_NAME}.lnk"

RmDir "$SMPROGRAMS\$SM_Folder"
!endif

!ifndef REG_START_MENU
Delete "$SMPROGRAMS\DMRatt\${APP_NAME}.lnk"
Delete "$SMPROGRAMS\DMRatt\Uninstall ${APP_NAME}.lnk"
!ifdef WEB_SITE
Delete "$SMPROGRAMS\DMRatt\${APP_NAME} Website.lnk"
!endif
Delete "$DESKTOP\${APP_NAME}.lnk"

RmDir "$SMPROGRAMS\DMRatt"
!endif

DeleteRegKey ${REG_ROOT} "${REG_APP_PATH}"
DeleteRegKey ${REG_ROOT} "${UNINSTALL_PATH}"
SectionEnd

######################################################################

