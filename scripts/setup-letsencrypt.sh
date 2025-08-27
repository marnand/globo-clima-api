#!/bin/bash
set -e

echo "Configurando Let's Encrypt para globoclima.dedyn.io..."

# Instalar certbot se não estiver instalado
sudo apt update
sudo apt install -y certbot python3-certbot-nginx

# Parar nginx temporariamente
sudo docker-compose stop nginx

# Gerar certificado Let's Encrypt
sudo certbot certonly --standalone -d globoclima.dedyn.io --email seu-email@exemplo.com --agree-tos --non-interactive

# Copiar certificados para o diretório ssl
sudo cp /etc/letsencrypt/live/globoclima.dedyn.io/fullchain.pem ssl/server.crt
sudo cp /etc/letsencrypt/live/globoclima.dedyn.io/privkey.pem ssl/server.key
sudo chown $USER:$USER ssl/server.*

# Reiniciar nginx
sudo docker-compose start nginx

echo "Certificado Let's Encrypt configurado com sucesso!"
echo "Lembre-se de configurar renovação automática com: sudo crontab -e"
echo "Adicione: 0 12 * * * /usr/bin/certbot renew --quiet"