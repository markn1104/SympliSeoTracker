import React, { ReactNode } from "react";

interface MainLayoutProps {
  children: ReactNode;
}

const MainLayout: React.FC<MainLayoutProps> = ({ children }) => {
  return (
    <div className="min-h-screen bg-gray-100 flex flex-col items-center p-6">
      <header className="w-full max-w-4xl mb-6">
        <h1 className="text-2xl font-bold text-center mb-4">
          Search Application
        </h1>
      </header>
      <main className="w-full max-w-4xl bg-white shadow-lg rounded-lg p-6">
        {children}
      </main>
    </div>
  );
};

export default MainLayout;
