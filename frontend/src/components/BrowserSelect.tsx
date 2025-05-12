import React from "react";

interface BrowserSelectProps {
  value: number;
  onChange: (browserType: number) => void;
}

// Hardcoded browser options
const supportBrowsers = [
  { browserType: 0, browserName: "Google" },
  { browserType: 1, browserName: "Bing" }
];

const BrowserSelect: React.FC<BrowserSelectProps> = ({ value, onChange }) => {
  return (
    <div>
      <label
        htmlFor="browser"
        className="block text-sm font-medium text-gray-700"
      >
        Choose Browser
      </label>
      <select
        id="browser"
        value={value}
        onChange={(e) => onChange(Number(e.target.value))}
        className="mt-1 w-full px-4 py-2 border rounded-lg focus:outline-none focus:ring focus:border-blue-300"
        required
      >
        {supportBrowsers.map((browserOption) => (
          <option
            key={browserOption.browserType}
            value={browserOption.browserType}
          >
            {browserOption.browserName}
          </option>
        ))}
      </select>
    </div>
  );
};

export default BrowserSelect;
