import { useState, useEffect } from "react";
import Image from "next/image";
import { post } from "../../functions/httpClient";

// GUID generator
function generateGuid() {
  return "xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx".replace(/[xy]/g, function (c) {
    const r = (Math.random() * 16) | 0,
      v = c === "x" ? r : (r & 0x3) | 0x8;
    return v.toString(16);
  });
}

export default function Chatbot() {
  const [userInput, setUserInput] = useState("");
  const [chatHistory, setChatHistory] = useState<{ sender: string; text: string }[]>([]);
  const [guidForSession, setGuidForSession] = useState("");
  const [isLoading, setIsLoading] = useState(false);
  const [hasGreeted, setHasGreeted] = useState(false); // Zajišťuje, že úvodní zpráva se pošle pouze jednou

  // Funkce pro odeslání zprávy uživatele a následnou odpověď chatbota
  const handleSubmit = async () => {
    if (!userInput.trim()) return;
    const guid = guidForSession;
    console.log(guid);
    const message = userInput;

    setChatHistory((prev) => [...prev, { sender: "user", text: message }]);
    setUserInput("");
    setIsLoading(true);

    try {
      // Pošli data na tvoje API
      console.log({ Message: message, guidId: guid })
      const response = await post("/send", { Message: message, guidId: guid });
      console.log(response.data.message.content);

      const reply = response.data?.message.content || "Chyba: prázdná odpověď.";

      setChatHistory((prev) => [...prev, { sender: "bot", text: reply }]);
    } catch (error) {
      console.error("Chyba při komunikaci s API:", error);
      setChatHistory((prev) => [
        ...prev,
        { sender: "bot", text: "Chyba při načítání odpovědi." },
      ]);
    }

    setIsLoading(false);
  };

  // Načti úvodní zprávu po jedné sekundě
  useEffect(() => {
    if (!hasGreeted) {
      const sessionGuid = generateGuid();
      setGuidForSession(sessionGuid);
  
      const timer = setTimeout(() => {
        setChatHistory((prev) => [
          ...prev,
          { sender: "bot", text: "Dobrý den, já jsem M.A.I.A.! Jak Vám mohu pomoci? 🐾🐾" },
        ]);
        setHasGreeted(true);
      }, 1000);
  
      return () => clearTimeout(timer);
    }
  }, [hasGreeted]);

  // Funkce pro postupné odesílání zpráv s intervalem
  const sendMessagesSequentially = async (messages: string[]) => {
    for (let i = 0; i < messages.length; i++) {
      await new Promise<void>((resolve) => {
        setTimeout(async () => {
          const message = messages[i];
          setChatHistory((prev) => [...prev, { sender: "bot", text: message }]);
          resolve();
        }, i * 1000); // Mezi každou zprávou je 1 sekunda pauza
      });
    }
  };

  useEffect(() => {
    sessionStorage.removeItem("chatHistory");
  }, [])

  return (
    <div className="w-[400px] h-[600px] border rounded-2xl shadow-md overflow-hidden flex flex-col font-sans">
      {/* Header */}
      <div className="bg-red-600 text-white flex items-center  px-4 ">
        <Image src="/maia-nase-milovana.png" alt="Liberec logo" width={80} height={80} />
        <div>
          <p className="font-[1000]">Statutární město Liberec</p>
          <p className="font-semibold">M.A.I.A. AI Chatbot 1.0</p>
        </div>
      </div>

      {/* Chat area */}
      <div className="flex-1 p-4 bg-white overflow-y-auto space-y-2">
        {chatHistory.map((msg, idx) => (
          <div
            key={idx}
            className={`max-w-[80%] px-4 py-2 text-sm rounded-3xl shadow-md ${
              msg.sender === "user"
                ? "ml-auto border-2 border-red-500 text-red-600"
                : "bg-red-600 text-white"
            }`}
          >
            {msg.text}
          </div>
        ))}
        {isLoading && (
          <div className="text-sm text-gray-500">M.A.I.A. přemýšlí...</div>
        )}
      </div>

      {/* Input */}
      <div className="p-3 border-t bg-white">
        <div className="flex items-center gap-2 bg-gray-100 rounded-full px-4 py-2">
          <span className="text-gray-500 text-lg font-medium">Tt</span>
          <input
            type="text"
            placeholder="Vyhledej informace"
            value={userInput}
            onChange={(e) => setUserInput(e.target.value)}
            onKeyDown={(e) => e.key === "Enter" && handleSubmit()}
            className="flex-1 bg-transparent outline-none text-sm"
          />
          <button onClick={handleSubmit} className="text-gray-500 text-lg">🎤</button>
        </div>
      </div>
    </div>
  );
}
