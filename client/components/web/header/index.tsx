type HeaderProps = {
  onToggleChat: () => void;
};

export default function Header({ onToggleChat }: HeaderProps) {
  return (
    <header className="w-full text-white text-sm font-light font-sans">
      {/* Top Bar */}
      <div className="bg-black flex justify-between items-center px-8 py-4 flex-wrap">
        {/* Logo + text */}
        <div className="flex gap-4 items-center">
        </div>

        {/* NavBar */}
        <nav className="flex gap-3 items-center flex-wrap mt-2">
          <a href="#" className="px-4 py-2 bg-gray-300 text-black font-semibold rounded-sm shadow-sm">CITIZEN</a>
          <a href="#" className="px-4 py-2 bg-red-600 font-semibold rounded-sm shadow-sm">CITY HALL</a>
          <a href="#" className="px-4 py-2 bg-red-600 font-semibold rounded-sm shadow-sm">CITIZEN PORTAL</a>

          {/* Language + search */}
          <div className="flex items-center gap-2 ml-4">
            <div className="flex items-center bg-gray-300 text-black px-2 py-1 rounded-sm shadow-sm">
              <img
                src="https://flagcdn.com/w40/gb.png"
                alt="English flag"
                className="w-5 h-4 mr-2"
              />
              <span>English</span>
            </div>

            <div className="flex items-center bg-white text-black rounded-sm overflow-hidden shadow-sm">
              <input
                type="text"
                placeholder="search term"
                className="bg-white text-sm px-2 py-1 focus:outline-none w-40"
              />
              <button className="bg-red-600 text-white px-3 py-1 hover:bg-red-700">
                🔍
              </button>
            </div>
          </div>
        </nav>
      </div>

      {/* Bottom NavBar */}
      <div className="bg-black flex justify-center items-center px-8 py-3 border-t border-[#1a1a1a] text-white text-base  flex-wrap gap-x-6 gap-y-2">
        {[
          "OFFICE",
          "I NEED TO TAKE CARE OF",
          "I NEED HELP",
          "OFFICE ONLINE",
          "FUNDS AND GRANTS",
          "CITY COMPANIES",
          "CONTACTS",
        ].map((item) => (
          <a
            key={item}
            href="#"
            className="hover:underline  "
          >
            {item}
          </a>
        ))}
        <button
        onClick={onToggleChat}
        className="bg-red-600 font-extrabold px-4 py-2 rounded-full text-white shadow-md"
      >
        Chat with AI
      </button>
      </div>
      
    </header>
  );
}