import { IRankingResult } from "../types/RankingResult";
import { ISearchRequest } from "../types/SearchRequest";
import { ISupportBrowser } from "../types/SupportBrowser";
import APIService from "./base";

export default class SearchService {
  static async search(request: ISearchRequest): Promise<IRankingResult | IRankingResult[]> {
  try {
    const query = `?keywords=${request.keywords}&url=${request.url}&provider=${request.provider}`;
    const response = await APIService.get(`/search${query}`);
     console.log('Raw API response:', response);
    // Add defensive check
    if (!response || !response) {
      console.error('API returned empty response');
      return [];
    }
    
    return response;
  } catch (error) {
    console.error('Search error:', error);
    throw error;
  }
}

  static async getSupportBrowsers(): Promise<ISupportBrowser[]> {
    return APIService.get("/get-support-browsers");
  }
}
