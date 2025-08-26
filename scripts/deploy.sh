#!/bin/bash
set -e

# Parar aplicação existente
sudo systemctl stop globoclima || true

# Extrair nova versão
cd /home/$USER
tar -xzf globoclima-app.tar.gz -C /opt/globoclima/

# Reiniciar aplicação
sudo systemctl start globoclima
sudo systemctl enable globoclima

echo "Deploy concluído com sucesso!"