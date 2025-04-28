import Image from 'next/image';

type HeaderProps = {
  onToggleChat: () => void;
};

export default function Header({ onToggleChat }: HeaderProps) {
  return (
    <header className="w-full text-white text-sm font-light font-sans">
      {/* Horní lišta */}
      <div className="bg-black flex justify-between items-center px-8 py-4 flex-wrap">
        {/* Logo + text */}
        <div className="flex gap-4 items-center">
          <Image
            src="/liberec_logoo.png"
            alt="Liberec logo"
            width={265}
            height={45}
            className="object-contain"
          />
          <div className="leading-snug text-left">
            <p className="text-sm">oficiální stránky</p>
            <p className="text-sm">statutárního města Liberec</p>
          </div>
        </div>

        {/* Navigace */}
        <nav className="flex gap-3 items-center flex-wrap mt-2">
          <a href="#" className="px-4 py-2 bg-gray-300 text-black font-semibold rounded-sm shadow-sm">OBČAN</a>
          <a href="#" className="px-4 py-2 bg-red-600 font-semibold rounded-sm shadow-sm">RADNICE</a>
          <a href="#" className="px-4 py-2 bg-red-600 font-semibold rounded-sm shadow-sm">PORTÁL OBČANA</a>

          {/* Language + search */}
          <div className="flex items-center gap-2 ml-4">
            <div className="flex items-center bg-gray-300 text-black px-2 py-1 rounded-sm shadow-sm">
              <img
                src="https://flagcdn.com/w40/cz.png"
                alt="cz"
                className="w-5 h-4 mr-2"
              />
              <span>Česky</span>
            </div>

            <div className="flex items-center bg-white text-black rounded-sm overflow-hidden shadow-sm">
              <input
                type="text"
                placeholder="hledaný výraz"
                className="bg-white text-sm px-2 py-1 focus:outline-none w-40"
              />
              <button className="bg-red-600 text-white px-3 py-1 hover:bg-red-700">
                🔍
              </button>
            </div>
          </div>
        </nav>
      </div>

      {/* Spodní navigace */}
      <div className="bg-black flex justify-center items-center px-8 py-3 border-t border-[#1a1a1a] text-white text-base  flex-wrap gap-x-6 gap-y-2">
        {[
          "ÚŘAD",
          "POTŘEBUJI VYŘÍDIT",
          "POTŘEBUJI POMOC",
          "ÚŘAD ON-LINE",
          "FONDY A GRANTY",
          "SPOLEČNOSTI MĚSTA",
          "KONTAKTY",
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
        Chat s AI
      </button>
      </div>
      
    </header>
  );
}