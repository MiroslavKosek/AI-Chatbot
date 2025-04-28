import { PropsWithChildren, useState } from "react";
import { IconContext } from "react-icons";
import Footer from "../footer";
import Header from "../header";
import Chatbot from "../../aiChatPopup"; 

export default function Layout({ children }: PropsWithChildren) {
  const [isChatOpen, setIsChatOpen] = useState(false);

  const toggleChat = () => setIsChatOpen((prev) => !prev);

  return (
    <div>
      <IconContext.Provider value={{ size: "40", color: "black" }}>
        <div className="web relative">
          <Header onToggleChat={toggleChat} />
          {isChatOpen && (
            <div
              className="relative flex justify-end items-end "
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
          <div className="children min-h-screen">{children}</div>

          <Footer />
        </div>
      </IconContext.Provider>
    </div>
  );
}
