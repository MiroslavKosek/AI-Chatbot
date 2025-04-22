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

## How does it work?

![Connection diagram of the app](./docs/images/connection_diagram.svg)

### AI

#### BGE-M3

The BGE-M3 model is used for embedding scraped data, which is then added to the Qdrant database. BGE-M3 is also used for embedding user prompts, which are then used to query database entries, which are used in prompts with the role 'system'.

#### Phi-4

The Phi-4 model is used to generate relevant answers to user prompts. It draws information from prompts with the role 'system', which are selected from the Qdrant database.

### Web Scrapers

Web Scrapers are used to scrape relevant data from websites. The data are then used for AI knowledge.

### Qdrant database

The Qdrant database is used for storing embedded scraped data. The API queries the database to find relevant information for a given user prompt.

### API

API sits between the front-end, the Qdrant database, and AIs. The front-end sends a user prompt to the API, which uses the BGE-M3 model to embed the prompt. The API then queries the database to find relevant information and sends it to the Phi-4 model. The generated answer is then sent to the front-end.

### Front-end

...
