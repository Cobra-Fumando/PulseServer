CPU Monitor & Process Manager (C / Windows)

Este projeto é um monitor de CPU e gerenciador de processos para Windows, desenvolvido em C utilizando diretamente a Windows API. Ele permite iniciar um processo externo, monitorar seu uso de CPU em tempo real e encerrá-lo via comando.

⚠️ Projeto em desenvolvimento — ainda pode passar por melhorias, otimizações e novas funcionalidades.

📌 Funcionalidades
Inicia um processo externo (npm start) em uma nova thread
Monitora uso de CPU do sistema e de um processo específico
Registra dados de CPU em um arquivo de log (log.txt)
Salva o PID do processo em arquivo (PidSave.txt)
Permite encerrar o processo via comando
Interface baseada em linha de comando (CLI)
Uso de threads para execução não bloqueante
⚙️ Como funciona

O programa aceita comandos via argumentos:

▶️ Start
program.exe start
Inicia um processo dentro da pasta site
Cria uma thread para execução independente
Captura o PID do processo
Inicia o monitoramento de CPU em loop
⛔ Stop
program.exe stopmonitoring
Lê o PID salvo em PidSave.txt
Encerra o processo monitorado
📊 Monitoramento de CPU

O cálculo de uso de CPU é feito utilizando funções nativas do Windows:

GetSystemTimes()
GetProcessTimes()

A cada segundo, o sistema calcula:

Uso de CPU do sistema (%)
Uso de CPU do processo (%)

Os resultados são:

Exibidos no console
Registrados em log.txt
🧠 Tecnologias utilizadas
C (C17)
Windows API
Threads (CreateThread)
Process Management (CreateProcess, OpenProcess, TerminateProcess)
File I/O
📁 Estrutura de arquivos
/site              -> aplicação executada (npm start)
/log.txt           -> logs de CPU
/PidSave.txt       -> PID do processo monitorado
/main.c            -> código principal
/CpuUsage.c/.h     -> módulo de monitoramento
🚀 Objetivo

O objetivo do projeto é praticar:

Manipulação de processos no Windows
Programação com threads em C
Monitoramento de desempenho do sistema
Uso da Windows API em baixo nível
