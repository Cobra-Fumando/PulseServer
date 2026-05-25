#pragma once
#include <Windows.h>

ULONGLONG FileTimeToULL(const FILETIME* ft);

void SavePid(DWORD pid);

void StartCpuUsageMonitoring(DWORD pid);

void StartProgram();

void StopProgramMonitoring();

DWORD LerPid();
