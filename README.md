# AI-Chatbot

## How to run it?

### CPU only

```shell
docker compose up -d
```

### Nvidia GPU
Install the [NVIDIA Container Toolkit](https://docs.nvidia.com/datacenter/cloud-native/container-toolkit/latest/install-guide.html#installation).

#### Install with Apt
1.  Configure the repository

    ```shell
    curl -fsSL https://nvidia.github.io/libnvidia-container/gpgkey \
        | sudo gpg --dearmor -o /usr/share/keyrings/nvidia-container-toolkit-keyring.gpg
    curl -s -L https://nvidia.github.io/libnvidia-container/stable/deb/nvidia-container-toolkit.list \
        | sed 's#deb https://#deb [signed-by=/usr/share/keyrings/nvidia-container-toolkit-keyring.gpg] https://#g' \
        | sudo tee /etc/apt/sources.list.d/nvidia-container-toolkit.list
    sudo apt-get update
    ```

2.  Install the NVIDIA Container Toolkit packages

    ```shell
    sudo apt-get install -y nvidia-container-toolkit
    ```

#### Install with Yum or Dnf
1.  Configure the repository

    ```shell
    curl -s -L https://nvidia.github.io/libnvidia-container/stable/rpm/nvidia-container-toolkit.repo \
        | sudo tee /etc/yum.repos.d/nvidia-container-toolkit.repo
    ```

2. Install the NVIDIA Container Toolkit packages

    ```shell
    sudo yum install -y nvidia-container-toolkit
    ```

#### Configure Docker to use Nvidia driver

```shell
sudo nvidia-ctk runtime configure --runtime=docker
sudo systemctl restart docker
```

#### Start the containers

```shell
docker compose -f docker-compose-nvidia-gpu.yaml up -d
```

### AMD GPU

```shell
docker compose -f docker-compose-amd-gpu.yaml up -d
```

## How it works?

![Connection diagram of the app](./docs/images/connection_diagram.svg)
<img src="./docs/images/connection_diagram.svg">

### AI

#### BGE-M3

BGE-M3 model is used for embedding scraped data, that are then added to Qdrant database. BGE-M3 is also used for embedding user's prompts, which are then used for querying database entries, that are used in prompts with role 'system'.

#### Phi-4

Phi-4 model is used for generating relevant answers for user's prompts. It draws informations from prompts with role 'system', which are selected from Qdrant database.

### Web Scrapers

...

### Qdrant database

...

### API

...

### Front-end

...
