::@echo off
cls
setlocal enabledelayedexpansion
title �Զ�ͬ��

set url=https://svn.newlifex.com/svn/X/trunk/Src
for %%i in (NewLife.Core NewLife.Net XCode XAgent XCoder XControl XTemplate NewLife.Cube) do (
	pushd %%i

	if not exist .git (
		git svn clone %url%/%%i --no-metadata --authors-file=..\user.txt .\
		git remote add origin https://github.com/NewLifeX/%%i.git
	) else (
		git fetch -v --progress "origin"
		git svn fetch
	)

	git push --all --force-with-lease --progress "origin"

	popd
)
