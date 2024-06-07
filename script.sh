#!/bin/bash
sudo docker build . -f Dockerfile --no-cache -t registry.centurionx.net/interplanewrangler
sudo docker push registry.centurionx.net/interplanewrangler
ssh 10.0.0.104 "docker pull registry.centurionx.net/interplanewrangler; docker stop interplanewrangler; docker rm interplanewrangler; docker run --name interplanewrangler --restart=unless-stopped --net inter_network -m=500m -d registry.centurionx.net/interplanewrangler; docker exec cat appsettings.json;"
