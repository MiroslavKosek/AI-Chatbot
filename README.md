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

The front-end of this application is built using **Next.js**, **Tailwind CSS**, and **TypeScript**. It is a recreation a website, with the addition of an interactive chatbot (M.A.I.A.) that helps users find information.

#### Main Features:
- **AI Chatbot**: The chatbot allows users to search for information and provides answers based on user queries.
- **Responsive Design**: The layout is optimized for both desktop and mobile devices using Tailwind CSS for quick styling.
- **State Management**: React hooks like **`useState`** and **`useEffect`** are used for state management and component lifecycle handling.
- **Dynamic Interaction**: The chatbot processes user inputs and displays both user messages and bot replies.

#### Key Components:
1. **Chatbot (`client/components/aiChatPopup/index.tsx`)**:
   - Manages the chat history and processes user inputs.
   - Sends messages to the API and displays responses from the backend (AI model).
   - Utilizes a unique GUID for maintaining the session for individual users.
   - Displays a greeting message upon opening the chat window.

2. **Header (`client/components/header/index.tsx`)**:
   - Displays the navigation items.
   - Includes a button to open the chatbot, providing access to M.A.I.A.

3. **Layout (`client/components/layout/index.tsx`)**:
   - Provides the overall structure for the webpage, wrapping the header, main content, and footer.
   - Includes logic to show the chatbot popup window when activated.

4. **HTTP Client (`client/functions/httpClient.tsx`)**:
   - Contains functions (`get`, `post`, `put`, `del`) for communicating with the backend API to send and receive data.

5. **Home Page (`client/pages/index.tsx`)**:
   - The main page of the application, integrating the chatbot and acting as the landing page.

#### How to Run the Front-end:

1. Install dependencies:

   ```shell
    npm install
   ```

2. Run the development server:

   ```shell
    npm run dev
   ```

Open your browser and visit http://localhost:3000 to view the application in action.


Technologies Used:

- Next.js: A React-based framework with built-in support for server-side rendering and static site generation.

- Tailwind CSS: A utility-first CSS framework for creating custom designs.

- Axios: A promise-based HTTP client for easy API communication.

- TypeScript: A superset of JavaScript that adds static types for better code quality and maintainability.

- React: While Next.js is used as the framework, it’s built on top of React for building user interfaces with components.

Design Notes:

- The chatbot is implemented as a popup element that can be opened or closed by the user. It uses state management hooks to ensure the proper flow of messages.

- The chatbot sends user inputs to the backend via API calls and displays the responses in the chat window.

- Tailwind CSS ensures the application is highly customizable and responsive across various screen sizes.
