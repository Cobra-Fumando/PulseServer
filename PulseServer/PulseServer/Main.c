#include <stdio.h>
#include <string.h>
#include <ctype.h>
#include <stdlib.h>
#include <Windows.h>
#include "CpuUsage.h"

void tolower_string(char* str) {

	while (*str) {
		*str = tolower(*str);
		str++;
	}

}

DWORD WINAPI MinhaThread(LPVOID lpParam) {
	StartProgram();
	return 0;
}

int main(int argc, char* argv[]) {

	HANDLE ThreadHandle = NULL;
	int threadCreated = 0;

	if (argc < 2) {
		printf("O comando está invalid\n");
		return 1;
	}

	tolower_string(argv[1]);

	if (strcmp(argv[1], "start") == 0) {
		printf("Iniciando Servidor...\n");
		ThreadHandle = CreateThread(
			NULL,
			0,
			MinhaThread,
			NULL,
			0,
			NULL
		);

		if (ThreadHandle == NULL) {
			printf("Erro ao criar a thread\n");
			return;
		}

		threadCreated = 1;
	}
	else if (strcmp(argv[1], "stopmonitoring") == 0) {
		printf("Parando o servidor...\n");
		StopProgramMonitoring();
	}

	if (threadCreated) {
		WaitForSingleObject(ThreadHandle, INFINITE);
		CloseHandle(ThreadHandle);
	}

	printf("Fim\n");

	return 0;
}