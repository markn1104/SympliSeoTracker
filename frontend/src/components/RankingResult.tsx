import React from "react";
import { IRankingResult } from "../types/RankingResult";

interface RankingResultProps {
  data: IRankingResult | IRankingResult[]; 
}

const RankingResult: React.FC<RankingResultProps> = ({ data }) => {
  // Ensure `data` is always an array
  const results = Array.isArray(data) ? data : [data];

  // Check if results array is empty or contains only empty objects
  if (results.length === 0 || results.every(item => !item || Object.keys(item).length === 0)) {
    return <div className="mt-4 text-gray-500">No results found</div>;
  }

  return (
    <div className="container mx-auto p-4 mt-6">
      <h2 className="text-xl font-semibold mb-3">Search Results</h2>
      <table className="min-w-full table-auto border-collapse border border-gray-200">
        <thead>
          <tr>
            <th className="py-2 px-4 border-b text-left font-medium text-gray-600">
              Browser Name
            </th>
            <th className="py-2 px-4 border-b text-left font-medium text-gray-600">
              Positions
            </th>
          </tr>
        </thead>
        <tbody>
        {results.map((item, index) => (
            <tr key={index} className="hover:bg-gray-50">
              <td className="py-2 px-4 border-b">{item?.browserName || 'Unknown'}</td>
              <td className="py-2 px-4 border-b">{item?.positions || 'Not found'}</td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
};

export default RankingResult;
