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
Interação direta com a Windows API
Controle de processos em nível baixo do sistema
⚙️ Como executar

Para iniciar o programa, vá até a pasta onde está o executável e execute o comando abaixo:

PulseServer.exe start

⚙️ Comandos disponíveis
▶️ Iniciar monitoramento e processo

PulseServer.exe start

Inicia um processo dentro da pasta site
Cria uma thread para execução independente
Captura o PID do processo
Inicia o monitoramento de CPU em loop
Gera logs de uso de CPU em tempo real
⛔ Parar processo monitorado

PulseServer.exe stopmonitoring

Lê o PID salvo em PidSave.txt
Encerra o processo monitorado
Finaliza o controle do programa
📊 Monitoramento de CPU

O cálculo de uso de CPU é feito utilizando funções nativas do Windows:

GetSystemTimes()
GetProcessTimes()

A cada segundo, o sistema calcula:

Uso de CPU do sistema (%)
Uso de CPU do processo (%)

Os resultados são:

Exibidos no console
Registrados no arquivo log.txt
🧠 Tecnologias utilizadas
Linguagem C (C17)
Windows API
Threads (CreateThread)
Process Management:
CreateProcessA
OpenProcess
TerminateProcess
File I/O (manipulação de arquivos)
Programação de sistema (System Programming)
📁 Estrutura do projeto

/site → aplicação executada (npm start)
/log.txt → logs de CPU
/PidSave.txt → PID do processo monitorado
/main.c → código principal
/CpuUsage.c/.h → módulo de monitoramento de CPU
/PulseServer.exe → executável do sistema

🚀 Objetivo

O objetivo deste projeto é praticar:

Manipulação de processos no Windows
Programação com threads em C
Monitoramento de desempenho do sistema
Uso da Windows API em baixo nível
Controle de execução de processos externos
Leitura e escrita de arquivos em C
📌 Status do projeto

🚧 Em desenvolvimento
O projeto ainda está sendo aprimorado e pode receber novas funcionalidades e melhorias de performance.
