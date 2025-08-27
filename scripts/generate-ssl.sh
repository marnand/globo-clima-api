#!/bin/bash
set -e

echo "Gerando certificado SSL auto-assinado..."

# Criar diretório para certificados
mkdir -p ssl

# Gerar chave privada
openssl genrsa -out ssl/server.key 2048

# Gerar certificado auto-assinado (válido por 365 dias)
openssl req -new -x509 -key ssl/server.key -out ssl/server.crt -days 365 -subj "/C=BR/ST=State/L=City/O=Organization/CN=localhost"

echo "Certificado SSL gerado com sucesso!"
echo "Arquivos criados:"
echo "- ssl/server.key (chave privada)"
echo "- ssl/server.crt (certificado)"