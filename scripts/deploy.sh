#!/bin/bash
set -e

# Este script agora é executado via GitHub Actions
# As operações Docker são feitas diretamente no workflow

echo "Script de deploy executado via GitHub Actions"
echo "Container Docker está sendo gerenciado pelo workflow"

# Parar aplicação existente
sudo systemctl stop globoclima || true

# Extrair nova versão
cd /home/$USER
tar -xzf globoclima-app.tar.gz -C /opt/globoclima/

# Reiniciar aplicação
sudo systemctl start globoclima
sudo systemctl enable globoclima

echo "Deploy concluído com sucesso!"