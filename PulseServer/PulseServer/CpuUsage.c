#include "CpuUsage.h"
#include <Windows.h>
#include <stdio.h>

ULONGLONG FileTimeToULL(const FILETIME* ft)
{
	ULARGE_INTEGER uli;
	uli.LowPart = ft->dwLowDateTime;
	uli.HighPart = ft->dwHighDateTime;
	return uli.QuadPart;
}

void Logging(double cpuSystem, double cpuProcess) {
	char dir[MAX_PATH];
	DWORD result = GetCurrentDirectoryA(MAX_PATH, dir);
	if (result != 0) {
		printf("Diretório atual: %s\n", dir);
	}
	else {
		printf("Erro ao obter diretório atual. Código: %lu\n", GetLastError());
		return;
	}

	strcat_s(dir, MAX_PATH, "\\log.txt");

	FILE* logFile = fopen(dir, "a");
	if (!logFile) {
		printf("Erro ao abrir o arquivo \n");
		return;
	}

	fprintf(logFile, "CPU Sistema: %.2f%%\n", cpuSystem);
	fprintf(logFile, "CPU Processo: %.2f%%\n", cpuProcess);
	printf("Dados de CPU registrados no log.txt\n");

	fclose(logFile);
}

void SavePid(DWORD pid) {
	char dir[MAX_PATH];
	DWORD result = GetCurrentDirectoryA(MAX_PATH, dir);
	if (result != 0) {
		printf("Diretório atual: %s\n", dir);
	}
	else {
		printf("Erro ao obter diretório atual. Código: %lu\n", GetLastError());
		return;
	}

	strcat_s(dir, MAX_PATH, "\\PidSave.txt");

	FILE* logFile = fopen(dir, "w");
	if (!logFile) {
		printf("Erro ao abrir o arquivo \n");
		return;
	}

	fprintf(logFile, "PID: %lu\n", pid);
	fclose(logFile);

	printf("PID registrado no PidSave.txt\n");
}

DWORD LerPid() {
	char dir[MAX_PATH];
	DWORD result = GetCurrentDirectoryA(MAX_PATH, dir);
	if (result != 0) {
		printf("Diretório atual: %s\n", dir);
	}
	else {
		printf("Erro ao obter diretório atual. Código: %lu\n", GetLastError());
		return 0;
	}

	strcat_s(dir, MAX_PATH, "\\PidSave.txt");

	FILE* Arquivo = fopen(dir, "r");
	if (!Arquivo) {
		printf("Erro ao abrir o arquivo \n");
		return 0;
	}

	char buffer[256];
	fgets(buffer, sizeof(buffer), Arquivo);

	char* pid = strchr(buffer, ':');

	if (!pid) {
		printf("Formato invalid\n");
		return 0;
	}

	pid++;
	while (*pid == ' ') {
		pid++;
	}

	pid[strcspn(pid, "\r\n")] = '\0';

	DWORD pidValue = (DWORD)strtoul(pid, NULL, 10);

	fclose(Arquivo);

	return pidValue;

}

void StartCpuUsageMonitoring(DWORD pid)
{
	HANDLE hProcess = OpenProcess(PROCESS_QUERY_LIMITED_INFORMATION, FALSE, pid);

	if (!hProcess)
	{
		printf("Erro ao abrir processo. Código: %lu\n", GetLastError());
		return;
	}

	while (1) {

		DWORD exitCode;

		if (GetExitCodeProcess(hProcess, &exitCode) && exitCode != STILL_ACTIVE)
			break;

		FILETIME idle1, kernel1, user1;
		FILETIME idle2, kernel2, user2;

		FILETIME pCreate, pExit;
		FILETIME pKernel1, pUser1;
		FILETIME pKernel2, pUser2;

		GetSystemTimes(&idle1, &kernel1, &user1);
		GetProcessTimes(hProcess, &pCreate, &pExit, &pKernel1, &pUser1);

		Sleep(1000);

		GetSystemTimes(&idle2, &kernel2, &user2);
		GetProcessTimes(hProcess, &pCreate, &pExit, &pKernel2, &pUser2);

		ULONGLONG sysIdleDelta = FileTimeToULL(&idle2) - FileTimeToULL(&idle1);
		ULONGLONG sysKernelDelta = FileTimeToULL(&kernel2) - FileTimeToULL(&kernel1);
		ULONGLONG sysUserDelta = FileTimeToULL(&user2) - FileTimeToULL(&user1);

		ULONGLONG sysTotal = sysKernelDelta + sysUserDelta;

		ULONGLONG procKernelDelta = FileTimeToULL(&pKernel2) - FileTimeToULL(&pKernel1);
		ULONGLONG procUserDelta = FileTimeToULL(&pUser2) - FileTimeToULL(&pUser1);
		ULONGLONG procTotal = procKernelDelta + procUserDelta;

		if (sysTotal == 0)
		{
			printf("Erro: sysTotal = 0\n");
			CloseHandle(hProcess);
			return;
		}

		double cpuSystem = (1.0 - ((double)sysIdleDelta / (double)sysTotal)) * 100.0;

		double cpuProcess = ((double)procTotal / (double)sysTotal) * 100.0;

		if (cpuSystem < 0.0) cpuSystem = 0.0;
		if (cpuProcess < 0.0) cpuProcess = 0.0;

		printf("CPU Sistema: %.2f%%\n", cpuSystem);
		printf("CPU Processo: %.2f%%\n", cpuProcess);

		Logging(cpuSystem, cpuProcess);
		Sleep(1000);
	}
	CloseHandle(hProcess);
}

void StartProgram() {
	STARTUPINFOA si;
	PROCESS_INFORMATION pi;

	ZeroMemory(&si, sizeof(si));
	si.cb = sizeof(si);

	ZeroMemory(&pi, sizeof(pi));

	char cmd[] = "cmd.exe /c npm start";
	char dir[MAX_PATH];
	DWORD Result = GetCurrentDirectoryA(MAX_PATH, dir);
	if (!Result) {
		printf("Erro ao obter diretório atual. Código: %lu\n", GetLastError());
		return;
	}

	strcat_s(dir, MAX_PATH, "\\site");

	if (CreateProcessA(
		NULL,
		cmd,
		NULL,
		NULL,
		FALSE,
		CREATE_NEW_CONSOLE,
		NULL,
		dir,
		&si,
		&pi
	)) {

		printf("Processo iniciado com sucesso\n");

		printf("PID: %lu\n", pi.dwProcessId);

		printf("HANDLE Processo: %p\n", pi.hProcess);
		printf("HANDLE Thread: %p\n", pi.hThread);

		DWORD pid = pi.dwProcessId;
		SavePid(pid);
		StartCpuUsageMonitoring(pid);

		CloseHandle(pi.hThread);
		CloseHandle(pi.hProcess);
	}
	else
	{
		printf("Erro ao iniciar processo.\n");
		printf("Codigo erro: %lu\n", GetLastError());
	}
}

void StopProgramMonitoring() {

	DWORD Pid = LerPid();

	HANDLE hProcess = OpenProcess(PROCESS_TERMINATE, FALSE, Pid);
	if (!hProcess) {
		printf("Erro ao abrir o processo: Codigo: %lu", GetLastError());
		return;
	}

	if (TerminateProcess(hProcess, 0)){
		printf("Processo %lu finalizado com sucesso\n", Pid);
	}
	else {
		printf("Erro ao terminar processo. Código: %lu\n", GetLastError());
	}

	CloseHandle(hProcess);
}