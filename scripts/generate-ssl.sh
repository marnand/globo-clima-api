#!/bin/bash
set -e

echo "Gerando certificado SSL para globoclima.dedyn.io..."

# Criar diretório para certificados
mkdir -p ssl

# Gerar chave privada
openssl genrsa -out ssl/server.key 2048

# Gerar certificado auto-assinado para o domínio (válido por 365 dias)
openssl req -new -x509 -key ssl/server.key -out ssl/server.crt -days 365 -subj "/C=BR/ST=State/L=City/O=GloboClima/CN=globoclima.dedyn.io"

echo "Certificado SSL gerado com sucesso para globoclima.dedyn.io!"
echo "Arquivos criados:"
echo "- ssl/server.key (chave privada)"
echo "- ssl/server.crt (certificado)"
echo ""
echo "IMPORTANTE: Para produção, considere usar Let's Encrypt:"
echo "sudo certbot --nginx -d globoclima.dedyn.io"