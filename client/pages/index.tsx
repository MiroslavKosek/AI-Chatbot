import Head from "next/head";
import { useState } from "react";
import router from "next/router";
import Chatbot from "../components/aiChatPopup";

export default function Home() {
  const [isChatOpen, setIsChatOpen] = useState(false);

  const toggleChat = () => {
    setIsChatOpen(!isChatOpen); 
  };

  return (
    <div>
      {isChatOpen && (
        <div
          className="relative top-0 left-0  flex justify-end items-end"
          onClick={toggleChat}
        >
          <div
            className="relative w-[400px] h-[600px] border rounded-2xl shadow-lg bg-white"
            onClick={(e) => e.stopPropagation()} 
          >
            <button
              onClick={toggleChat}
              className="absolute top-2 right-2 text-red-600 text-xl"
            >
              ✖
            </button>

            <Chatbot />
          </div>
        </div>
      )}
    </div>
  );
}
