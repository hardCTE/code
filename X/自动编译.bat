:: �Զ�����X����������µ�DLL��ȥ
:: 1����������Դ��Src
:: 2������DLL
:: 3�������������
:: 4������DLL
:: 5���ύDLL����
:: 6�����Src��DLL��FTP

::@echo off
::cls
setlocal enabledelayedexpansion
title �Զ�����

:: 1����������Դ��Src
:: 2������DLL
:: ���浱ǰĿ¼�����л�Ŀ¼
pushd ..
set svn=https://svn.newlifex.com/svn/X/trunk
:: do else �ȹؼ���ǰ��Ӧ��Ԥ���ո�
for %%i in (Src DLL DLL4 XCoder) do (
	if not exist %%i (
		svn checkout %svn%/%%i %%i
	) else (
		svn info %svn%/%%i
		svn revert %%i
		svn update %%i
	)
)
:: �ָ�Ŀ¼
popd

:: 3�������������
::"D:\MS\Microsoft Visual Studio 10.0\Common7\IDE\devenv.exe" X���.sln /Build Release
::set vs="B:\MS\Microsoft Visual Studio 12.0\Common7\IDE\devenv.com"
set vs="B:\MS\Microsoft Visual Studio 14.0\Common7\IDE\devenv.com"
for %%i in (NewLife.Core XCode NewLife.CommonEntity NewLife.Net XAgent XControl XTemplate) do (
	%vs% X���.sln /Build Release /Project %%i
)
for %%i in (XCoder NewLife.Cube) do (
	%vs% X���.sln /Build Release /Project %%i
)

:: 4������DLL
copy ..\Bin\N*.* ..\DLL\ /y
copy ..\Bin\X*.* ..\DLL\ /y
del ..\DLL\*.config /f/s/q
copy ..\Bin4\N*.* ..\DLL4\ /y
copy ..\Bin4\X*.* ..\DLL4\ /y
del ..\DLL4\*.config /f/s/q

if not exist ..\XCoder\Zip (
	md ..\XCoder\Zip
)
for %%i in (XCoder.exe XCoder.exe.config NewLife.Core.dll XCode.dll XTemplate.dll NewLife.Net.dll) do (
	copy ..\XCoder\%%i ..\XCoder\Zip\%%i /y
)

:: 5���ύDLL����
svn commit -m "�Զ�����" ..\DLL
svn commit -m "�Զ�����" ..\DLL4
svn commit -m "�Զ�����" ..\XCoder

:: 6�����Src��DLL��FTP
set zipexe="B:\Pro\WinRAR\WinRAR.exe"
set zip=%zipexe% a -m5 -s -z..\Src\Readme.txt -ibck
::set zipexe="D:\Pro\7-zip\7z.exe"
::set zip=%zipexe% a -tzip -mx9
set dest=E:\XX\X

:: ����SrcԴ��
rd XCoder\bin /s/q
rd XCoder\obj /s/q
set zipfile=Src.zip
del Src*.zip /f/q
%zip% -r %zipfile% XCoder\*.*
move /y Src*.zip %dest%\%zipfile%

:: ����XCode����Դ��

:: ����DLLѹ����
:: ���浱ǰĿ¼�����л�Ŀ¼
pushd ..\DLL
set zipfile=DLL.zip
del DLL*.zip /f/q
%zip% %zipfile% *.dll *.exe *.pdb *.xml
move /y DLL*.zip %dest%\%zipfile%
:: �ָ�Ŀ¼
popd

:: ����DLL4ѹ����
:: ���浱ǰĿ¼�����л�Ŀ¼
pushd ..\DLL4
set zipfile=DLL4.zip
del DLL*.zip /f/q
set zip4=%zipexe% a -m5 -s -z..\Src\Readme.txt -ibck
%zip4% %zipfile% *.dll *.exe *.pdb *.xml
move /y DLL*.zip %dest%\%zipfile%
:: �ָ�Ŀ¼
popd

:: ��������������XCoder
:: ���浱ǰĿ¼�����л�Ŀ¼
pushd ..\XCoder\Zip
set zipfile=XCoder.zip
del XCoder*.zip /f/q
::del *.vshost.* /f/q
::del Setting.config /f/q
set zip=%zipexe% a -m5 -s -z..\..\Src\Readme.txt -ibck
%zip% %zipfile% *.dll *.exe *.config
move /y XCoder*.zip %dest%\%zipfile%
:: �ָ�Ŀ¼
popd

::pause