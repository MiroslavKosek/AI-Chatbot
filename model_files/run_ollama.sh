#!/bin/bash

echo "Starting Ollama server..."
ollama serve &

sleep 5

echo "Ollama is ready, creating the model..."

ollama create ai_chatbot -f model_files/Modelfile
ollama pull bge-m3

sleep 5

ollama run ai_chatbot
