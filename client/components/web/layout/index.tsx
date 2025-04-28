import { PropsWithChildren, useState } from "react";
import { IconContext } from "react-icons";
import Footer from "../footer";
import Header from "../header";
import Chatbot from "../../aiChatPopup"; 
import Head from "next/head";

export default function Layout({ children }: PropsWithChildren) {
  const [isChatOpen, setIsChatOpen] = useState(false);

  const toggleChat = () => setIsChatOpen((prev) => !prev);

  return (
    <div>
      <Head>
        <title>M.A.I.A. AI Chatbot</title>
        <meta name="description" content="M.A.I.A. AI Chatbot" />
        <link rel="icon" href="/favicon.ico" />
      </Head>
      <IconContext.Provider value={{ size: "40", color: "black" }}>
        <div className="web relative">
          <Header onToggleChat={toggleChat} />
          {isChatOpen && (
            <div
              className="relative flex justify-end items-end "
              onClick={toggleChat}
            >
              <div
                className="relative w-[400px] h-[600px] border rounded-2xl shadow-lg bg-white mr-3"
                onClick={(e) => e.stopPropagation()}
              >
                <button
                  onClick={toggleChat}
                  className="absolute top-2 right-2 text-white text-xl"
                >
                  ✖
                </button>
                <Chatbot />
              </div>
            </div>
          )}
          <div className="children">{children}</div>

          <Footer />
        </div>
      </IconContext.Provider>
    </div>
  );
}
